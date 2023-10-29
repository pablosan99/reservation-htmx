using Microsoft.AspNetCore.Mvc.Rendering;

namespace Reservation.Frontend.Models;

public class SelectorModel
{
    public string Value { get; set; }
    public List<SelectListItem> Items { get; set; } = new();
}