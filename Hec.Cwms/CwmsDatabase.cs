using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using Hec.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hec.Cwms
{
  public class CwmsDatabase
  {
    private Oracle oracle;
    private string officeID = "";
    public CwmsDatabase(Oracle oracle)
    {
      this.oracle = oracle;
    }

    public enum StorageRule {
      /// <summary>
      /// Insert values at new times and replace any values at existing times, even if incoming values are specified as missing
      /// </summary>
      Replace_All,
      /// <summary>
      /// Insert values at new times but do not replace any values at existing times
      /// </summary>
      Do_Not_Replace,
      /// <summary>
      /// Insert values at new times but do not replace any values at existing times unless the existing values are specified as missing
      /// </summary>
      Replace_Missing_Values_Only,
      /// <summary>
      /// Insert values at new times and replace any values at existing times, unless the incoming values are specified as missing
      /// </summary>
      Replace_With_Non_Missing,
      /// <summary>
      /// Delete all existing values in time window of incoming data and then insert incoming data
      /// </summary>
      Delete_Insert
    }


    /// <summary>
    /// --exec cwms_env.set_session_office_id('NAB');
    /// </summary>
    /// <param name="officeId"></param>
    public int SetOffice(string officeID)
    {
      OracleCommand cmd = new OracleCommand();
      cmd.Parameters.Add("P_OFFICE_ID", OracleDbType.Varchar2, 200, officeID, ParameterDirection.Input);
      cmd.CommandText = "cwms_env.set_session_office_id";
      cmd.CommandType = CommandType.StoredProcedure;
      int status = oracle.RunStoredProc(cmd);
      this.officeID = officeID;
      return status;
    }

    public string[] GetOfficeIDS()
    {
      var tbl = oracle.Table("officeids", "select distinct db_office_id from cwms_v_ts_id");
      string[] rval = new string[tbl.Rows.Count];
      for (int i = 0; i < rval.Length; i++)
      {
        rval[i] = tbl.Rows[i][0].ToString();
      }
      return rval;
    }

    private string LookupUnits(string tsid)
    {
      DataTable tbl = oracle.Table("cwms_v_ts_id", "select DB_OFFICE_ID, CWMS_TS_ID,UNIT_ID from cwms_v_ts_id where CWMS_TS_ID = '" + tsid + "' and DB_OFFICE_ID='" + officeID + "' ");
      if (tbl.Rows.Count > 0)
        return tbl.Rows[0]["UNIT_ID"].ToString();
      return "";
    }


    public ITimeSeries GetTimeSeries(string tsid, DateTime t1, DateTime t2)
    {
      TimeSeries rval = new TimeSeries(tsid);
      //01-JAN-1980 1530
      string fmt = "dd-MMM-yyyy HHmm";
      string start_time = t1.ToString(fmt);
      string end_time = t2.ToString(fmt);
      string units = LookupUnits(tsid);
      OracleConnection conn = oracle.GetConnection();
      OracleCommand cmd = new OracleCommand();
      cmd.Connection = conn;
      try
      {
        conn.Open();
      }
      catch (OracleException e)
      {
        Console.Out.WriteLine(e.Message);
        return rval;
      }

      // get  TS_CODE, units, inteval, 
      cmd.CommandText =
     "begin " +
     "  cwms_ts.retrieve_ts( " +
     "    :ts_cur, " +
     "    :tsid, " +
     "    :units, " +
     "    to_date(:start_time, 'dd-mon-yyyy hh24mi'), " +
     "    to_date(:end_time,   'dd-mon-yyyy hh24mi'), " +
     "    'UTC');" +
     "end;";
      cmd.CommandType = CommandType.Text;
      OracleRefCursor ts_cur = null;
      cmd.Parameters.Add(
          new OracleParameter(
              "ts_cur",
              OracleDbType.RefCursor,
              0,
              ts_cur,
              ParameterDirection.Output));
      cmd.Parameters.Add(
          new OracleParameter(
              "tsid",
              OracleDbType.Varchar2,
              183,
              tsid,
              ParameterDirection.Input));
      cmd.Parameters.Add(
          new OracleParameter(
              "units",
              OracleDbType.Varchar2,
              16,
              units,
              ParameterDirection.Input));
      cmd.Parameters.Add(
          new OracleParameter(
              "start_time",
              OracleDbType.Varchar2,
              16,
              start_time,
              ParameterDirection.Input));
      cmd.Parameters.Add(
          new OracleParameter(
              "end_time",
              OracleDbType.Varchar2,
              16,
              end_time,
              ParameterDirection.Input));
      cmd.ExecuteNonQuery();
      ts_cur = (OracleRefCursor)cmd.Parameters["ts_cur"].Value;
      var dr = ts_cur.GetDataReader();

      while (dr.Read())
      {
        DateTime date_time = dr.GetOracleDate(0).Value;
        double? value = dr.IsDBNull(1) ? (double?)null : dr.GetDouble(1);
        int quality = (int)dr.GetDecimal(2);

        rval.Add(date_time, value, quality);
      }
      ts_cur.Dispose();
      cmd.Dispose();
      conn.Close();
      conn.Dispose();
      return rval;

    }

    public void SaveTimeSeries(TimeSeries ts, StorageRule saveOption = StorageRule.Replace_All)
    {
      /*
       PROCEDURE store_ts (
    p_cwms_ts_id      IN VARCHAR2,
    p_units           IN VARCHAR2,
    p_times           IN number_array,
    p_values          IN double_array,
    p_qualities       IN number_array,
    p_store_rule      IN VARCHAR2,

     p_override_prot   IN VARCHAR2 DEFAULT 'F',
    p_version_date    IN DATE DEFAULT cwms_util.non_versioned,
    p_office_id       IN VARCHAR2 DEFAULT NULL);
   IS*/
      string sql = "BEGIN cwms_ts.store_ts ( "
      + " :p_cwms_ts_id, "
      + " :p_units, "
      + " :p_times, "
      + " :p_values, "
      + " :p_qualities, "
      + " :p_store_rule);  END;";
      //        + " :p_override_prot IN VARCHAR2 DEFAULT 'F', "
      //      + " p_version_date IN DATE DEFAULT cwms_util.non_versioned, "
      //     + " p_office_id IN VARCHAR2 DEFAULT NULL)";
      OracleConnection conn = oracle.GetConnection();
      conn.InfoMessage += Conn_InfoMessage;
      OracleCommand cmd = new OracleCommand(sql,conn);

      cmd.Parameters.Add("p_cwms_ts_id", ts.TSID);
      cmd.Parameters.Add("p_units", ts.Units);

      var op = new OracleParameter("p_times", ts.TimesAsJavaMilliSeconds());
      op.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
      cmd.Parameters.Add(op);

      op = new OracleParameter("p_values", ts.Values);
      op.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
      cmd.Parameters.Add(op);

      op = new OracleParameter("p_qualities", ts.Qualities);
      op.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
      cmd.Parameters.Add(op);

      cmd.Parameters.Add("p_store_rule", saveOption.ToString().ToUpper().Replace('_', ' '));
      cmd.Connection = conn;
      try
      {
        conn.Open();
        cmd.ExecuteNonQuery();
      }
      catch (OracleException e)
      {
        Console.Out.WriteLine(e.Message);
      }

    }

    private void Conn_InfoMessage(object sender, OracleInfoMessageEventArgs eventArgs)
    {
      Console.WriteLine(eventArgs.Message);
    }
    //public ITimeSeries SetTimeSeries(TimeSeries ts)
    //{

    //  //TimeSeries rval = new TimeSeries(tsid);
    //  //01 - JAN - 1980 1530
    //  string fmt = "MM-MMM-yyyy HHmm";
    //  string start_time = ts.getTSStartTime().ToString(fmt);
    //  string end_time = ts.getTSSEndTime().ToString(fmt);
    //  string units = LookupUnits(ts.TSID);
    //  OracleConnection conn = oracle.GetConnection();
    //  OracleCommand cmd = new OracleCommand();
    //  cmd.Connection = conn;
    //  try
    //  {
    //    conn.Open();
    //  }
    //  catch (OracleException e)
    //  {
    //    Console.Out.WriteLine(e.Message);
    //  }

    //  //get TS_CODE, units, inteval,
    //  //Store_Ts(P_Cwms_Ts_Id      In    Varchar2,
    //  //P_Units   In    Varchar2,
    //  //P_Times   In    Number_Array,
    //  //P_Values      In    Double_Array,
    //  //P_Qualities   In    Number_Array,
    //  //P_Store_Rule      In    Varchar2,
    //  //P_Override_Prot   In    Varchar2 Default 'F',
    //  //P_Version_Date    In    Date,
    //  //P_Office_Id   In    Varchar2 Default Null,
    //  //P_Create_As_Lrts      In    Varchar2 Default 'F')

    //  cmd.CommandText =
    // "begin " +
    // "  cwms_ts.Store_Ts( " +
    // "    :P_Cwms_Ts_Id, " +
    // "    :P_Units, " +
    // "    :P_Units, " +
    // "    to_date(:start_time, 'dd-mon-yyyy hh24mi'), " +
    // "    to_date(:end_time,   'dd-mon-yyyy hh24mi'), " +
    // "    :P_Store_Rule,  " +
    // "    :P_Override_Prot,  " +
    // "    :P_Version_Date,  " +
    // "    :P_Office_Id,  " +
    // "    :P_Create_As_Lrts,  " +

    // "    'UTC');" +
    // "end;";
    //  cmd.CommandType = CommandType.Text;
    //  cmd.Parameters.Add(
    //      new OracleParameter(
    //          "P_Cwms_Ts_Id",
    //          OracleDbType.Varchar2,
    //          183,
    //          ts.TSID,
    //          ParameterDirection.Input));
    //  cmd.Parameters.Add(
    //      new OracleParameter(
    //          "P_Units",
    //          OracleDbType.Varchar2,
    //          16,
    //          units,
    //          ParameterDirection.Input));
    //  cmd.Parameters.Add(
    //      new OracleParameter(
    //          "P_Times",
    //          OracleDbType.,
    //          16,
    //          start_time,
    //          ParameterDirection.Input));
    //  cmd.Parameters.Add(
    //      new OracleParameter(
    //          "end_time",
    //          OracleDbType.Varchar2,
    //          16,
    //          end_time,
    //          ParameterDirection.Input));
    //  cmd.ExecuteNonQuery();
    //  ts_cur = (OracleRefCursor)cmd.Parameters["ts_cur"].Value;
    //  var dr = ts_cur.GetDataReader();

    //  while (dr.Read())
    //  {
    //    DateTime date_time = dr.GetOracleDate(0).Value;
    //    double? value = dr.IsDBNull(1) ? (double?)null : dr.GetDouble(1);
    //    int quality = (int)dr.GetDecimal(2);

    //    rval.Add(date_time, value, quality);
    //  }
    //  ts_cur.Dispose();
    //  cmd.Dispose();
    //  conn.Close();
    //  conn.Dispose();
    //  return rval;

    //}
  }

}
