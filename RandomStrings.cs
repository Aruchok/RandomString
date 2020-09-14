using System;

    internal class RandomStrings
    {
        public void GenerateInt(int length)
        {
            Random rnd = new Random();
            string result = "";
            for (int i = 0; i < length; i++)
            {
                int rndNum = rnd.Next(0, 9);
                result += rndNum;
            }
            Console.Write(result + "\n");
        }

        public void GenerateStr(int length)
        {
            Random random = new Random();
            string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(characters[random.Next(characters.Length)]);
            }
            Console.Write(result.ToString() + "\n");
        }
    }
