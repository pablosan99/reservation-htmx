﻿using Microsoft.AspNetCore.Mvc;
using Htmx;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Reservation.Backend.Models;
using Reservation.Frontend.Models;
using Reservation.Frontend.Pages.Hubs;

namespace Reservation.Frontend.Pages.Controllers;

public class TyreChangeReservationController : Controller
{
    private const string ReservationDate = "_ReservationDate";
    private const string TyreChangeReservationDate = "_TyreChangeReservationDate";
    private const string TyreChangeReservationTime = "_TyreChangeReservationTime";
    private const string HtmxEventDataName = "form-event-data";
    private const string HtmxFormTypeChanged = "form-type-changed";
    private const string AvailableLocalizations = "Dostępne lokalizacje";

    private readonly DataFormProvider _dataFormProvider;
    private readonly IHubContext<ReservationHub> _hubContext;

    public TyreChangeReservationController(DataFormProvider dataFormProvider, IHubContext<ReservationHub> hubContext)
    {
        _dataFormProvider = dataFormProvider;
        _hubContext = hubContext;
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
        var model = new TyreChangeReservationReservationFormModel
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
        var model = new TyreChangeReservationReservationFormModel
        {
            PossibleLocations = possibleLocations
        };
        
        Response.Htmx(htmx =>
        {
            htmx.WithTrigger(HtmxFormTypeChanged,
                new { type = "tyre-change"});
        });
        return PartialView("_TyreChangeForm", model);
    }

    [HttpGet]
    public async Task<IActionResult> LocationChanged([FromQuery] int location)
    {
        var possibleDates = await _dataFormProvider.GetPossibleDatesAsync(location);
        DateOnly.TryParse(possibleDates[0].Value, out var reservationDate);
        var possibleHours = await _dataFormProvider.GetPossibleHoursAsync(reservationDate, null, location);

        Response.Htmx(htmx =>
        {
            htmx.WithTrigger(HtmxEventDataName,
                new {hasFreeHours = possibleHours.All(x => x.Value != DataFormProvider.NoFreeReservationIdx)});
        });

        var model = new TyreChangeReservationReservationFormModel()
        {
            PossibleDates = possibleDates,
            ReservationDate = reservationDate,
            PossibleHours = possibleHours,
            Location = location
        };

        return PartialView(ReservationDate, model);
    }

    [HttpGet]
    public async Task<IActionResult> TimeSlots(DateOnly reservationDate, int location, int? reservationTime = null)
    {
        var possibleHours = await _dataFormProvider.GetPossibleHoursAsync(reservationDate, reservationTime, location);
        Response.Htmx(htmx =>
        {
            htmx.WithTrigger(HtmxEventDataName,
                new {hasFreeHours = possibleHours.All(x => x.Value != DataFormProvider.NoFreeReservationIdx)});
        });

        return PartialView(TyreChangeReservationTime, new TyreChangeReservationReservationFormModel()
        {
            PossibleHours = possibleHours
        });
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> OnPostSubmit(TyreChangeReservationReservationFormModel model)
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
        var response = await _dataFormProvider.SaveAsync(requestModel);
        _hubContext.Clients.All.SendAsync("InformAboutReservation", new ReservationInfo
        {
            Id = response.ReservationId,
            Date = response.ReservationDate,
            LocationName = response.Location
        });
        return PartialView("_ReservationCompleted", response);
    }
}