using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Net6.APIs.GoogleApi
{
    class ApiOption
    {
        /// <summary>
        /// 用于进行翻译的中间语言列表 (可以不含起始和结尾)
        /// </summary>
        public List<Language> LanguageList { get; private set; } = new List<Language>
        {
            new Language("af", null),
            new Language("cs", null),
            new Language("ja", "日文"),
            new Language("fr", "法语"),
            new Language("kn", null)
        };
        /// <summary>
        /// 翻译的次数 ( 如果是 4 个语言，则次数为 3 )
        /// </summary>
        public int ExecuteTimes { get; private set; } = 10;
        /// <summary>
        /// 调用 API 的等待间隔 ( 防止调用速度过快导致IP冷却 )
        /// </summary>
        public int Interval { get; private set; } = 2000;
        /// <summary>
        /// 是否使用随机语言
        /// </summary>
        public bool UseRandom { get; private set; } = true;
        /// <summary>
        /// 起始语言 (翻译前文本的语言)
        /// </summary>
        public Language Lan_Start { get; private set; } = new Language("zh", "简体中文");
        /// <summary>
        /// 结束语言 (翻译全部完成后应为本语言)
        /// </summary>
        public Language Lan_End { get; private set; } = new Language("zh", "简体中文");
        /// <summary>
        /// 所属的API (用于获取语言列表等)
        /// </summary>
        private GoogleAPI Api { get; init; }
        /// <summary>
        /// 设置文件的保存路径
        /// </summary>
        private string FilePath { get; init; }
        public ApiOption(GoogleAPI api)
        {
            Api = api;
            FilePath = Api.DirectoryPath + @"\Config.txt";
        }
        /// <summary>
        /// 修改设置
        /// </summary>
        public void Modify()
        {
            var loopFlag = true;
            while (loopFlag)
            {
                Console.Write(
                "\n==== 修改API设置 ====\n\n" +
                $" API - {Api.Name}\n\n" +
                $"  [0] 返回 API 界面\n\n" +
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
        /// <summary>
        /// 修改语言列表
        /// </summary>
        private void ChangeLanguageList()
        {
            // 输出当前语言列表

            Console.Write("\n起始语言 = "); Lan_Start.Print(); 
            Console.Write("\n中间语言 = "); 
            for(int i = 0; i< LanguageList.Count; i++) 
            { 
                LanguageList.ElementAt(i).Print(); 
                if(i < LanguageList.Count - 1) { Console.Write(", "); }
            }
            Console.Write("\n结尾语言 = "); Lan_End.Print(); 

            // 输入新语言列表

            Console.Write("\n\n输入要指定的起始语言：");
            var input = ConsoleColors.ReadLineWithTempColors(); if (input == null) { Tools.ShowError("无效的输入[2301300826]", false); return; }
            var lan_start_str = input.Trim();
            var lan_start = Api.Languages.FirstOrDefault(x => x.ShortName == lan_start_str);
            if (lan_start==null) 
            {
                lan_start = new Language(lan_start_str, null);
                Tools.ShowWarning($"语言 {lan_start_str} 不存在于 Languages.txt，请注意 API 是否支持该语言");
            }

            Console.Write("\n输入要指定的中间语言，以英文逗号分割：");
            input = ConsoleColors.ReadLineWithTempColors(); if (input == null) { Tools.ShowError("无效的输入[2301300838]", false); return; }
            var lan_list_strs = input.Split(',').Select(x=>x.Trim());
            var lan_list = new List<Language>();
            foreach(var lan_str in lan_list_strs)
            {
                var lan = Api.Languages.FirstOrDefault(x => x.ShortName == lan_str);
                if (lan == null)
                {
                    lan = new Language(lan_str, null);
                    Tools.ShowWarning($"语言 {lan_str} 不存在于 Languages.txt，请注意 API 是否支持该语言");
                }
                lan_list.Add(lan);
            }

            Console.Write("\n输入要指定的结尾语言：");
            input = ConsoleColors.ReadLineWithTempColors(); if (input == null) { Tools.ShowError("无效的输入[2301300842]", false); return; }
            var lan_end_str = input.Trim();
            var lan_end = Api.Languages.FirstOrDefault(x => x.ShortName == lan_end_str);
            if (lan_end == null)
            {
                lan_end = new Language(lan_end_str, null);
                Tools.ShowWarning($"语言 {lan_end_str} 不存在于 Languages.txt，请注意 API 是否支持该语言");
            }

            // 确认修改

            Console.Write("\n\n修改起始语言 = "); lan_start.Print();
            Console.Write("\n修改中间语言 = ");
            for (int i = 0; i < lan_list.Count; i++)
            {
                lan_list.ElementAt(i).Print();
                if (i < lan_list.Count - 1) { Console.Write(", "); }
            }
            Console.Write("\n修改结尾语言 = "); lan_end.Print();
            Console.Write("\n输入Y确认，输入其它取消. [Y]: ");
            input = ConsoleColors.ReadLineWithTempColors(); if (input == null) { Tools.ShowError("无效的输入[2301300856]", false); return; }
            if(input.Trim().ToLower() != "y"){ Console.WriteLine("操作已取消"); return; }

            Lan_Start = lan_start;
            LanguageList = lan_list;
            Lan_End = lan_end;
            Save();
            Console.WriteLine("已修改");
        }
        private void ChangeExecuteTimes()
        {
            Console.Write(
                $"\n当前翻译次数为 {ExecuteTimes}\n" +
                $"输入新的翻译次数 (短间隔频繁调用API可能导致冷却)\n\n>"
                );
            var input = ConsoleColors.ReadLineWithTempColors();
            if (input == null) { Tools.ShowError("无效的输入[2301292047]", false); return; }

            int newExecuteTimes;
            var isNum = int.TryParse(input, out newExecuteTimes);
            if (!isNum) { Tools.ShowError("输入不是有效的32位整数[2301292048]", false); return; }

            ExecuteTimes = newExecuteTimes;
            Save();
            Console.WriteLine($"API翻译次数修改成功\n当前次数为 {ExecuteTimes}");
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

            Interval = newInterval;
            Save();
            Console.WriteLine($"API调用间隔修改成功\n当前间隔为 {Interval}ms");
        }
        private void Save()
        {
            //try
            //{
            //    File.WriteAllText(FilePath, );
            //}
            //catch(Exception ex)
            //{
            //    Tools.ShowError($"保存设置文件错误\n{FilePath}\n{ex.Message}", true);
            //}
        }
    }
}
