using Microsoft.Extensions.Logging.Abstractions;
using RJCP.IO.Ports;
using SerialPortLib;

namespace Vips
{
    public class RelaySwitch : BaseDevice
    {
        public Vip TestVip { get; set; } = new Vip();

        public RelaySwitch(string name, TypeDevice type) : base(name, type)
        {
        }

        public void ConfigDevice(string pornName, int baud, int parity, int dataBits, int stopBits)
        {
            port = new SerialInput();
            port.SetPort(Config.PortName, Config.Baud, Config.StopBits, Config.Parity, Config.DataBits);
            port.Dtr = Config.Dtr;
        }
    }
}