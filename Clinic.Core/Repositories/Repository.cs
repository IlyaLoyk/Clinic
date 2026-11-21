using Clinic.Core.Exceptions;
using Clinic.Core.Interfaces;

namespace Clinic.Core.Repositories;

/// Універсальне сховище сутностей на основі узагальненої колекції.
public class Repository<T> : IRepository<T> where T : class, IEntity
{
    private readonly List<T> _items = new();

    public int Count => _items.Count;

    public void Add(T item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));
        if (_items.Exists(existing => existing.Id == item.Id))
            throw new ClinicException($"Елемент з ідентифікатором {item.Id} вже існує.");
        _items.Add(item);
    }

    public bool Remove(T item) => _items.Remove(item);

    public T? GetById(Guid id) => _items.Find(item => item.Id == id);

    public List<T> GetAll() => new List<T>(_items);

    public List<T> GetAll(IComparer<T> comparer)
    {
        var sorted = new List<T>(_items);
        sorted.Sort(comparer);
        return sorted;
    }

    public List<T> Find(Predicate<T> match) => _items.FindAll(match);

    public void ReplaceAll(IEnumerable<T> items)
    {
        _items.Clear();
        _items.AddRange(items);
    }
}
