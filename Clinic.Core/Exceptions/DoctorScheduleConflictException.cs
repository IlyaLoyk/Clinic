namespace Clinic.Core.Exceptions;

/// Виникає, коли час прийому вже зайнятий іншим пацієнтом
/// або виходить за межі робочих годин лікаря.
public class DoctorScheduleConflictException : ClinicException
{
    /// Початок зайнятого проміжку (якщо конфлікт із іншим прийомом).
    public DateTime? ConflictStart { get; }

    /// Кінець зайнятого проміжку (якщо конфлікт із іншим прийомом).
    public DateTime? ConflictEnd { get; }

    public DoctorScheduleConflictException(string message) : base(message)
    {
    }

    public DoctorScheduleConflictException(string message, DateTime conflictStart, DateTime conflictEnd)
        : base($"{message} (зайнятий проміжок: {conflictStart:dd.MM.yyyy HH:mm} - {conflictEnd:HH:mm})")
    {
        ConflictStart = conflictStart;
        ConflictEnd = conflictEnd;
    }
}
