using System;
using DialControllerTools;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

internal class Logger
{
    private IVsOutputWindowPane _pane;
    private readonly IVsOutputWindow _output;

    internal static Logger Instance { get; private set; }

    internal static void Initialize(IVsOutputWindow output)
    {
        Instance = new Logger(output);
    }

    private Logger(IVsOutputWindow output)
    {
        this._output = output;
    }

    public void Log(object message)
    {
        if (DialPackage.Options.ShowLogInOutput)
        {
            try
            {
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
                if (EnsurePane())
                {
                    _pane.OutputString($"{DateTime.Now}: {message}{Environment.NewLine}");
                }
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }
    }

    private bool EnsurePane()
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        if (_pane == null)
        {
            var guid = Guid.NewGuid();
            _output.CreatePane(ref guid, DialControllerTools.Vsix.Name, 1, 1);
            _output.GetPane(ref guid, out _pane);
        }

        return _pane != null;
    }
}