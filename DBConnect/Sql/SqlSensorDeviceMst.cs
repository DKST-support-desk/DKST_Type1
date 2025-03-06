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
    /// <summary>
    /// Sensor_Device_Mstテーブルの列名の定義
    /// </summary>
    public class ColumnSensorDeviceMst
    {
        /// <summary>
        /// テーブル名
        /// </summary>
        public const string TABLE_NAME = "Sensor_Device_Mst";

        /// <summary>
        /// センサID
        /// </summary>
        public const string SENSOR_ID = "SensorId";

        /// <summary>
        /// センサ名
        /// </summary>
        public const string SENSOR_NAME = "SensorName";

        /// <summary>
        /// DI接点番号
        /// </summary>
        public const string INPUT_NO = "InputNo";

        /// <summary>
        /// 機器説明
        /// </summary>
        public const string DEVICE_REMARK = "DeviceRemark";

        /// <summary>
        /// 作成日時
        /// </summary>
        public const string CREATE_DATETIME = "CreateDateTime";
    }

    /// <summary>
    /// センサ機器マスタのデータ操作
    /// </summary>
    public class SqlSensorDeviceMst : ISqlBase
    {
        #region <Field>
        /// <summary>
        /// 接続文字列
        /// </summary>
        private string mConnectString;

        /// <summary>
        /// データ収集トリガーマスタに同じ品番タイプIDが存在するか
        /// </summary>
        public const string EXIST_IN_DATA_COLLECTION_TRIGGER = "ExistInDataCollectionTrigger";

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
        /// センサ機器マスタのデータ操作
        /// </summary>
        public SqlSensorDeviceMst(string connectString)
        {
            mConnectString = connectString;
        }
        #endregion

        #region <Method>
        /// <summary>
        /// DB接続
        /// </summary>
        /// <returns></returns>
        public bool OpenConnection()
        {
            //logger.Debug($"DbConnection:{mConnectString}");

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
            DataTable dt = new DataTable(ColumnSensorDeviceMst.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Select:{ColumnSensorDeviceMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT *, ");
                    sb.Append(String.Format("(SELECT COUNT(*) From {0} DCTM WHERE ( DCTM.{1} = SDM.{2} OR DCTM.{3} = SDM.{2})) {4} ",
                        ColumnDataCollectionTriggerMst.TABLE_NAME,
                        ColumnDataCollectionTriggerMst.SENSOR_ID_1,
                        ColumnSensorDeviceMst.SENSOR_ID,
                        ColumnDataCollectionTriggerMst.SENSOR_ID_2,
                        EXIST_IN_DATA_COLLECTION_TRIGGER)
                        );
                    sb.Append(String.Format("FROM {0} SDM ", ColumnSensorDeviceMst.TABLE_NAME));
                    sb.Append(String.Format("Order by {0};", ColumnSensorDeviceMst.CREATE_DATETIME));

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
        /// Insert処理の実行
        /// </summary>
        /// <param name="insertDataTable"></param>
        /// <returns></returns>
        public bool Insert(DataTable insertDataTable)
        {
            // Upsert()を使用
            return false;
        }

        /// <summary>
        /// UPDATE処理の実行
        /// </summary>
        /// <param name="updateDataTable">更新データ</param>
        /// <returns>処理結果 true=成功 false=失敗</returns>
        public bool Update(DataTable updateDataTable)
        {
            // Upsert()を使用
            return false;
        }

        /// <summary>
        /// Upsert処理の実行
        /// </summary>
        /// <param name="updateDataTable"></param>
        /// <returns>処理結果 true=成功 false=失敗</returns>
        public bool Upsert(DataTable updateDataTable)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Upsert:{ColumnSensorDeviceMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    foreach (DataRow rowItem in updateDataTable.Rows)
                    {
                        if (rowItem.RowState == DataRowState.Modified || rowItem.RowState == DataRowState.Added)
                        {
                            sb.Clear();
                            sb.Append(string.Format("MERGE INTO {0} AS T1 ", ColumnSensorDeviceMst.TABLE_NAME));
                            sb.Append(string.Format("USING (SELECT @{0} AS {0}) AS T2 ", ColumnSensorDeviceMst.SENSOR_ID));
                            sb.Append(string.Format("ON (T1.{0} = T2.{0}) ", ColumnSensorDeviceMst.SENSOR_ID));
                            sb.Append("WHEN MATCHED THEN UPDATE SET ");
                            sb.Append(string.Format("{0} = @{0}, ", ColumnSensorDeviceMst.SENSOR_ID));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnSensorDeviceMst.SENSOR_NAME));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnSensorDeviceMst.INPUT_NO));
                            sb.Append(string.Format("{0} = @{0}  ", ColumnSensorDeviceMst.DEVICE_REMARK));
                            sb.Append("WHEN NOT MATCHED THEN INSERT (");
                            sb.Append(string.Format("{0}, ", ColumnSensorDeviceMst.SENSOR_ID));
                            sb.Append(string.Format("{0}, ", ColumnSensorDeviceMst.SENSOR_NAME));
                            sb.Append(string.Format("{0}, ", ColumnSensorDeviceMst.INPUT_NO));
                            sb.Append(string.Format("{0}, ", ColumnSensorDeviceMst.DEVICE_REMARK));
                            sb.Append(string.Format("{0}) ", ColumnSensorDeviceMst.CREATE_DATETIME));
                            sb.Append("VALUES (");
                            sb.Append(string.Format("@{0}, ", ColumnSensorDeviceMst.SENSOR_ID));
                            sb.Append(string.Format("@{0}, ", ColumnSensorDeviceMst.SENSOR_NAME));
                            sb.Append(string.Format("@{0}, ", ColumnSensorDeviceMst.INPUT_NO));
                            sb.Append(string.Format("@{0}, ", ColumnSensorDeviceMst.DEVICE_REMARK));
                            sb.Append(string.Format("@{0});", ColumnSensorDeviceMst.CREATE_DATETIME));

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnSensorDeviceMst.SENSOR_ID}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnSensorDeviceMst.SENSOR_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnSensorDeviceMst.SENSOR_NAME}", SqlDbType.NVarChar)).Value = rowItem.Field<string>(ColumnSensorDeviceMst.SENSOR_NAME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnSensorDeviceMst.INPUT_NO}", SqlDbType.Int)).Value = rowItem.Field<int>(ColumnSensorDeviceMst.INPUT_NO);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnSensorDeviceMst.DEVICE_REMARK}", SqlDbType.NVarChar)).Value = rowItem.Field<string>(ColumnSensorDeviceMst.DEVICE_REMARK) ?? string.Empty;
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnSensorDeviceMst.CREATE_DATETIME}", SqlDbType.DateTime)).Value = rowItem.Field<DateTime>(ColumnSensorDeviceMst.CREATE_DATETIME);

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

        /// <summary>
        /// Delete処理の実行
        /// </summary>
        /// <param name="deleteDataTable"></param>
        /// <returns>処理結果 true=成功 false=失敗</returns>
        public bool Delete(DataTable deleteDataTable)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Delete:{ColumnSensorDeviceMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }
                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    foreach (DataRow deleteItem in deleteDataTable.Rows)
                    {
                        sb.Clear();
                        sb.Append(string.Format("DELETE FROM {0} ", ColumnSensorDeviceMst.TABLE_NAME));
                        sb.Append(string.Format("WHERE {0} = @{0};", ColumnSensorDeviceMst.SENSOR_ID));

                        cmd.CommandText = sb.ToString();
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter(string.Format("@{0}", ColumnSensorDeviceMst.SENSOR_ID), SqlDbType.UniqueIdentifier)).Value = deleteItem.Field<Guid>(ColumnSensorDeviceMst.SENSOR_ID);

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
        /// DELETE処理の実行(List)
        /// </summary>
        /// <param name="deleteGuid">削除データ</param>
        /// <returns>処理結果 true=成功 false=失敗</returns>
        public bool Delete(List<Guid> deleteGuid)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Delete:{ColumnSensorDeviceMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }
                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    foreach (Guid deleteItem in deleteGuid)
                    {
                        sb.Clear();
                        sb.Append(string.Format("DELETE FROM {0} ", ColumnSensorDeviceMst.TABLE_NAME));
                        sb.Append(string.Format("WHERE {0} = @{0};", ColumnSensorDeviceMst.SENSOR_ID));

                        cmd.CommandText = sb.ToString();
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter(string.Format("@{0}", ColumnSensorDeviceMst.SENSOR_ID), SqlDbType.UniqueIdentifier)).Value = deleteItem;

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
        #endregion
    }
}
