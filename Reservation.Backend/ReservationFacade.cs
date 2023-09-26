namespace Reservation.Backend;

public class ReservationFacade
{
    private readonly ApiClient _client;

    public ReservationFacade(ApiClient client)
    {
        _client = client;
    }
    
    
}