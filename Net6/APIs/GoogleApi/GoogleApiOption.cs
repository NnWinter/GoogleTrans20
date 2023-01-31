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
                    lanListPreview = new StringBuilder(Lan_Start.ShortName + ", ");
                    foreach (Language lan in Lan_List) { lanListPreview.Append(lan.ShortName + ", "); }
                    lanListPreview.Append(Lan_End.ShortName);
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
                    case "0": loopFlag = false; continue;
                    case "1": ChangeLanguageList(); continue;
                    case "2": ChangeExecuteTimes(); continue;
                    case "3": ChangeInterval(); continue;
                    case "4": UseRandom = !UseRandom; continue;
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
            else { Console.Write("语言列表 = "); PrintLanListOptionStr(Lan_Start, Lan_List, Lan_End); Console.WriteLine(); }

            // 翻译次数

            Console.WriteLine($"翻译次数 = {ExecuteTimes}");

            // 翻译间隔

            Console.WriteLine($"翻译次数 = {Interval}ms");
        }
        public override void Load()
        {
            if (!File.Exists(FilePath)) { Save(); }
            else
            {
                Tools.LoadParamFromFile();
            }
        }
        public override void Save()
        {
            try
            {
                // File.WriteAllText(FilePath, );
            }
            catch (Exception ex)
            {
                Tools.ShowError($"保存设置文件错误\n{FilePath}\n{ex.Message}", true);
            }
        }

        #endregion
        #region ==== 构造函数 ====

        public GoogleApiOption(GoogleAPI api)
        {
            Api = api;
            FilePath = Api.DirectoryPath + @"\Config.txt";
        }

        #endregion
        #region ==== 特有方法 ====

        /// <summary>
        /// 修改语言列表
        /// </summary>
        private void ChangeLanguageList()
        {
            // 输出当前语言列表

            PrintLanListOptionStr(Lan_Start, Lan_List, Lan_End);

            // 将语言文本转换为 Language 类的函数

            Func<string, Language> GetLan = (lanStr) =>
            {
                var lan = Api.Languages.FirstOrDefault(x => x.ShortName == lanStr);
                if (lan == null)
                {
                    lan = new Language(lanStr, null);
                    Tools.ShowWarning($"语言 {lanStr} 不存在于 Languages.txt，请注意 API 是否支持该语言");
                }
                return lan;
            };

            // 输入新语言列表

            Console.Write("\n\n输入要指定的起始语言：");
            var input = ConsoleColors.ReadLineWithTempColors(); if (input == null) { Tools.ShowError("无效的输入[2301300826]", false); return; }
            var lan_start = GetLan(input.Trim());

            Console.Write("\n输入要指定的中间语言，以英文逗号分割：");
            input = ConsoleColors.ReadLineWithTempColors(); if (input == null) { Tools.ShowError("无效的输入[2301300838]", false); return; }
            var lan_list_strs = input.Split(',').Select(x => x.Trim());
            var lan_list = new List<Language>();
            foreach (var lan_str in lan_list_strs)
            {
                var lan = GetLan(lan_str.Trim());
                lan_list.Add(lan);
            }

            Console.Write("\n输入要指定的结束语言：");
            input = ConsoleColors.ReadLineWithTempColors(); if (input == null) { Tools.ShowError("无效的输入[2301300842]", false); return; }
            var lan_end = GetLan(input.Trim());

            // 显示预览

            Console.WriteLine("修改预览：");
            PrintLanListOptionStr(lan_start, lan_list, lan_end);

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
        /// <summary>
        /// 修改翻译次数
        /// </summary>
        private void ChangeExecuteTimes()
        {
            Console.Write(
                $"\n当前翻译次数为 {ExecuteTimes}\n" +
                $"短间隔频繁调用API可能导致冷却\n" +
                $"输入新的翻译次数 (含最终输出时的翻译)\n\n"
                );
            var input = ConsoleColors.ReadLineWithTempColors();
            if (input == null) { Tools.ShowError("无效的输入[2301292047]", false); return; }

            var isNum = int.TryParse(input, out int newExecuteTimes);
            if (!isNum) { Tools.ShowError("输入不是有效的32位整数[2301292048]", false); return; }
            if (newExecuteTimes < 1) { Tools.ShowError("至少需要翻译1次[2301301038]", false); return; }

            ExecuteTimes = newExecuteTimes;
            Save();
            Console.WriteLine($"\nAPI翻译次数修改成功\n当前次数为 {ExecuteTimes}");
        }
        /// <summary>
        /// 修改翻译间隔
        /// </summary>
        private void ChangeInterval()
        {
            Console.Write(
                $"\n当前调用API的间隔时间为 {Interval}ms\n" +
                $"输入新的间隔 [ms] (短间隔频繁调用API可能导致冷却)\n\n"
                );
            var input = ConsoleColors.ReadLineWithTempColors();
            if (input == null) { Tools.ShowError("无效的输入[2301292027]", false); return; }

            var isNum = int.TryParse(input, out int newInterval);
            if (!isNum) { Tools.ShowError("输入不是有效的32位整数[2301292029]", false); return; }

            Interval = newInterval;
            Save();
            Console.WriteLine($"\nAPI调用间隔修改成功\n当前间隔为 {Interval}ms");
        }
        /// <summary>
        /// 输出语言列表
        /// </summary>
        /// <param name="start">起始语言</param>
        /// <param name="list">中间语言</param>
        /// <param name="end">结束语言</param>
        private static void PrintLanListOptionStr(Language start, List<Language> list, Language end)
        {
            Console.Write("\n起始语言 = "); start.Print();
            Console.Write("\n中间语言 = ");
            for (int i = 0; i < list.Count; i++)
            {
                list.ElementAt(i).Print();
                if (i < list.Count - 1) { Console.Write(", "); }
            }
            Console.Write("\n结束语言 = "); end.Print();
        }

        #endregion
    }
}
