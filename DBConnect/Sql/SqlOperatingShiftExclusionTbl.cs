using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using log4net;

namespace DBConnect.SQL
{
    public class ColumnOperatingShiftExclusionTbl
    {
        public const String TABLE_NAME = "Operating_Shift_Exclusion_Tbl";
        public const String OPERATION_DATE = "OperationDate";
        public const String OPERATION_SHIFT = "OperationShift";
        public const String EXCLUSION_IDX = "ExclusionIdx";
        public const String EXCLUSION_START_TIME = "ExclusionStartTime";
        public const String EXCLUSION_END_TIME = "ExclusionEndTime";
        public const String EXCLUSION_TIME = "ExclusionTime";
        public const String EXCLUSION_CHECK = "ExclusionCheck";
        public const String EXCLUSION_REMARK = "ExclusionRemark";
        public const String EXCLUSION_CLASS = "ExclusionClass";
    }

    public class SqlOperatingShiftExclusionTbl : ISqlBase
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
        public SqlOperatingShiftExclusionTbl(String connectString)
        {
            mConnectString = connectString;
        }

        #endregion

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
            DataTable dt = new DataTable(ColumnOperatingShiftExclusionTbl.TABLE_NAME);
            StringBuilder sb = new StringBuilder();
            try
            {
                logger.Debug($"Select:{ColumnOperatingShiftExclusionTbl.TABLE_NAME}");
                
                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT * ");
                    sb.Append(String.Format("FROM {0} ", ColumnOperatingShiftExclusionTbl.TABLE_NAME));
                    sb.Append(String.Format("Order by {0}, {1};", ColumnOperatingShiftExclusionTbl.OPERATION_DATE, ColumnOperatingShiftExclusionTbl.OPERATION_SHIFT));

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
            DataTable dt = new DataTable(ColumnOperatingShiftExclusionTbl.TABLE_NAME);
            StringBuilder sb = new StringBuilder();
            try
            {
                logger.Debug($"ConditionalSelect:{ColumnOperatingShiftExclusionTbl.TABLE_NAME}");
                
                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT * ");
                    sb.Append(String.Format("FROM {0} ", ColumnOperatingShiftExclusionTbl.TABLE_NAME));
                    sb.Append("WHERE 1 = 1 ");
                    sb.Append(String.Format("And {0} = @{0} ", ColumnOperatingShiftExclusionTbl.OPERATION_DATE));
                    if (OperationShift != 0)
                    {
                        sb.Append(String.Format("And {0} = @{0} ", ColumnOperatingShiftExclusionTbl.OPERATION_SHIFT));
                    }
                    sb.Append(String.Format("Order by {0}, {1};", ColumnOperatingShiftExclusionTbl.OPERATION_DATE, ColumnOperatingShiftExclusionTbl.OPERATION_SHIFT));

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(String.Format("@{0}", ColumnOperatingShiftExclusionTbl.OPERATION_DATE), SqlDbType.DateTime).Value = OperationDate;
                    if (OperationShift != 0)
                    {
                        cmd.Parameters.Add(String.Format("@{0}", ColumnOperatingShiftExclusionTbl.OPERATION_SHIFT), SqlDbType.Int).Value = OperationShift;
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
                logger.Debug($"Upsert:{ColumnOperatingShiftExclusionTbl.TABLE_NAME}");

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
                            sb.Append(string.Format("MERGE INTO {0} AS T1 ", ColumnOperatingShiftExclusionTbl.TABLE_NAME));
                            sb.Append(string.Format("USING (SELECT @{0} AS {0}, @{1} AS {1}, @{2} AS {2}) AS T2 ", 
                                ColumnOperatingShiftExclusionTbl.OPERATION_DATE, 
                                ColumnOperatingShiftExclusionTbl.OPERATION_SHIFT,
                                ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX
                                ));
                            sb.Append(string.Format("ON (T1.{0} = T2.{0} AND T1.{1} = T2.{1} AND T1.{2} = T2.{2}) ", 
                                ColumnOperatingShiftExclusionTbl.OPERATION_DATE,
                                ColumnOperatingShiftExclusionTbl.OPERATION_SHIFT,
                                ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX));
                            sb.Append("WHEN MATCHED THEN UPDATE SET ");
                            sb.Append(string.Format("{0} = @{0}, ", ColumnOperatingShiftExclusionTbl.EXCLUSION_START_TIME));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnOperatingShiftExclusionTbl.EXCLUSION_END_TIME));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnOperatingShiftExclusionTbl.EXCLUSION_TIME));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnOperatingShiftExclusionTbl.EXCLUSION_CHECK));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnOperatingShiftExclusionTbl.EXCLUSION_REMARK));
                            sb.Append(string.Format("{0} = @{0} ", ColumnOperatingShiftExclusionTbl.EXCLUSION_CLASS));
                            sb.Append("WHEN NOT MATCHED THEN INSERT (");
                            sb.Append(string.Format("{0}, ", ColumnOperatingShiftExclusionTbl.OPERATION_DATE));
                            sb.Append(string.Format("{0}, ", ColumnOperatingShiftExclusionTbl.OPERATION_SHIFT));
                            sb.Append(string.Format("{0}, ", ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX));
                            sb.Append(string.Format("{0}, ", ColumnOperatingShiftExclusionTbl.EXCLUSION_START_TIME));
                            sb.Append(string.Format("{0}, ", ColumnOperatingShiftExclusionTbl.EXCLUSION_END_TIME));
                            sb.Append(string.Format("{0}, ", ColumnOperatingShiftExclusionTbl.EXCLUSION_TIME));
                            sb.Append(string.Format("{0}, ", ColumnOperatingShiftExclusionTbl.EXCLUSION_CHECK));
                            sb.Append(string.Format("{0}, ", ColumnOperatingShiftExclusionTbl.EXCLUSION_REMARK));
                            sb.Append(string.Format("{0} ", ColumnOperatingShiftExclusionTbl.EXCLUSION_CLASS));
                            sb.Append(") VALUES (");
                            sb.Append(string.Format("@{0}, ", ColumnOperatingShiftExclusionTbl.OPERATION_DATE));
                            sb.Append(string.Format("@{0}, ", ColumnOperatingShiftExclusionTbl.OPERATION_SHIFT));
                            sb.Append(string.Format("@{0}, ", ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX));
                            sb.Append(string.Format("@{0}, ", ColumnOperatingShiftExclusionTbl.EXCLUSION_START_TIME));
                            sb.Append(string.Format("@{0}, ", ColumnOperatingShiftExclusionTbl.EXCLUSION_END_TIME));
                            sb.Append(string.Format("@{0}, ", ColumnOperatingShiftExclusionTbl.EXCLUSION_TIME));
                            sb.Append(string.Format("@{0}, ", ColumnOperatingShiftExclusionTbl.EXCLUSION_CHECK));
                            sb.Append(string.Format("@{0}, ", ColumnOperatingShiftExclusionTbl.EXCLUSION_REMARK));
                            sb.Append(string.Format("@{0});", ColumnOperatingShiftExclusionTbl.EXCLUSION_CLASS));

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftExclusionTbl.OPERATION_DATE}", SqlDbType.Date)).Value = udpateItem.Field<DateTime>(ColumnOperatingShiftExclusionTbl.OPERATION_DATE);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftExclusionTbl.OPERATION_SHIFT}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnOperatingShiftExclusionTbl.OPERATION_SHIFT);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_IDX);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftExclusionTbl.EXCLUSION_START_TIME}", SqlDbType.DateTime)).Value = udpateItem.Field<DateTime>(ColumnOperatingShiftExclusionTbl.EXCLUSION_START_TIME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftExclusionTbl.EXCLUSION_END_TIME}", SqlDbType.DateTime)).Value = udpateItem.Field<DateTime>(ColumnOperatingShiftExclusionTbl.EXCLUSION_END_TIME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftExclusionTbl.EXCLUSION_TIME}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_TIME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftExclusionTbl.EXCLUSION_CHECK}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnOperatingShiftExclusionTbl.EXCLUSION_CHECK);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftExclusionTbl.EXCLUSION_REMARK}", SqlDbType.NVarChar)).Value = udpateItem.Field<String>(ColumnOperatingShiftExclusionTbl.EXCLUSION_REMARK);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftExclusionTbl.EXCLUSION_CLASS}", SqlDbType.UniqueIdentifier)).Value = udpateItem.Field<Guid>(ColumnOperatingShiftExclusionTbl.EXCLUSION_CLASS);

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
                logger.Debug($"Delete:{ColumnOperatingShiftExclusionTbl.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {

                    sb.Clear();
                    sb.Append(String.Format("DELETE {0} ", ColumnOperatingShiftExclusionTbl.TABLE_NAME));
                    sb.Append(String.Format("WHERE {0} < @{0} ", ColumnOperatingShiftExclusionTbl.OPERATION_DATE));
                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftExclusionTbl.OPERATION_DATE}", SqlDbType.Date)).Value = deleteDateTime;

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
    }
}
