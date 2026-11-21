namespace Clinic.Core.Exceptions;

/// Виникає, коли сутність із заданим ідентифікатором не знайдена.
public class EntityNotFoundException : ClinicException
{
    public Guid EntityId { get; }

    public EntityNotFoundException(string entityName, Guid id)
        : base($"{entityName} з ідентифікатором {id} не знайдено.")
    {
        EntityId = id;
    }
}
