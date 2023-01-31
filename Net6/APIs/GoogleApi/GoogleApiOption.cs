using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Net6.APIs.GoogleApi
{
    class GoogleApiOption : ApiOption
    {
        #region ==== 通用方法 ====

        public override string Lan_Start { get; protected set; } = "zh";
        public override List<string> Lan_List { get; protected set; } = new List<string>
        {
            "af",
            "cs",
            "ja",
            "fr",
            "kn"
        };
        public override string Lan_End { get; protected set; } = "zh";
        public override int ExecuteTimes { get; protected set; } = 10;
        public override int Interval { get; protected set; } = 2000;
        public override bool UseRandom { get; protected set; } = true;
        public override API Api { get; init; }
        public override string FilePath { get; init; }
        public override void Modify()
        {
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
                if (input == null) { Tools.ShowError("无效的输入[2301292014]", false); continue; }

                switch (input.Trim())
                {
                    case "0": loopFlag = false; Save(); continue; // 修改完要保存，嗯？
                    case "1": ChangeLanguageList(); continue;
                    case "2": ChangeExecuteTimes(); continue;
                    case "3": ChangeInterval(); continue;
                    case "4": UseRandom = !UseRandom; Save(); continue; // 修改完要保存，嗯？
                    default: Tools.ShowError("无效的选择[2301292019]", false); continue;
                }
            }
        }
        public override void Print()
        {
            // API 名称

            Console.WriteLine($"使用的API = {Api.Name}");

            // 语言列表

            if (UseRandom) { Console.WriteLine("语言列表 = 随机"); }
            else { PrintLanListOptionStr(Lan_Start, Lan_List, Lan_End, Api.Languages); Console.WriteLine(); }

            // 翻译次数

            Console.WriteLine($"翻译次数 = {ExecuteTimes}");

            // 翻译间隔

            Console.WriteLine($"翻译次数 = {Interval}ms");
        }

        #endregion
        #region ==== 构造函数 ====

        public GoogleApiOption(GoogleAPI api)
        {
            Api = api;
            FilePath = Api.DirectoryPath + @"\Config.txt";
            Load(); // 从文件加载API设置
        }

        #endregion
        #region ==== 特有方法 ====

        /// <summary>
        /// 修改语言列表
        /// </summary>
        private void ChangeLanguageList()
        {
            // 输出当前语言列表

            PrintLanListOptionStr(Lan_Start, Lan_List, Lan_End, Api.Languages);

            // 输入新语言列表

            Console.Write("\n\n输入要指定的起始语言：");
            var input = ConsoleColors.ReadLineWithTempColors(); if (input == null) { Tools.ShowError("无效的输入[2301300826]", false); return; }
            var lan_start = input.Trim();

            Console.Write("\n输入要指定的中间语言，以英文逗号分割：");
            input = ConsoleColors.ReadLineWithTempColors(); if (input == null) { Tools.ShowError("无效的输入[2301300838]", false); return; }
            var lan_list_strs = input.Split(',').Select(x => x.Trim());
            var lan_list = new List<string>();
            foreach (var lan_str in lan_list_strs)
            {
                var lan = lan_str.Trim();
                lan_list.Add(lan);
            }

            Console.Write("\n输入要指定的结束语言：");
            input = ConsoleColors.ReadLineWithTempColors(); if (input == null) { Tools.ShowError("无效的输入[2301300842]", false); return; }
            var lan_end = input.Trim();

            // 显示预览

            Console.WriteLine("修改预览：");
            PrintLanListOptionStr(lan_start, lan_list, lan_end, Api.Languages);

            // 确认修改

            Console.Write("\n\n输入Y确认，输入其它取消. [Y]: ");
            input = ConsoleColors.ReadLineWithTempColors(); if (input == null) { Tools.ShowError("无效的输入[2301300856]", false); return; }
            if (input.Trim().ToLower() != "y") { Console.WriteLine("操作已取消"); return; }

            // 修改并保存

            Lan_Start = lan_start;
            Lan_List = lan_list;
            Lan_End = lan_end;
            Save();
            Console.WriteLine("\nAPI修改翻译列表成功");
        }

        #endregion
    }
}
