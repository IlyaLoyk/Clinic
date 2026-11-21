using Clinic.Core.Models;
using Clinic.Core.Services;

namespace Clinic.UI;

/// Вибір сутностей зі списків (за номером) та введення послуг.
public static class EntityInput
{
    public static Doctor? ChooseDoctor(ClinicService service, List<Doctor>? list = null)
    {
        list ??= service.GetAllDoctors();
        if (list.Count == 0)
        {
            InputHelper.PrintInfo("Лікарів немає. Спочатку додайте лікаря.");
            return null;
        }

        Printer.Doctors(list);
        int number = InputHelper.ReadInt("Оберіть номер лікаря (0 - скасувати): ", 0, list.Count);
        return number == 0 ? null : list[number - 1];
    }

    public static Patient? ChoosePatient(ClinicService service, List<Patient>? list = null)
    {
        list ??= service.GetAllPatients();
        if (list.Count == 0)
        {
            InputHelper.PrintInfo("Пацієнтів немає. Спочатку додайте пацієнта.");
            return null;
        }

        Printer.Patients(list);
        int number = InputHelper.ReadInt("Оберіть номер пацієнта (0 - скасувати): ", 0, list.Count);
        return number == 0 ? null : list[number - 1];
    }

    public static Appointment? ChooseAppointment(ClinicService service, List<Appointment> list)
    {
        if (list.Count == 0)
        {
            InputHelper.PrintInfo("Запланованих прийомів немає.");
            return null;
        }

        Printer.Appointments(service, list);
        int number = InputHelper.ReadInt("Оберіть номер прийому (0 - скасувати): ", 0, list.Count);
        return number == 0 ? null : list[number - 1];
    }

    /// Вибирає пацієнта, а потім один із його запланованих прийомів.
    public static Appointment? ChooseScheduledAppointment(ClinicService service)
    {
        Patient? patient = ChoosePatient(service);
        if (patient == null) return null;

        List<Appointment> scheduled = service.GetPatientAppointments(patient.Id)
            .Where(a => a.Status == AppointmentStatus.Scheduled)
            .ToList();
        return ChooseAppointment(service, scheduled);
    }

    /// Вводить список додаткових медичних послуг (порожня назва завершує введення).
    public static List<MedicalService> ReadServices()
    {
        var services = new List<MedicalService>();
        Console.WriteLine("Додаткові послуги (залиште назву порожньою, щоб завершити):");

        while (true)
        {
            string name = InputHelper.ReadOptional("  Назва послуги: ");
            if (name.Length == 0)
                break;

            var medicalService = new MedicalService();
            try
            {
                medicalService.Name = name;
            }
            catch (ArgumentException ex)
            {
                InputHelper.PrintError(ex.Message);
                continue;
            }

            InputHelper.Assign("  Ціна (грн): ", s => medicalService.Price = InputHelper.ParseDecimal(s));
            services.Add(medicalService);
        }

        return services;
    }
}
