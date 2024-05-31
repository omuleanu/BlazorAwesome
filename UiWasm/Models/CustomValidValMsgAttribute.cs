using System.ComponentModel.DataAnnotations;

namespace UiWasm.Models
{
    /// <summary>
    /// Will set the property Value as the validation message,
    /// Validation passes when property Value is null
    /// </summary>
    public class CustomValidValMsgAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is null) return true;

            ErrorMessage = (string)value;
            return false;
        }
    }
}