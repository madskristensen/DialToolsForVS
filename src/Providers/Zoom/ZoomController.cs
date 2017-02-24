using System;
using System.Collections.Generic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace DialToolsForVS
{
    internal class ZoomController : IDialController
    {
        public string Moniker => ZoomControllerProvider.Moniker;

        public bool CanHandleClick => true;

        public bool CanHandleRotate => true;

        public bool OnClick()
        {
            if (!VsHelpers.DTE.ActiveWindow.IsDocument())
                return false;

            ResetZoomInOpenTextViews();

            return true;
        }

        public bool OnRotate(RotationDirection direction)
        {
            if (direction == RotationDirection.Right)
            {
                return VsHelpers.ExecuteCommand("View.ZoomIn");
            }
            else
            {
                return VsHelpers.ExecuteCommand("View.ZoomOut");
            }
        }

        private void ResetZoomInOpenTextViews()
        {
            IEnumerable<IVsWindowFrame> frames = EnumerateDocumentWindowFrames();

            foreach (IVsWindowFrame frame in frames)
            {
                try
                {
                    IVsTextView nativeView = VsShellUtilities.GetTextView(frame);
                    IWpfTextView view = VsHelpers.GetTextView(nativeView);

                    if (view != null)
                    {
                        view.ZoomLevel = 100;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex);
                }
            }
        }

        private static IEnumerable<IVsWindowFrame> EnumerateDocumentWindowFrames()
        {
            IVsUIShell shell = VsHelpers.GetService<SVsUIShell, IVsUIShell>();

            if (shell != null)
            {

                int hr = shell.GetDocumentWindowEnum(out IEnumWindowFrames framesEnum);

                if (hr == VSConstants.S_OK && framesEnum != null)
                {
                    IVsWindowFrame[] frames = new IVsWindowFrame[1];

                    while (framesEnum.Next(1, frames, out uint fetched) == VSConstants.S_OK && fetched == 1)
                    {
                        yield return frames[0];
                    }
                }
            }
        }
    }
}
