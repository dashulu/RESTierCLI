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
using NuGet.CommandLine;

namespace RestierCLI
{
    class Program
    {
        static void GetProviderFactoryClasses()
        {
            // Retrieve the installed providers and factories.
            DataTable table = DbProviderFactories.GetFactoryClasses();

            // Display each row and column value.
            foreach(DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    Console.WriteLine(row[column]);
                }
            }
          //  return table;
        }

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

               // GetProviderFactoryClasses();
                /*
                CommandLineApplication t = ExecuteCommand.Create();
               
                string[] c = { "create", connectiongString};
          
                t.Execute(c);
              */
              
                CommandLineApplication t2 = ExecuteCommand.Create();
                t2.Execute(args);
                string[] install = { "install", Directory.GetCurrentDirectory() + "\\Packages\\packages.config" };
                NuGet.CommandLine.Program.MainCore(Directory.GetCurrentDirectory() + "\\Packages", install);


            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
         
            return 0;
        }
    }

}
