namespace Vips;

public class Supply : BaseDevice
{
    public double Power { get; set; }

    public Supply(string name, TypeDevice type) : base(name, type)
    {
    }
}