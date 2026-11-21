namespace Clinic.Core.Models;

/// Медична послуга (аналіз, процедура, УЗД тощо) з ціною.
public class MedicalService
{
    private string _name = string.Empty;
    private decimal _price;

    public MedicalService()
    {
    }

    public MedicalService(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Назва послуги не може бути порожньою.");
            _name = value.Trim();
        }
    }

    public decimal Price
    {
        get => _price;
        set
        {
            if (value <= 0 || value > 1000000)
                throw new ArgumentException("Ціна послуги має бути більшою за 0.");
            _price = value;
        }
    }

    public override string ToString() => $"{Name} - {Price:F2} грн";
}
