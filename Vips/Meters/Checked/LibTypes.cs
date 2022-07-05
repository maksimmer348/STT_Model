using System.Collections.Generic;

namespace Vips
{
//TODO сделать инстантом, правильно ли я сделал библиотекчку, ничего что она отличаеся от библиотеки стандартных комманд
    public class LibDefaultTypes

    {
        public Dictionary<string, CheckedMeterCommand> DefaultCommand { get; set; } =
            new Dictionary<string, CheckedMeterCommand>();

        public LibDefaultTypes()
        {
            DefaultCommand.Add("GDM-74303",
                new CheckedMeterCommand() {ToPortCmd = "IDN?", InPorToCmd = "Ok", Delay = 50});
            DefaultCommand.Add("ASS-743",
                new CheckedMeterCommand() {ToPortCmd = "Status?", InPorToCmd = "GWInst", Delay = 50});
            DefaultCommand.Add("DPO-3014",
                new CheckedMeterCommand() {ToPortCmd = "HEH?", InPorToCmd = "Ok", Delay = 50});
            //и тд
        }

    }
}