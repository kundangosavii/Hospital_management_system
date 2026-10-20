using System.Diagnostics;
using System.Text.Json;
using Hospital_management_system.Extensions;
using Hospital_management_system.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Hospital_management_system.Controllers;

public class HomeController : Controller
{
    private const string DoctorCacheKey = "doctors";
    private const string PatientNameCookie = "patientName";
    private readonly IMemoryCache _cache;
    private readonly IHttpClientFactory _httpClientFactory;

    public HomeController(IMemoryCache cache, IHttpClientFactory httpClientFactory)
    {
        _cache = cache;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var doctors = GetDoctors();
        var posts = await GetPostsAsync();

        var viewModel = new HomeViewModel
        {
            Doctors = doctors,
            Posts = posts,
            PatientName = Request.Cookies[PatientNameCookie] ?? string.Empty,
            SelectedSpecialization = HttpContext.Session.GetString("specialization") ?? string.Empty
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SavePreferences(string patientName, string specialization)
    {
        if (string.IsNullOrWhiteSpace(patientName))
        {
            ModelState.AddModelError(nameof(patientName), "Patient name is required.");
            return RedirectToAction(nameof(Index));
        }

        Response.Cookies.Append(PatientNameCookie, patientName.Trim(), new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax
        });
        HttpContext.Session.SetString("specialization", specialization ?? string.Empty);

        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
    public IActionResult Details(int doctorId)
    {
        var doctor = GetDoctors().FirstOrDefault(d => d.DoctorId == doctorId);
        if (doctor is null)
        {
            return NotFound();
        }

        ViewData["PatientName"] = Request.Cookies[PatientNameCookie] ?? string.Empty;
        ViewData["Specialization"] = HttpContext.Session.GetString("specialization") ?? doctor.Specialization;
        return View(doctor);
    }

    public IActionResult Appointment(int doctorId)
    {
        var doctor = GetDoctors().FirstOrDefault(d => d.DoctorId == doctorId);
        if (doctor is null)
        {
            return NotFound();
        }

        ViewData["DoctorName"] = doctor.DoctorName;
        ViewData["PatientName"] = Request.Cookies[PatientNameCookie] ?? string.Empty;
        return View(new AppointmentViewModel { DoctorId = doctor.DoctorId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult BookAppointment(AppointmentViewModel appointment)
    {
        var doctor = GetDoctors().FirstOrDefault(d => d.DoctorId == appointment.DoctorId);
        if (doctor is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewData["DoctorName"] = doctor.DoctorName;
            ViewData["PatientName"] = appointment.PatientName;
            return View("Appointment", appointment);
        }

        Response.Cookies.Append(PatientNameCookie, appointment.PatientName.Trim(), new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax
        });

        TempData["AppointmentMessage"] =
            $"Appointment requested with {doctor.DoctorName} for {appointment.AppointmentDate:dd MMM yyyy}.";
        return RedirectToAction(nameof(Details), new { doctorId = doctor.DoctorId });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private List<Doctor> GetDoctors()
    {
        return _cache.GetOrCreate(DoctorCacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3);
            return new List<Doctor>
            {
                new() { DoctorId = 1, DoctorName = "Dr. Ananya Sharma", Specialization = "Cardiology", Experience = 12, ConsultationFee = 1200m },
                new() { DoctorId = 2, DoctorName = "Dr. Rohan Mehta", Specialization = "Neurology", Experience = 9, ConsultationFee = 1000m },
                new() { DoctorId = 3, DoctorName = "Dr. Priya Nair", Specialization = "Pediatrics", Experience = 7, ConsultationFee = 800m }
            };
        }) ?? [];
    }

    private async Task<List<Post>> GetPostsAsync()
    {
        var client = _httpClientFactory.CreateClient();
        using var response = await client.GetAsync("https://jsonplaceholder.typicode.com/posts");
        response.EnsureSuccessStatusCode();

        await using var contentStream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<List<Post>>(
            contentStream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
    }
}
