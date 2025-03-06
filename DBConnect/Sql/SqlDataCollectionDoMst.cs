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
    /// DataCollection_DO_Mstテーブルの列名の定義
    /// </summary>
    public class ColumnDataCollectionDoMst
    {
        /// <summary>
        /// テーブル名
        /// </summary>
        public const string TABLE_NAME = "DataCollection_DO_Mst";

        public const string COMPOSITION_ID = "CompositionId";

        public const string PRODUCT_TYPE_ID = "ProductTypeId";

        public const string PROCESS_IDX = "ProcessIdx";

        public const string DO_ID = "DOId";
    }

    /// <summary>
    /// DO機器マスタのデータ操作
    /// </summary>
    public class SqlDataCollectionDoMst : ISqlBase
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
        public SqlDBConnector DbConnector { get; set; }

        #endregion

        #region <Constructor>
        public SqlDataCollectionDoMst(String connectString)
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
        /// SELECT処理
        /// </summary>
        public DataTable Select(Guid compositionId, Guid productTypeId, int processIdx = -1)
        {
            DataTable dt = new DataTable(ColumnDataCollectionDoMst.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Select:{ColumnDataCollectionDoMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT * ");
                    sb.Append(String.Format("FROM {0} T1 ", ColumnDataCollectionDoMst.TABLE_NAME));
                    sb.Append(String.Format("WHERE {0} = @{0} ", ColumnDataCollectionDoMst.COMPOSITION_ID));
                    sb.Append(String.Format("And {0} = @{0} ", ColumnDataCollectionDoMst.PRODUCT_TYPE_ID));
                    if (processIdx > -1)
                    {
                        sb.Append(String.Format("And {0} = @{0} ", ColumnDataCollectionDoMst.PROCESS_IDX));
                    }
                    sb.Append(String.Format("Order by {0}; ", ColumnDataCollectionDoMst.PROCESS_IDX));

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Add(new SqlParameter(String.Format("@{0}", ColumnDataCollectionDoMst.COMPOSITION_ID), SqlDbType.UniqueIdentifier)).Value = compositionId;
                    cmd.Parameters.Add(new SqlParameter(String.Format("@{0}", ColumnDataCollectionDoMst.PRODUCT_TYPE_ID), SqlDbType.UniqueIdentifier)).Value = productTypeId;
                    if (processIdx > -1)
                    {
                        cmd.Parameters.Add(new SqlParameter(String.Format("@{0}", ColumnDataCollectionDoMst.PROCESS_IDX), SqlDbType.Int)).Value = processIdx;
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
        /// Upsert処理
        /// </summary>
        /// <param name="updateDataTable"></param>
        /// <returns></returns>
        public bool Upsert(DataTable updateDataTable, SqlTransaction transaction)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Upsert:{ColumnDataCollectionDoMst.TABLE_NAME}");

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
                            sb.Append(string.Format("MERGE INTO {0} AS T1 ", ColumnDataCollectionDoMst.TABLE_NAME));
                            sb.Append("USING (SELECT ");
                            sb.Append(string.Format("@{0} AS {0} ", ColumnDataCollectionDoMst.COMPOSITION_ID));
                            sb.Append(string.Format(",@{0} AS {0} ", ColumnDataCollectionDoMst.PRODUCT_TYPE_ID));
                            sb.Append(string.Format(",@{0} AS {0}) AS T2 ", ColumnDataCollectionDoMst.PROCESS_IDX));
                            sb.Append(string.Format("ON (T1.{0} = T2.{0} ", ColumnDataCollectionDoMst.COMPOSITION_ID));
                            sb.Append(string.Format("AND T1.{0} = T2.{0} ", ColumnDataCollectionDoMst.PRODUCT_TYPE_ID));
                            sb.Append(string.Format("AND T1.{0} = T2.{0}) ", ColumnDataCollectionDoMst.PROCESS_IDX));
                            sb.Append("WHEN MATCHED THEN UPDATE SET ");
                            sb.Append(string.Format("{0} = @{0} ", ColumnDataCollectionDoMst.DO_ID));
                            sb.Append("WHEN NOT MATCHED THEN INSERT (");
                            sb.Append(string.Format("{0} ", ColumnDataCollectionDoMst.COMPOSITION_ID));
                            sb.Append(string.Format(",{0} ", ColumnDataCollectionDoMst.PRODUCT_TYPE_ID));
                            sb.Append(string.Format(",{0} ", ColumnDataCollectionDoMst.PROCESS_IDX));
                            sb.Append(string.Format(",{0}) ", ColumnDataCollectionDoMst.DO_ID));
                            sb.Append("VALUES (");
                            sb.Append(string.Format("@{0} ", ColumnDataCollectionDoMst.COMPOSITION_ID));
                            sb.Append(string.Format(",@{0} ", ColumnDataCollectionDoMst.PRODUCT_TYPE_ID));
                            sb.Append(string.Format(",@{0} ", ColumnDataCollectionDoMst.PROCESS_IDX));
                            sb.Append(string.Format(",@{0});", ColumnDataCollectionDoMst.DO_ID));

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionDoMst.COMPOSITION_ID}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnDataCollectionDoMst.COMPOSITION_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionDoMst.PRODUCT_TYPE_ID}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnDataCollectionDoMst.PRODUCT_TYPE_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionDoMst.PROCESS_IDX}", SqlDbType.Int)).Value = rowItem.Field<int>(ColumnDataCollectionDoMst.PROCESS_IDX);
                            // 2022/06/13 西部修正　オール0のGUIDの場合はNULLでＤＢ登録を行う
                            if (rowItem.Field<Guid>(ColumnDataCollectionDoMst.DO_ID) != new Guid("00000000-0000-0000-0000-000000000000"))
                            {
                                cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionDoMst.DO_ID}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnDataCollectionDoMst.DO_ID);
                            }
                            else
                            { 
                                cmd.Parameters.Add(new SqlParameter($"@{ColumnDataCollectionDoMst.DO_ID}", SqlDbType.UniqueIdentifier)).Value = System.Data.SqlTypes.SqlString.Null;
                            }

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
