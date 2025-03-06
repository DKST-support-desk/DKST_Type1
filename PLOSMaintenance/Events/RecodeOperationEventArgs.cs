using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLOSMaintenance.Events
{
	public class RecodeOperationEventArgs : EventArgs
	{
		public enum OperationType
		{ 
			Delete = 1,
		}

		public Guid TagetId { get; set; }

		public OperationType Type { get; set; }

		public RecodeOperationEventArgs()
		{
			
		}

		public RecodeOperationEventArgs(OperationType type, Guid tagetId)
		{
			TagetId = tagetId;
			Type = type;
		}
	}
}
