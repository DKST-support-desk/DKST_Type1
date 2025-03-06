using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Topshelf;

namespace PLOSEventController
{
	class Program
	{
        private static readonly string _serviceName = "PLOSEventService";
        private static readonly string _displayName = "PLOSEventService";
        private static readonly string _description = "PLOSEventService Description";

        static void Main(string[] args) => HostFactory.Run(x =>
        {
            x.EnableShutdown();

			Common.Logger.Instance.SystemName = "PLOS";
			Common.Logger.Instance.AppName = "EventController";
			Common.Logger.Instance.WriteTraceLog("Start System");
			Common.Logger.Instance.CheckLogFile();

			// Reference to Logic Class
			x.Service<ServiceHost>(s =>
            {
                s.ConstructUsing(settings => 
                        new ServiceHost(typeof(PLOSEventService),
                        new Uri(Properties.Settings.Default.EventControllerUriStr)));
                s.WhenStarted(host => host.Open());
                s.WhenStopped(host => host.Close());
            });

            // Service Start mode
            x.StartAutomaticallyDelayed();

            // Service RunAs
            x.RunAsLocalSystem();

            // Service information
            x.SetServiceName(_serviceName);
            x.SetDisplayName(_displayName);
            x.SetDescription(_description);
        });
    }
}
