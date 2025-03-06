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
using PLOS.Gui.Core.CustumContol;

namespace PLOSMaintenance
{
    public partial class UCStandardVal_Table_Item : UserControl
    {
        //********************************************
        //* メンバー変数
        //********************************************
        #region "メンバー変数"
        private class RowStdValType
        {
            /// <summary>
            /// CT標準値の設定項目
            /// </summary>
            /// <param name="rowPos">CT標準値の設定行位置</param>
            /// <param name="stdValType"></param>
            /// <param name="rowName">CT標準値の設定行名称</param>
            /// <param name="readOnly">読み取り専用</param>
            public RowStdValType(int rowPos, int stdValType, string rowName, Boolean readOnly = false)
            {
                RowPos = rowPos;
                StdValType = stdValType;
                RowName = rowName;
                ReadOnly = readOnly;
            }

            /// <summary>
            /// CT標準値の設定行位置
            /// </summary>
            public int RowPos { get; set; }

            /// <summary>
            /// タイプ
            /// </summary>
            public int StdValType { get; set; }

            /// <summary>
            /// CT標準値の設定行名称
            /// </summary>
            public string RowName { get; set; }

            /// <summary>
            /// 読み取り専用
            /// </summary>
            public Boolean ReadOnly { get; set; }
        }

        /// <summary>
        /// CT標準値の設定項目
        /// </summary>
        public class StdValItem
        {
            /// <summary>
            /// 工程インデックス
            /// </summary>
            public int ProcessIdx { get; set; }

            /// <summary>
            /// タイプ
            /// </summary>
            public int StdValType { get; set; }

            /// <summary>
            /// CT標準値の設定値
            /// </summary>
            public decimal StdVal { get; set; }
        }

        /// <summary>
        /// 機器の設定行位置
        /// </summary>
        private enum DeviceSettingRowPos
        {
            IsEnable = 0,
            GoodProductCount,
            TriggerType,
            IntervalFilter,
            Sensor_A,
            ChatteringFilter_A,
            Sensor_B,
            ChatteringFilter_B,
            TriggerImage,
            DODevice = 10,
            CameraStat = 12,
        }

        /// <summary>
        /// カメラの設定数
        /// </summary>
        private const int CameraSettingCount = 7;

        /// <summary>
        /// カメラ名称 なし
        /// </summary>
        private const string CameraNameNothing = "(Nothing)";

        /// <summary>
        /// センサ機器マスタテーブルアクセス
        /// </summary>
        private SqlSensorDeviceMst mSqlSensorDeviceMst;

        /// <summary>
        /// センサ機器マスタテーブルデータ
        /// </summary>
        private DataTable mDtSensorDeviceMst;

        /// <summary>
        /// DO(ブザー)機器マスタテーブルアクセス
        /// </summary>
        private SqlDoDeviceMst mSqlDoDeviceMst;

        /// <summary>
        ///  DO(ブザー)機器マスタテーブルデータ
        /// </summary>
        private DataTable mDtDoDeviceMst;

        /// <summary>
        /// カメラ機器マスタテーブルアクセス
        /// </summary>
        private SqlCameraDeviceMst mSqlCameraDeviceMst;

        /// <summary>
        /// カメラ機器マスタテーブルデータ
        /// </summary>
        private DataTable mDtCameraDeviceMst;

        /// <summary>
        /// 標準値品番工程別マスタテーブルアクセス
        /// </summary>
        private SqlStandardValProductTypeProcessMst mSqlStandardValProductTypeProcessMst;

        /// <summary>
        /// 標準値品番工程別マスタテーブルデータ
        /// </summary>
        private DataTable mDtStandardValProductTypeProcessMst;

        /// <summary>
        /// データ収集トリガーマスタテーブルアクセス
        /// </summary>
        private SqlDataCollectionTriggerMst mSqlDataCollectionTriggerMst;

        /// <summary>
        /// データ収集トリガーマスタテーブルアクセス
        /// </summary>
        private DataTable mDtDataCollectionTriggerMst;

        /// <summary>
        /// データ収集DO(ブザー)マスタテーブルアクセス
        /// </summary>
        private SqlDataCollectionDoMst mSqlDataCollectionDoMst;

        /// <summary>
        /// データ収集DO(ブザー)マスタテーブルデータ
        /// </summary>
        private DataTable mDtDataCollectionDOMst;

        /// <summary>
        /// データ収集カメラマスタテーブルアクセス
        /// </summary>
        private SqlDataCollectionCameraMst mSqlDataCollectionCameraMst;

        /// <summary>
        /// データ収集カメラマスタテーブルデータ
        /// </summary>
        private DataTable mDtDataCollectiolCameraMst;

        /// <summary>
        /// 編成工程データ
        /// </summary>
        private DataTable mDtCompositionProcessMst;

        /// <summary>
        /// 機種の設定表コントロールリスト
        /// </summary>
        private List<Control> mDeviceSettingControls;

        /// <summary>
        /// CT標準値の設定項目
        /// </summary>
        private List<RowStdValType> mRowStdValTypeList;

        /// <summary>
        /// データ収集カメラマスタのカメラIDカラム名のリスト
        /// </summary>
        private List<string> mCameraIdColumnList;

        /// <summary>
        /// データ収集カメラマスタのカメラ保存設定カラム名のリスト
        /// </summary>
        private List<string> mCameraSaveModeColumnList;

        /// <summary>
        /// カメラ名称表示ラベルリスト
        /// </summary>
        private List<Label> mLblCameraNameList;

        /// <summary>
        /// ログインスタンス
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// CT標準値の設定変更イベント デリゲート
        /// </summary>
        public event EventHandler StandardValChanged = null;

        /// <summary>
        /// グラフの表示変更イベント デリゲート
        /// </summary>
        public event EventHandler HideGraphCheckedChanged;

        private UCStandardVal_Table_Composition mUCStandardTableComposition;

        private bool loadFlg = false;
        #endregion "メンバー変数"

        //********************************************
        //* コンストラクタ
        //********************************************
        #region "コンストラクタ"
        /// <summary>
        /// 標準値マスタ CT標準値/機器の設定
        /// </summary>
        public UCStandardVal_Table_Item(Guid compositionId, Guid productTypeId, DataTable dtCompositionProcessMst, UCStandardVal_Table_Composition ucStandardValTableComposition)
        {
            InitializeComponent();

            // 2022/06/14 西部追加 
            //横スクロールバーを出すための余白を確保する
            this.Padding = new Padding(0, 0, 0, 20);
            this.AutoScroll = true;

            mUCStandardTableComposition = ucStandardValTableComposition;

            CompositionId = compositionId;
            ProductTypeId = productTypeId;
            DtCompositionProcessMst = dtCompositionProcessMst;

            mSqlSensorDeviceMst = new SqlSensorDeviceMst(Properties.Settings.Default.ConnectionString_New);
            mSqlDoDeviceMst = new SqlDoDeviceMst(Properties.Settings.Default.ConnectionString_New);
            mSqlCameraDeviceMst = new SqlCameraDeviceMst(Properties.Settings.Default.ConnectionString_New);

            mSqlStandardValProductTypeProcessMst = new SqlStandardValProductTypeProcessMst(Properties.Settings.Default.ConnectionString_New);
            mSqlDataCollectionDoMst = new SqlDataCollectionDoMst(Properties.Settings.Default.ConnectionString_New);
            mSqlDataCollectionTriggerMst = new SqlDataCollectionTriggerMst(Properties.Settings.Default.ConnectionString_New);
            mSqlDataCollectionCameraMst = new SqlDataCollectionCameraMst(Properties.Settings.Default.ConnectionString_New);

            mDeviceSettingControls = new List<Control>();

            mRowStdValTypeList = new List<RowStdValType>()
            {
                new RowStdValType(0, 1, ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MAX),
                new RowStdValType(1, 2, ColumnStandardValProductTypeProcessMst.CYCLE_TIME_AVERAGE),
                new RowStdValType(2, 3, ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MIN),
                new RowStdValType(3, 4, ColumnStandardValProductTypeProcessMst.CYCLE_TIME_DISPERSION, true),
                new RowStdValType(4, 5, ColumnStandardValProductTypeProcessMst.CYCLE_TIME_UPPER),
                new RowStdValType(5, 6, ColumnStandardValProductTypeProcessMst.CYCLE_TIME_LOWER),
                new RowStdValType(6, 11, ColumnStandardValProductTypeProcessMst.ANCILLARY),
                new RowStdValType(7, 12, ColumnStandardValProductTypeProcessMst.SETUP),
            };

            mCameraIdColumnList = new List<string>
            {
                ColumnDataCollectionCameraMst.CAMERA_ID_1,
                ColumnDataCollectionCameraMst.CAMERA_ID_2,
                ColumnDataCollectionCameraMst.CAMERA_ID_3,
                ColumnDataCollectionCameraMst.CAMERA_ID_4,
                ColumnDataCollectionCameraMst.CAMERA_ID_5,
                ColumnDataCollectionCameraMst.CAMERA_ID_6,
                ColumnDataCollectionCameraMst.CAMERA_ID_7,
            };

            mCameraSaveModeColumnList = new List<string>
            {
                ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_1,
                ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_2,
                ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_3,
                ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_4,
                ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_5,
                ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_6,
                ColumnDataCollectionCameraMst.CAMERA_SAVE_MODE_7,
            };

            mLblCameraNameList = new List<Label>
            {
                lblCameraName_0,
                lblCameraName_1,
                lblCameraName_2,
                lblCameraName_3,
                lblCameraName_4,
                lblCameraName_5,
                lblCameraName_6,
            };

            EnableStandardValView = InitializeComponentData();
        }
        #endregion "コンストラクタ"

