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
    /// <summary>
    /// Management for the SQLServer database
    /// </summary>
    class SQLServerManager
    {
        // the connection string of the SQLServer database
        private readonly string connectionString;
        private EntityConnection ec = null;
        // specific to SQLServer
        private const string providerInvariantName = "System.Data.SqlClient";
        private Version maxVersion = new Version(3, 0, 0, 0);
        public SQLServerManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        ~SQLServerManager()
        {
            if (ec != null && ec.State == ConnectionState.Open)
                ec.Close();
        }

        /// <summary>
        /// Connect to the database figured out by the connection string
        /// </summary>
        /// <returns>true for success and false for failure</returns>
        public bool connect()
        {
            try
            {
                Version actualEntityFrameworkConnectionVersion;
                ec = new StoreSchemaConnectionFactory().Create(
                    DependencyResolver.Instance,
                    providerInvariantName,
                    connectionString,
                    maxVersion,
                    out actualEntityFrameworkConnectionVersion);
                ec.Open();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Console.WriteLine(ex.InnerException.Message);
                else
                    Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Get the name of the database figured out by the connection string
        /// </summary>
        /// <returns>return the name of the database figured out by the connection string,
        /// return null if the database can not be connected by the connection string</returns>
        public string GetDatabaseName()
        {
            if (!string.IsNullOrEmpty(ec.Database))
                return ignorePrefixPathAndSuffix(ec.Database);
            if (ec.StoreConnection != null && !string.IsNullOrEmpty(ec.StoreConnection.Database))
                return ignorePrefixPathAndSuffix(ec.StoreConnection.Database);
            return null;
        }

        /// <summary>
        /// get all tables in the database
        /// </summary>
        /// <returns>return all tables in the database</returns>
        public ArrayList GetDatabaseTables()
        {
            ArrayList tables = new ArrayList();

            using (var command = new EntityCommand(null, ec, DependencyResolver.Instance))
            {
                command.CommandType = CommandType.Text;
                command.CommandText = SelectTablesESqlQuery;
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
                                tables.Add(new DatabaseTable(catalogName, schemaName, name));
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
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
            return tables;
        }

      
        // Get the file name from a path by ignoring the prefix directory and suffix file type
        private string ignorePrefixPathAndSuffix(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;
            int lastSlashIndex = path.LastIndexOf('\\');
            int lastDotIndex = path.LastIndexOf('.');
            if (lastSlashIndex == -1)
                lastSlashIndex = 0;
            if (lastDotIndex == -1)
                lastSlashIndex = path.Length;
            lastSlashIndex++;
            if (lastSlashIndex > lastDotIndex)
                return path.Substring(lastSlashIndex);
            else if (lastSlashIndex == lastDotIndex)
                return "";
            else
                return path.Substring(lastSlashIndex, lastDotIndex - lastSlashIndex);
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
