using Microsoft.AspNetCore.Mvc;
using Reservation.Backend;

namespace Reservation.Frontend.Views.Shared.Components.ReservationDate;

public class ReservationDate: ViewComponent
{
    private readonly ApiClient _client;

    public ReservationDate(ApiClient client)
    {
        _client = client;
    }

    public async Task<IViewComponentResult> InvokeAsync(int locationId)
    {
        var locations = await _client.GetLocations();
        var location = locations.SingleOrDefault(x => x.Id == locationId);
        
        return View("Default", location);
    }
}