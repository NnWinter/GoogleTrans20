using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net6.APIs.YoudaoApi
{
    class YoudaoApiOption : ApiOption
    {
        public override Language Lan_Start { get; protected set; }
        public override List<Language> Lan_List { get; protected set; }
        public override Language Lan_End { get; protected set; }
        public override int ExecuteTimes { get; protected set; }
        public override int Interval { get; protected set; }
        public override bool UseRandom { get; protected set; }
        public override API Api { get; init; }
        public override string FilePath { get; init; }

        public YoudaoApiOption(YoudaoAPI api)
        {
            // 默认参数没有数值 需要添加数值才能用
            throw new NotImplementedException();
            Api = api;
            FilePath = Api.DirectoryPath + @"\Config.txt";
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
                $"输入新的间隔 [ms] (短间隔频繁调用API可能导致冷却)\n\n"
                );
            var input = ConsoleColors.ReadLineWithTempColors();
            if (input == null) { Tools.ShowError("无效的输入[2301292027]", false); return; }

            int newInterval;
            var isNum = int.TryParse(input, out newInterval);
            if (!isNum) { Tools.ShowError("输入不是有效的32位整数[2301292029]", false); return; }
        }

        public override void Modify()
        {
            throw new NotImplementedException();

            var loopFlag = true;
            while (loopFlag)
            {
                Console.Write(
                "\n==== 修改API设置 ====\n\n" +
                $" API - {Api.Name}\n\n" +
                $"  [0] 返回 API 界面\n" +
                $"  [1] 修改语言列表\n" +
                $"  [2] 修改翻译次数\n" +
                $"  [3] 修改调用API的间隔\n\n"
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

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Save()
        {
            throw new NotImplementedException();
        }
    }
}
