using Microsoft.AspNetCore.Mvc.Rendering;
using Reservation.Backend;
using Reservation.Backend.Models;
using Reservation.Frontend.Models;

namespace Reservation.Frontend.Pages;

public class ModelBuilder
{
    private const string DateFormat = "yyyy-MM-dd";
    private const string StartTime = "540";
    public static string NoFreeReservations = "Brak wolnych rezerwacji";

    private readonly ApiClient client;

    public ModelBuilder(ApiClient client)
    {
        this.client = client;
    }

    public TyreChangeReservationFormModel CreateModelOnGet()
    {
        return new TyreChangeReservationFormModel()
        {
            
        };
    }
    
    public async Task<List<SelectListItem>> GetLocationsAsync()
    {
        var locations = await this.client.GetLocations();
        if (locations is null)
        {
            return new List<SelectListItem>();
        }

        return locations.Where(x => x.Active).Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name,
        }).ToList();
    }
    
    public async Task<List<SelectListItem>> GetPossibleDatesAsync(int location)
    {
        var dayOffsForNext30Days = await client.GetDayOffsForNext30Days();
        Dictionary<DateTime, DayOff> dictDayOffs = new();
        if (dayOffsForNext30Days.Any())
        {
            dictDayOffs = dayOffsForNext30Days.ToDictionary(x => x.Date);
        }

        var minimalTime = 3; //hours

        if (location > 0)
        {
            var attributes = await client.ListForObjectNameAndObjectId("Location", location);
            if (attributes.Any())
            {
                var _attributes = attributes.ToList();
                int.TryParse(_attributes[0].AttrValue, out minimalTime);
            }
        }

        return BuildSelectListItems(dictDayOffs, minimalTime);
    }

    public async Task<WebReservationResponse> SaveAsync(TyreChangeReservation request)
    {
        return await client.RegisterTyreChange(request);
    }
    
    public async Task<List<SelectListItem>> GetPossibleHoursAsync(DateOnly reservationDate, int? reservationTime, int location)
    {
        var timeSlots = await client.GetFreeTimeSlotsForTyreChange(new TimeSlotRequest(reservationDate, location));
        List<SelectListItem> items;
        if (timeSlots.Count > 0)
        {
            items = timeSlots.Select(x => new SelectListItem()
            {
                Value = x.Counter.ToString(),
                Text = x.Hour,
                Selected = (reservationTime.HasValue && (x.Counter == reservationTime))
            }).ToList();
        }
        else
        {
            items = new List<SelectListItem>
            {
                new() {Value = StartTime, Text = NoFreeReservations, Selected = true}
            };
        }

        return items;
    }
    
    private static List<SelectListItem> BuildSelectListItems(IReadOnlyDictionary<DateTime, DayOff> dictDayOffs, int minimalTime)
    {
        var add = DateTime.Now.Minute > 30 ? 1 : 0;
        var threeHourAhead = DateTime.Now.Add(new TimeSpan(minimalTime + add, 0, 0));
        var dtStart = new DateTime(threeHourAhead.Year, threeHourAhead.Month, threeHourAhead.Day, threeHourAhead.Hour, 0, 0);
        var dtEnd = dtStart.AddDays(14);
        var possibleDates = new List<SelectListItem>();
        for (var dt = dtStart; dt <= dtEnd; dt = dt.AddDays(1))
        {
            if (dictDayOffs.ContainsKey(dt.Date))
                continue;
            possibleDates.Add(
                new SelectListItem()
                {
                    Value = dt.ToString(DateFormat),
                    Text = dt.ToString(DateFormat)
                });
        }

        return possibleDates;
    }
}