using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using log4net;

namespace DBConnect.SQL
{
    /// <summary>
    /// OperatingShiftTblテーブルの列名の定義
    /// </summary>
    public class ColumnOperatingShiftPatternTbl
    {
        /// <summary>
        /// テーブル名
        /// </summary>
        public const String TABLE_NAME = "Operating_Shift_Pattern_Tbl";

        /// <summary>
        /// パターンID
        /// </summary>
        public const String PATTERN_ID = "PatternId";

        /// <summary>
        /// 稼働シフト
        /// </summary>
        public const String OPERATION_SHIFT = "OperationShift";

        /// <summary>
        /// 生産数
        /// </summary>
        public const String PRODUCTION_QUANTITY = "ProductionQuantity";

        /// <summary>
        /// 開始時刻
        /// </summary>
        public const String START_TIME = "StartTime";
        /// <summary>
        /// 計画稼働開始時間翌日フラグ
        /// </summary>
        public const String START_TIME_NEXT_DAY_FLAG = "StartTimeNextDayFlag";

        /// <summary>
        /// 終了時刻
        /// </summary>
        public const String END_TIME = "EndTime";
        /// <summary>
        /// 計画稼働開始時間翌日フラグ
        /// </summary>
        public const String END_TIME_NEXT_DAY_FLAG = "EndTimeNextDayFlag";

        /// <summary>
        /// 稼働時間(秒)
        /// </summary>
        public const String OPERATION_SECOND = "OperationSecond";
    }

