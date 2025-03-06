using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace PLOSEventControllerSub
{
	public class WebcamServiceContWCFSender
	{
		private Uri WCFAddress { get; set; }

		private Binding WCFBinding { get; set; }

		public WebcamServiceContWCFSender(Uri address)
		{
			WCFAddress = address;
			WCFBinding = new NetTcpBinding();
		}

		public bool ConnectionTest(Guid deviceId)
		{
			return false;
		}

		public void NotifyCameraChange()
		{
			Common.Logger.Instance.WriteTraceLog("Start NotifyCameraChange");

			Task task = Task.Run(() =>
			{
				try
				{
					using (ChannelFactory<WebcamServiceLib.IWebcamService> channelFactory
						 = new ChannelFactory<WebcamServiceLib.IWebcamService>(WCFBinding, new EndpointAddress(WCFAddress)))
					{
						WebcamServiceLib.IWebcamService proxy = channelFactory.CreateChannel();

						proxy.NotifyCameraChange();
					}
				}
				catch (System.ServiceModel.CommunicationObjectFaultedException ex)
				{
					Common.Logger.Instance.WriteTraceLog("Exception NotifyCameraChange WCFメソッド呼出失敗 ServiceChannel は、状態が Faulted であるため通信に使用できません。");
				}
				catch (Exception ex)
				{
					Common.Logger.Instance.WriteTraceLog("Exception NotifyCameraChange WCFメソッド呼出失敗", ex);
				}
			});
		}
	}
}
