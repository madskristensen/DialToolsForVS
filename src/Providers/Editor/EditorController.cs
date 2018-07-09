using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EnvDTE80;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.TextManager.Interop;

namespace DialControllerTools
{
    internal class EditorController : BaseTextController
    {
        private readonly DTE2 _dte;

        private readonly ICompletionBroker _broker;
        private IWpfTextView _view;
        private delegate string Shift(SnapshotSpan bufferSpan, RotationDirection direction);
        private Dictionary<string, Shift> _dic = new Dictionary<string, Shift>()
        {
            { @"(#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3}))\b", ColorShifter.Shift },
            { @"(\b|\-)[0-9\.]+", NumberShifter.Shift }
        };

        public EditorController(IDialControllerHost host, IVsTextManager textManager, ICompletionBroker completionBroker)
            : base(host, textManager)
        {
            _dte = host.DTE;
            _broker = completionBroker;
        }

        public override string Moniker => EditorControllerProvider.Moniker;

        public override bool CanHandleRotate
        {
            get
            {
                if (!_dte.ActiveWindow.IsDocument())
                    return false;

                _view = GetCurrentTextView();

                return _view != null && _view.HasAggregateFocus;
            }
        }

        public override bool CanHandleClick => true;

        public override bool OnClick()
        {
            _view = GetCurrentTextView();

            if (_view == null || !_view.HasAggregateFocus)
                return false;

            if (_broker.IsCompletionActive(_view))
            {
                _broker.GetSessions(_view)[0].Commit();
            }
            else
            {
                _dte.Commands.ExecuteCommand("Edit.ListMembers");
            }

            return true;
        }

        public override bool OnRotate(RotationDirection direction)
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

            return IntellisenseShifter.TryShift(_view, _broker, direction);
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
