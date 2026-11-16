using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text.Editor;

namespace DialControllerTools;

/// <summary>
/// Finds the <see cref="T:Microsoft.Web.WebView2.Wpf.WebView2"/> control that a window of Visual Studio shows and changes its zoom
/// factor. The Zoom item of the Dial uses it when the active window holds web content, such as the standalone Markdown preview.
/// <para>The WebView2 SDK is not referenced on purpose. Visual Studio runs its own copy of <c>Microsoft.Web.WebView2.Wpf</c>, which
/// can be missing or older in the oldest Visual Studio that the extension supports, and the identity of its types depends on the
/// binding redirects of the host. So the controls are recognized by the full name of their type, and the zoom is read and written
/// through <see cref="F:Microsoft.Web.WebView2.Wpf.WebView2.ZoomFactorProperty"/>, the public dependency property that both WPF
/// controls of the SDK declare. When a future SDK renames a type or the property, the web view is not found any more and the Zoom
/// item falls back to the zoom commands of the editor.</para>
/// <para>The zoom keys of the browser are no option: the host can turn them off
/// (<see cref="P:Microsoft.Web.WebView2.Core.CoreWebView2Settings.AreBrowserAcceleratorKeysEnabled"/>), and Visual Studio binds the
/// same keys to its own commands before the browser sees them.</para>
/// <para>The references to the SDK are documentation IDs (<c>T:</c>, <c>F:</c>, <c>P:</c>): the compiler cannot resolve a type that
/// the project does not reference and passes such an ID through without a check, so compare an ID with the SDK when you change one.</para>
/// </summary>
internal static class WebViewZoom
{
    private const string WebViewTypeName = "Microsoft.Web.WebView2.Wpf.WebView2";
    private const string CompositionControlTypeName = "Microsoft.Web.WebView2.Wpf.WebView2CompositionControl";
    private const string ZoomFactorFieldName = "ZoomFactorProperty";

    // The zoom range of WebView2: it normalizes everything outside of it.
    private const double MinZoomFactor = 0.25;
    private const double MaxZoomFactor = 5.0;
    private const double DefaultZoomFactor = 1.0;

    // The dependency property per type of control, with null for a type that is no web view: a search asks about every element of a
    // visual tree. Only the UI thread uses it.
    private static readonly Dictionary<Type, DependencyProperty> ZoomFactorProperties = new Dictionary<Type, DependencyProperty>();

    /// <summary>
    /// Finds the web view that <paramref name="root"/> is or contains.
    /// </summary>
    /// <param name="root">The content of a window, or <see langword="null"/>.</param>
    /// <returns>The first web view that is shown, or <see langword="null"/> when there is none.</returns>
    internal static DependencyObject Find(DependencyObject root)
    {
        // FindDescendant walks the visual tree and throws for an element that is not a visual.
        if (root is not Visual)
            return null;

        return IsShownWebView(root) ? root : root.FindDescendant(IsShownWebView);
    }

    /// <summary>
    /// Zooms a web view in or out by one step, the same step as the zoom commands of the text editor.
    /// </summary>
    /// <param name="webView">A web view that <see cref="Find"/> returned.</param>
    /// <param name="direction">The direction of the rotation of the Dial.</param>
    internal static void Zoom(DependencyObject webView, RotationDirection direction)
    {
        DependencyProperty property = GetZoomFactorProperty(webView.GetType());

        if (property is null || webView.GetValue(property) is not double current)
            return;

        double zoomFactor = direction == RotationDirection.Right
            ? current * ZoomConstants.ScalingFactor
            : current / ZoomConstants.ScalingFactor;

        // SetCurrentValue, as the SDK does itself: a binding of the host to the zoom stays in place.
        webView.SetCurrentValue(property, Math.Min(MaxZoomFactor, Math.Max(MinZoomFactor, zoomFactor)));
    }

    /// <summary>
    /// Sets the zoom of a web view back to 100%.
    /// </summary>
    /// <param name="webView">A web view that <see cref="Find"/> returned.</param>
    internal static void Reset(DependencyObject webView)
    {
        DependencyProperty property = GetZoomFactorProperty(webView.GetType());

        if (property is not null)
            webView.SetCurrentValue(property, DefaultZoomFactor);
    }

    // A hidden web view stays in the visual tree: a collapsed preview margin, or a tab that is not the selected one.
    private static bool IsShownWebView(DependencyObject element)
        => element is UIElement { IsVisible: true } && GetZoomFactorProperty(element.GetType()) is not null;

    private static DependencyProperty GetZoomFactorProperty(Type type)
    {
        if (ZoomFactorProperties.TryGetValue(type, out DependencyProperty cached))
            return cached;

        DependencyProperty property = null;

        // Visual Studio, like an extension, hosts the web view in a subclass of one of the two controls of the SDK.
        for (Type current = type; current is not null && property is null; current = current.BaseType)
        {
            if (current.FullName is WebViewTypeName or CompositionControlTypeName)
                property = ReadZoomFactorProperty(current);
        }

        ZoomFactorProperties[type] = property;
        return property;
    }

    private static DependencyProperty ReadZoomFactorProperty(Type webViewType)
    {
        var property = webViewType.GetField(ZoomFactorFieldName, BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as DependencyProperty;

        // Only the writable double that the controls declare is the zoom; anything else is a property that this class does not know.
        return property is { ReadOnly: false } && property.PropertyType == typeof(double) ? property : null;
    }
}
