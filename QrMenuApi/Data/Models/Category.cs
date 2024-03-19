using System.ComponentModel.DataAnnotations;

namespace QrMenuApi.Data.Models
{
    public class Category
    {
        public int Id { get; set; }


        [StringLength(50, MinimumLength = 5)]
        public string Name { get; set; } = "";


        [StringLength(200)]
        public string Description { get; set; } = "";


        public DateTime RegisterDate { get; set; }



        public byte StateId { get; set; }

        public int RestraurantId { get; set; }



        public State? State { get; set; }

        public Restaurant? Restaurant { get; set; }

        public virtual List<Food>? Foods { get; set; }
    }
}
