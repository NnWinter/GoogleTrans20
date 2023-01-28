using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net6
{
    internal class Option
    {
        /// <summary>
        /// 用于进行翻译的语言列表
        /// </summary>
        public List<string> LanguageList { get; private set; }
        /// <summary>
        /// 翻译的次数 ( 如果是 4 个语言，则次数为 3 )
        /// </summary>
        public int ExecuteTimes { get; private set; }
        /// <summary>
        /// 输入文本后用于结束输入的按键
        /// </summary>
        public ConsoleKey ExitKey { get; private set; } 
        /// <summary>
        /// 输入文本后用于结束输入的文本<br/>
        /// (如果一行文本内只有 ExitStr 就结束输入)
        /// </summary>
        public string ExitStr { get; private set; }

        /// <summary>
        /// 默认设置 
        /// </summary>
        public Option()
        {
            LanguageList= new List<string> { "zh", "en", "af", "zh" };
            ExecuteTimes = LanguageList.Count - 1;
            ExitKey = ConsoleKey.End;
            ExitStr = "%%";
        }
        public void Modify()
        {
            throw new NotImplementedException();
            ConsoleColors.ReadLineWithTempColors();
        }
    }
}
