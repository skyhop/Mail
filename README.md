<a href="https://skyhop.org"><img src="https://app.skyhop.org/assets/images/skyhop.svg" width=200 alt="skyhop logo" /></a>

----

This .NET Core 3.1 library is an abstraction of the approach we use over at Skyhop to send transactional emails to our subscribers. We believe that having a flexible method is vital to the ability to flexibly send emails.

This core principle behind this library is based on several prior blog posts:

- [Rendering Razor views by supplying a model instance](https://corstianboerman.com/2019-05-27/rendering-razor-views-by-supplying-a-model-instance.html)
- [Using the RazorViewToStringRenderer with Asp.Net Core 3](https://corstianboerman.com/2019-12-25/using-the-razorviewtostringrenderer-with-asp-net-core-3.html)

There is a blog post which extensively describes the approach and background of this library [available **over here**](http://corstianboerman.com/2020-01-07/sending-transactional-emails-from-asp-net-core.html).

# Installation
The NuGet hosted package is available. 

[![Nuget](https://img.shields.io/nuget/vpre/Skyhop.Mail?label=Skyhop.Mail)](https://www.nuget.org/packages/Skyhop.Mail)

Install it using the following commands:

**Using the NuGet Package Manager**
```
Install-Package Skyhop.Mail
```

**Using the .NET CLI**
```
dotnet add package Skyhop.Mail
```

---

You should reference this project on your template project (or at least the project which contains your views and view-models). After which there are two changes you will need to make to your `.csproj` file:

1. Set the SDK target to `Microsoft.NET.Sdk.Razor`.
2. Make sure you add the `AddRazorSupportForMvc` element to the file so that it looks like this:

```xml
<PropertyGroup>
    <TargetFramework>netcoreapp3.1</TargetFramework>
    <AddRazorSupportForMvc>true</AddRazorSupportForMvc>
</PropertyGroup>
```

Samples which can be used as a reference can be found in this repository.

# Usage

1. Add the `Skyhop.Mail` project to your template project.
2. Use the `.AddMailDispatcher()` extension method on your `IServiceCollection` as follows. Note that this library expects you to bring your own transport mechanism.

```csharp
services.AddMailDispatcher(builder =>
{
    builder.DefaultFromAddress = new MimeKit.MailboxAddress("Email Support", "support@example.tld");
});
```

3. Add an implementation of `IMailSender` to your `IServiceCollection`. In the example project a simple Smtp implementation is included.
4. Add views and viewmodels to your templating project.
5. Use the DI container to grab a `MailDispatcher` instance. Usage from code can be as follows. After this you can send an email based on the viewmodel as follows:

```csharp
await _mailDispatcher.SendMail(
    data: new ServiceActionModel
    {
        ActionName = "Starting",
        Timestamp = DateTime.UtcNow
    },
    to: new[] { new MailboxAddress("John Doe", "john.doe@example.tld") });
```

# Convention based view loading
If you would like to load all views in `*.Views.dll`'s  you can use an overload `AddMailDispatcher`, this overload enables the extension of `IMvcCoreBuilder`. We created an extension which will find and load all `*.Views.dll` files as application parts:

```csharp
services.AddMailDispatcher(options =>
{
    options.DefaultFromAddress = new MailboxAddress("Email Support", "support@example.tld");
},
builder => builder.AddViewsApplicationParts());
```


# Gotchas
The following limitations are currently available. Feel free to submit a PR to fix one or more of those ☺.

- This library only works with projects which target `netcoreapp3.1`. This is a limitation based on the requirements of the `Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation` dependency.
- It is expected that a view-model is only used once within a view. The code will use the first view it encounters that has the chosen model.
- If your implementation of `IMailSender` is scoped (uses a `DbContext` for example), you can also change the scope of the `MailDispatcher` as needed. By default the `MailDispatcher` is added as a singleton, but using an overload of the `AddMailDispatcher` you can set the `ServiceLifetime` as needed.
