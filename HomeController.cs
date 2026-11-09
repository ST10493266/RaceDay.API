using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly RaceDayClientService _client;

        public HomeController(RaceDayClientService client)
        {
            _client = client;
        }

        public async Task<IActionResult> Index(string searchString, string eventType)
        {
            var collectedRaces = await _client.FetchActiveEventsAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                collectedRaces = collectedRaces.Where(r => r.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) || 
                                                           r.Location.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(eventType))
            {
                collectedRaces = collectedRaces.Where(r => r.EventType.Equals(eventType, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return View(collectedRaces);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto credentials)
        {
            if (!ModelState.IsValid) return View(credentials);

            var verificationOutput = await _client.ExecuteLoginAsync(credentials);
            if (verificationOutput != null)
            {
                HttpContext.Session.SetString("AuthTokenKey", verificationOutput.Token);
                HttpContext.Session.SetString("AssignedUserRole", verificationOutput.Role);
                HttpContext.Session.SetString("LoggedUserEmail", verificationOutput.Email);

                if (verificationOutput.Role.Equals("Organiser", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("Dashboard", "Organiser");
                }
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Authentication failed. Check credentials against endpoint proxy.");
            return View(credentials);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
