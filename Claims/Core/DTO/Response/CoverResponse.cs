namespace Claims.Core.DTO.Response;

public class CoverResponse
{
    
    public Guid Id { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public CoverType Type { get; set; }
    public decimal Premium { get; set; }
}