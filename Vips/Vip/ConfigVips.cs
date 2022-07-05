using System;
using System.Collections.ObjectModel;

namespace Vips
{
    public class ConfigVips
    {
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

        public void AddVip(string name, int indexTypeVip)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
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

        public void RemoveVip(int indexVip)
        {
            try
            {
                Vips.RemoveAt(indexVip);
                Console.WriteLine("Вип c индексом: " + indexVip + " был удален");
                //уведомить
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Вип c индексом: " + indexVip + "не был был удален");
                //уведомить
                throw;
            }
        }
    }
}