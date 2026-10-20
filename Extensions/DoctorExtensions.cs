using Hospital_management_system.Models;

namespace Hospital_management_system.Extensions;

public static class DoctorExtensions
{
    // Adds the fixed service charge to a doctor's consultation fee.
    public static decimal CalculateTotalCharge(this Doctor doctor)
    {
        return doctor.ConsultationFee + 100m;
    }
}
