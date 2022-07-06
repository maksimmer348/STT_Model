using System.Collections.Generic;

namespace Vips
{
//TODO сделать инстантом, правильно ли я сделал библиотекчку, ничего что она отличаеся от библиотеки стандартных комманд
    public class LibDefaultCmd
    {
        protected string type;

        public LibDefaultCmd( string deviceType)
        {
            type = deviceType;
        }

        public Dictionary<string, MeterCmd> DefaultCommand { get; set; } =
            new Dictionary<string, MeterCmd>();


        public virtual void AddDefaultCommand(string name, MeterCmd cmd)
        {
        }
    }
}