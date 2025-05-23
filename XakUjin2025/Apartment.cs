using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XakUjin2025
{
    public class Apartment
    {
        [Key]
        public int ApartmentId { get; set; }
        public string? ApartmentTitle { get; set; }
        public List<Signal> Signals { get; set; } = new();
        public bool IsDelete { get; set; }
        
        public int HomeId { get; set; }
        public int? UserId { get; set; }
        public Home Home { get; set; } = null;
        public ApplicationUser ApplicationUser { get; set; } = null;
    }
}
