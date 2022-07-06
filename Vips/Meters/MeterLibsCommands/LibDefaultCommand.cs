using System.Collections.Generic;

namespace Vips
{
    public class LibDefaultCommand
    {
        public Dictionary<string, MeterCmd> DefaultCommandRelay { get; set; } =
            new Dictionary<string, MeterCmd>();

        public Dictionary<string, MeterCmd> DefaultCommandSupply { get; set; } =
            new Dictionary<string, MeterCmd>();

        public LibDefaultCommand()
        {
            DefaultCommandRelay.Add("On",
                new MeterCmd() {ToPortCmd = "cmdon", InPorToCmd = "Ok", Delay = 50});
            DefaultCommandRelay.Add("Off",
                new MeterCmd() {ToPortCmd = "cmdoff", InPorToCmd = "Ok", Delay = 50});

            //TODO сделать отличным наполнение словарей ниже, в завимисотти от утсройтва, но куда добавлять устройство в констуртор
            //TODO  или сделать метод
            DefaultCommandRelay.Add("OutputOn", new MeterCmd() {ToPortCmd = "cmdon", Delay = 50});
            DefaultCommandRelay.Add("OutputOff", new MeterCmd() {ToPortCmd = "cmdoff", Delay = 50});

            DefaultCommandRelay.Add("VoltageMeter", new MeterCmd() {ToPortCmd = "voltmet", Delay = 50});
            DefaultCommandRelay.Add("CurrentMeter", new MeterCmd() {ToPortCmd = "currentmet", Delay = 50});
        }


        //TODO все что ниже сделать позже елси вообще понадобится
        public void AddNewCommand()
        {
        }

        public void DeleteCommand()
        {
        }
    }
}