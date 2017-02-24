using Microsoft.VisualStudio.Text;
using System.Drawing;
using System.Globalization;

namespace DialToolsForVS
{
    public class ColorShifter
    {
        public static string Shift(SnapshotSpan bufferSpan, RotationDirection direction)
        {
            string text = bufferSpan.GetText();

            int argb = int.Parse(text.Substring(1), NumberStyles.HexNumber);
            var color = Color.FromArgb(argb);

            double factor = direction == RotationDirection.Left ? -3 : 3;

            Color newColor = AdjustBrightness(color, factor);

            return ConvertToHex(newColor).ToLowerInvariant();
        }

        private static string ConvertToHex(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        public static Color AdjustBrightness(Color color, double darkenAmount)
        {
            var hslColor = new HSLColor(color);
            hslColor.Luminosity += darkenAmount; // 0 to 1
            return hslColor;
        }
    }
}
