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
    /// Composition_Process_Mstテーブルの列名の定義
    /// </summary>
    public class ColumnCompositionProcessMst
    {
        /// <summary>
        /// テーブル名
        /// </summary>
        public const string TABLE_NAME = "Composition_Process_Mst";

        /// <summary>
        /// 編成ID
        /// </summary>
        public const string COMPOSITION_ID = "CompositionId";

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
    /// 編成プロセスマスタのデータ操作
    /// </summary>
    public class SqlCompositionProcessMst : ISqlBase
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
        /// 編成プロセスマスタのデータ操作
        /// </summary>
        public SqlCompositionProcessMst(string connectString)
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
            DataTable dt = new DataTable(ColumnCompositionProcessMst.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Select:{ColumnCompositionProcessMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT * ");
                    sb.Append(string.Format(" FROM {0} ", ColumnCompositionProcessMst.TABLE_NAME));
                    sb.Append(string.Format("  Order by {0} ", ColumnCompositionProcessMst.PROCESS_IDX));

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
        public DataTable SelectProCompositionRow(Guid compositionID)
        {
            DataTable dt = new DataTable(ColumnCompositionProcessMst.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"SelectProCompositionRow:{ColumnCompositionProcessMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append(string.Format("SELECT T1.{0} ", ColumnCompositionProcessMst.COMPOSITION_ID));
                    sb.Append(string.Format(" ,T1.{0}", ColumnCompositionProcessMst.PROCESS_IDX));
                    sb.Append(string.Format(" ,T1.{0} + 1 DispProcessIdx", ColumnCompositionProcessMst.PROCESS_IDX));
                    sb.Append(string.Format(" ,T1.{0} ", ColumnCompositionProcessMst.PROCESS_NAME));
                    sb.Append(string.Format(" FROM {0} T1 ", ColumnCompositionProcessMst.TABLE_NAME));
                    sb.Append(string.Format(" Where T1.{0} = @{0} ", ColumnCompositionProcessMst.COMPOSITION_ID));
                    sb.Append(string.Format("  Order by {0} ", ColumnCompositionProcessMst.PROCESS_IDX));

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Add(new SqlParameter($"@{ColumnCompositionProcessMst.COMPOSITION_ID}", SqlDbType.UniqueIdentifier)).Value = compositionID;

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
        /// SELECT処理の実行
        /// </summary>
        /// <returns></returns>
        public DataTable Select(string compositionNumber)
        {
            DataTable dt = new DataTable();
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Select:{ColumnCompositionProcessMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append(string.Format("SELECT CM.{0},CM.{1}, CPM.{2}, CPM.{3} ",
                        ColumnCompositionMst.COMPOSITION_ID,
                        ColumnCompositionMst.UNIQUE_NAME,
                        ColumnCompositionProcessMst.PROCESS_IDX,
                        ColumnCompositionProcessMst.PROCESS_NAME));
                    sb.Append(string.Format("FROM {0} CM ", ColumnCompositionMst.TABLE_NAME));
                    sb.Append(string.Format("INNER JOIN {0} CPM ", ColumnCompositionProcessMst.TABLE_NAME));
                    sb.Append(string.Format("ON CM.{0}= CPM.{0} ", ColumnCompositionProcessMst.COMPOSITION_ID));
                    sb.Append(string.Format("AND CM.{0} = @{0} ", ColumnCompositionMst.COMPOSITION_NUMBER));

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter(string.Format("@{0}", ColumnCompositionMst.COMPOSITION_NUMBER), SqlDbType.NVarChar)).Value = compositionNumber;

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
                logger.Debug($"Upsert:{ColumnCompositionProcessMst.TABLE_NAME}");

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
                            sb.Append($"MERGE INTO {ColumnCompositionProcessMst.TABLE_NAME} AS T1 ");
                            sb.Append(string.Format("USING (SELECT @{0} AS {0},", ColumnCompositionProcessMst.COMPOSITION_ID));
                            sb.Append(string.Format("@{0} AS {0}) AS T2 ", ColumnCompositionProcessMst.PROCESS_IDX));
                            sb.Append(string.Format("ON (T1.{0} = T2.{0} ", ColumnCompositionProcessMst.COMPOSITION_ID));
                            sb.Append(string.Format("AND T1.{0} = T2.{0}) ", ColumnCompositionProcessMst.PROCESS_IDX));
                            sb.Append("WHEN MATCHED THEN UPDATE SET ");
                            sb.Append(string.Format("{0} = @{0}, ", ColumnCompositionProcessMst.COMPOSITION_ID));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnCompositionProcessMst.PROCESS_IDX));
                            sb.Append(string.Format("{0} = @{0} ", ColumnCompositionProcessMst.PROCESS_NAME));
                            sb.Append("WHEN NOT MATCHED THEN INSERT (");
                            sb.Append($"{ColumnCompositionProcessMst.COMPOSITION_ID}, ");
                            sb.Append($"{ColumnCompositionProcessMst.PROCESS_IDX}, ");
                            sb.Append($"{ColumnCompositionProcessMst.PROCESS_NAME}");
                            sb.Append(") VALUES (");
                            sb.Append($"@{ColumnCompositionProcessMst.COMPOSITION_ID}, ");
                            sb.Append($"@{ColumnCompositionProcessMst.PROCESS_IDX}, ");
                            sb.Append($"@{ColumnCompositionProcessMst.PROCESS_NAME}");
                            sb.Append(");");

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCompositionProcessMst.COMPOSITION_ID}", SqlDbType.UniqueIdentifier)).Value = udpateItem.Field<Guid>(ColumnCompositionProcessMst.COMPOSITION_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCompositionProcessMst.PROCESS_IDX}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnCompositionProcessMst.PROCESS_IDX) - 1;
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCompositionProcessMst.PROCESS_NAME}", SqlDbType.NVarChar)).Value = udpateItem.Field<string>(ColumnCompositionProcessMst.PROCESS_NAME);

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
        public bool InsertFrmAdd(int processIdx, Guid compositionId)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"InsertFrmAdd:{ColumnCompositionProcessMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    for (int i = 0; i < processIdx; i++)
                    {
                        sb.Clear();
                        sb.Append($"INSERT INTO {ColumnCompositionProcessMst.TABLE_NAME} ( ");
                        sb.Append($"{ColumnCompositionProcessMst.COMPOSITION_ID}, ");
                        sb.Append($"{ColumnCompositionProcessMst.PROCESS_IDX}, ");
                        sb.Append($"{ColumnCompositionProcessMst.PROCESS_NAME} ");
                        sb.Append(") VALUES (");
                        sb.Append($"@{ColumnCompositionProcessMst.COMPOSITION_ID}, ");
                        sb.Append($"@{ColumnCompositionProcessMst.PROCESS_IDX}, ");
                        sb.Append($"@{ColumnCompositionProcessMst.PROCESS_NAME} ");
                        sb.Append(");");

                        cmd.CommandText = sb.ToString();

                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter($"@{ColumnCompositionProcessMst.COMPOSITION_ID}", SqlDbType.UniqueIdentifier)).Value = compositionId;
                        cmd.Parameters.Add(new SqlParameter($"@{ColumnCompositionProcessMst.PROCESS_IDX}", SqlDbType.Int)).Value = i;
                        cmd.Parameters.Add(new SqlParameter($"@{ColumnCompositionProcessMst.PROCESS_NAME}", SqlDbType.NVarChar)).Value = $"工程({i + 1})";

                        String paramStr = "";
                        if (SqlDBConnector.CreateParamsText(cmd.Parameters, out paramStr))
                        {
                            sb.Append(paramStr);
                        }

                        cmd.ExecuteNonQuery();
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
                logger.Debug($"Delete:{ColumnCompositionProcessMst.TABLE_NAME}");

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
                        sb.Append($"DELETE FROM {ColumnCompositionProcessMst.TABLE_NAME} ");
                        sb.Append(string.Format("WHERE {0} = @{0};", ColumnCompositionProcessMst.COMPOSITION_ID));

                        cmd.CommandText = sb.ToString();
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter($"@{ColumnCompositionProcessMst.COMPOSITION_ID}", SqlDbType.UniqueIdentifier)).Value = deleteItem.Field<Guid>(ColumnCompositionProcessMst.COMPOSITION_ID);

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
                logger.Debug($"Delete:{ColumnCompositionProcessMst.TABLE_NAME}");

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
                        sb.Append(string.Format("DELETE FROM {0} ", ColumnCompositionProcessMst.TABLE_NAME));
                        sb.Append(string.Format("WHERE {0} = @{0};", ColumnCompositionProcessMst.COMPOSITION_ID));

                        cmd.CommandText = sb.ToString();
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter(string.Format("@{0}", ColumnCompositionProcessMst.COMPOSITION_ID), SqlDbType.UniqueIdentifier)).Value = deleteItem;

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
