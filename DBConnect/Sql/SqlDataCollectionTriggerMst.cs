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
    public class ColumnDataCollectionTriggerMst
    {
        public const String TABLE_NAME = "DataCollection_Trigger_Mst";
        public const String COMPOSITION_ID = "CompositionId";
        public const String PRODUCT_TYPE_ID = "ProductTypeId";
        public const String PROCESS_IDX = "ProcessIdx";
        public const String TRIGGER_TYPE = "TriggerType";
        public const String IS_PRODUCTION_QUANTITY_COUNTER = "IsProductionQuantityCounter";
        public const String INTERVAL_FILTER = "IntervalFilter";
        public const String SENSOR_ID_1 = "SensorId1";
        public const String EXCLUSION_FILTER_1 = "ExclusionFilter1";
        public const String SENSOR_ID_2 = "SensorId2";
        public const String EXCLUSION_FILTER_2 = "ExclusionFilter2";
    }

    public class SqlDataCollectionTriggerMst : ISqlBase
    {

        #region <Field>
        /// <summary>
        /// 接続文字列
        /// </summary>
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
        public SqlDataCollectionTriggerMst(String connectString)
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

        public DataTable Select()
        {
            DataTable dt = new DataTable(ColumnDataCollectionTriggerMst.TABLE_NAME);
            StringBuilder sb = new StringBuilder();
            try
            {
                logger.Debug($"Select:{ColumnDataCollectionTriggerMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT * ");
                    sb.Append(String.Format("FROM {0} ", ColumnDataCollectionTriggerMst.TABLE_NAME));
                    sb.Append(String.Format("Order by {0};", ColumnDataCollectionTriggerMst.COMPOSITION_ID));

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
        /// SELECT処理
        /// </summary>
        public DataTable Select(Guid compositionId, Guid productTypeId, int processIdx = -1)
        {
            DataTable dt = new DataTable(ColumnDataCollectionTriggerMst.TABLE_NAME);
            StringBuilder sb = new StringBuilder();
            try
            {
                logger.Debug($"Select:{ColumnDataCollectionTriggerMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT * ");
                    sb.Append(String.Format("FROM {0} ", ColumnDataCollectionTriggerMst.TABLE_NAME));
                    sb.Append(String.Format("Where {0} = @{0} ", ColumnDataCollectionTriggerMst.COMPOSITION_ID));
                    sb.Append(String.Format("And {0} = @{0} ", ColumnDataCollectionTriggerMst.PRODUCT_TYPE_ID));
                    if (processIdx > -1)
                    {
                        sb.Append(String.Format("And {0} = @{0};", ColumnDataCollectionTriggerMst.PROCESS_IDX));
                    }

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter(String.Format("@{0}", ColumnDataCollectionTriggerMst.COMPOSITION_ID), SqlDbType.UniqueIdentifier)).Value = compositionId;
                    cmd.Parameters.Add(new SqlParameter(String.Format("@{0}", ColumnDataCollectionTriggerMst.PRODUCT_TYPE_ID), SqlDbType.UniqueIdentifier)).Value = productTypeId;
                    if (processIdx > -1)
                    {
                        cmd.Parameters.Add(new SqlParameter(String.Format("@{0}", ColumnDataCollectionTriggerMst.PROCESS_IDX), SqlDbType.Int)).Value = processIdx;
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
        /// Upsert処理の実行
        /// </summary>
        /// <param name="updateDataTable"></param>
        /// <returns>処理結果 true=成功 false=失敗</returns>
        public bool Upsert(DataTable updateDataTable, SqlTransaction transaction)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Upsert:{ColumnDataCollectionTriggerMst.TABLE_NAME}");

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
                    foreach (DataRow rowItem in updateDataTable.Rows)
                    {
                        if (rowItem.RowState == DataRowState.Modified || rowItem.RowState == DataRowState.Added)
                        {
                            sb.Clear();
                            sb.Append(string.Format("MERGE INTO {0} AS T1 ", ColumnDataCollectionTriggerMst.TABLE_NAME));
                            sb.Append("USING (SELECT ");
                            sb.Append(string.Format("@{0} AS {0} ", ColumnDataCollectionTriggerMst.COMPOSITION_ID));
                            sb.Append(string.Format(",@{0} AS {0} ", ColumnDataCollectionTriggerMst.PRODUCT_TYPE_ID));
                            sb.Append(string.Format(",@{0} AS {0}) AS T2 ", ColumnDataCollectionTriggerMst.PROCESS_IDX));

                            sb.Append(string.Format("ON (T1.{0} = T2.{0} ", ColumnDataCollectionTriggerMst.COMPOSITION_ID));
                            sb.Append(string.Format("AND T1.{0} = T2.{0} ", ColumnDataCollectionTriggerMst.PRODUCT_TYPE_ID));
                            sb.Append(string.Format("AND T1.{0} = T2.{0}) ", ColumnDataCollectionTriggerMst.PROCESS_IDX));

                            sb.Append("WHEN MATCHED THEN UPDATE SET ");
                            sb.Append(string.Format("{0} = @{0} ", ColumnDataCollectionTriggerMst.TRIGGER_TYPE));
                            sb.Append(string.Format(",{0} = @{0} ", ColumnDataCollectionTriggerMst.IS_PRODUCTION_QUANTITY_COUNTER));
                            sb.Append(string.Format(",{0} = @{0} ", ColumnDataCollectionTriggerMst.INTERVAL_FILTER));
                            sb.Append(string.Format(",{0} = @{0} ", ColumnDataCollectionTriggerMst.SENSOR_ID_1));
                            sb.Append(string.Format(",{0} = @{0} ", ColumnDataCollectionTriggerMst.EXCLUSION_FILTER_1));
                            sb.Append(string.Format(",{0} = @{0} ", ColumnDataCollectionTriggerMst.SENSOR_ID_2));
                            sb.Append(string.Format(",{0} = @{0} ", ColumnDataCollectionTriggerMst.EXCLUSION_FILTER_2));

                            sb.Append(" WHEN NOT MATCHED THEN INSERT (");
                            sb.Append(string.Format("{0} ", ColumnDataCollectionTriggerMst.COMPOSITION_ID));
                            sb.Append(string.Format(",{0} ", ColumnDataCollectionTriggerMst.PRODUCT_TYPE_ID));
                            sb.Append(string.Format(",{0} ", ColumnDataCollectionTriggerMst.PROCESS_IDX));
                            sb.Append(string.Format(",{0} ", ColumnDataCollectionTriggerMst.TRIGGER_TYPE));
                            sb.Append(string.Format(",{0} ", ColumnDataCollectionTriggerMst.IS_PRODUCTION_QUANTITY_COUNTER));
                            sb.Append(string.Format(",{0} ", ColumnDataCollectionTriggerMst.INTERVAL_FILTER));
                            sb.Append(string.Format(",{0} ", ColumnDataCollectionTriggerMst.SENSOR_ID_1));
                            sb.Append(string.Format(",{0} ", ColumnDataCollectionTriggerMst.EXCLUSION_FILTER_1));
                            sb.Append(string.Format(",{0} ", ColumnDataCollectionTriggerMst.SENSOR_ID_2));
                            sb.Append(string.Format(",{0} ", ColumnDataCollectionTriggerMst.EXCLUSION_FILTER_2));
                            sb.Append(") ");
                            sb.Append("VALUES (");
                            sb.Append(string.Format("@{0} ", ColumnDataCollectionTriggerMst.COMPOSITION_ID));
                            sb.Append(string.Format(",@{0} ", ColumnDataCollectionTriggerMst.PRODUCT_TYPE_ID));
                            sb.Append(string.Format(",@{0} ", ColumnDataCollectionTriggerMst.PROCESS_IDX));
                            sb.Append(string.Format(",@{0} ", ColumnDataCollectionTriggerMst.TRIGGER_TYPE));
                            sb.Append(string.Format(",@{0} ", ColumnDataCollectionTriggerMst.IS_PRODUCTION_QUANTITY_COUNTER));
                            sb.Append(string.Format(",@{0} ", ColumnDataCollectionTriggerMst.INTERVAL_FILTER));
                            sb.Append(string.Format(",@{0} ", ColumnDataCollectionTriggerMst.SENSOR_ID_1));
                            sb.Append(string.Format(",@{0} ", ColumnDataCollectionTriggerMst.EXCLUSION_FILTER_1));
                            sb.Append(string.Format(",@{0} ", ColumnDataCollectionTriggerMst.SENSOR_ID_2));
                            sb.Append(string.Format(",@{0} ", ColumnDataCollectionTriggerMst.EXCLUSION_FILTER_2));
                            sb.Append("); ");

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionTriggerMst.COMPOSITION_ID}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnDataCollectionTriggerMst.COMPOSITION_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionTriggerMst.PRODUCT_TYPE_ID}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnDataCollectionTriggerMst.PRODUCT_TYPE_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionTriggerMst.PROCESS_IDX}", SqlDbType.Int)).Value = rowItem.Field<int>(ColumnDataCollectionTriggerMst.PROCESS_IDX);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionTriggerMst.TRIGGER_TYPE}", SqlDbType.Int)).Value = rowItem.Field<int>(ColumnDataCollectionTriggerMst.TRIGGER_TYPE);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionTriggerMst.IS_PRODUCTION_QUANTITY_COUNTER}", SqlDbType.Int)).Value = rowItem.Field<int>(ColumnDataCollectionTriggerMst.IS_PRODUCTION_QUANTITY_COUNTER);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionTriggerMst.INTERVAL_FILTER}", SqlDbType.Decimal)).Value = rowItem.Field<decimal>(ColumnDataCollectionTriggerMst.INTERVAL_FILTER);
                            // 2022/06/13 西部修正 オール0のGUIDの場合はNULLでDB登録を行う
                            if (rowItem.Field<Guid>(ColumnDataCollectionTriggerMst.SENSOR_ID_1) != new Guid("00000000-0000-0000-0000-000000000000"))
                            {
                                cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionTriggerMst.SENSOR_ID_1}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnDataCollectionTriggerMst.SENSOR_ID_1);
                            }
                            else
                            {
                                cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionTriggerMst.SENSOR_ID_1}", SqlDbType.UniqueIdentifier)).Value = System.Data.SqlTypes.SqlString.Null;
                            }
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionTriggerMst.EXCLUSION_FILTER_1}", SqlDbType.Decimal)).Value = rowItem.Field<decimal>(ColumnDataCollectionTriggerMst.EXCLUSION_FILTER_1);

                            if (rowItem.Field<Guid>(ColumnDataCollectionTriggerMst.SENSOR_ID_2) != new Guid("00000000-0000-0000-0000-000000000000"))
                            {
                                cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionTriggerMst.SENSOR_ID_2}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnDataCollectionTriggerMst.SENSOR_ID_2);
                            }
                            else
                            {
                                cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionTriggerMst.SENSOR_ID_2}", SqlDbType.UniqueIdentifier)).Value = System.Data.SqlTypes.SqlString.Null;
                            }
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionTriggerMst.EXCLUSION_FILTER_2}", SqlDbType.Decimal)).Value = rowItem.Field<decimal>(ColumnDataCollectionTriggerMst.EXCLUSION_FILTER_2);

                            String paramStr = "";
                            if (SqlDBConnector.CreateParamsText(cmd.Parameters, out paramStr))
                            {
                                sb.Append(paramStr);
                            }

                            cmd.Transaction = transaction;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                ret = true;
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
    }
}
