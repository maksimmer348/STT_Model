namespace Vips;

public class RelaySwitch : BaseMeter
{
    /// <summary>
    /// Задается значение в секундах для проверки значений при запуске стенда
    /// </summary>
    public int TimeForTestRelayStart { get; set; } = 2;

    public Vip TestVip { get; set; } = new Vip();
}