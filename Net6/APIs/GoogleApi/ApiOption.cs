using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net6.APIs.GoogleApi
{
    class ApiOption
    {
        /// <summary>
        /// 用于进行翻译的语言列表
        /// </summary>
        public List<Language> LanguageList { get; private set; } = new List<Language>
        {
            new Language("zh", "简体中文"),
            new Language("af", null),
            new Language("cs", null),
            new Language("ja", "日文"),
            new Language("fr", "法语"),
            new Language("zh", "简体中文")
        };
        /// <summary>
        /// 翻译的次数 ( 如果是 4 个语言，则次数为 3 )
        /// </summary>
        public int ExecuteTimes { get; private set; } = 10;
        /// <summary>
        /// 调用 API 的等待间隔 ( 防止调用速度过快导致IP冷却 )
        /// </summary>
        public int Interval { get; private set; } = 2000;

        public void Modify()
        {

        }
    }
}
