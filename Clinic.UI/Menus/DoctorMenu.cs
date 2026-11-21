using Clinic.Core.Exceptions;
using Clinic.Core.Models;
using Clinic.Core.Services;

namespace Clinic.UI.Menus;

/// Меню роботи з лікарями.
public static class DoctorMenu
{
    public static void Run(ClinicService service)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== ЛІКАРІ ===");
            Console.WriteLine("1. Додати лікаря");
            Console.WriteLine("2. Список лікарів (за спеціальністю)");
            Console.WriteLine("3. Пошук за спеціальністю");
            Console.WriteLine("4. Пошук за ПІБ");
            Console.WriteLine("5. Редагувати лікаря");
            Console.WriteLine("6. Видалити лікаря");
            Console.WriteLine("0. Назад");

            switch (InputHelper.ReadInt("Ваш вибір: ", 0, 6))
            {
                case 1: InputHelper.Execute(() => Add(service)); break;
                case 2: Printer.Doctors(service.GetAllDoctors()); break;
                case 3: InputHelper.Execute(() => SearchBySpecialization(service)); break;
                case 4: InputHelper.Execute(() => SearchByName(service)); break;
                case 5: InputHelper.Execute(() => Edit(service)); break;
                case 6: InputHelper.Execute(() => Remove(service)); break;
                case 0: return;
            }
        }
    }

    private static void Add(ClinicService service)
    {
        var doctor = new Doctor();
        InputHelper.Assign("ПІБ лікаря: ", s => doctor.FullName = s);
        InputHelper.Assign("Телефон: ", s => doctor.Phone = s);
        InputHelper.Assign("Дата народження (дд.мм.рррр): ", s => doctor.BirthDate = InputHelper.ParseDate(s));
        doctor.Specialization = InputHelper.ReadEnum<Specialization>("Спеціальність");
        InputHelper.Assign("Вартість консультації (грн): ", s => doctor.ConsultationPrice = InputHelper.ParseDecimal(s));
        ReadWorkingHours(doctor);

        service.AddDoctor(doctor);
        InputHelper.PrintSuccess($"Лікаря {doctor.FullName} додано.");
    }

    private static void ReadWorkingHours(Doctor doctor)
    {
        while (true)
        {
            TimeSpan start = TimeSpan.Zero;
            TimeSpan end = TimeSpan.Zero;
            InputHelper.Assign("Початок робочого дня (гг:хх): ", s => start = InputHelper.ParseTime(s));
            InputHelper.Assign("Кінець робочого дня (гг:хх): ", s => end = InputHelper.ParseTime(s));

            try
            {
                doctor.SetWorkingHours(start, end);
                return;
            }
            catch (ClinicException ex)
            {
                InputHelper.PrintError(ex.Message);
            }
        }
    }

    private static void SearchBySpecialization(ClinicService service)
    {
        Specialization specialization = InputHelper.ReadEnum<Specialization>("Спеціальність");
        Printer.Doctors(service.FindDoctorsBySpecialization(specialization));
    }

    private static void SearchByName(ClinicService service)
    {
        string text = InputHelper.ReadString("Введіть частину ПІБ: ");
        Printer.Doctors(service.FindDoctorsByName(text));
    }

    private static void Edit(ClinicService service)
    {
        Doctor? doctor = EntityInput.ChooseDoctor(service);
        if (doctor == null) return;

        Console.WriteLine("Що змінити?");
        Console.WriteLine("1. Телефон");
        Console.WriteLine("2. Вартість консультації");
        Console.WriteLine("3. Робочі години");
        switch (InputHelper.ReadInt("Ваш вибір: ", 1, 3))
        {
            case 1: InputHelper.Assign("Новий телефон: ", s => doctor.Phone = s); break;
            case 2: InputHelper.Assign("Нова вартість (грн): ", s => doctor.ConsultationPrice = InputHelper.ParseDecimal(s)); break;
            case 3: ReadWorkingHours(doctor); break;
        }

        InputHelper.PrintSuccess("Дані лікаря оновлено.");
    }

    private static void Remove(ClinicService service)
    {
        Doctor? doctor = EntityInput.ChooseDoctor(service);
        if (doctor == null) return;

        if (!InputHelper.Confirm($"Видалити лікаря {doctor.FullName}?")) return;
        service.RemoveDoctor(doctor.Id);
        InputHelper.PrintSuccess("Лікаря видалено.");
    }
}
