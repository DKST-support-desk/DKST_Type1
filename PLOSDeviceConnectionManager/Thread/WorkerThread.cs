// ワーカースレッドクラス
//
// --DATE--   --VRT--     --NAME--    	--EVENT--
// 09.04.15   01-00-00    TSK yamaga  	初版作成
// 09.05.23   01-01-00    TSK yamaga  	ｲﾍﾞﾝﾄQue管理方法変更
// 09.06.18   01-02-00    TSK yamaga  	終了時ｲﾍﾞﾝﾄQue空待ち処理追加
// 09.06.22   01-03-00    TSK yamaga  	Run処理をタイマー管理に変更
// 09.11.15   01-04-00    TSK yamaga  	.NET Compact Framework対応
// 11.06.14   01-05-00    TSK ishimaru	時刻補正時の処理停止を防止
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Collections;
using System.Diagnostics;

namespace TskCommon
{
    /// <summary>
    /// イベントメッセージ通知ハンドラ
    /// </summary>
    public delegate void ObjEventMsgHandler(Object obj);

    /// <summary>
    /// ログイベントメッセージ通知ハンドラ
    /// </summary>
    public delegate void LogEventMsgHandler(int iLevel, String sMesag);

#if WindowsCE
    /// <summary>
    /// 同期イベントキュー
    /// </summary>
    public class SyncQueue : Queue
    {
        /// <summary>
        /// Ｃｏｕｎｔ
        /// </summary>
        public override int Count
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return base.Count;
                }
            }
        }

        /// <summary>
        /// Ｅｎｑｕｅｕｅ
        /// </summary>
        /// <param name="obj"></param>
        public override void Enqueue(object obj)
        {
            lock (this.SyncRoot)
            {
                base.Enqueue(obj);
            }
        }

        /// <summary>
        /// Ｄｅｑｕｅｕｅ
        /// </summary>
        /// <returns></returns>
        public override object Dequeue()
        {
            lock (this.SyncRoot)
            {
                return base.Dequeue();
            }
        }

        /// <summary>
        /// Ｃｌｅａｒ
        /// </summary>
        public override void Clear()
        {
            lock (this.SyncRoot)
            {
                base.Clear();
            }
        }
    }
#endif

    /// <summary>
    /// ワーカースレッドクラス
    /// </summary>
    public abstract class WorkerThread
    {
        /// <summary> ワーカースレッド </summary>
        public Thread wt;

        public ThreadPriority pr = ThreadPriority.Normal;
        /// <summary> イベントキュー </summary>
#if !WindowsCE
        private Queue eventQue = Queue.Synchronized(new Queue());
#else
//        private SyncQueue eventQue = new SyncQueue();
#endif

        /// <summary> 終了フラグ </summary>
        private volatile Boolean endFlag;

        /// <summary> キューイベント停止フラグ </summary>
        protected Boolean eventStopFlag;

        /// <summary> 最終実行時間 </summary>
        private DateTime lastRunTime;

        /// <summary> 周期 </summary>
        private Int32 cycleTime;

        /// <summary> ログイベントメッセージ </summary>
        public event LogEventMsgHandler _logEventMsg;
        /// <summary> イベントメッセージ </summary>
        public event ObjEventMsgHandler _objEventMsg;

        /// <summary> 周期(ms) </summary>
        public Int32 CycleTime
        {
            get { return this.cycleTime; }
            set { this.cycleTime = value; }
        }

        // -----------------------------------------------------
        // 2022.05.27 【独自機能】現在のイベントキュー数の取得
        private Int32 queCnt = 0;
        public Int32 QueCount { get { return this.queCnt; } }
        // -----------------------------------------------------

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WorkerThread()
            : this(0)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WorkerThread(Int32 cycleTime)
        {
            // 終了フラグOFF
            this.endFlag = false;

            // キューイベント停止フラグOFF
            this.eventStopFlag = false;

            // 現在時刻セット
            this.lastRunTime = DateTime.Now;

            // 周期セット
            this.cycleTime = cycleTime;
        }

        /// <summary>
        /// 実行
        /// </summary>
        private void Run()
        {
            while (endFlag == false || eventQue.Count > 0)
            {
                if (endFlag == false)
                {

                    // 時刻補正チェック
                    if (this.lastRunTime > DateTime.Now)
                    {
                        this.lastRunTime = DateTime.Now.AddMilliseconds(-this.cycleTime);
                    }
                    // 周期確認
                    if (this.lastRunTime.AddMilliseconds(this.cycleTime) > DateTime.Now)
                    {
                        Thread.Sleep(1);
                        continue;
                    }
                }

                if (endFlag == false)
                {
                    // メイン処理
                    this.Proc();
                }

                while (eventQue.Count > 0 && this.eventStopFlag == false)
                {
                    // キューイベント
                    QueueEvent(eventQue.Dequeue());

                }

                // 最終実行時間
                this.lastRunTime = DateTime.Now;

                // -----------------------------------------------------
                // 2022.05.27 【独自機能】現在のイベントキュー数の取得
                queCnt = this.eventQue.Count;
                // -----------------------------------------------------

                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// 開始
        /// </summary>
        public void Start()
        {
            if (this.wt != null)
            {
                return;
            }

            // スレッド生成
            this.wt = new Thread(Run);

            // スレッド優先度設定
            this.wt.Priority = this.pr;

            // バックグラウンドスレッドに指定
            this.wt.IsBackground = true;
            // 終了フラグOFF
            this.endFlag = false;
            // スレッド開始
            this.wt.Start();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public virtual void Stop()
        {
            if (this.wt == null)
            {
                return;
            }
#if !WindowsCE
            if (this.wt.IsAlive == false)
            {
                return;
            }
#endif
            // 終了フラグON
            this.endFlag = true;
            // 終了待ち
            this.wt.Join();
            // 
            this.wt = null;
        }

        /// <summary>
        /// イベント追加
        /// </summary>
        public void AddEvent(Object obj)
        {
            eventQue.Enqueue(obj);
        }

        /// <summary>
        /// イベント削除
        /// </summary>
        public void DelEvent()
        {
            eventQue.Clear();
        }

        /// <summary>
        /// メイン処理
        /// </summary>
        protected virtual void Proc()
        {
            // (virtual) 処理なし
        }

        /// <summary>
        /// キューイベント処理
        /// </summary>
        protected virtual void QueueEvent(Object obj)
        {
            // (virtual)  処理なし
        }

        public void OnChangeEvent(object obj)
        {
            _objEventMsg(obj);
        }

        public void OnChangeLog(int iLevel, String sMesage)
        {
            _logEventMsg(iLevel, sMesage);
        }

    }
}
