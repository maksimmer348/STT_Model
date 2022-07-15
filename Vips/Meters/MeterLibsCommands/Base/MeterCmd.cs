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
        /// Задержка между передачей команды и приемом ответа 
        /// </summary>
        /// <summary>
        /// Задержка между передачей команды и приемом ответа 
        /// </summary>
        public string Terminator { get; set; }

        public TypeAnswer ReceiveType { get; set; }

        /// <summary>
        /// Прием из утройства ответа
        /// </summary>
        public string Receive { get; set; }

        /// <summary>
        /// Задержка между передачей команды и приемом ответа 
        /// </summary>
        public int Delay { get; set; }

        /// <summary>
        ///  Количество Запросов на прибор если в ответ пришла ошибка
        /// </summary>
        public int PingCountIfError { get; set; }

        /// <summary>
        ///  Ожидамое окончание строки если в ответ пришла ошибка
        /// </summary>
        public string StartOfStringIfError { get; set; }

        /// <summary>
        ///  Ожидамое окончание строки если в ответ пришла ошибка
        /// </summary>
        public string EndOfStringIfError { get; set; }

        protected bool Equals(MeterCmd other)
        {
            return Transmit == other.Transmit && Terminator == other.Terminator && Receive == other.Receive &&
                   Delay == other.Delay && PingCountIfError == other.PingCountIfError &&
                   StartOfStringIfError == other.StartOfStringIfError && EndOfStringIfError == other.EndOfStringIfError;
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
            return HashCode.Combine(Transmit, Terminator, Receive, Delay);
        }
    }
}