        //********************************************
        //* プロパティ
        //********************************************
        #region "プロパティ"
        /// <summary>
        /// 編成ID
        /// </summary>
        [Category("Custom")]
        [Description("UCStandardVal")]
        [DefaultValue("")]
        public Guid CompositionId { get; set; }

        /// <summary>
        /// 品番タイプ
        /// </summary>
        [Category("Custom")]
        [Description("UCStandardVal")]
        [DefaultValue("")]
        public Guid ProductTypeId { get; set; }

        /// <summary>
        /// 編成情報
        /// </summary>
        public DataTable DtCompositionProcessMst
        {
            get { return mDtCompositionProcessMst; }
            set
            {
                mDtCompositionProcessMst = value;
                //InitializeComponentData();
            }
        }

        /// <summary>
        /// グラフ表示/非表示チェックボックス
        /// </summary>
        public CheckState GraphVisibleCheckBoxStatus
        {
            set
            {
                chkbxcHideGraph.CheckState = value;
            }
        }

        public Boolean EnableStandardValView
        {
            get; set;
        }
        #endregion "プロパティ"

        //********************************************
        //* イベント
        //********************************************
        #region "イベント"
        /// <summary>
        /// CT標準値の設定変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextChanged(object sender, EventArgs e)
        {
            if (!mUCStandardTableComposition.ChangeValueFlg)
            {
                mUCStandardTableComposition.ChangeValueFlg = true;
            }

            NotifyStandardValChanged();

            int ProcessIdx;
            int StdValType;
            decimal StdVal;
            decimal nmStdValxx_2, nmStdValxx_3;

            if (sender is NumericTextBox numText)
            {
                if (numText.Tag is String tagStr)
                {
                    String[] tagStrDim = tagStr.Split(',');
                    if (tagStrDim.Length == 2)
                    {
                        if (int.TryParse(tagStrDim[0], out ProcessIdx)
                            && int.TryParse(tagStrDim[1], out StdValType)
                            && decimal.TryParse(numText.Text, out StdVal))
                        {
                            switch (StdValType)
                            {
                                case 2:
                                case 3:
                                    {
                                        if (decimal.TryParse(tLPanelCTSetting.Controls[$"nmtxtStdVal_{ProcessIdx}_{2}"].Text, out nmStdValxx_2)
                                            && decimal.TryParse(tLPanelCTSetting.Controls[$"nmtxtStdVal_{ProcessIdx}_{3}"].Text, out nmStdValxx_3))
                                            tLPanelCTSetting.Controls[$"nmtxtStdVal_{ProcessIdx}_{4}"].Text = (nmStdValxx_2 - nmStdValxx_3).ToString();
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 機器の有効化変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDevSettingEnableStateChanged(object sender, EventArgs e)
        {
            if (!mUCStandardTableComposition.ChangeValueFlg)
            {
                mUCStandardTableComposition.ChangeValueFlg = true;
            }

            CustumCheckBox chkbx = (CustumCheckBox)sender;

            if (chkbx != null)
            {
                mDeviceSettingControls.Where(x =>
                    (x.Tag?.ToString() ?? "") == (string)chkbx.Tag
                    && x.Tag != chkbx.Tag).ToList().ForEach(x =>
                    { x.Enabled = chkbx.Checked; });

                // カメラ設定がない場合は常にコントロールを無効化
                for (int i = 0; i < CameraSettingCount; i++)
                {
                    ComboBox cmbCameraStat = (ComboBox)mDeviceSettingControls.FirstOrDefault(x => (x.Tag?.ToString() ?? "") == (string)chkbx.Tag &&
                                                                                                x.Name == $"DevSetting_{x.Tag}_{(int)DeviceSettingRowPos.CameraStat + i}");

                    if (mLblCameraNameList[i].Text == CameraNameNothing)
                    {
                        cmbCameraStat.Enabled = false;
                    }
                }
            }
        }

        /// <summary>
        /// トリガ動作変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTriggerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmbTriggerType = (ComboBox)sender;

            if (!mUCStandardTableComposition.ChangeValueFlg)
            {
                mUCStandardTableComposition.ChangeValueFlg = true;
            }

            if (cmbTriggerType != null)
            {
                SensorTypeChanged((String)cmbTriggerType.Tag, cmbTriggerType.SelectedValue);
            }
        }

        /// <summary>
        /// 良品カウント工程選択の入力値変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeCheckValue(object sender, EventArgs e)
        {
            if (!mUCStandardTableComposition.ChangeValueFlg)
            {
                mUCStandardTableComposition.ChangeValueFlg = true;
            }
        }

        /// <summary>
        /// トリガ間フィルタ(秒)、連続信号排除フィルタ(秒)の入力値変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeTextValue(object sender, EventArgs e)
        {
            if (!mUCStandardTableComposition.ChangeValueFlg)
            {
                mUCStandardTableComposition.ChangeValueFlg = true;
            }
        }

        /// <summary>
        /// センサーA、センサーB、QR用ブザー設定、現象動画カメラ設定の入力値変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeComboValue(object sender, EventArgs e)
        {
            if (!mUCStandardTableComposition.ChangeValueFlg)
            {
                mUCStandardTableComposition.ChangeValueFlg = true;
            }
        }

        /// <summary>
        /// グラフの表示変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHideGraphCheckedChanged(object sender, EventArgs e)
        {
            EventHandler handler = HideGraphCheckedChanged;
            if (handler != null)
            {
                foreach (EventHandler evhd in handler.GetInvocationList())
                {
                    evhd(sender, e);
                }
            }
        }

        // 2022/06/14西部追加
        /// <summary>
        /// マウスホイールイベント
        /// 横スクロールがある際にホイールで横スクロールが動くのを防ぐ
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            //何もしない
        }

        #endregion "イベント"

        //********************************************
        //* パブリックメソッド
        //********************************************
        #region "パブリックメソッド"

        /// <summary>
        /// 入力チェック
        /// </summary>
        public bool CheckInput(out string errMsg, out bool isGoodProductCount)
        {
            const int StdMin = 0;
            const int StdMax = 9999;
            const int IntervalFilterMin = 0;
            const int IntervalFilterMax = 9999;
            const int ExclusionFilterMin = 0;
            const int ExclusionFilterMax = 9999;

            int cntCameraEnable = 0;

            errMsg = String.Empty;
            isGoodProductCount = false;

            foreach (DataRow dr in DtCompositionProcessMst.AsEnumerable())
            {
                int processIdx = dr.Field<Int32>(ColumnCompositionProcessMst.PROCESS_IDX);

                // 値比較用の変数
                decimal maxVal = 0;
                decimal averageVal = 0;
                decimal minVal = 0;
                decimal upperVal = 0;
                decimal lowerVal = 0;

                // CT標準値の設定表
                foreach (var item in mRowStdValTypeList)
                {
                    NumericTextBox nmtxtStdVal = (NumericTextBox)tLPanelCTSetting.GetControlFromPosition(processIdx + 3, item.RowPos + 2);
                    decimal stdVal = 0;

                    if (Decimal.TryParse(nmtxtStdVal.Text, out stdVal))
                    {
                        switch (item.RowName)
                        {
                            case ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MAX:
                                maxVal = stdVal;
                                if (stdVal < StdMin || StdMax < stdVal)
                                {
                                    errMsg = "CTの最大の値が範囲外です。\nCTの最大は0以上9999以下の数値のみ入力できます。";
                                    return false;
                                }
                                break;

                            case ColumnStandardValProductTypeProcessMst.CYCLE_TIME_AVERAGE:
                                averageVal = stdVal;
                                if (stdVal < StdMin || StdMax < stdVal)
                                {
                                    errMsg = "CTの平均の値が範囲外です。\nCTの平均は0以上9999以下の数値のみ入力できます。";
                                    return false;
                                }
                                else if (maxVal < averageVal)
                                {
                                    errMsg = "CTの平均の値が最大の値を超えています。";
                                    return false;
                                }
                                break;

                            case ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MIN:
                                minVal = stdVal;
                                if (stdVal < StdMin || StdMax < stdVal)
                                {
                                    errMsg = "CTの最小の値が範囲外です。\nCTの最小は0以上9999以下の数値のみ入力できます。";
                                    return false;
                                }
                                else if (maxVal < minVal)
                                {
                                    errMsg = "CTの最小の値が最大の値を超えています。";
                                    return false;
                                }
                                else if (averageVal < minVal)
                                {
                                    errMsg = "CTの最小の値が平均の値を超えています。";
                                    return false;
                                }
                                break;

                            case ColumnStandardValProductTypeProcessMst.CYCLE_TIME_UPPER:
                                upperVal = stdVal;
                                if (stdVal < StdMin || StdMax < stdVal)
                                {
                                    errMsg = "CTの上限カットの値が範囲外です。\nCTの上限カットは0以上9999以下の数値のみ入力できます。";
                                    return false;
                                }
                                break;

                            case ColumnStandardValProductTypeProcessMst.CYCLE_TIME_LOWER:
                                lowerVal = stdVal;
                                if (stdVal < StdMin || StdMax < stdVal)
                                {
                                    errMsg = "CTの下限カットの値が範囲外です。\nCTの下限カットは0以上9999以下の数値のみ入力できます。";
                                    return false;
                                }
                                else if (upperVal < lowerVal)
                                {
                                    errMsg = "CTの下限カットの値が上限カットの値を超えています。";
                                    return false;
                                }
                                break;

                            case ColumnStandardValProductTypeProcessMst.ANCILLARY:
                                if (stdVal < StdMin || StdMax < stdVal)
                                {
                                    errMsg = "CTの付帯の値が範囲外です。\nCTの付帯は0以上9999以下の数値のみ入力できます。";
                                    return false;
                                }
                                break;

                            case ColumnStandardValProductTypeProcessMst.SETUP:
                                if (stdVal < StdMin || StdMax < stdVal)
                                {
                                    errMsg = "CTの段取の値が範囲外です。\nCTの段取は0以上9999以下の数値のみ入力できます。";
                                    return false;
                                }
                                break;
                        }
                    }
                    else
                    {
                        switch (item.RowName)
                        {
                            case ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MAX:
                                errMsg = "CTの最大の値が入力されていません。";
                                break;

                            case ColumnStandardValProductTypeProcessMst.CYCLE_TIME_AVERAGE:
                                errMsg = "CTの平均の値が入力されていません。";
                                break;

                            case ColumnStandardValProductTypeProcessMst.CYCLE_TIME_MIN:
                                errMsg = "CTの最小の値が入力されていません。";
                                break;

                            case ColumnStandardValProductTypeProcessMst.CYCLE_TIME_UPPER:
                                errMsg = "CTの上限カットの値が入力されていません。";
                                break;

                            case ColumnStandardValProductTypeProcessMst.CYCLE_TIME_LOWER:
                                errMsg = "CTの下限カットの値が入力されていません。";
                                break;

                            case ColumnStandardValProductTypeProcessMst.ANCILLARY:
                                errMsg = "CTの付帯の値が入力されていません。";
                                break;

                            case ColumnStandardValProductTypeProcessMst.SETUP:
                                errMsg = "CTの段取の値が入力されていません。";
                                break;
                        }

                        return false;
                    }
                }

                // 2022/06/13 西部追加 機器有効化にチェックがついていなければそれ以降の入力は見なくて良い
                CheckBox isEnableCheckBox = (CheckBox)mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.IsEnable}");
                if (isEnableCheckBox.Checked == false)
                {
                    continue;
                }

                // 2022/06/11 西部追加
                // 良品工程の選択を上に返す
                RadioButton goodProductRadio = (RadioButton)mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.GoodProductCount}");
                if (goodProductRadio.Checked)
                {
                    isGoodProductCount = true;
                }

                ComboBox cmbTriggerType = (ComboBox)mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.TriggerType}");
                if (cmbTriggerType.Text == "")
                {
                    errMsg = "トリガ動作が選択されていません。";
                    return false;
                }
                int triggerType = 0;
                int.TryParse(cmbTriggerType.SelectedValue.ToString(), out triggerType);

                // 機種の設定表
                // トリガ間フィルタ(秒)
                NumericTextBox nmtxtIntervalFilter = (NumericTextBox)mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.IntervalFilter}");
                decimal intervalFilter = 0;
                if (Decimal.TryParse(nmtxtIntervalFilter.Text, out intervalFilter))
                {
                    if (intervalFilter < IntervalFilterMin || IntervalFilterMax < intervalFilter)
                    {
                        errMsg = "トリガ間フィルタ(秒)の値が範囲外です。\nトリガ間フィルタ(秒)は0以上9999以下の数値のみ入力できます。";
                        return false;
                    }
                }
                else
                {
                    errMsg = "トリガ間フィルタ(秒)の値が入力されていません。";
                    return false;
                }

                // センサA
                ComboBox sensorAComboBox = (ComboBox)mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.Sensor_A}");
                if (sensorAComboBox.Text == "")
                {
                    errMsg = "センサAが選択されていません。";
                    return false;
                }

                // センサAの連続信号排除フィルタ(秒)
                NumericTextBox nmtxtExclusionFilter1 = (NumericTextBox)mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.ChatteringFilter_A}");
                decimal exclusionFilter1 = 0;
                if (Decimal.TryParse(nmtxtExclusionFilter1.Text, out exclusionFilter1))
                {
                    if (exclusionFilter1 < ExclusionFilterMin || ExclusionFilterMax < exclusionFilter1)
                    {
                        errMsg = "センサAの連続信号排除フィルタ(秒)の値が範囲外です。\n連続信号排除フィルタ(秒)は0以上9999以下の数値のみ入力できます。";
                        return false;
                    }
                }
                else
                {
                    errMsg = "センサAの連続信号排除フィルタ(秒)の値が入力されていません。";
                    return false;
                }

                // センサBは設定項目が表示されている場合のみ入力チェックを実施
                if (GetSecndDeviceEnable(triggerType))
                {
                    // センサB
                    ComboBox sensorBComboBox = (ComboBox)mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.Sensor_B}");
                    if (sensorBComboBox.Text == "")
                    {
                        errMsg = "センサBが選択されていません。";
                        return false;
                    }

                    // センサBの連続信号排除フィルタ(秒)
                    NumericTextBox nmtxtExclusionFilter2 = (NumericTextBox)mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.ChatteringFilter_B}");
                    decimal exclusionFilte2 = 0;
                    if (Decimal.TryParse(nmtxtExclusionFilter2.Text, out exclusionFilte2))
                    {
                        if (exclusionFilte2 < ExclusionFilterMin || ExclusionFilterMax < exclusionFilte2)
                        {
                            errMsg = "センサBの連続信号排除フィルタ(秒)の値が範囲外です。\n連続信号排除フィルタ(秒)は0以上9999以下の数値のみ入力できます。";
                            return false;
                        }
                    }
                    else
                    {
                        errMsg = "センサーBの連続信号排除フィルタ(秒)の値が入力されていません。";
                        return false;
                    }

