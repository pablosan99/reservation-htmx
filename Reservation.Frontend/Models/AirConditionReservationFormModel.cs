using System.ComponentModel.DataAnnotations;
using Reservation.Frontend.Resources;

namespace Reservation.Frontend.Models;

public class AirConditionReservationFormModel : BaseReservationFormModel
{
   [Display(Name = "AirConditionReservationModel_ProductionYear", ResourceType = typeof(LABELS))]
   public string ProductionYear { get; set; }
        
   [Display(Name = "AirConditionReservationMode_EngineCapacity", ResourceType = typeof(LABELS))]
   public string EngineCapacity { get; set; }

   [Display(Name = "AirConditionReservationMode_EnginePower", ResourceType = typeof(LABELS))]
   public string EnginePower { get; set; }
}