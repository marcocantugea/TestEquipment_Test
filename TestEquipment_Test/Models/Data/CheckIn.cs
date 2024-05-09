using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TestEquipment_Test.Models.Data
{
    
    public class CheckIn_
    {
        [Key]
        public string? success { get; set; }
        public string? message { get; set; }
    }
}
