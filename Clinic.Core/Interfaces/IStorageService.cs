namespace Clinic.Core.Interfaces;

/// Сервіс збереження та завантаження списку об'єктів (наприклад, у файл).
public interface IStorageService<T>
{
    void Save(List<T> data);

    /// Завантажує дані. Якщо даних ще немає - повертає порожній список.
    List<T> Load();
}
