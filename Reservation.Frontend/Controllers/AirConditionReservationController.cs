using Microsoft.AspNetCore.Mvc;

namespace Reservation.Frontend.Pages.Controllers;

public class AirConditionReservationController : Controller
{
    public IActionResult Index()
    {
        ViewData["loadFirst"] = true;
        return View();
    }

    [HttpGet]
    public IActionResult GetForm()
    {
        return PartialView("_AirConditionForm");
    }
}