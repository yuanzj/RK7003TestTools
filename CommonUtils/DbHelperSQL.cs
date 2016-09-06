using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DbHelperSQL
{
    private DbHelperSQL()
    { }
    //  private static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AccessConnectionString"].ConnectionString;

    //public static string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + System.AppDomain.CurrentDomain.BaseDirectory + @"生产数据.mdb";

    public static string connectionString;
    /// <summary>
    /// 执行一条计算查询结果语句，返回查询结果（object）。
    /// </summary>
    /// <param name="SQLString">计算查询结果语句</param>
    /// <returns>查询结果（object）</returns>
    public static object GetSingle(string SQLString)
    {
        using (OleDbConnection connection = new OleDbConnection(connectionString))
        {
            using (OleDbCommand cmd = new OleDbCommand(SQLString, connection))
            {
                try
                {
                    connection.Open();
                    object obj = cmd.ExecuteScalar();
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (System.Data.OleDb.OleDbException e)
                {
                    connection.Close();
                    throw e;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
    }
    /// <summary>
    /// 执行一条计算查询结果语句，返回查询结果（object）。
    /// </summary>
    /// <param name="SQLString">计算查询结果语句</param>
    /// <returns>查询结果（object）</returns>
    public static object GetSingle(string SQLString, params OleDbParameter[] cmdParms)
    {
        using (OleDbConnection connection = new OleDbConnection(connectionString))
        {
            using (OleDbCommand cmd = new OleDbCommand())
            {
                try
                {
                    PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                    object obj = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
    }
    /// <summary>
    /// 执行查询语句，返回DataSet
    /// </summary>
    /// <param name="SQLString">查询语句</param>
    /// <returns>DataSet</returns>
    public static DataSet Query(string SQLString)
    {
        using (OleDbConnection connection = new OleDbConnection(connectionString))
        {
            DataSet ds = new DataSet();
            try
            {
                connection.Open();
                OleDbDataAdapter command = new OleDbDataAdapter(SQLString, connection);
                command.Fill(ds, "ds");
            }
            catch (System.Data.OleDb.OleDbException ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
            return ds;
        }
    }
    public static DataSet Query(string SQLString, int Times)
    {
        using (OleDbConnection connection = new OleDbConnection(connectionString))
        {
            DataSet ds = new DataSet();
            try
            {
                connection.Open();
                OleDbDataAdapter command = new OleDbDataAdapter(SQLString, connection);
                command.SelectCommand.CommandTimeout = Times;
                command.Fill(ds, "ds");
            }
            catch (System.Data.OleDb.OleDbException ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
            return ds;
        }
    }
    /// <summary>
    /// 执行查询语句，返回DataSet
    /// </summary>
    /// <param name="SQLString">查询语句</param>
    /// <returns>DataSet</returns>
    public static DataSet Query(string SQLString, params OleDbParameter[] cmdParms)
    {
        using (OleDbConnection connection = new OleDbConnection(connectionString))
        {
            OleDbCommand cmd = new OleDbCommand();
            PrepareCommand(cmd, connection, null, SQLString, cmdParms);
            using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                try
                {
                    da.Fill(ds, "ds");
                    cmd.Parameters.Clear();
                }
                catch (System.Data.OleDb.OleDbException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
                return ds;
            }
        }
    }
    public static bool Exists(string strSql)
    {
        object obj = GetSingle(strSql);
        int cmdresult;
        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
        {
            cmdresult = 0;
        }
        else
        {
            cmdresult = int.Parse(obj.ToString());
        }
        if (cmdresult == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public static bool Exists(string strSql, params OleDbParameter[] cmdParms)
    {
        object obj = GetSingle(strSql, cmdParms);
        int cmdresult;
        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
        {
            cmdresult = 0;
        }
        else
        {
            cmdresult = int.Parse(obj.ToString());
        }
        if (cmdresult == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    #region 执行带参数的sql语句
    /// <summary>
    /// 执行SQL语句，返回影响的记录数
    /// </summary>
    /// <param name="SQLString">SQL语句</param>
    /// <returns>影响的记录数</returns>
    public static int ExecuteSql(string SQLString, params OleDbParameter[] cmdParms)
    {
        using (OleDbConnection connection = new OleDbConnection(connectionString))
        {
            using (OleDbCommand cmd = new OleDbCommand())
            {
                try
                {
                    PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                    int rows = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return rows;
                }
                catch (System.Data.OleDb.OleDbException e)
                {
                    throw e;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
    }


    /// <summary>
    /// 执行一条返回结果集的OleDbCommand命令，通过专用的连接字符串。
    /// 使用参数数组提供参数
    public static OleDbDataReader ExecuterReader(string connectionstring, CommandType type, string cmdText, params OleDbParameter[] commandParameters)
    {
        OleDbCommand cmd = new OleDbCommand();  //准备命令
        OleDbConnection conn = new OleDbConnection(connectionstring);  //创建连接

        // 在这里使用try/catch处理是因为如果方法出现异常，则SqlDataReader就不存在，
        //CommandBehavior.CloseConnection的语句就不会执行，触发的异常由catch捕获。
        //关闭数据库连接，并通过throw再次引发捕捉到的异常。

        //判断数据库连接状态
        if (conn.State != ConnectionState.Open)
        {
            conn.Open();            //打开数据库连接
        }
        cmd.Connection = conn;      //建立连接
        cmd.CommandText = cmdText;  //指定命令文本
        cmd.CommandType = type;  //指定命令类型

        if (commandParameters != null)
        {
            foreach (OleDbParameter param in commandParameters)
            {
                cmd.Parameters.Add(param);  //添加参数到参数列表中
            }
        }

        OleDbDataReader reader = cmd.ExecuteReader(); //执行命令
        //返回一个数据库记录集
        return reader;


    }
    #endregion

    #region 执行简单sql语句
    /// <summary>
    /// 执行SQL语句，返回影响的记录数
    /// </summary>
    /// <param name="SQLString">SQL语句</param>
    /// <returns>影响的记录数</returns>
    public static int ExecuteSql(string SQLString)
    {
        using (OleDbConnection connection = new OleDbConnection(connectionString))
        {
            using (OleDbCommand cmd = new OleDbCommand(SQLString, connection))
            {
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.OleDb.OleDbException e)
                {
                    connection.Close();
                    throw e;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
    }

    /// <summary>
    /// 准备执行一个命令
    /// </summary>
    /// <param name="cmd">sql命令</param>
    /// <param name="conn">Sql连接</param>
    /// <param name="trans">Sql事务</param>
    /// <param name="cmdText">命令文本,例如：Select * from Products</param>
    /// <param name="cmdParms">执行命令的参数</param>
    private static void PrepareCommand(OleDbCommand cmd, OleDbConnection conn, OleDbTransaction trans, string cmdText, OleDbParameter[] cmdParms)
    {
        //判断连接的状态。如果是关闭状态，则打开
        if (conn.State != ConnectionState.Open)
            conn.Open();
        //cmd属性赋值
        cmd.Connection = conn;
        cmd.CommandText = cmdText;
        //是否需要用到事务处理
        if (trans != null)
            cmd.Transaction = trans;
        cmd.CommandType = CommandType.Text;
        //添加cmd需要的存储过程参数
        if (cmdParms != null)
        {
            foreach (OleDbParameter parm in cmdParms)
                cmd.Parameters.Add(parm);
        }
    }


    /// <summary>
    /// 执行SQL语句，返回影响的记录数
    /// </summary>
    /// <param name="SQLString">SQL语句</param>
    /// <returns>影响的记录数</returns>
    public static void RunSql(string SQLString)
    {
        using (OleDbConnection connection = new OleDbConnection(connectionString))
        {
            using (OleDbCommand cmd = new OleDbCommand(SQLString, connection))
            {
                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (System.Data.OleDb.OleDbException e)
                {
                    connection.Close();
                    throw e;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
    }
    //private static void PrepareCommand(OleDbCommand cmd, OleDbConnection conn, OleDbTransaction trans, string cmdText, OleDbParameter[] cmdParms)
    //{
    //    if (conn.State != ConnectionState.Open)
    //        conn.Open();
    //    cmd.Connection = conn;
    //    cmd.CommandText = cmdText;
    //    if (trans != null)
    //        cmd.Transaction = trans;
    //    cmd.CommandType = CommandType.Text;//cmdType;
    //    if (cmdParms != null)
    //    {


    //        foreach (OleDbParameter parameter in cmdParms)
    //        {
    //            if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
    //                (parameter.Value == null))
    //            {
    //                parameter.Value = DBNull.Value;
    //            }
    //            cmd.Parameters.Add(parameter);
    //        }
    //    }
    //}
    #endregion
}

