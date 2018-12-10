using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Temp.Server
{
    public interface IDeviceRepository
    {
        IList<TempDevice> GetDevices();

        TempDevice GetDevice(int id);

        void SaveDevice(TempDevice device);
    }
}
