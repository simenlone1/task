using System.ComponentModel.DataAnnotations;

namespace Claims.Util;

public class CoverDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        try
        {
            var input = (DateOnly)value!;
            var currentDate = DateTimeOffset.UtcNow; 
            return new DateOnly(currentDate.Year, currentDate.Month, currentDate.Day) <= input;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}