using System;
using System.Collections.Generic;
using System.Drawing;
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
                Console.WriteLine("\n\n由于遇到了致命错误，" + (readLine ? "按回车退出" : "按任意键退出"));

                if (readLine) { Console.ReadLine(); }
                else { Console.ReadKey(); }

                Environment.Exit(1);
            }
        }
        public static void ShowWarning(string message)
        {
            ConsoleColors.WriteWithTempColors(message, ConsoleColor.Yellow, ConsoleColor.Black);
        }
        /// <summary>
        /// 读取用户的输入信息(多行)
        /// </summary>
        /// <returns>用户输入的文本</returns>
        public static string? ReadLines()
        {
            var input = new StringBuilder();
            while (true)
            {
                var line = Console.ReadLine();
                if (line == null) { input.AppendLine(); continue; }
                if (line.Trim() == GlobalOptions.ExitStr) { break; }
                input.AppendLine(line);
            }
            return input.ToString();
        }
        /// <summary>
        /// 从文件中读取特定参数 (读取失败则会输出提示并退出)
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="attribute">参数名</param>
        /// <returns>参数的值(Value)</returns>
        public static string LoadParamFromFile(string path, string attribute)
        {
            var lines = File.ReadAllLines(path);

            var param = lines.FirstOrDefault(x => x.StartsWith(attribute));
            if (param == null)
            {
                ShowError(
                    $"[2301302356]\n" +
                    $"设置文件中无法找到参数 \"{attribute}\"\n" +
                    $"检查程序目录下的 \"{path}\" 文件\n" +
                    $"如果无法修复问题，可以尝试删除上述文件并重新运行\n" +
                    $"这将会重置程序的全局设置为默认值\n",
                    true
                );
                return "";
            }
            try
            {
                // 消除注释
                if (param.Contains("//")) { param = param[..param.IndexOf("//")]; }
                // 拆分等号
                return param[(param.IndexOf('=') + 1)..].Trim();
            }
            catch (Exception ex)
            {
                ShowError(
                    $"[2301302357]\n" +
                    $"参数 \"{attribute}\" 格式有误\n" +
                    $"检查程序目录下的 \"{path}\" 文件\n" +
                    $"如果无法修复问题，可以尝试删除上述文件并重新运行\n" +
                    $"这将会重置程序的全局设置为默认值\n" +
                    $"错误信息: {ex.Message}",
                    true
                );
                return "";
            }
        }
        /// <summary>
        /// 保存多个参数到文件 (保存失败时提示并退出)
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="attibutes">参数数组</param>
        public static void SaveParamsToFile(string path, Attribute[] attibutes)
        {
            // 参数转文本
            var text = new StringBuilder();
            foreach(Attribute att in attibutes)
            {
                if (!string.IsNullOrWhiteSpace(att.Comment)) { text.AppendLine($"// {att.Comment}"); }
                text.AppendLine($"{att.Arttibute.Trim()} = {att.Value.Trim()}");
            }

            // 写入文件
            try
            {
                File.WriteAllText(path, text.ToString());
            }
            catch(Exception ex)
            {
                ShowError(
                    $"参数保存失败[2301310923]\n" +
                    $"检查程序目录下的 \"{path}\" 文件\n" +
                    $"错误信息: {ex.Message}",
                    true
                );
            }
        }
    }
    /// <summary>
    /// 用于读写文件中参数的结构<br/>
    /// 虽然其实可以用JSON来做，但是，neh~
    /// </summary>
    public class Attribute
    {
        /// <summary>
        /// 参数名
        /// </summary>
        public string Arttibute;
        /// <summary>
        /// 值
        /// </summary>
        public string Value;
        /// <summary>
        /// 注释
        /// </summary>
        public string? Comment;
        /// <summary>
        /// 创建新的参数
        /// </summary>
        /// <param name="attribute">参数名</param>
        /// <param name="value">值</param>
        /// <param name="comment">注释</param>
        public Attribute(string attribute, string value, string? comment)
        {
            Arttibute = attribute;
            Value = value;
            Comment = comment;
        }
    }
    class ConsoleColors
    {
        public ConsoleColor Foreground;
        public ConsoleColor Background;

        public const ConsoleColor DEFAULT_FG = ConsoleColor.White;
        public const ConsoleColor DEFAULT_BG = ConsoleColor.Black;
        public const ConsoleColor DEFAULT_READ_FG = ConsoleColor.DarkCyan;
        public const ConsoleColor DEFAULT_READ_BG = ConsoleColor.Black;

        public ConsoleColors()
        {
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
            ConsoleColors origin = new();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.BackgroundColor = ConsoleColor.Black;

            Console.Write(">");

            origin.SetToConsole();

            return ReadLineWithTempColors(DEFAULT_READ_FG, DEFAULT_READ_BG);
        }
    }
}
