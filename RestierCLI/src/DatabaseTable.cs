using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestierCLI
{
    internal class DatabaseTable
    {
        public string catalogName { get; set; }
        public string schemaName { get; set; }
        public string tableName { get; set; }

        public DatabaseTable(string catalogName, string schemaName, string tableName)
        {
            this.catalogName = catalogName;
            this.schemaName = schemaName;
            this.tableName = tableName;
        }
    }
}
