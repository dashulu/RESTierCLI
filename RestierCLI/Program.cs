using System;
using Microsoft.Data.Entity.Design.CodeGeneration;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using Microsoft.Data.Entity.Design.VersioningFacade;
using Microsoft.Data.Entity.Design.VisualStudio.ModelWizard.Engine;
using System.Data;
using Microsoft.Data.Entity.Design.VersioningFacade.ReverseEngineerDb;
using System.IO;
using System.Xml;
using Microsoft.Extensions.CommandLineUtils;

namespace RestierCLI
{
    class Program
    {
        private const string connectiongString = "Server=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\t-qiche\\Downloads\\AdventureWorksLT2012_Data.mdf;Integrated Security=True;Trusted_Connection=True;";
        static int Main(string[] args)
        {
            /*
            ProjectManager p = new ProjectManager(connectiongString, "C:\\Users\\t-qiche\\Documents\\Visual Studio 2015\\Projects", "Ad12");
            if(p.CreateProject())
                Console.WriteLine("successed.");
                */
            try
            {
               /*
                CommandLineApplication t = ExecuteCommand.Create();
               
                string[] c = { "create", connectiongString, "-n", "HelloWorld", "-d", "C:\\Users\\t-qiche\\Downloads" };
                for (int i = 0; i < c.Length; i++)
                    Console.WriteLine(c[i]);
                t.Execute(c);
                for (int i = 0; i < args.Length; i++)
                    Console.WriteLine(args[i]);
                    */
                CommandLineApplication t2 = ExecuteCommand.Create();
                t2.Execute(args);
             
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
         
            return 0;
        }
    }

}
