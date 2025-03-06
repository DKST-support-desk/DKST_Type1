using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using DBConnect;
using DBConnect.SQL;
using log4net;

namespace PLOSMaintenance
{
	public partial class UCOperating_Shift_Pattern : UserControl
	{
        //********************************************
        //* メンバー変数
        //********************************************
        #region "メンバー変数"
        /// <summary>
        /// ログインスタンス
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<Button> PatternButtonList = new List<Button>();
        //private DataTable dtOperatingPatternTbl;

        /// <summary> 稼働パターンテーブルの参照 </summary>
        private DataTable mDtOperatingPatternTbl;
        /// <summary> 稼働パターンテーブルのSQL操作インスタンス </summary>
        private SqlOperatingPatternTbl mSqlOperatingPatternTbl;

        public event EventHandler<Events.Operating_Shift_PatternEventArgs> PatternRegist;

        public event EventHandler<Events.Operating_Shift_PatternEventArgs> PatternLoad;
        #endregion "メンバー変数"

        //********************************************
        //* コンストラクタ
        //********************************************
        #region "コンストラクタ"
        public UCOperating_Shift_Pattern()
        {
            InitializeComponent();

            mSqlOperatingPatternTbl = new SqlOperatingPatternTbl(Properties.Settings.Default.ConnectionString_New);

            //dtOperatingPatternTbl = QueryOperatingPatternTbl();
            mDtOperatingPatternTbl = mSqlOperatingPatternTbl.Select();

            for (int iRowCnt = 0; iRowCnt < 10; iRowCnt++)
            {
                for (int iClmCnt = 0; iClmCnt < 3; iClmCnt++)
                {
                    Button buttonWk = new Button();

                    int PatternId = iClmCnt + iRowCnt * 3;
                    DataRow dr = mDtOperatingPatternTbl.AsEnumerable().FirstOrDefault(x => x.Field<Int32>(ColumnOperatingPatternTbl.PATTERN_ID) == PatternId);

                    tableLayoutPanel3.Controls.Add(buttonWk, iClmCnt, iRowCnt);

                    buttonWk.BackColor = (String.IsNullOrEmpty(dr?.Field<String>(ColumnOperatingPatternTbl.PATTERN_NAME) ?? "") ? System.Drawing.Color.White : System.Drawing.Color.Cornsilk);
                    buttonWk.Dock = System.Windows.Forms.DockStyle.Fill;
                    buttonWk.Location = new System.Drawing.Point(3, 3);
                    buttonWk.Name = $"PatternButton{PatternId:00}";
                    buttonWk.Size = new System.Drawing.Size(109, 54);
                    buttonWk.TabIndex = 0;
                    buttonWk.Text = dr?.Field<String>("PatternName") ?? "";
                    buttonWk.UseVisualStyleBackColor = false;
                    buttonWk.Tag = PatternId;
                    buttonWk.Click += OnPatternButton_Clicked;
                    buttonWk.ContextMenuStrip = this.contextMenuStrip;
                }
            }
        }
        #endregion "コンストラクタ"

        //********************************************
        //* プロパティ
        //********************************************
        #region "プロパティ"
        #endregion "プロパティ"

        //********************************************
        //* イベント
        //********************************************
        #region "イベント"
        /// <summary>
        /// 勤怠パターンボタン押下イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPatternButton_Clicked(object sender, EventArgs e)
        {
            logger.Info("稼働登録　勤怠パターンボタン押下");

            int PatternId;

            if (sender is Button patternButton)
            {
                if (!String.IsNullOrWhiteSpace(patternButton.Text) && int.TryParse(patternButton.Tag.ToString(), out PatternId))
                {
                    NotifyPatternUse(new PLOSMaintenance.Events.Operating_Shift_PatternEventArgs(PatternId, true));
                }
            }
        }
        #endregion "イベント"

        //********************************************
        //* パブリックメソッド
        //********************************************
        #region "パブリックメソッド"
        #endregion "パブリックメソッド"

        //********************************************
        //* プライベートメソッド
        //********************************************
        #region "プライベートメソッド"
        //private DataTable QueryOperatingPatternTbl()
        //{
        //    try
        //    {
        //        DataTable dt = new DataTable("Operating_Pattern_Tbl");

        //        SqlDBConnector dbConnector = new SqlDBConnector(Properties.Settings.Default.ConnectionString_New);
        //        if (dbConnector != null)
        //        {
        //            dbConnector.Create();
        //            dbConnector.OpenDatabase();

        //            using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
        //            {
        //                StringBuilder sb = new StringBuilder();
        //                sb.Append("SELECT * ");
        //                sb.Append(" FROM Operating_Pattern_Tbl ");
        //                sb.Append("  Order by PatternId  ");

        //                cmd.CommandText = sb.ToString();

        //                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
        //                {
        //                    adapter.Fill(dt);
        //                }
        //            }

        //            dbConnector.CloseDatabase();
        //            dbConnector.Dispose();
        //        }

        //        return dt;
        //    }
        //    catch (System.Data.SqlClient.SqlException ex)
        //    {
        //        throw ex;//new Exceptions.ImportPLCLogException(Exceptions.ImportPLCLogException.ExceptionType.Others, $"SQLコマンドの発行に失敗しました。SQL = {CommandText}/ {ex.Message}", ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;// Exceptions.ImportPLCLogException(Exceptions.ImportPLCLogException.ExceptionType.Others, $"例外メッセージ(SECTION)：{ex.Message}", ex);
        //    }
        //}

