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
    public class ColumnStandardValProductTypeProcessMst
    {
        public const String TABLE_NAME = "StandardVal_ProductType_Process_Mst";
        public const String COMPOSITION_ID = "CompositionId";
        public const String PRODUCT_TYPE_ID = "ProductTypeId";
        public const String PROCESS_IDX = "ProcessIdx";
        public const String CYCLE_TIME_MAX = "CycleTimeMax";
        public const String CYCLE_TIME_AVERAGE = "CycleTimeAverage";
        public const String CYCLE_TIME_MIN = "CycleTimeMin";
        public const String CYCLE_TIME_DISPERSION = "CycleTimeDispersion";
        public const String CYCLE_TIME_UPPER = "CycleTimeUpper";
        public const String CYCLE_TIME_LOWER = "CycleTimeLower";
        public const String ANCILLARY = "Ancillary";
        public const String SETUP = "Setup";
        public const String USE_FLAG = "UseFlag";

    }
    public class SqlStandardValProductTypeProcessMst : ISqlBase
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
        public SqlStandardValProductTypeProcessMst(String connectString)
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// 条件付きSELECT処理
        /// </summary>
        /// <returns></returns>
        public DataTable Select(Guid compositionId, Guid productTypeId, int processIdx = -1)
        {
            DataTable dt = new DataTable(ColumnStandardValProductTypeProcessMst.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Select:{ColumnStandardValProductTypeProcessMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT * ");
                    sb.Append(String.Format("FROM {0} ", ColumnStandardValProductTypeProcessMst.TABLE_NAME));
                    sb.Append(String.Format("Where {0} = @{0} ", ColumnStandardValProductTypeProcessMst.COMPOSITION_ID));
                    sb.Append(String.Format("AND {0} = @{0} ", ColumnStandardValProductTypeProcessMst.PRODUCT_TYPE_ID));
                    if (processIdx > -1)
                    {
                        sb.Append(string.Format("AND {0} = @{0} ", ColumnStandardValProductTypeProcessMst.PROCESS_IDX));
                    }

                    sb.Append(String.Format("Order by {0};", ColumnStandardValProductTypeProcessMst.PROCESS_IDX));

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.COMPOSITION_ID}", SqlDbType.UniqueIdentifier)).Value = compositionId;
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.PRODUCT_TYPE_ID}", SqlDbType.UniqueIdentifier)).Value = productTypeId;
                    if (processIdx > -1)
                    {
                        cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.PROCESS_IDX}", SqlDbType.Int)).Value = processIdx;
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
        /// 標準値品番工程別マスタとデータ収集トリガマスタからサイクル実績を作成するために必要なパラメータを取得する
        /// 編成または品番QRの読み込み時のみ発生し、一意の編成・品番の組み合わせからCT上下限、良品カウンタを取得して保持する
        /// </summary>
        /// <param name="compositionId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public DataTable SelectCycleDataWithDataCollectionTrigerMst(Guid compositionId, Guid productId, int processIdx = -1)
        {
            DataTable dt = new DataTable(ColumnCompositionMst.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"SelectCycleDataWithDataCollectionTrigerMst:{ColumnCompositionMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append(String.Format("SELECT svptpm.{0}, svptpm.{1}, dctm.{2} ",
                        ColumnStandardValProductTypeProcessMst.CYCLE_TIME_UPPER,
                        ColumnStandardValProductTypeProcessMst.CYCLE_TIME_LOWER,
                        ColumnDataCollectionTriggerMst.IS_PRODUCTION_QUANTITY_COUNTER));
                    sb.Append(string.Format("FROM {0} svptpm ", ColumnStandardValProductTypeProcessMst.TABLE_NAME));
                    sb.Append(string.Format("LEFT JOIN {0} dctm ", ColumnDataCollectionTriggerMst.TABLE_NAME));
                    sb.Append(string.Format("ON svptpm.{0} = dctm.{0} ", ColumnStandardValProductTypeProcessMst.COMPOSITION_ID));
                    sb.Append(string.Format("AND svptpm.{0} = dctm.{0} ", ColumnStandardValProductTypeProcessMst.PRODUCT_TYPE_ID));
                    sb.Append(string.Format("AND svptpm.{0} = dctm.{0} ", ColumnStandardValProductTypeProcessMst.PROCESS_IDX));

                    sb.Append(string.Format("Where svptpm.{0} = @{0} ", ColumnStandardValProductTypeProcessMst.COMPOSITION_ID));
                    sb.Append(string.Format("AND svptpm.{0} = @{0} ", ColumnStandardValProductTypeProcessMst.PRODUCT_TYPE_ID));
                    if (processIdx > -1)
                    {
                        sb.Append(string.Format("AND svptpm.{0} = @{0} ", ColumnStandardValProductTypeProcessMst.PROCESS_IDX));
                    }

                    sb.Append(string.Format("ORDER BY svptpm.{0} ", ColumnStandardValProductTypeProcessMst.PROCESS_IDX));

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.COMPOSITION_ID}", SqlDbType.UniqueIdentifier)).Value = compositionId;
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.PRODUCT_TYPE_ID}", SqlDbType.UniqueIdentifier)).Value = productId;
                    if (processIdx > -1)
                    {
                        cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.PROCESS_IDX}", SqlDbType.Int)).Value = processIdx;
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
                logger.Debug($"Upsert:{ColumnStandardValProductTypeProcessMst.TABLE_NAME}");

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
                            sb.Append(string.Format("MERGE INTO {0} AS T1 ", ColumnStandardValProductTypeProcessMst.TABLE_NAME));
                            sb.Append("USING (SELECT ");
                            sb.Append(string.Format("@{0} AS {0} ", ColumnStandardValProductTypeProcessMst.COMPOSITION_ID));
                            sb.Append(string.Format(",@{0} AS {0} ", ColumnStandardValProductTypeProcessMst.PRODUCT_TYPE_ID));
                            sb.Append(string.Format(",@{0} AS {0}) AS T2 ", ColumnStandardValProductTypeProcessMst.PROCESS_IDX));

                            sb.Append(string.Format("ON (T1.{0} = T2.{0} ", ColumnStandardValProductTypeProcessMst.COMPOSITION_ID));
                            sb.Append(string.Format("AND T1.{0} = T2.{0} ", ColumnStandardValProductTypeProcessMst.PRODUCT_TYPE_ID));
                            sb.Append(string.Format("AND T1.{0} = T2.{0}) ", ColumnStandardValProductTypeProcessMst.PROCESS_IDX));

                            sb.Append("WHEN MATCHED THEN UPDATE SET ");
                            sb.Append(string.Format("{0} = @{0} ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MAX));
                            sb.Append(string.Format(",{0} = @{0} ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_AVERAGE));
                            sb.Append(string.Format(",{0} = @{0} ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MIN));
                            sb.Append(string.Format(",{0} = @{0} ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_DISPERSION));
                            sb.Append(string.Format(",{0} = @{0} ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_UPPER));
                            sb.Append(string.Format(",{0} = @{0} ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_LOWER));
                            sb.Append(string.Format(",{0} = @{0} ", ColumnStandardValProductTypeProcessMst.ANCILLARY));
                            sb.Append(string.Format(",{0} = @{0} ", ColumnStandardValProductTypeProcessMst.SETUP));
                            sb.Append(string.Format(",{0} = @{0} ", ColumnStandardValProductTypeProcessMst.USE_FLAG));

                            sb.Append(" WHEN NOT MATCHED THEN INSERT (");
                            sb.Append(string.Format("{0} ", ColumnStandardValProductTypeProcessMst.COMPOSITION_ID));
                            sb.Append(string.Format(",{0} ", ColumnStandardValProductTypeProcessMst.PRODUCT_TYPE_ID));
                            sb.Append(string.Format(",{0} ", ColumnStandardValProductTypeProcessMst.PROCESS_IDX));
                            sb.Append(string.Format(",{0} ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MAX));
                            sb.Append(string.Format(",{0} ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_AVERAGE));
                            sb.Append(string.Format(",{0} ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MIN));
                            sb.Append(string.Format(",{0} ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_DISPERSION));
                            sb.Append(string.Format(",{0} ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_UPPER));
                            sb.Append(string.Format(",{0} ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_LOWER));
                            sb.Append(string.Format(",{0} ", ColumnStandardValProductTypeProcessMst.ANCILLARY));
                            sb.Append(string.Format(",{0} ", ColumnStandardValProductTypeProcessMst.SETUP));
                            sb.Append(string.Format(",{0} ", ColumnStandardValProductTypeProcessMst.USE_FLAG));
                            sb.Append(")VALUES (");
                            sb.Append(string.Format("@{0} ", ColumnStandardValProductTypeProcessMst.COMPOSITION_ID));
                            sb.Append(string.Format(",@{0} ", ColumnStandardValProductTypeProcessMst.PRODUCT_TYPE_ID));
                            sb.Append(string.Format(",@{0} ", ColumnStandardValProductTypeProcessMst.PROCESS_IDX));
                            sb.Append(string.Format(",@{0} ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MAX));
                            sb.Append(string.Format(",@{0} ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_AVERAGE));
                            sb.Append(string.Format(",@{0} ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MIN));
                            sb.Append(string.Format(",@{0} ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_DISPERSION));
                            sb.Append(string.Format(",@{0} ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_UPPER));
                            sb.Append(string.Format(",@{0} ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_LOWER));
                            sb.Append(string.Format(",@{0} ", ColumnStandardValProductTypeProcessMst.ANCILLARY));
                            sb.Append(string.Format(",@{0} ", ColumnStandardValProductTypeProcessMst.SETUP));
                            sb.Append(string.Format(",@{0} ", ColumnStandardValProductTypeProcessMst.USE_FLAG));
                            sb.Append("); ");

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.COMPOSITION_ID}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnStandardValProductTypeProcessMst.COMPOSITION_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.PRODUCT_TYPE_ID}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnStandardValProductTypeProcessMst.PRODUCT_TYPE_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.PROCESS_IDX}", SqlDbType.Int)).Value = rowItem.Field<Int32>(ColumnStandardValProductTypeProcessMst.PROCESS_IDX);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MAX}", SqlDbType.Decimal)).Value = rowItem.Field<Decimal>(ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MAX);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.CYCLE_TIME_AVERAGE}", SqlDbType.Decimal)).Value = rowItem.Field<Decimal>(ColumnStandardValProductTypeProcessMst.CYCLE_TIME_AVERAGE);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MIN}", SqlDbType.Decimal)).Value = rowItem.Field<Decimal>(ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MIN);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.CYCLE_TIME_DISPERSION}", SqlDbType.Decimal)).Value = rowItem.Field<Decimal>(ColumnStandardValProductTypeProcessMst.CYCLE_TIME_DISPERSION);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.CYCLE_TIME_UPPER}", SqlDbType.Decimal)).Value = rowItem.Field<Decimal>(ColumnStandardValProductTypeProcessMst.CYCLE_TIME_UPPER);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.CYCLE_TIME_LOWER}", SqlDbType.Decimal)).Value = rowItem.Field<Decimal>(ColumnStandardValProductTypeProcessMst.CYCLE_TIME_LOWER);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.ANCILLARY}", SqlDbType.Decimal)).Value = rowItem.Field<Decimal>(ColumnStandardValProductTypeProcessMst.ANCILLARY);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.SETUP}", SqlDbType.Decimal)).Value = rowItem.Field<Decimal>(ColumnStandardValProductTypeProcessMst.SETUP);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.USE_FLAG}", SqlDbType.Bit)).Value = rowItem.Field<Boolean>(ColumnStandardValProductTypeProcessMst.USE_FLAG);

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

        public bool Update(DataTable updateDataTable)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Upsert処理
        /// </summary>
        /// <param name="updateDataTable"></param>
        /// <param name="compositionId"></param>
        /// <param name="productId"></param>
        /// <param name="processIdx"></param>
        /// <returns></returns>
        public bool Upsert(DataTable updateDataTable, Guid compositionId, Guid productId, int processIdx = -1)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Upsert:{ColumnStandardValProductTypeProcessMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    foreach (DataRow rowItem in updateDataTable.Rows)
                    {
                        if (rowItem.RowState == DataRowState.Modified)
                        {
                            sb.Clear();
                            sb.Append(String.Format("MERGE INTO {0} AS T1 USING ", ColumnStandardValProductTypeProcessMst.TABLE_NAME));
                            sb.Append(String.Format("(SELECT @{0} AS {0} , ", ColumnStandardValProductTypeProcessMst.COMPOSITION_ID));
                            sb.Append(String.Format("@{0} AS {0} , ", ColumnStandardValProductTypeProcessMst.PRODUCT_TYPE_ID));
                            sb.Append(String.Format("@{0} AS {0}) AS T2 ", ColumnStandardValProductTypeProcessMst.PROCESS_IDX));
                            sb.Append(String.Format("ON (T1.{0} = T2.{0} ", ColumnStandardValProductTypeProcessMst.COMPOSITION_ID));
                            sb.Append(String.Format("AND T1.{0} = T2.{0} ", ColumnStandardValProductTypeProcessMst.PROCESS_IDX));
                            sb.Append(String.Format("AND T1.{0} = T2.{0}) ", ColumnStandardValProductTypeProcessMst.PRODUCT_TYPE_ID));
                            sb.Append("WHEN MATCHED THEN UPDATE SET ");
                            sb.Append(String.Format("{0} = @{0}, ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MAX));
                            sb.Append(String.Format("{0} = @{0}, ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_AVERAGE));
                            sb.Append(String.Format("{0} = @{0}, ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MIN));
                            sb.Append(String.Format("{0} = @{0}, ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_DISPERSION));
                            sb.Append(String.Format("{0} = @{0}, ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_UPPER));
                            sb.Append(String.Format("{0} = @{0}, ", ColumnStandardValProductTypeProcessMst.CYCLE_TIME_LOWER));
                            sb.Append(String.Format("{0} = @{0}, ", ColumnStandardValProductTypeProcessMst.ANCILLARY));
                            sb.Append(String.Format("{0} = @{0}, ", ColumnStandardValProductTypeProcessMst.SETUP));
                            sb.Append(String.Format("{0} = @{0} ", ColumnStandardValProductTypeProcessMst.USE_FLAG));
                            sb.Append(String.Format("WHEN NOT MATCHED THEN "));
                            sb.Append(String.Format("INSERT ( "));
                            sb.Append(String.Format("{0} , {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}) ",
                                ColumnStandardValProductTypeProcessMst.COMPOSITION_ID,
                                ColumnStandardValProductTypeProcessMst.PRODUCT_TYPE_ID,
                                ColumnStandardValProductTypeProcessMst.PROCESS_IDX,
                                ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MAX,
                                ColumnStandardValProductTypeProcessMst.CYCLE_TIME_AVERAGE,
                                ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MIN,
                                ColumnStandardValProductTypeProcessMst.CYCLE_TIME_DISPERSION,
                                ColumnStandardValProductTypeProcessMst.CYCLE_TIME_UPPER,
                                ColumnStandardValProductTypeProcessMst.CYCLE_TIME_LOWER,
                                ColumnStandardValProductTypeProcessMst.ANCILLARY,
                                ColumnStandardValProductTypeProcessMst.SETUP,
                                ColumnStandardValProductTypeProcessMst.USE_FLAG));
                            sb.Append(String.Format("VALUES ( "));
                            sb.Append(String.Format("@{0} , @{1}, @{2}, @{3}, @{4}, @{5}, @{6}, @{7}, @{8}, @{9}, @{10}, @{11}); ",
                                ColumnStandardValProductTypeProcessMst.COMPOSITION_ID,
                                ColumnStandardValProductTypeProcessMst.PRODUCT_TYPE_ID,
                                ColumnStandardValProductTypeProcessMst.PROCESS_IDX,
                                ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MAX,
                                ColumnStandardValProductTypeProcessMst.CYCLE_TIME_AVERAGE,
                                ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MIN,
                                ColumnStandardValProductTypeProcessMst.CYCLE_TIME_DISPERSION,
                                ColumnStandardValProductTypeProcessMst.CYCLE_TIME_UPPER,
                                ColumnStandardValProductTypeProcessMst.CYCLE_TIME_LOWER,
                                ColumnStandardValProductTypeProcessMst.ANCILLARY,
                                ColumnStandardValProductTypeProcessMst.SETUP,
                                ColumnStandardValProductTypeProcessMst.USE_FLAG));

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.COMPOSITION_ID}", SqlDbType.UniqueIdentifier)).Value = compositionId;
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.PRODUCT_TYPE_ID}", SqlDbType.UniqueIdentifier)).Value = productId;
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MAX}", SqlDbType.Decimal)).Value = productId;
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.CYCLE_TIME_AVERAGE}", SqlDbType.Decimal)).Value = productId;
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MIN}", SqlDbType.Decimal)).Value = productId;
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.CYCLE_TIME_DISPERSION}", SqlDbType.Decimal)).Value = productId;
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.CYCLE_TIME_UPPER}", SqlDbType.Decimal)).Value = productId;
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.CYCLE_TIME_LOWER}", SqlDbType.Decimal)).Value = productId;
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.ANCILLARY}", SqlDbType.Decimal)).Value = productId;
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.SETUP}", SqlDbType.Decimal)).Value = productId;
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.USE_FLAG}", SqlDbType.Bit)).Value = productId;
                            //cmd.Parameters.Add(new SqlParameter($"@{ColumnDoDeviceMst.DO_ID}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnDoDeviceMst.DO_ID);
                            if (processIdx > -1)
                            {
                                cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeProcessMst.PROCESS_IDX}", SqlDbType.Int)).Value = processIdx;
                            }

                            String paramStr = "";
                            if (SqlDBConnector.CreateParamsText(cmd.Parameters, out paramStr))
                            {
                                sb.Append(paramStr);
                            }

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

