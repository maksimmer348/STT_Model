using System.Globalization;

namespace Vips;

public abstract class BaseRecieve
{
    public abstract bool IsRightResponse(string response);
    
    public static implicit operator BaseRecieve(string t) => new Recieve<string>(t);
    public static implicit operator BaseRecieve(double t) => new Recieve<double>(t);
}
class Recieve<T> : BaseRecieve
{
    private T data;
    
    public Recieve(T pData)
    {
        data = pData;
    }

    public override bool IsRightResponse(string response)
    {
        return response == data.ToString();
    }
    
    public static implicit operator Recieve<T>(T t) => new Recieve<T>(t);
}