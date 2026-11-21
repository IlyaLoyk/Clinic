namespace Clinic.Core.Exceptions;

/// Виникає при помилках збереження або завантаження даних з файлу.
public class DataStorageException : ClinicException
{
    public DataStorageException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
