using Clinic.Core.Models;

namespace Clinic.Core.Comparers;

/// Сортування лікарів: спочатку за спеціалізацією, потім за ПІБ.
public class DoctorBySpecializationComparer : IComparer<Doctor>
{
    public int Compare(Doctor? x, Doctor? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        int bySpecialization = x.Specialization.CompareTo(y.Specialization);
        return bySpecialization != 0 ? bySpecialization : x.CompareTo(y);
    }
}
