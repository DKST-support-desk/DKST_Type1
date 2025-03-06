using DBConnect;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
// using Common;

namespace DBConnect.SQL
{
    /// <summary>
    /// Camera_Device_Mstテーブルの列名の定義
    /// </summary>
    public static class ColumnCameraDeviceMst
    {
        /// <summary>
        /// テーブル名
        /// </summary>
        public const string TABLE_NAME = "Camera_Device_Mst";

        /// <summary>
        /// カメラID
        /// </summary>
        public const string CAMERA_ID = "CameraId";

        /// <summary>
        /// カメラ名称
        /// </summary>
        public const string CAMERA_NAME = "CameraName";

        /// <summary>
        /// カメラ認識コード
        /// </summary>
        public const string CAMERA_DEVICE_CODE = "CameraDeviceCode";

        /// <summary>
        /// FPS設定 30 or 60
        /// </summary>
        public const string DEVICE_NUMNBER = "DeviceNumber";

        /// <summary>
        /// マイク認識コード
        /// </summary>
        public const string MIC_DEVICE_CODE = "MicDeviceCode";

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
    /// カメラ機器マスタのデータ操作
    /// </summary>
    public class SqlCameraDeviceMst : ISqlBase
    {
        #region <Field>
        /// <summary>
        /// 接続文字列
        /// </summary>
        private string mConnectString;

        /// <summary>
        /// データ収集カメラマスタに同じ品番タイプIDが存在するか
        /// </summary>
        public const string EXIST_IN_DATA_COLLECTION_CAMERA = "ExistInDataCollectionCamera";

        /// <summary>
        /// ログインスタンス
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion <Field>

        #region <Property>
        /// <summary>
        /// DB接続オブジェクト
        /// </summary>
        public SqlDBConnector DbConnector { get; set; }
        #endregion <Property>

        #region <Constructor>
        /// <summary>
        /// カメラ機器マスタのデータ操作
        /// </summary>
        public SqlCameraDeviceMst(string connectString)
        {
            mConnectString = connectString;
        }
        #endregion <Constructor>

