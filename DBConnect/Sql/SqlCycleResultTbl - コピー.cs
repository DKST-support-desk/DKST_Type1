﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace DBConnect.SQL
{
    public class ColumnCycleResult
    {
        public const String TABLE_NAME = "CycleResult_Tbl";
        public const String COMPOSITION_ID = "CompositionId";
        public const String PRODUCT_TYPE_ID = "ProductTypeId";
        public const String PROCESS_IDX = "ProcessIdx";
        public const String START_TIME = "StartTime";
        public const String END_TIME = "EndTime";
        public const String CYCLE_TIME = "CycleTime";
        public const String OPERATION_DATE = "OperationDate";
        public const String OPERATION_SHIFT = "OperationShift";
        public const String WORKER_NAME = "WorkerName";
        public const String VIDEO_FILE_NAME = "VideoFileName";
        public const String ERROR_FLAG = "ErrorFlag";
        public const String GOOD_FLAG = "GoodFlag";
    }
    /// <summary>
    /// サイクル実績テーブル定義
    /// </summary>
    public class SqlCycleResultTbl : ISqlBase
    {
        #region <Field>
        /// <summary>
        /// 接続文字列
        /// </summary>
        private string mConnectString { get; set; }

        public const String SELECT_COUNT_COLUMN_NAME = "SelectCountProductionByDate";

        /// <summary>
        /// 良品カウンタカラム
        /// </summary>
        public const string COLUMN_GOOD_FLAG_COUNT = "GoodFlagCount";

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
        public SqlCycleResultTbl(string connectString)
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

        public DataTable Select()
        {
            throw new NotImplementedException();
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
                logger.Debug($"SelectComposition:{ColumnCycleResult.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Clear();
                    sb.Append(String.Format("SELECT DISTINCT T1.{0},T2.{1},T1.{2},T1.{3}, T2.{4} ", 
                        ColumnCycleResult.COMPOSITION_ID, ColumnCompositionMst.UNIQUE_NAME, ColumnCycleResult.OPERATION_DATE, ColumnCycleResult.OPERATION_SHIFT, ColumnCompositionMst.CREATE_DATETIME));
                    sb.Append(String.Format("FROM {0} T1  ", ColumnCycleResult.TABLE_NAME));
                    sb.Append(String.Format("INNER JOIN {0} T2 ", ColumnCompositionMst.TABLE_NAME));
                    sb.Append(String.Format("ON T1.{0} = T2.{1} ", ColumnCycleResult.COMPOSITION_ID, ColumnCompositionMst.COMPOSITION_ID));

                    sb.Append("WHERE 1 = 1 ");
                    sb.Append(String.Format("And {0} = @{0} ", ColumnCycleResult.OPERATION_DATE));
                    sb.Append(String.Format("And {0} = @{0} ", ColumnCycleResult.OPERATION_SHIFT));
                    sb.Append(String.Format("Order by T2.{0};", ColumnCompositionMst.CREATE_DATETIME));

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(String.Format("@{0}", ColumnCycleResult.OPERATION_DATE), SqlDbType.DateTime).Value = OperationDate;
                    cmd.Parameters.Add(String.Format("@{0}", ColumnCycleResult.OPERATION_SHIFT), SqlDbType.Int).Value = OperationShift;

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
        /// SELECT処理の実行（品番タイプ情報取得）
        /// </summary>
        /// <param name="OperationDate">稼働日</param>
        /// <param name="OperationShift">稼働シフト</param>
        /// <param name="CompositionId">編成ID</param>
        /// <returns>取得データ</returns>
        public DataTable SelectProductType(DateTime OperationDate, int OperationShift, Guid CompositionId)
        {
            DataTable dt = new DataTable(ColumnCycleResult.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"SelectProductType:{ColumnCycleResult.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Clear();
                    sb.Append(String.Format("SELECT DISTINCT {0}.{1} ", ColumnProductTypeMst.TABLE_NAME, ColumnProductTypeMst.PRODUCT_TYPE_ID));
                    sb.Append(String.Format(",{0}.{1} ", ColumnProductTypeMst.TABLE_NAME, ColumnProductTypeMst.PRODUCT_TYPE_NAME));
                    sb.Append(String.Format(",{0}.{1} ", ColumnProductTypeMst.TABLE_NAME, ColumnProductTypeMst.ORDER_IDX));
                    sb.Append(String.Format("FROM {0} INNER JOIN {1} ", ColumnCycleResult.TABLE_NAME, ColumnProductTypeMst.TABLE_NAME));
                    sb.Append(String.Format("ON {0}.{1} = {2}.{3} ", ColumnCycleResult.TABLE_NAME, ColumnCycleResult.PRODUCT_TYPE_ID
                                                                      , ColumnProductTypeMst.TABLE_NAME, ColumnProductTypeMst.PRODUCT_TYPE_ID));
                    sb.Append(String.Format("Where {0}.{1} = @{1} ", ColumnCycleResult.TABLE_NAME, ColumnCycleResult.OPERATION_DATE));
                    sb.Append(String.Format("AND {0}.{1} = @{1} ", ColumnCycleResult.TABLE_NAME, ColumnCycleResult.OPERATION_SHIFT));
                    sb.Append(String.Format("AND {0}.{1} = @{1} ", ColumnCycleResult.TABLE_NAME, ColumnCycleResult.COMPOSITION_ID));
                    sb.Append(String.Format("ORDER BY {0}.{1};", ColumnProductTypeMst.TABLE_NAME, ColumnProductTypeMst.ORDER_IDX));

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(String.Format("@{0}", ColumnCycleResult.OPERATION_DATE), SqlDbType.DateTime).Value = OperationDate;
                    cmd.Parameters.Add(String.Format("@{0}", ColumnCycleResult.OPERATION_SHIFT), SqlDbType.Int).Value = OperationShift;
                    cmd.Parameters.Add(String.Format("@{0}", ColumnCycleResult.COMPOSITION_ID), SqlDbType.UniqueIdentifier).Value = CompositionId;

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
        /// SELECT処理の実行（生産数情報取得）
        /// →実績サイクルテーブルから良品工程通過データの件数を抽出し、取得
        /// </summary>
        /// <param name="OperationDate">稼働日</param>
        /// <param name="OperationShift">稼働シフト</param>
        /// <param name="CompositionId">編成ID</param>
        /// <param name="ProductTypeId">品番タイプID</param>
        /// <returns>取得データ</returns>
        public DataTable SelectProductionQuantity(DateTime OperationDate, int OperationShift, Guid CompositionId, Guid ProductTypeId)
        {
            DataTable dt = new DataTable(ColumnCycleResult.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"SelectProductionQuantity:{ColumnCycleResult.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Clear();
                    sb.Append(String.Format("SELECT  COUNT(*) as {0} from {1} ", COLUMN_GOOD_FLAG_COUNT, ColumnCycleResult.TABLE_NAME));
                    sb.Append(String.Format("Where {0} = @{0} ", ColumnCycleResult.OPERATION_DATE));
                    sb.Append(String.Format("AND {0} = @{0} ", ColumnCycleResult.OPERATION_SHIFT));
                    sb.Append(String.Format("AND {0}= @{0} ", ColumnCycleResult.COMPOSITION_ID));
                    sb.Append(String.Format("AND {0} = @{0} ", ColumnProductTypeMst.PRODUCT_TYPE_ID));
                    sb.Append(String.Format("AND {0} = 1 ", ColumnCycleResult.GOOD_FLAG));

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(String.Format("@{0}", ColumnCycleResult.OPERATION_DATE), SqlDbType.DateTime).Value = OperationDate;
                    cmd.Parameters.Add(String.Format("@{0}", ColumnCycleResult.OPERATION_SHIFT), SqlDbType.Int).Value = OperationShift;
                    cmd.Parameters.Add(String.Format("@{0}", ColumnCycleResult.COMPOSITION_ID), SqlDbType.UniqueIdentifier).Value = CompositionId;
                    cmd.Parameters.Add(String.Format("@{0}", ColumnProductTypeMst.PRODUCT_TYPE_ID), SqlDbType.UniqueIdentifier).Value = ProductTypeId;

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
        /// SELECT処理の実行（生産数情報取得）
        /// </summary>
        /// <param name="OperationDate">稼働日</param>
        /// <param name="OperationShift">稼働シフト</param>
        /// <param name="CompositionId">編成ID</param>
        /// <param name="ProductTypeId">品番タイプID</param>
        /// <returns>取得データ</returns>
        public DataTable SelectProductionByDate(DateTime start, DateTime end, int processIndex)
        {
            DataTable dt = new DataTable(ColumnCycleResult.TABLE_NAME);
            StringBuilder sb = new StringBuilder();
            try
            {
                logger.Debug($"SelectProductionByDate:{ColumnCycleResult.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Clear();
                    sb.Append(String.Format("SELECT {0} ", ColumnCycleResult.ERROR_FLAG));
                    sb.Append(String.Format(",(SELECT  COUNT(*)) as {0} ", SELECT_COUNT_COLUMN_NAME));
                    sb.Append(String.Format("FROM {0} ", ColumnCycleResult.TABLE_NAME));
                    sb.Append(String.Format("Where {0} = @{0} ", ColumnCycleResult.START_TIME));
                    sb.Append(String.Format("And {0} = @{0} ", ColumnCycleResult.END_TIME));
                    sb.Append(String.Format("And {0} = @{0} ", ColumnCycleResult.PROCESS_IDX));
                    sb.Append(String.Format(";"));

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(String.Format("@{0}", ColumnCycleResult.START_TIME), SqlDbType.DateTime).Value = start;
                    cmd.Parameters.Add(String.Format("@{0}", ColumnCycleResult.END_TIME), SqlDbType.DateTime).Value = end;
                    cmd.Parameters.Add(String.Format("@{0}", ColumnCycleResult.PROCESS_IDX), SqlDbType.Int).Value = processIndex;

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
        /// <param name="insertDataTable"></param>
        /// <returns></returns>
        public bool Insert(DataTable insertDataTable)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Insert:{ColumnCycleResult.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    foreach (DataRow udpateItem in insertDataTable.Rows)
                    {
                        if (udpateItem.RowState == DataRowState.Modified || udpateItem.RowState == DataRowState.Added)
                        {
                            sb.Clear();
                            sb.Append(String.Format("INSERT INTO {0} (", ColumnCycleResult.TABLE_NAME));
                            sb.Append($"{ColumnCycleResult.COMPOSITION_ID}");
                            sb.Append($", {ColumnCycleResult.PRODUCT_TYPE_ID}");
                            sb.Append($", {ColumnCycleResult.PROCESS_IDX}");
                            sb.Append($", {ColumnCycleResult.START_TIME}");
                            sb.Append($", {ColumnCycleResult.END_TIME}");
                            sb.Append($", {ColumnCycleResult.CYCLE_TIME}");
                            sb.Append($", {ColumnCycleResult.OPERATION_DATE}");
                            sb.Append($", {ColumnCycleResult.OPERATION_SHIFT}");
                            sb.Append($", {ColumnCycleResult.WORKER_NAME}");
                            sb.Append($", {ColumnCycleResult.VIDEO_FILE_NAME}");
                            sb.Append($", {ColumnCycleResult.ERROR_FLAG}");
                            sb.Append($", {ColumnCycleResult.GOOD_FLAG}");
                            sb.Append(") VALUES (");
                            sb.Append($" @{ColumnCycleResult.COMPOSITION_ID}");
                            sb.Append($", @{ColumnCycleResult.PRODUCT_TYPE_ID}");
                            sb.Append($", @{ColumnCycleResult.PROCESS_IDX}");
                            sb.Append($", @{ColumnCycleResult.START_TIME}");
                            sb.Append($", @{ColumnCycleResult.END_TIME}");
                            sb.Append($", @{ColumnCycleResult.CYCLE_TIME}");
                            sb.Append($", @{ColumnCycleResult.OPERATION_DATE}");
                            sb.Append($", @{ColumnCycleResult.OPERATION_SHIFT}");
                            sb.Append($", @{ColumnCycleResult.WORKER_NAME}");
                            sb.Append($", @{ColumnCycleResult.VIDEO_FILE_NAME}");
                            sb.Append($", @{ColumnCycleResult.ERROR_FLAG}");
                            sb.Append($", @{ColumnCycleResult.GOOD_FLAG}");
                            sb.Append(");");

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.COMPOSITION_ID}", SqlDbType.UniqueIdentifier)).Value = udpateItem.Field<Guid>(ColumnCycleResult.COMPOSITION_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.PRODUCT_TYPE_ID}", SqlDbType.UniqueIdentifier)).Value = udpateItem.Field<Guid>(ColumnCycleResult.PRODUCT_TYPE_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.PROCESS_IDX}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnCycleResult.PROCESS_IDX);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.START_TIME}", SqlDbType.DateTime)).Value = udpateItem.Field<DateTime>(ColumnCycleResult.START_TIME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.END_TIME}", SqlDbType.DateTime)).Value = udpateItem.Field<DateTime>(ColumnCycleResult.END_TIME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.CYCLE_TIME}", SqlDbType.Decimal)).Value = (decimal)udpateItem.Field<decimal>(ColumnCycleResult.CYCLE_TIME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.OPERATION_DATE}", SqlDbType.Date)).Value = udpateItem.Field<DateTime>(ColumnCycleResult.OPERATION_DATE);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.OPERATION_SHIFT}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnCycleResult.OPERATION_SHIFT);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.WORKER_NAME}", SqlDbType.NVarChar)).Value = udpateItem.Field<String>(ColumnCycleResult.WORKER_NAME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.VIDEO_FILE_NAME}", SqlDbType.NVarChar)).Value = "";
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.ERROR_FLAG}", SqlDbType.Bit)).Value = udpateItem.Field<int>(ColumnCycleResult.ERROR_FLAG);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.GOOD_FLAG}", SqlDbType.Bit)).Value = udpateItem.Field<int>(ColumnCycleResult.GOOD_FLAG);

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
        /// UPDATE処理の実行
        /// </summary>
        /// <param name="updateDataTable"></param>
        /// <returns></returns>
        public bool Update(DataTable updateDataTable)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Update:{ColumnCycleResult.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    foreach (DataRow udpateItem in updateDataTable.Rows)
                    {
                        if (udpateItem.RowState == DataRowState.Modified || udpateItem.RowState == DataRowState.Added)
                        {
                            sb.Clear();
                            sb.Append(String.Format("UPDATE {0} SET ", ColumnCycleResult.TABLE_NAME));
                            sb.Append($"{ColumnCycleResult.VIDEO_FILE_NAME} = @{ColumnCycleResult.VIDEO_FILE_NAME} ");
                            sb.Append($"WHERE {ColumnCycleResult.COMPOSITION_ID} = @{ColumnCycleResult.COMPOSITION_ID} ");
                            sb.Append($"AND {ColumnCycleResult.PRODUCT_TYPE_ID} = @{ColumnCycleResult.PRODUCT_TYPE_ID} ");
                            sb.Append($"AND {ColumnCycleResult.PROCESS_IDX} = @{ColumnCycleResult.PROCESS_IDX} ");
                            sb.Append($"AND {ColumnCycleResult.START_TIME} = @{ColumnCycleResult.START_TIME} ");
                            sb.Append($"AND {ColumnCycleResult.OPERATION_DATE} = @{ColumnCycleResult.OPERATION_DATE} ");
                            //sb.Append($"AND {ColumnCycleResult.OPERATION_SHIFT} = @{ColumnCycleResult.OPERATION_SHIFT} ");
                            sb.Append(";");

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.VIDEO_FILE_NAME}", SqlDbType.NVarChar)).Value = udpateItem.Field<string>(ColumnCycleResult.VIDEO_FILE_NAME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.COMPOSITION_ID}", SqlDbType.UniqueIdentifier)).Value = udpateItem.Field<Guid>(ColumnCycleResult.COMPOSITION_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.PRODUCT_TYPE_ID}", SqlDbType.UniqueIdentifier)).Value = udpateItem.Field<Guid>(ColumnCycleResult.PRODUCT_TYPE_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.PROCESS_IDX}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnCycleResult.PROCESS_IDX);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.START_TIME}", SqlDbType.DateTime)).Value = udpateItem.Field<DateTime>(ColumnCycleResult.START_TIME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.OPERATION_DATE}", SqlDbType.Date)).Value = udpateItem.Field<DateTime>(ColumnCycleResult.OPERATION_DATE);
                            //cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.OPERATION_SHIFT}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnCycleResult.OPERATION_SHIFT);

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

        public bool Update(DateTime dateTimeStart, DateTime dateTimeEnd, int processIdx)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Info($"Update:{ColumnCycleResult.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Clear();
                    sb.Append(String.Format("UPDATE {0} SET ", ColumnCycleResult.TABLE_NAME));
                    sb.Append($"{ColumnCycleResult.VIDEO_FILE_NAME} = @{ColumnCycleResult.VIDEO_FILE_NAME} ");
                    sb.Append($"Where {ColumnCycleResult.PROCESS_IDX} = @{ColumnCycleResult.PROCESS_IDX} ");
                    sb.Append($"AND {ColumnCycleResult.START_TIME} BETWEEN @START AND @END ");
                    //sb.Append($"AND {ColumnCycleResult.OPERATION_SHIFT} = @{ColumnCycleResult.OPERATION_SHIFT} ");
                    sb.Append(";");

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.VIDEO_FILE_NAME}", SqlDbType.NVarChar)).Value = "";
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.PROCESS_IDX}", SqlDbType.Int)).Value = processIdx;
                    cmd.Parameters.Add(new SqlParameter($"@START", SqlDbType.DateTime)).Value = dateTimeStart;
                    cmd.Parameters.Add(new SqlParameter($"@END", SqlDbType.DateTime)).Value = dateTimeEnd;

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
        /// UPSERT処理の実行
        /// </summary>
        /// <param name="updateDataTable"></param>
        /// <returns></returns>
        public bool Upsert(DataTable updateDataTable)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Upsert:{ColumnCycleResult.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    foreach (DataRow udpateItem in updateDataTable.Rows)
                    {
                        if (udpateItem.RowState == DataRowState.Modified || udpateItem.RowState == DataRowState.Added)
                        {
                            sb.Clear();
                            sb.Append(String.Format("UPDATE {0} SET ", ColumnCycleResult.TABLE_NAME));
                            sb.Append($"{ColumnCycleResult.VIDEO_FILE_NAME} = @{ColumnCycleResult.VIDEO_FILE_NAME} ");
                            sb.Append($"WHERE {ColumnCycleResult.COMPOSITION_ID} = @{ColumnCycleResult.COMPOSITION_ID} ");
                            sb.Append($"AND {ColumnCycleResult.PRODUCT_TYPE_ID} = @{ColumnCycleResult.PRODUCT_TYPE_ID} ");
                            sb.Append($"AND {ColumnCycleResult.PROCESS_IDX} = @{ColumnCycleResult.PROCESS_IDX} ");
                            sb.Append($"AND {ColumnCycleResult.START_TIME} = @{ColumnCycleResult.START_TIME} ");
                            sb.Append($"AND {ColumnCycleResult.OPERATION_DATE} = @{ColumnCycleResult.OPERATION_DATE} ");
                            //sb.Append($"AND {ColumnCycleResult.OPERATION_SHIFT} = @{ColumnCycleResult.OPERATION_SHIFT} ");
                            sb.Append(";");

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.VIDEO_FILE_NAME}", SqlDbType.NVarChar)).Value = udpateItem.Field<string>(ColumnCycleResult.VIDEO_FILE_NAME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.COMPOSITION_ID}", SqlDbType.UniqueIdentifier)).Value = udpateItem.Field<Guid>(ColumnCycleResult.COMPOSITION_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.PRODUCT_TYPE_ID}", SqlDbType.UniqueIdentifier)).Value = udpateItem.Field<Guid>(ColumnCycleResult.PRODUCT_TYPE_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.PROCESS_IDX}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnCycleResult.PROCESS_IDX);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.START_TIME}", SqlDbType.DateTime)).Value = udpateItem.Field<DateTime>(ColumnCycleResult.START_TIME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.OPERATION_DATE}", SqlDbType.Date)).Value = udpateItem.Field<DateTime>(ColumnCycleResult.OPERATION_DATE);
                            //cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.OPERATION_SHIFT}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnCycleResult.OPERATION_SHIFT);

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
                logger.Debug($"Delete:{ColumnCycleResult.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {

                    sb.Clear();
                    sb.Append(String.Format("DELETE {0} ", ColumnCycleResult.TABLE_NAME));
                    sb.Append(String.Format("WHERE {0} < @{0} ", ColumnCycleResult.OPERATION_DATE));
                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnCycleResult.OPERATION_DATE}", SqlDbType.Date)).Value = deleteDateTime;

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
