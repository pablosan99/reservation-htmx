using Microsoft.AspNetCore.Mvc;
using Htmx;
using Microsoft.AspNetCore.Mvc.Rendering;
using Reservation.Backend.Models;
using Reservation.Frontend.Models;

namespace Reservation.Frontend.Pages.Controllers;

public class ReservationFormController : Controller
{
    private const string ReservationDate = "_ReservationDate";
    private const string TyreChangeReservationDate = "_TyreChangeReservationDate";
    private const string TyreChangeReservationTime = "_TyreChangeReservationTime";

    private readonly ModelBuilder _modelBuilder;

    public ReservationFormController(ModelBuilder modelBuilder)
    {
        _modelBuilder = modelBuilder;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var possibleLocations = await _modelBuilder.GetLocationsAsync();
        possibleLocations.Insert(0, new SelectListItem("Dostępne lokalizacje", "0")
        {
            Disabled = true,
            Selected = true
        });
        var model = new TyreChangeReservationFormModel
        {
            PossibleLocations = possibleLocations
        };
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> LocationChanged([FromQuery] int location)
    {
        var possibleDates = await _modelBuilder.GetPossibleDatesAsync(location);
        DateOnly.TryParse(possibleDates[0].Value, out var reservationDate);
        var possibleHours = await _modelBuilder.GetPossibleHoursAsync(reservationDate, null, location);

        Response.Htmx(htmx =>
        {
            htmx.WithTrigger("locationChanged", null, HtmxTriggerTiming.AfterSwap);
            htmx.WithTrigger("datesLoaded", null, HtmxTriggerTiming.AfterSwap);
        });
        
        var model = new TyreChangeReservationFormModel()
        {
            PossibleDates = possibleDates,
            ReservationDate = reservationDate,
            PossibleHours = possibleHours
        };

        return PartialView(ReservationDate, model);
    }

    [HttpGet]
    public async Task<IActionResult> TimeSlots(DateOnly reservationDate, int location, int? reservationTime = null)
    {
        var possibleHours = await _modelBuilder.GetPossibleHoursAsync(reservationDate, reservationTime, location);
        
        Response.Htmx(htmx => { htmx.WithTrigger("allowSubmit"); });
        return PartialView(TyreChangeReservationTime, new TyreChangeReservationFormModel()
        {
            PossibleHours = possibleHours
        });
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> OnPostSubmit(TyreChangeReservationFormModel model)
    {
        if (!ModelState.IsValid)
        {
            var possibleLocations = await _modelBuilder.GetLocationsAsync();
            possibleLocations.Insert(0, new SelectListItem("Dostępne lokalizacje", "0")
            {
                Disabled = true,
                Selected = model.Location is 0
            });
            var possibleDates = await _modelBuilder.GetPossibleDatesAsync(model.Location.Value);
            var possibleHours = await _modelBuilder.GetPossibleHoursAsync(model.ReservationDate.Value, model.ReservationTime, model.Location.Value);
            
            model.PossibleLocations = possibleLocations;
            model.PossibleDates = possibleDates;
            model.PossibleHours = possibleHours;
            model.IsPost = true;
            Response.Htmx(htmx => { htmx.WithTrigger("allowSubmit"); });
            return PartialView("_TyreChangeForm", model);
        }

        var requestModel = new TyreChangeReservation
        {
            Location = model.Location.Value,
            ReservationDate = model.ReservationDate.Value,
            ReservationTime = model.ReservationTime.Value,
            CarModel = model.CarModel!,
            CarNumber = model.CarNumber!,
            CarType = model.CarType!,
            DepositNumber = model.DepositNumber ?? string.Empty,
            OrderType = model.OrderType,
            WheelType = model.WheelType
        };
        var response = await _modelBuilder.SaveAsync(requestModel);
        return PartialView("_ReservationCompleted", response);
    }
}