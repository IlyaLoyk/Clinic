using Clinic.Core.Models;

namespace Clinic.UI;

/// Українські назви для значень enum (для відображення в консолі).
public static class EnumNames
{
    public static string Get(object value) => value switch
    {
        Specialization.Therapist => "Терапевт",
        Specialization.Cardiologist => "Кардіолог",
        Specialization.Neurologist => "Невролог",
        Specialization.Surgeon => "Хірург",
        Specialization.Pediatrician => "Педіатр",
        Specialization.Dentist => "Стоматолог",
        Specialization.Ophthalmologist => "Офтальмолог",
        Gender.Male => "Чоловіча",
        Gender.Female => "Жіноча",
        AppointmentStatus.Scheduled => "Запланований",
        AppointmentStatus.Completed => "Завершений",
        AppointmentStatus.Cancelled => "Скасований",
        _ => value.ToString() ?? string.Empty
    };
}
