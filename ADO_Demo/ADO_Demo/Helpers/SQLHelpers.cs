using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;


namespace ADO_Demo.SQLHelpers
{
    public static class SQLHelpers
    {

        /// <summary>
        /// used to differentiate different SP return types when executing a SqlCommand object
        /// </summary>
        private enum CommandReturn
        {
            Table,
            RowsAffected,
            FirstCell
        }


        private static Dictionary<string, string> _connectionStringOverrides;
        //Singleton Property
        private static Dictionary<string, string> ConnectionStringOverrides
        {
            get
            {
                if (_connectionStringOverrides == null) { _connectionStringOverrides = new Dictionary<string, string>(); }
                return _connectionStringOverrides;
            }
        }

        public static void AddConnectionStringOverride(string name, string connectionString)
        {
            name = name.ToLower().Trim();

            if (ConnectionStringOverrides.ContainsKey(name))
            {
                ConnectionStringOverrides[name] = connectionString;
            }
            else
            {
                ConnectionStringOverrides.Add(name, connectionString);
            }
        }

        public static void RemoveConnectionStringOverride(string name)
        {
            name = name.ToLower().Trim();
            if (ConnectionStringOverrides.ContainsKey(name))
            {
                ConnectionStringOverrides.Remove(name);
            }
        }


        private static SqlConnection GetConnection(string connectionStringName)
        {

            string connectionString = null;
            connectionStringName = connectionStringName.ToLower().Trim();

            //check for overrides (eg from Console Application that has no Web.config
            if (ConnectionStringOverrides.ContainsKey(connectionStringName))
            {
                connectionString = ConnectionStringOverrides[connectionStringName];
            }

            //check for Web.config connection
            if (connectionString.IsEmptyX())
            {
                connectionString = Convert.ToString(ConfigurationManager.ConnectionStrings[connectionStringName]);
            }

            return new SqlConnection(connectionString);

        }

        public static DataTable GetTable(string connectionStringName,
                                           string storedProcedureNameOrCommandText,
                                           SqlParameter[] parameters,
                                           bool isStoredProcedure = true)
        {

            DataTable returnTable;
            int returnRowsAffected;
            object returnFirstCell;

            GetData(connectionStringName, storedProcedureNameOrCommandText, parameters, out returnTable, out returnRowsAffected, out returnFirstCell, CommandReturn.Table, isStoredProcedure);

            return returnTable;

        }


        public static int GetRowsAffected(string connectionStringName,
                                            string storedProcedureNameOrCommandText,
                                            SqlParameter[] parameters,
                                           bool isStoredProcedure = true)
        {

            DataTable returnTable;
            int returnRowsAffected;
            object returnFirstCell;

            GetData(connectionStringName, storedProcedureNameOrCommandText, parameters, out returnTable, out returnRowsAffected, out returnFirstCell, CommandReturn.RowsAffected, isStoredProcedure);

            return returnRowsAffected;

        }


        public static object GetFirstCell(string connectionStringName,
                                          string storedProcedureNameOrCommandText,
                                          SqlParameter[] parameters,
                                          bool isStoredProcedure = true)
        {

            DataTable returnTable;
            int returnRowsAffected;
            object returnFirstCell;

            GetData(connectionStringName, storedProcedureNameOrCommandText, parameters, out returnTable, out returnRowsAffected, out returnFirstCell, CommandReturn.FirstCell, isStoredProcedure);

            return returnFirstCell;

        }



        private static bool GetData(string connectionStringName,
                               string storedProcedureNameOrCommandText,
                               SqlParameter[] parameters,
                               out DataTable returnTable,
                               out int returnRowsAffected,
                               out object returnFirstCell,
                               CommandReturn kind,
                               bool isStoredProcedure = true)
        {
            returnRowsAffected = -1;
            returnTable = new DataTable(); //This is a non-null return
            returnFirstCell = null;

            using (SqlConnection connection = GetConnection(connectionStringName))
            {
                //Stored Procedure Command
                using (SqlCommand command = new SqlCommand(storedProcedureNameOrCommandText, connection))
                {

                    //wire up connection to the command
                    if (isStoredProcedure) // if not, then assume that SQL text was passed as the storedProcedureNameOrCommandText
                    {
                        command.CommandType = CommandType.StoredProcedure;
                    }

                    //Add Parameters
                    if (parameters != null)
                    {
                        foreach (SqlParameter paramItem in parameters)
                        {
                            command.Parameters.Add(paramItem);
                        }
                    }


                    //Get appropriate return value based on type
                    connection.Open();

                    switch (kind)
                    {
                        case CommandReturn.Table:
                            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                            {
                                adapter.Fill(returnTable);
                            }
                            break;

                        case CommandReturn.RowsAffected:
                            returnRowsAffected = command.ExecuteNonQuery();
                            break;

                        case CommandReturn.FirstCell:
                            returnFirstCell = command.ExecuteScalar();
                            break;

                        default:
                            throw new NotImplementedException("Unimplemented CommandReturn in ADO request");
                    }

                    return true;
                }
            }


        }

