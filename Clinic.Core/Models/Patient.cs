using Clinic.Core.Exceptions;

namespace Clinic.Core.Models;

/// Пацієнт клініки.
public class Patient : Person
{
    public Gender Gender { get; set; } = Gender.Male;

    protected override ClinicException CreateValidationError(string fieldName, string message)
        => new InvalidPatientDataException(fieldName, message);
}
