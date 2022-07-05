namespace Vips;

public class BaseMeter
{
    public string Name { get; set; }
    private TypeDevice type;
    private BaseSerial port;
    private LibDefaultTypes lib = new();


    public bool Config(int portNum, int baudRate, int stopBits,int checkTimes = 1, bool check = true)
    {
        port = new BaseSerial();
        
        //TODO дополнить остальными занчения компорта
        port.PortNum = portNum;
        //port.portConf
        //port.baudRate
        //port.stopBits
        if (check)
        {
            return Checked(checkTimes);
        }

        return true;
    }

    public bool Checked(int checkTimes = 1)
    {
        
        if (lib.DefaultCommand.ContainsKey(Name))
        {
            var value = lib.DefaultCommand[Name];
            port.Write(value.ToPortCmd);
            //int delay = cmd.Value.Delay;
            for (int i = 0; i < checkTimes; i++)
            {
                //TODO правильно ли я делаю точную проверку может сделать контейнс
                if (value.InPorToCmd == port.Read())
                {  
                    Console.WriteLine(Name + " работает");
                    return true;
                }
            }
        }
        Console.WriteLine(Name + " не работает");
        return false;
    }
}