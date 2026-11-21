using Clinic.Core.Services;

namespace Clinic.UI.Menus;

/// Головне меню програми.
public static class MainMenu
{
    public static void Run(ClinicService service)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("======= МЕДИЧНА КЛІНІКА =======");
            Console.WriteLine($"Лікарів: {service.DoctorsCount}, пацієнтів: {service.PatientsCount}");
            Console.WriteLine("1. Лікарі");
            Console.WriteLine("2. Пацієнти");
            Console.WriteLine("3. Прийоми");
            Console.WriteLine("4. Історія хвороб та фінанси");
            Console.WriteLine("5. Заповнити тестовими даними");
            Console.WriteLine("0. Вихід (зі збереженням)");

            switch (InputHelper.ReadInt("Ваш вибір: ", 0, 5))
            {
                case 1: DoctorMenu.Run(service); break;
                case 2: PatientMenu.Run(service); break;
                case 3: AppointmentMenu.Run(service); break;
                case 4: RecordMenu.Run(service); break;
                case 5: InputHelper.Execute(() => DemoData.Fill(service)); break;
                case 0: return;
            }
        }
    }
}
