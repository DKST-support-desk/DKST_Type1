using DBConnect;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnect.SQL
{
    interface ISqlBase
    {
        /// <summary>
        /// SQL接続
        /// </summary>
        SqlDBConnector DbConnector { get; set; }

        /// <summary>
        /// SQLのオープン処理
        /// </summary>
        /// <returns></returns>
        Boolean OpenConnection();

        /// <summary>
        /// SQLのクローズ処理
        /// </summary>
        void CloseConnection();

        /// <summary>
        /// SELECT文のインターフェースメソッド
        /// </summary>
        /// <returns>取得されたデータのDataTable</returns>
        DataTable Select();
        
        /// <summary>
        /// INSERT文のインターフェースメソッド
        /// </summary>
        /// <param name="insertDataTable">INSERTするデータのDataTable</param>
        /// <returns>発行したSQLの成否</returns>
        Boolean Insert(DataTable insertDataTable);

        /// <summary>
        /// UPDATE文のインターフェースメソッド
        /// </summary>
        /// <param name="updateDataTable">UPDATEするデータのDataTable</param>
        /// <returns>発行したSQLの成否</returns>
        Boolean Update(DataTable updateDataTable);

        /// <summary>
        /// DELETE文のインターフェースメソッド
        /// </summary>
        /// <param name="deleteDataTable">DELETEするデータのDataTable</param>
        /// <returns>発行したSQLの成否</returns>
        Boolean Delete(DataTable deleteDataTable);

    }
}
