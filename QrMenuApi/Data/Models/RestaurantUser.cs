
namespace QrMenuApi.Data.Models
{
    public class RestaurantUser
    {
        public int RestaurantId { get; set; }

        public int UserId { get; set; }



        public Restaurant? Restaurant { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }
    }
}
