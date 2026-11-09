using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RaceDay.Web.Models;

namespace RaceDay.Web.Services
{
    public class RaceDayClientService
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly JsonSerializerOptions _options;

        public RaceDayClientService(HttpClient http, IHttpContextAccessor contextAccessor)
        {
            _http = http;
            _contextAccessor = contextAccessor;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        private void InjectAuthorizationToken()
        {
            var token = _contextAccessor.HttpContext?.Session.GetString("AuthTokenKey");
            if (!string.IsNullOrEmpty(token))
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<AuthSuccessResponseDto?> ExecuteLoginAsync(LoginDto loginPayload)
        {
            var content = new StringContent(JsonSerializer.Serialize(loginPayload), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("auth/login", content);
            if (!response.IsSuccessStatusCode) return null;

            return JsonSerializer.Deserialize<AuthSuccessResponseDto>(await response.Content.ReadAsStringAsync(), _options);
        }

        public async Task<List<EventDisplayDto>> FetchActiveEventsAsync()
        {
            InjectAuthorizationToken();
            var response = await _http.GetAsync("events");
            if (!response.IsSuccessStatusCode) return new List<EventDisplayDto>();

            return JsonSerializer.Deserialize<List<EventDisplayDto>>(await response.Content.ReadAsStringAsync(), _options) ?? new List<EventDisplayDto>();
        }

        public async Task<bool> DispatchNewEventAsync(EventDisplayDto baseData, IFormFile? binaryImage)
        {
            InjectAuthorizationToken();
            using var multiPartForm = new MultipartFormDataContent();

            multiPartForm.Add(new StringContent(baseData.Name), nameof(baseData.Name));
            multiPartForm.Add(new StringContent(baseData.Description), nameof(baseData.Description));
            multiPartForm.Add(new StringContent(baseData.Date.ToString("yyyy-MM-dd")), nameof(baseData.Date));
            multiPartForm.Add(new StringContent(baseData.Location), nameof(baseData.Location));
            multiPartForm.Add(new StringContent(baseData.Distance.ToString()), nameof(baseData.Distance));
            multiPartForm.Add(new StringContent(baseData.EventType), nameof(baseData.EventType));

            if (binaryImage != null)
            {
                var fileStream = new StreamContent(binaryImage.OpenReadStream());
                fileStream.Headers.ContentType = new MediaTypeHeaderValue(binaryImage.ContentType);
                multiPartForm.Add(fileStream, "BannerFile", binaryImage.FileName);
            }

            var requestResult = await _http.PostAsync("events", multiPartForm);
            return requestResult.IsSuccessStatusCode;
        }

        public async Task<List<EnrolmentItemDto>> FetchUserEnrolmentsAsync()
        {
            InjectAuthorizationToken();
            var result = await _http.GetAsync("enrolments/my");
            if (!result.IsSuccessStatusCode) return new List<EnrolmentItemDto>();

            return JsonSerializer.Deserialize<List<EnrolmentItemDto>>(await result.Content.ReadAsStringAsync(), _options) ?? new List<EnrolmentItemDto>();
        }

        public async Task<bool> SubmitEnrolmentRequestAsync(int linkedEventId, string assignedCategory)
        {
            InjectAuthorizationToken();
            var dataPayload = new { EventId = linkedEventId, CategoryName = assignedCategory };
            var outputContent = new StringContent(JsonSerializer.Serialize(dataPayload), Encoding.UTF8, "application/json");

            var outcome = await _http.PostAsync("enrolments", outputContent);
            return outcome.IsSuccessStatusCode;
        }
    }
}
