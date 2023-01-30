using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net6.APIs.YoudaoApi
{
    class ApiOption
    {
        /// <summary>
        /// 用于进行翻译的语言列表
        /// </summary>
        public List<Language> LanguageList { get; private set; } = new List<Language>
        {

        };
        /// <summary>
        /// 翻译的次数 ( 如果是 4 个语言，则次数为 3 )
        /// </summary>
        public int ExecuteTimes { get; private set; } = 10;
        /// <summary>
        /// 调用 API 的等待间隔 ( 防止调用速度过快导致IP冷却 )
        /// </summary>
        public int Interval { get; private set; } = 2000;
        public YoudaoAPI Api { get; init; }

        public ApiOption(YoudaoAPI api)
        {
            Api = api;
        }

        public void Modify()
        {
            var loopFlag = true;
            while (loopFlag)
            {
                Console.Write(
                "\n==== 修改API设置 ====\n\n" +
                $" API - {Api.Name}\n\n" +
                $"  [0] 返回 API 界面\n" +
                $"  [1] 修改语言列表\n" +
                $"  [2] 修改翻译次数\n" +
                $"  [3] 修改调用API的间隔\n\n>"
                );

                var input = ConsoleColors.ReadLineWithTempColors();
                if (input == null) { Tools.ShowError("无效的输入[2301292014]", false); continue; }

                switch (input.Trim())
                {
                    case "0": loopFlag = false; continue;
                    case "1": ChangeLanguageList(); continue;
                    case "2": ChangeExecuteTimes(); continue;
                    case "3": ChangeInterval(); continue;
                    default: Tools.ShowError("无效的选择[2301292019]", false); continue;
                }
            }
        }
        private void ChangeLanguageList()
        {

        }
        private void ChangeExecuteTimes()
        {

        }
        private void ChangeInterval()
        {
            Console.Write(
                $"\n当前调用API的间隔时间为 {Interval}ms\n" +
                $"输入新的间隔 [ms] (短间隔频繁调用API可能导致冷却)\n\n>"
                );
            var input = ConsoleColors.ReadLineWithTempColors();
            if (input == null) { Tools.ShowError("无效的输入[2301292027]", false); return; }

            int newInterval;
            var isNum = int.TryParse(input, out newInterval);
            if (!isNum) { Tools.ShowError("输入不是有效的32位整数[2301292029]", false); return; }
        }
    }
}
