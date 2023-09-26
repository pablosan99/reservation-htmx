using Microsoft.Extensions.DependencyInjection;

namespace Reservation.Backend;

public static class ReservationModuleRegistry
{
    public static void ReservationModule(this IServiceCollection services)
    {
        services.AddHttpClient<ApiClient>();
        services.AddTransient<ReservationFacade>();
    }
}