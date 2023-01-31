using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net6.APIs.YoudaoApi
{
    class YoudaoApiOption : ApiOption
    {
        #region ==== 通用方法 ====

        public override string Lan_Start { get; protected set; } = "ZH_CN";
        public override List<string> Lan_List { get; protected set; } = new List<string> 
        { 
            // Youdao API 官方说明中，只支持其它几个语言和中文进行转换，
            // 不支持像 JA -> RU 这种转换，需要 JA -> ZH_CH -> RU 替代
            // 但也不排除能用，后边加警告吧
            "JA",
            "ZH_CH",
            "RU",
            "ZH_CH",
            "SP"
        };
        public override string Lan_End { get; protected set; } = "ZH_CN";
        public override int ExecuteTimes { get; protected set; } = 10;
        public override int Interval { get; protected set; } = 2000;
        public override bool UseRandom { get; protected set; } = true;
        public override API Api { get; init; }
        public override string FilePath { get; init; }
        public override void Modify()
        {
            // 从隔壁GoogleAPI抄的，应该这部分是一样的
            // 但为了保留后续添加API的自由度，就不合并方法了
            // 反正一会儿测试下就知道了
            var loopFlag = true;
            while (loopFlag)
            {
                var lanListPreview = new StringBuilder();
                if (UseRandom) { lanListPreview.Append("随机"); }
                else
                {
                    lanListPreview.Append(Lan_Start + ", ");
                    lanListPreview.Append(Language.LanListToString(Lan_List) + ", ");
                    lanListPreview.Append(Lan_End);
                }

                Console.Write(
                "\n==== 修改API设置 ====\n\n" +
                $" API - {Api.Name}\n\n" +
                $"  [0] 返回 API 界面\n\n" +
                $"  [1] 修改语言列表 <{lanListPreview}>\n" +
                $"  [2] 修改翻译次数 <{ExecuteTimes}>\n" +
                $"  [3] 修改调用API的间隔 <{Interval}ms>\n" +
                $"  [4] 切换 随机语言/固定语言 模式 <{(UseRandom ? "随机" : "固定")}>\n\n"
                );

                var input = ConsoleColors.ReadLineWithTempColors();
                if (input == null) { Tools.ShowError("无效的输入[2302010532]", false); continue; }

                switch (input.Trim())
                {
                    case "0": loopFlag = false; continue;
                    case "1": ChangeLanguageList(); continue;
                    case "2": ChangeExecuteTimes(); continue;
                    case "3": ChangeInterval(); continue;
                    case "4": UseRandom = !UseRandom; continue;
                    default: Tools.ShowError("无效的选择[2302010533]", false); continue;
                }
            }
        }
        public override void Print()
        {
            // 这个 Print 方法应该也和 GoogleAPI 一样
            // 但还是为了后续添加新API的低耦合，不合并了

            // API 名称

            Console.WriteLine($"使用的API = {Api.Name}");

            // 语言列表

            if (UseRandom) { Console.WriteLine("语言列表 = 随机"); }
            else { Console.Write("语言列表 = "); PrintLanListOptionStr(Lan_Start, Lan_List, Lan_End, Api.Languages); Console.WriteLine(); }

            // 翻译次数

            Console.WriteLine($"翻译次数 = {ExecuteTimes}");

            // 翻译间隔

            Console.WriteLine($"翻译次数 = {Interval}ms");
        }

        #endregion
        #region ==== 构造函数 ====

        public YoudaoApiOption(YoudaoAPI api)
        {
            Api = api;
            FilePath = Api.DirectoryPath + @"\Config.txt";
        }

        #endregion
        #region ==== 特有方法 ====

        private void ChangeLanguageList()
        {
            throw new NotImplementedException();
        }
        private void ChangeExecuteTimes()
        {
            throw new NotImplementedException();
        }
        private void ChangeInterval()
        {
            throw new NotImplementedException();

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

        #endregion
    }
}