        //Extension Methods
        /// <summary>
        /// Gets a column value as string. If column is missing or NULL it returns the default. Optional bool indicates if empty strings should receive the default (by passing false, empty strings are replaced with the default)
        /// </summary>
        /// <param name="row">any DataRow object</param>
        /// <param name="columnName">the name of the column inthe DataRow</param>
        /// <param name="defaultIfNull">the default value in case the column is null or missing</param>
        /// <param name="emptyStringsAllowed">pass false to force empty strings to get the default. true indicates that an empty string is a permitted return value.</param>
        /// <returns></returns>
        public static string GetStringOr(this DataRow row, string columnName, string defaultIfNull, bool emptyStringsAllowed = true)
        {
            //didn't pass original value by ref, since it may need to be assigned to a class property which cant be passed by ref

            if (columnName.IsEmptyX() || row.Table.Columns[columnName] == null) { return defaultIfNull; } //validate non-null column

            string newValue = Convert.ToString(row[columnName]);
            if (!emptyStringsAllowed && string.IsNullOrEmpty(newValue)) { return defaultIfNull; } //validate whether empty string are allowed etc

            return newValue;
        }

        /// <summary>
        /// Gets a column value as integer. If column is missing or NULL it returns the default. Optional minvalue  that the result must be higher or else you will receive the default (by passing a minvalue of 0, a parse of -1 or -2 will be replaced with the default, but 0 will return ok)
        /// </summary>
        /// <param name="row">any DataRow object</param>
        /// <param name="columnName">the name of the column inthe DataRow</param>
        /// <param name="defaultIfNull">the default value in case the column is null or missing</param>
        /// <param name="minimumAllowedValue">if supplied, the resulting int must be above this min value or else the default is returned (eg. 0 to prevent a return of -1)</param>
        /// <returns></returns>
        public static int GetIntOr(this DataRow row, string columnName, int defaultIfNull, int? minimumAllowedValue = null)
        {
            if (columnName.IsEmptyX() || row.Table.Columns[columnName.Trim()] == null) { return defaultIfNull; } //validate non-null column

            int resultInt = 0;

            if (row.Table.Columns[columnName].DataType == typeof(int))
            {
                if (row[columnName] == DBNull.Value)
                {
                    return defaultIfNull;
                }
                resultInt = Convert.ToInt32(row[columnName]); //simple conversion
            }
            else if (!int.TryParse(Convert.ToString(row[columnName]), out resultInt))
            {
                //We were unable to parse the string value to an int, so return default
                return defaultIfNull;
            }

            //we have a parsed int


            if (minimumAllowedValue != null && resultInt < minimumAllowedValue)
            {
                //Doesn't match minimum requirement (eg. must be above -1)
                return defaultIfNull;
            }

            return resultInt; //successful return

        }

        /// <summary>
        /// Gets a column value as boolean. If column is missing or NULL it returns the default. If Column is not null, but also not a bit, it tries to convert to string and parse as bool using "1" "true" "t" "y" "yes" etc. Optional optionalTrueString allows the caller to specify a string that should return TRUE if the record contains it. Likewise, the optionalFalseString.
        /// </summary>
        /// <param name="row">any DataRow object</param>
        /// <param name="columnName">the name of the column inthe DataRow</param>
        /// <param name="defaultIfNull">the default value in case the column is null or missing</param>
        /// <param name="optionalTrueString">If supplied, this will cause a cell value matching this string to return as true</param>
        /// <param name="optionalFalseString">If supplied, this will cause a cell value matching this string to return as false</param>
        /// <returns></returns>
        public static bool GetBoolOr(this DataRow row, string columnName, bool defaultIfNull, string optionalTrueString = null, string optionalFalseString = null)
        {
            if (columnName.IsEmptyX() || row.Table.Columns[columnName.Trim()] == null) { return defaultIfNull; } //validate non-null column


            if (row.Table.Columns[columnName].DataType == typeof(bool))
            {
                if (row[columnName] == DBNull.Value)
                {
                    return defaultIfNull;
                }
                return Convert.ToBoolean(row[columnName]);
            }

            //not a standard bool column, so eval as string
            string boolValue = Convert.ToString(row[columnName]).ToLower();

            if (optionalFalseString != null && boolValue == optionalFalseString) { return false; }
            if (optionalTrueString != null && boolValue == optionalTrueString) { return true; }

            switch (boolValue)
            {
                case "true":
                case "1":
                case "t":
                case "y":
                case "yes":
                    return true;
                case "false":
                case "0":
                case "f":
                case "n":
                case "no":
                    return false;
                default:
                    return defaultIfNull;
            }
        }

