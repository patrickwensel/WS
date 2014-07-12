using System.ComponentModel.DataAnnotations;

namespace WS.Framework.Validators
{
    public class EmailAttribute : RegularExpressionAttribute
    {
        public EmailAttribute()
            : base(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}")
        {
            ErrorMessage = "Please provide a valid email address";
        }
    }
}