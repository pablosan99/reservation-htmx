using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Reservation.Frontend.Resources;

namespace Reservation.Frontend.Models;

public class BaseReservationFormModel
{
    [Required(ErrorMessageResourceType = typeof(LABELS), ErrorMessageResourceName = "Required")]
    [Display(Name = "ReservationDate", ResourceType = typeof(LABELS))]
    public DateOnly? ReservationDate { get; set; }

    [Required(ErrorMessageResourceType = typeof(LABELS), ErrorMessageResourceName = "Required")]
    [Display(Name = "ReservationTime", ResourceType = typeof(LABELS))]
    public int? ReservationTime { get; set; }
    
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
    
    public bool IsPost { get; set; }

    public List<SelectListItem> PossibleLocations = new();

    public List<SelectListItem> PossibleDates = new();

    public List<SelectListItem> PossibleHours = new();
}