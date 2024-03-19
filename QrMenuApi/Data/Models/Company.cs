using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QrMenuApi.Data.Models
{
    public class Company
    {
        public int Id { get; set; }


        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; } = "";


        [StringLength(5, MinimumLength = 5)]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; } = "";


        [StringLength(200, MinimumLength = 5)]
        public string Address { get; set; } = "";


        [StringLength(30)]
        public string PhoneNumber { get; set; } = "";


        [EmailAddress]
        [StringLength(100)]
        public string EMail { get; set; } = "";



        [StringLength(11, MinimumLength = 10)]
        public string TaxNumber { get; set; } = "";


        [StringLength(100)]
        public string? WebAddress { get; set; } = "";


        public DateTime RegisterDate { get; set; }



        public byte StateId { get; set; }


        public int? ParentCompanyId { get; set; }

        public Company? ParentCompany { get; set; }

        public State? State { get; set; }

        public virtual List<Restaurant>? Restaurants { get; set; }
    }
}
