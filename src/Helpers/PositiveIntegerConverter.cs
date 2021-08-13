using System;
using System.ComponentModel;
using System.Globalization;

namespace DialControllerTools.Helpers
{
    public class PositiveIntegerConverter : TypeConverter
    {
        private const int MinVal = 1;
        private const int MaxVal = 20;

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is not string)
            {
                return base.ConvertFrom(context, culture, value);
            }

            int input;
            try
            {
                input = Convert.ToInt32(value);
            }
            catch (FormatException exception)
            {
                throw new FormatException($"The value should be between {MinVal} and {MaxVal}.", exception);
            }

            if (input is < MinVal or > MaxVal)
            {
                throw new FormatException($"The value should be between {MinVal} and {MaxVal}.");
            }

            return input;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }
            if (destinationType == typeof(string))
            {
                int num = Convert.ToInt32(value);
                return num.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}