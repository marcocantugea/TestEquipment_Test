using System.ComponentModel.DataAnnotations;

namespace TestEquipment_Test.Models.Data
{
    public class Station_
    {
        [Key]
        public string? StationId { get; set; }
        public string? Station { get; set; }
    }
}
