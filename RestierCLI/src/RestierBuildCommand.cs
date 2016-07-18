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
    internal class RestierBuildCommand
    {
        private const string MSBuildDir = "C:\\Windows\\Microsoft.NET\\Framework64\\v4.0.30319\\";
        public static void Configure(CommandLineApplication command)
        {
            command.Description = "Build a .Net WebApplication";
            command.HelpOption("-h|--help");

            CommandOption projectName = command.Option("-p|--project-name",
               " The project to build, default to the first project in current directory",
               CommandOptionType.SingleValue);
            CommandOption buildSetting = command.Option("--build-setting",
               "Parameter for msbuild when complile the project",
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
 
                Console.WriteLine(CmdMSBuild(pName, buildSetting.Value()));
                
                return 0;
            });
        }

        // execute the msbuild to build a project
        private static string CmdMSBuild(string projectName, string buildSetting)
        {

            Process p = new Process();

            p.StartInfo.FileName = "cmd.exe";

            p.StartInfo.UseShellExecute = false;

            p.StartInfo.RedirectStandardInput = true;

            p.StartInfo.RedirectStandardOutput = true;

            p.StartInfo.RedirectStandardError = true;

            p.StartInfo.CreateNoWindow = true;

            p.Start();

            p.StandardInput.WriteLine(MSBuildDir + "MSBuild.exe " + projectName + 
                (string.IsNullOrEmpty(buildSetting) ? "" : " " + buildSetting));
            p.StandardInput.WriteLine("exit");
            string strRst = p.StandardOutput.ReadToEnd();
            p.Close();
            return ignoreLastLineContainWordExit(strRst);
        }

        // Ignore the last line that contains the word "exit"
        // Used to delete the output when execute "exit" command
        private static string ignoreLastLineContainWordExit(string str)
        {
            int index = str.LastIndexOf('\n', str.LastIndexOf("exit"));
            if (index < 0) // unlikely to happen
                return str;
            return str.Substring(0, index);
        }
        
        
    }
}
