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
    public class ColumnOperatingShiftExclusionPatternTbl
    {
        public const String TABLE_NAME = "Operating_Shift_Exclusion_Pattern_Tbl";
        public const String PATTERN_ID = "PatternId";
        public const String OPERATION_SHIFT = "OperationShift";
        public const String EXCLUSION_IDX = "ExclusionIdx";
        public const String EXCLUSION_START_TIME = "ExclusionStartTime";
        public const String EXCLUSION_START_TIME_FLAG = "ExclusionStartTimeFlag";
        public const String EXCLUSION_END_TIME = "ExclusionEndTime";
        public const String EXCLUSION_END_TIME_FLAG = "ExclusionEndTimeFlag";
        public const String EXCLUSION_CHECK = "ExclusionCheck";
        public const String EXCLUSION_ID = "ExclusionId";
        public const String EXCLUSION_REMARK = "ExclusionRemark";
    }

    public class SqlOperatingShiftExclusionPatternTbl : ISqlBase
    {
        #region <Field>
        private String mConnectString;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region <Property>
        public SqlDBConnector DbConnector { get; set; }

        #endregion

        #region <Constructor>
        public SqlOperatingShiftExclusionPatternTbl(String connectString)
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
        /// SELECT処理の実行
        /// </summary>
        /// <returns></returns>
        public DataTable Select()
        {
            DataTable dt = new DataTable(ColumnOperatingShiftExclusionPatternTbl.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Select:{ColumnOperatingShiftExclusionPatternTbl.TABLE_NAME}");
                
                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT * ");
                    sb.Append(String.Format("FROM {0} ", ColumnOperatingShiftExclusionPatternTbl.TABLE_NAME));
                    sb.Append(String.Format("Order by {0}, {1};", ColumnOperatingShiftExclusionPatternTbl.PATTERN_ID, ColumnOperatingShiftExclusionPatternTbl.OPERATION_SHIFT));

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
        /// <param name="patternId"></param>
        /// <param name="operationShift"></param>
        /// <returns></returns>
        public DataTable Select(int patternId, int operationShift)
        {
            DataTable dt = new DataTable(ColumnOperatingShiftExclusionPatternTbl.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"ConditionalSelect:{ColumnOperatingShiftExclusionPatternTbl.TABLE_NAME}");
                
                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT * ");
                    sb.Append(String.Format("FROM {0} ", ColumnOperatingShiftExclusionPatternTbl.TABLE_NAME));
                    sb.Append("WHERE 1 = 1 ");
                    sb.Append(String.Format("And {0} = @{0} ", ColumnOperatingShiftExclusionPatternTbl.PATTERN_ID));
                    sb.Append(String.Format("And {0} = @{0} ", ColumnOperatingShiftExclusionPatternTbl.OPERATION_SHIFT));
                    sb.Append(String.Format("Order by {0}, {1};", ColumnOperatingShiftExclusionPatternTbl.PATTERN_ID, ColumnOperatingShiftExclusionPatternTbl.OPERATION_SHIFT));

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(String.Format("@{0}", ColumnOperatingShiftExclusionPatternTbl.PATTERN_ID), SqlDbType.Int).Value = patternId;
                    cmd.Parameters.Add(String.Format("@{0}", ColumnOperatingShiftExclusionPatternTbl.OPERATION_SHIFT), SqlDbType.Int).Value = operationShift;

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
                logger.Debug($"Upsert:{ColumnOperatingShiftExclusionPatternTbl.TABLE_NAME}");

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
                            sb.Append(string.Format("MERGE INTO {0} AS T1 ", ColumnOperatingShiftExclusionPatternTbl.TABLE_NAME));
                            sb.Append(string.Format("USING (SELECT @{0} AS {0}, @{1} AS {1}, @{2} AS {2} )AS T2 ",
                                ColumnOperatingShiftExclusionPatternTbl.PATTERN_ID, 
                                ColumnOperatingShiftExclusionPatternTbl.OPERATION_SHIFT,
                                ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_IDX));
                            sb.Append(string.Format("ON (T1.{0} = T2.{0} AND T1.{1} = T2.{1} AND T1.{2} = T2.{2}) ", 
                                ColumnOperatingShiftExclusionPatternTbl.PATTERN_ID, 
                                ColumnOperatingShiftExclusionPatternTbl.OPERATION_SHIFT,
                                ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_IDX));
                            sb.Append(string.Format("WHEN MATCHED THEN UPDATE SET "));
                            //sb.Append(string.Format("{0} = @{0}, ", ColumnOperatingShiftExclusionPatternTbl.OPERATION_SHIFT));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_START_TIME));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_START_TIME_FLAG));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_END_TIME));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_END_TIME_FLAG));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_CHECK));
                            sb.Append(string.Format("{0} = @{0}, ", ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_ID));
                            sb.Append(string.Format("{0} = @{0} ", ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_REMARK));
                            sb.Append(string.Format("WHEN NOT MATCHED THEN INSERT ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9}) VALUES (@{0},@{1},@{2},@{3},@{4},@{5},@{6},@{7},@{8},@{9});"
                                , ColumnOperatingShiftExclusionPatternTbl.PATTERN_ID
                                , ColumnOperatingShiftExclusionPatternTbl.OPERATION_SHIFT
                                , ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_IDX
                                , ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_START_TIME
                                , ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_START_TIME_FLAG
                                , ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_END_TIME
                                , ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_END_TIME_FLAG
                                , ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_CHECK
                                , ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_ID
                                , ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_REMARK
                                ));
                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftExclusionPatternTbl.PATTERN_ID}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnOperatingShiftExclusionPatternTbl.PATTERN_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftExclusionPatternTbl.OPERATION_SHIFT}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnOperatingShiftExclusionPatternTbl.OPERATION_SHIFT);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_IDX}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_IDX);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_START_TIME}", SqlDbType.Time)).Value = udpateItem.Field<TimeSpan>(ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_START_TIME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_START_TIME_FLAG}", SqlDbType.Bit)).Value = udpateItem.Field<Boolean>(ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_START_TIME_FLAG);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_END_TIME}", SqlDbType.Time)).Value = udpateItem.Field<TimeSpan>(ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_END_TIME);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_END_TIME_FLAG}", SqlDbType.Bit)).Value = udpateItem.Field<Boolean>(ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_END_TIME_FLAG);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_CHECK}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_CHECK);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_ID}", SqlDbType.UniqueIdentifier)).Value = udpateItem.Field<Guid>(ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_REMARK}", SqlDbType.NVarChar)).Value = udpateItem.Field<String>(ColumnOperatingShiftExclusionPatternTbl.EXCLUSION_REMARK);

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
        /// <param name="patternId"></param>
        /// <param name="shiftIdx"></param>
        /// <returns></returns>
        public bool Delete(int patternId, int shiftIdx)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Delete:{ColumnOperatingShiftExclusionPatternTbl.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {

                    sb.Clear();
                    sb.Append(String.Format("DELETE {0} ", ColumnOperatingShiftExclusionPatternTbl.TABLE_NAME));
                    sb.Append(String.Format("WHERE {0} = @{0} ", ColumnOperatingShiftExclusionPatternTbl.PATTERN_ID));
                    sb.Append(String.Format("AND {0} = @{0};", ColumnOperatingShiftExclusionPatternTbl.OPERATION_SHIFT));
                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftExclusionPatternTbl.PATTERN_ID}", SqlDbType.Int)).Value = patternId;
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingShiftExclusionPatternTbl.OPERATION_SHIFT}", SqlDbType.Int)).Value = shiftIdx;

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
    }
}
