using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace DialControllerTools
{
    internal class DebugController : BaseController
    {
        private readonly DTE2 _dte;
        private readonly DebuggerEvents _events;

        public DebugController(IDialControllerHost host)
        {
            _dte = host.DTE;
            _events = _dte.Events.DebuggerEvents;
            _events.OnEnterBreakMode += delegate { host.RequestActivation(_dte.MainWindow, Moniker); };
            _events.OnEnterDesignMode += delegate { host.ReleaseActivation(_dte.MainWindow); };
        }

        public override string Moniker => DebugControllerProvider.Moniker;
        public override bool CanHandleClick => true;
        public override bool CanHandleRotate => true;

        public override bool OnClick()
        {
            dbgDebugMode? debugMode = _dte.Application?.Debugger.CurrentMode;

            if (!debugMode.HasValue)
                return false;

            if (debugMode == dbgDebugMode.dbgBreakMode)
            {
                _dte.Application.Debugger.StepInto();
            }
            else if (debugMode == dbgDebugMode.dbgDesignMode)
            {
                _dte.Debugger.Go();
            }

            return true;
        }

        public override bool OnRotate(RotationDirection direction)
        {
            dbgDebugMode? debugMode = _dte.Application?.Debugger.CurrentMode;

            if (debugMode == dbgDebugMode.dbgBreakMode)
            {
                if (direction == RotationDirection.Right)
                {
                    _dte.Application.Debugger.StepOver();
                }
                else
                {
                    _dte.Application.Debugger.StepOut();
                }
            }
            else if (debugMode == dbgDebugMode.dbgDesignMode)
            {
                if (direction == RotationDirection.Right)
                {
                    MoveToBreakpoint((s, bs, d) =>
                    {
                        return bs.FirstOrDefault(b =>
                            string.Equals(b.File, d.FullName, StringComparison.InvariantCultureIgnoreCase)
                            && b.FileLine > s.CurrentLine
                            || string.Compare(b.File, d.FullName, true) > 0) ?? bs.FirstOrDefault();
                    });
                }
                else
                {
                    MoveToBreakpoint((s, bs, d) =>
                    {
                        return bs.LastOrDefault(b =>
                            string.Equals(b.File, d.FullName, StringComparison.InvariantCultureIgnoreCase)
                            && b.FileLine < s.CurrentLine
                            || string.Compare(b.File, d.FullName, true) < 0) ?? bs.LastOrDefault();
                    });
                }

            }
            return true;
        }

        private void MoveToBreakpoint(Func<TextSelection, IEnumerable<Breakpoint>, Document, Breakpoint> findBreakpoint)
        {
            var selection = ((TextSelection)_dte.ActiveDocument.Selection);

            IOrderedEnumerable<Breakpoint> breakpoints = _dte.Debugger.Breakpoints.OfType<Breakpoint>().OrderBy(b => b, new FileOrderer());

            Breakpoint breakpoint = findBreakpoint(selection, breakpoints, _dte.ActiveDocument);

            if (breakpoint != null)
            {
                var s = _dte.Documents.OfType<Document>()
                    .FirstOrDefault(d => string.Equals(d.FullName, breakpoint.File, StringComparison.InvariantCultureIgnoreCase))
                    .Selection as TextSelection;

                _dte.Documents.Open(breakpoint.File);
                s.MoveToLineAndOffset(breakpoint.FileLine, breakpoint.FileColumn);
            }
        }
    }

    class FileOrderer : IComparer<Breakpoint>
    {
        public int Compare(Breakpoint x, Breakpoint y)
        {
            if (x.File == y.File)
            {
                return x.FileLine.CompareTo(y.FileLine);
            }
            else
            {
                return string.Compare(x.File, y.File);
            }
        }
    }
}
