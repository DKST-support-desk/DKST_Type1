using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Common
{
    [ServiceContract]
    public interface IPLOSEventService
    {
        [OperationContract]
        void NotifyCycleVideoUploaded(
                string CompositionId,
                int ProcessIdx,
                int CameraIdx,
                DateTime StartTime);

        [OperationContract]
        void NotifyDIStausChanged(
                string DeviceId);

        [OperationContract]
        void NotifyQRReadChanged(
                string DeviceId);

        [OperationContract]
        bool ConnectionTest(string DeviceId);

    }
}
