using Htmx;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Reservation.Frontend.Pages;

public class ReservationForm : PageModel
{
    public readonly List<SelectListItem> PossibleLocations = new()
    {
        new("Warszawska 12a", 1.ToString()),
        new("Nadbystrzycka 33", 2.ToString()),
    };

    public readonly List<SelectListItem> PossibleDates = new();

    public readonly List<SelectListItem> PossibleHours = new();
    
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
    
    [BindProperty(SupportsGet = true)]
    public DateOnly ReservationDate { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public int ReservationTime { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string DepositNumber { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string CarModel { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string CarNumber { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public int Location { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string CarType { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string WheelType { get; set; }

    [BindProperty(SupportsGet = true)]
    public string OrderType { get; set; }
    
    public IActionResult OnGet()
    {
        return Request.IsHtmx() ? Content("", "text/html") : Page();
    }

    public async Task<IActionResult> OnPost()
    {
        return Request.IsHtmx() ? Partial("ReservationCompleted", this) : Page();
    }
}