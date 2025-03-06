using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace PLOSEventController
{
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single,
			 InstanceContextMode = InstanceContextMode.Single)]
	public partial class PLOSEventService : Common.IPLOSEventService
	{
		private PLOSEventControllerSub.PLOSEventManager eventManager { get; } = new PLOSEventControllerSub.PLOSEventManager();

		#region Constructors and Destructor
		public PLOSEventService()
		{
			eventManager.ConnectionString = Properties.Settings.Default.ConnectionString;
			eventManager.VideoRetrievalWCFAddress = new Uri(Properties.Settings.Default.VideoRetrievalServiceUriStr);
			eventManager.DioServiceWCFAddress = new Uri(Properties.Settings.Default.DioServiceUriStr);
			eventManager.QROKPortNo = Properties.Settings.Default.QROKPortNo;
			eventManager.QROKDelayMs = Properties.Settings.Default.QROKDelayMs;
			eventManager.QRNGPortNo = Properties.Settings.Default.QRNGPortNo;
			eventManager.QRNGDelayMs = Properties.Settings.Default.QRNGDelayMs;
			eventManager.QRAssignPortDelayMs = Properties.Settings.Default.QRAssignPortDelayMs;
			eventManager.WebcamServiceWCFAddress = new Uri(Properties.Settings.Default.WebcamServiceUriStr);

			eventManager.CycleVideoMarginMinutes = Properties.Settings.Default.CycleVideoMarginMinutes;
			eventManager.RawVideoSecond = Properties.Settings.Default.RawVideoSecond;
			eventManager.MonitoringTimerInterval = Properties.Settings.Default.MonitoringTimerInterval;
			eventManager.CycleVideoPath = Properties.Settings.Default.CycleVideoPath;
			eventManager.RowVideoPath = Properties.Settings.Default.RowVideoPath;

			eventManager.StartMonitoringTrigger();
		}
		#endregion

		/// <summary>
		/// Cycle Video Uploaded
		/// </summary>
		/// <param name="CompositionId"></param>
		/// <param name="ProcessIdx"></param>
		/// <param name="CameraIdx"></param>
		/// <param name="StartTime"></param>
		public void NotifyCycleVideoUploaded(
                string CompositionId,
                int ProcessIdx,
                int CameraIdx,
                DateTime StartTime)
        {
			Common.Logger.Instance.WriteTraceLog("Called NotifyCycleVideoUploaded");
		}

		/// <summary>
		/// DI 状態変更
		/// </summary>
		/// <param name="DeviceId"></param>
		public void NotifyDIStausChanged(string DeviceId)
        {
			Common.Logger.Instance.WriteTraceLog("Called NotifyDIStausChanged");
			// このメソッドは再入可だが、以下の処理は再入不可、なぜなら処理状態が0で同時に同じ処理をしてしまうため。
			eventManager.HandleDIEvent();
		}

		public void NotifyQRReadChanged(string DeviceId)
        {
			Common.Logger.Instance.WriteTraceLog("Called NotifyQRReadChanged");
			// このメソッドは再入可だが、以下の処理は再入不可、なぜなら処理状態が0で同時に同じ処理をしてしまうため。
			eventManager.HandleQREvent();
		}

		public bool ConnectionTest(string DeviceId)
		{
			Common.Logger.Instance.WriteTraceLog("Called ConnectionTest");
			return true;
		}

	}
}
