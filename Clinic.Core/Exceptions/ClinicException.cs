namespace Clinic.Core.Exceptions;

/// Базовий клас для всіх виключень бізнес-логіки клініки.
/// Дозволяє в UI перехоплювати всі "очікувані" помилки одним блоком catch.
public class ClinicException : Exception
{
    public ClinicException(string message) : base(message)
    {
    }

    public ClinicException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
