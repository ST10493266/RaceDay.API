using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Filters;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{
    [SessionGuard("Participant")]
    public class ParticipantController : Controller
    {
        private readonly RaceDayClientService _client;

        public ParticipantController(RaceDayClientService client)
        {
            _client = client;
        }

        public async Task<IActionResult> MyRaces()
        {
            var items = await _client.FetchUserEnrolmentsAsync();
            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> Enroll(int eventId, string categoryName)
        {
            var outcome = await _client.SubmitEnrolmentRequestAsync(eventId, categoryName);
            if (outcome) return RedirectToAction("MyRaces");

            return RedirectToAction("Index", "Home", new { error = "Unable to process enrolment." });
        }
    }
}
