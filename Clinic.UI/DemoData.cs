using Clinic.Core.Models;
using Clinic.Core.Services;

namespace Clinic.UI;

/// Тестові дані для швидкої демонстрації роботи програми.
public static class DemoData
{
    public static void Fill(ClinicService service)
    {
        if (service.DoctorsCount > 0 || service.PatientsCount > 0)
        {
            InputHelper.PrintInfo("Дані вже існують - тестові дані не додано.");
            return;
        }

        Doctor therapist = NewDoctor("Коваленко Андрій Петрович", "+380501112233",
            new DateTime(1980, 5, 14), Specialization.Therapist, 600m);
        Doctor cardiologist = NewDoctor("Шевченко Олена Іванівна", "+380502223344",
            new DateTime(1975, 9, 3), Specialization.Cardiologist, 900m);
        Doctor dentist = NewDoctor("Бондаренко Максим Олегович", "+380503334455",
            new DateTime(1988, 1, 27), Specialization.Dentist, 750m);
        service.AddDoctor(therapist);
        service.AddDoctor(cardiologist);
        service.AddDoctor(dentist);

        Patient ivan = NewPatient("Мельник Іван Сергійович", "+380671110001", new DateTime(1995, 3, 12), Gender.Male);
        Patient maria = NewPatient("Ткачук Марія Василівна", "+380671110002", new DateTime(1990, 7, 21), Gender.Female);
        Patient petro = NewPatient("Гриценко Петро Миколайович", "+380671110003", new DateTime(1962, 11, 5), Gender.Male);
        service.AddPatient(ivan);
        service.AddPatient(maria);
        service.AddPatient(petro);

        // Майбутні прийоми 
        DateTime tomorrow = DateTime.Today.AddDays(1);
        service.BookAppointment(therapist.Id, ivan.Id, tomorrow.AddHours(10), 30, "Головний біль");
        service.BookAppointment(therapist.Id, maria.Id, tomorrow.AddHours(11), 30, "Кашель");
        service.BookAppointment(cardiologist.Id, petro.Id, tomorrow.AddHours(12), 45, "Тиск");

        // Старі записи в історії хвороб
        service.AddMedicalRecord(ivan.Id, therapist.Id, "ГРВІ",
            new[] { new MedicalService("Загальний аналіз крові", 250m) }, DateTime.Today.AddDays(-30));
        service.AddMedicalRecord(petro.Id, cardiologist.Id, "Артеріальна гіпертензія",
            new[] { new MedicalService("ЕКГ", 400m), new MedicalService("УЗД серця", 1200m) },
            DateTime.Today.AddDays(-14));

        InputHelper.PrintSuccess("Тестові дані додано (3 лікарі, 3 пацієнти, 3 прийоми, 2 медичні записи).");
    }

    private static Doctor NewDoctor(string name, string phone, DateTime birth,
                                    Specialization specialization, decimal price)
    {
        return new Doctor
        {
            FullName = name,
            Phone = phone,
            BirthDate = birth,
            Specialization = specialization,
            ConsultationPrice = price
        };
    }

    private static Patient NewPatient(string name, string phone, DateTime birth, Gender gender)
    {
        return new Patient
        {
            FullName = name,
            Phone = phone,
            BirthDate = birth,
            Gender = gender
        };
    }
}