                    ComboBox sensor_A = (ComboBox)mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.Sensor_A}");
                    ComboBox sensor_B = (ComboBox)mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.Sensor_B}");
                    if (sensor_A.Text == sensor_B.Text)
                    {
                        errMsg = "センサーAとセンサーBで同一のセンサーが選択されています。";
                        return false;
                    }
                }

                // カメラに何件の撮像設定が入っている？のカウント(MAX:12(仮))
                for (int i = 0; i < CameraSettingCount; i++)
                {
                    if (mLblCameraNameList[i].Text != CameraNameNothing)
                    {
                        ComboBox cmbCameraStat = (ComboBox)tLPanelDeviceSetting.GetControlFromPosition(processIdx + 3, (int)DeviceSettingRowPos.CameraStat + i);
                        if ((int)cmbCameraStat.SelectedValue > 0)
                        {
                            cntCameraEnable++;
                        }
                    }
                }
            }
            if (cntCameraEnable > Properties.Settings.Default.VideoFileMaxCnt)
            {
                errMsg = String.Format("撮影設定数が設定を超えています。規定数：{0}, 設定数：{1}", Properties.Settings.Default.VideoFileMaxCnt, cntCameraEnable);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 登録処理
        /// </summary>
        public bool Regist(SqlTransaction connection)
        {
            try
            {
                Dictionary<String, Guid> sensorItems = mDtSensorDeviceMst.AsEnumerable().ToDictionary(x => x.Field<string>(ColumnSensorDeviceMst.SENSOR_NAME), x => x.Field<Guid>(ColumnSensorDeviceMst.SENSOR_ID));
                Dictionary<String, Guid> buzzerItems = mDtDoDeviceMst.AsEnumerable().ToDictionary(x => x.Field<string>(ColumnDoDeviceMst.DO_NAME), x => x.Field<Guid>(ColumnDoDeviceMst.DO_ID));
                Dictionary<String, Guid> cameraItems = mDtCameraDeviceMst.AsEnumerable().ToDictionary(x => x.Field<string>(ColumnCameraDeviceMst.CAMERA_NAME), x => x.Field<Guid>(ColumnCameraDeviceMst.CAMERA_ID));

                foreach (DataRow dr in DtCompositionProcessMst.AsEnumerable())
                {
                    DataRow productTypeProcess;
                    DataRow DO;
                    DataRow trigger;
                    DataRow camera;

                    int processIdx = dr.Field<Int32>(ColumnCompositionProcessMst.PROCESS_IDX);

                    // インデックス取得
                    int productTypeProcessDataIdx = mDtStandardValProductTypeProcessMst.Rows.IndexOf(mDtStandardValProductTypeProcessMst.AsEnumerable()
                                                                                                    .Where(x => x.Field<Int32>(ColumnStandardValProductTypeProcessMst.PROCESS_IDX) == processIdx).FirstOrDefault()
                                                                                                );

                    int DODataIdx = mDtDataCollectionDOMst.Rows.IndexOf(mDtDataCollectionDOMst.AsEnumerable()
                                                                            .Where(x => x.Field<Int32>(ColumnDataCollectionDoMst.PROCESS_IDX) == processIdx).FirstOrDefault()
                                                                        );

                    int triggerDataIdx = mDtDataCollectionTriggerMst.Rows.IndexOf(mDtDataCollectionTriggerMst.AsEnumerable()
                                                                                    .Where(x => x.Field<Int32>(ColumnDataCollectionTriggerMst.PROCESS_IDX) == processIdx).FirstOrDefault()
                                                                                    );

                    int cameraDataIdx = mDtDataCollectiolCameraMst.Rows.IndexOf(mDtDataCollectiolCameraMst.AsEnumerable()
                                                                                    .Where(x => x.Field<Int32>(ColumnDataCollectionCameraMst.PROCESS_IDX) == processIdx).FirstOrDefault()
                                                                                );

                    if (productTypeProcessDataIdx != -1)
                    {
                        productTypeProcess = mDtStandardValProductTypeProcessMst.Rows[productTypeProcessDataIdx];
                    }
                    else
                    {
                        mDtStandardValProductTypeProcessMst.Rows.Add(mDtStandardValProductTypeProcessMst.NewRow());
                        productTypeProcess = mDtStandardValProductTypeProcessMst.Rows[mDtStandardValProductTypeProcessMst.Rows.Count - 1];
                    }

                    if (DODataIdx != -1)
                    {
                        DO = mDtDataCollectionDOMst.Rows[DODataIdx];
                    }
                    else
                    {
                        mDtDataCollectionDOMst.Rows.Add(mDtDataCollectionDOMst.NewRow());
                        DO = mDtDataCollectionDOMst.Rows[mDtDataCollectionDOMst.Rows.Count - 1];
                    }

                    if (triggerDataIdx != -1)
                    {
                        trigger = mDtDataCollectionTriggerMst.Rows[triggerDataIdx];
                    }
                    else
                    {
                        mDtDataCollectionTriggerMst.Rows.Add(mDtDataCollectionTriggerMst.NewRow());
                        trigger = mDtDataCollectionTriggerMst.Rows[mDtDataCollectionTriggerMst.Rows.Count - 1];
                    }

                    if (cameraDataIdx != -1)
                    {
                        camera = mDtDataCollectiolCameraMst.Rows[cameraDataIdx];
                    }
                    else
                    {
                        mDtDataCollectiolCameraMst.Rows.Add(mDtDataCollectiolCameraMst.NewRow());
                        camera = mDtDataCollectiolCameraMst.Rows[mDtDataCollectiolCameraMst.Rows.Count - 1];
                    }

                    // 編成ID・工程ID・品番タイプID
                    productTypeProcess[ColumnStandardValProductTypeProcessMst.COMPOSITION_ID] = CompositionId;
                    productTypeProcess[ColumnStandardValProductTypeProcessMst.PROCESS_IDX] = processIdx;
                    productTypeProcess[ColumnStandardValProductTypeProcessMst.PRODUCT_TYPE_ID] = ProductTypeId;

                    DO[ColumnDataCollectionDoMst.COMPOSITION_ID] = CompositionId;
                    DO[ColumnDataCollectionDoMst.PROCESS_IDX] = processIdx;
                    DO[ColumnDataCollectionDoMst.PRODUCT_TYPE_ID] = ProductTypeId;

                    trigger[ColumnDataCollectionTriggerMst.COMPOSITION_ID] = CompositionId;
                    trigger[ColumnDataCollectionTriggerMst.PROCESS_IDX] = processIdx;
                    trigger[ColumnDataCollectionTriggerMst.PRODUCT_TYPE_ID] = ProductTypeId;

                    camera[ColumnDataCollectionCameraMst.COMPOSITION_ID] = CompositionId;
                    camera[ColumnDataCollectionCameraMst.PROCESS_IDX] = processIdx;
                    camera[ColumnDataCollectionCameraMst.PRODUCT_TYPE_ID] = ProductTypeId;

                    // CT標準値の設定表
                    foreach (var item in mRowStdValTypeList)
                    {
                        NumericTextBox nmtxtStdVal = (NumericTextBox)tLPanelCTSetting.GetControlFromPosition(processIdx + 3, item.RowPos + 2);
                        decimal stdVal = 0;
                        Decimal.TryParse(nmtxtStdVal.Text, out stdVal);

                        productTypeProcess[item.RowName] = stdVal;
                    }

                    // 機種の設定表
                    // 機器の有効化
                    CustumCheckBox ChkBxUse = (CustumCheckBox)mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.IsEnable}");
                    productTypeProcess[ColumnStandardValProductTypeProcessMst.USE_FLAG] = ChkBxUse.Checked;

                    // 良品カウント工程選択
                    RadioButton rdoBtnGoodProductCount = (RadioButton)mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.GoodProductCount}");
                    trigger[ColumnDataCollectionTriggerMst.IS_PRODUCTION_QUANTITY_COUNTER] = rdoBtnGoodProductCount.Checked ? 1 : 0;

                    // トリガ動作
                    ComboBox cmbTriggerType = (ComboBox)mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.TriggerType}");
                    int triggerType = 0;
                    if (cmbTriggerType.SelectedValue != null && cmbTriggerType.SelectedValue.ToString() != "")
                    {
                        int.TryParse(cmbTriggerType.SelectedValue.ToString(), out triggerType);
                    }
                    trigger[ColumnDataCollectionTriggerMst.TRIGGER_TYPE] = triggerType;

                    // トリガ間フィルタ(秒)
                    NumericTextBox nmtxtIntervalFilter = (NumericTextBox)mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.IntervalFilter}");
                    decimal intervalFilter = 0;
                    Decimal.TryParse(nmtxtIntervalFilter.Text, out intervalFilter);
                    trigger[ColumnDataCollectionTriggerMst.INTERVAL_FILTER] = intervalFilter;

                    // センサA
                    ComboBox cmbSensorId1 = (ComboBox)mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.Sensor_A}");
                    if (cmbSensorId1.SelectedIndex == -1)
                    {
                        trigger[ColumnDataCollectionTriggerMst.SENSOR_ID_1] = new Guid("00000000-0000-0000-0000-000000000000");
                    }
                    else
                    {
                        Guid sensorId1 = sensorItems[cmbSensorId1.Text];
                        trigger[ColumnDataCollectionTriggerMst.SENSOR_ID_1] = sensorId1;
                    }

                    // 連続信号排除フィルタ(秒)
                    NumericTextBox nmtxtExclusionFilter1 = (NumericTextBox)mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.ChatteringFilter_A}");
                    decimal exclusionFilter1 = 0;
                    Decimal.TryParse(nmtxtExclusionFilter1.Text, out exclusionFilter1);
                    trigger[ColumnDataCollectionTriggerMst.EXCLUSION_FILTER_1] = exclusionFilter1;

                    // センサB
                    if (GetSecndDeviceEnable((int)trigger[ColumnDataCollectionTriggerMst.TRIGGER_TYPE]))
                    {
                        ComboBox cmbSensorId2 = (ComboBox)mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.Sensor_B}");
                        NumericTextBox nmtxtExclusionFilter2 = (NumericTextBox)mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.ChatteringFilter_B}");

                        Guid sensorId2 = sensorItems[cmbSensorId2.Text];
                        trigger[ColumnDataCollectionTriggerMst.SENSOR_ID_2] = sensorId2;

                        decimal exclusionFilte2;
                        Decimal.TryParse(nmtxtExclusionFilter2.Text, out exclusionFilte2);
                        trigger[ColumnDataCollectionTriggerMst.EXCLUSION_FILTER_2] = exclusionFilte2;
                    }
                    else
                    {
                        // センサB設定項目が表示されていない場合
                        // センサIDはリストの先頭のIDを設定
                        // 連続信号排除フィルタは1を設定
                        trigger[ColumnDataCollectionTriggerMst.SENSOR_ID_2] = new Guid("00000000-0000-0000-0000-000000000000");
                        trigger[ColumnDataCollectionTriggerMst.EXCLUSION_FILTER_2] = 0;
                    }

                    // QR用ブザー設定
                    ComboBox cmbDODevice = (ComboBox)mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.DODevice}");
                    if (cmbDODevice.Text == "不使用")
                    {
                        DO[ColumnDataCollectionDoMst.DO_ID] = new Guid("00000000-0000-0000-0000-000000000000");
                    }
                    else
                    {
                        Guid DODevice = buzzerItems[cmbDODevice.Text];
                        DO[ColumnDataCollectionDoMst.DO_ID] = DODevice;
                    }
                    // 現象動画カメラ設定
                    for (int i = 0; i < CameraSettingCount; i++)
                    {
                        if (mLblCameraNameList[i].Text != CameraNameNothing)
                        {
                            ComboBox cmbCameraStat = (ComboBox)tLPanelDeviceSetting.GetControlFromPosition(processIdx + 3, (int)DeviceSettingRowPos.CameraStat + i);

                            Guid cameraId = cameraItems[mLblCameraNameList[i].Text];
                            camera[mCameraIdColumnList[i]] = cameraId;

                            int cameraSaveMode = (int)cmbCameraStat.SelectedValue;
                            camera[mCameraSaveModeColumnList[i]] = cameraSaveMode;
                        }
                        else
                        {
                            camera[mCameraIdColumnList[i]] = new Guid("00000000-0000-0000-0000-000000000000");
                            camera[mCameraSaveModeColumnList[i]] = -1;
                        }
                    }
                }

                // 登録処理呼び出し
                bool ret1 = mSqlStandardValProductTypeProcessMst.Upsert(mDtStandardValProductTypeProcessMst, connection);
                bool ret2 = mSqlDataCollectionDoMst.Upsert(mDtDataCollectionDOMst, connection);
                bool ret3 = mSqlDataCollectionTriggerMst.Upsert(mDtDataCollectionTriggerMst, connection);
                bool ret4 = mSqlDataCollectionCameraMst.Upsert(mDtDataCollectiolCameraMst, connection);

                return ret1 && ret2 && ret3 && ret4;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + "," + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// CT標準値の設定取得
        /// </summary>
        /// <returns></returns>
        public List<StdValItem> GetAllStdVal()
        {
            List<StdValItem> stdValList = new List<StdValItem>();

            int ProcessIdx;
            int StdValType;
            decimal StdVal;

            foreach (var item in tLPanelCTSetting.Controls)
            {
                if (item is NumericTextBox numText)
                {
                    if (numText.Tag is String tagStr)
                    {
                        String[] tagStrDim = tagStr.Split(',');
                        if (tagStrDim.Length == 2)
                        {
                            if (int.TryParse(tagStrDim[0], out ProcessIdx)
                                && int.TryParse(tagStrDim[1], out StdValType)
                                && decimal.TryParse(numText.Text, out StdVal))
                            {
                                stdValList.Add(new StdValItem { ProcessIdx = ProcessIdx, StdValType = StdValType, StdVal = StdVal, });
                            }
                        }
                    }
                }
            }

            return stdValList;
        }

        // 2022/06/14西部追加
        /// <summary>
        /// テーブルレイアウトパネルの最大幅を変更する
        /// </summary>
        /// <param name="width"></param>
        public void ChangeTableAreaWidth(int width)
        {
            //横スクロールバーを表示するために親パネルの横幅をテーブルレイアウトパネルの最大幅にする
            this.MaximumSize = new Size(width, 8000);
            this.Width = width;
        }
        #endregion "パブリックメソッド"

        //********************************************
        //* プライベートメソッド
        //********************************************
        #region "プライベートメソッド"
        /// <summary>
        /// 設定データ初期化処理
        /// </summary>
        private Boolean InitializeComponentData()
        {
            try
            {
                if (CompositionId == Guid.Empty) return false;
                if (mDeviceSettingControls != null)
                {
                    mDeviceSettingControls.Clear();
                }

                mDtSensorDeviceMst = mSqlSensorDeviceMst.Select();
                if (mDtSensorDeviceMst == null || mDtSensorDeviceMst.Rows.Count < 1)
                {
                    logger.Error(String.Format("センサ機器マスタに設定なし"));
                    MessageBox.Show("センサ機器マスタに設定がありません。\n標準値マスタ登録画面は表示できません。", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                mDtDoDeviceMst = mSqlDoDeviceMst.Select();
                if (mDtDoDeviceMst == null || mDtDoDeviceMst.Rows.Count < 1)
                {
                    logger.Error(String.Format("ブザー機器マスタに設定なし"));
                    MessageBox.Show("ブザー機器マスタに設定がありません。\n標準値マスタ登録画面は表示できません。", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                mDtCameraDeviceMst = mSqlCameraDeviceMst.Select();
                if (mDtCameraDeviceMst == null || mDtCameraDeviceMst.Rows.Count < 1)
                {
                    logger.Error(String.Format("カメラマスタに設定なし"));
                    MessageBox.Show("カメラマスタに設定がありません。\n標準値マスタ登録画面は表示できません。", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                mDtStandardValProductTypeProcessMst = mSqlStandardValProductTypeProcessMst.Select(CompositionId, ProductTypeId);
                mDtDataCollectionDOMst = mSqlDataCollectionDoMst.Select(CompositionId, ProductTypeId);
                mDtDataCollectionTriggerMst = mSqlDataCollectionTriggerMst.Select(CompositionId, ProductTypeId);
                mDtDataCollectiolCameraMst = mSqlDataCollectionCameraMst.Select(CompositionId, ProductTypeId);

                var triggerTypeItem = new List<object>
                {
                    new { Value = 1, Name = "ONエッジ" },
                    new { Value = 2, Name = "AND" },
                    new { Value = 3, Name = "ONメモリ" },
                    new { Value = 4, Name = "OFFエッジ" },
                    new { Value = 5, Name = "OR" },
                };

                var sensorItem = mDtSensorDeviceMst.AsEnumerable().Select(x =>
                    new { Value = x.Field<Guid>(ColumnSensorDeviceMst.SENSOR_ID), Name = x.Field<String>(ColumnSensorDeviceMst.SENSOR_NAME) }).ToList();

                var buzzerItem = mDtDoDeviceMst.AsEnumerable().Select(x =>
                    new { Value = x.Field<Guid>(ColumnDoDeviceMst.DO_ID), Name = x.Field<String>(ColumnDoDeviceMst.DO_NAME) }).ToList();
                buzzerItem.Add(new { Value = new Guid("00000000-0000-0000-0000-000000000000"), Name = "不使用" });
                var cameraUseItem = new List<object>
                {
                    new { Value = 0, Name = "不使用" },
                    new { Value = 1, Name = "使用" },
                    new { Value = 2, Name = "使用(異常のみ)" },
                };

                // 2022/06/14 西部修正
                //表示する工程数分テーブルレイアウトパネルの列数を増やす ※データ数＋ヘッダー列＋スクロールバー表示用の余白1列
                int tableColumnNum = DtCompositionProcessMst.Rows.Count + 3 + 1;
                tLPanelCTSetting.ColumnCount = tableColumnNum;
                tLPanelDeviceSetting.ColumnCount = tableColumnNum;

                foreach (DataRow dr in DtCompositionProcessMst.AsEnumerable())
                {
                    tLPanelCTSetting.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115F));
                    tLPanelDeviceSetting.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115F));
                }

                //最終行は縦スクロールバー分の余白なので幅の小さい列にする
                tLPanelCTSetting.ColumnStyles[tableColumnNum - 1].Width = 30;

                // カメラ名称セット
                SetCameraName();

                int processIdx = 0;
                foreach (DataRow dr in DtCompositionProcessMst.AsEnumerable())
                {
                    processIdx = dr.Field<Int32>(ColumnCompositionProcessMst.PROCESS_IDX);

                    // 工程数
                    Label lblProcessTitle = new Label();
                    tLPanelCTSetting.Controls.Add(lblProcessTitle, processIdx + 3, 0);

                    lblProcessTitle.BackColor = SystemColors.Control;
                    lblProcessTitle.BorderStyle = BorderStyle.FixedSingle;
                    lblProcessTitle.Dock = DockStyle.Fill;
                    lblProcessTitle.Margin = new Padding(0);
                    lblProcessTitle.TextAlign = ContentAlignment.MiddleCenter;
                    lblProcessTitle.Text = $"工程{processIdx + 1}";

                    // 工程名称
                    Label lblProcessProcessName = new Label();
                    tLPanelCTSetting.Controls.Add(lblProcessProcessName, processIdx + 3, 1);

                    lblProcessProcessName.BackColor = SystemColors.Control;
                    lblProcessProcessName.BorderStyle = BorderStyle.FixedSingle;
                    lblProcessProcessName.Font = new Font("MS UI Gothic", 10.2F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
                    lblProcessProcessName.Dock = DockStyle.Fill;
                    lblProcessProcessName.Margin = new Padding(0);
                    lblProcessProcessName.TextAlign = ContentAlignment.MiddleCenter;
                    lblProcessProcessName.Text = dr.Field<String>(ColumnCompositionProcessMst.PROCESS_NAME);

                    // CT標準値の設定表作成
                    foreach (var item in mRowStdValTypeList)
                    {
                        NumericTextBox nmtxtStdVal = new NumericTextBox();
                        tLPanelCTSetting.Controls.Add(nmtxtStdVal, processIdx + 3, item.RowPos + 2);

                        nmtxtStdVal.AllowDecimalPoint = true;
                        nmtxtStdVal.AllowLeadingSign = false;
                        nmtxtStdVal.BorderStyle = BorderStyle.FixedSingle;
                        nmtxtStdVal.CultureInfo = new System.Globalization.CultureInfo("ja-JP");
                        nmtxtStdVal.Dock = DockStyle.Fill;
                        nmtxtStdVal.Font = new Font("MS UI Gothic", 10.8F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
                        nmtxtStdVal.Location = new Point(190, 84);
                        nmtxtStdVal.Margin = new Padding(0, 1, 0, 0);
                        nmtxtStdVal.MaxFractionalDigits = ((uint)(1u));
                        nmtxtStdVal.MaxIntegerDigits = ((uint)(4u));
                        nmtxtStdVal.Name = $"nmtxtStdVal_{processIdx}_{item.StdValType}";
                        nmtxtStdVal.Size = new Size(60, 17);
                        nmtxtStdVal.TabIndex = processIdx + item.RowPos * 100;
                        nmtxtStdVal.TextAlign = HorizontalAlignment.Right;
                        nmtxtStdVal.Tag = $"{processIdx},{item.StdValType}";
                        nmtxtStdVal.ReadOnly = item.ReadOnly;
                        nmtxtStdVal.Text = mDtStandardValProductTypeProcessMst.AsEnumerable().FirstOrDefault(
                                           x => x.Field<Int32>(ColumnStandardValProductTypeProcessMst.PROCESS_IDX) == processIdx &&
                                           x.Table.Columns[item.RowPos + 3].ColumnName == item.RowName)?.Field<Decimal>(item.RowName).ToString() ?? String.Empty;

                        nmtxtStdVal.TextChanged += new System.EventHandler(this.OnTextChanged);
                    }

                    // 機種の設定表作成
                    object cmbTriggerTypeSelectedValue = null;
                    Boolean devSettingEnable = false;
                    // 機器の有効化
                    {
                        CustumCheckBox ChkBxUse = new CustumCheckBox();
                        tLPanelDeviceSetting.Controls.Add(ChkBxUse, processIdx + 3, (int)DeviceSettingRowPos.IsEnable);
                        mDeviceSettingControls.Add(ChkBxUse);

                        ChkBxUse.BackColor = SystemColors.Control;
                        ChkBxUse.Dock = DockStyle.Fill;
                        ChkBxUse.Font = new Font("MS UI Gothic", 10.8F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
                        ChkBxUse.Location = new Point(190, 84);
                        ChkBxUse.Margin = new Padding(0, 1, 0, 0);
                        ChkBxUse.Name = $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.IsEnable}";
                        ChkBxUse.Size = new Size(60, 17);
                        ChkBxUse.TabIndex = processIdx + ((int)DeviceSettingRowPos.IsEnable + 20) * 100;
                        ChkBxUse.Text = "有効";
                        ChkBxUse.Tag = $"{processIdx}";
                        ChkBxUse.Checked =
                            (mDtStandardValProductTypeProcessMst.AsEnumerable().FirstOrDefault(
                                x => x.Field<Int32>(ColumnStandardValProductTypeProcessMst.PROCESS_IDX) == processIdx)?.Field<Boolean>(ColumnStandardValProductTypeProcessMst.USE_FLAG) ?? false);
                        devSettingEnable = ChkBxUse.Checked;
                        ChkBxUse.CheckStateChanged += new System.EventHandler(OnDevSettingEnableStateChanged);
                    }

                    // 良品カウント工程選択
                    {
                        RadioButton rdoBtnGoodProductCount = new RadioButton();
                        tLPanelDeviceSetting.Controls.Add(rdoBtnGoodProductCount, processIdx + 3, (int)DeviceSettingRowPos.GoodProductCount);
                        mDeviceSettingControls.Add(rdoBtnGoodProductCount);

                        rdoBtnGoodProductCount.BackColor = SystemColors.Control;
                        rdoBtnGoodProductCount.Dock = DockStyle.Fill;
                        rdoBtnGoodProductCount.Font = new Font("MS UI Gothic", 10.8F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
                        rdoBtnGoodProductCount.Location = new Point(190, 84);
                        rdoBtnGoodProductCount.Margin = new Padding(0, 1, 0, 0);
                        rdoBtnGoodProductCount.Name = $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.GoodProductCount}";
                        rdoBtnGoodProductCount.Size = new Size(60, 17);
                        rdoBtnGoodProductCount.TabIndex = processIdx + ((int)DeviceSettingRowPos.GoodProductCount + 20) * 100;
                        rdoBtnGoodProductCount.Text = "";
                        rdoBtnGoodProductCount.Tag = $"{processIdx}";
                        rdoBtnGoodProductCount.Enabled = devSettingEnable;
                        rdoBtnGoodProductCount.Checked =
                            (mDtDataCollectionTriggerMst.AsEnumerable().FirstOrDefault(
                                x => x.Field<Int32>(ColumnDataCollectionTriggerMst.PROCESS_IDX) == processIdx)?.Field<Int32>(ColumnDataCollectionTriggerMst.IS_PRODUCTION_QUANTITY_COUNTER) ?? 0) == 1;

                        rdoBtnGoodProductCount.CheckedChanged += new EventHandler(ChangeCheckValue);
                    }

                    // トリガ動作
                    {
                        ComboBox cmbTriggerType = new ComboBox();
                        tLPanelDeviceSetting.Controls.Add(cmbTriggerType, processIdx + 3, (int)DeviceSettingRowPos.TriggerType);
                        mDeviceSettingControls.Add(cmbTriggerType);

                        cmbTriggerType.BackColor = SystemColors.Control;
                        cmbTriggerType.Dock = DockStyle.Fill;
                        cmbTriggerType.Font = new Font("MS UI Gothic", 10.8F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
                        cmbTriggerType.Location = new Point(190, 84);
                        cmbTriggerType.Margin = new Padding(0, 1, 0, 0);
                        cmbTriggerType.Name = $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.TriggerType}";
                        cmbTriggerType.Size = new Size(60, 17);
                        cmbTriggerType.TabIndex = processIdx + ((int)DeviceSettingRowPos.TriggerType + 20) * 100;
                        cmbTriggerType.DataSource = new List<object>((List<object>)triggerTypeItem);
                        // 実際の値が"Value"列、表示するテキストが"Display"列とする
                        cmbTriggerType.ValueMember = "Value";
                        cmbTriggerType.DisplayMember = "Name";
                        cmbTriggerType.FormattingEnabled = true;
                        cmbTriggerType.DropDownStyle = ComboBoxStyle.DropDownList;
                        cmbTriggerType.Tag = $"{processIdx}";
                        cmbTriggerType.SelectedValue =
                            mDtDataCollectionTriggerMst.AsEnumerable().FirstOrDefault(
                                x => x.Field<Int32>(ColumnDataCollectionTriggerMst.PROCESS_IDX) == processIdx)?.Field<Int32>(ColumnDataCollectionTriggerMst.TRIGGER_TYPE) ?? -1;
                        cmbTriggerType.Enabled = devSettingEnable;
                        cmbTriggerType.SelectedIndexChanged += OnTriggerType_SelectedIndexChanged;
                        cmbTriggerTypeSelectedValue = cmbTriggerType.SelectedValue;
                    }

                    // トリガ間フィルタ(秒)
                    {
                        NumericTextBox nmtxtIntervalFilter = new NumericTextBox();
                        tLPanelDeviceSetting.Controls.Add(nmtxtIntervalFilter, processIdx + 3, (int)DeviceSettingRowPos.IntervalFilter);
                        mDeviceSettingControls.Add(nmtxtIntervalFilter);

                        nmtxtIntervalFilter.AllowDecimalPoint = true;
                        nmtxtIntervalFilter.AllowLeadingSign = false;
                        nmtxtIntervalFilter.BorderStyle = BorderStyle.FixedSingle;
                        nmtxtIntervalFilter.CultureInfo = new System.Globalization.CultureInfo("ja-JP");
                        nmtxtIntervalFilter.Dock = DockStyle.Fill;
                        nmtxtIntervalFilter.Font = new Font("MS UI Gothic", 10.8F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
                        nmtxtIntervalFilter.Location = new Point(190, 84);
                        nmtxtIntervalFilter.Margin = new Padding(0, 1, 0, 0);
                        nmtxtIntervalFilter.Name = $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.IntervalFilter}";
                        nmtxtIntervalFilter.Size = new Size(60, 17);
                        nmtxtIntervalFilter.TabIndex = processIdx + ((int)DeviceSettingRowPos.IntervalFilter + 20) * 100;
                        nmtxtIntervalFilter.Tag = $"{processIdx}";
                        nmtxtIntervalFilter.HideZero = true;
                        nmtxtIntervalFilter.MaxFractionalDigits = ((uint)(1u));
                        nmtxtIntervalFilter.MaxIntegerDigits = ((uint)(4u));
                        nmtxtIntervalFilter.Text =
                            mDtDataCollectionTriggerMst.AsEnumerable().FirstOrDefault(
                                x => x.Field<Int32>(ColumnDataCollectionTriggerMst.PROCESS_IDX) == processIdx)?.Field<Decimal>(ColumnDataCollectionTriggerMst.INTERVAL_FILTER).ToString() ?? "0.0";
                        nmtxtIntervalFilter.Enabled = devSettingEnable;
                        nmtxtIntervalFilter.TextChanged += new EventHandler(ChangeTextValue);
                    }

                    // センサA
                    {
                        ComboBox cmbSensorId1 = new ComboBox();
                        tLPanelDeviceSetting.Controls.Add(cmbSensorId1, processIdx + 3, (int)DeviceSettingRowPos.Sensor_A);
                        mDeviceSettingControls.Add(cmbSensorId1);

                        cmbSensorId1.BackColor = SystemColors.Control;
                        cmbSensorId1.Dock = DockStyle.Fill;
                        cmbSensorId1.Font = new Font("MS UI Gothic", 10.8F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
                        cmbSensorId1.Location = new Point(190, 84);
                        cmbSensorId1.Margin = new Padding(0, 1, 0, 0);
                        cmbSensorId1.Name = $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.Sensor_A}";
                        cmbSensorId1.Size = new Size(60, 17);
                        cmbSensorId1.TabIndex = processIdx + ((int)DeviceSettingRowPos.Sensor_A + 20) * 100;

                        cmbSensorId1.DataSource = new List<object>(sensorItem);
                        // 実際の値が"Value"列、表示するテキストが"Display"列とする
                        cmbSensorId1.ValueMember = "Value";
                        cmbSensorId1.DisplayMember = "Name";
                        cmbSensorId1.FormattingEnabled = true;
                        cmbSensorId1.DropDownStyle = ComboBoxStyle.DropDownList;
                        cmbSensorId1.Tag = $"{processIdx}";

                        var sensor1Index = mDtDataCollectionTriggerMst.AsEnumerable().FirstOrDefault(x => x.Field<Int32>(ColumnDataCollectionTriggerMst.PROCESS_IDX) == processIdx);
                        if (sensor1Index == null || sensor1Index[6] != DBNull.Value)
                        {
                            cmbSensorId1.SelectedValue =
                            mDtDataCollectionTriggerMst.AsEnumerable().FirstOrDefault(
                                x => x.Field<Int32>(ColumnDataCollectionTriggerMst.PROCESS_IDX) == processIdx)?.Field<Guid>(ColumnDataCollectionTriggerMst.SENSOR_ID_1) ?? new Guid("00000000-0000-0000-0000-000000000000");
                        }
                        else
                        {
                            cmbSensorId1.SelectedValue = new Guid("00000000-0000-0000-0000-000000000000");
                        }
                        if (cmbSensorId1.SelectedIndex < 0) cmbSensorId1.SelectedIndex = -1;
                        cmbSensorId1.Enabled = devSettingEnable;
                        cmbSensorId1.SelectedIndexChanged += new EventHandler(ChangeComboValue);
                    }

                    // 連続信号排除フィルタ(秒)
                    {
                        NumericTextBox nmtxtExclusionFilter1 = new NumericTextBox();
                        tLPanelDeviceSetting.Controls.Add(nmtxtExclusionFilter1, processIdx + 3, (int)DeviceSettingRowPos.ChatteringFilter_A);
                        mDeviceSettingControls.Add(nmtxtExclusionFilter1);

                        nmtxtExclusionFilter1.AllowDecimalPoint = true;
                        nmtxtExclusionFilter1.AllowLeadingSign = false;
                        nmtxtExclusionFilter1.BorderStyle = BorderStyle.FixedSingle;
                        nmtxtExclusionFilter1.CultureInfo = new System.Globalization.CultureInfo("ja-JP");
                        nmtxtExclusionFilter1.Dock = DockStyle.Fill;
                        nmtxtExclusionFilter1.Font = new Font("MS UI Gothic", 10.8F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
                        nmtxtExclusionFilter1.Location = new Point(190, 84);
                        nmtxtExclusionFilter1.Margin = new Padding(0, 1, 0, 0);
                        nmtxtExclusionFilter1.Name = $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.ChatteringFilter_A}";
                        nmtxtExclusionFilter1.Size = new Size(60, 17);
                        nmtxtExclusionFilter1.TabIndex = processIdx + ((int)DeviceSettingRowPos.ChatteringFilter_A + 20) * 100;
                        nmtxtExclusionFilter1.Tag = $"{processIdx}";
                        nmtxtExclusionFilter1.HideZero = true;
                        nmtxtExclusionFilter1.MaxFractionalDigits = ((uint)(1u));
                        nmtxtExclusionFilter1.MaxIntegerDigits = ((uint)(4u));

                        nmtxtExclusionFilter1.Text =
                            mDtDataCollectionTriggerMst.AsEnumerable().FirstOrDefault(
                                x => x.Field<Int32>(ColumnDataCollectionTriggerMst.PROCESS_IDX) == processIdx)?.Field<Decimal>(ColumnDataCollectionTriggerMst.EXCLUSION_FILTER_1).ToString() ?? "0.0";
                        nmtxtExclusionFilter1.Enabled = devSettingEnable;
                        nmtxtExclusionFilter1.TextChanged += new EventHandler(ChangeTextValue);
                    }

                    // センサB
                    {
                        ComboBox cmbSensorId2 = new ComboBox();
                        tLPanelDeviceSetting.Controls.Add(cmbSensorId2, processIdx + 3, (int)DeviceSettingRowPos.Sensor_B);
                        mDeviceSettingControls.Add(cmbSensorId2);

                        cmbSensorId2.BackColor = SystemColors.Control;
                        cmbSensorId2.Dock = DockStyle.Fill;
                        cmbSensorId2.Font = new Font("MS UI Gothic", 10.8F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
                        cmbSensorId2.Location = new Point(190, 84);
                        cmbSensorId2.Margin = new Padding(0, 1, 0, 0);
                        cmbSensorId2.Name = $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.Sensor_B}";
                        cmbSensorId2.Size = new Size(60, 17);
                        cmbSensorId2.TabIndex = processIdx + ((int)DeviceSettingRowPos.Sensor_B + 20) * 100;
                        cmbSensorId2.DataSource = new List<object>(sensorItem);
                        // 実際の値が"Value"列、表示するテキストが"Display"列とする
                        cmbSensorId2.ValueMember = "Value";
                        cmbSensorId2.DisplayMember = "Name";
                        cmbSensorId2.FormattingEnabled = true;
                        cmbSensorId2.DropDownStyle = ComboBoxStyle.DropDownList;
                        cmbSensorId2.Tag = $"{processIdx}";

                        var sensor2Index = mDtDataCollectionTriggerMst.AsEnumerable().FirstOrDefault(x => x.Field<Int32>(ColumnDataCollectionTriggerMst.PROCESS_IDX) == processIdx);
                        if (sensor2Index == null || sensor2Index[8] != DBNull.Value)
                        {
                            cmbSensorId2.SelectedValue =
                            mDtDataCollectionTriggerMst.AsEnumerable().FirstOrDefault(
                                x => x.Field<Int32>(ColumnDataCollectionTriggerMst.PROCESS_IDX) == processIdx)?.Field<Guid>(ColumnDataCollectionTriggerMst.SENSOR_ID_2) ?? new Guid("00000000-0000-0000-0000-000000000000");
                        }
                        else
                        {
                            cmbSensorId2.SelectedValue = new Guid("00000000-0000-0000-0000-000000000000");
                        }

                        if (cmbSensorId2.SelectedIndex < 0) cmbSensorId2.SelectedIndex = -1;
                        cmbSensorId2.Enabled = devSettingEnable;

                        cmbSensorId2.SelectedIndexChanged += new EventHandler(ChangeComboValue);
                    }

                    // 連続信号排除フィルタ(秒)
                    {
                        NumericTextBox nmtxtExclusionFilter2 = new NumericTextBox();
                        tLPanelDeviceSetting.Controls.Add(nmtxtExclusionFilter2, processIdx + 3, (int)DeviceSettingRowPos.ChatteringFilter_B);
                        mDeviceSettingControls.Add(nmtxtExclusionFilter2);

                        nmtxtExclusionFilter2.AllowDecimalPoint = true;
                        nmtxtExclusionFilter2.AllowLeadingSign = false;
                        nmtxtExclusionFilter2.BorderStyle = BorderStyle.FixedSingle;
                        nmtxtExclusionFilter2.CultureInfo = new System.Globalization.CultureInfo("ja-JP");
                        nmtxtExclusionFilter2.Dock = DockStyle.Fill;
                        nmtxtExclusionFilter2.Font = new Font("MS UI Gothic", 10.8F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
                        nmtxtExclusionFilter2.Location = new Point(190, 84);
                        nmtxtExclusionFilter2.Margin = new Padding(0, 1, 0, 0);
                        nmtxtExclusionFilter2.Name = $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.ChatteringFilter_B}";
                        nmtxtExclusionFilter2.Size = new Size(60, 17);
                        nmtxtExclusionFilter2.TabIndex = processIdx + ((int)DeviceSettingRowPos.ChatteringFilter_B + 20) * 100;
                        nmtxtExclusionFilter2.Tag = $"{processIdx}";
                        nmtxtExclusionFilter2.HideZero = true;
                        nmtxtExclusionFilter2.MaxFractionalDigits = ((uint)(1u));
                        nmtxtExclusionFilter2.MaxIntegerDigits = ((uint)(4u));

                        nmtxtExclusionFilter2.Text =
                            mDtDataCollectionTriggerMst.AsEnumerable().FirstOrDefault(
                                x => x.Field<Int32>(ColumnDataCollectionTriggerMst.PROCESS_IDX) == processIdx)?.Field<Decimal>(ColumnDataCollectionTriggerMst.EXCLUSION_FILTER_2).ToString() ?? "0.0";
                        nmtxtExclusionFilter2.Enabled = devSettingEnable;
                        nmtxtExclusionFilter2.TextChanged += new EventHandler(ChangeTextValue);
                    }

                    // 動作イメージ
                    {
                        PictureBox picSensor = new PictureBox();
                        tLPanelDeviceSetting.Controls.Add(picSensor, processIdx + 3, (int)DeviceSettingRowPos.TriggerImage);
                        mDeviceSettingControls.Add(picSensor);

                        picSensor.Dock = DockStyle.Fill;
                        picSensor.Name = $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.TriggerImage}";
                        picSensor.SizeMode = PictureBoxSizeMode.Zoom;
                        picSensor.TabStop = false;

                        SensorTypeChanged(processIdx.ToString(), cmbTriggerTypeSelectedValue);
                    }

                    // QR用ブザー設定
                    {
                        ComboBox cmbDODevice = new ComboBox();
                        tLPanelDeviceSetting.Controls.Add(cmbDODevice, processIdx + 3, (int)DeviceSettingRowPos.DODevice);
                        mDeviceSettingControls.Add(cmbDODevice);

                        cmbDODevice.BackColor = SystemColors.Control;
                        cmbDODevice.Dock = DockStyle.Fill;
                        cmbDODevice.Font = new Font("MS UI Gothic", 10.8F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
                        cmbDODevice.Location = new Point(190, 84);
                        cmbDODevice.Margin = new Padding(0, 1, 0, 0);
                        cmbDODevice.Name = $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.DODevice}";
                        cmbDODevice.Size = new Size(60, 17);
                        cmbDODevice.TabIndex = processIdx + ((int)DeviceSettingRowPos.DODevice + 20) * 100;
                        cmbDODevice.DataSource = new List<object>(buzzerItem);
                        cmbDODevice.ValueMember = "Value";
                        cmbDODevice.DisplayMember = "Name";
                        cmbDODevice.FormattingEnabled = true;
                        cmbDODevice.DropDownStyle = ComboBoxStyle.DropDownList;
                        cmbDODevice.Tag = $"{processIdx}";

                        //取得した工程がnullなら初期GUIDを入れる
                        var doIndex = mDtDataCollectionDOMst.AsEnumerable().FirstOrDefault(x => x.Field<Int32>(ColumnDataCollectionDoMst.PROCESS_IDX) == processIdx);
                        if (doIndex == null || doIndex[3] != DBNull.Value)
                        {
                            cmbDODevice.SelectedValue =
                            mDtDataCollectionDOMst.AsEnumerable().FirstOrDefault(
                                x => x.Field<Int32>(ColumnDataCollectionDoMst.PROCESS_IDX) == processIdx)?.Field<Guid>(ColumnDataCollectionDoMst.DO_ID) ?? new Guid("00000000-0000-0000-0000-000000000000");
                        }
                        else
                        {
                            cmbDODevice.SelectedValue = new Guid("00000000-0000-0000-0000-000000000000");
                        }

                        cmbDODevice.Enabled = devSettingEnable;
                        if (cmbDODevice.SelectedIndex < 0) cmbDODevice.SelectedIndex = -1;

                        cmbDODevice.SelectedIndexChanged += new EventHandler(ChangeComboValue);
                    }

                    // 現象動画カメラ設定
                    for (int i = 0; i < CameraSettingCount; i++)
                    {
                        ComboBox cmbCameraStat = new ComboBox();
                        tLPanelDeviceSetting.Controls.Add(cmbCameraStat, processIdx + 3, (int)DeviceSettingRowPos.CameraStat + i);
                        mDeviceSettingControls.Add(cmbCameraStat);

                        cmbCameraStat.BackColor = SystemColors.Control;
                        cmbCameraStat.Dock = DockStyle.Fill;
                        cmbCameraStat.Font = new Font("MS UI Gothic", 10.8F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(128)));
                        cmbCameraStat.Location = new Point(190, 84);
                        cmbCameraStat.Margin = new Padding(0, 1, 0, 0);
                        cmbCameraStat.Name = $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.CameraStat + i}";
                        cmbCameraStat.Size = new Size(60, 17);
                        cmbCameraStat.TabIndex = processIdx + ((int)DeviceSettingRowPos.CameraStat + i + 20) * 100;
                        cmbCameraStat.DataSource = new List<object>((List<object>)cameraUseItem);
                        // 実際の値が"Value"列、表示するテキストが"Display"列とする
                        cmbCameraStat.ValueMember = "Value";
                        cmbCameraStat.DisplayMember = "Name";
                        cmbCameraStat.FormattingEnabled = true;
                        cmbCameraStat.DropDownStyle = ComboBoxStyle.DropDownList;
                        cmbCameraStat.Tag = $"{processIdx}";

                        if (mLblCameraNameList[i].Text != CameraNameNothing)
                        {
                            cmbCameraStat.Enabled = devSettingEnable;

                            cmbCameraStat.SelectedValue =
                                mDtDataCollectiolCameraMst.AsEnumerable().FirstOrDefault(
                                    x => x.Field<Int32>(ColumnDataCollectionCameraMst.PROCESS_IDX) == processIdx)?.Field<int?>(mCameraSaveModeColumnList[i]) ?? -1;

                            if (cmbCameraStat.SelectedIndex < 0) cmbCameraStat.SelectedIndex = 0;
                        }
                        else
                        {
                            cmbCameraStat.Enabled = false;
                            cmbCameraStat.SelectedIndex = -1;
                        }

                        cmbCameraStat.SelectedIndexChanged += new EventHandler(ChangeComboValue);
                    }
                }
                loadFlg = true;
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + "," + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// センサB設定項目 有効/無効取得
        /// </summary>
        /// <param name="TriggerTypeSelectedValue"></param>
        /// <returns></returns>
        private Boolean GetSecndDeviceEnable(int TriggerTypeSelectedValue)
        {
            switch (TriggerTypeSelectedValue)
            {
                case 1:
                case 4:
                default:
                    return false;
                case 2:
                case 3:
                case 5:
                    return true;
            }
        }

        /// <summary>
        /// トリガ動作変更処理
        /// </summary>
        /// <param name="processIdx"></param>
        /// <param name="selectedValue"></param>
        private void SensorTypeChanged(String processIdx, object selectedValue)
        {
            // 2022/06/13 西部修正 DBから値が取れてこなかった場合は何も表示しない
            if (selectedValue == null || (int)selectedValue == -1)
            {
                mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.Sensor_B}").Visible = false;
                mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.ChatteringFilter_B}").Visible = false;
                return;
            }

            int iSelectedValue;
            bool secndDeviceEnable = false;
            if (int.TryParse(selectedValue?.ToString() ?? "", out iSelectedValue))
            {
                secndDeviceEnable = GetSecndDeviceEnable(iSelectedValue);

                PictureBox picSensor = (PictureBox)mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.TriggerImage}");
                switch (iSelectedValue)
                {
                    case 1:
                        picSensor.Image = global::PLOSMaintenance.Properties.Resources.TriggerType1;
                        break;
                    case 2:
                        picSensor.Image = global::PLOSMaintenance.Properties.Resources.TriggerType2;
                        break;
                    case 3:
                        picSensor.Image = global::PLOSMaintenance.Properties.Resources.TriggerType3;
                        break;
                    case 4:
                        picSensor.Image = global::PLOSMaintenance.Properties.Resources.TriggerType4;
                        break;
                    case 5:
                        picSensor.Image = global::PLOSMaintenance.Properties.Resources.TriggerType5;
                        break;
                    default:
                        picSensor.Image = null;
                        break;
                }
            }

            mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.Sensor_B}").Visible = secndDeviceEnable;
            mDeviceSettingControls.FirstOrDefault(x => x.Name == $"DevSetting_{processIdx}_{(int)DeviceSettingRowPos.ChatteringFilter_B}").Visible = secndDeviceEnable;
        }

        /// <summary>
        /// カメラ名設定
        /// </summary>
        private void SetCameraName()
        {
            for (int i = 0; i < CameraSettingCount; i++)
            {
                mLblCameraNameList[i].Text = i < mDtCameraDeviceMst.Rows.Count ? mDtCameraDeviceMst.Rows[i][ColumnCameraDeviceMst.CAMERA_NAME].ToString() : CameraNameNothing;
            }
        }

        /// <summary>
        /// CT標準値の設定変更通知イベント
        /// </summary>
        private void NotifyStandardValChanged()
        {
            EventHandler handler = StandardValChanged;
            if (handler != null)
            {
                EventArgs args = new EventArgs();

                foreach (EventHandler evhd in handler.GetInvocationList())
                {
                    evhd(this, args);
                }
            }
        }
        #endregion "プライベートメソッド"
    }
}
