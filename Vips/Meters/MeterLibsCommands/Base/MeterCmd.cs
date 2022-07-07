namespace Vips
{

    /// <summary>
    /// Стандартная команда, ответ и задержка
    /// </summary>
    public class MeterCmd
    {
        /// <summary>
        /// Передача в утройство команды
        /// </summary>
        public string Transmit { get; set; }
        /// <summary>
        /// Прием из утройства ответа
        /// </summary>
        public string Receive { get; set; }
        /// <summary>
        /// Задержка между передачей команды и приемом ответа 
        /// </summary>
        public int Delay { get; set; }

      
        protected bool Equals(MeterCmd other)
        {
            return Transmit == other.Transmit && Receive == other.Receive && Delay == other.Delay;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MeterCmd) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Transmit, Receive, Delay);
        }
    }
}