        #region <Method>
        /// <summary>
        /// DB接続
        /// </summary>
        /// <returns>接続結果 true=成功 false=失敗</returns>
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
            DataTable dt = new DataTable(ColumnCameraDeviceMst.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Select:{ColumnCameraDeviceMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Info("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT *, ");
                    sb.Append(String.Format("(SELECT COUNT(*) From {0} DCCM WHERE ( " +
                                            "DCCM.{1} = CDM.{8} OR " +
                                            "DCCM.{2} = CDM.{8} OR " +
                                            "DCCM.{3} = CDM.{8} OR " +
                                            "DCCM.{4} = CDM.{8} OR " +
                                            "DCCM.{5} = CDM.{8} OR " +
                                            "DCCM.{6} = CDM.{8} OR " +
                                            "DCCM.{7} = CDM.{8})) {9} ",
                        ColumnDataCollectionCameraMst.TABLE_NAME,
                        ColumnDataCollectionCameraMst.CAMERA_ID_1,
                        ColumnDataCollectionCameraMst.CAMERA_ID_2,
                        ColumnDataCollectionCameraMst.CAMERA_ID_3,
                        ColumnDataCollectionCameraMst.CAMERA_ID_4,
                        ColumnDataCollectionCameraMst.CAMERA_ID_5,
                        ColumnDataCollectionCameraMst.CAMERA_ID_6,
                        ColumnDataCollectionCameraMst.CAMERA_ID_7,
                        ColumnCameraDeviceMst.CAMERA_ID,
                        EXIST_IN_DATA_COLLECTION_CAMERA));
                    sb.Append(String.Format("FROM {0} CDM ", ColumnCameraDeviceMst.TABLE_NAME));
                    sb.Append(String.Format("Order by {0};", ColumnCameraDeviceMst.CREATE_DATETIME));

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
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public bool Insert(DataTable dataTable)
        {
            // Upsert()を使用
            return false;
        }

        /// <summary>
        /// UPDATE処理の実行
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public bool Update(DataTable dataTable)
        {
            // Upsert()を使用
            return false;
        }

        /// <summary>
        /// UPSERT処理の実行
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public bool Upsert(DataTable dataTable)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Upsert:{ColumnCameraDeviceMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    foreach (DataRow udpateItem in dataTable.Rows)
                    {
                        if (udpateItem.RowState == DataRowState.Modified || udpateItem.RowState == DataRowState.Added)
                        {
                            sb.Clear();
                            sb.Append($"MERGE INTO {ColumnCameraDeviceMst.TABLE_NAME} AS T1 ");
                            sb.Append(string.Format("USING (SELECT @{0} AS {0}) AS T2 ", ColumnCameraDeviceMst.CAMERA_ID));
                            sb.Append(string.Format("ON (T1.{0} = T2.{0})", ColumnCameraDeviceMst.CAMERA_ID));
                            sb.Append("WHEN MATCHED THEN UPDATE SET ");
                            sb.Append(string.Format("{0} = @{0}, ", ColumnCameraDeviceMst.CAMERA_ID));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnCameraDeviceMst.CAMERA_NAME));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnCameraDeviceMst.CAMERA_DEVICE_CODE));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnCameraDeviceMst.MIC_DEVICE_CODE));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnCameraDeviceMst.DEVICE_NUMNBER));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnCameraDeviceMst.DEVICE_REMARK));
                            sb.Append(string.Format("{0} = @{0} ", ColumnCameraDeviceMst.CREATE_DATETIME));
                            sb.Append("WHEN NOT MATCHED THEN INSERT (");
                            sb.Append($"{ColumnCameraDeviceMst.CAMERA_ID}, ");
                            sb.Append($"{ColumnCameraDeviceMst.CAMERA_NAME}, ");
                            sb.Append($"{ColumnCameraDeviceMst.CAMERA_DEVICE_CODE}, ");
                            sb.Append($"{ColumnCameraDeviceMst.MIC_DEVICE_CODE}, ");
                            sb.Append($"{ColumnCameraDeviceMst.DEVICE_NUMNBER}, ");
                            sb.Append($"{ColumnCameraDeviceMst.DEVICE_REMARK}, ");
                            sb.Append($"{ColumnCameraDeviceMst.CREATE_DATETIME} ");
                            sb.Append(") VALUES (");
                            sb.Append($"@{ColumnCameraDeviceMst.CAMERA_ID}, ");
                            sb.Append($"@{ColumnCameraDeviceMst.CAMERA_NAME}, ");
                            sb.Append($"@{ColumnCameraDeviceMst.CAMERA_DEVICE_CODE}, ");
                            sb.Append($"@{ColumnCameraDeviceMst.MIC_DEVICE_CODE}, ");
                            sb.Append($"@{ColumnCameraDeviceMst.DEVICE_NUMNBER}, ");
                            sb.Append($"@{ColumnCameraDeviceMst.DEVICE_REMARK}, ");
                            sb.Append($"@{ColumnCameraDeviceMst.CREATE_DATETIME} ");
                            sb.Append(");");

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCameraDeviceMst.CAMERA_ID}", SqlDbType.UniqueIdentifier)).Value = udpateItem.Field<Guid>(ColumnCameraDeviceMst.CAMERA_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCameraDeviceMst.CAMERA_NAME}", SqlDbType.NVarChar)).Value = udpateItem.Field<string>(ColumnCameraDeviceMst.CAMERA_NAME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCameraDeviceMst.CAMERA_DEVICE_CODE}", SqlDbType.NVarChar)).Value = udpateItem.Field<string>(ColumnCameraDeviceMst.CAMERA_DEVICE_CODE);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCameraDeviceMst.MIC_DEVICE_CODE}", SqlDbType.NVarChar)).Value = udpateItem.Field<string>(ColumnCameraDeviceMst.MIC_DEVICE_CODE) ?? string.Empty;
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCameraDeviceMst.DEVICE_NUMNBER}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnCameraDeviceMst.DEVICE_NUMNBER);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCameraDeviceMst.DEVICE_REMARK}", SqlDbType.NVarChar)).Value = udpateItem.Field<string>(ColumnCameraDeviceMst.DEVICE_REMARK) ?? string.Empty;
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCameraDeviceMst.CREATE_DATETIME}", SqlDbType.DateTime)).Value = udpateItem.Field<DateTime>(ColumnCameraDeviceMst.CREATE_DATETIME);

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

        /// <summary>
        /// DELETE処理の実行(DataTable)
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public bool Delete(DataTable dataTable)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Delete:{ColumnCameraDeviceMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Info("DBに接続されていません。");
                    return false;
                }
                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    foreach (DataRow deleteItem in dataTable.Rows)
                    {
                        sb.Clear();
                        sb.Append($"DELETE FROM {ColumnCameraDeviceMst.TABLE_NAME} ");
                        sb.Append(string.Format("WHERE {0} = @{0};", ColumnCameraDeviceMst.CAMERA_ID));

                        cmd.CommandText = sb.ToString();
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter($"@{ColumnCameraDeviceMst.CAMERA_ID}", SqlDbType.UniqueIdentifier)).Value = deleteItem.Field<Guid>(ColumnCameraDeviceMst.CAMERA_ID);

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
                logger.Debug($"Delete:{ColumnCameraDeviceMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Info("DBに接続されていません。");
                    return false;
                }
                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    foreach (Guid deleteItem in deleteGuid)
                    {
                        sb.Clear();
                        sb.Append(string.Format("DELETE FROM {0} ", ColumnCameraDeviceMst.TABLE_NAME));
                        sb.Append(string.Format("WHERE {0} = @{0};", ColumnCameraDeviceMst.CAMERA_ID));

                        cmd.CommandText = sb.ToString();
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter(string.Format("@{0}", ColumnCameraDeviceMst.CAMERA_ID), SqlDbType.UniqueIdentifier)).Value = deleteItem;

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
        #endregion <Method>
    }
}
