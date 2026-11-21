using System.Text.Json.Serialization;
using Clinic.Core.Interfaces;

namespace Clinic.Core.Models;

/// Прийом пацієнта у лікаря. Зберігає лише Id лікаря та пацієнта
public class Appointment : IEntity, IComparable<Appointment>
{
    private int _durationMinutes = 30;
    private string _complaint = string.Empty;

    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid DoctorId { get; set; }

    public Guid PatientId { get; set; }

    public DateTime Start { get; set; }

    /// Тривалість прийому, хвилин (5-240).
    public int DurationMinutes
    {
        get => _durationMinutes;
        set
        {
            if (value < 5 || value > 240)
                throw new ArgumentException("Тривалість прийому має бути від 5 до 240 хвилин.");
            _durationMinutes = value;
        }
    }

    /// Скарги пацієнта (необов'язково).
    public string Complaint
    {
        get => _complaint;
        set => _complaint = (value ?? string.Empty).Trim();
    }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    /// Час завершення прийому (обчислюється).
    [JsonIgnore]
    public DateTime End => Start.AddMinutes(DurationMinutes);

    /// Чи перетинається прийом із заданим часовим інтервалом.
    public bool OverlapsWith(DateTime otherStart, DateTime otherEnd)
        => Start < otherEnd && otherStart < End;

    public int CompareTo(Appointment? other)
    {
        if (other == null) return 1;
        return Start.CompareTo(other.Start);
    }
}
