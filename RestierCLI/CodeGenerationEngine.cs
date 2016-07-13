using System;
using Microsoft.Data.Entity.Design.CodeGeneration;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using Microsoft.Data.Entity.Design.VersioningFacade;
using Microsoft.Data.Entity.Design.VisualStudio.ModelWizard.Engine;
using System.Data;
using Microsoft.Data.Entity.Design.VersioningFacade.ReverseEngineerDb;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace RestierCLI
{
    public class CodeGenerationEngine
    {
        public string connectionString { get; set; }
        public string providerInvariantName { get; set; }

        private string appConfigConnectionPropertyName{get; set;}

        private Version maxVersion = new Version(3, 0, 0, 0);

        private ArrayList databaseTables = new ArrayList();

        class DatabaseTable
        {
            public string catalogName { get; set; }
            public string schemaName { get; set; }
            public string tableName { get; set; }

            public DatabaseTable (string catalogName, string schemaName, string tableName)
            {
                this.catalogName = catalogName;
                this.schemaName = schemaName;
                this.tableName = tableName;
            }
        }

        public CodeGenerationEngine (string connectionString)
        {
            this.connectionString = connectionString;
            providerInvariantName = "System.Data.SqlClient";
            appConfigConnectionPropertyName = null;
        }

        private bool GenerateDatabaseTables()
        {
            EntityConnection ec = null;
            try
            {
                Version actualEntityFrameworkConnectionVersion;
                ec = new StoreSchemaConnectionFactory().Create(
                    DependencyResolver.Instance,
                    providerInvariantName,
                    connectionString,
                    maxVersion,
                    out actualEntityFrameworkConnectionVersion);

                using (var command = new EntityCommand(null, ec, DependencyResolver.Instance))
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = SelectTablesESqlQuery;
                    ec.Open();
                    DbDataReader reader = null;
                    try
                    {
                        reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
                        while (reader.Read())
                        {
                            if (reader.FieldCount == 3)
                            {
                                // the types coming back through the reader may not be a string 
                                // (eg, SqlCE returns System.DbNull for catalogName & schemaName), so cast carefully
                                var catalogName = reader.GetValue(0) as String;
                                var schemaName = reader.GetValue(1) as String;
                                var name = reader.GetValue(2) as String;

                                if (String.IsNullOrEmpty(name) == false)
                                {
                                    databaseTables.Add(new DatabaseTable(catalogName, schemaName, name));
                                }
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        if (reader != null)
                        {
                            try
                            {
                                reader.Close();
                                reader.Dispose();
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            finally
            {
                if (ec != null)
                {
                    try
                    {
                        ec.Close();
                        ec.Dispose();
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            return true;
        }

        public IEnumerable<KeyValuePair<string, string>> GenerateCode()
        {
            try
            {
                GenerateDatabaseTables();
                if (databaseTables.Count == 0)
                    return null;
                string databaseName;
                DatabaseTable tableItem = (DatabaseTable) (databaseTables[0]);
                if (tableItem.catalogName != null && tableItem.catalogName.Length != 0)
                {
                    int subStringBegin = tableItem.catalogName.LastIndexOf('\\');
                    subStringBegin = subStringBegin == -1 ? 0 : subStringBegin;
                    int subStringEnd = tableItem.catalogName.LastIndexOf('.');
                    if (subStringBegin < subStringEnd - 1)
                        databaseName = tableItem.catalogName.Substring(subStringBegin + 1, subStringEnd - subStringBegin - 1);
                    else
                        databaseName = tableItem.catalogName.Substring(subStringBegin + 1);
                }
                else
                {
                    databaseName = "database";
                }
                ModelBuilderSettings modelBuilderSettings = new ModelBuilderSettings();
                modelBuilderSettings._designTimeConnectionString = connectionString;
                modelBuilderSettings._appConfigConnectionString = connectionString;
                modelBuilderSettings._designTimeProviderInvariantName = providerInvariantName;
                modelBuilderSettings._runtimeProviderInvariantName = providerInvariantName;
                modelBuilderSettings.SaveConnectionStringInAppConfig = true;

                modelBuilderSettings.AppConfigConnectionPropertyName = databaseName;
                //                modelBuilderSettings.ModelNamespace = "ModelNamespace";
                modelBuilderSettings.UsePluralizationService = true;
                modelBuilderSettings.IncludeForeignKeysInModel = true;
                SchemaFilterEntryBag schemaFilterEntryBag = new Microsoft.Data.Entity.Design.VisualStudio.ModelWizard.Engine.SchemaFilterEntryBag();
                for (int i = 0; i < databaseTables.Count; i++)
                {
                    tableItem = (DatabaseTable)(databaseTables[i]);
                    EntityStoreSchemaFilterEntry item = new EntityStoreSchemaFilterEntry(tableItem.catalogName ,
                        tableItem.schemaName, tableItem.tableName, EntityStoreSchemaFilterObjectTypes.Table, 
                        EntityStoreSchemaFilterEffect.Allow);
                    schemaFilterEntryBag.IncludedTableEntries.Add(item);
                }


                modelBuilderSettings.DatabaseObjectFilters = schemaFilterEntryBag.CollapseAndOptimize(SchemaFilterPolicy.GetByValEdmxPolicy());
                modelBuilderSettings.ModelBuilderEngine = new MyCodeFirstModelBuilderEngine();
//                modelBuilderSettings.ModelPath = "C:\\Users\\t-qiche\\Documents\\Visual Studio 2015\\Projects\\WebApplication4\\WebApplication4";
                modelBuilderSettings.TargetSchemaVersion = new Version(3, 0, 0, 0);
                modelBuilderSettings.ProviderManifestToken = "2008";

                var mbe = modelBuilderSettings.ModelBuilderEngine;
                mbe.GenerateModel(modelBuilderSettings);

                var generator = new MyCodeFirstModelGenerator();
                return generator.Generate(mbe.Model, databaseName + ".Models", databaseName, databaseName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        private const string SelectTablesESqlQuery = @"
            SELECT 
                t.CatalogName
            ,   t.SchemaName                    
            ,   t.Name
            FROM
                SchemaInformation.Tables as t
            ORDER BY
                t.SchemaName
            ,   t.Name
            ";
    }
}
