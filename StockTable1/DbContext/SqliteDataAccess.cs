using Dapper;
using Microsoft.Data.Sqlite;
using StockTable1.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StockTable1.DbContext
{
    public class SqliteDataAccess
    {
        static string relativePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static void InsertStock(StockUpdate stockUpdate)
        {
            using(var connection = new SqliteConnection(LoadConnectionString()))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = $"Insert into StockData (Symbol, Price, Change) values ('{stockUpdate.Symbol}', {stockUpdate.Price}, { stockUpdate.Change})";
                    object i = cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateStock(StockUpdate stockUpdate)
        {
            using (var connection = new SqliteConnection(LoadConnectionString()))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = $"Update StockData set Price = {stockUpdate.Price}, Change = { stockUpdate.Change} where Symbol = '{stockUpdate.Symbol}'";
                    cmd.ExecuteScalar();
                }
            }
        }

        public static void TruncateStock()
        {
            using (var connection = new SqliteConnection(LoadConnectionString()))
            {
                SQLitePCL.Batteries.Init();
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = $"Delete from StockData; VACUUM; ";
                    cmd.ExecuteScalar();
                }
            }
        }

        public static string LoadConnectionString()
        {
            return @"Data Source=" + relativePath + @"\StockTableDb.db"; 
        }
    }
}
