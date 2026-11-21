using System.Text.Json.Serialization;
using Clinic.Core.Interfaces;

namespace Clinic.Core.Models;

/// Запис в історії хвороби пацієнта: діагноз, надані послуги та їх вартість.
/// Вартість консультації фіксується на момент створення запису.
public class MedicalRecord : IEntity, IComparable<MedicalRecord>
{
    private string _diagnosis = string.Empty;
    private decimal _consultationFee;

    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PatientId { get; set; }

    public Guid DoctorId { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    public string Diagnosis
    {
        get => _diagnosis;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Діагноз не може бути порожнім.");
            _diagnosis = value.Trim();
        }
    }

    /// Вартість консультації лікаря на момент прийому.
    public decimal ConsultationFee
    {
        get => _consultationFee;
        set
        {
            if (value < 0)
                throw new ArgumentException("Вартість консультації не може бути від'ємною.");
            _consultationFee = value;
        }
    }

    /// Додаткові медичні послуги, надані під час прийому.
    public List<MedicalService> Services { get; set; } = new();

    /// Сума вартості всіх додаткових послуг.
    [JsonIgnore]
    public decimal ServicesTotal => Services.Sum(service => service.Price);

    /// Загальна вартість: консультація + послуги.
    [JsonIgnore]
    public decimal TotalCost => ConsultationFee + ServicesTotal;

    public int CompareTo(MedicalRecord? other)
    {
        if (other == null) return 1;
        return Date.CompareTo(other.Date);
    }
}
