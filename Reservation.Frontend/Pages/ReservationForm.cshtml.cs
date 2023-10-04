using System.ComponentModel.DataAnnotations;
using Htmx;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Reservation.Backend;
using Reservation.Backend.Models;
using Reservation.Frontend.Resources;

namespace Reservation.Frontend.Pages;

public class ReservationForm : PageModel
{
    private const string StartTime = "540";
    private const string DateFormat = "yyyy-MM-dd";
    public static string NoFreeReservations = "Brak wolnych rezerwacji";
    private const string InitialDate = "1970-01-01";
    private const string TyreChangeReservationDate = "_TyreChangeReservationDate";
    private const string TyreChangeReservationTime = "_TyreChangeReservationTime";
    public static DateOnly InitialReservationDate = DateOnly.Parse(InitialDate);

    public List<SelectListItem> PossibleLocations = new();

    public List<SelectListItem> PossibleDates = new();

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
    
    public bool AllowSubmit { get; set; }
    public bool IsPost { get; set; }
    
    [BindProperty(SupportsGet = true)]
    [Required(ErrorMessageResourceType = typeof(LABELS), ErrorMessageResourceName = "Required")]
    [Display(Name = "ReservationDate", ResourceType = typeof(LABELS))]
    public DateOnly? ReservationDate { get; set; } = InitialReservationDate;

    [BindProperty(SupportsGet = true)]
    [Required(ErrorMessageResourceType = typeof(LABELS), ErrorMessageResourceName = "Required")]
    [Display(Name = "ReservationTime", ResourceType = typeof(LABELS))]
    public int? ReservationTime { get; set; }

    [BindProperty(SupportsGet = true)]
    [Display(Name = "DepositNumber", ResourceType = typeof(LABELS))]
    public string? DepositNumber { get; set; }

    [BindProperty(SupportsGet = true)]
    [Required(ErrorMessageResourceType = typeof(LABELS), ErrorMessageResourceName = "Required")]
    [Display(Name = "CarModel", ResourceType = typeof(LABELS))]
    public string? CarModel { get; set; }

    [BindProperty(SupportsGet = true)]
    [Display(Name = "CarNumber", ResourceType = typeof(LABELS))]
    [Required(ErrorMessageResourceType = typeof(LABELS), ErrorMessageResourceName = "Required")]
    public string CarNumber { get; set; }

    [BindProperty(SupportsGet = true)]
    [Required(ErrorMessageResourceType = typeof(LABELS), ErrorMessageResourceName = "Required")]
    [Display(Name = "Location", ResourceType = typeof(LABELS))]
    [Range(1, int.MaxValue, ErrorMessageResourceType  = typeof(LABELS),ErrorMessageResourceName = "Required")]
    public int Location { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    [Required(ErrorMessageResourceType = typeof(LABELS), ErrorMessageResourceName = "Required")]
    [Display(Name = "CarType", ResourceType = typeof(LABELS))]
    public string? CarType { get; set; }

    [BindProperty(SupportsGet = true)]
    [Required(ErrorMessageResourceType = typeof(LABELS), ErrorMessageResourceName = "Required")]
    [Display(Name = "WheelType", ResourceType = typeof(LABELS))]
    public string? WheelType { get; set; }

    [BindProperty(SupportsGet = true)]
    [Required(ErrorMessageResourceType = typeof(LABELS), ErrorMessageResourceName = "Required")]
    [Display(Name = "OrderType", ResourceType = typeof(LABELS))]
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

    private async Task LoadLocationsAsync()
    {
        PossibleLocations = await GetLocationsAsync();
    }
    
    void SetDefaultLocation(bool isSelected)
    {
        PossibleLocations.Insert(0, new SelectListItem("Dostępne lokalizacje", "0")
        {
            Disabled = true,
            Selected = isSelected
        });
    }

    public async Task<IActionResult> OnGet()
    {
        await LoadLocationsAsync();
        SetDefaultLocation(true);
        return Page();
    }

    public async Task<IActionResult> OnGetLocationChanged(int Location)
    {
        this.Location = Location;
        await LoadDatesAsync(Location);
        Response.Htmx(htmx =>
        {
            htmx.WithTrigger("datesLoaded");
        });
        return Partial(TyreChangeReservationDate, this);
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

    private async Task LoadDatesAsync(int Location)
    {
        PossibleDates = await GetPossibleDatesAsync();
        if (ReservationDate.Equals(DateOnly.Parse(InitialDate)))
        {
            SetDefaultDate();
        }
        else
        {
            DateOnly.TryParse(PossibleDates[0].Value, out var reservationDate);
            ReservationDate = reservationDate;
        }

        return;

        void SetDefaultDate()
        {
            PossibleDates.Insert(0, new SelectListItem("Wybierz Datę", InitialDate)
            {
                Disabled = false,
                Selected = true
            });
        }
    }

    public async Task<IActionResult> OnGetTimeSlots(DateOnly ReservationDate, int Location)
    {
        this.ReservationDate = ReservationDate;
        this.Location = Location;
        PossibleHours = await GetPossibleHoursAsync();
        AllowSubmit = true; 
        Response.Htmx(htmx =>
        {
            htmx.WithTrigger("allowSubmit");
        });
        return Partial(TyreChangeReservationTime, this);
    }
    
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnPostSubmit()
    {
        if (!ModelState.IsValid)
        {
            IsPost = true;
            PossibleLocations = await GetLocationsAsync();
            PossibleDates = await GetPossibleDatesAsync();
            PossibleHours = await GetPossibleHoursAsync();
            Response.Htmx(htmx =>
            {
                htmx.WithTrigger("allowSubmit");
            });
            return Partial("_TyreChangeForm", this);
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