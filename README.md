<a href="https://skyhop.org"><img src="https://app.skyhop.org/assets/images/skyhop.svg" width=200 alt="skyhop logo" /></a>

----

This .NET Core 3.1 library is an abstraction of the approach we use over at Skyhop to send transactional emails to our subscribers. We believe that having a flexible method is vital to the ability to flexibly send emails.

This core principle behind this library is based on several prior blog posts:

- [Rendering Razor views by supplying a model instance](https://corstianboerman.com/2019-05-27/rendering-razor-views-by-supplying-a-model-instance.html)
- [Using the RazorViewToStringRenderer with Asp.Net Core 3](https://corstianboerman.com/2019-12-25/using-the-razorviewtostringrenderer-with-asp-net-core-3.html)

# Installation

NuGet packages will be made available shortly. Until that time, just clone this repository.

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
services.AddMailDispatpcher(builder =>
{
    builder.DefaultFromAddress = new MimeKit.MailboxAddress("Email Support", "support@example.tld");

    builder.MailSender = async message =>
    {
        using (var client = new SmtpClient())
        {
            await client.ConnectAsync("mail.example.tld", 587, false);
            await client.AuthenticateAsync("support@example.tld", "**ExamplePassword**");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    };
});
```

3. Add views and viewmodels to your templating project.
4. Use the DI container to grab a `MailDispatcher` instance. Usage from code can be as follows. After this you can send an email based on the viewmodel as follows:

```csharp
await _mailDispatcher.SendMail(
    data: new ServiceActionModel
    {
        ActionName = "Starting",
        Timestamp = DateTime.UtcNow
    },
    to: new[] { new MailboxAddress("John Doe", "john.doe@example.tld") });
```

# Gotchas
The following limitations are currently available. Feel free to submit a PR to fix one or more of those ☺.

- This library only works with projects which target `netcoreapp3.1`. This is a limitation based on the requirements of the `Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation` dependency.
- Instantiation of the `MailDispatcher` happens from it's own dependency container. This is due to a dependency on the `WebHostBuilder`, and to enable usage outside of an asp.net core application.
- During instantiation we scan the output folder for assemblies ending in `.Views.dll`, and add these as an application part.
- It is expected that a view-model is only used once within a view. We do not have code resolving multiple usages of the same viewmodel, and an exception will be thrown.
