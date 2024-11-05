using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant_API.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        [NotMapped]
        public string RazorpayPaymentIntentId { get; set; }
        [NotMapped]
        public string clientSecret { get; set; }

        [NotMapped]
        public double CartTotal { get; set; }

        public ICollection<CartItem> CartItems { get; set; }
    }
}
