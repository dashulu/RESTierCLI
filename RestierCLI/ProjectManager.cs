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
    class ProjectManager
    {
        private string destinationPath;
        private string sourcePath = "C:\\Users\\t-qiche\\Documents\\Visual Studio 2015\\Projects\\RestierCLI\\RestierCLI\\TempleteProject";
        private const string templeteProjectName = "TempleteProject";
        private string targetProjectName;
        private string connectionString;
        CodeGenerationEngine engine;
        private ArrayList filesNeedToBeModified = new ArrayList();

        public ProjectManager(string connectionString, string destinationPath, string targetProjectName)
        {
            this.connectionString = connectionString;
            this.destinationPath = destinationPath + "\\" +  targetProjectName;
            Console.WriteLine(this.destinationPath);
            this.targetProjectName = targetProjectName;
        }

        public ProjectManager(string connectionString, string targetProjectName)
        {
            this.connectionString = connectionString;
            this.targetProjectName = targetProjectName;
            this.destinationPath = System.Environment.CurrentDirectory;
            Console.WriteLine("System.Environment.CurrentDirectory:" + System.Environment.CurrentDirectory);
            Console.WriteLine("Directory.GetCurrentDirectory:" + Directory.GetCurrentDirectory());
        }

        private void initFilesNeedToBeModified()
        {
            filesNeedToBeModified.Add(targetProjectName + ".sln");
            filesNeedToBeModified.Add(targetProjectName + "\\" + targetProjectName + ".csproj");
            filesNeedToBeModified.Add(targetProjectName + "\\Global.asax");
            filesNeedToBeModified.Add(targetProjectName + "\\Global.asax.cs");
        }

        private bool ChangeWebApiConfigFile()
        {
            string fileName = destinationPath + "\\" + targetProjectName + "\\App_Start\\WebApiConfig.cs";
            if (!File.Exists(fileName))
            {
                return false;
            }
            StreamReader sr = new StreamReader(fileName);
            string str = sr.ReadToEnd();
            sr.Close();
            int index = 0;
            // find the index where the templeProjectName appears for the third time in the text
            for (int i = 0; i < 3; i++)
            {
                index = str.IndexOf(templeteProjectName, index);
                index += templeteProjectName.Length;
            }
            str = str.Replace(templeteProjectName, targetProjectName);
            index = index - templeteProjectName.Length + 2 * (targetProjectName.Length - templeteProjectName.Length);
            index += targetProjectName.Length;
            str = str.Insert(index, "Context");

            StreamWriter sw = new StreamWriter(fileName, false);
            sw.WriteLine(str);
            sw.Close();
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


        private bool AddConnectionStringInWebConfigFile()
        {
            string WebConfigFileName = destinationPath + "\\" + targetProjectName + "\\Web.config";
            if (!File.Exists(WebConfigFileName))
                return false;
            XmlDocument doc = new XmlDocument();
            doc.Load(WebConfigFileName);
            XmlElement node = (XmlElement)doc.GetElementsByTagName("configuration").Item(0);
            XmlElement connectionStringsNode = doc.CreateElement("connectionStrings", node.NamespaceURI);
            XmlElement nodeInConnectionStringNode = doc.CreateElement("add", node.NamespaceURI);
            nodeInConnectionStringNode.SetAttribute("name", targetProjectName);
            nodeInConnectionStringNode.SetAttribute("connectionString", this.connectionString);
            nodeInConnectionStringNode.SetAttribute("providerName", "System.Data.SqlClient");
            connectionStringsNode.AppendChild(nodeInConnectionStringNode);
            node.AppendChild(connectionStringsNode);
            doc.Save(WebConfigFileName);
            return true;
        }

        private bool AddModelFileItemInCSPROJFile(IEnumerable<KeyValuePair<string, string>> modelFiles)
        {
            string CSPROJFileName = destinationPath + "\\" + targetProjectName + "\\" + targetProjectName + ".csproj";
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
            string modelDirPath = destinationPath + "\\" + targetProjectName + "\\Models\\"; 
            foreach(var file in modelFiles)
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



        public bool CreateProject()
        {
            bool flag;
            flag = _CopyFolder(sourcePath, destinationPath);
            initFilesNeedToBeModified();
            foreach (var file in filesNeedToBeModified)
            {
                string fileName = destinationPath + "\\" + file.ToString();
                if (!File.Exists(fileName))
                {
                    flag = false;
                }
                ChangeFileContent(fileName, templeteProjectName, targetProjectName);
            }

            if (!AddConnectionStringInWebConfigFile())
                flag = false;
            if (!ChangeWebApiConfigFile())
                flag = false;

            engine = new CodeGenerationEngine(connectionString, targetProjectName);
            AddModleFile(engine.GenerateCode());
            return flag;
        }
        private bool _CopyFolder(string sPath, string dPath)
        {
            bool flag = true;
            try
            {
                if (!Directory.Exists(sPath))
                {
                    return false;
                }
                // create destination directory
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
                        file.CopyTo(dPath + "\\" + targetProjectName + ".sln", true);
                    }
                    else if (file.Name.Equals(templeteProjectName + ".csproj"))
                    {
                        file.CopyTo(dPath + "\\" + targetProjectName + ".csproj", true);
                    }
                    else if (file.Name.Equals(templeteProjectName + ".csproj.user"))
                    {
                        file.CopyTo(dPath + "\\" + targetProjectName + ".csproj.user", true);
                    } else
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
                        flag = _CopyFolder(subDir.FullName, dPath + "//" + targetProjectName) ? flag : false;
                    } else
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
