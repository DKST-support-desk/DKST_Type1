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
    /// Composition_Mstテーブルの列名の定義
    /// </summary>
    public class ColumnCompositionMst
    {
        /// <summary>
        /// テーブル名
        /// </summary>
        public const string TABLE_NAME = "Composition_Mst";

        /// <summary>
        /// 結合テーブル名
        /// </summary>
        public const string JOIN_TABLE_NAME = "Composition_Process_Mst";

        /// <summary>
        /// 編成ID
        /// </summary>
        public const string COMPOSITION_ID = "CompositionId";

        /// <summary>
        /// 編成名
        /// </summary>
        public const string UNIQUE_NAME = "UniqueName";

        /// <summary>
        /// 編成番号
        /// </summary>
        public const string COMPOSITION_NUMBER = "CompositionNumber";

        /// <summary>
        /// 編成工程数
        /// </summary>
        public const string COMPOSITION_PROCESS_NUM = "CompositionProcessNum";

        /// <summary>
        /// 作成日時
        /// </summary>
        public const string CREATE_DATETIME = "CreateDateTime";

        /// <summary>
        /// 工程インデックス
        /// </summary>
        public const string PROCESS_IDX = "ProcessIdx";

        /// <summary>
        /// 工程名
        /// </summary>
        public const string PROCESS_NAME = "ProcessName";
    }

    /// <summary>
    /// 編成マスタのデータ操作
    /// </summary>
    public class SqlCompositionMst : ISqlBase
    {
        #region <Field>
        /// <summary>
        /// 接続文字列
        /// </summary>
        private string mConnectString;

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
        /// 編成マスタのデータ操作
        /// </summary>
        public SqlCompositionMst(string connectString)
        {
            mConnectString = connectString;
        }
        #endregion

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
            DataTable dt = new DataTable(ColumnCompositionMst.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Select:{ColumnCompositionMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append($"SELECT * FROM {ColumnCompositionMst.TABLE_NAME} as tb1");
                    sb.Append($" LEFT JOIN {ColumnCompositionMst.JOIN_TABLE_NAME} as tb2");
                    sb.Append($" ON tb1.{ColumnCompositionMst.COMPOSITION_ID} = tb2.{ColumnCompositionMst.COMPOSITION_ID}");
                    sb.Append($" ORDER BY tb1.{ColumnCompositionMst.CREATE_DATETIME}");

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
        /// SELECT処理の実行
        /// </summary>
        /// <returns></returns>
        public DataTable SelectCompositionRow(Guid compositionID)
        {
            DataTable dt = new DataTable(ColumnCompositionMst.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"SelectCompositionRow:{ColumnCompositionMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT Top(1) T1.* ");
                    sb.Append(string.Format(" FROM {0} T1 ", ColumnCompositionMst.TABLE_NAME));
                    sb.Append(string.Format(" Where T1.{0} = @{0} ", ColumnCompositionMst.COMPOSITION_ID));

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Add(new SqlParameter($"@{ColumnCompositionMst.COMPOSITION_ID}", SqlDbType.UniqueIdentifier)).Value = compositionID;

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
                logger.Debug($"Upsert:{ColumnCompositionMst.TABLE_NAME}");

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
                            sb.Append($"MERGE INTO {ColumnCompositionMst.TABLE_NAME} AS T1 ");
                            sb.Append(string.Format("USING (SELECT @{0} AS {0}) AS T2 ", ColumnCompositionMst.COMPOSITION_ID));
                            sb.Append(string.Format("ON (T1.{0} = T2.{0}) ", ColumnCompositionMst.COMPOSITION_ID));
                            sb.Append("WHEN MATCHED THEN UPDATE SET ");
                            sb.Append(string.Format("{0} = @{0}, ", ColumnCompositionMst.COMPOSITION_ID));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnCompositionMst.UNIQUE_NAME));
                            sb.Append(string.Format("{0} = @{0} ", ColumnCompositionMst.COMPOSITION_NUMBER));
                            sb.Append("WHEN NOT MATCHED THEN INSERT (");
                            sb.Append($"{ColumnCompositionMst.COMPOSITION_ID}, ");
                            sb.Append($"{ColumnCompositionMst.UNIQUE_NAME}, ");
                            sb.Append($"{ColumnCompositionMst.COMPOSITION_NUMBER}, ");
                            sb.Append($"{ColumnCompositionMst.COMPOSITION_PROCESS_NUM}, ");
                            sb.Append($"{ColumnCompositionMst.CREATE_DATETIME}");
                            sb.Append(") VALUES (");
                            sb.Append($"@{ColumnCompositionMst.COMPOSITION_ID}, ");
                            sb.Append($"@{ColumnCompositionMst.UNIQUE_NAME}, ");
                            sb.Append($"@{ColumnCompositionMst.COMPOSITION_NUMBER}, ");
                            sb.Append($"@{ColumnCompositionMst.COMPOSITION_PROCESS_NUM}, ");
                            sb.Append($"@{ColumnCompositionMst.CREATE_DATETIME}");
                            sb.Append(");");

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCompositionMst.COMPOSITION_ID}", SqlDbType.UniqueIdentifier)).Value = udpateItem.Field<Guid>(ColumnCompositionMst.COMPOSITION_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCompositionMst.UNIQUE_NAME}", SqlDbType.NVarChar)).Value = udpateItem.Field<string>(ColumnCompositionMst.UNIQUE_NAME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCompositionMst.COMPOSITION_NUMBER}", SqlDbType.NVarChar)).Value = udpateItem.Field<string>(ColumnCompositionMst.COMPOSITION_NUMBER);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCompositionMst.COMPOSITION_PROCESS_NUM}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnCompositionMst.COMPOSITION_PROCESS_NUM);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCompositionMst.CREATE_DATETIME}", SqlDbType.DateTime)).Value = udpateItem.Field<DateTime>(ColumnCompositionMst.CREATE_DATETIME);

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
        /// INSERT処理の実行(FrmAddComposition)
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public bool InsertFrmAdd(Guid compositionID, string UniqueName, int compositionNumber)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"InsertFrmAdd:{ColumnCompositionMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    //foreach (DataRow udpateItem in dataTable.Rows)
                    //{
                    //    if (udpateItem.RowState == DataRowState.Modified || udpateItem.RowState == DataRowState.Added)
                    //    {
                    sb.Clear();
                    sb.Append($"INSERT INTO {ColumnCompositionMst.TABLE_NAME}( ");
                    sb.Append($"{ColumnCompositionMst.COMPOSITION_ID}, ");
                    sb.Append($"{ColumnCompositionMst.UNIQUE_NAME}, ");
                    sb.Append($"{ColumnCompositionMst.COMPOSITION_NUMBER}, ");
                    sb.Append($"{ColumnCompositionMst.CREATE_DATETIME} ");
                    sb.Append(") VALUES ( ");
                    sb.Append($"@{ColumnCompositionMst.COMPOSITION_ID}, ");
                    sb.Append($"@{ColumnCompositionMst.UNIQUE_NAME}, ");
                    sb.Append($"@{ColumnCompositionMst.COMPOSITION_NUMBER}, ");
                    sb.Append($"@{ColumnCompositionMst.CREATE_DATETIME} ");
                    sb.Append(");");

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnCompositionMst.COMPOSITION_ID}", SqlDbType.UniqueIdentifier)).Value = compositionID;
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnCompositionMst.UNIQUE_NAME}", SqlDbType.NVarChar)).Value = UniqueName;
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnCompositionMst.COMPOSITION_NUMBER}", SqlDbType.NVarChar)).Value = compositionNumber;
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnCompositionMst.CREATE_DATETIME}", SqlDbType.DateTime)).Value = DateTime.Now;

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
        /// <returns></returns>
        public bool Delete(DataTable dataTable)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Delete:{ColumnCompositionMst.TABLE_NAME}");

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
                        sb.Append($"DELETE FROM {ColumnCompositionMst.TABLE_NAME} ");
                        sb.Append(string.Format("WHERE {0} = @{0};", ColumnCompositionMst.COMPOSITION_ID));

                        cmd.CommandText = sb.ToString();
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter($"@{ColumnCompositionMst.COMPOSITION_ID}", SqlDbType.UniqueIdentifier)).Value = deleteItem.Field<Guid>(ColumnCompositionMst.COMPOSITION_ID);

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
        public bool DeleteMultipleTable(List<Guid> deleteGuid)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"DeleteMultipleTable:{ColumnCompositionMst.TABLE_NAME}");

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
                        sb.Append(string.Format("DELETE FROM {0} ", ColumnCompositionMst.TABLE_NAME));
                        sb.Append(string.Format("WHERE {0} = @{0};", ColumnCompositionMst.COMPOSITION_ID));
                        sb.Append(string.Format("DELETE FROM {0} ", ColumnCompositionProcessMst.TABLE_NAME));
                        sb.Append(string.Format("WHERE {0} = @{0};", ColumnCompositionMst.COMPOSITION_ID));
                        sb.Append(string.Format("DELETE FROM {0} ", ColumnStandardValProductTypeMst.TABLE_NAME));
                        sb.Append(string.Format("WHERE {0} = @{0};", ColumnCompositionMst.COMPOSITION_ID));
                        sb.Append(string.Format("DELETE FROM {0} ", ColumnStandardValProductTypeProcessMst.TABLE_NAME));
                        sb.Append(string.Format("WHERE {0} = @{0};", ColumnCompositionMst.COMPOSITION_ID));
                        sb.Append(string.Format("DELETE FROM {0} ", ColumnDataCollectionCameraMst.TABLE_NAME));
                        sb.Append(string.Format("WHERE {0} = @{0};", ColumnCompositionMst.COMPOSITION_ID));
                        sb.Append(string.Format("DELETE FROM {0} ", ColumnDataCollectionTriggerMst.TABLE_NAME));
                        sb.Append(string.Format("WHERE {0} = @{0};", ColumnCompositionMst.COMPOSITION_ID));
                        sb.Append(string.Format("DELETE FROM {0} ", ColumnDataCollectionDoMst.TABLE_NAME));
                        sb.Append(string.Format("WHERE {0} = @{0};", ColumnCompositionMst.COMPOSITION_ID));

                        cmd.CommandText = sb.ToString();
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter(string.Format("@{0}", ColumnCompositionMst.COMPOSITION_ID), SqlDbType.UniqueIdentifier)).Value = deleteItem;

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
