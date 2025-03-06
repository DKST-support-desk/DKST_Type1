using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace PLOSEventControllerSub
{
	public class DioServiceContWCFSender
	{
		private Uri WCFAddress { get; set; }

		private Binding WCFBinding { get; set; }

		public int QROKPortNo { get; set; }
		public int QROKDelayMs { get; set; }
		public int QRNGPortNo { get; set; }
		public int QRNGDelayMs { get; set; }
		public int QRAssignPortDelayMs { get; set; }

		public DioServiceContWCFSender(Uri address)
		{
			WCFAddress = address;
			WCFBinding = new NetTcpBinding();
		}

		public bool ConnectionTest(Guid deviceId)
		{
			return false;
		}

		public void RequestDO(int portNo, int delayMs)
		{
			Common.Logger.Instance.WriteTraceLog("Start RequestDO");
			if (portNo < 0) return;

			Task task = Task.Run(() =>
			{
				try
				{
					using (ChannelFactory<DioServiceLib.IDioService> channelFactory
						 = new ChannelFactory<DioServiceLib.IDioService>(WCFBinding, new EndpointAddress(WCFAddress)))
					{
						DioServiceLib.IDioService proxy = channelFactory.CreateChannel();
						proxy.RequestDO(portNo, delayMs);
					}
				}
				catch (System.ServiceModel.CommunicationObjectFaultedException ex)
				{
					Common.Logger.Instance.WriteTraceLog("Exception RequestDO WCFメソッド呼出失敗 ServiceChannel は、状態が Faulted であるため通信に使用できません。");
				}
				catch (Exception ex)
				{
					Common.Logger.Instance.WriteTraceLog("Exception RequestDO WCFメソッド呼出失敗", ex);
				}
			});
		}

		public void QROKDO()
		{
			RequestDO(QROKPortNo, QROKDelayMs);
		}

		public void QRNGDO()
		{
			RequestDO(QRNGPortNo, QRNGDelayMs);
		}
	}
}
