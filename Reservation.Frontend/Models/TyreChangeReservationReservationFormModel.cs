using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Reservation.Frontend.Resources;

namespace Reservation.Frontend.Models;

public class TyreChangeReservationReservationFormModel : BaseReservationFormModel
{
    [Display(Name = "DepositNumber", ResourceType = typeof(LABELS))]
    public string? DepositNumber { get; set; }

    [Required(ErrorMessageResourceType = typeof(LABELS), ErrorMessageResourceName = "Required")]
    [Display(Name = "CarType", ResourceType = typeof(LABELS))]
    public string? CarType { get; set; }

    [Required(ErrorMessageResourceType = typeof(LABELS), ErrorMessageResourceName = "Required")]
    [Display(Name = "WheelType", ResourceType = typeof(LABELS))]
    public string? WheelType { get; set; }

    [Required(ErrorMessageResourceType = typeof(LABELS), ErrorMessageResourceName = "Required")]
    [Display(Name = "OrderType", ResourceType = typeof(LABELS))]
    public string? OrderType { get; set; }

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