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
        // the path of the templete project
        private string sourcePath = "C:\\Users\\t-qiche\\Documents\\Visual Studio 2015\\Projects\\RestierCLI\\RestierCLI\\TempleteProject";
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

        /// <summary>
        /// Generate an .NET Web Application Projcet which support a standardized, 
        /// OData V4 based RESTful services on .NET platform from a connection string
        /// </summary>
        /// <returns>true for success, false for failure</returns>
        public bool Generate()
        {
            bool flag;
            flag = _CopyFolder(sourcePath, projectPath);
            initFilesNeedToBeModified();
            foreach (var file in filesNeedToBeModified)
            {
                string fileName = projectPath + "\\" + file.ToString();
                if (!File.Exists(fileName))
                {
                    flag = false;
                }
                ChangeFileContent(fileName, templeteProjectName, projectName);
            }

            if (!AddConnectionStringInWebConfigFile())
                flag = false;

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

        private void initFilesNeedToBeModified()
        {
            filesNeedToBeModified.Add(projectName + ".sln");
            filesNeedToBeModified.Add(projectName + "\\" + projectName + ".csproj");
            filesNeedToBeModified.Add(projectName + "\\Global.asax");
            filesNeedToBeModified.Add(projectName + "\\Global.asax.cs");
            filesNeedToBeModified.Add(projectName + "\\App_Start\\WebApiConfig.cs");
        }


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
                        flag = _CopyFolder(subDir.FullName, dPath + "//" + projectName) ? flag : false;
                    }
                    else
                    {
                        flag = _CopyFolder(subDir.FullName, dPath + "//" + subDir.Name) ? flag : false;
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
