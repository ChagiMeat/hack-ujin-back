using System.ComponentModel.DataAnnotations;

namespace XakUjin2025
{
    public class Home
    {
        [Key]
        public int IdHome { get; set; }
        [MaxLength(255)]
        public string? Address { get; set; }
        public List<Apartment> Apartments { get; set; } = new();
        public bool IsDelete { get; set; }
    }
}
