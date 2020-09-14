using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatthiWare.CommandLine.Core.Attributes;
using MatthiWare.CommandLine;
using System.IO;
using System.Text.RegularExpressions;

namespace Randomizer
{
    /// <summary>
    /// Класс параметров командной строки
    /// </summary>
    public class Options
    {
        [Required, Name("l", "length"), Description("Количество символов")]
        public int Length { get; set; }

        [Name("p", "path"), Description("Путь до файла"), DefaultValue("0")]
        public string Path { get; set; }

        [Name("t", "type"), Description("Тип строки"), DefaultValue("int")]
        public string Type { get; set; }

        [Name("c", "charset"), Description("Словарь"), DefaultValue("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!\"№;%:?*()_+")]
        public string Charset { get; set; }

        [Name("s", "seed"), Description("Коэф. рандома")]
        public int Seed { get; set; }

        public bool CheckSeed = false;

        [Name ("h", "help"), Description ("Помощь")]
        public string Help { get; set; }
    }

    /// <summary>
    /// Класс вывода случайных строк типа int, string, float
    /// </summary>
    public class RandomStrings
    {
        /// <summary>
        /// Генерация случайной строки типа int
        /// </summary>
        /// <param name="length"> Длинна строки </param>
        /// <param name="rnd"> Параметр для установки seed </param>
        /// <returns> Строку из случайных цифр </returns>
        public static string GenerateInt(int length, Random rnd)
        {
            string result = "";
            for (int i = 0; i < length; i++)
            {
                int rndNum = rnd.Next(0, 9);
                result += rndNum;
            }
            return result;
        }

        /// <summary>
        /// Генерация случайной строки типа string
        /// </summary>
        /// <param name="length"> Длинна строки </param>
        /// <param name="charset"> Параметр задает словарь, из символов которого будет состоять строка </param>
        /// <param name="rnd"> Параметр для установки seed </param>
        /// <returns> Строку из случайных символов </returns>
        public static string GenerateStr(int length, string charset, Random rnd)
        {
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(charset[rnd.Next(charset.Length)]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Генерация случайной строки типа float
        /// </summary>
        /// <param name="length"> Длинна строки </param>
        /// <param name="rnd"> Параметр для установки seed </param>
        /// <returns> Строку из случайных цифр c с плавающей точкой </returns>
        public static string GenerateFlt(int length, Random rnd)
        {
            string result = "";
            for (int i = 0; i <= length; i++)
            {
                int rndNum = rnd.Next(0, 9);
                result += rndNum;
            }
            return result.Remove(1, 1).Insert(1, ","); ;
        }
    }

    class Program
    {
        //Запись в файл
        public static void PastInPath(Options Ops, Random rnd)
        {
            using (FileStream fstream = new FileStream($"{Ops.Path}\\note.txt", FileMode.Create))
            {
                // Преобразуем строку в байты
                byte[] array = System.Text.Encoding.Default
                    .GetBytes(RandomStrings.GenerateInt(Ops.Length, rnd));

                // Запись массива байтов в файл
                fstream.Write(array, 0, array.Length);
                Console.WriteLine("Текст успешно записан в файл");
            }
        }

        //Вывод ошибки о некоректном пути
        public static void ErrorPath()
        {
            Console.Error.WriteLine("Неверный путь");
            return;
        }

        static void Main(string[] args)
        {
            //Настройка командной строки
            var options = new CommandLineParserOptions
            {
                AppName = "Randomizer",
                EnableHelpOption = true,
                PostfixOption = "h"
            };

            var parser = new CommandLineParser<Options>(options);

            
            parser.AddCommand<Options>()
                .Name("-h")
                .Required(false)
                .Description("Help")
                .OnExecuting((p) =>
                {
                    Console.WriteLine("Usage: Lab_1 [options]\n" +
                        "Options: \n" +
                        "- l | --length         Количество символов\n" +
                        "- p | --path           Путь до файла\n" +
                        "- t | --type           Тип строки\n" +
                        "- c | --charset        Словарь\n" +
                        "- s | --seed           Коэф.рандома\n" +
                        "- h | --help           Помощь\n");
                    return;
                });

            var result = parser.Parse(args);

            if (result.HasErrors)
            {
                Console.Error.WriteLine("Error parsing");
               // return;
            }

            var Ops = result.Result;

            //Проверка пути для сохранения данных 
            var dirInfo = new DirectoryInfo(Ops.Path);


            var rnd = new Random();


            if (!dirInfo.Exists && Ops.Path != "0")
            {
                dirInfo.Create();
            }


            //Выполнение программы для строки int
            if (Ops.Type == "int")
            {

                //Проверка наличия пути
                if (dirInfo.Exists)
                {
                    PastInPath(Ops, rnd);
                }

                //Если путь некорректен 
                else if (!dirInfo.Exists && Ops.Path != "0")
                {
                    ErrorPath();
                }

                //Если путь не введен
                else
                    Console.WriteLine(RandomStrings.GenerateInt(Ops.Length, rnd));
            }

            //Для строки string
            else if (Ops.Type == "str")
            {
                if (dirInfo.Exists)
                {
                    PastInPath(Ops, rnd);
                }
                else if (!dirInfo.Exists && Ops.Path != "0")
                {
                    ErrorPath();
                }
                else
                    Console.WriteLine(RandomStrings.GenerateStr(Ops.Length, Ops.Charset, rnd));
            }

            //Для строки float
            else if (Ops.Type == "flt")
            {
                if (Ops.Length == 1)
                {
                    Console.Error.WriteLine("Длинна должна быть > 2");
                    return;
                }

                if (dirInfo.Exists)
                {
                    PastInPath(Ops, rnd);
                }
                else if (!dirInfo.Exists && Ops.Path != "0")
                {
                    ErrorPath();
                }
                else
                    Console.WriteLine(RandomStrings.GenerateFlt(Ops.Length, rnd));
            }

            //Если введен неверный тип данных
            else
            {
                Console.Error.WriteLine("Ошибка вывода, неверный тип");
                return;
            }
        }
    }
}
