using System;

namespace Reservation.Backend.Models;

public class Location
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Color { get; set; } = default!;
    public DateTime? DisableTime { get; set; }
    public bool Active { get; set; }
    public string InvoiceShortcut { get; set; } = default!;
    public bool Default { get; set; }
}