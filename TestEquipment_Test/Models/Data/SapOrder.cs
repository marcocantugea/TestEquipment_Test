using System.ComponentModel.DataAnnotations;

namespace TestEquipment_Test.Models.Data
{
    public class CheckOrder_
    {
        [Key]
        public string? success { get; set; }
        public string? message { get; set; }
    }

}
