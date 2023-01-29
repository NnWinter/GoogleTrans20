using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net6
{
    internal class GlobalOptions
    {
        public const string OPTIONS_FILE_PATH = "GlobalOptions.txt";
        /// <summary>
        /// 输入文本后用于结束输入的按键
        /// </summary>
        public static ConsoleKey ExitKey { get; private set; } = ConsoleKey.End;
        /// <summary>
        /// 输入文本后用于结束输入的文本<br/>
        /// (如果一行文本内只有 ExitStr 就结束输入)
        /// </summary>
        public static string ExitStr { get; private set; } = "%%";

        /// <summary>
        /// 从文件中读取设置
        /// </summary>
        public static void Load()
        {
            ExitKey = ConsoleKey.End;
            ExitStr = "%%";
        }
        public static void Save()
        {
            string ExitKeyStr = 
                "// 结束输入的快捷键 (ConsoleKey)\n" +
                $"ExitKey = {(long)ExitKey}";
            string ExitStrStr =
                "// 结束输入使用的文本\n" +
                $"ExitStr = ExitStr";

            File.WriteAllText(OPTIONS_FILE_PATH, ExitKeyStr);
        }
        public void Modify()
        {
            throw new NotImplementedException();
            ConsoleColors.ReadLineWithTempColors();
        }
    }
    public class Option
    {
        /// <summary>
        /// 用于进行翻译的语言列表
        /// </summary>
        public List<string> LanguageList { get; private set; }
        /// <summary>
        /// 翻译的次数 ( 如果是 4 个语言，则次数为 3 )
        /// </summary>
        public int ExecuteTimes { get; private set; }
    }
}
