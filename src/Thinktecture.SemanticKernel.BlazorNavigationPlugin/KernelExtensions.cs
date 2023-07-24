using Microsoft.Extensions.DependencyInjection;
using Thinktecture.SemanticKernel.BlazorNavigationPlugin.Plugin;


// ReSharper disable once CheckNamespace; Justification: Extension should be available without additional using directives
namespace Microsoft.SemanticKernel;

public static class KernelExtensions
{
	public static IKernel AddBlazorNavigationPlugin(this IKernel kernel, IServiceProvider serviceProvider)
	{
		kernel.ImportSkill(serviceProvider.GetRequiredService<BlazorApplicationNavigationPlugin>());

		return kernel;
	}
}
