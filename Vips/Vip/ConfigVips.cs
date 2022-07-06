using System;
using System.Collections.ObjectModel;

namespace Vips
{
    public class ConfigVips
    {
        private MainValidator mainValidator = new();
        public ConfigVips()
        {
            Vips = new ObservableCollection<Vip>();
            PrepareAddTypeVips();
        }

        public ObservableCollection<Vip> Vips { get; set; }
        public ObservableCollection<TypeVip> TypeVips { get; set; } = new ObservableCollection<TypeVip>();

        /// <summary>
        /// Тип випа от него зависит его предварительные и рабочие макс значения  
        /// </summary>
        /// <param name="type"></param>
        void AddTypeVips(TypeVip type)
        {
            TypeVips.Add(type);
        }

        void PrepareAddTypeVips()
        {
            AddTypeVips(new TypeVip
                {Type = "Vip71", MaxTemperature = 71, MaxVoltage1 = 20, MaxVoltage2 = 23, MaxCurrent = 10});
            AddTypeVips(new TypeVip
                {Type = "Vip70", MaxTemperature = 70, MaxVoltage1 = 30, MaxVoltage2 = 27, MaxCurrent = 5});
        }

      
     
        //TODO сделать чтобы выполнялось при потере контекста в текстбоксе
        /// <summary>
        /// Доабавить новый Вип
        /// </summary>
        /// <param name="name">Имя Випа (Берется из текстбокса)</param>
        /// <param name="indexTypeVip">Тип Випа (берется из списка который будет привязан к индексу сомбобокса)</param>
        public void AddVip(string name, int indexTypeVip)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                // проверка на недопуст символы 
                if (!mainValidator.ValidateInvalidSymbols(name))
                {
                    //TODO уточнить где кидать исключение здесь или в классе MainValidator
                    //TODO сделать чтобы исключение выбрасывалось при потере контекста в текстбоксе
                    throw new VipException($"Название добавляемого Випа - {name}, содержит недопустимые символы");
                }
                
                // проверка на повторяющиеся имена Випов 
                if (!mainValidator.ValidateCollisionName(name, Vips))
                {
                    //TODO уточнить где кидать исключение здесь или в классе MainValidator
                    //TODO сделать чтобы исключение выбрасывалось при потере контекста в текстбоксе
                    throw new VipException($"Название добавляемого Випа - {name}, уже есть в списке");
                }
                
                var vip = new Vip()
                {
                    Name = name,
                    Type = TypeVips[indexTypeVip],
                    Status = StatusVip.None
                };
                Vips.Add(vip);
                Console.WriteLine("Вип имя: " + vip.Name + " был добалвен");
                //уведомить
            }
        }
        
        //TODO должно срабоать при удалении текста из текстбокса 
        /// <summary>
        /// Удаление Випа
        /// </summary>
        /// <param name="indexVip">Индекс Випа (берется из списка который будет привязан к индексу сомбобокса)</param>
        public void RemoveVip(Vip vip)
        {
            try
            {
                Vips.Remove(vip);
                Console.WriteLine("Вип : " + vip.Name + " был удален");
                //уведомить
            }
            catch (VipException e)
            {
                throw new VipException("Вип c индексом: " + vip.Name + "не был был удален");
            }
           
        }
    }
}