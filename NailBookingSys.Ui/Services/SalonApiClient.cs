using NailBookingSys.Ui.Contracts;

namespace NailBookingSys.Ui.Services;

public class SalonApiClient
{
    private readonly HttpClient _http;
    public SalonApiClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<ServiceDto>> GetServicesAsync()
        => await _http.GetFromJsonAsync<List<ServiceDto>>("api/services") ?? [];

    public async Task<IReadOnlyList<StaffDto>> GetStaffAsync()
        => await _http.GetFromJsonAsync<List<StaffDto>>("api/staff") ?? [];

    public async Task<IReadOnlyList<AppointmentDto>> GetAppointmentsForDayAsync(DateTime day)
        => await _http.GetFromJsonAsync<List<AppointmentDto>>(
               $"api/appointments?day={day:yyyy-MM-dd}") ?? [];

    public async Task<(bool ok, string? error, BookAppointmentResponseDto? result)> BookAsync(BookAppointmentRequestDto req)
    {
        var resp = await _http.PostAsJsonAsync("api/appointments", req);
        if (resp.IsSuccessStatusCode)
            return (true, null, await resp.Content.ReadFromJsonAsync<BookAppointmentResponseDto>());
        return (false, await ExtractErrorAsync(resp), null);
    }

    public async Task<(bool ok, string? error)> CancelAsync(Guid id)
    {
        var resp = await _http.PostAsync($"api/appointments/{id}/cancel", null);
        return resp.IsSuccessStatusCode ? (true, null) : (false, await ExtractErrorAsync(resp));
    }

    public async Task<(bool ok, string? error)> RescheduleAsync(Guid id, DateTime newStart)
    {
        var resp = await _http.PostAsJsonAsync($"api/appointments/{id}/reschedule",
            new { newStartTime = newStart });
        return resp.IsSuccessStatusCode ? (true, null) : (false, await ExtractErrorAsync(resp));
    }

    private static async Task<string?> ExtractErrorAsync(HttpResponseMessage resp)
    {
        try
        {
            var doc = await resp.Content.ReadFromJsonAsync<ErrorEnvelope>();
            return doc?.Errors switch
            {
                null => resp.ReasonPhrase,
                System.Text.Json.JsonElement el when el.ValueKind == System.Text.Json.JsonValueKind.Array =>
                    string.Join(" ", el.EnumerateArray().Select(x => x.GetString())),
                System.Text.Json.JsonElement el when el.ValueKind == System.Text.Json.JsonValueKind.Object =>
                    string.Join(" ", el.EnumerateObject().SelectMany(p =>
                        p.Value.ValueKind == System.Text.Json.JsonValueKind.Array
                            ? p.Value.EnumerateArray().Select(x => x.GetString())
                            : new[] { p.Value.GetString() })),
                _ => resp.ReasonPhrase
            };
        }
        catch
        {
            return resp.ReasonPhrase;
        }
    }

    private sealed record ErrorEnvelope(System.Text.Json.JsonElement? Errors);
}
