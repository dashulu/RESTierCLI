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

            CommandOption projectName = command.Option("-d|--project-directory",
               " The directory that contains the project to run, default to current directory",
               CommandOptionType.SingleValue);
   
            command.OnExecute(() =>
            {
                string pName = "";
                if (string.IsNullOrEmpty(projectName.Value()))
                {
                    string dir = Directory.GetCurrentDirectory();
                    DirectoryInfo sDir = new DirectoryInfo(dir);
                    FileInfo[] fileArray = sDir.GetFiles();
                    foreach (FileInfo file in fileArray)
                    {
                        if (file.Extension.Equals("sln"))
                        {
                            pName = file.Name;
                            break;
                        }
                    }
                }
                else
                {
                    pName = projectName.Value();
                }

                CmdIISExpress(pName);

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
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();

        }

    }
}
