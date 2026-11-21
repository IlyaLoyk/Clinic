using Clinic.Core.Models;

namespace Clinic.Core.Comparers;

///Сортування прийомів за датою і часом початку (від найближчих)
public class AppointmentByDateComparer : IComparer<Appointment>
{
    public int Compare(Appointment? x, Appointment? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        return x.CompareTo(y);
    }
}
