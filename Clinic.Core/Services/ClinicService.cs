using Clinic.Core.Comparers;
using Clinic.Core.Exceptions;
using Clinic.Core.Interfaces;
using Clinic.Core.Models;
using Clinic.Core.Repositories;

namespace Clinic.Core.Services;

/// Уся бізнес-логіка клініки: лікарі, пацієнти, запис на прийом,
/// історія хвороб, підрахунок вартості та статистика.
public class ClinicService
{
    private readonly IRepository<Doctor> _doctors = new Repository<Doctor>();
    private readonly IRepository<Patient> _patients = new Repository<Patient>();
    private readonly IRepository<Appointment> _appointments = new Repository<Appointment>();
    private readonly IRepository<MedicalRecord> _records = new Repository<MedicalRecord>();

    private readonly IStorageService<Doctor> _doctorStorage;
    private readonly IStorageService<Patient> _patientStorage;
    private readonly IStorageService<Appointment> _appointmentStorage;
    private readonly IStorageService<MedicalRecord> _recordStorage;

    public ClinicService(string dataDirectory)
    {
        _doctorStorage = new JsonFileService<Doctor>(Path.Combine(dataDirectory, "doctors.json"));
        _patientStorage = new JsonFileService<Patient>(Path.Combine(dataDirectory, "patients.json"));
        _appointmentStorage = new JsonFileService<Appointment>(Path.Combine(dataDirectory, "appointments.json"));
        _recordStorage = new JsonFileService<MedicalRecord>(Path.Combine(dataDirectory, "records.json"));
    }

    // ===================== Збереження / завантаження =====================

    /// Завантажує всі дані з файлів (викликається при старті програми).
    public void LoadAll()
    {
        _doctors.ReplaceAll(_doctorStorage.Load());
        _patients.ReplaceAll(_patientStorage.Load());
        _appointments.ReplaceAll(_appointmentStorage.Load());
        _records.ReplaceAll(_recordStorage.Load());
    }

    /// Зберігає всі дані у файли (викликається при виході).
    public void SaveAll()
    {
        _doctorStorage.Save(_doctors.GetAll());
        _patientStorage.Save(_patients.GetAll());
        _appointmentStorage.Save(_appointments.GetAll());
        _recordStorage.Save(_records.GetAll());
    }

    // ============================== Лікарі ===============================

    public void AddDoctor(Doctor doctor) => _doctors.Add(doctor);

    public Doctor GetDoctor(Guid id)
        => _doctors.GetById(id) ?? throw new EntityNotFoundException("Лікар", id);

    /// Усі лікарі, відсортовані за спеціалізацією та ПІБ.
    public List<Doctor> GetAllDoctors()
        => _doctors.GetAll(new DoctorBySpecializationComparer());

    public List<Doctor> FindDoctorsBySpecialization(Specialization specialization)
        => _doctors.Find(d => d.Specialization == specialization);

    public List<Doctor> FindDoctorsByName(string text)
        => _doctors.Find(d => d.FullName.Contains(text.Trim(), StringComparison.CurrentCultureIgnoreCase));

    public void RemoveDoctor(Guid id)
    {
        Doctor doctor = GetDoctor(id);
        bool hasHistory = _appointments.Find(a => a.DoctorId == id).Count > 0
                          || _records.Find(r => r.DoctorId == id).Count > 0;
        if (hasHistory)
            throw new ClinicException("Неможливо видалити лікаря: для нього існують прийоми або медичні записи.");
        _doctors.Remove(doctor);
    }

    // ============================= Пацієнти ==============================

    public void AddPatient(Patient patient) => _patients.Add(patient);

    public Patient GetPatient(Guid id)
        => _patients.GetById(id) ?? throw new EntityNotFoundException("Пацієнт", id);

    /// Усі пацієнти за алфавітом.
    public List<Patient> GetAllPatients()
        => _patients.GetAll(new PatientByNameComparer());

    public List<Patient> FindPatientsByName(string text)
        => _patients.Find(p => p.FullName.Contains(text.Trim(), StringComparison.CurrentCultureIgnoreCase));

    public void RemovePatient(Guid id)
    {
        Patient patient = GetPatient(id);
        bool hasHistory = _appointments.Find(a => a.PatientId == id).Count > 0
                          || _records.Find(r => r.PatientId == id).Count > 0;
        if (hasHistory)
            throw new ClinicException("Неможливо видалити пацієнта: для нього існують прийоми або медичні записи.");
        _patients.Remove(patient);
    }

    // ============================== Прийоми ==============================

