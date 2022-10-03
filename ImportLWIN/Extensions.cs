using ClosedXML.Excel;
using ImportLWIN.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheSwamp.Shared;

namespace ImportLWIN
{
    public static class DataSetExtensions
    {
        public static DataSet ToDataSet(this XLWorkbook workBook)
        {
            var ds = new DataSet();

            foreach (var sheet in workBook.Worksheets)
            {
                var dt = new DataTable(sheet.Name);

                bool isFirstRow = true;

                //Loop through the Worksheet rows.
                foreach (var row in sheet.Rows())
                {
                    //Use the first row to add columns to DataTable.
                    if (isFirstRow)
                    {
                        // If first row is empty, ignore this sheet.
                        var first = row.FirstCellUsed();
                        var last = row.LastCellUsed();
                        if (first == null || last == null)
                        {
                            break;
                        }

                        foreach (var cell in row.Cells(first.Address.ColumnNumber, last.Address.ColumnNumber))
                        {
                            dt.Columns.Add(cell.Value.ToString());
                        }
                        dt.Columns.Add("__row");
                        isFirstRow = false;
                    }
                    else
                    {
                        DataRow r = dt.NewRow();
                        for (int i = 1; i <= dt.Columns.Count; i++)
                        {
                            var cell = row.Cell(i);
                            var value = cell.CachedValue.ToString();

                            // Handle big numbers that might have come through in scientific notation
                            if (Regex.IsMatch(value, "\\d*\\.\\d*E\\+\\d*"))
                            {
                                value = decimal.Parse(value, System.Globalization.NumberStyles.Any).ToString();
                            }

                            r[i - 1] = value;
                        }

                        if (!r.IsEmpty())
                        {
                            r["__row"] = row.RowNumber();
                        }

                        dt.Rows.Add(r);
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    ds.Tables.Add(dt);
                }
            }

            return ds;
        }

        public static object Value(this DataRow row, string columnName)
        {
            object val = null;

            var col = row.Table.Columns.Cast<DataColumn>().SingleOrDefault(c => c.ColumnName.EqualsNoCase(columnName));
            if (col != null)
            {
                val = row[columnName];
            }
            return val;
        }


        public static string StringValue(this DataRow row, string columnName, string defaultValue = "")
        {
            var val = row.Value(columnName);

            return val == null ? defaultValue : val.ToString().Trim();
        }

        public static DateTime DateTimeValue(this DataRow row, string columnName)
        {
            var val = row.StringValue(columnName);

            return DateTime.Parse(val);
        }

        public static long LongValue(this DataRow row, string columnName, int defaultValue = 0)
        {
            var val = row.StringValue(columnName, defaultValue.ToString());

            return string.IsNullOrEmpty(val) ? defaultValue : Convert.ToInt64(val);
        }

        public static long? NullableLongValue(this DataRow row, string columnName, long? defaultValue = null)
        {
            var val = row.StringValue(columnName, defaultValue?.ToString());

            return string.IsNullOrEmpty(val) ? defaultValue : Convert.ToInt64(val);
        }


        public static decimal? NullableDecimalValue(this DataRow row, string columnName, decimal? defaultValue = null)
        {
            var val = row.StringValue(columnName, defaultValue?.ToString());

            return string.IsNullOrEmpty(val) ? defaultValue : Convert.ToDecimal(val);
        }

        public static bool IsEmpty(this DataRow row, IEnumerable<string> columns = null)
        {
            bool isEmpty = true;

            for (int i = 0; i < row.ItemArray.Count(); i++)
            {
                object o = row.ItemArray[i];
                if ((columns == null || columns.ContainsNoCase(row.Table.Columns[i].ColumnName)) && !string.IsNullOrEmpty(o?.ToString()))
                {
                    isEmpty = false;
                    break;
                }
            }

            return isEmpty;
        }

        public static LWINRaw[] ToRaw(this DataTable source)
        {
            var raw = new LWINRaw[source.Rows.Count];

            for (int i = 0; i < source.Rows.Count; i++)
            {
                var row = source.Rows[i];
                raw[i] = new LWINRaw
                {
                    LWIN = row.LongValue("LWIN"),
                    STATUS = row.StringValue("STATUS"),
                    DISPLAY_NAME = row.StringValue("DISPLAY_NAME"),
                    PRODUCER_TITLE = row.StringValue("PRODUCER_TITLE"),
                    PRODUCER_NAME = row.StringValue("PRODUCER_NAME"),
                    WINE = row.StringValue("WINE"),
                    COUNTRY = row.StringValue("COUNTRY"),
                    REGION = row.StringValue("REGION"),
                    SUB_REGION = row.StringValue("SUB_REGION"),
                    SITE = row.StringValue("SITE"),
                    PARCEL = row.StringValue("PARCEL"),
                    COLOUR = row.StringValue("COLOUR"),
                    TYPE = row.StringValue("TYPE"),
                    SUB_TYPE = row.StringValue("SUB_TYPE"),
                    DESIGNATION = row.StringValue("DESIGNATION"),
                    CLASSIFICATION = row.StringValue("CLASSIFICATION"),
                    VINTAGE_CONFIG = row.StringValue("VINTAGE_CONFIG"),
                    FIRST_VINTAGE = row.StringValue("FIRST_VINTAGE"),
                    FINAL_VINTAGE = row.StringValue("FINAL_VINTAGE"),
                    DATE_ADDED = row.DateTimeValue("DATE_ADDED"),
                    DATE_UPDATED = row.DateTimeValue("DATE_UPDATED"),
                    REFERENCE = row.NullableLongValue("REFERENCE")
                };
            }

            return raw;
        }
    }
}
