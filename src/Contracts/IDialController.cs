namespace DialControllerTools;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;

using Windows.UI.Input;

public interface IDialController
{
    string Moniker { get; }
    RadialControllerMenuItem MenuItem { get; }
    bool CanHandleClick { get; }
    bool CanHandleRotate { get; }
    bool IsHapticFeedbackEnabled { get; }
    bool OnClick();
    bool OnRotate(RotationDirection direction);
    void OnActivate();
}

internal static class DialControllerExtensions
{
    internal static async Task<ImmutableArray<IDialController>> GetControllersAsync(this IAsyncServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var compositionService = await serviceProvider.GetServiceAsync<SComponentModel, IComponentModel>(cancellationToken);
        var providers = compositionService.DefaultExportProvider.GetExports<IDialControllerProvider, IDialMetadata>();
        var tasks = providers
            //this is the true add to the menu: TryCreateControllerAsync calls back to AddMenuItemAsync
            .Select(async provider =>
            {
                try
                {
                    var controller = await provider.Value.TryCreateControllerAsync(serviceProvider, cancellationToken);
                    return (Controller: controller, provider.Metadata.Order);
                }
                catch (Exception ex)
                {
                    var outputPane = await DialPackage.GetOutputPaneAsync();
                    var providerName = provider.GetType().GenericTypeArguments.First().GetType().Name;
                    await outputPane.WriteLineAsync($"Error creating controller: {providerName}");
                    await outputPane.WriteLineAsync(ex.ToString());
                    return (Controller: null, provider.Metadata.Order);
                }
            }).ToList();

        var controllers = await Task.WhenAll(tasks);
        return controllers
                .Where(result => result.Controller != null)
                .OrderBy(result => result.Order)
                .Select(result => result.Controller)
                .ToImmutableArray();
    }

}