    /// Записує пацієнта на прийом. Перевіряє: чи існують лікар і пацієнт, що час у майбутньому,
    /// що прийом у межах робочих годин лікаря та що немає перетину з іншими прийомами.
    /// <exception cref="DoctorScheduleConflictException">Час зайнятий або лікар не працює.</exception>
    public Appointment BookAppointment(Guid doctorId, Guid patientId, DateTime start,
                                       int durationMinutes, string complaint = "")
    {
        Doctor doctor = GetDoctor(doctorId);
        GetPatient(patientId);

        if (start <= DateTime.Now)
            throw new ClinicException("Не можна записати на минулий час.");

        var appointment = new Appointment
        {
            DoctorId = doctorId,
            PatientId = patientId,
            Start = start,
            DurationMinutes = durationMinutes,
            Complaint = complaint
        };

        if (!doctor.IsWorkingAt(appointment.Start, appointment.End))
            throw new DoctorScheduleConflictException(
                $"Лікар {doctor.FullName} не працює у цей час (робочі години: {doctor.WorkingHours}).");

        Appointment? doctorConflict = _appointments.Find(a =>
                a.DoctorId == doctorId
                && a.Status == AppointmentStatus.Scheduled
                && a.OverlapsWith(appointment.Start, appointment.End))
            .FirstOrDefault();
        if (doctorConflict != null)
            throw new DoctorScheduleConflictException(
                "Цей час уже зайнятий іншим пацієнтом.", doctorConflict.Start, doctorConflict.End);

        Appointment? patientConflict = _appointments.Find(a =>
                a.PatientId == patientId
                && a.Status == AppointmentStatus.Scheduled
                && a.OverlapsWith(appointment.Start, appointment.End))
            .FirstOrDefault();
        if (patientConflict != null)
            throw new ClinicException("Пацієнт уже має інший прийом у цей час.");

        _appointments.Add(appointment);
        return appointment;
    }

    public Appointment GetAppointment(Guid id)
        => _appointments.GetById(id) ?? throw new EntityNotFoundException("Прийом", id);

    public void CancelAppointment(Guid id)
    {
        Appointment appointment = GetAppointment(id);
        if (appointment.Status != AppointmentStatus.Scheduled)
            throw new ClinicException("Скасувати можна лише запланований прийом.");
        appointment.Status = AppointmentStatus.Cancelled;
    }

    /// Завершує прийом і створює запис в історії хвороби.
    public MedicalRecord CompleteAppointment(Guid id, string diagnosis, IEnumerable<MedicalService> services)
    {
        Appointment appointment = GetAppointment(id);
        if (appointment.Status != AppointmentStatus.Scheduled)
            throw new ClinicException("Завершити можна лише запланований прийом.");

        MedicalRecord record = AddMedicalRecord(
            appointment.PatientId, appointment.DoctorId, diagnosis, services, appointment.Start);
        appointment.Status = AppointmentStatus.Completed;
        return record;
    }

    /// Розклад лікаря на день (без скасованих прийомів), за часом.
    public List<Appointment> GetDoctorSchedule(Guid doctorId, DateTime date)
        => _appointments.GetAll(new AppointmentByDateComparer())
            .Where(a => a.DoctorId == doctorId
                        && a.Start.Date == date.Date
                        && a.Status != AppointmentStatus.Cancelled)
            .ToList();

    /// Усі прийоми клініки на вказану дату.
    public List<Appointment> GetAppointmentsByDate(DateTime date)
        => _appointments.GetAll(new AppointmentByDateComparer())
            .Where(a => a.Start.Date == date.Date)
            .ToList();

    /// Усі прийоми пацієнта (будь-який статус), за часом.
    public List<Appointment> GetPatientAppointments(Guid patientId)
        => _appointments.GetAll(new AppointmentByDateComparer())
            .Where(a => a.PatientId == patientId)
            .ToList();

    // ========================= Історія хвороб ============================

    /// Додає запис в історію хвороби. Вартість консультації береться з профілю лікаря.
    public MedicalRecord AddMedicalRecord(Guid patientId, Guid doctorId, string diagnosis,
                                          IEnumerable<MedicalService> services, DateTime? date = null)
    {
        Doctor doctor = GetDoctor(doctorId);
        GetPatient(patientId);

        var record = new MedicalRecord
        {
            PatientId = patientId,
            DoctorId = doctorId,
            Date = date ?? DateTime.Now,
            Diagnosis = diagnosis,
            ConsultationFee = doctor.ConsultationPrice,
            Services = services.ToList()
        };
        _records.Add(record);
        return record;
    }

    /// Історія хвороби пацієнта від найстарішого запису до найновішого.
    public List<MedicalRecord> GetPatientHistory(Guid patientId)
    {
        GetPatient(patientId);
        List<MedicalRecord> history = _records.Find(r => r.PatientId == patientId);
        history.Sort(); // використовує IComparable<MedicalRecord> (за датою)
        return history;
    }

    // ===================== Вартість та статистика ========================

    /// Загальна вартість усіх наданих пацієнту послуг (консультації + послуги).
    public decimal CalculatePatientTotalCost(Guid patientId)
        => GetPatientHistory(patientId).Sum(r => r.TotalCost);

    /// Завантаженість лікаря за день у відсотках від його робочого часу.
    public double GetDoctorWorkload(Guid doctorId, DateTime date)
    {
        Doctor doctor = GetDoctor(doctorId);
        if (doctor.WorkingMinutes <= 0)
            return 0;

        double bookedMinutes = GetDoctorSchedule(doctorId, date).Sum(a => a.DurationMinutes);
        return Math.Round(bookedMinutes / doctor.WorkingMinutes * 100, 1);
    }

    /// Виручка клініки за період (включно) за медичними записами.
    public decimal GetRevenue(DateTime from, DateTime to)
    {
        if (to.Date < from.Date)
            throw new ClinicException("Кінець періоду не може бути раніше за початок.");

        return _records
            .Find(r => r.Date.Date >= from.Date && r.Date.Date <= to.Date)
            .Sum(r => r.TotalCost);
    }

    public int DoctorsCount => _doctors.Count;

    public int PatientsCount => _patients.Count;
}
