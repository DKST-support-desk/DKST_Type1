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
    /// Exclusion_Mstテーブルの列名の定義
    /// </summary>
    public static class ColumnExclusionMst
    {
        /// <summary>
        /// テーブル名
        /// </summary>
        public const String TABLE_NAME = "Exclusion_Mst";

        /// <summary>
        /// 除外区分ID
        /// </summary>
        public const String EXCLUSION_ID = "ExclusionId";

        /// <summary>
        /// 除外区分名
        /// </summary>
        public const String EXCLUSION_NAME = "ExclusionName";

        /// <summary>
        /// 作成日時
        /// </summary>
        public const String CREATE_DATETIME = "CreateDateTime";
    }

    /// <summary>
    /// 除外区分マスタのデータ操作
    /// </summary>
    public class SqlExclusionMst : ISqlBase
    {
        #region <Field>
        /// <summary>
        /// 接続文字列
        /// </summary>
        private String mConnectString;

        /// <summary>
        /// データ収集トリガーマスタに同じ品番タイプIDが存在するか
        /// </summary>
        public const string EXIST_IN_OPERATING_SHIFT_EXCLUSION = "ExistInOperatingShiftExclusion";

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
        public SqlExclusionMst(String connectString)
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
        /// SELECT処理の実行
        /// </summary>
        /// <returns></returns>
        public DataTable Select()
        {
            DataTable dt = new DataTable(ColumnExclusionMst.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                //logger.Debug($"Select:{ColumnExclusionMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT *, ");
                    sb.Append(String.Format("((SELECT COUNT(*) FROM {0} WHERE ( {0}.{1} = EM.{2})) ",
                        ColumnOperatingShiftExclusionTbl.TABLE_NAME,
                        ColumnOperatingShiftExclusionTbl.EXCLUSION_CLASS,
                        ColumnExclusionMst.EXCLUSION_ID));
                    sb.Append(String.Format("+ (SELECT COUNT(*) FROM {0} WHERE ( {0}.{1} = EM.{2}))) {3} ",
                        ColumnOperatingShiftExclusionPatternTbl.TABLE_NAME,
                        ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_ID,
                        ColumnExclusionMst.EXCLUSION_ID,
                        EXIST_IN_OPERATING_SHIFT_EXCLUSION));

                    sb.Append(String.Format("FROM {0} EM ", ColumnExclusionMst.TABLE_NAME));
                    sb.Append(String.Format("ORDER BY {0};", ColumnExclusionMst.CREATE_DATETIME));

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
                logger.Debug($"Upsert:{ColumnExclusionMst.TABLE_NAME}");

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
                            sb.Append(string.Format("MERGE INTO {0} AS T1 ", ColumnExclusionMst.TABLE_NAME));
                            sb.Append(string.Format("USING (SELECT @{0} AS {0}) AS T2 ", ColumnExclusionMst.EXCLUSION_ID));
                            sb.Append(string.Format("ON (T1.{0} = T2.{0})", ColumnExclusionMst.EXCLUSION_ID));
                            sb.Append("WHEN MATCHED THEN UPDATE SET ");
                            sb.Append(string.Format("{0} = @{0}, ", ColumnExclusionMst.EXCLUSION_ID));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnExclusionMst.EXCLUSION_NAME));
                            sb.Append(string.Format("{0} = @{0} ", ColumnExclusionMst.CREATE_DATETIME));
                            sb.Append("WHEN NOT MATCHED THEN INSERT (");
                            sb.Append(string.Format("{0}, ", ColumnExclusionMst.EXCLUSION_ID));
                            sb.Append(string.Format("{0}, ", ColumnExclusionMst.EXCLUSION_NAME));
                            sb.Append(string.Format("{0}) ", ColumnExclusionMst.CREATE_DATETIME));
                            sb.Append("VALUES (");
                            sb.Append(string.Format("@{0}, ", ColumnExclusionMst.EXCLUSION_ID));
                            sb.Append(string.Format("@{0}, ", ColumnExclusionMst.EXCLUSION_NAME));
                            sb.Append(string.Format("@{0});", ColumnExclusionMst.CREATE_DATETIME));

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnExclusionMst.EXCLUSION_ID}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnExclusionMst.EXCLUSION_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnExclusionMst.EXCLUSION_NAME}", SqlDbType.NVarChar)).Value = rowItem.Field<string>(ColumnExclusionMst.EXCLUSION_NAME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnExclusionMst.CREATE_DATETIME}", SqlDbType.DateTime)).Value = rowItem.Field<DateTime>(ColumnExclusionMst.CREATE_DATETIME);

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
                logger.Debug($"Delete:{ColumnExclusionMst.TABLE_NAME}");

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
                        sb.Append(string.Format("DELETE FROM {0} ", ColumnExclusionMst.TABLE_NAME));
                        sb.Append(string.Format("WHERE {0} = @{0};", ColumnExclusionMst.EXCLUSION_ID));

                        cmd.CommandText = sb.ToString();
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter(string.Format("@{0}", ColumnExclusionMst.EXCLUSION_ID), SqlDbType.UniqueIdentifier)).Value = deleteItem.Field<Guid>(ColumnExclusionMst.EXCLUSION_ID);

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
                logger.Debug($"Delete:{ColumnExclusionMst.TABLE_NAME}");

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
                        sb.Append(string.Format("DELETE FROM {0} ", ColumnExclusionMst.TABLE_NAME));
                        sb.Append(string.Format("WHERE {0} = @{0};", ColumnExclusionMst.EXCLUSION_ID));

                        cmd.CommandText = sb.ToString();
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter(string.Format("@{0}", ColumnExclusionMst.EXCLUSION_ID), SqlDbType.UniqueIdentifier)).Value = deleteItem;

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
