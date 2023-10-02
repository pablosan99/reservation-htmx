namespace Reservation.Backend.Models;

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
}

public class WebReservationResponse
{
    public int ReservationId { get; set; }
    public string ReservationDate { get; set; }
    public string ChangeDate { get; set; }
    public string DepositNumber { get; set; }
    public string CarModel { get; set; }
    public string CarNumber { get; set; }
    public string Location { get; set; }
    public int LocationId { get; set; }
    public string CarType { get; set; }
    public string WheelType { get; set; }
    public string OrderType { get; set; }
}