using Clinic.Core.Models;
using Clinic.Core.Services;

namespace Clinic.UI.Menus;

/// Меню запису на прийом та роботи з розкладом.
public static class AppointmentMenu
{
    public static void Run(ClinicService service)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== ПРИЙОМИ ===");
            Console.WriteLine("1. Записати пацієнта на прийом");
            Console.WriteLine("2. Розклад лікаря на дату");
            Console.WriteLine("3. Усі прийоми на дату");
            Console.WriteLine("4. Прийоми пацієнта");
            Console.WriteLine("5. Скасувати прийом");
            Console.WriteLine("6. Завершити прийом (запис в історію хвороби)");
            Console.WriteLine("0. Назад");

            switch (InputHelper.ReadInt("Ваш вибір: ", 0, 6))
            {
                case 1: InputHelper.Execute(() => Book(service)); break;
                case 2: InputHelper.Execute(() => DoctorSchedule(service)); break;
                case 3: InputHelper.Execute(() => ByDate(service)); break;
                case 4: InputHelper.Execute(() => ByPatient(service)); break;
                case 5: InputHelper.Execute(() => Cancel(service)); break;
                case 6: InputHelper.Execute(() => Complete(service)); break;
                case 0: return;
            }
        }
    }

    private static void Book(ClinicService service)
    {
        Patient? patient = EntityInput.ChoosePatient(service);
        if (patient == null) return;

        Doctor? doctor = EntityInput.ChooseDoctor(service);
        if (doctor == null) return;

        DateTime start = InputHelper.ReadDateTime("Дата і час прийому (дд.мм.рррр гг:хх): ");
        int duration = InputHelper.ReadInt("Тривалість, хв (5-240): ", 5, 240);
        string complaint = InputHelper.ReadOptional("Скарги (необов'язково): ");

        // Якщо час зайнятий - сервіс кине DoctorScheduleConflictException,
        // яку перехопить InputHelper.Execute і покаже повідомлення.
        Appointment appointment = service.BookAppointment(doctor.Id, patient.Id, start, duration, complaint);
        InputHelper.PrintSuccess($"Запис створено: {appointment.Start:dd.MM.yyyy HH:mm} - {appointment.End:HH:mm}, " +
                                 $"лікар {doctor.FullName}.");
    }

    private static void DoctorSchedule(ClinicService service)
    {
        Doctor? doctor = EntityInput.ChooseDoctor(service);
        if (doctor == null) return;

        DateTime date = InputHelper.ReadDate("Дата (дд.мм.рррр): ");
        Printer.Appointments(service, service.GetDoctorSchedule(doctor.Id, date));
        Console.WriteLine($"Завантаженість лікаря: {service.GetDoctorWorkload(doctor.Id, date)}%");
    }

    private static void ByDate(ClinicService service)
    {
        DateTime date = InputHelper.ReadDate("Дата (дд.мм.рррр): ");
        Printer.Appointments(service, service.GetAppointmentsByDate(date));
    }

    private static void ByPatient(ClinicService service)
    {
        Patient? patient = EntityInput.ChoosePatient(service);
        if (patient == null) return;
        Printer.Appointments(service, service.GetPatientAppointments(patient.Id));
    }

    private static void Cancel(ClinicService service)
    {
        Appointment? appointment = EntityInput.ChooseScheduledAppointment(service);
        if (appointment == null) return;

        service.CancelAppointment(appointment.Id);
        InputHelper.PrintSuccess("Прийом скасовано.");
    }

    private static void Complete(ClinicService service)
    {
        Appointment? appointment = EntityInput.ChooseScheduledAppointment(service);
        if (appointment == null) return;

        string diagnosis = InputHelper.ReadString("Діагноз: ");
        List<MedicalService> services = EntityInput.ReadServices();

        MedicalRecord record = service.CompleteAppointment(appointment.Id, diagnosis, services);
        InputHelper.PrintSuccess($"Прийом завершено. Вартість: {record.TotalCost:F2} грн.");
    }
}
