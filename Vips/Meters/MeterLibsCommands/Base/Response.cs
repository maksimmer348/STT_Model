using System.Globalization;

namespace Vips;

// public abstract class BaseReceive
// {
//     public abstract bool IsRightResponse(string response);
//     
//     public static implicit operator BaseReceive(string t) => new Receive<string>(t);
//     public static implicit operator BaseReceive(double t) => new Receive<double>(t);
//     public abstract string GetDataToString();
// }
// public class Receive<T> : BaseReceive
// {
//     private T data;
//     
//     public Receive(T pData)
//     {
//         data = pData;
//     }
//
//     public override bool IsRightResponse(string response)
//     {
//         return response == data.ToString();
//     }
//     public override string GetDataToString() => data.ToString();
//
//     public static implicit operator Receive<T>(T t) => new Receive<T>(t);
//    
// }