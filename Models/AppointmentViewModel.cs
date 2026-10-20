using System.ComponentModel.DataAnnotations;

namespace Hospital_management_system.Models;

// Contains the values submitted through the appointment form.
public class AppointmentViewModel
{
    [Required]
    public int DoctorId { get; set; }

    [Required]
    [StringLength(100)]
    public string PatientName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime AppointmentDate { get; set; } = DateTime.Today;
}
