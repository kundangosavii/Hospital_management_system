namespace Hospital_management_system.Models;

// Contains all data displayed on the home page.
public class HomeViewModel
{
    public IReadOnlyList<Doctor> Doctors { get; set; } = [];
    public IReadOnlyList<Post> Posts { get; set; } = [];
    public string PatientName { get; set; } = string.Empty;
    public string SelectedSpecialization { get; set; } = string.Empty;
}
