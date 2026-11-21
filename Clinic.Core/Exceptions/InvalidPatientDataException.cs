namespace Clinic.Core.Exceptions;

/// Виникає при некоректних даних пацієнта (ПІБ, телефон, дата народження).
public class InvalidPatientDataException : ClinicException
{
    /// Назва поля, в якому знайдено помилку.
    public string FieldName { get; }

    public InvalidPatientDataException(string fieldName, string message) : base(message)
    {
        FieldName = fieldName;
    }
}
