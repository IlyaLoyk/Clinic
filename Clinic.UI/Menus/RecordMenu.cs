using Clinic.Core.Models;
using Clinic.Core.Services;

namespace Clinic.UI.Menus;

/// Меню історії хвороб, вартості послуг та статистики.
public static class RecordMenu
{
    public static void Run(ClinicService service)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== ІСТОРІЯ ХВОРОБ ТА ФІНАНСИ ===");
            Console.WriteLine("1. Додати запис в історію хвороби");
            Console.WriteLine("2. Переглянути історію хвороби пацієнта");
            Console.WriteLine("3. Загальна вартість послуг пацієнта");
            Console.WriteLine("4. Завантаженість лікаря за день");
            Console.WriteLine("5. Виручка клініки за період");
            Console.WriteLine("0. Назад");

            switch (InputHelper.ReadInt("Ваш вибір: ", 0, 5))
            {
                case 1: InputHelper.Execute(() => AddRecord(service)); break;
                case 2: InputHelper.Execute(() => ShowHistory(service)); break;
                case 3: InputHelper.Execute(() => ShowTotalCost(service)); break;
                case 4: InputHelper.Execute(() => ShowWorkload(service)); break;
                case 5: InputHelper.Execute(() => ShowRevenue(service)); break;
                case 0: return;
            }
        }
    }

    private static void AddRecord(ClinicService service)
    {
        Patient? patient = EntityInput.ChoosePatient(service);
        if (patient == null) return;

        Doctor? doctor = EntityInput.ChooseDoctor(service);
        if (doctor == null) return;

        string diagnosis = InputHelper.ReadString("Діагноз: ");
        List<MedicalService> services = EntityInput.ReadServices();

        MedicalRecord record = service.AddMedicalRecord(patient.Id, doctor.Id, diagnosis, services);
        InputHelper.PrintSuccess($"Запис додано. Вартість: {record.TotalCost:F2} грн.");
    }

    private static void ShowHistory(ClinicService service)
    {
        Patient? patient = EntityInput.ChoosePatient(service);
        if (patient == null) return;

        Console.WriteLine($"Історія хвороби: {patient.FullName}");
        Printer.Records(service, service.GetPatientHistory(patient.Id));
    }

    private static void ShowTotalCost(ClinicService service)
    {
        Patient? patient = EntityInput.ChoosePatient(service);
        if (patient == null) return;

        decimal total = service.CalculatePatientTotalCost(patient.Id);
        Console.WriteLine($"Загальна вартість послуг пацієнта {patient.FullName}: {total:F2} грн");
    }

    private static void ShowWorkload(ClinicService service)
    {
        Doctor? doctor = EntityInput.ChooseDoctor(service);
        if (doctor == null) return;

        DateTime date = InputHelper.ReadDate("Дата (дд.мм.рррр): ");
        Console.WriteLine($"Завантаженість {doctor.FullName} на {date:dd.MM.yyyy}: {service.GetDoctorWorkload(doctor.Id, date)}%");
    }

    private static void ShowRevenue(ClinicService service)
    {
        DateTime from = InputHelper.ReadDate("Початок періоду (дд.мм.рррр): ");
        DateTime to = InputHelper.ReadDate("Кінець періоду (дд.мм.рррр): ");
        Console.WriteLine($"Виручка за {from:dd.MM.yyyy} - {to:dd.MM.yyyy}: {service.GetRevenue(from, to):F2} грн");
    }
}
