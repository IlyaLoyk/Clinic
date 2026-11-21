namespace Clinic.Core.Exceptions;

/// Виникає при некоректних даних лікаря (ПІБ, телефон, ціна, робочі години).
public class InvalidDoctorDataException : ClinicException
{
    /// Назва поля, в якому знайдено помилку.
    public string FieldName { get; }

    public InvalidDoctorDataException(string fieldName, string message) : base(message)
    {
        FieldName = fieldName;
    }
}
