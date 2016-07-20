using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RestierCLI
{
    /// <summary>
    /// Build an .NET Web Application Projcet which support a standardized, 
    /// OData V4 based RESTful services on .NET platform from a connection string
    /// </summary>
    class WebApplicationBuilder
    {
        // Path for the newly-build project
        private readonly string projectPath;
        // Name of the newly-build project
        private readonly string projectName;
        // the connection string of the database which gonna create a OData V4 based RESTful services
        private readonly string connectionString;

        // the name of the templete project
        private const string templeteProjectName = "TempleteProject";
        // files whose content should be updated in the new web application project
        private ArrayList filesNeedToBeModified = new ArrayList();

        public WebApplicationBuilder(string connection, string projectName, string projectPath)
        {
            this.connectionString = connection;
            this.projectName = projectName;
            this.projectPath = projectPath + "\\" + projectName;
        }

        // Create the folders for the .Net Web Applications
        private void CreateFolders()
        {
            if (!Directory.Exists(projectPath))
            {
                Directory.CreateDirectory(projectPath);
            }
            Directory.CreateDirectory(projectPath + "\\.vs");
            Directory.CreateDirectory(projectPath + "\\.vs\\config");
            Directory.CreateDirectory(projectPath + "\\packages");
            Directory.CreateDirectory(projectPath + "\\" + projectName);
            Directory.CreateDirectory(projectPath + "\\" + projectName + "\\App_Data");
            Directory.CreateDirectory(projectPath + "\\" + projectName + "\\App_Start");
            Directory.CreateDirectory(projectPath + "\\" + projectName + "\\bin");
            Directory.CreateDirectory(projectPath + "\\" + projectName + "\\Controllers");
            Directory.CreateDirectory(projectPath + "\\" + projectName + "\\Models");
            Directory.CreateDirectory(projectPath + "\\" + projectName + "\\obj");
            Directory.CreateDirectory(projectPath + "\\" + projectName + "\\Properties");
            Directory.CreateDirectory(projectPath + "\\" + projectName + "\\scripts");
        }

        // Create a file that contains specific content
        private void CreateFile(string filename, string content)
        {
            var fs = File.Create(filename);
            fs.Close();
            StreamWriter sw = new StreamWriter(filename, false);
            sw.WriteLine(content);
            sw.Close();
        }

        // Create solution file for the C# project
        private void CreateSolutionFile()
        {
            string filename = projectPath + "\\" + projectName + ".sln";
            CreateFile(filename, FileContent.solutionFileContent.Replace("{0}", projectName));
        }
        // Create applicationhost.config file for the IIS Express
        private void CreateApplicationhostConfigFile()
        {
            string filename = projectPath + @"\.vs\config\applicationhost.config";
            string content = FileContent.applicationhostConfigFileContent.Replace("{1}", projectPath + "\\" + projectName);
            int index = content.IndexOf("{0}");
            content = content.Remove(index, 3);
            content = content.Insert(index, projectName);          
            CreateFile(filename, content);
        }

        private void CreateWebApiConfigFile()
        {
            string filename = projectPath + "\\" + projectName + @"\App_Start\WebApiConfig.cs";
            CreateFile(filename, FileContent.webApiConfigFileContent.Replace("{0}", projectName));
        }

        private void CreateAssemblyInfoFile()
        {
            string filename = projectPath + "\\" + projectName + @"\Properties\AssemblyInfo.cs";
            CreateFile(filename, FileContent.assemblyInfoFileContent);
        }

        private void CreateAiJSFile()
        {
            string filename = projectPath + "\\" + projectName + @"\scripts\ai.0.22.9-build00167.js";
            CreateFile(filename, FileContent.aiJsFileContent);
        }

        private void CreateAiJSMinFile()
        {
            string filename = projectPath + "\\" + projectName + @"\scripts\ai.0.22.9-build00167.min.js";
            CreateFile(filename, FileContent.aiJSMinFileContent);
        }

        private void CreateApplicationInsightsConfigFile()
        {
            string filename = projectPath + "\\" + projectName + @"\ApplicationInsights.config";
            CreateFile(filename, FileContent.applicationInsightsConfigFileContent);
        }

        private void CreateGlobalAsaxFile()
        {
            string filename = projectPath + "\\" + projectName + @"\Global.asax";
            CreateFile(filename, FileContent.globalAsaxFileContent.Replace("{0}", projectName));
        }

        private void CreateGlobalAsaxCSFile()
        {
            string filename = projectPath + "\\" + projectName + @"\Global.asax.cs";
            CreateFile(filename, FileContent.globalAsaxCSFileContent.Replace("{0}", projectName));
        }

        private void CreatePackagesConfigFile()
        {
            string filename = projectPath + "\\" + projectName + @"\packages.config";
            CreateFile(filename, FileContent.packagesConfigFileContent);
        }

        private void CreateCSPROJFile()
        {
            string filename = projectPath + "\\" + projectName + @"\" + projectName + ".csproj";
            CreateFile(filename, FileContent.CSPROJFileContent.Replace("{0}", projectName));
        }

        private void CreateCSPROJUserFile()
        {
            string filename = projectPath + "\\" + projectName + @"\" + projectName + ".csproj.user";
            CreateFile(filename, FileContent.CSPROJUserFileContent);
        }

        private void CreateWebConfigFile()
        {
            string filename = projectPath + "\\" + projectName + @"\Web.config";
            string content = FileContent.webConfigFileContent.Replace("{1}", connectionString);
            int index = content.IndexOf("{0}");
            content = content.Remove(index, 3);
            content = content.Insert(index, projectName);
            CreateFile(filename, content);
        }

        private void CreateWebDebugConfigFile()
        {
            string filename = projectPath + "\\" + projectName + @"\Web.Debug.config";
            CreateFile(filename, FileContent.webDebugConfigFileContent);
        }


        private void CreateWebReleaseConfigFile()
        {
            string filename = projectPath + "\\" + projectName + @"\Web.Release.config";
            CreateFile(filename, FileContent.webReleaseConfigFileContent);
        }
        /// <summary>
        /// Generate an .NET Web Application Projcet which support a standardized, 
        /// OData V4 based RESTful services on .NET platform from a connection string
        /// </summary>
        /// <returns>true for success, false for failure</returns>
        public bool Generate()
        {
            bool flag = true;
            CreateFolders();
            CreateSolutionFile();
            CreateApplicationhostConfigFile();
            CreateWebApiConfigFile();
            CreateAssemblyInfoFile();
            CreateAiJSFile();
            CreateAiJSMinFile();
            CreateApplicationInsightsConfigFile();
            CreateGlobalAsaxFile();
            CreateGlobalAsaxCSFile();
            CreatePackagesConfigFile();
            CreateCSPROJFile();
            CreateCSPROJUserFile();
            CreateWebConfigFile();
            CreateWebDebugConfigFile();
            CreateWebReleaseConfigFile();
            string[] install = { "install", projectPath + "\\" + projectName + @"\packages.config" };
            NuGet.CommandLine.Program.MainCore(projectPath + "\\packages", install);

            var engine = new CodeGenerationEngine(connectionString, projectName);
            AddModleFile(engine.GenerateCode());
            return flag;
        }

        // add a connection node in the Web.config file
        private bool AddConnectionStringInWebConfigFile()
        {
            string WebConfigFileName = projectPath + "\\" + projectName + "\\Web.config";
            if (!File.Exists(WebConfigFileName))
                return false;
            XmlDocument doc = new XmlDocument();
            doc.Load(WebConfigFileName);
            XmlElement node = (XmlElement)doc.GetElementsByTagName("configuration").Item(0);
            XmlElement connectionStringsNode = doc.CreateElement("connectionStrings", node.NamespaceURI);
            XmlElement nodeInConnectionStringNode = doc.CreateElement("add", node.NamespaceURI);
            nodeInConnectionStringNode.SetAttribute("name", projectName);
            nodeInConnectionStringNode.SetAttribute("connectionString", this.connectionString);
            nodeInConnectionStringNode.SetAttribute("providerName", "System.Data.SqlClient");
            connectionStringsNode.AppendChild(nodeInConnectionStringNode);
            node.AppendChild(connectionStringsNode);
            doc.Save(WebConfigFileName);
            return true;
        }


        // replace the originalWord with the newWord in the file
        private void ChangeFileContent(string filePath, string originalWord, string newWord)
        {
            StreamReader sr = new StreamReader(filePath);
            string str = sr.ReadToEnd();
            sr.Close();
            str = str.Replace(originalWord, newWord);
            StreamWriter sw = new StreamWriter(filePath, false);
            sw.WriteLine(str);
            sw.Close();
        }

        // Get the file that should be updated in the new web application
        private void initFilesNeedToBeModified()
        {
            filesNeedToBeModified.Add(projectName + ".sln");
            filesNeedToBeModified.Add(projectName + "\\" + projectName + ".csproj");
            filesNeedToBeModified.Add(projectName + "\\Global.asax");
            filesNeedToBeModified.Add(projectName + "\\Global.asax.cs");
            filesNeedToBeModified.Add(projectName + "\\App_Start\\WebApiConfig.cs");
            filesNeedToBeModified.Add(".vs\\config\\applicationhost.config");
        }

        // To add files to the cs project, we need to update the .csproj file
        private bool AddModelFileItemInCSPROJFile(IEnumerable<KeyValuePair<string, string>> modelFiles)
        {
            string CSPROJFileName = projectPath + "\\" + projectName + "\\" + projectName + ".csproj";
            if (!File.Exists(CSPROJFileName))
                return false;
            XmlDocument doc = new XmlDocument();
            doc.Load(CSPROJFileName);
            XmlElement node = (XmlElement)doc.GetElementsByTagName("Project").Item(0);
            XmlElement ItemGroupNode = doc.CreateElement("ItemGroup", node.NamespaceURI);
            foreach (var file in modelFiles)
            {
                XmlElement CompileNode = doc.CreateElement("Compile", node.NamespaceURI);
                CompileNode.SetAttribute("Include", "Models\\" + file.Key);
                ItemGroupNode.AppendChild(CompileNode);
            }

            node.AppendChild(ItemGroupNode);
            doc.Save(CSPROJFileName);
            return true;
        }

        private bool UpdateApplicationhostConfigFile()
        {
            string fileName = projectPath + "\\.vs\\config\\applicationhost.config";
            if (!File.Exists(fileName))
            {
                return false;
            }
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            XmlElement node = (XmlElement)doc.GetElementsByTagName("virtualDirectory").Item(0);
            node.SetAttribute("physicalPath", projectPath + "\\" + projectName);
            doc.Save(fileName);
            return true;
        }

        // Generate the files in the Modles directory
        private bool AddModleFile(IEnumerable<KeyValuePair<string, string>> modelFiles)
        {
            FileStream fs;
            StreamWriter streamwrite;
            string modelDirPath = projectPath + "\\" + projectName + "\\Models\\";
            foreach (var file in modelFiles)
            {
                fs = File.Create(modelDirPath + file.Key);
                fs.Close();
                streamwrite = new StreamWriter(modelDirPath + file.Key);
                streamwrite.Write(file.Value);
                streamwrite.Close();
            }
            AddModelFileItemInCSPROJFile(modelFiles);
            return true;
        }


        // copy all content in directory sPath to dPath
        private bool _CopyFolder(string sPath, string dPath)
        {
            bool flag = true;
            try
            {
                if (!Directory.Exists(sPath))
                {
                    return false;
                }

                if (!Directory.Exists(dPath))
                {
                    Directory.CreateDirectory(dPath);
                }

                // copy files
                DirectoryInfo sDir = new DirectoryInfo(sPath);
                FileInfo[] fileArray = sDir.GetFiles();
                foreach (FileInfo file in fileArray)
                {
                    if (file.Name.Equals(templeteProjectName + ".sln"))
                    {
                        file.CopyTo(dPath + "\\" + projectName + ".sln", true);
                    }
                    else if (file.Name.Equals(templeteProjectName + ".csproj"))
                    {
                        file.CopyTo(dPath + "\\" + projectName + ".csproj", true);
                    }
                    else if (file.Name.Equals(templeteProjectName + ".csproj.user"))
                    {
                        file.CopyTo(dPath + "\\" + projectName + ".csproj.user", true);
                    }
                    else
                    {
                        file.CopyTo(dPath + "\\" + file.Name, true);
                    }

                }

                // copy sub directory recursively
                DirectoryInfo dDir = new DirectoryInfo(dPath);
                DirectoryInfo[] subDirArray = sDir.GetDirectories();
                foreach (DirectoryInfo subDir in subDirArray)
                {
                    if (subDir.Name.Equals(templeteProjectName))
                    {
                        flag = _CopyFolder(subDir.FullName, dPath + "\\" + projectName) ? flag : false;
                    }
                    else
                    {
                        flag = _CopyFolder(subDir.FullName, dPath + "\\" + subDir.Name) ? flag : false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            return flag;
        }



    }
    
    

}
