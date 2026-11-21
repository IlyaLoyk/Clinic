namespace Clinic.Core.Interfaces;

/// Сутність предметної області з унікальним ідентифікатором.
public interface IEntity
{
    Guid Id { get; }
}
