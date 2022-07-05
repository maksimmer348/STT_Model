namespace Vips.MeterLibsCommands;

public class LibDefaultCommand
{
    public Dictionary<string, CheckedMeterCommand> DefaultCommandRelay { get; set; } =
        new Dictionary<string, CheckedMeterCommand>();

    public Dictionary<string, CheckedMeterCommand> DefaultCommandSupply { get; set; } =
        new Dictionary<string, CheckedMeterCommand>();

    public LibDefaultCommand()
    {
        DefaultCommandRelay.Add("On", new CheckedMeterCommand() {ToPortCmd = "cmdon", InPorToCmd = "Ok", Delay = 50});
        DefaultCommandRelay.Add("Off", new CheckedMeterCommand() {ToPortCmd = "cmdoff", InPorToCmd = "Ok", Delay = 50});

        //TODO сделать отличным наполнение словарей ниже, в завимисотти от утсройтва, но куда добавлять устройство в констуртор
        //TODO  или сделать метод
        DefaultCommandRelay.Add("OutputOn", new CheckedMeterCommand() {ToPortCmd = "cmdon", Delay = 50});
        DefaultCommandRelay.Add("OutputOff", new CheckedMeterCommand() {ToPortCmd = "cmdoff", Delay = 50});
        
        DefaultCommandRelay.Add("VoltageMeter", new CheckedMeterCommand() {ToPortCmd = "voltmet", Delay = 50});
        DefaultCommandRelay.Add("CurrentMeter", new CheckedMeterCommand() {ToPortCmd = "currentmet", Delay = 50});
    }


    //TODO все что ниже сделать позже елси вообще понадобится
    public void AddNewCommand()
    {
    }

    public void DeleteCommand()
    {
    }
}