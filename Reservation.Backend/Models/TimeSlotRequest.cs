using System;

namespace Reservation.Backend.Models;

public record TimeSlotRequest(DateOnly ChangeDateTime, int Location);