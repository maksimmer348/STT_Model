namespace Vips
{
    public class BaseSerial
    {
        public int PortNum { get; set; }


        public void Write(string write)
        {
            //write -> to port
        }

        public string Read()
        {
            //in port -> return 
            return "Ok";
        }
    }
}