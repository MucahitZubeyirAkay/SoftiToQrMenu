using System;
using System.ComponentModel.DataAnnotations;

namespace QrMenuApi.Data.DtoModels
{
	public class FoodDto
	{
        public string Name { get; set; } = "";

        public float Price { get; set; }

        public string? Description { get; set; }

        public string? ImagePath { get; set; }


        public byte StateId { get; set; }

        public int CategoryId { get; set; }

    }
}

