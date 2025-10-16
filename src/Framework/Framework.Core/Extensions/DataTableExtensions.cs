using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Framework.Core.Extensions
{
    public static class DataTableHelper
    {
        public static DataTable Delete(this DataTable table, string filter)
        {
            table.Select(filter).Delete();
            return table;
        }
        public static void Delete(this IEnumerable<DataRow> rows)
        {
            foreach (var row in rows)
                row.Delete();
        }
        public static DataTable GetMergedTable(this IEnumerable<DataTable> tables, char ColumnNameSpliter)
        {
            var tdSelectedIndex = 0;
            var tdColumnsCount = 0;
            var index = 0;
            foreach (var table in tables)
            {
                foreach (DataColumn column in table.Columns)
                {
                    column.ColumnName = column.ColumnName.Replace('_', ColumnNameSpliter).Replace(' ', ColumnNameSpliter);
                }
                if (table.Columns.Count > tdColumnsCount)
                {
                    tdColumnsCount = table.Columns.Count;
                    tdSelectedIndex = index;
                }
                index++;
            }

            var defaultDT = tables.ToArray()[tdSelectedIndex];


            index = -1;
            foreach (var table in tables)
            {
                index++;
                if (tdSelectedIndex == index)
                {
                    continue;
                }

                defaultDT.Merge(table);

            }

            return defaultDT;
        }
    }
}
