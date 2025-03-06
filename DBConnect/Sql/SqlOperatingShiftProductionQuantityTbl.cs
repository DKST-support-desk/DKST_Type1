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
    public class ColumnOperatingShiftProductionQuantityTbl
    {
        public const String TABLE_NAME = "Operating_Shift_ProductionQuantity_Tbl";
        public const String OPERATION_DATE = "OperationDate";
        public const String OPERATION_SHIFT = "OperationShift";
        public const String COMPOSITION_ID = "CompositionId";
        public const String PRODUCT_TYPE_ID = "ProductTypeId";
        public const String PRODUCTION_QUANTITY = "ProductionQuantity";
        public const String PRODUCTION_QUANTITY_ON_CYCLE = "ProductionQuantityOnCycle";

    }
    public class SqlOperatingShiftProductionQuantityTbl : ISqlBase
    {
        #region <Field>
        /// <summary>
        /// 接続文字列
        /// </summary>
        private string mConnectString { get; set; }

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
        /// コンストラクタ
        /// </summary>
        public SqlOperatingShiftProductionQuantityTbl(string connectString)
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
            DataTable dt = new DataTable(ColumnOperatingShiftProductionQuantityTbl.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                //logger.Debug($"Select:{ColumnOperatingShiftProductionQuantityTbl.TABLE_NAME}");
                
                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT * ");
                    sb.Append(String.Format("FROM {0} ", ColumnOperatingShiftProductionQuantityTbl.TABLE_NAME));
                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();

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
        /// 条件付きSELECT処理の実行
        /// </summary>
        /// <param name="OperationDate">稼働日</param>
        /// <param name="OperationShift">稼働シフト</param>
        /// <returns></returns>
        public DataTable Select(DateTime OperationDate, int OperationShift = 0)
        {
            DataTable dt = new DataTable(ColumnOperatingShiftProductionQuantityTbl.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                //logger.Debug($"ConditionalSelect:{ColumnOperatingShiftProductionQuantityTbl.TABLE_NAME}");
                
                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT * ");
                    sb.Append(String.Format("FROM {0} ", ColumnOperatingShiftProductionQuantityTbl.TABLE_NAME));
                    sb.Append("WHERE 1 = 1 ");
                    sb.Append(String.Format("And {0} = @{0} ", ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE));
                    if (OperationShift != 0)
                    {
                        sb.Append(String.Format("And {0} = @{0} ", ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT));
                    }
                    sb.Append(String.Format("Order by {0}, {1};", ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE, ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT));

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(String.Format("@{0}", ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE), SqlDbType.DateTime).Value = OperationDate;
                    if (OperationShift != 0)
                    {
                        cmd.Parameters.Add(String.Format("@{0}", ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT), SqlDbType.Int).Value = OperationShift;
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

        /// <summary>
        /// SELECT処理の実行（編成情報取得）
        /// </summary>
        /// <param name="OperationDate">稼働日</param>
        /// <param name="OperationShift">稼働シフト</param>
        /// <returns>取得データ</returns>
        public DataTable SelectComposition(DateTime OperationDate, int OperationShift)
        {
            DataTable dt = new DataTable(ColumnCycleResult.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"SelectComposition:{ColumnOperatingShiftProductionQuantityTbl.TABLE_NAME}");
                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append(String.Format("SELECT DISTINCT * "));
                    sb.Append(String.Format("FROM {0} INNER JOIN {1} ", ColumnOperatingShiftProductionQuantityTbl.TABLE_NAME, ColumnCompositionMst.TABLE_NAME));
                    sb.Append(String.Format("ON {0}.{1} = {2}.{3} ", ColumnOperatingShiftProductionQuantityTbl.TABLE_NAME, ColumnOperatingShiftProductionQuantityTbl.COMPOSITION_ID
                                                                   , ColumnCompositionMst.TABLE_NAME, ColumnCompositionMst.COMPOSITION_ID));

                    sb.Append("WHERE 1 = 1 ");
                    sb.Append(String.Format("And {0} = @{0} ", ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE));
                    sb.Append(String.Format("And {0} = @{0} ", ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT));
                    sb.Append(String.Format("Order by {0}, {1};", ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE, ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT));

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(String.Format("@{0}", ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE), SqlDbType.DateTime).Value = OperationDate;
                    cmd.Parameters.Add(String.Format("@{0}", ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT), SqlDbType.Int).Value = OperationShift;

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
            // NO USE
            return false;
        }

        /// <summary>
        /// UPDERT処理の実行
        /// </summary>
        /// <param name="updateDataTable"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public bool Update(DataTable updateDataTable, SqlTransaction transaction = null)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Update:{ColumnOperatingShiftProductionQuantityTbl.TABLE_NAME}");

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
                    foreach (DataRow udpateItem in updateDataTable.Rows)
                    {
                        if (udpateItem.RowState == DataRowState.Modified /*|| udpateItem.RowState == DataRowState.Added*/)
                        {
                            sb.Clear();
                            sb.Append(string.Format("UPDATE {0} SET {1} = @{1} ",
                                ColumnOperatingShiftProductionQuantityTbl.TABLE_NAME,
                                ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY));
                            sb.Append(string.Format("WHERE {0} = @{0} AND {1} = @{1} AND {2} = @{2} AND {3} = @{3} AND {4} IS NOT NULL; ",
                                ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE,
                                ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT,
                                ColumnOperatingShiftProductionQuantityTbl.COMPOSITION_ID,
                                ColumnOperatingShiftProductionQuantityTbl.PRODUCT_TYPE_ID,
                                ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY_ON_CYCLE));

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE}", SqlDbType.Date)).Value = udpateItem.Field<DateTime>(ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftProductionQuantityTbl.COMPOSITION_ID}", SqlDbType.UniqueIdentifier)).Value = udpateItem.Field<Guid>(ColumnOperatingShiftProductionQuantityTbl.COMPOSITION_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftProductionQuantityTbl.PRODUCT_TYPE_ID}", SqlDbType.UniqueIdentifier)).Value = udpateItem.Field<Guid>(ColumnOperatingShiftProductionQuantityTbl.PRODUCT_TYPE_ID);
                            if(udpateItem.Field<int?>(ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY) == null)
                            {
                                continue;
                            }
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY);

                            String paramStr = "";
                            if (SqlDBConnector.CreateParamsText(cmd.Parameters, out paramStr))
                            {
                                sb.Append(paramStr);
                            }

                            cmd.Transaction = transaction;
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

        public bool Upsert(DataTable dataTable)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Upsert:{ColumnOperatingShiftProductionQuantityTbl.TABLE_NAME}");

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
                            sb.Append($"MERGE INTO {ColumnOperatingShiftProductionQuantityTbl.TABLE_NAME} AS T1 ");
                            //sb.Append(string.Format("USING (SELECT * from {0} )AS T2 ", ColumnResultOperatingShiftTbl.TABLE_NAME));
                            sb.Append(string.Format("USING (SELECT @{0} AS {0}, @{1} AS {1} )AS T2 ", ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE, ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT));
                            sb.Append(string.Format("ON (T1.{0} = T2.{0} AND T1.{1} = T2.{1} AND T1.{2} = '{3}' AND T1.{4} = '{5}') "
                                , ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE
                                , ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT
                                , ColumnOperatingShiftProductionQuantityTbl.COMPOSITION_ID
                                , udpateItem.Field<Guid>(ColumnOperatingShiftProductionQuantityTbl.COMPOSITION_ID)
                                , ColumnOperatingShiftProductionQuantityTbl.PRODUCT_TYPE_ID
                                , udpateItem.Field<Guid>(ColumnOperatingShiftProductionQuantityTbl.PRODUCT_TYPE_ID)
                                ));
                            sb.Append("WHEN MATCHED THEN UPDATE SET ");
                            sb.Append(string.Format("{0} = {0} + 1 ", ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY_ON_CYCLE));
                            sb.Append("WHEN NOT MATCHED THEN INSERT (");
                            sb.Append($"{ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE}, ");
                            sb.Append($"{ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT}, ");
                            sb.Append($"{ColumnOperatingShiftProductionQuantityTbl.COMPOSITION_ID}, ");
                            sb.Append($"{ColumnOperatingShiftProductionQuantityTbl.PRODUCT_TYPE_ID}, ");
                            sb.Append($"{ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY_ON_CYCLE} ");
                            sb.Append(") VALUES (");
                            sb.Append($"@{ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE}, ");
                            sb.Append($"@{ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT}, ");
                            sb.Append($"@{ColumnOperatingShiftProductionQuantityTbl.COMPOSITION_ID}, ");
                            sb.Append($"@{ColumnOperatingShiftProductionQuantityTbl.PRODUCT_TYPE_ID}, ");
                            sb.Append($"@{ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY_ON_CYCLE} ");
                            sb.Append(");");

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE}", SqlDbType.Date)).Value = udpateItem.Field<DateTime>(ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnOperatingShiftProductionQuantityTbl.OPERATION_SHIFT);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftProductionQuantityTbl.COMPOSITION_ID}", SqlDbType.UniqueIdentifier)).Value = udpateItem.Field<Guid>(ColumnOperatingShiftProductionQuantityTbl.COMPOSITION_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftProductionQuantityTbl.PRODUCT_TYPE_ID}", SqlDbType.UniqueIdentifier)).Value = udpateItem.Field<Guid>(ColumnOperatingShiftProductionQuantityTbl.PRODUCT_TYPE_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY_ON_CYCLE}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnOperatingShiftProductionQuantityTbl.PRODUCTION_QUANTITY_ON_CYCLE);

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

        public bool Delete(DataTable deleteDataTable)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// DELETE処理の実行
        /// </summary>
        /// <param name="deleteDateTime"></param>
        /// <returns></returns>
        public bool Delete(DateTime deleteDateTime)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Delete:{ColumnOperatingShiftProductionQuantityTbl.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {

                    sb.Clear();
                    sb.Append(String.Format("DELETE {0} ", ColumnOperatingShiftProductionQuantityTbl.TABLE_NAME));
                    sb.Append(String.Format("WHERE {0} < @{0} ", ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE));
                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftProductionQuantityTbl.OPERATION_DATE}", SqlDbType.Date)).Value = deleteDateTime;

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
                logger.Error($"Error Query:{sb.ToString()}");
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
