using System.Text.Json.Serialization;
using Clinic.Core.Exceptions;

namespace Clinic.Core.Models;

/// Лікар клініки: спеціалізація, вартість консультації, робочі години.
public class Doctor : Person
{
    private decimal _consultationPrice = 500m;
    private TimeSpan _workDayStart = TimeSpan.FromHours(9);
    private TimeSpan _workDayEnd = TimeSpan.FromHours(18);

    public Specialization Specialization { get; set; } = Specialization.Therapist;

    /// Вартість однієї консультації в гривнях (більше 0).
    public decimal ConsultationPrice
    {
        get => _consultationPrice;
        set
        {
            if (value <= 0 || value > 100000)
                throw CreateValidationError(nameof(ConsultationPrice), "Вартість консультації має бути в межах від 0 до 100000 грн.");
            _consultationPrice = value;
        }
    }

    /// Початок робочого дня (00:00 - 23:59).
    public TimeSpan WorkDayStart
    {
        get => _workDayStart;
        set
        {
            CheckTimeOfDay(value, nameof(WorkDayStart));
            _workDayStart = value;
        }
    }

    /// Кінець робочого дня (00:00 - 23:59).
    public TimeSpan WorkDayEnd
    {
        get => _workDayEnd;
        set
        {
            CheckTimeOfDay(value, nameof(WorkDayEnd));
            _workDayEnd = value;
        }
    }

    /// Робочі години у вигляді рядка "09:00-18:00".
    [JsonIgnore]
    public string WorkingHours =>
        WorkDayStart.ToString(@"hh\:mm") + "-" + WorkDayEnd.ToString(@"hh\:mm");

    /// Тривалість робочого дня у хвилинах.
    [JsonIgnore]
    public double WorkingMinutes => (WorkDayEnd - WorkDayStart).TotalMinutes;

    /// Одночасно встановлює обидві межі робочого дня та перевіряє, що початок раніше кінця.
    public void SetWorkingHours(TimeSpan start, TimeSpan end)
    {
        CheckTimeOfDay(start, nameof(WorkDayStart));
        CheckTimeOfDay(end, nameof(WorkDayEnd));
        if (start >= end)
            throw CreateValidationError(nameof(WorkDayStart), "Початок робочого дня має бути раніше за його кінець.");
        _workDayStart = start;
        _workDayEnd = end;
    }

    /// Перевіряє, що інтервал прийому повністю лежить у межах одного робочого дня лікаря.
    public bool IsWorkingAt(DateTime start, DateTime end)
    {
        return start.Date == end.Date
               && start.TimeOfDay >= WorkDayStart
               && end.TimeOfDay <= WorkDayEnd;
    }

    private void CheckTimeOfDay(TimeSpan value, string fieldName)
    {
        if (value < TimeSpan.Zero || value >= TimeSpan.FromHours(24))
            throw CreateValidationError(fieldName, "Час має бути в межах від 00:00 до 23:59.");
    }

    protected override ClinicException CreateValidationError(string fieldName, string message)
        => new InvalidDoctorDataException(fieldName, message);
}
