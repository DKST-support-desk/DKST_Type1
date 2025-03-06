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
    /// Reder_Device_Mstテーブルの列名の定義
    /// </summary>
    public class ColumnDataCollectionCameraMst
    {
        /// <summary>
        /// テーブル名
        /// </summary>
        public const String TABLE_NAME = "DataCollection_Camera_Mst";

        /// <summary>
        /// 編成ID
        /// </summary>
        public const String COMPOSITION_ID = "CompositionId";

        /// <summary>
        /// 品番タイプID
        /// </summary>
        public const String PRODUCT_TYPE_ID = "ProductTypeId";

        /// <summary>
        /// 工程インデックス
        /// </summary>
        public const String PROCESS_IDX = "ProcessIdx";

        /// <summary>
        /// カメラID1
        /// </summary>
        public const String CAMERA_ID_1 = "CameraID1";

        /// <summary>
        /// カメラ保存設定1
        /// </summary>
        public const String CAMERA_SAVE_MODE_1 = "CameraSaveMode1";

        /// <summary>
        /// カメラID1
        /// </summary>
        public const String CAMERA_ID_2 = "CameraID2";

        /// <summary>
        /// カメラ保存設定1
        /// </summary>
        public const String CAMERA_SAVE_MODE_2 = "CameraSaveMode2";

        /// <summary>
        /// カメラID1
        /// </summary>
        public const String CAMERA_ID_3 = "CameraID3";

        /// <summary>
        /// カメラ保存設定1
        /// </summary>
        public const String CAMERA_SAVE_MODE_3 = "CameraSaveMode3";

        /// <summary>
        /// カメラID4
        /// </summary>
        public const String CAMERA_ID_4 = "CameraID4";

        /// <summary>
        /// カメラ保存設定4
        /// </summary>
        public const String CAMERA_SAVE_MODE_4 = "CameraSaveMode4";

        /// <summary>
        /// カメラID5
        /// </summary>
        public const String CAMERA_ID_5 = "CameraID5";

        /// <summary>
        /// カメラ保存設定5
        /// </summary>
        public const String CAMERA_SAVE_MODE_5 = "CameraSaveMode5";

        /// <summary>
        /// カメラID6
        /// </summary>
        public const String CAMERA_ID_6 = "CameraID6";

        /// <summary>
        /// カメラ保存設定6
        /// </summary>
        public const String CAMERA_SAVE_MODE_6 = "CameraSaveMode6";

        /// <summary>
        /// カメラID7
        /// </summary>
        public const String CAMERA_ID_7 = "CameraID7";

        /// <summary>
        /// カメラ保存設定7
        /// </summary>
        public const String CAMERA_SAVE_MODE_7 = "CameraSaveMode7";
    }

    /// <summary>
    /// データ収集カメラマスタのデータ操作
    /// </summary>
    public class SqlDataCollectionCameraMst : ISqlBase
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

        /// <summary>
        /// DB接続オブジェクト
        /// </summary>
        #region <Property>
        public SqlDBConnector DbConnector { get; set; }
        #endregion

        #region <Constructor>
        /// <summary>
        /// データ収集カメラマスタのデータ操作
        /// </summary>
        public SqlDataCollectionCameraMst(String connectString)
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
        /// SELECT処理
        /// </summary>
        public DataTable Select()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 特定の編成、品番、工程のデータ収集カメラマスタを参照する
        /// </summary>
        /// <param name="compositionId"></param>
        /// <param name="productTypeId"></param>
        /// <param name="processIdx"></param>
        /// <returns></returns>
        public DataTable Select(Guid compositionId, Guid productTypeId, int processIdx = -1)
        {
            DataTable dt = new DataTable(ColumnDataCollectionCameraMst.TABLE_NAME);
            StringBuilder sb = new StringBuilder();
            try
            {
                logger.Debug($"Select:{ColumnDataCollectionCameraMst.TABLE_NAME}");
                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append(String.Format("SELECT * from {0} ", ColumnDataCollectionCameraMst.TABLE_NAME));
                    sb.Append(String.Format("WHERE {0} = @{0} ", ColumnDataCollectionCameraMst.COMPOSITION_ID));
                    sb.Append(String.Format("And {0} = @{0} ", ColumnDataCollectionCameraMst.PRODUCT_TYPE_ID));
                    if (processIdx > -1)
                    {
                        sb.Append(String.Format("And {0} = @{0} ", ColumnDataCollectionCameraMst.PROCESS_IDX));
                    }
                    sb.Append(String.Format("Order by {0}; ", ColumnDataCollectionCameraMst.PROCESS_IDX));

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter(String.Format("@{0}", ColumnDataCollectionCameraMst.COMPOSITION_ID), SqlDbType.UniqueIdentifier)).Value = compositionId;
                    cmd.Parameters.Add(new SqlParameter(String.Format("@{0}", ColumnDataCollectionCameraMst.PRODUCT_TYPE_ID), SqlDbType.UniqueIdentifier)).Value = productTypeId;
                    if (processIdx > -1)
                    {
                        cmd.Parameters.Add(new SqlParameter(String.Format("@{0}", ColumnDataCollectionCameraMst.PROCESS_IDX), SqlDbType.UniqueIdentifier)).Value = processIdx;
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

            int updateCameraDataCount = 0;

            try
            {
                logger.Debug($"Upsert:{ColumnDataCollectionCameraMst.TABLE_NAME}");

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
                            updateCameraDataCount = 0;

                            sb.Append(string.Format("MERGE INTO {0} AS T1 ", ColumnDataCollectionCameraMst.TABLE_NAME));
                            sb.Append("USING (SELECT ");
                            sb.Append(string.Format("@{0} AS {0} ", ColumnDataCollectionCameraMst.COMPOSITION_ID));
                            sb.Append(string.Format(",@{0} AS {0} ", ColumnDataCollectionCameraMst.PRODUCT_TYPE_ID));
                            sb.Append(string.Format(",@{0} AS {0}) AS T2 ", ColumnDataCollectionCameraMst.PROCESS_IDX));


                            sb.Append(string.Format("ON (T1.{0} = T2.{0} ", ColumnDataCollectionCameraMst.COMPOSITION_ID));
                            sb.Append(string.Format("AND T1.{0} = T2.{0} ", ColumnDataCollectionCameraMst.PRODUCT_TYPE_ID));
                            sb.Append(string.Format("AND T1.{0} = T2.{0}) ", ColumnDataCollectionCameraMst.PROCESS_IDX));

                            sb.Append("WHEN MATCHED THEN UPDATE SET ");

                            if (rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_1) > -1)
                            {
                                sb.Append(string.Format("{0} = @{0} ", ColumnDataCollectionCameraMst.CAMERA_ID_1));
                                sb.Append(string.Format(",{0} = @{0} ", ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_1));

                                updateCameraDataCount++;
                            }

                            if (rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_2) > -1)
                            {
                                if (updateCameraDataCount != 0)
                                {
                                    sb.Append(",");
                                }

                                sb.Append(string.Format("{0} = @{0} ", ColumnDataCollectionCameraMst.CAMERA_ID_2));
                                sb.Append(string.Format(",{0} = @{0} ", ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_2));

                                updateCameraDataCount++;
                            }

                            if (rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_3) > -1)
                            {
                                if (updateCameraDataCount != 0)
                                {
                                    sb.Append(",");
                                }

                                sb.Append(string.Format("{0} = @{0} ", ColumnDataCollectionCameraMst.CAMERA_ID_3));
                                sb.Append(string.Format(",{0} = @{0} ", ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_3));

                                updateCameraDataCount++;
                            }

                            if (rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_4) > -1)
                            {
                                if (updateCameraDataCount != 0)
                                {
                                    sb.Append(",");
                                }

                                sb.Append(string.Format("{0} = @{0} ", ColumnDataCollectionCameraMst.CAMERA_ID_4));
                                sb.Append(string.Format(",{0} = @{0} ", ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_4));

                                updateCameraDataCount++;
                            }

                            if (rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_5) > -1)
                            {
                                if (updateCameraDataCount != 0)
                                {
                                    sb.Append(",");
                                }

                                sb.Append(string.Format("{0} = @{0} ", ColumnDataCollectionCameraMst.CAMERA_ID_5));
                                sb.Append(string.Format(",{0} = @{0} ", ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_5));

                                updateCameraDataCount++;
                            }

                            if (rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_6) > -1)
                            {
                                if (updateCameraDataCount != 0)
                                {
                                    sb.Append(",");
                                }

                                sb.Append(string.Format("{0} = @{0} ", ColumnDataCollectionCameraMst.CAMERA_ID_6));
                                sb.Append(string.Format(",{0} = @{0} ", ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_6));

                                updateCameraDataCount++;
                            }

                            if (rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_7) > -1)
                            {
                                if (updateCameraDataCount != 0)
                                {
                                    sb.Append(",");
                                }

                                sb.Append(string.Format(",{0} = @{0} ", ColumnDataCollectionCameraMst.CAMERA_ID_7));
                                sb.Append(string.Format(",{0} = @{0} ", ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_7));
                            }

                            sb.Append(" WHEN NOT MATCHED THEN INSERT (");
                            sb.Append(string.Format("{0} ", ColumnDataCollectionCameraMst.COMPOSITION_ID));
                            sb.Append(string.Format(",{0} ", ColumnDataCollectionCameraMst.PRODUCT_TYPE_ID));
                            sb.Append(string.Format(",{0} ", ColumnDataCollectionCameraMst.PROCESS_IDX));

                            if (rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_1) > -1)
                            {
                                sb.Append(string.Format(",{0} ", ColumnDataCollectionCameraMst.CAMERA_ID_1));
                                sb.Append(string.Format(",{0} ", ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_1));
                            }

                            if (rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_2) > -1)
                            {
                                sb.Append(string.Format(",{0} ", ColumnDataCollectionCameraMst.CAMERA_ID_2));
                                sb.Append(string.Format(",{0} ", ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_2));
                            }

                            if (rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_3) > -1)
                            {
                                sb.Append(string.Format(",{0} ", ColumnDataCollectionCameraMst.CAMERA_ID_3));
                                sb.Append(string.Format(",{0} ", ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_3));
                            }

                            if (rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_4) > -1)
                            {
                                sb.Append(string.Format(",{0} ", ColumnDataCollectionCameraMst.CAMERA_ID_4));
                                sb.Append(string.Format(",{0} ", ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_4));
                            }

                            if (rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_5) > -1)
                            {
                                sb.Append(string.Format(",{0} ", ColumnDataCollectionCameraMst.CAMERA_ID_5));
                                sb.Append(string.Format(",{0} ", ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_5));
                            }

                            if (rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_6) > -1)
                            {
                                sb.Append(string.Format(",{0} ", ColumnDataCollectionCameraMst.CAMERA_ID_6));
                                sb.Append(string.Format(",{0} ", ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_6));
                            }

                            if (rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_7) > -1)
                            {
                                sb.Append(string.Format(",{0} ", ColumnDataCollectionCameraMst.CAMERA_ID_7));
                                sb.Append(string.Format(",{0} ", ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_7));
                            }

                            sb.Append(")");
                            sb.Append("VALUES ( ");
                            sb.Append(string.Format("@{0} ", ColumnDataCollectionCameraMst.COMPOSITION_ID));
                            sb.Append(string.Format(",@{0} ", ColumnDataCollectionCameraMst.PRODUCT_TYPE_ID));
                            sb.Append(string.Format(",@{0} ", ColumnDataCollectionCameraMst.PROCESS_IDX));

                            if (rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_1) > -1)
                            {
                                sb.Append(string.Format(",@{0} ", ColumnDataCollectionCameraMst.CAMERA_ID_1));
                                sb.Append(string.Format(",@{0} ", ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_1));
                            }

                            if (rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_2) > -1)
                            {
                                sb.Append(string.Format(",@{0} ", ColumnDataCollectionCameraMst.CAMERA_ID_2));
                                sb.Append(string.Format(",@{0} ", ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_2));
                            }

                            if (rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_3) > -1)
                            {
                                sb.Append(string.Format(",@{0} ", ColumnDataCollectionCameraMst.CAMERA_ID_3));
                                sb.Append(string.Format(",@{0} ", ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_3));
                            }

                            if (rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_4) > -1)
                            {
                                sb.Append(string.Format(",@{0} ", ColumnDataCollectionCameraMst.CAMERA_ID_4));
                                sb.Append(string.Format(",@{0} ", ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_4));
                            }

                            if (rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_5) > -1)
                            {
                                sb.Append(string.Format(",@{0} ", ColumnDataCollectionCameraMst.CAMERA_ID_5));
                                sb.Append(string.Format(",@{0} ", ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_5));
                            }

                            if (rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_6) > -1)
                            {
                                sb.Append(string.Format(",@{0} ", ColumnDataCollectionCameraMst.CAMERA_ID_6));
                                sb.Append(string.Format(",@{0} ", ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_6));
                            }

                            if (rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_7) > -1)
                            {
                                sb.Append(string.Format(",@{0} ", ColumnDataCollectionCameraMst.CAMERA_ID_7));
                                sb.Append(string.Format(",@{0} ", ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_7));
                            }
                            sb.Append("); ");

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionCameraMst.COMPOSITION_ID}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnDataCollectionCameraMst.COMPOSITION_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionCameraMst.PRODUCT_TYPE_ID}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnDataCollectionCameraMst.PRODUCT_TYPE_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionCameraMst.PROCESS_IDX}", SqlDbType.Int)).Value = rowItem.Field<int>(ColumnDataCollectionCameraMst.PROCESS_IDX);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionCameraMst.CAMERA_ID_1}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnDataCollectionCameraMst.CAMERA_ID_1);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_1}", SqlDbType.Int)).Value = rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_1);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionCameraMst.CAMERA_ID_2}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnDataCollectionCameraMst.CAMERA_ID_2);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_2}", SqlDbType.Int)).Value = rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_2);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionCameraMst.CAMERA_ID_3}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnDataCollectionCameraMst.CAMERA_ID_3);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_3}", SqlDbType.Int)).Value = rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_3);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionCameraMst.CAMERA_ID_4}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnDataCollectionCameraMst.CAMERA_ID_4);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_4}", SqlDbType.Int)).Value = rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_4);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionCameraMst.CAMERA_ID_5}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnDataCollectionCameraMst.CAMERA_ID_5);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_5}", SqlDbType.Int)).Value = rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_5);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionCameraMst.CAMERA_ID_6}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnDataCollectionCameraMst.CAMERA_ID_6);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_6}", SqlDbType.Int)).Value = rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_6);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionCameraMst.CAMERA_ID_7}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnDataCollectionCameraMst.CAMERA_ID_7);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_7}", SqlDbType.Int)).Value = rowItem.Field<int>(ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_7);

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