        /// <summary>
        /// Gets a column value as DateTime. If column is missing or NULL it returns the default. If datetime parsing fails, it returns the default.
        /// </summary>
        /// <param name="row">any DataRow object</param>
        /// <param name="columnName">the name of the column inthe DataRow</param>
        /// <param name="defaultIfNull">the default value in case the column is null or missing</param>
        /// <returns></returns>
        public static DateTime GetDateTimeOr(this DataRow row, string columnName, DateTime defaultIfNull)
        {
            if (columnName.IsEmptyX() || row.Table.Columns[columnName] == null || row[columnName] == DBNull.Value) { return defaultIfNull; } //validate non-null column

            DateTime resultDateTime = DateTime.MinValue;
            if (DateTime.TryParse(Convert.ToString(row[columnName]), out resultDateTime))
            {
                return resultDateTime;
            }
            return defaultIfNull;
        }

        public static decimal GetDecimalOr(this DataRow row, string columnName, decimal defaultIfNull)
        {
            columnName = columnName.Trim();
            if (columnName.IsEmptyX() || row.Table.Columns[columnName] == null) { return defaultIfNull; } //validate non-null column

            if (row.Table.Columns[columnName].DataType == typeof(decimal))
            {
                if (row[columnName] == DBNull.Value)
                {
                    return defaultIfNull;
                }
                return Convert.ToDecimal(row[columnName]); //simple conversion, return
            }

            decimal resultDecimal = 0;
            if (decimal.TryParse(Convert.ToString(row[columnName]), out resultDecimal))
            {
                //We were able to parse the string value to an int, so return it
                return resultDecimal;
            }

            return defaultIfNull; //parse failed, return default
        }

        //Older Static Methods

        /// <summary>
        /// This will assign a column's string value if it is not null and depending on whether empty strings are allowed via parameter. Otherwise, it returns the original value.
        /// </summary>
        /// <param name="original">The orignial value (to persist in case of error)</param>
        /// <param name="row">The DataTable's DataRow that contains the column to be evaluated</param>
        /// <param name="columnName">The name of the column in the DataTable</param>
        /// <param name="emptyStringsAllowed">Should an empty string be returned if the column contains it? (If null, the origninal is returned in either case)</param>
        /// <returns></returns>
        public static string AssignColumnStringIfPresent(string original, DataRow row, string columnName, bool emptyStringsAllowed)
        {
            //didn't pass original value by ref, since it may need to be assigned to a class property which cant be passed by ref

            if (columnName.IsEmptyX() || row.Table.Columns[columnName] == null) { return original; } //validate non-null column

            string newValue = Convert.ToString(row[columnName]);
            if (!emptyStringsAllowed && string.IsNullOrEmpty(newValue)) { return original; } //validate whether empty string are allowed etc

            return newValue;
        }

        /// <summary>
        /// This will assign a column's bool value if it is not null. Otherwise, it returns the default passed value.
        /// </summary>
        /// <param name="defaultIfNull">The default or previous value to return in case of null or failure to parse</param>
        /// <param name="row">The DataTable's DataRow that contains the column to be evaluated</param>
        /// <param name="columnName">The name of the column in the DataTable</param>
        /// <returns></returns>
        public static bool AssignColumnBoolIfPresent(bool defaultIfNull, DataRow row, string columnName)
        {
            if (columnName.IsEmptyX() || row.Table.Columns[columnName.Trim()] == null) { return defaultIfNull; } //validate non-null column


            if (row.Table.Columns[columnName].DataType == typeof(bool))
            {
                return Convert.ToBoolean(row[columnName]);
            }

            //not a standard bool column, so eval as string
            string boolValue = Convert.ToString(row[columnName]).ToLower();
            switch (boolValue)
            {
                case "true":
                case "1":
                case "t":
                case "y":
                case "yes":
                    return true;
                case "false":
                case "0":
                case "f":
                case "n":
                case "no":
                    return false;
                default:
                    return defaultIfNull;
            }

        }

