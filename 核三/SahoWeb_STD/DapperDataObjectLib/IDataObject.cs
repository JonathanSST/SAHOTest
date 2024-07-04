using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace DapperDataObjectLib
{
    public interface IDataObject
    {
        /// <summary>
        /// Executes a insert, update or delete sql.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>Affected rows. If the value is more than 0, it means the sql succeeded.</returns>
        int ExecuteNonQuery(string sql);


        /// <summary>
        /// Executes a query sql.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>Contains the query result.</returns>
        DataTable GetDataTableBySql(string sql);


        /// <summary>
        /// Executes a query sql and return paticular range of rows.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="startRecord">The zero-based record number to start with.</param>
        /// <param name="maxRecord">The maximum number of records to retrieve.</param>
        /// <param name="srcTable">The name of the source table to use for table mapping.</param>
        /// <returns>Contains the query result.</returns>
        DataTable GetDataTableBySql(string sql, int startRecord, int maxRecord, String srcTable);


        /// <summary>
        /// Begins a transaction.
        /// </summary>
        void BeginTransaction();


        /// <summary>
        /// Commits the transaction.
        /// </summary>
        void Commit();


        /// <summary>
        /// Rollbacks the transaction.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Get Connection
        /// </summary>
        IDbConnection Connection
        {
            get;
        }
    }
}
