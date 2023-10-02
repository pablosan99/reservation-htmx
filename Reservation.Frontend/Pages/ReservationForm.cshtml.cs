using System.ComponentModel.DataAnnotations;
using Htmx;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Reservation.Backend;
using Reservation.Backend.Models;

namespace Reservation.Frontend.Pages;

public class ReservationForm : PageModel
{
    private const string StartTime = "540";
    private const string DateFormat = "yyyy-MM-dd";
    public static string NoFreeReservations = "Brak wolnych rezerwacji";
    
    public List<SelectListItem> PossibleLocations;

    public List<SelectListItem> PossibleDates;

    public List<SelectListItem> PossibleHours = new();
    
    public readonly List<SelectListItem> CarTypes = new()
    {
        new("Osobowy", "Personal"),
        new("Bus", "Bus"),
        new("Run-Flat", "RunFlat"),
    };

    public readonly List<SelectListItem> WheelTypes = new()
    {
        new("Aluminiowe", "Aluminium"),
        new("Stalowe", "Steel")
    };

    public readonly List<SelectListItem> OrderTypes = new()
    {
        new("Wymiana opon", "SwapTyres"),
        new("Przekładka całych kół", "SwapWheels")
    };
    
    [BindProperty(SupportsGet = true), Required]
    public DateOnly? ReservationDate { get; set; }
    
    [BindProperty(SupportsGet = true), Required]
    public int? ReservationTime { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? DepositNumber { get; set; }
    
    [BindProperty(SupportsGet = true), Required]
    public string? CarModel { get; set; }
    
    [BindProperty(SupportsGet = true), Required]
    public string? CarNumber { get; set; }

    [BindProperty(SupportsGet = true), Required] public int Location { get; set; } = default!;
    
    [BindProperty(SupportsGet = true), Required]
    public string? CarType { get; set; }
    
    [BindProperty(SupportsGet = true), Required]
    public string? WheelType { get; set; }

    [BindProperty(SupportsGet = true), Required]
    public string? OrderType { get; set; }

    private readonly ApiClient client;

    public ReservationForm(ApiClient client)
    {
        this.client = client;
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

    private async Task<List<SelectListItem>> GetLocationsAsync()
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

    /// <summary>
    /// Returns list of dates within 
    /// </summary>
    /// <returns></returns>
    private async Task<List<SelectListItem>> GetPossibleDatesAsync()
    {
        var dayOffsForNext30Days = await client.GetDayOffsForNext30Days();
        Dictionary<DateTime, DayOff> dictDayOffs = new();
        if (dayOffsForNext30Days.Any())
        {
            dictDayOffs = dayOffsForNext30Days.ToDictionary(x => x.Date);
        }

        var minimalTime = 3; //hours

        if (Location > 0)
        {
            var attributes = await client.ListForObjectNameAndObjectId("Location", Location);
            if (attributes.Any())
            {
                var _attributes = attributes.ToList();
                int.TryParse(_attributes[0].AttrValue, out minimalTime);
            }
        }

        return BuildSelectListItems(dictDayOffs, minimalTime);
    }
    
    public async Task<IActionResult> OnGet()
    {
        var locations = await GetLocationsAsync();
        if (locations.Count > 0)
        {
            if (Location == 0)
            {
                Location = int.Parse(locations[0].Value);
            
            }

            PossibleLocations = locations;
        }        
        
        return Page();
    }

    private async Task<List<SelectListItem>> GetPossibleHoursAsync()
    {
        var timeSlots = await client.GetFreeTimeSlotsForTyreChange(new TimeSlotRequest(ReservationDate.Value, Location));
        List<SelectListItem> items;
        if (timeSlots.Count > 0)
        {
            items = timeSlots.Select(x => new SelectListItem()
            {
                Value = x.Counter.ToString(),
                Text = x.Hour,
                Selected = (ReservationTime.HasValue && (x.Counter == ReservationTime))
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
    
    public async Task<IActionResult> OnGetDates(int Location)
    {
        this.Location = Location;
        PossibleDates = await GetPossibleDatesAsync();
        
        return Partial("_TyreChangeReservationDateHtmx", this);
    }

    public async Task<IActionResult> OnGetTimeSlots(DateOnly ReservationDate)
    {
        this.ReservationDate = ReservationDate;
        PossibleHours = await GetPossibleHoursAsync();

        return Partial("_TyreChangeReservationTimeHtmx", this);
    }
    
    
    
    public async Task<IActionResult> OnPost()
    {
        if (!this.ModelState.IsValid) 
        {
            
        }
        
        var requestModel = new TyreChangeReservation()
        {
            Location = this.Location,
            ReservationDate = this.ReservationDate.Value,
            ReservationTime = this.ReservationTime.Value,
            CarModel = this.CarModel,
            CarNumber = this.CarNumber,
            CarType = this.CarType,
            DepositNumber = this.DepositNumber,
            OrderType = this.OrderType,
            WheelType = this.WheelType
        };
        var response = await client.RegisterTyreChange(requestModel);
        
        return Request.IsHtmx() ? Partial("ReservationCompleted", this) : Page();
    }
}