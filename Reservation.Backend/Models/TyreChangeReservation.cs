namespace Reservation.Backend.Models;

public class TyreChangeReservation
{
    public DateOnly ReservationDate { get; set; } = DateOnly.MinValue;
    public int ReservationTime { get; set; } = default!;
    public string DepositNumber { get; set; } = default!;
    public string CarModel { get; set; } = default!;
    public string CarNumber { get; set; } = default!;
    public int Location { get; set; } = default!;
    public string CarType { get; set; } = default!;
    public string WheelType { get; set; } = default!;
    public string OrderType { get; set; } = default!;
}

public class WebReservationResponse
{
    public int ReservationId { get; set; } = default!;
    public string ReservationDate { get; set; } = default!;
    public string ChangeDate { get; set; } = default!;
    public string DepositNumber { get; set; } = default!;
    public string CarModel { get; set; } = default!;
    public string CarNumber { get; set; } = default!;
    public string Location { get; set; } = default!;
    public int LocationId { get; set; } = default!;
    public string CarType { get; set; } = default!;
    public string WheelType { get; set; } = default!;
    public string OrderType { get; set; } = default!;
}