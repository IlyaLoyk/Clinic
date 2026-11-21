# Medical Clinic

Два проекти в одному solution:

- `Clinic.Core` - Class Library (DLL): моделі, інтерфейси, репозиторій, компаратори, виключення, бізнес-логіка, JSON-сервіс.
- `Clinic.UI` - Console App: меню та введення/виведення. Посилається на `Clinic.Core`.

## Запуск

Потрібен .NET 8 SDK.

    dotnet run --project Clinic.UI

Або відкрийте `Clinic.sln` у Visual Studio 2022 і запустіть проект `Clinic.UI`.
Якщо .sln не відкривається, створіть свій:

    dotnet new sln -n Clinic
    dotnet sln add Clinic.Core/Clinic.Core.csproj Clinic.UI/Clinic.UI.csproj

Дані зберігаються у JSON-файлах у папці `Data` поруч із exe (bin/Debug/net8.0/Data).
Пункт меню 5 заповнює програму тестовими даними.

1. Діаграма моделей предметної області

```mermaid
classDiagram
    class IEntity {
        <<interface>>
        +int Id
    }
    class Person {
        <<abstract>>
        +int Id
        +string FullName
        +string Phone
        +Gender Gender
    }
    class Patient
    class Doctor {
        +Specialization Specialization
    }
    class Appointment {
        +int Id
        +int PatientId
        +int DoctorId
        +int ServiceId
        +DateTime DateTime
        +AppointmentStatus Status
    }
    class MedicalService {
        +int Id
        +string Name
        +decimal Price
    }
    class MedicalRecord {
        +int Id
        +int AppointmentId
        +string Diagnosis
        +string Treatment
    }

    IEntity <|.. Person
    Person <|-- Patient
    Person <|-- Doctor
    IEntity <|.. Appointment
    IEntity <|.. MedicalService
    IEntity <|.. MedicalRecord
```

2. Діаграма сервісів та сховища

```mermaid
classDiagram
    class IRepository~T~ {
        <<interface>>
        +Add(T item)
        +Remove(int id)
        +GetById(int id) T
        +GetAll() List~T~
        +Find(Predicate~T~ match) List~T~
    }
    class Repository~T~ {
        -List~T~ _items
    }
    class IStorageService {
        <<interface>>
        +Save(...)
        +Load(...)
    }
    class JsonFileService
    class ClinicService {
        +RegisterPatient(...)
        +RegisterDoctor(...)
        +ScheduleAppointment(...)
        +CompleteAppointment(...)
        +CreateMedicalRecord(...)
    }

    IRepository~T~ <|.. Repository~T~
    IStorageService <|.. JsonFileService
    ClinicService --> Repository~T~ : використовує
    ClinicService --> IStorageService : зберігає дані
```

3. Ієрархія власних класів виключень

```mermaid
classDiagram
    class Exception
    class ClinicException {
        <<abstract>>
    }
    class DoctorScheduleConflictException
    class EntityNotFoundException
    class InvalidPatientDataException
    class InvalidDoctorDataException
    class DataStorageException

    Exception <|-- ClinicException
    ClinicException <|-- DoctorScheduleConflictException
    ClinicException <|-- EntityNotFoundException
    ClinicException <|-- InvalidPatientDataException
    ClinicException <|-- InvalidDoctorDataException
    ClinicException <|-- DataStorageException
```
