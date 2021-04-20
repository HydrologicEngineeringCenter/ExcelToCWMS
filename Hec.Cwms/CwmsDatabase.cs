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
    /// <summary>
    /// --exec cwms_env.set_session_office_id('NAB');
    /// </summary>
    /// <param name="officeId"></param>
    public int SetOffice(string officeID)
    {
      OracleCommand cmd = new OracleCommand();
      cmd.Parameters.Add("P_OFFICE_ID",OracleDbType.Varchar2,200,officeID,ParameterDirection.Input);
      cmd.CommandText = "cwms_env.set_session_office_id";
      cmd.CommandType = CommandType.StoredProcedure;
      int status = oracle.RunStoredProc(cmd);
      this.officeID = officeID;
      return status;
    }

    public string[] GetOfficeIDS()
    {
      var tbl = oracle.Table("officeids","select distinct db_office_id from cwms_v_ts_id");
      string[] rval = new string[tbl.Rows.Count];
      for (int i = 0; i < rval.Length; i++)
      {
        rval[i] = tbl.Rows[i][0].ToString();
      }
      return rval;
    }

    private string LookupUnits(string tsid)
    {
      DataTable tbl = oracle.Table("cwms_v_ts_id","select DB_OFFICE_ID, CWMS_TS_ID,UNIT_ID from cwms_v_ts_id where CWMS_TS_ID = '" + tsid + "' and DB_OFFICE_ID='" + officeID + "' ");
      if (tbl.Rows.Count > 0)
        return tbl.Rows[0]["UNIT_ID"].ToString();
        return "";
    }


    public ITimeSeries GetTimeSeries(string tsid, DateTime t1, DateTime t2)
    {
      TimeSeries rval = new TimeSeries(tsid);
      //01-JAN-1980 1530
      string fmt = "MM-MMM-yyyy HHmm";
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

        rval.Add(date_time, value,quality);
      }
      ts_cur.Dispose();
      cmd.Dispose();
      conn.Close();
      conn.Dispose();
      return rval;

    }
  }
}
