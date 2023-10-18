using Microsoft.AspNetCore.SignalR;
using Reservation.Frontend.Models;

namespace Reservation.Frontend.Pages.Hubs;

public interface IReservationClient
{
    Task NotifyReservationInfo(ReservationInfo info);

    Task CreateNotification();

    Task InformAboutReservation(ReservationInfo info);
}

// [HubName("reservationHub")]
public class ReservationHub : Hub<IReservationClient>
{
    
}