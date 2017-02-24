using System;
using System.ComponentModel.Composition;

namespace DialToolsForVS
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DialControllerProviderAttribute : ExportAttribute
    {
        public DialControllerProviderAttribute(int order) : base(typeof(IDialControllerProvider))
        {
            Order = order;
        }

        public int Order { get; }
    }

    public interface IDialMetadata
    {
        int Order { get; }
    }
}
