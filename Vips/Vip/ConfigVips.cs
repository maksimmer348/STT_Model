﻿using System;
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
        /// <param name="type">Не удалось добавить новый тип випа</param>
        public void AddTypeVips(TypeVip type)
        {
            try
            {
                TypeVips.Add(type);
                Console.WriteLine($"Создан тип Випа {type.Type}, максимальная тепмпература {type.MaxTemperature}," +
                                  $" максимальнный предварительный ток 1 {type.PrepareMaxVoltageOut1}, " +
                                  $"максимальнный предварительный ток 2 {type.PrepareMaxVoltageOut2}");
                //уведомить
            }
            catch (Exception e)
            {
                throw new VipException($"Не создан тип Випа {type.Type}, ошибка{e}");
            }
        }

        public void RemoveTypeVips(int indextypeVip)
        {
            try
            {
                Console.WriteLine($"Удален тип Випа {TypeVips[indextypeVip]}");
                TypeVips.RemoveAt(indextypeVip);
                //уведомить
            }
            catch (Exception e)
            {
                throw new VipException($"Не удален тип Випа {TypeVips[indextypeVip]}, ошибка{e}");
            }
        }
        
        public void ChangedTypeVips(int indextypeVip, TypeVip newTypeVips)
        {
            try
            {
                //Console.WriteLine($"До изменения типа Випа {TypeVips[indextypeVip].PrepareMaxVoltageOut1}, {TypeVips[indextypeVip].PrepareMaxVoltageOut2}");
                Console.WriteLine($"До изменения типа Випа {TypeVips[indextypeVip].MaxVoltageOut1}, {TypeVips[indextypeVip].MaxVoltageOut2}");
                TypeVips[indextypeVip] = newTypeVips;
                //Console.WriteLine($"После изменения тип Випа {TypeVips[indextypeVip].PrepareMaxVoltageOut1}, {TypeVips[indextypeVip].PrepareMaxVoltageOut2}");
                Console.WriteLine($"После изменения тип Випа {TypeVips[indextypeVip].MaxVoltageOut1}, {TypeVips[indextypeVip].MaxVoltageOut2}");
            }
            catch (Exception e)
            {
                throw new VipException($"Не изменен тип Випа {TypeVips[indextypeVip]}, ошибка{e}");
            }
        }
        void PrepareAddTypeVips()
        {
            AddTypeVips(new TypeVip
            {
                Type = "Vip71",
                //максимаьные значения во время испытаниий они означают ошибку
                MaxTemperature = 70,
                MaxVoltageIn = 220,
                MaxVoltageOut1 = 20,
                MaxVoltageOut2 = 25,
                MaxCurrentIn = 5,
                //максимальные значения во время предпотготовки испытания 
                PrepareMaxCurrentIn = 0.5
            });
            AddTypeVips(new TypeVip
            {
                Type = "Vip70", MaxTemperature = 70,
                MaxVoltageIn = 220,
                MaxVoltageOut1 = 40,
                MaxVoltageOut2 = 45,
                MaxCurrentIn = 2.5, PrepareMaxCurrentIn = 0.5
            });
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