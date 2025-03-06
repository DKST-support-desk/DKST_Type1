using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using log4net;

namespace DBConnect.SQL
{
    /// <summary>
    /// Reder_Device_Mstテーブルの列名の定義
    /// </summary>
    public static class ColumnReaderDeviceMst
    {
        /// <summary>
        /// テーブル名
        /// </summary>
        public const String TABLE_NAME = "Reder_Device_Mst";

        /// <summary>
        /// リーダID
        /// </summary>
        public const String READER_ID = "ReaderID";

        /// <summary>
        /// リーダ名
        /// </summary>
        public const String READER_NAME = "ReaderName";

        /// <summary>
        /// ポート番号
        /// </summary>
        public const String PORT_NO = "PortNo";

        /// <summary>
        /// 機器説明
        /// </summary>
        public const String DEVICE_REMARK = "DeviceRemark";

        /// <summary>
        /// 作成日時
        /// </summary>
        public const String CREATE_DATETIME = "CreateDateTime";
    }

    /// <summary>
    /// QRリーダ機器マスタのデータ操作
    /// </summary>
    public class SqlReaderDeviceMst : ISqlBase
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
        /// <summary>
        /// DB接続オブジェクト
        /// </summary>
        public SqlDBConnector DbConnector { get; set; }

        #endregion

        #region <Constructor>
        /// <summary>
        /// QRリーダ機器マスタのデータ操作
        /// </summary>
        public SqlReaderDeviceMst(String connectString)
        {
            mConnectString = connectString;
        }
        #endregion

        #region <Method>
        /// <summary>
        /// DB接続オープン
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
            DataTable dt = new DataTable(ColumnReaderDeviceMst.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Select:{ColumnReaderDeviceMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT * ");
                    sb.Append(String.Format("FROM {0} ", ColumnReaderDeviceMst.TABLE_NAME));
                    sb.Append(String.Format("ORDER BY {0};", ColumnReaderDeviceMst.CREATE_DATETIME));

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
        /// <returns>処理結果 true=成功 false=失敗</returns>
        public bool Upsert(DataTable dataTable)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Upsert:{ColumnReaderDeviceMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    foreach (DataRow rowItem in dataTable.Rows)
                    {
                        if (rowItem.RowState == DataRowState.Modified || rowItem.RowState == DataRowState.Added)
                        {
                            sb.Clear();
                            sb.Append(string.Format("MERGE INTO {0} AS T1 ", ColumnReaderDeviceMst.TABLE_NAME));
                            sb.Append(string.Format("USING (SELECT @{0} AS {0}) AS T2 ", ColumnReaderDeviceMst.READER_ID));
                            sb.Append(string.Format("ON (T1.{0} = T2.{0})", ColumnReaderDeviceMst.READER_ID));
                            sb.Append("WHEN MATCHED THEN UPDATE SET ");
                            sb.Append(string.Format("{0} = @{0}, ", ColumnReaderDeviceMst.READER_ID));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnReaderDeviceMst.READER_NAME));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnReaderDeviceMst.PORT_NO));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnReaderDeviceMst.DEVICE_REMARK));
                            sb.Append(string.Format("{0} = @{0} ", ColumnReaderDeviceMst.CREATE_DATETIME));
                            sb.Append("WHEN NOT MATCHED THEN INSERT (");
                            sb.Append(string.Format("{0}, ", ColumnReaderDeviceMst.READER_ID));
                            sb.Append(string.Format("{0}, ", ColumnReaderDeviceMst.READER_NAME));
                            sb.Append(string.Format("{0}, ", ColumnReaderDeviceMst.PORT_NO));
                            sb.Append(string.Format("{0}, ", ColumnReaderDeviceMst.DEVICE_REMARK));
                            sb.Append(string.Format("{0}) ", ColumnReaderDeviceMst.CREATE_DATETIME));
                            sb.Append("VALUES (");
                            sb.Append(string.Format("@{0}, ", ColumnReaderDeviceMst.READER_ID));
                            sb.Append(string.Format("@{0}, ", ColumnReaderDeviceMst.READER_NAME));
                            sb.Append(string.Format("@{0}, ", ColumnReaderDeviceMst.PORT_NO));
                            sb.Append(string.Format("@{0}, ", ColumnReaderDeviceMst.DEVICE_REMARK));
                            sb.Append(string.Format("@{0});", ColumnReaderDeviceMst.CREATE_DATETIME));

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnReaderDeviceMst.READER_ID}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnReaderDeviceMst.READER_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnReaderDeviceMst.READER_NAME}", SqlDbType.NVarChar)).Value = rowItem.Field<string>(ColumnReaderDeviceMst.READER_NAME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnReaderDeviceMst.PORT_NO}", SqlDbType.Int)).Value = rowItem.Field<int>(ColumnReaderDeviceMst.PORT_NO);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnReaderDeviceMst.DEVICE_REMARK}", SqlDbType.NVarChar)).Value = rowItem.Field<string>(ColumnReaderDeviceMst.DEVICE_REMARK) ?? string.Empty;
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnReaderDeviceMst.CREATE_DATETIME}", SqlDbType.DateTime)).Value = rowItem.Field<DateTime>(ColumnReaderDeviceMst.CREATE_DATETIME);

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
        /// DELETE処理の実行
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns>処理結果 true=成功 false=失敗</returns>
        public bool Delete(DataTable dataTable)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Delete:{ColumnReaderDeviceMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }
                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    foreach (DataRow deleteItem in dataTable.Rows)
                    {
                        sb.Clear();
                        sb.Append(string.Format("DELETE FROM {0} ", ColumnReaderDeviceMst.TABLE_NAME));
                        sb.Append(string.Format("WHERE {0} = @{0};", ColumnReaderDeviceMst.READER_ID));

                        cmd.CommandText = sb.ToString();
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter(string.Format("@{0}", ColumnReaderDeviceMst.READER_ID), SqlDbType.UniqueIdentifier)).Value = deleteItem.Field<Guid>(ColumnReaderDeviceMst.READER_ID);

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
                logger.Debug($"Delete:{ColumnReaderDeviceMst.TABLE_NAME}");

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
                        sb.Append(string.Format("DELETE FROM {0} ", ColumnReaderDeviceMst.TABLE_NAME));
                        sb.Append(string.Format("WHERE {0} = @{0};", ColumnReaderDeviceMst.READER_ID));

                        cmd.CommandText = sb.ToString();
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter(string.Format("@{0}", ColumnReaderDeviceMst.READER_ID), SqlDbType.UniqueIdentifier)).Value = deleteItem;

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
