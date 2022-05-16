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

        public DebugController(RadialControllerMenuItem menuItem, DTE2 dte) : base(menuItem)
        {
            _dte = dte;
            // Switched in provider
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
            _events = dte.Events.DebuggerEvents;
            _events.OnEnterBreakMode += delegate { DialPackage.DialControllerHost.RequestActivation(this); };
            _events.OnEnterDesignMode += delegate { DialPackage.DialControllerHost.ReleaseActivation(); };
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
        }

        public override string Moniker => DebugControllerProvider.Moniker;
        public override bool CanHandleClick => true;
        public override bool CanHandleRotate => true;

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
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
                _dte.Application.ExecuteCommand("Debug.Start"); //.Debugger.Go() does not seem to synchronize the VS Debug toolbar
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
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
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
