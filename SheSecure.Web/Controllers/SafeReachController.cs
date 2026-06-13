using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace SheSecure.Web.Controllers
{
    public class SafeReachController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SafeReachController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient GetClient()
        {
            var client = _httpClientFactory.CreateClient("SafetyService");
            var token = HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        // GET — show all check-ins + form
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Token") == null)
                return RedirectToAction("Login", "Auth");

            var client = GetClient();
            var response = await client.GetStringAsync("api/SafeReach/all");
            var records = JsonDocument.Parse(response).RootElement;

            ViewBag.Records = records;
            ViewData["Title"] = "Safe Check-In";
            return View();
        }

        // POST — create a new check-in
        [HttpPost]
        public async Task<IActionResult> Create(string expectedArrivalTime)
        {
            var client = GetClient();
            var employeeId = HttpContext.Session.GetString("UserId") ?? "1";

            var payload = JsonSerializer.Serialize(new
            {
                employeeId = int.TryParse(employeeId, out var eid) ? eid : 1,
                expectedArrivalTime = DateTime.Parse(expectedArrivalTime).ToString("o")
            });

            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/SafeReach/create", content);

            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Safe check-in created successfully!";
            else
                TempData["Error"] = "Failed to create check-in. Please try again.";

            return RedirectToAction("Index");
        }

        // POST — acknowledge arrival
        [HttpPost]
        public async Task<IActionResult> Acknowledge(int safeReachId)
        {
            var client = GetClient();
            var payload = JsonSerializer.Serialize(new { safeReachId });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await client.PutAsync("api/SafeReach/acknowledge", content);

            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Arrival acknowledged successfully!";
            else
                TempData["Error"] = "Failed to acknowledge. Already acknowledged?";

            return RedirectToAction("Index");
        }

        // POST — escalate
        [HttpPost]
        public async Task<IActionResult> Escalate(int id)
        {
            var client = GetClient();
            var response = await client.PutAsync($"api/SafeReach/escalate/{id}", null);

            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Escalated successfully!";
            else
                TempData["Error"] = "Cannot escalate — employee may have already acknowledged.";

            return RedirectToAction("Index");
        }
    }
}