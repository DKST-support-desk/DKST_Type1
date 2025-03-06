using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace PLOSEventControllerSub
{
	public class VideoRetrievalContWCFSender
	{
		private Uri WCFAddress { get; set; }

		private Binding WCFBinding { get; set; }

		public VideoRetrievalContWCFSender(Uri address)
		{
			WCFAddress = address;
			WCFBinding = new NetTcpBinding();
		}

		public bool ConnectionTest(Guid deviceId)
		{
			return false;
		}

		public void CreateCycleVideo(Guid configId, int procIdx, int camIdx, DateTime startTime)
		{
			Common.Logger.Instance.WriteTraceLog("Start CreateCycleVideo");

			Task task = Task.Run(() =>
			{
				try
				{
					using (ChannelFactory<VideoRetrievalServiceLib.IVideoRetrivalService> channelFactory
						 = new ChannelFactory<VideoRetrievalServiceLib.IVideoRetrivalService>(WCFBinding, new EndpointAddress(WCFAddress)))
					{
						VideoRetrievalServiceLib.IVideoRetrivalService proxy = channelFactory.CreateChannel();
						proxy.CreateCycleVideo(configId, procIdx, camIdx, startTime);
					}
				}
				catch (System.ServiceModel.CommunicationObjectFaultedException ex)
				{
					Common.Logger.Instance.WriteTraceLog("Exception CreateCycleVideo WCFメソッド呼出失敗 ServiceChannel は、状態が Faulted であるため通信に使用できません。");
				}
				catch (Exception ex)
				{
					Common.Logger.Instance.WriteTraceLog("Exception CreateCycleVideo WCFメソッド呼出失敗", ex);
				}
			});
		}
	}
}
