using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace DialControllerTools
{
    public interface IDialControllerProvider
    {
        Task<IDialController> TryCreateControllerAsync(IDialControllerHost host, IAsyncServiceProvider provider, CancellationToken cancellationToken);
    }
}
