using QrMenuApi.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace QrMenuApi.Data.DtoModels
{
    public class CompanyDto
    {
       
        public string Name { get; set; } = "";

        public string PostalCode { get; set; } = "";

        public string Address { get; set; } = "";

        public string PhoneNumber { get; set; } = "";

        public string EMail { get; set; } = "";

        public string TaxNumber { get; set; } = "";

        public string? WebAddress { get; set; } = "";

        public DateTime RegisterDate { get; set; }

        public byte StateId { get; set; }
    }
}