        /// <summary>
        /// This will assign a column's datetime value if it is not null. Otherwise it returns the default passed value
        /// </summary>
        /// <param name="defaultIfNull">NOTE! If saving to SQL, the min valu is in 1753, so use (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;    Otherwise set the default or previous value to return in case of null or failure to parse</param>
        /// <param name="row">The DataTable's DataRow that contains the column to be evaluated</param>
        /// <param name="columnName">The name of the column in the DataTable</param>
        /// <returns></returns>
        public static DateTime AssignColumnDateTimeIfPresent(DateTime defaultIfNull, DataRow row, string columnName)
        {
            if (columnName.IsEmptyX() || row.Table.Columns[columnName] == null) { return defaultIfNull; } //validate non-null column

            DateTime resultDateTime = DateTime.MinValue;
            if (DateTime.TryParse(Convert.ToString(row[columnName]), out resultDateTime))
            {
                return resultDateTime;
            }
            return defaultIfNull;
        }

        public static int AssignColumnIntIfPresent(int defaultIfNull, DataRow row, string columnName)
        {
            if (columnName.IsEmptyX() || row.Table.Columns[columnName.Trim()] == null) { return defaultIfNull; } //validate non-null column


            if (row.Table.Columns[columnName].DataType == typeof(int))
            {
                return Convert.ToInt32(row[columnName]); //simple conversion, return
            }

            int resultInt = 0;
            if (int.TryParse(Convert.ToString(row[columnName]), out resultInt))
            {
                //We were able to parse the string value to an int, so return it
                return resultInt;
            }

            return defaultIfNull; //parse failed, return default
        }

        public static decimal AssignColumnDecimalIfPresent(decimal defaultIfNull, DataRow row, string columnName)
        {
            if (columnName.IsEmptyX() || row.Table.Columns[columnName.Trim()] == null) { return defaultIfNull; } //validate non-null column


            if (row.Table.Columns[columnName].DataType == typeof(decimal))
            {
                return Convert.ToDecimal(row[columnName]); //simple conversion, return
            }

            decimal resultDecimal = 0;
            if (decimal.TryParse(Convert.ToString(row[columnName]), out resultDecimal))
            {
                //We were able to parse the string value to an int, so return it
                return resultDecimal;
            }

            return defaultIfNull; //parse failed, return default
        }


        // DataReader

        /// <summary>This checks if a column exists in a SqlDataReader result
        /// </summary>
        public static bool HasColumn(IDataReader Reader, string ColumnName)
        {
            // http://stackoverflow.com/questions/373230/check-for-column-name-in-a-sqldatareader-object

            foreach (DataRow row in Reader.GetSchemaTable().Rows)
            {
                if (row["ColumnName"].ToString() == ColumnName)
                    return true;
            } //Still here? Column not found. 
            return false;
        }

        public static string GetStringOr(this IDataReader reader, string columnName, string defaultIfNull, bool emptyStringsAllowed = true)
        {
            //didn't pass original value by ref, since it may need to be assigned to a class property which cant be passed by ref

            if (columnName.IsEmptyX() || !HasColumn(reader, columnName)) { return defaultIfNull; } //validate non-null column

            string newValue = Convert.ToString(reader[columnName]);
            if (!emptyStringsAllowed && string.IsNullOrEmpty(newValue)) { return defaultIfNull; } //validate whether empty string are allowed etc

            return newValue;
        }

        public static decimal GetDecimalOr(this IDataReader reader, string columnName, decimal defaultIfNull)
        {
            columnName = columnName.Trim();
            if (columnName.IsEmptyX() || !HasColumn(reader, columnName)) { return defaultIfNull; } //validate non-null column

            if (reader[columnName] == DBNull.Value)
            {
                return defaultIfNull;
            }

            try
            {
                return Convert.ToDecimal(reader[columnName]); //simple conversion, return
            }
            catch (Exception)
            {
                decimal resultDecimal = 0;
                if (decimal.TryParse(Convert.ToString(reader[columnName]), out resultDecimal))
                {
                    //We were able to parse the string value to an int, so return it
                    return resultDecimal;
                }
            }


            return defaultIfNull; //parse failed, return default
        }
    }
}