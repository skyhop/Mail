using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Skyhop.Mail.Internal
{
    internal class WebHostEnvironmentShell : IWebHostEnvironment
    {
        private readonly IHostEnvironment _hostEnvironment;

        public WebHostEnvironmentShell(IHostEnvironment environment)
        {
            _hostEnvironment = environment;
        }

        public IFileProvider WebRootFileProvider
        {
            get => ContentRootFileProvider;
            set => ContentRootFileProvider = value;
        }
        public string WebRootPath
        {
            get => ContentRootPath;
            set => ContentRootPath = value;
        }

        public string ApplicationName
        {
            get => _hostEnvironment.ApplicationName;
            set => _hostEnvironment.ApplicationName = value;
        }

        public IFileProvider ContentRootFileProvider
        {
            get => _hostEnvironment.ContentRootFileProvider;
            set => _hostEnvironment.ContentRootFileProvider = value;
        }

        public string ContentRootPath
        {
            get => _hostEnvironment.ContentRootPath;
            set => _hostEnvironment.ContentRootPath = value;
        }

        public string EnvironmentName
        {
            get => _hostEnvironment.EnvironmentName;
            set => _hostEnvironment.EnvironmentName = value;
        }
    }
}
