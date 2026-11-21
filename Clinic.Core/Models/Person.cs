using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Clinic.Core.Exceptions;
using Clinic.Core.Interfaces;

namespace Clinic.Core.Models;

/// Абстрактна базова модель людини (лікар або пацієнт).
public abstract class Person : IEntity, IComparable<Person>
{
    private static readonly Regex PhonePattern = new(@"^\+?\d{10,13}$");

    private string _fullName = string.Empty;
    private string _phone = string.Empty;
    private DateTime _birthDate = new DateTime(2000, 1, 1);

    public Guid Id { get; set; } = Guid.NewGuid();

    /// ПІБ: щонайменше два слова, без цифр.
    public string FullName
    {
        get => _fullName;
        set
        {
            string name = (value ?? string.Empty).Trim();
            if (name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length < 2)
                throw CreateValidationError(nameof(FullName), "Вкажіть прізвище та ім'я (щонайменше два слова).");
            if (name.Any(char.IsDigit))
                throw CreateValidationError(nameof(FullName), "ПІБ не може містити цифри.");
            _fullName = name;
        }
    }

    /// Телефон: 10-13 цифр, може починатися з '+'. Пробіли, дужки та дефіси ігноруються.
    public string Phone
    {
        get => _phone;
        set
        {
            string cleaned = Regex.Replace(value ?? string.Empty, @"[\s\-()]", string.Empty);
            if (!PhonePattern.IsMatch(cleaned))
                throw CreateValidationError(nameof(Phone), "Телефон має містити від 10 до 13 цифр (наприклад, +380501234567).");
            _phone = cleaned;
        }
    }

    /// Дата народження: не в майбутньому та не раніше 1900 року.
    public DateTime BirthDate
    {
        get => _birthDate;
        set
        {
            if (value.Date > DateTime.Today || value.Year < 1900)
                throw CreateValidationError(nameof(BirthDate), "Некоректна дата народження.");
            _birthDate = value.Date;
        }
    }

    /// Вік у повних роках.
    [JsonIgnore]
    public int Age
    {
        get
        {
            int age = DateTime.Today.Year - _birthDate.Year;
            if (_birthDate.Date > DateTime.Today.AddYears(-age))
                age--;
            return age;
        }
    }

    /// Кожен нащадок створює власний тип виключення для помилок валідації.
    protected abstract ClinicException CreateValidationError(string fieldName, string message);

    /// Порівняння за ПІБ (алфавітний порядок).
    public int CompareTo(Person? other)
    {
        if (other == null) return 1;
        return string.Compare(FullName, other.FullName, StringComparison.CurrentCultureIgnoreCase);
    }

    public override string ToString() => $"{FullName} ({Phone})";
}
