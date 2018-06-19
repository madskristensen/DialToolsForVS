using Microsoft.VisualStudio.Text;
using System;
using System.Globalization;

namespace DialToolsForVS
{
    public static class NumberShifter
    {
        public static string Shift(SnapshotSpan bufferSpan, RotationDirection direction)
        {
            string text = bufferSpan.GetText();

            float delta = GetDelta(text);
            string format = text.IndexOf('.') >= 0 ? "#.#0" : string.Empty;

            if (NumberDecimalPlaces(text) == 1)
                format = "F1";

            if (!double.TryParse(text, out double _number))
                return null;

            if (direction == RotationDirection.Left)
            {
                return (_number - delta).ToString(format, CultureInfo.InvariantCulture);
            }
            else
            {
                return (_number + delta).ToString(format, CultureInfo.InvariantCulture);
            }
        }

        private static float GetDelta(string value)
        {
            int decimals = NumberDecimalPlaces(value);
            if (decimals > 0)
            {
                if (decimals > 1)
                    return 0.01F;
                else
                    return 0.1F;
            }

            return 1F;
        }

        private static int NumberDecimalPlaces(string value)
        {
            int s = value.IndexOf('.') + 1; // the first numbers plus decimal point
            if (s == 0)                     // No decimal point
                return 0;

            return value.Length - s;     //total length minus beginning numbers and decimal = number of decimal points
        }
    }
}
