using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using EnvDTE;

using EnvDTE80;

using Windows.UI.Input;

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

            bool isShiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            bool isControlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            bool isAltPressed = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);
            if (debugMode == dbgDebugMode.dbgBreakMode)
            {
                switch ((Control: isControlPressed, Shift: isShiftPressed))
                {
                    // Alt
                    // to have it the same for both modes
                    case (Control: false, Shift: false) when isAltPressed: _dte.Debugger.RunToCursor(); break;
                    // Ctrl + Shift
                    // like Ctrl + Shift  + F5
                    case (Control: true, Shift: true): _dte.Application.ExecuteCommand("Debug.Restart"); break;
                    // Shift
                    // like Shift + F5
                    case (Control: false, Shift: true): _dte.Debugger.Stop(); break;
                    default: _dte.Debugger.Go(); break;
                }
            }
            else if (debugMode == dbgDebugMode.dbgDesignMode)
            {
                switch ((Control: isControlPressed, Shift: isShiftPressed))
                {
                    // Alt
                    // to have it the same for both modes
                    case (Control: false, Shift: false) when isAltPressed:
                        _dte.Application.ExecuteCommand("Debug.RunToCursor");
                        break;
                    // Ctrl
                    // like Ctrl + F5
                    case (Control: true, Shift: false):
                        _dte.Application.ExecuteCommand("Debug.StartWithoutDebugging");
                        break;
                    // Shift
                    // like F10
                    case (Control: false, Shift: true):
                        _dte.Application.ExecuteCommand("Debug.StepOver");
                        break;
                    // Ctrl + Shift
                    // like F11
                    case (Control: true, Shift: true):
                        _dte.Application.ExecuteCommand("Debug.StepInto");
                        break;
                    // like F5
                    default:
                        //.Debugger.Go() does not seem to synchronize the VS Debug toolbar
                        _dte.Application.ExecuteCommand("Debug.Start");
                        break;
                }
            }

            return true;
        }

        public override bool OnRotate(RotationDirection direction)
        {
            dbgDebugMode? debugMode = _dte.Application?.Debugger.CurrentMode;

            // TODO: Find how to determine Historical Debugger enabled
            if (debugMode == dbgDebugMode.dbgBreakMode)
            {
                bool isShiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
                bool isControlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
                bool isAltPressed = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);

                if (direction == RotationDirection.Right)
                {
                    switch ((Control: isControlPressed, Alt: isAltPressed, Shift: isShiftPressed))
                    {
                        // like F10
                        case (Control: false, Alt: false, Shift: false): _dte.Debugger.StepOver(); break;
                        case (Control: false, Alt: true, Shift: false):
                            // TODO: Find the right command
                            _dte.Application.ExecuteCommand("Debug.StepOverNoBreakpoints"); break;
                        // Ctrl
                        // like F11
                        case (Control: true, Alt: false, Shift: false): _dte.Debugger.StepInto(); break;
                        // Shift
                        // like Shift + F11
                        case (Control: _, Alt: false, Shift: true): _dte.Debugger.StepOut(); break;
                        case (Control: _, Alt: true, Shift: true):
                            // TODO: Find the right command
                            _dte.Application.ExecuteCommand("Debug.StepOutNoBreakpoints"); break;
                        // any other
                        default: _dte.Debugger.StepOver(); break;
                    }
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
