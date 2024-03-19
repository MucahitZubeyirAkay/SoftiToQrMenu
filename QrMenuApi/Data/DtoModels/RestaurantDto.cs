using System;
using System.ComponentModel.DataAnnotations;

namespace QrMenuApi.Data.DtoModels
{
	public class RestaurantDto
	{
        
        public string Name { get; set; } = "";

        public string PhoneNumber { get; set; } = "";

        public string Address { get; set; } = "";

        public string PostalCode { get; set; } = "";

        public byte StateId { get; set; }

        public int CompanyId { get; set; }
    }
}

