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
    public static class PlanOperatingShiftTblColumn
    {
        public const String TABLE_NAME = "Plan_Operating_Shift_Tbl";
        public const String OPERATION_DATE = "OperationDate";
        public const String OPERATION_SHIFT = "OperationShift";
        public const String PRODUCTION_QUANTITY = "ProductionQuantity";
        public const String START_TIME = "StartTime";
        public const String END_TIME = "EndTime";
        public const String OPERATION_SECOND = "OperationSecond";
        public const String USE_FLAG = "UseFlag";
    }

    public class SqlPlanOperatingShiftTbl : ISqlBase
    {

        #region <Field>
        private String mConnectString;

        /// <summary>
        /// ログインスタンス
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region <Property>
        public SqlDBConnector DbConnector { get; set; }
        #endregion

        #region <Constructor>
        public SqlPlanOperatingShiftTbl(String connectString)
        {
            mConnectString = connectString;
        }
        #endregion

        #region <Method>


        /// <summary>
        /// DB接続オープン
        /// </summary>
        /// <returns></returns>
        public Boolean OpenConnection()
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
        /// DB接続クローズ
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
            DataTable dt = new DataTable(PlanOperatingShiftTblColumn.TABLE_NAME);
            StringBuilder sb = new StringBuilder();
            try
            {
                logger.Debug($"Select:{PlanOperatingShiftTblColumn.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT * ");
                    sb.Append(String.Format("FROM {0} ", PlanOperatingShiftTblColumn.TABLE_NAME));
                    sb.Append(String.Format("Order by {0}, {1};", PlanOperatingShiftTblColumn.OPERATION_DATE, PlanOperatingShiftTblColumn.OPERATION_SHIFT));

                    cmd.CommandText = sb.ToString();

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
            DataTable dt = new DataTable(PlanOperatingShiftTblColumn.TABLE_NAME);
            StringBuilder sb = new StringBuilder();
            try
            {
                //logger.Debug($"ConditionalSelect:{PlanOperatingShiftTblColumn.TABLE_NAME}");
                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT * ");
                    sb.Append(String.Format("FROM {0} ", PlanOperatingShiftTblColumn.TABLE_NAME));
                    sb.Append("WHERE 1 = 1 ");
                    sb.Append(String.Format("And {0} = @{0} ", PlanOperatingShiftTblColumn.OPERATION_DATE));
                    if (OperationShift != 0)
                    {
                        sb.Append(String.Format("And {0} = @{0} ", PlanOperatingShiftTblColumn.OPERATION_SHIFT));
                    }
                    sb.Append(String.Format("Order by {0}, {1};", PlanOperatingShiftTblColumn.OPERATION_DATE, PlanOperatingShiftTblColumn.OPERATION_SHIFT));

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(String.Format("@{0}", PlanOperatingShiftTblColumn.OPERATION_DATE), SqlDbType.DateTime).Value = OperationDate;
                    if (OperationShift != 0)
                    {
                        cmd.Parameters.Add(String.Format("@{0}", PlanOperatingShiftTblColumn.OPERATION_SHIFT), SqlDbType.Int).Value = OperationShift;
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

        public bool Insert(DataTable insertDataTable)
        {
            throw new NotImplementedException();
        }

        public bool Update(DataTable updateDataTable)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// UPSERT処理の実行
        /// </summary>
        /// <param name="updateDataTable"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public bool Upsert(DataTable updateDataTable, SqlTransaction transaction)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Upsert:{PlanOperatingShiftTblColumn.TABLE_NAME}");

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
                        if (udpateItem.RowState == DataRowState.Modified || udpateItem.RowState == DataRowState.Added)
                        {
                            sb.Clear();
                            sb.Append(string.Format("MERGE INTO {0} AS T1 ", PlanOperatingShiftTblColumn.TABLE_NAME));
                            sb.Append(string.Format("USING (SELECT @{0} AS {0}, @{1} AS {1}) AS T2 ", PlanOperatingShiftTblColumn.OPERATION_DATE, PlanOperatingShiftTblColumn.OPERATION_SHIFT));
                            sb.Append(string.Format("ON (T1.{0} = T2.{0} AND T1.{1} = T2.{1}) ", PlanOperatingShiftTblColumn.OPERATION_DATE, PlanOperatingShiftTblColumn.OPERATION_SHIFT));
                            sb.Append("WHEN MATCHED THEN UPDATE SET ");
                            sb.Append(string.Format("{0} = @{0}, ", PlanOperatingShiftTblColumn.PRODUCTION_QUANTITY));
                            sb.Append(string.Format("{0} = @{0}, ", PlanOperatingShiftTblColumn.START_TIME));
                            sb.Append(string.Format("{0} = @{0}, ", PlanOperatingShiftTblColumn.END_TIME));
                            sb.Append(string.Format("{0} = @{0}, ", PlanOperatingShiftTblColumn.OPERATION_SECOND));
                            sb.Append(string.Format("{0} = @{0} ", PlanOperatingShiftTblColumn.USE_FLAG));
                            sb.Append("WHEN NOT MATCHED THEN INSERT (");
                            sb.Append(string.Format("{0}, ", PlanOperatingShiftTblColumn.OPERATION_DATE));
                            sb.Append(string.Format("{0}, ", PlanOperatingShiftTblColumn.OPERATION_SHIFT));
                            sb.Append(string.Format("{0}, ", PlanOperatingShiftTblColumn.PRODUCTION_QUANTITY));
                            sb.Append(string.Format("{0}, ", PlanOperatingShiftTblColumn.START_TIME));
                            sb.Append(string.Format("{0}, ", PlanOperatingShiftTblColumn.END_TIME));
                            sb.Append(string.Format("{0}, ", PlanOperatingShiftTblColumn.OPERATION_SECOND));
                            sb.Append(string.Format("{0} ", PlanOperatingShiftTblColumn.USE_FLAG));
                            sb.Append(") VALUES (");
                            sb.Append(string.Format("@{0}, ", PlanOperatingShiftTblColumn.OPERATION_DATE));
                            sb.Append(string.Format("@{0}, ", PlanOperatingShiftTblColumn.OPERATION_SHIFT));
                            sb.Append(string.Format("@{0}, ", PlanOperatingShiftTblColumn.PRODUCTION_QUANTITY));
                            sb.Append(string.Format("@{0}, ", PlanOperatingShiftTblColumn.START_TIME));
                            sb.Append(string.Format("@{0}, ", PlanOperatingShiftTblColumn.END_TIME));
                            sb.Append(string.Format("@{0}, ", PlanOperatingShiftTblColumn.OPERATION_SECOND));
                            sb.Append(string.Format("@{0} ", PlanOperatingShiftTblColumn.USE_FLAG));
                            sb.Append(");");

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{PlanOperatingShiftTblColumn.OPERATION_DATE}", SqlDbType.Date)).Value = udpateItem.Field<DateTime>(PlanOperatingShiftTblColumn.OPERATION_DATE);
                            cmd.Parameters.Add(new SqlParameter($"@{PlanOperatingShiftTblColumn.OPERATION_SHIFT}", SqlDbType.Int)).Value = udpateItem.Field<int>(PlanOperatingShiftTblColumn.OPERATION_SHIFT);
                            cmd.Parameters.Add(new SqlParameter($"@{PlanOperatingShiftTblColumn.PRODUCTION_QUANTITY}", SqlDbType.Int)).Value = udpateItem.Field<int>(PlanOperatingShiftTblColumn.PRODUCTION_QUANTITY);
                            cmd.Parameters.Add(new SqlParameter($"@{PlanOperatingShiftTblColumn.START_TIME}", SqlDbType.DateTime)).Value = udpateItem.Field<DateTime>(PlanOperatingShiftTblColumn.START_TIME);
                            cmd.Parameters.Add(new SqlParameter($"@{PlanOperatingShiftTblColumn.END_TIME}", SqlDbType.DateTime)).Value = udpateItem.Field<DateTime>(PlanOperatingShiftTblColumn.END_TIME);
                            cmd.Parameters.Add(new SqlParameter($"@{PlanOperatingShiftTblColumn.OPERATION_SECOND}", SqlDbType.Int)).Value = udpateItem.Field<int>(PlanOperatingShiftTblColumn.OPERATION_SECOND);
                            cmd.Parameters.Add(new SqlParameter($"@{PlanOperatingShiftTblColumn.USE_FLAG}", SqlDbType.Bit)).Value = udpateItem.Field<Boolean>(PlanOperatingShiftTblColumn.USE_FLAG);

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

        /// <summary>
        /// DELETE処理の実行
        /// </summary>
        /// <param name="deleteDataTable"></param>
        /// <returns></returns>
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
                logger.Debug($"Delete:{PlanOperatingShiftTblColumn.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {

                    sb.Clear();
                    sb.Append(String.Format("DELETE {0} ", PlanOperatingShiftTblColumn.TABLE_NAME));
                    sb.Append(String.Format("WHERE {0} < @{0} ", PlanOperatingShiftTblColumn.OPERATION_DATE));
                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter($"@{PlanOperatingShiftTblColumn.OPERATION_DATE}", SqlDbType.Date)).Value = deleteDateTime;

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
                logger.Error($"Error Query:{sb.ToString()}");
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
