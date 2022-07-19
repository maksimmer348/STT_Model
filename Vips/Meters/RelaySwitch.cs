using Microsoft.Extensions.Logging.Abstractions;
using RJCP.IO.Ports;
using SerialPortLib;

namespace Vips
{
    public class RelaySwitch : BaseMeter
    {
        public Vip TestVip { get; set; } = new Vip();

        public void Config(string pornName, int baud, int parity, int dataBits, int stopBits)
        {
           // port = new SerialInput(new NullLogger<SerialPortInput>());
            port.SetPort(pornName, baud, stopBits, parity, dataBits);
        }

        public void CheckedConnect(int checkedOnConnectTimes = 1)
        {
            var selectCmd = libCmd.DeviceCommands
                .FirstOrDefault(x => x.Key.NameCmd == "Status" && x.Key.NameDevice == "Relay");
            
            //Количество попыток досутчатся до прибора
            for (int i = 0; i < checkedOnConnectTimes; i++)
            {
                
               // TransmitCmd(selectCmd.Value.Transmit);
                //TODO разобратся шаблон ответа должен в сбее содержать ответ прибора или наоборот
                // if (selectCmd.Value.Receive.Contains(receive))
                // {
                // }
            }

        }
    }
}