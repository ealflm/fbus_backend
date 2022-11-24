using System;
using System.ComponentModel.DataAnnotations;

namespace FBus.Business.Utils.CustomAnnotation
{
    public class TimeSpanComparator : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public TimeSpanComparator(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                if (ErrorMessage == null || ErrorMessage.Trim() == "")
                {
                    ErrorMessage = "TimeStart cannot be greater than TimeEnd";
                }

                var currentValue = TimeSpan.Parse(value.ToString());

                var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

                if (property == null)
                    throw new ArgumentException("Property with this name not found");

                if (property.GetValue(validationContext.ObjectInstance) == null)
                    return ValidationResult.Success;

                var comparisonValue = TimeSpan.Parse(property.GetValue(validationContext.ObjectInstance).ToString());

                if (currentValue > comparisonValue)
                    return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}