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
            command.Description = "Create";
            command.HelpOption("-h|--help");

            var connection = command.Argument("[connection]",
                "The connection string of the database");
            CommandOption directory = command.Option("-d|--directory", 
                "The directory to store the .NET WebApplication, default to current directory",
                CommandOptionType.SingleValue);
            CommandOption projectName = command.Option("-n|--name",
                "The project name to be craeted, default to the database name",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                string dir;
                string pName;
                if (string.IsNullOrEmpty(connection.Value))
                {
                    Console.WriteLine("Missing required argument '" + connection.Name + "'");
                    command.ShowHelp();
                    return 0;
                }
                if (string.IsNullOrEmpty(directory.Value()))
                    dir = Directory.GetCurrentDirectory();
                else
                    dir = directory.Value();

                var sqlManager = new SQLServerManager(connection.Value);
                if (!sqlManager.connect())
                    return 0;

                pName = sqlManager.GetDatabaseName();
                var builder = new WebApplicationBuilder(connection.Value, pName, dir);
                builder.Generate();
                return 0;
            });
        }
    }
}
