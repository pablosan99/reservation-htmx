using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Reservation.Frontend.Resources;

namespace Reservation.Frontend.Models;

public class TyreChangeReservationFormModel
{
    [Required(ErrorMessageResourceType = typeof(LABELS), ErrorMessageResourceName = "Required")]
    [Display(Name = "ReservationDate", ResourceType = typeof(LABELS))]
    public DateOnly? ReservationDate { get; set; }

    [Required(ErrorMessageResourceType = typeof(LABELS), ErrorMessageResourceName = "Required")]
    [Display(Name = "ReservationTime", ResourceType = typeof(LABELS))]
    public int? ReservationTime { get; set; }

    [Display(Name = "DepositNumber", ResourceType = typeof(LABELS))]
    public string? DepositNumber { get; set; }

    [Required(ErrorMessageResourceType = typeof(LABELS), ErrorMessageResourceName = "Required")]
    [Display(Name = "CarModel", ResourceType = typeof(LABELS))]
    public string? CarModel { get; set; }

    [Display(Name = "CarNumber", ResourceType = typeof(LABELS))]
    [Required(ErrorMessageResourceType = typeof(LABELS), ErrorMessageResourceName = "Required")]
    [StringLength(maximumLength: 7, MinimumLength = 5, ErrorMessageResourceType = typeof(LABELS), ErrorMessageResourceName = "CarNumber_InvalidLength")]
    public string? CarNumber { get; set; }

    [Required(ErrorMessageResourceType = typeof(LABELS), ErrorMessageResourceName = "Required")]
    [Display(Name = "Location", ResourceType = typeof(LABELS))]
    [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(LABELS), ErrorMessageResourceName = "Required")]
    public int? Location { get; set; }

    [Required(ErrorMessageResourceType = typeof(LABELS), ErrorMessageResourceName = "Required")]
    [Display(Name = "CarType", ResourceType = typeof(LABELS))]
    public string? CarType { get; set; }

    [Required(ErrorMessageResourceType = typeof(LABELS), ErrorMessageResourceName = "Required")]
    [Display(Name = "WheelType", ResourceType = typeof(LABELS))]
    public string? WheelType { get; set; }

    [Required(ErrorMessageResourceType = typeof(LABELS), ErrorMessageResourceName = "Required")]
    [Display(Name = "OrderType", ResourceType = typeof(LABELS))]
    public string? OrderType { get; set; }

    public bool IsPost { get; set; }

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
}