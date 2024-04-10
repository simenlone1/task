using Claims.Util;

namespace Claims.Core.DTO.Requests;

public class CreateCoverRequest
{
        
    [CoverDate(ErrorMessage="Date is in an invalid format or set in the past")]
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public CoverType Type { get; set; }
}