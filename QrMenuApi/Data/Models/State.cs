using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QrMenuApi.Data.Models
{
    public class State
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte Id { get; set; }


        [StringLength(10)]
        public string Name { get; set; } = "";
    }
}
