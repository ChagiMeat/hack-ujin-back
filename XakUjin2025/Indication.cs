using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XakUjin2025
{
    public class Indication
    {
        [Key]
        public int IndicationId { get; set; }
        public string IndicationName { get; set; }
        public string IndicationLabel {  get; set; }
        public double IndicationValue { get; set; }
        public int SignalId { get; set; }
        public Signal Signal { get; set; } = null;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