        private void UpdateOperatingPatternTbl(int PatternId, String PatternName)
        {
            try
            {
                SqlDBConnector dbConnector = new SqlDBConnector(Properties.Settings.Default.ConnectionString_New);
                if (dbConnector != null)
                {
                    dbConnector.Create();
                    dbConnector.OpenDatabase();

                    using (SqlCommand cmd = dbConnector.DbConnection.CreateCommand())
                    {
                        StringBuilder sb = new StringBuilder();

                        sb.Clear();
                        sb.Append("MERGE INTO Operating_Pattern_Tbl AS T1  ");
                        sb.Append("  USING ");
                        sb.Append("    (SELECT ");
                        sb.Append("      @PatternId AS PatternId ");
                        sb.Append("    ) AS T2");
                        sb.Append("  ON (");
                        sb.Append("   T1.PatternId = T2.PatternId ");
                        sb.Append("  )");
                        sb.Append(" WHEN MATCHED THEN ");
                        sb.Append("  UPDATE SET ");
                        sb.Append("   PatternName = @PatternName");
                        sb.Append(" WHEN NOT MATCHED THEN ");
                        sb.Append("  INSERT (");
                        sb.Append("   PatternId");
                        sb.Append("   ,PatternName");
                        sb.Append("   ) VALUES (");
                        sb.Append("   @PatternId ");
                        sb.Append("   ,@PatternName ");
                        sb.Append("   )");
                        sb.Append(";");

                        cmd.CommandText = sb.ToString();
                        cmd.Parameters.Add(new SqlParameter("@PatternId", SqlDbType.Int)).Value = PatternId;
                        cmd.Parameters.Add(new SqlParameter("@PatternName", SqlDbType.NVarChar)).Value = PatternName;

                        cmd.ExecuteNonQuery();
                    }

                    dbConnector.CloseDatabase();
                    dbConnector.Dispose();
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;//new Exceptions.ImportPLCLogException(Exceptions.ImportPLCLogException.ExceptionType.Others, $"SQLコマンドの発行に失敗しました。SQL = {CommandText}/ {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw ex;// Exceptions.ImportPLCLogException(Exceptions.ImportPLCLogException.ExceptionType.Others, $"例外メッセージ(SECTION)：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 勤怠パターンをクリアします
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClearOperatingPatternClicked(object sender, EventArgs e)
        {
            logger.Info("稼働登録　登録パターンをクリアボタン押下");

            int patternId;

            if (((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl is Button patternButton)
            {
                if (int.TryParse(patternButton.Tag.ToString(), out patternId))
                {
                    mSqlOperatingPatternTbl.Delete(patternId);

                    patternButton.BackColor = System.Drawing.Color.White;
                    patternButton.Text = "";

                    mDtOperatingPatternTbl = mSqlOperatingPatternTbl.Select();
                    NotifyPatternRegist(new PLOSMaintenance.Events.Operating_Shift_PatternEventArgs(patternId, false));
                }
            }
        }

        /// <summary>
        /// 勤怠パターンを登録します
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAddOperatingPatternClicked(object sender, EventArgs e)
        {
            logger.Info("現状画面を登録ボタン押下");

            FrmOperatingPattern frm = new FrmOperatingPattern();
            int patternId;

            if (((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl is Button patternButton)
            {
                if (int.TryParse(patternButton.Tag.ToString(), out patternId))
                {
                    DataRow drt = mDtOperatingPatternTbl.AsEnumerable().FirstOrDefault(x => x.Field<Int32>(ColumnOperatingPatternTbl.PATTERN_ID) == patternId);

                    DataTable dt = mDtOperatingPatternTbl.Clone();
                    if (drt != null)
                    {
                        DataRow dr = dt.NewRow();
                        dr.ItemArray = drt.ItemArray;
                        dt.Rows.Add(dr);
                        frm.PatternName = drt.Field<String>(ColumnOperatingPatternTbl.PATTERN_NAME) ;
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        dr[ColumnOperatingPatternTbl.PATTERN_ID] = patternId;
                        dt.Rows.Add(dr);

                        frm.PatternName = "";
                    }

                    if (frm.ShowDialog(this.Parent) == DialogResult.OK)
                    {
                        dt.Rows[0][ColumnOperatingPatternTbl.PATTERN_NAME] = frm.PatternName;
                        mSqlOperatingPatternTbl.Upsert(dt);

                        patternButton.BackColor = (String.IsNullOrWhiteSpace(frm.PatternName) ? System.Drawing.Color.White : System.Drawing.Color.Cornsilk);
                        patternButton.Text = frm.PatternName;

                        mDtOperatingPatternTbl = mSqlOperatingPatternTbl.Select();
                        NotifyPatternRegist(new PLOSMaintenance.Events.Operating_Shift_PatternEventArgs(patternId, true));
                    }
                }
            }
        }

        /// <summary>
        /// NotifyPatternRegist
        /// </summary>
        private void NotifyPatternRegist(Events.Operating_Shift_PatternEventArgs args)
        {
            EventHandler<Events.Operating_Shift_PatternEventArgs> handler = PatternRegist;
            if (handler != null)
            {
                foreach (EventHandler<Events.Operating_Shift_PatternEventArgs> evhd in handler.GetInvocationList())
                {
                    evhd(this, args);
                }
            }
        }

        /// <summary>
        /// NotifyPatternUse
        /// </summary>
        private void NotifyPatternUse(Events.Operating_Shift_PatternEventArgs args)
        {
            EventHandler<Events.Operating_Shift_PatternEventArgs> handler = PatternLoad;
            if (handler != null)
            {
                foreach (EventHandler<Events.Operating_Shift_PatternEventArgs> evhd in handler.GetInvocationList())
                {
                    evhd(this, args);
                }
            }
        }        
        #endregion "プライベートメソッド
    }
}
