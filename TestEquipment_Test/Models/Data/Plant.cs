using System.ComponentModel.DataAnnotations;

namespace TestEquipment_Test.Models.Data
{
    public class Plant_
    {
        [Key]
        public string? PlantId { get; set; }
        public string? Plant { get; set; }
    }
}
