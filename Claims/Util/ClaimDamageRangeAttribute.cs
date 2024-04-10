using System.ComponentModel.DataAnnotations;
using Claims.Domain.Constants;

namespace Claims.Util;

public class ClaimDamageRangeAttribute: ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        try
        {
            var input = (decimal)value!;
          return  CoverConstants.MAX_DAMAGE_COST >= input && input > 0;
        }
        catch (Exception e)
        {
            return false;
        }
    } 
}