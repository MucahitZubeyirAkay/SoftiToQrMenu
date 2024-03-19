using System.ComponentModel.DataAnnotations;

namespace QrMenuApi.Data.Models
{
    public class Restaurant
    {
        public int Id { get; set; }


        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; } = "";


        [StringLength(30)]
        public string PhoneNumber { get; set; } = "";


        [StringLength(200, MinimumLength = 5)]
        public string Address { get; set; } = "";


        [StringLength(5, MinimumLength = 5)]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; } = "";

        public DateTime RegisterDate { get; set; }


        public byte StateId { get; set; }

        public int CompanyId { get; set; }




        public State? State { get; set; }

        public Company? Company { get; set; }

        public virtual List<Category>? Categories { get; set; }
    }
}
