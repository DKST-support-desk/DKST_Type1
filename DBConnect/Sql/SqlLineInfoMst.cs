using DBConnect;
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
    /// LineInfo_Mstテーブルの列名定義
    /// </summary>
    public static class ColumnLineInfoMst
    {
        public const String TABLE_NAME = "LineInfo_Mst";                        // テーブル名
        public const String LINE_NAME = "LineName";                             // ライン名
        public const String OCCUPANCY_RATE = "OccupancyRate";                   // 稼働率
        public const String COMPOSITION_EFFICIENCY = "CompositionEfficiency";   // 編成効率
        public const String RESULT_SAVE_SPAN = "ResultsSaveSpan";                // 実績CT
        public const String MOVIE_SAVE_SPAN = "MovieSaveSpan";                  // 動画
    }

    public class SqlLineInfoMst : ISqlBase
    {
        #region <Field>
        /// <summary>
        /// ログインスタンス
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region <Property>
        public SqlDBConnector DbConnector { get; set; }
        private String ConnectString { get; set; }
        #endregion

        #region <Constructor>
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SqlLineInfoMst(String connectString)
        {
            this.ConnectString = connectString;
        }

        /// <summary>
        /// DB接続オープン
        /// </summary>
        /// <returns></returns>
        public Boolean OpenConnection()
        {
            DbConnector = new SqlDBConnector(ConnectString);
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
            DataTable dt = new DataTable(ColumnLineInfoMst.TABLE_NAME);
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Select:{ColumnLineInfoMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return null;
                }

                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    sb.Append("SELECT *");
                    sb.Append(" FROM LineInfo_Mst ");

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
                logger.Error(ex.Message + ", " + ex.StackTrace);
                return null;
            }
            finally
            {
                CloseConnection();
            }

            return dt;
        }

        /// <summary>
        /// Insert処理の実行
        /// </summary>
        /// <param name="insertDataTable"></param>
        /// <returns></returns>
        public bool Insert(DataTable insertDataTable)
        {
            Boolean ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Insert:{ColumnLineInfoMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }
                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    foreach (DataRow insertItem in insertDataTable.Rows)
                    {
                        if (insertItem.RowState == DataRowState.Modified || insertItem.RowState == DataRowState.Added)
                        {
                            sb.Clear();
                            sb.Append("  INSERT INTO LineInfo_Mst(");
                            sb.Append("   LineName");
                            sb.Append("   ,OccupancyRate");
                            sb.Append("   ,CompositionEfficiency");
                            sb.Append("   ,ResultsSaveSpan");
                            sb.Append("   ,MovieSaveSpan");
                            sb.Append("   ) VALUES (");
                            sb.Append("   @LineName");
                            sb.Append("   ,@OccupancyRate ");
                            sb.Append("   ,@CompositionEfficiency ");
                            sb.Append("   ,@ResultsSaveSpan ");
                            sb.Append("   ,@MovieSaveSpan ");
                            sb.Append("   )");
                            sb.Append(";");

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter("@LineName", SqlDbType.NVarChar)).Value = insertItem.Field<String>(ColumnLineInfoMst.LINE_NAME);
                            cmd.Parameters.Add(new SqlParameter("@OccupancyRate", SqlDbType.Decimal)).Value = insertItem.Field<Decimal>(ColumnLineInfoMst.OCCUPANCY_RATE);
                            cmd.Parameters.Add(new SqlParameter("@CompositionEfficiency", SqlDbType.Decimal)).Value = insertItem.Field<Decimal>(ColumnLineInfoMst.COMPOSITION_EFFICIENCY);
                            cmd.Parameters.Add(new SqlParameter("@ResultsSaveSpan", SqlDbType.Int)).Value = insertItem.Field<int>(ColumnLineInfoMst.RESULT_SAVE_SPAN);
                            cmd.Parameters.Add(new SqlParameter("@MovieSaveSpan", SqlDbType.Int)).Value = insertItem.Field<int>(ColumnLineInfoMst.MOVIE_SAVE_SPAN);

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
                logger.Error(ex.Message + ", " + ex.StackTrace);
                ret = false;
            }
            finally
            {
                CloseConnection();
            }
            return ret;
        }

        /// <summary>
        /// Update処理の実行
        /// </summary>
        /// <param name="updateDataTable"></param>
        /// <returns></returns>
        public bool Update(DataTable updateDataTable)
        {
            Boolean ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Update:{ColumnLineInfoMst.TABLE_NAME}");

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
                            sb.Append("  UPDATE LineInfo_Mst SET ");
                            sb.Append("   LineName = @LineName");
                            sb.Append("   ,OccupancyRate = @OccupancyRate");
                            sb.Append("   ,CompositionEfficiency = @CompositionEfficiency");
                            sb.Append("   ,ResultsSaveSpan = @ResultsSaveSpan");
                            sb.Append("   ,MovieSaveSpan = @MovieSaveSpan");
                            sb.Append(";");

                            cmd.CommandText = sb.ToString();

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter("@LineName", SqlDbType.NVarChar)).Value =udpateItem.Field<String>(ColumnLineInfoMst.LINE_NAME);
                            cmd.Parameters.Add(new SqlParameter("@OccupancyRate", SqlDbType.Decimal)).Value = udpateItem.Field<Decimal>(ColumnLineInfoMst.OCCUPANCY_RATE);
                            cmd.Parameters.Add(new SqlParameter("@CompositionEfficiency", SqlDbType.Decimal)).Value = udpateItem.Field<Decimal>(ColumnLineInfoMst.COMPOSITION_EFFICIENCY);
                            cmd.Parameters.Add(new SqlParameter("@ResultsSaveSpan", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnLineInfoMst.RESULT_SAVE_SPAN);
                            cmd.Parameters.Add(new SqlParameter("@MovieSaveSpan", SqlDbType.Int)).Value = udpateItem.Field<int>(ColumnLineInfoMst.MOVIE_SAVE_SPAN);

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
                logger.Error(ex.Message + ", " + ex.StackTrace);
                ret = false;
            }
            finally
            {
                CloseConnection();
            }
            return ret;
        }

        /// <summary>
        /// Delete処理の実行
        /// </summary>
        /// <param name="deleteDataTable"></param>
        /// <returns></returns>
        public bool Delete(DataTable deleteDataTable)
        {
            Boolean ret = false;
            StringBuilder sb = new StringBuilder();

            try
            {
                logger.Debug($"Delete:{ColumnLineInfoMst.TABLE_NAME}");

                if (OpenConnection() == false)
                {
                    logger.Error("DBに接続されていません。");
                    return false;
                }
                using (SqlCommand cmd = DbConnector.DbConnection.CreateCommand())
                {
                    foreach (DataRow deleteItem in deleteDataTable.Rows)
                    {
                        sb.Append("DELETE FROM LineInfo_Mst");
                        sb.Append(" WHERE LineName = @LineName ");

                        cmd.CommandText = sb.ToString();
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@LineName", SqlDbType.NVarChar)).Value = deleteItem.Field<String>(ColumnLineInfoMst.LINE_NAME);

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
                logger.Error(ex.Message + ", " + ex.StackTrace);
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
