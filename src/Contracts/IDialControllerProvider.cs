using System.Threading;
using System.Threading.Tasks;

namespace DialToolsForVS
{
    public interface IDialControllerProvider
    {
        Task<IDialController> TryCreateControllerAsync(IDialControllerHost host, CancellationToken cancellationToken);
    }
}
