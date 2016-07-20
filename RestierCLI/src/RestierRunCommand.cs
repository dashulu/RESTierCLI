using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestierCLI
{
    class RestierRunCommand
    {
        
        public static void Configure(CommandLineApplication command)
        {
            command.Description = "Run a .Net WebApplication";
            command.HelpOption("-h|--help");

            CommandOption projectDir = command.Option("-d|--project-directory",
               " The directory that contains the project to run, default to current directory",
               CommandOptionType.SingleValue);
   
            command.OnExecute(() =>
            {
                string pDir = "";
                if (string.IsNullOrEmpty(projectDir.Value()))
                {
                    pDir = Directory.GetCurrentDirectory();
                    
                }
                else
                {
                    pDir = projectDir.Value();
                }
                if (!File.Exists(pDir + "\\" + ".vs\\config\\applicationhost.config"))
                {
                    Console.Write("Can't find the configration file '" + pDir + "\\" + ".vs\\config\\applicationhost.config' \n" +
                        "Make sure you have set the correct project directory");
                    command.ShowHelp();
                    return 0;
                }

                CmdIISExpress(pDir);

                return 0;
            });
        }

        // execute the msbuild to build a project
        private static void CmdIISExpress(string projectDir)
        {

            Process p = new Process();

            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/c \"" + Config.IISExpressPath + "iisexpress.exe\" " +
                 "/config:" + projectDir + "\\" + ".vs\\config\\applicationhost.config";
            Console.WriteLine(p.StartInfo.Arguments);
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();

        }

    }
}
