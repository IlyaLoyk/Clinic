using Clinic.Core.Models;
using Clinic.Core.Services;

namespace Clinic.UI;

/// Форматований вивід списків сутностей у вигляді таблиць.
public static class Printer
{
    public static void Doctors(List<Doctor> doctors)
    {
        if (doctors.Count == 0)
        {
            InputHelper.PrintInfo("Список лікарів порожній.");
            return;
        }

        Console.WriteLine($"{"№",-3} {"ПІБ",-30} {"Спеціальність",-14} {"Ціна, грн",10} {"Години",-12} {"Телефон"}");
        Console.WriteLine(new string('-', 90));
        for (int i = 0; i < doctors.Count; i++)
        {
            Doctor d = doctors[i];
            Console.WriteLine($"{i + 1,-3} {d.FullName,-30} {EnumNames.Get(d.Specialization),-14} " +
                              $"{d.ConsultationPrice,10:F2} {d.WorkingHours,-12} {d.Phone}");
        }
    }

    public static void Patients(List<Patient> patients)
    {
        if (patients.Count == 0)
        {
            InputHelper.PrintInfo("Список пацієнтів порожній.");
            return;
        }

        Console.WriteLine($"{"№",-3} {"ПІБ",-30} {"Вік",4} {"Стать",-9} {"Телефон"}");
        Console.WriteLine(new string('-', 75));
        for (int i = 0; i < patients.Count; i++)
        {
            Patient p = patients[i];
            Console.WriteLine($"{i + 1,-3} {p.FullName,-30} {p.Age,4} {EnumNames.Get(p.Gender),-9} {p.Phone}");
        }
    }

    public static void Appointments(ClinicService service, List<Appointment> appointments)
    {
        if (appointments.Count == 0)
        {
            InputHelper.PrintInfo("Прийомів не знайдено.");
            return;
        }

        Console.WriteLine($"{"№",-3} {"Дата і час",-17} {"Хв",4} {"Лікар",-26} {"Пацієнт",-26} {"Статус"}");
        Console.WriteLine(new string('-', 100));
        for (int i = 0; i < appointments.Count; i++)
        {
            Appointment a = appointments[i];
            string doctor = service.GetDoctor(a.DoctorId).FullName;
            string patient = service.GetPatient(a.PatientId).FullName;
            Console.WriteLine($"{i + 1,-3} {a.Start,-17:dd.MM.yyyy HH:mm} {a.DurationMinutes,4} " +
                              $"{doctor,-26} {patient,-26} {EnumNames.Get(a.Status)}");
        }
    }

    public static void Records(ClinicService service, List<MedicalRecord> records)
    {
        if (records.Count == 0)
        {
            InputHelper.PrintInfo("Медичних записів немає.");
            return;
        }

        for (int i = 0; i < records.Count; i++)
        {
            MedicalRecord r = records[i];
            Doctor doctor = service.GetDoctor(r.DoctorId);
            Console.WriteLine($"#{i + 1}  {r.Date:dd.MM.yyyy HH:mm}  Лікар: {doctor.FullName} ({EnumNames.Get(doctor.Specialization)})");
            Console.WriteLine($"    Діагноз: {r.Diagnosis}");
            Console.WriteLine($"    Консультація: {r.ConsultationFee:F2} грн");
            foreach (MedicalService s in r.Services)
                Console.WriteLine($"    - {s}");
            Console.WriteLine($"    Разом: {r.TotalCost:F2} грн");
            Console.WriteLine();
        }
    }
}
