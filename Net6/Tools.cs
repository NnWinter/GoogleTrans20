using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net6
{
    internal class Tools
    {
        /// <summary>
        /// 向控制台输出错误信息，如果是致命错误，等待用户确认并退出程序<br />
        /// (不含换行)
        /// </summary>
        /// <param name="message">要显示的错误信息</param>
        /// <param name="isFatel">是否为致命错误</param>
        /// <param name="readLine">等待用户输入是否将 按任意键退出 变为 换行退出</param>
        public static void ShowError(string message, bool isFatel, bool readLine = false)
        {
            ConsoleColors.WriteWithTempColors(message, ConsoleColor.Red, ConsoleColor.Black);

            if (isFatel)
            {
                Console.WriteLine("\n\n由于遇到了致命错误，" + (readLine ? "按回车退出":"按任意键退出"));

                if (readLine) { Console.ReadLine(); }
                else { Console.ReadKey(); }
                
                Environment.Exit(1);
            }
        }
        public static void ShowWarning(string message)
        {
            ConsoleColors.WriteWithTempColors(message, ConsoleColor.Yellow, ConsoleColor.Black);
        }
    }
    class ConsoleColors
    {
        public ConsoleColor Foreground;
        public ConsoleColor Background;

        public const ConsoleColor DEFAULT_FG = ConsoleColor.White;
        public const ConsoleColor DEFAULT_BG = ConsoleColor.Black;
        public const ConsoleColor DEFAULT_READ_FG = ConsoleColor.Blue;
        public const ConsoleColor DEFAULT_READ_BG = ConsoleColor.Black;

        public ConsoleColors() {
            Foreground = Console.ForegroundColor;
            Background = Console.BackgroundColor;
        }
        public ConsoleColors(ConsoleColor foreground, ConsoleColor background)
        {
            Foreground = foreground; 
            Background = background;
        }
        public void SetToConsole()
        {
            Console.ForegroundColor = Foreground; 
            Console.BackgroundColor = Background;
        }
        public static void ResetDefault()
        {
            Console.ForegroundColor = DEFAULT_FG;
            Console.BackgroundColor = DEFAULT_BG;
        }
        public static void WriteWithTempColors(string text, ConsoleColor foreground, ConsoleColor background)
        {
            ConsoleColors origin = new();

            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            

            Console.Write(text);

            origin.SetToConsole();
        }
        public static string? ReadLineWithTempColors(ConsoleColor foreground, ConsoleColor background)
        {
            ConsoleColors origin = new();

            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;

            string? str = Console.ReadLine();
            origin.SetToConsole();
            return str;
        }
        public static string? ReadLineWithTempColors()
        {
            return ReadLineWithTempColors(DEFAULT_READ_FG, DEFAULT_READ_BG);
        }
    }
}
