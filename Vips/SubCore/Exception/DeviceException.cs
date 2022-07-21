namespace Vips;

//TODO обработка исключений сделать
public class DeviceException : Exception
{
    public DeviceException(string message)
        : base(message)
    { }
}