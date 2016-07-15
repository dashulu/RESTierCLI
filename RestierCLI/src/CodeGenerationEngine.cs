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
        private string connectionString;
        private string providerInvariantName = "System.Data.SqlClient";

        private string appConfigConnectionPropertyName;

        private Version maxVersion = new Version(3, 0, 0, 0);

        private ArrayList databaseTables = new ArrayList();

        private string projectName;

        private SQLServerManager sqlManager = null;

        public CodeGenerationEngine (string connectionString, string projectName)
        {
            this.connectionString = connectionString;
            this.projectName = projectName;
            sqlManager = new SQLServerManager(connectionString);
        }

        /// <summary>
        /// Generate the mapping class for each table in the database
        /// </summary>
        /// <returns>return the mapping class for each table in the database,
        /// the key is the class file name and the value is the code for the class</returns>
        public IEnumerable<KeyValuePair<string, string>> GenerateCode()
        {
            try
            {
                if(!sqlManager.connect())
                {
                    return null;
                }
                databaseTables = sqlManager.GetDatabaseTables();
                if (databaseTables.Count == 0)
                    return null;
                DatabaseTable tableItem = (DatabaseTable) (databaseTables[0]);
                ModelBuilderSettings modelBuilderSettings = new ModelBuilderSettings();
                modelBuilderSettings._designTimeConnectionString = connectionString;
                modelBuilderSettings._appConfigConnectionString = connectionString;
                modelBuilderSettings._designTimeProviderInvariantName = providerInvariantName;
                modelBuilderSettings._runtimeProviderInvariantName = providerInvariantName;
                modelBuilderSettings.SaveConnectionStringInAppConfig = true;
                modelBuilderSettings.AppConfigConnectionPropertyName = projectName;
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
                modelBuilderSettings.TargetSchemaVersion = new Version(3, 0, 0, 0);
                modelBuilderSettings.ProviderManifestToken = "2008";

                var mbe = modelBuilderSettings.ModelBuilderEngine;
                mbe.GenerateModel(modelBuilderSettings);

                var generator = new MyCodeFirstModelGenerator();
                return generator.Generate(mbe.Model, projectName + ".Models", projectName + "Context", projectName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
