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
    public class ColumnOperatingPatternTbl
    {

        public const String TABLE_NAME = "Operating_Pattern_Tbl";
        public const String PATTERN_ID = "PatternId";
        public const String PATTERN_NAME = "PatternName";
    }

    public class SqlOperatingPatternTbl : ISqlBase
    {
        #region <Field>
        private String mConnectString;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region <Property>
        public SqlDBConnector DbConnector { get; set; }

        #endregion

        #region <Constructor>
        public SqlOperatingPatternTbl(String connectString)
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
            DataTable dt = new DataTable(ColumnOperatingPatternTbl.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Select:{ColumnOperatingPatternTbl.TABLE_NAME}");
                
                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append(String.Format("SELECT * FROM {0};", ColumnOperatingPatternTbl.TABLE_NAME));

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
                logger.Debug($"Upsert:{ColumnOperatingPatternTbl.TABLE_NAME}");

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
                            sb.Append(string.Format("MERGE INTO {0} AS T1 ", ColumnOperatingPatternTbl.TABLE_NAME));
                            sb.Append(string.Format("USING (SELECT @{0} AS {0}) AS T2 ", ColumnOperatingPatternTbl.PATTERN_ID));
                            sb.Append(string.Format("ON (T1.{0} = T2.{0}) ", ColumnOperatingPatternTbl.PATTERN_ID));
                            sb.Append(string.Format("WHEN MATCHED THEN UPDATE SET {0} = @{0} ", ColumnOperatingPatternTbl.PATTERN_NAME));
                            sb.Append(string.Format("WHEN NOT MATCHED THEN INSERT ({0},{1}) ", ColumnOperatingPatternTbl.PATTERN_ID, ColumnOperatingPatternTbl.PATTERN_NAME));
                            sb.Append(string.Format("VALUES (@{0}, @{1});", ColumnOperatingPatternTbl.PATTERN_ID, ColumnOperatingPatternTbl.PATTERN_NAME));

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingPatternTbl.PATTERN_ID}", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnOperatingPatternTbl.PATTERN_ID);
                            cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingPatternTbl.PATTERN_NAME}", SqlDbType.NVarChar)).Value = udpateItem.Field<String>(ColumnOperatingPatternTbl.PATTERN_NAME);

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
        /// <returns></returns>
        public bool Delete(int patternId)
        {
            bool ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Delete:{ColumnOperatingShiftPatternTbl.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Clear();
                    sb.Append(string.Format("DELETE {0} ", ColumnOperatingPatternTbl.TABLE_NAME));
                    sb.Append(string.Format("WHERE {0} = @{0};", ColumnOperatingPatternTbl.PATTERN_ID));

                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter($"@{ColumnOperatingPatternTbl.PATTERN_ID}", SqlDbType.Int)).Value = patternId;

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
