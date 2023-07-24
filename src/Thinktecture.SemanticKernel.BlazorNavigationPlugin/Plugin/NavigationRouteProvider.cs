using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using IComponent = Microsoft.AspNetCore.Components.IComponent;

namespace Thinktecture.SemanticKernel.BlazorNavigationPlugin.Plugin;

public class NavigationRouteProvider
{
	private readonly IEnumerable<Assembly> _assemblies;

	private string? _routeString = null;

	// All components that are routable (aka Pages)
	private IEnumerable<Type> RoutableComponents => _assemblies
		.SelectMany(a => a.ExportedTypes)
		.Where(t => typeof(IComponent).IsAssignableFrom(t) && t.IsDefined(typeof(RouteAttribute)));

	// Page descriptions and routes from the routable components and additional attributes
	private IEnumerable<PageDescription> PageDescriptions => RoutableComponents
		.Select(t => new PageDescription(
			t.Name,
			t.GetCustomAttributes<RouteAttribute>().Select(a => a.Template).ToArray(),
			t.GetCustomAttribute<DescriptionAttribute>()?.Description
		));

	public string PageDescriptionsJson => _routeString ??= JsonSerializer.Serialize(PageDescriptions);

	public NavigationRouteProvider(IEnumerable<Assembly> assemblies)
	{
		_assemblies = assemblies;
	}
}
