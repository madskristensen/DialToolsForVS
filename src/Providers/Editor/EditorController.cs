using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DialToolsForVS
{
    internal class EditorController : IDialController
    {
        private IWpfTextView _view;
        private delegate string Shift(SnapshotSpan bufferSpan, RotationDirection direction);
        private Dictionary<string, Shift> _dic = new Dictionary<string, Shift>()
        {
            { @"(#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3}))\b", ColorShifter.Shift },
            { @"(\b|\-)[0-9\.]+", NumberShifter.Shift }
        };

        public string Moniker => EditorControllerProvider.Moniker;

        public bool CanHandleClick => false;

        public bool CanHandleRotate
        {
            get
            {
                if (!VsHelpers.DTE.ActiveWindow.IsDocument())
                    return false;

                _view = VsHelpers.GetCurentTextView();

                return _view != null && _view.HasAggregateFocus;
            }
        }

        public bool OnClick()
        {
            return false;
        }

        public bool OnRotate(RotationDirection direction)
        {
            if (_view == null)
                return false;

            foreach (string pattern in _dic.Keys)
            {
                if (TryGetMatch(pattern, out SnapshotSpan span))
                {
                    string value = _dic[pattern].Invoke(span, direction);

                    if (string.IsNullOrEmpty(value))
                        continue;

                    UpdateSpan(span, value);

                    return true;
                }
            }

            return false;
        }

        private bool TryGetMatch(string pattern, out SnapshotSpan span)
        {
            SnapshotPoint position = _view.Caret.Position.BufferPosition;
            IWpfTextViewLine line = _view.GetTextViewLineContainingBufferPosition(position);
            MatchCollection matches = Regex.Matches(line.Extent.GetText(), pattern);

            foreach (Match match in matches)
            {
                var matchSpan = new Span(line.Start + match.Index, match.Length);

                if (matchSpan.Start <= position && matchSpan.End >= position)
                {
                    span = new SnapshotSpan(_view.TextBuffer.CurrentSnapshot, matchSpan);
                    return true;
                }
            }

            span = new SnapshotSpan();
            return false;
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
    }
}
