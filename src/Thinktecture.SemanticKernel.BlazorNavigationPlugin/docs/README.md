# Thinktecture SemanticKernel BlazorNavigationPlugin

With this plugin, you can use [SemanticKernel](https://github.com/microsoft/semantic-kernel) in your ASP.NET Core Blazor Application to
navigate your application based on a textual user request.

## Provide information to the model

In order to make good decisions, the LLM needs to know something about your application and the pages it contains.
We leverage the `DescriptionAttribute` to provide this information.:

This is probably the most important step, so we put this before the code to actually use the Plugin 😉

Assume we extended the default `Counter.razor` component from the Blazor template with an additional route that allows
you to set the initial counter value. You can add a description to the component like this:

```csharp
@page "/counter/"
@page "/counter/{currentCount:int}"

@attribute [Description("This is a counter page. You can use it to count things. The initial counter value can be set by specifying the currentCount parameter in the URL.")]
```

The model will pick up both routes and with this description it is capable of simply navigating to the component to count,
but also to navigate to the component and initialize the counter if you tell it that you want to count from, say, 42 on onwards.

When you use code behind, you can also simply add the attribute to the code-behind partial class like this:

```csharp
[Description("This is a form that lets you order Pizza.")]
public partial class PizzaOrder : IComponent
{
}
```


## Setup

First, install this package into your Blazor application:
```cli
dotnet add package Thinkecture.SemanticKernel.BlazorNavigationPlugin --prerelease
``` 

### Application Startup

To quickly get started, add the following code to your Blazor application `Program.cs`:

```csharp
builder.Services.AddBlazorNavigationPlugin();
```

If you use the `ApplicationAssembly` and/or the `AdditionalAssemblies` properties on the Blazor `Router` component, you
need to also provide these to this plugin by using the following code instead: 
```csharp
var applicationAssembly = typeof(Program).Assembly; // or any other main assembly
var additionalAssemblies = new[] { /* List your assemblies with routable components here */  };
builder.Services.AddBlazorNavigationPlugin(applicationAssembly, additionalAssemblies);
```

### Providing the Plugin to the Semantic Kernel

```csharp
// First, build the kernel as usual
var builder = new KernelBuilder()
    .WithLogger(_logger);

if (!String.IsNullOrWhiteSpace(_options.BaseUrl))
{
    builder.WithAzureChatCompletionService(_options.Model, _options.BaseUrl, _options.ApiKey, httpClient: new HttpClient());
}
else
{
    builder.WithOpenAIChatCompletionService(_options.Model, _options.ApiKey, httpClient: new HttpClient());
}

var kernel = builder.Build();


// And then, add this plugin to the kernel:
kernel.AddBlazorNavigationPlugin(_serviceProvider);
```

At the moment the Semantic Kernel is not yet fully included in the ASP.NET Core DI system.
Therefore, you need to provide the `IServiceProvider` to the plugin so that it can resolve the Blazor `NavigationManager` and our `NavigationRouteProvider` that we registered earlier at application startup.

Hopefully in a later iteration of Semantic Kernel we can leverage the normal DI system and remove the depencendy on the `IServiceProvider``.

## Usage

While testing we got the best results with the `StepwisePlanner` from the SemanticKernel team.
With a good system prompt you can advice the model to check the available pages and have it navigate your Blazor application.

This is an example from our demo application:

```csharp
public async Task<string> ExecuteUserRequest(string userInput)
{
    string prompt = @$"
    I am Joshua, an AI assistant agent embedded in an application that is mainly used for Customer Relationship Management, but it also provides some general functionalities that do not have to do with CRM.
    I have partial control over some functionality of the application, provided to me through functions.
    I am able to navigate the application and select screens for the user, and sometimes it may be possible to extract information from the users request and use it to fill in a form after navigating there.
    I probably want to start by listing the avilable screens and their functionalities to check if there is a page available that can help the user to do what he wants.
    I will give my final answer in the same language the user used to ask the question.

    User: {userInput}
    ";
    var planner = new StepwisePlanner(Kernel);
    
    var plan = planner.CreatePlan(prompt);

    _logger.LogInformation("Plan created: {Plan}", plan.Describe());
    var context = await plan.InvokeAsync();

    _logger.LogInformation("Result: {Result}", context.Result);

    return context.Result;
}
```

This will look like this:

![Example of an LLM navigating your Blazor application](./demo.gif)