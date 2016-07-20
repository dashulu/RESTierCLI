using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using System.IO;

namespace RestierCLI
{
    internal class RestierCreateCommand
    {
        public static void Configure(CommandLineApplication command)
        {
            command.Description = "Create a .Net WebApplication from a connnection string of the database";
            command.HelpOption("-h|--help");

            var connection = command.Argument("[connection]",
                "The connection string of the database");
            CommandOption directory = command.Option("-d|--directory", 
                "The directory to store the .NET WebApplication, default to current directory",
                CommandOptionType.SingleValue);
            CommandOption projectName = command.Option("-p|--project-name",
                "The project to be craeted, default to the name of the database",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                string dir;
                string pName = null;
                
                if (string.IsNullOrEmpty(connection.Value))
                {
                    Console.Write("Missing required argument '" + connection.Name + "'");
                    command.ShowHelp();
                    return 0;
                }

                if (string.IsNullOrEmpty(directory.Value()))
                    dir = Directory.GetCurrentDirectory();
                else
                    dir = directory.Value();

                if (!string.IsNullOrEmpty(projectName.Value()))
                    pName = projectName.Value();

                
                var sqlManager = new SQLServerManager(connection.Value);
                if (!sqlManager.connect())
                    return 0;
                if (pName == null)
                    pName = sqlManager.GetDatabaseName();
                var builder = new WebApplicationBuilder(connection.Value, pName, dir);
                builder.Generate();
                return 0;
            });
        }
    }
}
