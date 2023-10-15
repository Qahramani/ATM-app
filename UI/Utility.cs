using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ARMApp.UI
{
    public static class Utility
    {
        private static long tranId;
        private static CultureInfo _culture = new CultureInfo("en-US");

        public static long GetTransactionId()
        {
            return ++tranId;
        }
        public static string GetSecretInput(string prompt)
        {
            bool isPrompt = true;
            string asteric = "";

            StringBuilder input = new StringBuilder();
            while (true)
            {
                if (isPrompt)
                    Console.WriteLine(prompt);
                isPrompt = false;

                ConsoleKeyInfo inputKey = Console.ReadKey(true);

                if (inputKey.Key == ConsoleKey.Enter)
                {
                    if (input.Length == 6)
                        break;
                    else
                    {
                        PrintMessage("\nPlease enter 6 digits", false);
                        isPrompt = true;
                        input.Clear();
                        continue;
                    }
                }
                if(inputKey.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input.Remove(input.Length - 1, 1);
                }else if(inputKey.Key != ConsoleKey.Backspace)
                {
                    input.Append(inputKey.KeyChar);
                    Console.Write(asteric + "*");
                }
            }
            return input.ToString();
        }
        public static void PrintMessage(string message , bool succes = true)
        {
            if (succes)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
            PressEnterToContinue();
        }
        
        public static string GetUserInput(string prompt)
        {
            Console.WriteLine($"Enter {prompt}");
            return Console.ReadLine();
        }

        public static void PrintDotAnimation(int timer = 10)
        {
            for (int i = 0; i < timer; i++)
            {
                Console.Write(".");
                Thread.Sleep(200);
            }
            Console.Clear();
        }
        public static void PressEnterToContinue()
        {
            Console.WriteLine("\nEnter to continue......");
            Console.ReadKey();
        }
        public static string FormatAmount(decimal amt)
        {
            return String.Format(_culture, "{0:C3}" , amt);
        }
    }
}
