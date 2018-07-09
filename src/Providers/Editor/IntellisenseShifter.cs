using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Windows.Forms;

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
                Logger.Instance.Log(ex);
            }

            return false;
        }
    }
}
