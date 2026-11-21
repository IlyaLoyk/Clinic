using Clinic.Core.Models;
using Clinic.Core.Services;

namespace Clinic.UI.Menus;

/// Меню роботи з пацієнтами.
public static class PatientMenu
{
    public static void Run(ClinicService service)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== ПАЦІЄНТИ ===");
            Console.WriteLine("1. Додати пацієнта");
            Console.WriteLine("2. Список пацієнтів (за алфавітом)");
            Console.WriteLine("3. Пошук за ПІБ");
            Console.WriteLine("4. Редагувати пацієнта");
            Console.WriteLine("5. Видалити пацієнта");
            Console.WriteLine("0. Назад");

            switch (InputHelper.ReadInt("Ваш вибір: ", 0, 5))
            {
                case 1: InputHelper.Execute(() => Add(service)); break;
                case 2: Printer.Patients(service.GetAllPatients()); break;
                case 3: InputHelper.Execute(() => SearchByName(service)); break;
                case 4: InputHelper.Execute(() => Edit(service)); break;
                case 5: InputHelper.Execute(() => Remove(service)); break;
                case 0: return;
            }
        }
    }

    private static void Add(ClinicService service)
    {
        var patient = new Patient();
        InputHelper.Assign("ПІБ пацієнта: ", s => patient.FullName = s);
        InputHelper.Assign("Телефон: ", s => patient.Phone = s);
        InputHelper.Assign("Дата народження (дд.мм.рррр): ", s => patient.BirthDate = InputHelper.ParseDate(s));
        patient.Gender = InputHelper.ReadEnum<Gender>("Стать");

        service.AddPatient(patient);
        InputHelper.PrintSuccess($"Пацієнта {patient.FullName} додано.");
    }

    private static void SearchByName(ClinicService service)
    {
        string text = InputHelper.ReadString("Введіть частину ПІБ: ");
        Printer.Patients(service.FindPatientsByName(text));
    }

    private static void Edit(ClinicService service)
    {
        Patient? patient = EntityInput.ChoosePatient(service);
        if (patient == null) return;

        Console.WriteLine("Що змінити?");
        Console.WriteLine("1. ПІБ");
        Console.WriteLine("2. Телефон");
        Console.WriteLine("3. Дату народження");
        switch (InputHelper.ReadInt("Ваш вибір: ", 1, 3))
        {
            case 1: InputHelper.Assign("Новий ПІБ: ", s => patient.FullName = s); break;
            case 2: InputHelper.Assign("Новий телефон: ", s => patient.Phone = s); break;
            case 3: InputHelper.Assign("Нова дата народження (дд.мм.рррр): ", s => patient.BirthDate = InputHelper.ParseDate(s)); break;
        }

        InputHelper.PrintSuccess("Дані пацієнта оновлено.");
    }

    private static void Remove(ClinicService service)
    {
        Patient? patient = EntityInput.ChoosePatient(service);
        if (patient == null) return;

        if (!InputHelper.Confirm($"Видалити пацієнта {patient.FullName}?")) return;
        service.RemovePatient(patient.Id);
        InputHelper.PrintSuccess("Пацієнта видалено.");
    }
}
