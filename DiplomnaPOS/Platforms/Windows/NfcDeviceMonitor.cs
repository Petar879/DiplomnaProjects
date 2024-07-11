using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace DiplomnaPOS.Platforms.Windows
{
    public class NfcDeviceMonitor
    {
        private ManagementEventWatcher watcher;

        public event EventHandler DeviceConnected;
        public event EventHandler DeviceDisconnected;

        public NfcDeviceMonitor()
        {
            StartMonitoring();
        }

        private async void StartMonitoring()
        {
            await Task.Run(() =>
            {
                WqlEventQuery query = new WqlEventQuery("SELECT * FROM __InstanceOperationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_PnPEntity'");
                watcher = new ManagementEventWatcher(query);
                watcher.EventArrived += HandleEvent;
                watcher.Start();
            });
        }

        private void HandleEvent(object sender, EventArrivedEventArgs e)
        {
            ManagementBaseObject obj = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            string deviceId = obj["DeviceID"] as string;
            string className = obj["ClassGuid"] as string;

            if (deviceId != null && className != null)
            {
                //if (className.Equals("{990a2bd7-e738-46c7-b26f-1cf8fb9f1391}", StringComparison.OrdinalIgnoreCase))
                if (className.Equals("{50dd5230-ba8a-11d1-bf5d-0000f805f530}", StringComparison.OrdinalIgnoreCase))
                {
                    switch (e.NewEvent.ClassPath.ClassName)
                    {
                        case "__InstanceCreationEvent":
                            OnDeviceConnected();
                            break;
                        case "__InstanceDeletionEvent":
                            OnDeviceDisconnected();
                            break;
                    }
                }
            }
        }

        private void OnDeviceConnected()
        {
            Task.Run(() => DeviceConnected?.Invoke(this, EventArgs.Empty));
        }

        private void OnDeviceDisconnected()
        {
            Task.Run(() => DeviceDisconnected?.Invoke(this, EventArgs.Empty));
        }

        public void StopMonitoring()
        {
            if (watcher != null)
            {
                watcher.Stop();
                watcher.Dispose();
            }
        }
    }
}
