﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace DialControllerTools
{
    public abstract class BaseController : IDialController
    {
        public abstract string Moniker { get; }

        public virtual bool CanHandleClick => false;

        public virtual bool CanHandleRotate => false;

        public virtual void OnActivate()
        { }

        public virtual bool OnClick()
        {
            return false;
        }

        public virtual bool OnRotate(RotationDirection direction)
        {
            return false;
        }
    }
}
