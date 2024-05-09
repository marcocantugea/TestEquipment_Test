using System.ComponentModel.DataAnnotations;

namespace TestEquipment_Test.Models.Data
{
    public class Line_
    {
        [Key]
        public string? LineID { get; set; }
        public string? Line { get; set; }
    }
}
