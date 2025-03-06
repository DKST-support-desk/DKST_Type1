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
    public class ColumnStandardValProductTypeMst
    {
        public const String TABLE_NAME = "StandardVal_ProductType_Mst";
        public const String COMPOSITION_ID = "CompositionId";
        public const String PRODUCT_TYPE_ID = "ProductTypeId";
        public const String HUMAN_TIME = "HumanTime";
        public const String MACHINE_TIME = "MachineTime";
        public const String MACHINE_CYCLE_TIME = "MachineCycleTime";
    }
    public class SqlStandardValProductTypeMst : ISqlBase
    {
        #region <Field>
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
        public SqlStandardValProductTypeMst(String connectString)
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

        public DataTable Select()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 条件付きSELECT処理の実行
        /// </summary>
        /// <param name="compositionId"></param>
        /// <param name="productTypeId"></param>
        /// <returns></returns>
        public DataTable Select(Guid compositionId, Guid productTypeId)
        {
            DataTable dt = new DataTable(ColumnStandardValProductTypeMst.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Select:{ColumnStandardValProductTypeMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT * ");
                    sb.Append(String.Format("FROM {0} ", ColumnStandardValProductTypeMst.TABLE_NAME));
                    sb.Append(String.Format("WHERE {0} = @{0} ", ColumnStandardValProductTypeMst.COMPOSITION_ID));
                    sb.Append(String.Format("AND {0} = @{0} ", ColumnStandardValProductTypeMst.PRODUCT_TYPE_ID));

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeMst.COMPOSITION_ID}", SqlDbType.UniqueIdentifier)).Value = compositionId;
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeMst.PRODUCT_TYPE_ID}", SqlDbType.UniqueIdentifier)).Value = productTypeId;

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
                logger.Debug($"Upsert:{ColumnStandardValProductTypeMst.TABLE_NAME}");
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
                            sb.Append(string.Format("MERGE INTO {0} AS T1 ", ColumnStandardValProductTypeMst.TABLE_NAME));
                            sb.Append("USING (SELECT ");
                            sb.Append(string.Format("@{0} AS {0} ", ColumnStandardValProductTypeMst.COMPOSITION_ID));
                            sb.Append(string.Format(",@{0} AS {0}) AS T2 ", ColumnStandardValProductTypeMst.PRODUCT_TYPE_ID));
                            sb.Append(string.Format("ON (T1.{0} = T2.{0} ", ColumnStandardValProductTypeMst.COMPOSITION_ID));
                            sb.Append(string.Format("AND T1.{0} = T2.{0}) ", ColumnStandardValProductTypeMst.PRODUCT_TYPE_ID));
                            sb.Append("WHEN MATCHED THEN UPDATE SET ");
                            sb.Append(string.Format("{0} = @{0} ", ColumnStandardValProductTypeMst.HUMAN_TIME));
                            sb.Append(string.Format(",{0} = @{0} ", ColumnStandardValProductTypeMst.MACHINE_TIME));
                            sb.Append(string.Format(",{0} = @{0} ", ColumnStandardValProductTypeMst.MACHINE_CYCLE_TIME));
                            sb.Append("WHEN NOT MATCHED THEN INSERT (");
                            sb.Append(string.Format("{0} ", ColumnStandardValProductTypeMst.COMPOSITION_ID));
                            sb.Append(string.Format(",{0} ", ColumnStandardValProductTypeMst.PRODUCT_TYPE_ID));
                            sb.Append(string.Format(",{0} ", ColumnStandardValProductTypeMst.HUMAN_TIME));
                            sb.Append(string.Format(",{0} ", ColumnStandardValProductTypeMst.MACHINE_TIME));
                            sb.Append(string.Format(",{0}) ", ColumnStandardValProductTypeMst.MACHINE_CYCLE_TIME));
                            sb.Append("VALUES (");
                            sb.Append(string.Format("@{0} ", ColumnStandardValProductTypeMst.COMPOSITION_ID));
                            sb.Append(string.Format(",@{0} ", ColumnStandardValProductTypeMst.PRODUCT_TYPE_ID));
                            sb.Append(string.Format(",@{0} ", ColumnStandardValProductTypeMst.HUMAN_TIME));
                            sb.Append(string.Format(",@{0} ", ColumnStandardValProductTypeMst.MACHINE_TIME));
                            sb.Append(string.Format(",@{0});", ColumnStandardValProductTypeMst.MACHINE_CYCLE_TIME));

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeMst.COMPOSITION_ID}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnStandardValProductTypeMst.COMPOSITION_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeMst.PRODUCT_TYPE_ID}", SqlDbType.UniqueIdentifier)).Value = rowItem.Field<Guid>(ColumnStandardValProductTypeMst.PRODUCT_TYPE_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeMst.HUMAN_TIME}", SqlDbType.Decimal)).Value = rowItem.Field<decimal>(ColumnStandardValProductTypeMst.HUMAN_TIME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeMst.MACHINE_TIME}", SqlDbType.Decimal)).Value = rowItem.Field<decimal>(ColumnStandardValProductTypeMst.MACHINE_TIME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnStandardValProductTypeMst.MACHINE_CYCLE_TIME}", SqlDbType.Decimal)).Value = rowItem.Field<decimal>(ColumnStandardValProductTypeMst.MACHINE_CYCLE_TIME);

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
