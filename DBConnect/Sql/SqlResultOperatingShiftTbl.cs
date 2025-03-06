using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace DBConnect.SQL
{
    public class ColumnResultOperatingShiftTbl
    {
        public const String TABLE_NAME = "Result_Operating_Shift_Tbl";
        public const String OPERATION_DATE = "OperationDate";
        public const String OPERATION_SHIFT = "OperationShift";
        public const String START_TIME = "StartTime";
        public const String END_TIME = "EndTime";
    }

    public class SqlResultOperatingShiftTbl : ISqlBase
    {
        #region <Field>
        /// <summary>
        /// 接続文字列
        /// </summary>
        private string mConnectString { get; set; }

        /// <summary>
        /// ログインスタンス
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region <Property>
        /// <summary>
        /// DB接続オブジェクト
        /// </summary>
        public SqlDBConnector DbConnector { get; set; }
        #endregion

        #region <Constructor>
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SqlResultOperatingShiftTbl(string connectString)
        {
            mConnectString = connectString;
        }
        #endregion

        #region <Method>
        /// <summary>
        /// DB接続
        /// </summary>
        /// <returns>接続結果 true=成功 false=失敗</returns>
        public bool OpenConnection()
        {
            DbConnector = new SqlDBConnector(mConnectString);
            if (DbConnector == null)
            {
                logger.Error("DBのオープンに失敗しました");
                return false;
            }
            DbConnector.Create();
            DbConnector.OpenDatabase();

            return true;
        }

        /// <summary>
        /// DB切断
        /// </summary>
        public void CloseConnection()
        {
            DbConnector.CloseDatabase();
            DbConnector.Dispose();
        }

        /// <summary>
        /// SELECT処理の実行
        /// </summary>
        /// <returns></returns>
        public DataTable Select()
        {
            DataTable dt = new DataTable(ColumnResultOperatingShiftTbl.TABLE_NAME);
            StringBuilder sb = new StringBuilder();
            try
            {
                logger.Debug($"Select:{ColumnResultOperatingShiftTbl.TABLE_NAME}");
                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT * ");
                    sb.Append(String.Format("FROM {0} ", ColumnResultOperatingShiftTbl.TABLE_NAME));
                    sb.Append(String.Format("Order by {0}, {1};", ColumnResultOperatingShiftTbl.OPERATION_DATE, ColumnResultOperatingShiftTbl.OPERATION_SHIFT));

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Query:{sb.ToString()}");
                logger.Error(ex.Message + "," + ex.StackTrace);
                return null;
            }
            finally
            {
                CloseConnection();
            }
            return dt;
        }

        /// <summary>
        /// 条件付きSELECT処理の実行
        /// </summary>
        /// <param name="OperationDate">稼働日</param>
        /// <param name="OperationShift">稼働シフト</param>
        /// <returns></returns>
        public DataTable Select(DateTime OperationDate, int OperationShift = 0)
        {
            DataTable dt = new DataTable(ColumnResultOperatingShiftTbl.TABLE_NAME);
            StringBuilder sb = new StringBuilder();
            try
            {
                logger.Debug($"Select:{ColumnResultOperatingShiftTbl.TABLE_NAME}");
                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT * ");
                    sb.Append(String.Format("FROM {0} ", ColumnResultOperatingShiftTbl.TABLE_NAME));
                    sb.Append("WHERE 1 = 1 ");
                    sb.Append(String.Format("And {0} = @{0} ", ColumnResultOperatingShiftTbl.OPERATION_DATE));
                    if (OperationShift != 0)
                    {
                        sb.Append(String.Format("And {0} = @{0} ", ColumnResultOperatingShiftTbl.OPERATION_SHIFT));
                    }
                    sb.Append(String.Format("Order by {0}, {1};", ColumnResultOperatingShiftTbl.OPERATION_DATE, ColumnResultOperatingShiftTbl.OPERATION_SHIFT));

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(String.Format("@{0}", ColumnResultOperatingShiftTbl.OPERATION_DATE), SqlDbType.DateTime).Value = OperationDate;
                    if (OperationShift != 0)
                    {
                        cmd.Parameters.Add(String.Format("@{0}", ColumnResultOperatingShiftTbl.OPERATION_SHIFT), SqlDbType.Int).Value = OperationShift;
                    }

                    String paramStr = "";
                    if (SqlDBConnector.CreateParamsText(cmd.Parameters, out paramStr))
                    {
                        sb.Append(paramStr);
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Query:{sb.ToString()}");
                logger.Error(ex.Message + "," + ex.StackTrace);
                return null;
            }
            finally
            {
                CloseConnection();
            }
            return dt;
        }

        /// <summary>
        /// INSERT処理の実行
        /// </summary>
        /// <param name="insertDataTable"></param>
        /// <returns></returns>
        public bool Insert(DataTable insertDataTable)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Insert:{ColumnResultOperatingShiftTbl.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    foreach (DataRow udpateItem in insertDataTable.Rows)
                    {
                        if (udpateItem.RowState == DataRowState.Modified || udpateItem.RowState == DataRowState.Added)
                        {
                            sb.Clear();
                            sb.Append(String.Format("INSERT INTO {0} (", ColumnResultOperatingShiftTbl.TABLE_NAME));
                            sb.Append($"{ColumnResultOperatingShiftTbl.OPERATION_DATE}");
                            sb.Append($", {ColumnResultOperatingShiftTbl.OPERATION_SHIFT}");
                            sb.Append($", {ColumnResultOperatingShiftTbl.START_TIME}");
                            sb.Append($", {ColumnResultOperatingShiftTbl.END_TIME}");
                            sb.Append(") VALUES (");
                            sb.Append($"@{ColumnResultOperatingShiftTbl.OPERATION_DATE}");
                            sb.Append($", @{ColumnResultOperatingShiftTbl.OPERATION_SHIFT}");
                            sb.Append($", @{ColumnResultOperatingShiftTbl.START_TIME}");
                            sb.Append($", @{ColumnResultOperatingShiftTbl.END_TIME}");
                            sb.Append(");");

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnResultOperatingShiftTbl.OPERATION_DATE}", SqlDbType.Date)).Value = udpateItem.Field<DateTime>(ColumnResultOperatingShiftTbl.OPERATION_DATE);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnResultOperatingShiftTbl.OPERATION_SHIFT}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnResultOperatingShiftTbl.OPERATION_SHIFT);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnResultOperatingShiftTbl.START_TIME}", SqlDbType.DateTime)).Value = udpateItem.Field<DateTime>(ColumnResultOperatingShiftTbl.START_TIME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnResultOperatingShiftTbl.END_TIME}", SqlDbType.DateTime)).Value = udpateItem.Field<DateTime>(ColumnResultOperatingShiftTbl.END_TIME);

                            String paramStr = "";
                            if (SqlDBConnector.CreateParamsText(cmd.Parameters, out paramStr))
                            {
                                sb.Append(paramStr);
                            }

                            cmd.ExecuteNonQuery();
                        }
                    }
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Query:{sb.ToString()}");
                logger.Error(ex.Message + "," + ex.StackTrace);
                ret = false;
            }
            finally
            {
                CloseConnection();
            }
            return ret;
        }

        public bool Update(DataTable updateDataTable)
        {
            // NO USE
            return false;
        }

        /// <summary>
        /// UPDATE処理の実行
        /// </summary>
        /// <param name="updateDataTable"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public bool Update(DataTable updateDataTable, SqlTransaction transaction = null)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Update:{ColumnResultOperatingShiftTbl.TABLE_NAME}");

                SqlConnection conn = new SqlConnection();
                if (transaction != null)
                {
                    conn = transaction.Connection;
                }
                else
                {
                    if (OpenConnection() == false)
                    {
                        logger.Error("DBに接続されていません。");
                        return false;
                    }
                    conn = DbConnector.DbConnection;
                }

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    foreach (DataRow udpateItem in updateDataTable.Rows)
                    {
                        if (udpateItem.RowState == DataRowState.Modified /*|| udpateItem.RowState == DataRowState.Added*/)
                        {
                            sb.Clear();
                            sb.Append(string.Format("UPDATE {0} SET {1} = @{1}, {2} = @{2} ",
                                ColumnResultOperatingShiftTbl.TABLE_NAME,
                                ColumnResultOperatingShiftTbl.START_TIME,
                                ColumnResultOperatingShiftTbl.END_TIME));
                            sb.Append(string.Format("WHERE {0} = @{0} AND {1} = @{1}; ",
                                ColumnResultOperatingShiftTbl.OPERATION_DATE,
                                ColumnResultOperatingShiftTbl.OPERATION_SHIFT));

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnResultOperatingShiftTbl.OPERATION_DATE}", SqlDbType.Date)).Value = udpateItem.Field<DateTime>(ColumnResultOperatingShiftTbl.OPERATION_DATE);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnResultOperatingShiftTbl.OPERATION_SHIFT}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnResultOperatingShiftTbl.OPERATION_SHIFT);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnResultOperatingShiftTbl.START_TIME}", SqlDbType.DateTime)).Value = udpateItem.Field<DateTime>(ColumnResultOperatingShiftTbl.START_TIME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnResultOperatingShiftTbl.END_TIME}", SqlDbType.DateTime)).Value = udpateItem.Field<DateTime>(ColumnResultOperatingShiftTbl.END_TIME);

                            String paramStr = "";
                            if (SqlDBConnector.CreateParamsText(cmd.Parameters, out paramStr))
                            {
                                sb.Append(paramStr);
                            }

                            cmd.Transaction = transaction;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Query:{sb.ToString()}");
                logger.Error(ex.Message + "," + ex.StackTrace);
                ret = false;
            }
            finally
            {
                CloseConnection();
            }
            return ret;
        }

        public bool Delete(DataTable deleteDataTable)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// DELETE処理の実行
        /// </summary>
        /// <param name="deleteDateTime"></param>
        /// <returns></returns>
        public bool Delete(DateTime deleteDateTime)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Delete:{ColumnResultOperatingShiftTbl.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Clear();
                    sb.Append(String.Format("DELETE {0} ", ColumnResultOperatingShiftTbl.TABLE_NAME));
                    sb.Append(String.Format("WHERE {0} < @{0} ", ColumnResultOperatingShiftTbl.OPERATION_DATE));
                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnResultOperatingShiftTbl.OPERATION_DATE}", SqlDbType.Date)).Value = deleteDateTime;

                    String paramStr = "";
                    if (SqlDBConnector.CreateParamsText(cmd.Parameters, out paramStr))
                    {
                        sb.Append(paramStr);
                    }

                    cmd.ExecuteNonQuery();
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Query:{sb.ToString()}");
                logger.Error(ex.Message + "," + ex.StackTrace);
                ret = false;
            }
            finally
            {
                CloseConnection();
            }
            return ret;
        }
        #endregion
    }
}