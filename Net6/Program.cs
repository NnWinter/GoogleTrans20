using Net6;
using Net6.APIs.GoogleApi;
using Net6.APIs.YoudaoApi;
using System.Diagnostics;
using System.Text;

GlobalOptions.Load();           // 一定要预加载全剧设定
ConsoleColors.ResetDefault();   // 多次重复运行调试器可能会导致颜色不还原

Stage stage = Stage.MAIN;   // 菜单
API? api = null;            // API
var mainLoop = true;        // 循环flag
while (mainLoop)
{
    switch (stage)
    {
        // 退出程序
        case Stage.EXIT:
            mainLoop = false; break;
        // 主菜单 选择API
        case Stage.MAIN:
            {
                Console.Write(
                "\n==== 主菜单 ====\n\n" +
                " 选择API:\n" +
                $"  [1] Google API (谷歌翻译)\n" +
                $"  [2] Youdao API (有道翻译) [开发中]\n" +
                $"  [3] 修改全局设置 (终止文本等)\n\n" +
                $"  [0] 退出程序 ( 直接关掉√ )\n\n"
                );
                var apiSelection = ConsoleColors.ReadLineWithTempColors();

                // 无效的选择
                if (apiSelection == null) { Tools.ShowError("无效的输入[2301291852]", false); continue; }

                // 使用指定的 API
                switch (apiSelection.Trim())
                {
                    case "1": api = new GoogleAPI(); stage = Stage.API_MENU; continue;
                    case "2": api = new YoudaoAPI(); stage = Stage.API_MENU; continue;
                    case "3": GlobalOptions.Modify(); continue;
                    default: Tools.ShowError("无效的选择[2301291853]", false); continue;
                }
            }
        // API 选项菜单
        case Stage.API_MENU:
            {
                if (api == null) { Tools.ShowError("无效的API[2301291854]", false); stage = Stage.MAIN; continue; }
                Console.Write(
                "\n==== API ====\n\n" +
                $" API - {api.Name}\n\n" +
                $"  [0] 返回主菜单\n\n" +
                $"  [1] 翻译文本\n" +
                $"  [2] 修改设置\n\n"
                );
                var input = ConsoleColors.ReadLineWithTempColors();

                // 无效的选择
                if (input == null) { Tools.ShowError("无效的输入[2301291855]", false); stage = Stage.API_MENU; continue; }

                // 使用 API 进行操作 或 返回主菜单
                switch (input.Trim())
                {
                    case "0": stage = Stage.MAIN; continue;     // 返回主菜单
                    case "1": stage = Stage.TRANS; continue;    // 翻译
                    case "2": api.ApiOption.Modify(); continue; // 修改API设置
                    default: Tools.ShowError("无效的选择[2301291904]", false); continue;
                }
            }
        // 翻译输入界面
        case Stage.TRANS:
            {
                if (api == null) { Tools.ShowError("无效的API[2301301937]", false); stage = Stage.MAIN; continue; }

                // 显示API信息
                Console.WriteLine();
                api.ApiOption.Print();

                // 显示全局信息
                GlobalOptions.Print();

                // 用户输入
                Console.WriteLine("\n---- 输入要翻译的文本 ----");
                var input = Tools.ReadLines();
                if (input == null) { Tools.ShowError("输入的内容为 Null [2301302224]", false); stage = Stage.API_MENU; continue; }

                Console.WriteLine("\n---- 翻译信息 ----");
                var result = api.TranslateByConfig(input);

                Console.WriteLine("\n---- 翻译结果 ----\n\n");
                Console.WriteLine(result);

                stage = Stage.API_MENU;
                break;
            }
        // 怎么会跑到这呢？Bug！一定是Bug!
        case Stage.NULL:
        default: { Tools.ShowError("错误的程序状态[2301310752]", true); break; }
    }
}

Console.WriteLine();

enum Stage
{
    NULL = 0,       // 无效的阶段
    EXIT = 1,       // 退出程序
    MAIN = 2,       // 主菜单
    API_MENU = 3,   // API界面
    TRANS = 4       // 翻译界面
}