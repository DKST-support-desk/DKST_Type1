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
    /// DO_Device_Mstテーブルの列名の定義
    /// </summary>
    public class ColumnDoDeviceMst
    {
        /// <summary>
        /// テーブル名
        /// </summary>
        public const string TABLE_NAME = "DO_Device_Mst";

        /// <summary>
        /// DOID
        /// </summary>
        public const string DO_ID = "DOId";

        /// <summary>
        /// DO名
        /// </summary>
        public const string DO_NAME = "DOName";

        /// <summary>
        /// 長音接点番号
        /// </summary>
        public const string LONG_OUTPUT_NO = "LongOutputNo";

        /// <summary>
        /// 短音接点番号
        /// </summary>
        public const string SHORT_OUTPUT_NO = "ShortOutputNo";

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
    /// DO機器マスタのデータ操作
    /// </summary>
    public class SqlDoDeviceMst : ISqlBase
    {
        #region <Field>
        /// <summary>
        /// 接続文字列
        /// </summary>
        private string mConnectString;

        /// <summary>
        /// データ収集DO(ブザー)マスタに同じ品番タイプIDが存在するか
        /// </summary>
        public const string EXIST_IN_DATA_COLLECTION_DO = "ExistInDataCollectionDo";

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
        /// DO機器マスタのデータ操作
        /// </summary>
        public SqlDoDeviceMst(string connectString)
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
        /// Select処理の実行
        /// </summary>
        /// <returns></returns>
        public DataTable Select()
        {
            DataTable dt = new DataTable(ColumnDoDeviceMst.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Select:{ColumnDoDeviceMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT *, ");
                    sb.Append(String.Format("(SELECT COUNT(*) From {0} DCDM WHERE ( DCDM.{1} = DDM.{2})) {3} ",
                        ColumnDataCollectionDoMst.TABLE_NAME,
                        ColumnDataCollectionDoMst.DO_ID,
                        ColumnDoDeviceMst.DO_ID,
                        EXIST_IN_DATA_COLLECTION_DO)
                        );
                    sb.Append(String.Format("FROM {0} DDM ", ColumnDoDeviceMst.TABLE_NAME));
                    sb.Append(String.Format("Order by {0};", ColumnDoDeviceMst.CREATE_DATETIME));

                    cmd.CommandText = sb.ToString();

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


        public DataTable SelectWithDataCollectionDoMaster(Guid compositionId, Guid productId, int processIndex)
        {
            DataTable dt = new DataTable(ColumnDoDeviceMst.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"SelectWithDataCollectionDoMaster:{ColumnDoDeviceMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT DDM.* ");
                    sb.Append(String.Format("FROM {0} DDM ", ColumnDoDeviceMst.TABLE_NAME));
                    sb.Append(String.Format("LEFT JOIN {0} DCDM ", ColumnDataCollectionDoMst.TABLE_NAME));
                    sb.Append(String.Format("ON DDM.{0} = DCDM.{0} ", ColumnDataCollectionDoMst.DO_ID));

                    sb.Append(String.Format("WHERE DCDM.{0} = @{0} ", ColumnDataCollectionDoMst.COMPOSITION_ID));
                    sb.Append(String.Format("AND DCDM.{0} = @{0} ", ColumnDataCollectionDoMst.PRODUCT_TYPE_ID));
                    sb.Append(String.Format("AND DCDM.{0} = @{0} ", ColumnDataCollectionDoMst.PROCESS_IDX));

                    sb.Append(String.Format("Order by {0};", ColumnDoDeviceMst.CREATE_DATETIME));

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionDoMst.COMPOSITION_ID}", SqlDbType.UniqueIdentifier)).Value = compositionId;
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionDoMst.PRODUCT_TYPE_ID}", SqlDbType.UniqueIdentifier)).Value = productId;
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionDoMst.PROCESS_IDX}", SqlDbType.Int)).Value = processIndex;

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
                logger.Debug($"Upsert:{ColumnDoDeviceMst.TABLE_NAME}");

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
                            sb.Append(string.Format("MERGE INTO {0} AS T1 ", ColumnDoDeviceMst.TABLE_NAME));
                            sb.Append(string.Format("USING (SELECT @{0} AS {0}) AS T2 ", ColumnDoDeviceMst.DO_ID));
                            sb.Append(string.Format("ON (T1.{0} = T2.{0}) ", ColumnDoDeviceMst.DO_ID));
                            sb.Append("WHEN MATCHED THEN UPDATE SET ");
                            sb.Append(string.Format("{0} = @{0}, ", ColumnDoDeviceMst.DO_ID));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnDoDeviceMst.DO_NAME));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnDoDeviceMst.LONG_OUTPUT_NO));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnDoDeviceMst.SHORT_OUTPUT_NO));
                            sb.Append(string.Format("{0} = @{0}  ", ColumnDoDeviceMst.DEVICE_REMARK));
                            sb.Append("WHEN NOT MATCHED THEN INSERT (");
                            sb.Append(string.Format("{0}, ", ColumnDoDeviceMst.DO_ID));
                            sb.Append(string.Format("{0}, ", ColumnDoDeviceMst.DO_NAME));
                            sb.Append(string.Format("{0}, ", ColumnDoDeviceMst.LONG_OUTPUT_NO));
                            sb.Append(string.Format("{0}, ", ColumnDoDeviceMst.SHORT_OUTPUT_NO));
                            sb.Append(string.Format("{0}, ", ColumnDoDeviceMst.DEVICE_REMARK));
                            sb.Append(string.Format("{0}) ", ColumnDoDeviceMst.CREATE_DATETIME));
                            sb.Append("VALUES (");
                            sb.Append(string.Format("@{0}, ", ColumnDoDeviceMst.DO_ID));
                            sb.Append(string.Format("@{0}, ", ColumnDoDeviceMst.DO_NAME));
                            sb.Append(string.Format("@{0}, ", ColumnDoDeviceMst.LONG_OUTPUT_NO));
                            sb.Append(string.Format("@{0}, ", ColumnDoDeviceMst.SHORT_OUTPUT_NO));
                            sb.Append(string.Format("@{0}, ", ColumnDoDeviceMst.DEVICE_REMARK));
                            sb.Append(string.Format("@{0});", ColumnDoDeviceMst.CREATE_DATETIME));

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDoDeviceMst.DO_ID}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnDoDeviceMst.DO_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDoDeviceMst.DO_NAME}", SqlDbType.NVarChar)).Value = rowItem.Field<string>(ColumnDoDeviceMst.DO_NAME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDoDeviceMst.LONG_OUTPUT_NO}", SqlDbType.Int)).Value = rowItem.Field<int>(ColumnDoDeviceMst.LONG_OUTPUT_NO);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDoDeviceMst.SHORT_OUTPUT_NO}", SqlDbType.Int)).Value = rowItem.Field<int>(ColumnDoDeviceMst.SHORT_OUTPUT_NO);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDoDeviceMst.DEVICE_REMARK}", SqlDbType.NVarChar)).Value = rowItem.Field<string>(ColumnDoDeviceMst.DEVICE_REMARK) ?? string.Empty;
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDoDeviceMst.CREATE_DATETIME}", SqlDbType.DateTime)).Value = rowItem.Field<DateTime>(ColumnDoDeviceMst.CREATE_DATETIME);

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
                logger.Debug($"Delete:{ColumnDoDeviceMst.TABLE_NAME}");

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
                        sb.Append(string.Format("DELETE FROM {0} ", ColumnDoDeviceMst.TABLE_NAME));
                        sb.Append(string.Format("WHERE {0} = @{0};", ColumnDoDeviceMst.DO_ID));

                        cmd.CommandText = sb.ToString();
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter(string.Format("@{0}", ColumnDoDeviceMst.DO_ID), SqlDbType.UniqueIdentifier)).Value = deleteItem.Field<Guid>(ColumnDoDeviceMst.DO_ID);

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
                logger.Debug($"Delete:{ColumnDoDeviceMst.TABLE_NAME}");

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
                        sb.Append(string.Format("DELETE FROM {0} ", ColumnDoDeviceMst.TABLE_NAME));
                        sb.Append(string.Format("WHERE {0} = @{0};", ColumnDoDeviceMst.DO_ID));

                        cmd.CommandText = sb.ToString();
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter(string.Format("@{0}", ColumnDoDeviceMst.DO_ID), SqlDbType.UniqueIdentifier)).Value = deleteItem;

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
