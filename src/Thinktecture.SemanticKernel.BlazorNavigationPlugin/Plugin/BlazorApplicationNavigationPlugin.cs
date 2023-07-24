using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Thinktecture.SemanticKernel.BlazorNavigationPlugin.Plugin;

public class BlazorApplicationNavigationPlugin
{
	private readonly ILogger _logger;
	private readonly NavigationManager _navigationManager;
	private readonly NavigationRouteProvider _routeProvider;

	public BlazorApplicationNavigationPlugin(ILogger<BlazorApplicationNavigationPlugin> logger,
		NavigationRouteProvider routeProvider,
		NavigationManager navigationManager)
	{
		_logger = logger;
		_routeProvider = routeProvider;
		_navigationManager = navigationManager;
	}

	[SKFunction]
	[Description(
		"Provides a list of the screens that are available in the application with a description of what they do. A screen can be navigated to using different routes, some of which may have parameters. If a page has multiple routes, any of them can be used. If you can specify parameters in one of the routes by replacing the {parameterName:Type} part of the route with a known value, the route with the most known parameters should be preferred. Example: '{carPart:string}' could be 'Tire' and '{temperatureCelsius:int}' would be '32'.")]
	public Task<string> GetScreensAsync()
	{
		_logger.LogDebug("Getting screens");

		return Task.FromResult(_routeProvider.PageDescriptionsJson);
	}

	[SKFunction]
	[Description("Navigates the application to the specified screen.")]
	public Task<string> NavigateToAsync(
		[Description(
			"The route of the screen to navigate to. All parameter placeholders in the route must already be replaced with the values.")]
		string screenRoute
	)
	{
		_logger.LogDebug("Navigating to route {Route}", screenRoute);
		_navigationManager.NavigateTo(screenRoute);

		return Task.FromResult($"Application navigated to screen '{_navigationManager.Uri}'.");
	}
}
