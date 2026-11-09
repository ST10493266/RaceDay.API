using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Filters;
using RaceDay.Web.Models;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{
    [SessionGuard("Organiser")]
    public class OrganiserController : Controller
    {
        private readonly RaceDayClientService _client;

        public OrganiserController(RaceDayClientService client)
        {
            _client = client;
        }

        public async Task<IActionResult> Dashboard()
        {
            var list = await _client.FetchActiveEventsAsync();
            return View(list);
        }

        [HttpGet]
        public IActionResult CreateEvent() => View(new CreateEventFormWrapper());

        [HttpPost]
        public async Task<IActionResult> CreateEvent(CreateEventFormWrapper modelSubmission)
        {
            if (!ModelState.IsValid) return View(modelSubmission);

            var executionCheck = await _client.DispatchNewEventAsync(modelSubmission.EventDetails, modelSubmission.BannerImageFile);
            if (executionCheck) return RedirectToAction("Dashboard");

            ModelState.AddModelError("", "Unable to save record through Backend API integration pipeline.");
            return View(modelSubmission);
        }
    }
}
