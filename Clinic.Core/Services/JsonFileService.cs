using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Clinic.Core.Exceptions;
using Clinic.Core.Interfaces;

namespace Clinic.Core.Services;

/// Збереження та завантаження списку об'єктів у JSON-файл
/// за допомогою серіалізації System.Text.Json.
public class JsonFileService<T> : IStorageService<T>
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        // Щоб кирилиця записувалась читабельно
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        // Enum записуються словами
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly string _filePath;

    public JsonFileService(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Шлях до файлу не може бути порожнім.");
        _filePath = filePath;
    }

    public void Save(List<T> data)
    {
        try
        {
            string? directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            string json = JsonSerializer.Serialize(data, Options);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            throw new DataStorageException($"Не вдалося зберегти файл '{_filePath}': {ex.Message}", ex);
        }
    }

    public List<T> Load()
    {
        if (!File.Exists(_filePath))
            return new List<T>();

        try
        {
            string json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<T>>(json, Options) ?? new List<T>();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException
                                       or JsonException or ClinicException or ArgumentException)
        {
            throw new DataStorageException($"Не вдалося прочитати файл '{_filePath}': {ex.Message}", ex);
        }
    }
}
