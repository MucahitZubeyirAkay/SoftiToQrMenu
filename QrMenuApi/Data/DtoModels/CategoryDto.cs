using System;
using System.ComponentModel.DataAnnotations;

namespace QrMenuApi.Data.DtoModels
{
	public class CategoryDto
	{
        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public byte StateId { get; set; }

        public int RestraurantId { get; set; }
    }
}

