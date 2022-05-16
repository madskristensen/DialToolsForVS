using System;
using System.Windows.Forms;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;

namespace DialControllerTools
{
    public static class IntellisenseShifter
    {
        public static bool TryShift(ITextView view, ICompletionBroker broker, RotationDirection direction)
        {
            try
            {
                string key = direction == RotationDirection.Right ? "{DOWN}" : "{UP}";

                if (!broker.IsCompletionActive(view))
                {
                    ICompletionSession session = broker.TriggerCompletion(view);

                    if (session == null || !session.IsStarted)
                        return false;

                    Completion active = session.CompletionSets?[0]?.SelectionStatus?.Completion;

                    if (active != null)
                    {
                        SendKeys.SendWait(key);
                        session.Commit();

                        return true;
                    }
                }
                else
                {
                    SendKeys.Send(key);
                    return true;
                }
            }
            catch (Exception ex)
            {
                var outputPane = ThreadHelper.JoinableTaskContext.Factory.Run(DialPackage.GetOutputPaneAsync);
                outputPane.WriteLine("Intellisense shifter failed");
                outputPane.WriteLine(ex.ToString());
            }

            return false;
        }
    }
}
