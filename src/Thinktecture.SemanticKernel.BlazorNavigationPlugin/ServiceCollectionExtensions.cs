using System.Reflection;
using Thinktecture.SemanticKernel.BlazorNavigationPlugin.Plugin;

// ReSharper disable once CheckNamespace; Justification: Extensions should be available without additional using directives
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddBlazorNavigationPlugin(this IServiceCollection services,
		Assembly? applicationAssembly = null,
		IEnumerable<Assembly>? additionalAssemblies = default)
	{
		var assemblies = new[] { applicationAssembly ?? Assembly.GetCallingAssembly() }
			.Concat(additionalAssemblies ?? Enumerable.Empty<Assembly>());

		return services
			.AddSingleton(new NavigationRouteProvider(assemblies))
			.AddScoped<BlazorApplicationNavigationPlugin>();
	}
}
