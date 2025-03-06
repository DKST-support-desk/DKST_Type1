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
    public class ColumnWorkingStatus
    {
        public const String TABLE_NAME = "Working_Status";
        public const String PROCESS_IDX = "ProcessIdx";
        public const String COMPOSITION_ID = "CompositionId";
        public const String PRODUCT_TYPE_ID = "ProductTypeId";
        public const String WORKER_NAME = "WorkerName";
    }
    public class SqlWorkingStatus : ISqlBase
    {
        #region <Field>
        private String mConnectString;
        public const String NON_REGISTRY_NAME = "未登録";
        public const String INPUT_SENSOR1 = "InputSensor1";
        public const String INPUT_SENSOR2 = "InputSensor2";

        /// <summary>
        /// ログインスタンス
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region <Property>
        public SqlDBConnector DbConnector { get; set; }

        #endregion

        #region <Constructor>
        public SqlWorkingStatus(String connectString)
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
                logger.Error("DB接続に失敗しました。");
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
            DataTable dt = new DataTable(ColumnWorkingStatus.TABLE_NAME);
            StringBuilder sb = new StringBuilder();
            try
            {
                logger.Debug($"Select:{ColumnWorkingStatus.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Clear();
                    sb.Append("SELECT * ");
                    sb.Append(String.Format("FROM {0} ", ColumnWorkingStatus.TABLE_NAME));
                    sb.Append(String.Format("Order by {0};", ColumnWorkingStatus.PROCESS_IDX));

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
        /// 作業登録情報から、編成・品番タイプ・工程と、それに紐づいた使用センサと付随する情報(接点番号、連続信号排除フィルタなど)を取得する
        /// </summary>
        /// <returns></returns>
        public DataTable SelectProcessInfo()
        {
            DataTable dt = new DataTable(ColumnWorkingStatus.TABLE_NAME);
            StringBuilder sb = new StringBuilder();
            try
            {
                logger.Debug($"SelectProcessInfo:{ColumnWorkingStatus.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Clear();
                    sb.Append("SELECT ");
                    sb.Append(String.Format("ws.{0}, ", ColumnWorkingStatus.PROCESS_IDX));
                    sb.Append(String.Format("ws.{0}, ", ColumnWorkingStatus.COMPOSITION_ID));
                    sb.Append(String.Format("ws.{0}, ", ColumnWorkingStatus.PRODUCT_TYPE_ID));
                    sb.Append(String.Format("sppm.{0}, ", ColumnStandardValProductTypeProcessMst.USE_FLAG));
                    sb.Append(String.Format("dctm.{0}, ", ColumnDataCollectionTriggerMst.TRIGGER_TYPE));
                    sb.Append(String.Format("dctm.{0}, ", ColumnDataCollectionTriggerMst.INTERVAL_FILTER));
                    sb.Append(String.Format("dctm.{0}, ", ColumnDataCollectionTriggerMst.SENSOR_ID_1));
                    sb.Append(String.Format("(SELECT {0} FROM {1} sdm WHERE sdm.{2} = dctm.{3} ) AS {4}, ",
                        ColumnSensorDeviceMst.INPUT_NO,
                        ColumnSensorDeviceMst.TABLE_NAME,
                        ColumnSensorDeviceMst.SENSOR_ID,
                        ColumnDataCollectionTriggerMst.SENSOR_ID_1,
                        INPUT_SENSOR1));
                    sb.Append(String.Format("dctm.{0}, ", ColumnDataCollectionTriggerMst.EXCLUSION_FILTER_1));
                    sb.Append(String.Format("dctm.{0}, ", ColumnDataCollectionTriggerMst.SENSOR_ID_2));
                    sb.Append(String.Format("(SELECT {0} FROM {1} sdm WHERE sdm.{2} = dctm.{3} ) AS {4}, ",
                        ColumnSensorDeviceMst.INPUT_NO,
                        ColumnSensorDeviceMst.TABLE_NAME,
                        ColumnSensorDeviceMst.SENSOR_ID,
                        ColumnDataCollectionTriggerMst.SENSOR_ID_2,
                        INPUT_SENSOR2));
                    sb.Append(String.Format("dctm.{0} ", ColumnDataCollectionTriggerMst.EXCLUSION_FILTER_2));
                    sb.Append(String.Format("FROM {0} ws ", ColumnWorkingStatus.TABLE_NAME));

                    sb.Append(String.Format("LEFT JOIN {0} sppm ", ColumnStandardValProductTypeProcessMst.TABLE_NAME));
                    sb.Append(String.Format("ON sppm.{0} = ws.{0} ", ColumnWorkingStatus.PROCESS_IDX));
                    sb.Append(String.Format("AND sppm.{0} = ws.{0} ", ColumnWorkingStatus.COMPOSITION_ID));
                    sb.Append(String.Format("AND sppm.{0} = ws.{0} ", ColumnWorkingStatus.PRODUCT_TYPE_ID));

                    sb.Append(String.Format("LEFT JOIN {0} dctm ", ColumnDataCollectionTriggerMst.TABLE_NAME));
                    sb.Append(String.Format("ON dctm.{0} = ws.{0} ", ColumnWorkingStatus.PROCESS_IDX));
                    sb.Append(String.Format("AND dctm.{0} = ws.{0} ", ColumnWorkingStatus.COMPOSITION_ID));
                    sb.Append(String.Format("AND dctm.{0} = ws.{0} ", ColumnWorkingStatus.PRODUCT_TYPE_ID));

                    //sb.Append(String.Format("WHERE ws.{0} = @{0} ", ColumnWorkingStatus.PROCESS_IDX));
                    //sb.Append(String.Format("AND ws.{0} = @{0} ", ColumnWorkingStatus.COMPOSITION_ID));
                    //sb.Append(String.Format("AND ws.{0} = @{0} ", ColumnWorkingStatus.PRODUCT_TYPE_ID));
                    sb.Append(String.Format("Order by {0} ", ColumnWorkingStatus.PROCESS_IDX));
                    sb.Append(";");

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
        /// INSERT処理の実行
        /// </summary>
        /// <param name="insertDataTable"></param>
        /// <returns></returns>
        public bool Insert(DataTable insertDataTable)
        {
            Boolean ret = false;
            StringBuilder sb = new StringBuilder();
            try
            {
                logger.Debug($"Insert:{ColumnWorkingStatus.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    foreach (DataRow rowItem in insertDataTable.Rows)
                    {

                        sb.Clear();
                        sb.Append(String.Format("INSERT INTO {0} ( ", ColumnWorkingStatus.TABLE_NAME));
                        sb.Append(String.Format("{0}, {1}, ", ColumnWorkingStatus.PROCESS_IDX, ColumnWorkingStatus.COMPOSITION_ID));
                        if (!(rowItem[ColumnWorkingStatus.PRODUCT_TYPE_ID] is System.DBNull))
                        {
                            sb.Append(String.Format("{0},  ", ColumnWorkingStatus.PRODUCT_TYPE_ID));
                        }
                        sb.Append(String.Format("{0})  ", ColumnWorkingStatus.WORKER_NAME));

                        sb.Append(String.Format("Values (@{0}, @{1}, ", ColumnWorkingStatus.PROCESS_IDX, ColumnWorkingStatus.COMPOSITION_ID));
                        if (!(rowItem[ColumnWorkingStatus.PRODUCT_TYPE_ID] is System.DBNull))
                        {
                            sb.Append(String.Format("@{0},  ", ColumnWorkingStatus.PRODUCT_TYPE_ID));
                        }
                        sb.Append(String.Format("@{0});  ", ColumnWorkingStatus.WORKER_NAME));

                        cmd.CommandText = sb.ToString();

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter(String.Format("@{0}", ColumnWorkingStatus.PROCESS_IDX), SqlDbType.Int)).Value = rowItem.Field<int>(ColumnWorkingStatus.PROCESS_IDX);
                        cmd.Parameters.Add(new SqlParameter(String.Format("@{0}", ColumnWorkingStatus.COMPOSITION_ID), SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnWorkingStatus.COMPOSITION_ID);
                        if (!(rowItem[ColumnWorkingStatus.PRODUCT_TYPE_ID] is System.DBNull))
                        {
                            cmd.Parameters.Add(new SqlParameter(String.Format("@{0}", ColumnWorkingStatus.PRODUCT_TYPE_ID), SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnWorkingStatus.PRODUCT_TYPE_ID);
                        }
                        cmd.Parameters.Add(new SqlParameter(String.Format("@{0}", ColumnWorkingStatus.WORKER_NAME), SqlDbType.NVarChar)).Value = rowItem.Field<String>(ColumnWorkingStatus.WORKER_NAME);

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
        /// UPDATE処理の実行
        /// </summary>
        /// <param name="updateDataTable"></param>
        /// <returns></returns>
        public bool Update(DataTable updateDataTable)
        {
            Boolean ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Update:{ColumnWorkingStatus.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    foreach (DataRow rowItem in updateDataTable.Rows)
                    {
                        sb.Clear();
                        sb.Append(String.Format("UPDATE {0} SET ", ColumnWorkingStatus.TABLE_NAME));
                        sb.Append(String.Format("{0} = @{0}, ", ColumnWorkingStatus.COMPOSITION_ID));
                        sb.Append(String.Format("{0} = @{0}, ", ColumnWorkingStatus.PRODUCT_TYPE_ID));
                        sb.Append(String.Format("{0} = @{0} ", ColumnWorkingStatus.WORKER_NAME));
                        sb.Append(String.Format("Where {0} = @{0}; ", ColumnWorkingStatus.PROCESS_IDX));


                        cmd.CommandText = sb.ToString();

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter(String.Format("@{0}", ColumnWorkingStatus.PROCESS_IDX), SqlDbType.Int)).Value = rowItem.Field<int>(ColumnWorkingStatus.PROCESS_IDX);
                        cmd.Parameters.Add(new SqlParameter(String.Format("@{0}", ColumnWorkingStatus.COMPOSITION_ID), SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnWorkingStatus.COMPOSITION_ID);
                        cmd.Parameters.Add(new SqlParameter(String.Format("@{0}", ColumnWorkingStatus.PRODUCT_TYPE_ID), SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnWorkingStatus.PRODUCT_TYPE_ID);
                        cmd.Parameters.Add(new SqlParameter(String.Format("@{0}", ColumnWorkingStatus.WORKER_NAME), SqlDbType.NVarChar)).Value = rowItem.Field<String>(ColumnWorkingStatus.WORKER_NAME);

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
        /// 直変更時、登録されている作業者名を全て[未登録]で上書きする
        /// </summary>
        /// <returns></returns>
        public bool UpdateWokerNameReset()
        {
            Boolean ret = false;

            if (OpenConnection() == false)
            {
                logger.Error("DBに接続されていません。");
                return false;
            }

            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"UpdateWokerNameReset:{ColumnWorkingStatus.TABLE_NAME}");

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Clear();
                    sb.Append(String.Format("UPDATE {0} SET ", ColumnWorkingStatus.TABLE_NAME));
                    sb.Append(String.Format("{0} = @{0};", ColumnWorkingStatus.WORKER_NAME));

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter(String.Format("@{0}", ColumnWorkingStatus.WORKER_NAME), SqlDbType.NVarChar)).Value = NON_REGISTRY_NAME;

                    String paramStr = "";
                    if (SqlDBConnector.CreateParamsText(cmd.Parameters, out paramStr))
                    {
                        sb.Append(paramStr);
                    }

                    cmd.ExecuteNonQuery();
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

        /// <summary>
        /// DELETEの実行(総入れ替えのため条件なし全削除)
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            Boolean ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Delete:{ColumnWorkingStatus.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Clear();
                    sb.Append(String.Format("DELETE FROM {0};", ColumnWorkingStatus.TABLE_NAME));

                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
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
