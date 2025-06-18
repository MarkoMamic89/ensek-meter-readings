namespace Ensek.MeterReadings.Api.DTOs
{
    public record MeterReadingCsvRow(
        int AccountId,
        DateTime ReadingDateTime,
        string ReadingValue
    );
}
