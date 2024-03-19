using System.ComponentModel.DataAnnotations;

namespace QrMenuApi.Data.Models
{
    public class Food
    {
        public int Id { get; set; }


        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = "";


        [Range(0, float.MaxValue)]
        public float Price { get; set; }


        [StringLength(200)]
        public string? Description { get; set; }



        public byte StateId { get; set; }

        public int CategoryId { get; set; }




        public State? State { get; set; }

        public Category? Category { get; set; }


    }
}
