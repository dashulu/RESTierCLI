using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace RestierCLI
{
    class ExecuteCommand
    {
        // Currently we just set the version to "1.0.0"
        private const string versionString = "1.0.0";

        /// <summary>
        /// Create a RESTier CommandLineApplication
        /// </summary>
        /// <returns>Return the RESTier CommandLineApplication</returns>
        public static CommandLineApplication Create()
        {
            var app = new CommandLineApplication()
            {
                Name = "restier",
                FullName = "RESTier CLI Commands",
                Description = "RESTier CLI Commands for building standardized, OData V4 based RESTful services on .NET platform"
            };

            app.HelpOption("-h|--help");
            app.VersionOption("-v|--version", versionString);

            app.Command("create", c => RestierCreateCommand.Configure(c));
            app.Command("build", c => RestierBuildCommand.Configure(c));
            app.Command("run", c => RestierRunCommand.Configure(c));
            return app;
        }
    }
}
