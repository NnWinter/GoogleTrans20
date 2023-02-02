using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net6
{
    public abstract class ApiOption
    {
        /// <summary>
        /// 起始语言 (翻译前文本的语言)
        /// </summary>
        public abstract string Lan_Start { get; protected set; }
        public const string LAN_START = "Lan_Start";
        /// <summary>
        /// 用于进行翻译的中间语言列表 (可以不含起始和结尾)
        /// </summary>
        public abstract List<string> Lan_List { get; protected set; }
        public const string LAN_LIST = "Lan_List";
        /// <summary>
        /// 结束语言 (翻译全部完成后应为本语言)
        /// </summary>
        public abstract string Lan_End { get; protected set; }
        public const string LAN_END = "Lan_End";
        /// <summary>
        /// 翻译的次数 ( 如果是 4 个语言，则次数为 3 )
        /// </summary>
        public abstract int ExecuteTimes { get; protected set; }
        public const string EXECUTE_TIMES = "ExecuteTimes";
        /// <summary>
        /// 调用 API 的等待间隔 ( 防止调用速度过快导致IP冷却 )
        /// </summary>
        public abstract int Interval { get; protected set; }
        public const string INTERVAL = "Interval";
        /// <summary>
        /// 是否使用随机语言
        /// </summary>
        public abstract bool UseRandom { get; protected set; }
        public const string USERANDOM = "UseRandom";
        /// <summary>
        /// 所属的API (用于获取语言列表等)
        /// </summary>
        public abstract API Api { get; init; }
        /// <summary>
        /// 设置文件的保存路径
        /// </summary>
        public abstract string FilePath { get; init; }
        /// <summary>
        /// 修改设置
        /// </summary>
        public abstract void Modify();
        /// <summary>
        /// 输出 API 的设置信息到控制台
        /// </summary>
        public abstract void Print();
        /// <summary>
        /// 从文件中加载设置 [同Save()，抽象类: 所以爱会消失，是吗? ]
        /// </summary>
        public void Load()
        {
            if (!File.Exists(FilePath)) { Save(); }
            else
            {
                try
                {
                    Lan_Start = Tools.LoadParamFromFile(FilePath, LAN_START);

                    var lans = Tools.LoadParamFromFile(FilePath, LAN_LIST).Split(',');
                    if (lans == null) { Tools.ShowError($"加载 {Api.Name} 的语言列表时出现了错误[2302010609]", true); return; }
                    Lan_List = lans.Select(x=>x.Trim()).ToList();

                    Lan_End = Tools.LoadParamFromFile(FilePath, LAN_END);

                    ExecuteTimes = int.Parse(Tools.LoadParamFromFile(FilePath, EXECUTE_TIMES));

                    Interval = int.Parse(Tools.LoadParamFromFile(FilePath, INTERVAL));

                    UseRandom = bool.Parse(Tools.LoadParamFromFile(FilePath, USERANDOM));
                }
                catch(Exception ex) 
                {
                    Tools.ShowError($"加载 {Api.Name} 的设置时出现了错误[2302010605]\n{ex.Message}", true);
                }
            }
        }
        /// <summary>
        /// 保存设置到文件<br/>
        /// [估计是这几天暴雨家里淹了没睡好，忘记这个方法可以在抽象类里直接共享了，这就省着写两遍了]
        /// </summary>
        public void Save()
        {
            var attributes = new Attribute[] {
                new Attribute(LAN_START, Lan_Start, "起始语言"),
                new Attribute(LAN_LIST, Language.LanListToString(Lan_List), "中间语言"),
                new Attribute(LAN_END, Lan_End, "结束语言"),

                new Attribute(EXECUTE_TIMES, ExecuteTimes.ToString(), "翻译次数(含结束语言)"),
                new Attribute(INTERVAL, Interval.ToString(), "翻译间隔(毫秒)"),
                new Attribute(USERANDOM, UseRandom.ToString(), "是否随机")
            };
            Tools.SaveParamsToFile(FilePath, attributes);
        }
        /// <summary>
        /// 修改翻译次数 [这俩玩意好像可以放抽象里直接用，不同API应该都通用]
        /// </summary>
        protected void ChangeExecuteTimes()
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
        /// 修改翻译间隔 [同 ChangeExecuteTimes()]
        /// </summary>
        protected void ChangeInterval()
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
        protected static void PrintLanListOptionStr(string start, List<string> list, string end, Dictionary<string, string?> languages)
        {
            Console.Write($"\n起始语言 = "); Language.Print(start, languages);
            Console.Write($"\n中间语言 = ");
            for (int i = 0; i < list.Count; i++)
            {
                Language.Print(list.ElementAt(i), languages);
                if (i < list.Count - 1) { Console.Write(", "); }
            }
            Console.Write("\n结束语言 = "); Language.Print(end, languages);
        }
    }
}
