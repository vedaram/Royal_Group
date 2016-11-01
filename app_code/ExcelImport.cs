using System;
using System.Data;
using System.IO;
using ClosedXML.Excel;
using System.Diagnostics;

namespace SecurAX.Import.Excel
{
    public sealed class ExcelImport
    {

        public ExcelImport() { }

        public static DataTable ImportExcelToDataTable(string file_name, string sheet_name)
        {
            DataTable return_data_table = new DataTable();
            string value = string.Empty;
            IXLWorksheet ws;
            try
            {
                var wb = new XLWorkbook(file_name);
                if (!string.IsNullOrEmpty(sheet_name))
                {
                    wb.TryGetWorksheet(sheet_name, out ws);
                }
                else
                {
                    ws = wb.Worksheet(1);
                }

                if (ws != null)
                {
                    var data_range = ws.RangeUsed();
                    int i = 0,
                        row_count = data_range.RowCount();
                    bool first_row = true;

                    foreach (var cell in data_range.Cells())
                    {
                        if (first_row)
                        {
                            return_data_table.Columns.Add(cell.Value.ToString());
                        }
                        else
                        {
                            value = cell.GetValue<string>();
                            return_data_table.Rows[return_data_table.Rows.Count - 1][i] = value;
                        }
                        i++;

                        if (i == data_range.ColumnCount() && first_row)
                        {
                            first_row = false;
                        }

                        if (i == data_range.ColumnCount() && !first_row)
                        {
                            if (row_count != (return_data_table.Rows.Count + 1))
                                return_data_table.Rows.Add();
                            i = 0;
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }

            return return_data_table;
        }

    }

}
