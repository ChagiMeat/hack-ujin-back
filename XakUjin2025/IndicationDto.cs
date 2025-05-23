namespace XakUjin2025
{
    public class IndicationDto
    {
        public int IndicationId { get; set; }
        public string IndicationName { get; set; }
        public string IndicationLabel { get; set; }
        public double IndicationValue { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
