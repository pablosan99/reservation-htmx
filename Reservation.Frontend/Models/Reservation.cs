using Microsoft.AspNetCore.Mvc.Rendering;

namespace Reservation.Frontend.Models;

public class TyreChangeReservation
{
    public DateOnly ReservationDate { get; set; }

    public int ReservationTime { get; set; }

    public string DepositNumber { get; set; }

    public string CarModel { get; set; }

    public string CarNumber { get; set; }

    public int Location { get; set; }

    public string CarType { get; set; }

    public string WheelType { get; set; }

    public string OrderType { get; set; }

    public List<SelectListItem> PossibleCarTypes = new();
    public List<SelectListItem> PossibleWheelTypes = new();
    public List<SelectListItem> PossibleOrderTypes = new();
    public List<SelectListItem> PossibleLocations = new();
    public List<SelectListItem> PossibleDates = new();
    public List<SelectListItem> PossibleHours = new();
}

