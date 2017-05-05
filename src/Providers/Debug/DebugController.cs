using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;

namespace DialToolsForVS
{
    internal class DebugController : BaseController
    {
        private DebuggerEvents _events;

        public DebugController(IDialControllerHost host)
        {
            _events = VsHelpers.DTE.Events.DebuggerEvents;
            _events.OnEnterBreakMode += delegate { host.RequestActivation(Moniker); };
            _events.OnEnterDesignMode += delegate { host.ReleaseActivation(); };
        }

        public override string Moniker => DebugControllerProvider.Moniker;
        public override bool CanHandleClick => true;
        public override bool CanHandleRotate => true;

        public override bool OnClick()
        {
            dbgDebugMode? debugMode = VsHelpers.DTE.Application?.Debugger.CurrentMode;

            if (!debugMode.HasValue)
                return false;

            if (debugMode == dbgDebugMode.dbgBreakMode)
            {
                VsHelpers.DTE.Application.Debugger.StepInto();
            }
            else if (debugMode == dbgDebugMode.dbgDesignMode)
            {
                VsHelpers.DTE.Debugger.Go();
            }

            return true;
        }

        public override bool OnRotate(RotationDirection direction)
        {
            dbgDebugMode? debugMode = VsHelpers.DTE.Application?.Debugger.CurrentMode;

            if (debugMode == dbgDebugMode.dbgBreakMode)
            {
                if (direction == RotationDirection.Right)
                {
                    VsHelpers.DTE.Application.Debugger.StepOver();
                }
                else
                {
                    VsHelpers.DTE.Application.Debugger.StepOut();
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
            EnvDTE80.DTE2 dte = VsHelpers.DTE;

            var selection = ((TextSelection)dte.ActiveDocument.Selection);

            IOrderedEnumerable<Breakpoint> breakpoints = dte.Debugger.Breakpoints.OfType<Breakpoint>().OrderBy(b => b, new FileOrderer());

            Breakpoint breakpoint = findBreakpoint(selection, breakpoints, dte.ActiveDocument);

            if (breakpoint != null)
            {
                var s = dte.Documents.OfType<Document>()
                    .FirstOrDefault(d => string.Equals(d.FullName, breakpoint.File, StringComparison.InvariantCultureIgnoreCase))
                    .Selection as TextSelection;

                dte.Documents.Open(breakpoint.File);
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
