using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DialToolsForVS
{
    internal class NumbersController : IDialController
    {
        private IWpfTextView _view;
        private Span _span;
        private double _number;

        public string Moniker => PredefinedMonikers.Editor;
        public Specificity Specificity => Specificity.CaretPosition;
        public bool CanHandleClick => false;

        public bool CanHandleRotate
        {
            get
            {
                if (!VsHelpers.DTE.ActiveWindow.IsDocument())
                    return false;

                _view = VsHelpers.GetCurentTextView();

                if (!_view.HasAggregateFocus)
                    return false;

                SnapshotPoint position = _view.Caret.Position.BufferPosition;
                IWpfTextViewLine line = _view.GetTextViewLineContainingBufferPosition(position);

                if (line.Extent.IsEmpty)
                    return false;

                MatchCollection matches = Regex.Matches(line.Extent.GetText(), @"(?:[\s\(,\[])(?<number>-?[\d\.]+)");

                foreach (Match match in matches)
                {
                    Group group = match.Groups["number"];
                    var matchSpan = new Span(line.Start + group.Index, group.Length);

                    if (matchSpan.Start <= position && matchSpan.End >= position)
                    {
                        string text = _view.TextBuffer.CurrentSnapshot.GetText(matchSpan);

                        if (double.TryParse(text, out _number))
                        {
                            _span = matchSpan;
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public void OnClick(DialEventArgs e)
        { }

        public void OnRotate(RotationDirection direction, DialEventArgs e)
        {
            if (_view == null || _span == null || _span.IsEmpty)
                return;

            var bufferSpan = new SnapshotSpan(_view.TextBuffer.CurrentSnapshot, _span);
            string text = bufferSpan.GetText();

            float delta = GetDelta(text);
            string format = text.Contains(".") ? "#.#0" : string.Empty;

            if (NumberDecimalPlaces(text) == 1)
                format = "F1";

            if (direction == RotationDirection.Left)
            {
                e.Action = "Decrease value";
                UpdateSpan(bufferSpan, (_number - delta).ToString(format, CultureInfo.InvariantCulture));
            }
            else
            {
                e.Action = "Increase value";
                UpdateSpan(bufferSpan, (_number + delta).ToString(format, CultureInfo.InvariantCulture));
            }

            e.Handled = true;
        }

        private static void UpdateSpan(SnapshotSpan span, string result)
        {
            if (result.Length > 1)
                result = result.TrimStart('0');

            using (ITextEdit edit = span.Snapshot.TextBuffer.CreateEdit())
            {
                edit.Replace(span, result);
                edit.Apply();
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
            int s = value.IndexOf(".", StringComparison.CurrentCulture) + 1; // the first numbers plus decimal point
            if (s == 0)                     // No decimal point
                return 0;

            return value.Length - s;     //total length minus beginning numbers and decimal = number of decimal points
        }
    }
}
