using Microsoft.AspNetCore.Mvc;

namespace Reservation.Frontend.Pages.ViewComponents;

public class ReservationDate: ViewComponent
{
    public ReservationDate()
    {
    }

    public IViewComponentResult Invoke(string type)
    {
        return View("Default", type);
    }
}