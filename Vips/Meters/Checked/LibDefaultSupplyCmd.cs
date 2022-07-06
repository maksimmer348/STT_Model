using System;

namespace Vips
{
    public class LibDefaultSupplyCmd : LibDefaultCmd
    {
        
        public LibDefaultSupplyCmd(string deviceType) : base(deviceType)
        {
           
            if (type == "GDM-74303")
            {
                DefaultCommand.Add("Status",
                    new MeterCmd() {ToPortCmd = "IDN?", InPorToCmd = "Ok", Delay = 50});
            }

            if (type =="ASS-743")
            {
                DefaultCommand.Add("Status",
                    new MeterCmd() {ToPortCmd = "Status?", InPorToCmd = "GWInst", Delay = 50});
               
            }

            if (type =="DPO-3014")
            {
                 DefaultCommand.Add("Status",
                    new MeterCmd() {ToPortCmd = "HEH?", InPorToCmd = "Ok", Delay = 50});
            }
            //и тд
        }
        public override void AddDefaultCommand(string name, MeterCmd cmd)
        {
            if (type == "GDM-74303")
            {
                DefaultCommand.Add(name, cmd);
                Console.WriteLine($"была добавлена команда {name} для источника питания {type}");
            }
            if (type =="ASS-743")
            {
                DefaultCommand.Add(name, cmd);
                Console.WriteLine($"была добавлена команда {name} для источника питания {type}");
            }
            if (type =="DPO-3014")
            {
                DefaultCommand.Add(name, cmd);
                Console.WriteLine($"была добавлена команда {name} для источника питания {type}");
            }
        }
    }
}