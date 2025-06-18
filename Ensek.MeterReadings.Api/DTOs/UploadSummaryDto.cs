namespace Ensek.MeterReadings.Api.DTOs
{
    public record UploadSummaryDto(int Successful, int Failed, List<FailureDetail> Failures);

    public record FailureDetail(int RowNumber, string Reason);
}
