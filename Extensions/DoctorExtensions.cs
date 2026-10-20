using Hospital_management_system.Models;

namespace Hospital_management_system.Extensions;

public static class DoctorExtensions
{
    public static decimal CalculateTotalCharge(this Doctor doctor)
    {
        return doctor.ConsultationFee + 100m;
    }
}
