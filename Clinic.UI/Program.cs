using System.Text;
using Clinic.Core.Exceptions;
using Clinic.Core.Services;
using Clinic.UI;
using Clinic.UI.Menus;

// UTF-8, щоб коректно відображалась кирилиця
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

// Файли JSON лежать у папці Data поруч із виконуваним файлом
string dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");
var service = new ClinicService(dataDirectory);

// Завантаження даних при старті
try
{
    service.LoadAll();
    InputHelper.PrintInfo($"Дані завантажено з '{dataDirectory}'.");
}
catch (DataStorageException ex)
{
    InputHelper.PrintError(ex.Message);
    InputHelper.PrintInfo("Програма продовжить роботу з порожніми даними.");
}

MainMenu.Run(service);

// Збереження даних при виході
try
{
    service.SaveAll();
    InputHelper.PrintSuccess("Дані збережено. До побачення!");
}
catch (DataStorageException ex)
{
    InputHelper.PrintError(ex.Message);
}
