using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Shell;

using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Input;

namespace DialControllerTools
{
    public interface IDialControllerProvider
    {
        Task<IDialController> TryCreateControllerAsync(IAsyncServiceProvider provider, CancellationToken cancellationToken);
    }

    public abstract class BaseDialControllerProvider : IDialControllerProvider
    {
        [Import]
        private readonly ICompositionService compositionService;

        public async Task<IDialController> TryCreateControllerAsync(IAsyncServiceProvider provider, CancellationToken cancellationToken)
        {
            var controller = await TryCreateControllerAsyncOverride(provider, cancellationToken);
            compositionService.SatisfyImportsOnce(controller);
            return controller;
        }

#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
        protected abstract Task<IDialController> TryCreateControllerAsyncOverride(IAsyncServiceProvider provider, CancellationToken cancellationToken);
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods

        internal static async Task<RadialControllerMenuItem> CreateMenuItemAsync(string moniker, string iconFilePath)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(iconFilePath);

            var stream = RandomAccessStreamReference.CreateFromFile(file);
            return RadialControllerMenuItem.CreateFromIcon(moniker, stream);
        }
    }
}
