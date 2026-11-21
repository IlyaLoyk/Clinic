using Clinic.Core.Models;

namespace Clinic.Core.Comparers;

/// Сортування пацієнтів за ПІБ в алфавітному порядку.
public class PatientByNameComparer : IComparer<Patient>
{
    public int Compare(Patient? x, Patient? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        return x.CompareTo(y);
    }
}