    /// <summary>
    /// OperatingShiftTblテーブル
    /// </summary>
    public class SqlOperatingShiftPatternTbl : ISqlBase
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
        public SqlOperatingShiftPatternTbl(string connectString)
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
            DataTable dt = new DataTable(ColumnOperatingShiftPatternTbl.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Info($"Select:{ColumnOperatingShiftPatternTbl.TABLE_NAME}");
                
                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT * ");
                    sb.Append(String.Format("FROM {0} ", ColumnOperatingShiftPatternTbl.TABLE_NAME));
                    sb.Append(String.Format("Order by {0}, {1};", ColumnOperatingShiftPatternTbl.PATTERN_ID, ColumnOperatingShiftPatternTbl.OPERATION_SHIFT));

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
        /// <param name="patternId">パターンID</param>
        /// <param name="OperationShift">稼働シフト</param>
        /// <returns></returns>
        public DataTable Select(int patternId, int OperationShift = 0)
        {
            DataTable dt = new DataTable(ColumnOperatingShiftPatternTbl.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Info($"ConditionalSelect:{ColumnOperatingShiftPatternTbl.TABLE_NAME}");
                
                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT * ");
                    sb.Append(String.Format("FROM {0} ", ColumnOperatingShiftPatternTbl.TABLE_NAME));
                    sb.Append("WHERE 1 = 1 ");
                    sb.Append(String.Format("And {0} = @{0} ", ColumnOperatingShiftPatternTbl.PATTERN_ID));
                    if (OperationShift != 0)
                    {
                        sb.Append(String.Format("And {0} = @{0} ", ColumnOperatingShiftPatternTbl.OPERATION_SHIFT));
                    }
                    sb.Append(String.Format("Order by {0}, {1};", ColumnOperatingShiftPatternTbl.PATTERN_ID, ColumnOperatingShiftPatternTbl.OPERATION_SHIFT));

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(String.Format("@{0}", ColumnOperatingShiftPatternTbl.PATTERN_ID), SqlDbType.Int).Value = patternId;
                    if (OperationShift != 0)
                    {
                        cmd.Parameters.Add(String.Format("@{0}", ColumnOperatingShiftPatternTbl.OPERATION_SHIFT), SqlDbType.Int).Value = OperationShift;
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
        /// INSERT処理の実行
        /// </summary>
        /// <param name="insertDataTable"></param>
        /// <returns></returns>
        public bool Insert(DataTable insertDataTable)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// UPDATE処理の実行
        /// </summary>
        /// <param name="insertDataTable"></param>
        /// <returns></returns>
        public bool Update(DataTable updateDataTable)
        {
            throw new NotImplementedException();
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
                logger.Info($"Upsert:{ColumnOperatingShiftPatternTbl.TABLE_NAME}");

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
                            sb.Append(string.Format("MERGE INTO {0} AS T1 ", ColumnOperatingShiftPatternTbl.TABLE_NAME));
                            sb.Append(string.Format("USING (SELECT @{0} AS {0}, @{1} AS {1} )AS T2 ", ColumnOperatingPatternTbl.PATTERN_ID, ColumnOperatingShiftPatternTbl.OPERATION_SHIFT));
                            sb.Append(string.Format("ON (T1.{0} = T2.{0} AND T1.{1} = T2.{1}) ", ColumnOperatingShiftPatternTbl.PATTERN_ID, ColumnOperatingShiftPatternTbl.OPERATION_SHIFT));
                            sb.Append(string.Format("WHEN MATCHED THEN UPDATE SET "));
                            //sb.Append(string.Format("{0} = @{0}, ", ColumnOperatingShiftPatternTbl.OPERATION_SHIFT));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnOperatingShiftPatternTbl.PRODUCTION_QUANTITY));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnOperatingShiftPatternTbl.START_TIME));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnOperatingShiftPatternTbl.START_TIME_NEXT_DAY_FLAG));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnOperatingShiftPatternTbl.END_TIME));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnOperatingShiftPatternTbl.END_TIME_NEXT_DAY_FLAG));
                            sb.Append(string.Format("{0} = @{0} ", ColumnOperatingShiftPatternTbl.OPERATION_SECOND));
                            sb.Append(string.Format("WHEN NOT MATCHED THEN INSERT ({0},{1},{2},{3},{4},{5},{6},{7}) VALUES (@{0},@{1},@{2},@{3},@{4},@{5},@{6},@{7});"
                                , ColumnOperatingShiftPatternTbl.PATTERN_ID
                                , ColumnOperatingShiftPatternTbl.OPERATION_SHIFT
                                , ColumnOperatingShiftPatternTbl.PRODUCTION_QUANTITY
                                , ColumnOperatingShiftPatternTbl.START_TIME
                                , ColumnOperatingShiftPatternTbl.START_TIME_NEXT_DAY_FLAG
                                , ColumnOperatingShiftPatternTbl.END_TIME
                                , ColumnOperatingShiftPatternTbl.END_TIME_NEXT_DAY_FLAG
                                , ColumnOperatingShiftPatternTbl.OPERATION_SECOND
                                ));
                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftPatternTbl.PATTERN_ID}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnOperatingShiftPatternTbl.PATTERN_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftPatternTbl.OPERATION_SHIFT}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnOperatingShiftPatternTbl.OPERATION_SHIFT);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftPatternTbl.PRODUCTION_QUANTITY}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnOperatingShiftPatternTbl.PRODUCTION_QUANTITY);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftPatternTbl.START_TIME}", SqlDbType.Time)).Value = udpateItem.Field<TimeSpan>(ColumnOperatingShiftPatternTbl.START_TIME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftPatternTbl.START_TIME_NEXT_DAY_FLAG}", SqlDbType.Bit)).Value = udpateItem.Field<Boolean>(ColumnOperatingShiftPatternTbl.START_TIME_NEXT_DAY_FLAG);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftPatternTbl.END_TIME}", SqlDbType.Time)).Value = udpateItem.Field<TimeSpan>(ColumnOperatingShiftPatternTbl.END_TIME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftPatternTbl.END_TIME_NEXT_DAY_FLAG}", SqlDbType.Bit)).Value = udpateItem.Field<Boolean>(ColumnOperatingShiftPatternTbl.END_TIME_NEXT_DAY_FLAG);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftPatternTbl.OPERATION_SECOND}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnOperatingShiftPatternTbl.OPERATION_SECOND);

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
        /// DELETE処理の実行
        /// </summary>
        /// <param name="deleteDataTable"></param>
        /// <returns></returns>
        public bool Delete(DataTable deleteDataTable)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// DELETE処理の実行
        /// </summary>
        /// <param name="deleteDataTable"></param>
        /// <returns></returns>
        public bool Delete(int patternId, int shiftIdx)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Info($"Delete:{ColumnOperatingShiftPatternTbl.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {

                    sb.Clear();
                    sb.Append(String.Format("DELETE {0} ", ColumnOperatingShiftPatternTbl.TABLE_NAME));
                    sb.Append(String.Format("WHERE {0} = @{0} ", ColumnOperatingShiftPatternTbl.PATTERN_ID));
                    sb.Append(String.Format("AND {0} = @{0};", ColumnOperatingShiftPatternTbl.OPERATION_SHIFT));
                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftPatternTbl.PATTERN_ID}", SqlDbType.Int)).Value = patternId;
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftPatternTbl.OPERATION_SHIFT}", SqlDbType.Int)).Value = shiftIdx;

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
