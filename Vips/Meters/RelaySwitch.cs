namespace Vips
{
    public class RelaySwitch : BaseMeter
    {

        public Vip TestVip { get; set; } = new Vip();
        
        public bool Config(int portNum, int baudRate, int stopBits, int checkedOnConnectTimes = 1, int delayBetween = 2000)
        {
            port = new BaseSerial(portNum, baudRate, stopBits);
            DelayBetween = delayBetween;
            if (checkedOnConnectTimes >= 1)
            {
                return CheckedConnect(checkedOnConnectTimes);
            }

            return true;
        }

        public override bool CheckedConnect(int checkedOnConnectTimes = 1)
        {
            var selectCmd = libCmd.DeviceCommands
                .FirstOrDefault(x => x.Key.NameCmd == "Status" && x.Key.NameDevice == "Relay");
            
            //Количество попыток досутчатся до прибора
            for (int i = 0; i < checkedOnConnectTimes; i++)
            {
                //TODO задать задержку между командаой и ответом
                //TODO заглушка вместо задержки 
                int tempDelay = selectCmd.Value.Delay;
                Console.WriteLine($"Задержка \"Checked\" {tempDelay} мс == \"TransmitReceivedCmd\"");
                
                string receive = TransmitReceivedCmd(selectCmd.Value.Transmit, tempDelay);
                //TODO разобратся шаблон ответа должен в сбее содержать ответ прибора или наоборот
                if (selectCmd.Value.Receive.Contains(receive))
                {
                    return true;
                }
            }

            return false;
        }
    }
}