using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Restaurant_API.Models.Dto
{
    public class OrderHeaderCreateDTO
    {
        
        [Required]
        public string PickupName { get; set; }
        [Required]
        public string PickupPhoneNumber { get; set; }
        [Required]
        public string PickupEmail { get; set; }

        public string ApplicationUserId { get; set; }
        public double OrderTotal { get; set; }

        public string RazorpayPaymentIntentId { get; set; }
        public string Status { get; set; }
        public int TotalItems { get; set; }

        public IEnumerable<OrderDetailsCreateDTO> OrderDetailsDTO { get; set; }
    }
}
