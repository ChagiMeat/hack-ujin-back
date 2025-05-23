using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XakUjin2025
{
    public class Signal
    {
        [Key]
        public int SignalId { get; set; }
        public string SignalSN { get; set; }
        public bool IsDelete { get; set; }
        public List<Indication> Indications {get; set; } = new();
        public int ApartmentId { get; set; }
        public Apartment Apartment { get; set; } = null;
    }
}
