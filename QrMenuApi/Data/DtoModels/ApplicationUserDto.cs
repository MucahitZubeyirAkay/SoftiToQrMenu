using System;
using System.ComponentModel.DataAnnotations;

namespace QrMenuApi.Data.DtoModels
{
	public class ApplicationUserDto
	{
		public string UserName { get; set; } = "";
        public string Name { get; set; } = "";
        public string SurName { get; set; } = "";
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = "";
        public byte StateId { get; set; }
        public int CompanyId { get; set; }
    }
}

