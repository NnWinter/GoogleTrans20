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
        /// 输入文本后用于结束输入的文本<br/>
        /// (如果一行文本内只有 ExitStr 就结束输入)
        /// </summary>
        public static string ExitStr { get; private set; } = "%%";
        private const string EXITSTR = "ExitStr";
        /// <summary>
        /// 是否在翻译时输出翻译过程
        /// </summary>
        public static bool ShowProcess { get; set; } = true;
        private const string SHOWPROCESS = "ShowProcess";

        /// <summary>
        /// 从文件中读取设置
        /// </summary>
        public static void Load()
        {
            if (!File.Exists(OPTIONS_FILE_PATH)) { Save(); }
            else
            {
                var lines = File.ReadAllLines(OPTIONS_FILE_PATH);

                Func<string, string> GetParamStr = (attribute) =>
                {
                    var param = lines.FirstOrDefault(x => x.StartsWith(attribute));
                    if (param == null)
                    {
                        Tools.ShowError(
                            $"[2301302356]\n" +
                            $"设置文件中无法找到参数 \"{attribute}\"\n" +
                            $"检查程序目录下的 \"{OPTIONS_FILE_PATH}\" 文件\n" +
                            $"如果无法修复问题，可以尝试删除上述文件并重新运行\n" +
                            $"这将会重置程序的全局设置为默认值\n",
                            true
                        );
                        return "";
                    }
                    try
                    {
                        // 消除注释
                        if (param.Contains("//")) { param = param[..param.IndexOf("//")]; }
                        // 拆分等号
                        return param[(param.IndexOf('=') + 1)..].Trim();
                    }
                    catch (Exception ex)
                    {
                        Tools.ShowError(
                            $"[2301302357]\n" +
                            $"参数 \"{attribute}\" 格式有误\n" +
                            $"检查程序目录下的 \"{OPTIONS_FILE_PATH}\" 文件\n" +
                            $"如果无法修复问题，可以尝试删除上述文件并重新运行\n" +
                            $"这将会重置程序的全局设置为默认值\n" +
                            $"错误信息: {ex.Message}",
                            true
                        );
                        return "";
                    }
                };

                ExitStr = GetParamStr(EXITSTR);

                var sp = GetParamStr(SHOWPROCESS);
                if (bool.TryParse(sp, out bool showprocess)) { ShowProcess = showprocess; }
                else
                {
                    Tools.ShowError(
                        $"[2301310005]\n" +
                        $"参数 \"{SHOWPROCESS}\" 格式有误\n" +
                        $"无法转换为 bool 类型 - {sp}",
                        true
                    );
                }
            }
        }
        /// <summary>
        /// 保存设置到文件
        /// </summary>
        public static void Save()
        {
            string ExitStrStr =
                "// 结束输入使用的文本\n" +
                $"{EXITSTR} = {ExitStr}";
            string ShowProcessStr =
                "// 是否显示翻译过程\n" +
                $"{SHOWPROCESS} = {(ShowProcess ? "true" : "false")}";

            string str = ExitStrStr + "\n" + ShowProcessStr;

            File.WriteAllText(OPTIONS_FILE_PATH, str);
        }
        /// <summary>
        /// 修改全局设置
        /// </summary>
        public static void Modify()
        {
            while (true)
            {
                Console.Write(
               "\n==== 修改全局设置 ====\n\n" +
               $"  [0] 返回主菜单\n\n" +
               $"  [1] 修改终止输入的文本 <{ExitStr}>\n" +
               $"  [2] 切换显示翻译过程 <{(ShowProcess ? "显示" : "隐藏")}>\n\n"
               );

                var input = ConsoleColors.ReadLineWithTempColors(); Console.WriteLine();
                if (input == null) { Tools.ShowError("无效的输入[2301310731]", false); return; }

                switch (input.Trim())
                {
                    case "0": return;
                    case "1": ChangeExitStr(); break;
                    case "2": ShowProcess = !ShowProcess; break;
                }

                // 保存修改
                Save();
            }
        }
        /// <summary>
        /// 修改终止文本
        /// </summary>
        private static void ChangeExitStr()
        {
            Console.WriteLine("输入要用于作为终止输入的文本\n");

            var input = ConsoleColors.ReadLineWithTempColors(); Console.WriteLine();
            if (input == null) { Tools.ShowError("无效的输入[2301310739]", false); return; }
            if (string.IsNullOrWhiteSpace(input)) { Tools.ShowError("终止文本不可为空[2301310741]", false); return; }

            ExitStr = input.Trim();
        }
        /// <summary>
        /// 输出全局设置信息
        /// </summary>
        public static void Print()
        {
            Console.WriteLine(
                $"停止输入的文本(在新行输入) = {ExitStr}\n" +
                $"是否显示翻译过程 = {(ShowProcess ? "是" : "否")}");
        }
    }
}
