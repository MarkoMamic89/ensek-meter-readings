using System.ComponentModel.DataAnnotations;

namespace Ensek.MeterReadings.Api.Models
{
    public class MeterReading
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public DateTime ReadingDateTime { get; set; }

        [RegularExpression("^\\d{5}$")]
        public string ReadingValue { get; set; } = string.Empty;
    }
}
