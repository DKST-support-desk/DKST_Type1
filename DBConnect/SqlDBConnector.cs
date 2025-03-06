using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using log4net;

namespace DBConnect
{
    /// <summary>
    /// SqlDBConnector
    /// </summary>
    public class SqlDBConnector : IDisposable
    {
        #region Private Fields


        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Connection String
        /// </summary>
        private string mConnectionString;

        /// <summary>
        /// SqlConnection
        /// </summary>
        private SqlConnection mDbConnection = null;

        /// <summary>
        /// オープン時エラーメッセージ
        /// </summary>
        public const String DB_CONNECTION_ERROR = "DB接続に失敗しました。";

        /// <summary>
        /// クエリ実行時DB接続エラーメッセージ
        /// </summary>
        public const String DB_NOT_CONNECTION_ERROR = "DBに接続されていません。";

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Constructors
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlDBConnector(string connectionString)
        {
            mConnectionString = connectionString;
        }
        #endregion

        #region Public Property

        /// <summary>
        /// Connection String
        /// </summary>
        public String ConnectionString
        {
            set { mConnectionString = value; }
            get { return mConnectionString; }
        }

        /// <summary>
        /// Database Connection
        /// </summary>
        public SqlConnection DbConnection
        {
            set { mDbConnection = value; }
            get { return mDbConnection; }
        }

        /// <summary>
        /// Database ConnectionState
        /// </summary>
        public ConnectionState DBConnectionState
        {
            get
            {
                if (mDbConnection != null) return mDbConnection.State;
                return ConnectionState.Closed;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// DB接続
        /// </summary>
        public void OpenDatabase()
        {

            CloseDatabase();

            if (mDbConnection != null)
            {
                //logger.Info("DB接続を開始します");
                mDbConnection.Open();
            }
        }

        /// <summary>
        /// DB切断
        /// </summary>
        public void CloseDatabase()
        {
            if (mDbConnection != null)
            {
                try
                {
                    if (mDbConnection.State != ConnectionState.Closed)
                    {
                        mDbConnection.Close();
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message + "," + ex.StackTrace);
                }
            }
        }

        public static Boolean CreateParamsText(SqlParameterCollection parameterCollection, out String ret)
        {
            ret = "";
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Clear();
                if (parameterCollection.Count > 0)
                {
                    sb.Append("\n Params: ");
                    sb.Append(String.Format("{0} = {1}", parameterCollection[0], parameterCollection[0].Value));
                    for (int i = 1; i < parameterCollection.Count; i++)
                    {
                        sb.Append(String.Format(", {0} = {1}", parameterCollection[i], parameterCollection[i].Value));
                    }
                }
                ret = sb.ToString();
                return true; ;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + "," + ex.StackTrace);
                ret = "";
                return false;
            }
        }

        #endregion

        #region Public Override Methods

        /// <summary>
        /// Create
        /// </summary>
        /// <returns></returns>
        public SqlConnection Create()
        {
            if (DbConnection == null)
            {
                DbConnection = new SqlConnection(this.ConnectionString);
            }

            return DbConnection;
        }

        public SqlDataAdapter CreateAdapter()
        {
            return new SqlDataAdapter();
        }

        #endregion

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            if (DbConnection != null)
            {
                CloseDatabase();
                DbConnection.Dispose();
            }
        }
        #endregion
    }
}
