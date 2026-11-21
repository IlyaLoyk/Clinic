using Clinic.Core.Interfaces;

namespace Clinic.Core.Interfaces;

/// Узагальнене сховище сутностей у пам'яті.
public interface IRepository<T> where T : class, IEntity
{
    /// Кількість елементів у сховищі.
    int Count { get; }

    void Add(T item);

    bool Remove(T item);

    /// Повертає елемент за Id або null, якщо його немає.
    T? GetById(Guid id);

    /// Повертає копію списку всіх елементів.
    List<T> GetAll();

    /// Повертає відсортовану копію списку за заданим компаратором.
    List<T> GetAll(IComparer<T> comparer);

    /// Повертає елементи, що задовольняють умову.
    List<T> Find(Predicate<T> match);

    /// Повністю замінює вміст сховища (використовується після завантаження з файлу).
    void ReplaceAll(IEnumerable<T> items);
}
