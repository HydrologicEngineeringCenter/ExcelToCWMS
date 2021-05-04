﻿using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hec.Cwms
{
    public class Oracle
    {
        private string user;

        /// <summary>
        /// Creates Oracle connection by taking the following parameters.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="host"></param>
        /// <param name="sid"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static Oracle Connect(string user, string host, string sid, string port)
        {

            Console.WriteLine("User: " + user);
            Console.Write("password:");
            string pass = Console.ReadLine();

            var o = new Oracle(user, pass, host, sid, port);

            return o;

        }

        internal OracleConnection GetConnection()
        {
            var conn = new OracleConnection(ConnectionString);
            return conn;
        }


        private string pass;
        private string host;
        private string service;
        private string port;


        string ConnectionString { get; set; }


        public Oracle(string user, string pass, string host, string service, string port = "1521")
        {
            this.user = user;
            this.pass = pass;
            this.host = host;
            this.service = service;
            this.port = port;


            ConnectionString = GetConnectionString();
            Console.WriteLine(ConnectionString.Replace(";Password=" + pass, ";Password=***"));
        }

        private string GetConnectionString()
        {

            return "Data Source=(DESCRIPTION="
                 + "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + host + ")(PORT=" + port + ")))"
                 + "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + service + ")));"
                 + "User Id=" + user + ";Password=" + pass + ";";// enlist=false;pooling=false;";
        }


        public int RunStoredProc(OracleCommand cmd)
        {
            int rval = 0;
            OracleConnection conn = new OracleConnection(ConnectionString);
            Debug.Assert(cmd.CommandType == CommandType.StoredProcedure);
            cmd.Connection = conn;

            rval = -1;
            try
            {
                conn.Open();
                rval = cmd.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                throw exc;
            }
            conn.Close();

            for (int i = 0; i < cmd.Parameters.Count; i++)
            {
                Console.WriteLine(cmd.Parameters[i].ParameterName + ", " + cmd.Parameters[i].Value.ToString() + " [" + cmd.Parameters[i].Direction.ToString() + "]");
            }
            return rval;
        }


        public DataTable Table(string tableName)
        {
            return Table(tableName, "select * from " + tableName);
        }


        public DataTable Table(string tableName, string sql)
        {
            return Table(tableName, sql, true);
        }


        public DataTable Table(string tableName, string sql, bool throwErrors)
        {
            var conn = new OracleConnection(ConnectionString);
            var cmd = new OracleCommand();
            cmd.CommandText = sql;
            cmd.Connection = conn;
            var da = new OracleDataAdapter();
            da.SelectCommand = cmd;

            DataSet myDataSet = new DataSet();
            try
            {
                conn.Open();

                da.Fill(myDataSet, tableName);
            }
            catch (Exception e)
            {
                string msg = "Error reading from database \n" + sql + "\n Exception " + e.ToString();
                Console.WriteLine(msg);

                if (throwErrors)
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }
            DataTable tbl = myDataSet.Tables[tableName];
            return tbl;
        }
    }
}