using Htmx;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Reservation.Backend.Models;
using Reservation.Frontend.Models;

namespace Reservation.Frontend.Pages.Controllers;

public class AirConditionReservationController : Controller
{
    private const string AvailableLocalizations = "Dostępne lokalizacje";
    private const string HtmxEventDataName = "form-event-data";
    private const string HtmxFormTypeChanged = "form-type-changed";
    
    private readonly DataFormProvider _dataFormProvider;

    public AirConditionReservationController(DataFormProvider dataFormProvider)
    {
        _dataFormProvider = dataFormProvider;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var possibleLocations = await _dataFormProvider.GetLocationsAsync();
        possibleLocations.Insert(0, new SelectListItem(AvailableLocalizations, "0")
        {
            Disabled = true,
            Selected = true
        });
        var model = new AirConditionReservationFormModel()
        {
            PossibleLocations = possibleLocations
        };
        ViewData["loadFirst"] = true;
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> GetForm()
    {
        var possibleLocations = await _dataFormProvider.GetLocationsAsync();
        possibleLocations.Insert(0, new SelectListItem(AvailableLocalizations, "0")
        {
            Disabled = true,
            Selected = true
        });
        var model = new AirConditionReservationFormModel()
        {
            PossibleLocations = possibleLocations
        };
        
        Response.Htmx(htmx =>
        {
            htmx.WithTrigger(HtmxFormTypeChanged,
                new { type = "air-condition"});
        });
        return PartialView("_AirConditionForm", model);
    }
    
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> OnPostSubmit(AirConditionReservationFormModel model)
    {
        if (!ModelState.IsValid)
        {
            var possibleLocations = await _dataFormProvider.GetLocationsAsync();
            possibleLocations.Insert(0, new SelectListItem(AvailableLocalizations, "0")
            {
                Disabled = true,
                Selected = model.Location is 0
            });
            var possibleDates = await _dataFormProvider.GetPossibleDatesAsync(model.Location.Value);
            var possibleHours = await _dataFormProvider.GetPossibleHoursAsync(model.ReservationDate.Value, model.ReservationTime, model.Location.Value);

            model.PossibleLocations = possibleLocations;
            model.PossibleDates = possibleDates;
            model.PossibleHours = possibleHours;
            model.IsPost = true;

            Response.Htmx(htmx =>
            {
                htmx.WithTrigger(HtmxEventDataName,
                    new {hasFreeHours = possibleHours.All(x => x.Value != DataFormProvider.NoFreeReservationIdx)}, HtmxTriggerTiming.AfterSwap);
            });
            return PartialView("_AirConditionForm", model);
        }
        
        var requestModel = new AirConditionReservation()
        {
            Location = model.Location.Value,
            ReservationDate = model.ReservationDate.Value,
            ReservationTime = model.ReservationTime.Value,
            CarModel = model.CarModel!,
            CarNumber = model.CarNumber!,
        };
         var response = await _dataFormProvider.SaveAirCondition(requestModel);
        return PartialView("_ReservationCompleted", response);
    }
}