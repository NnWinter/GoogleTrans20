using Net6;
using Net6.APIs.GoogleApi;
using Net6.APIs.YoudaoApi;

GlobalOptions.Load();
ConsoleColors.ResetDefault();


//Test.TestAPIs();

// 菜单
Stage stage = Stage.MAIN;
// API
API? api = null;

while (true)
{
    switch (stage)
    {
        // 主菜单 选择API
        case Stage.MAIN:
            {
                Console.Write(
                "\n==== 主菜单 ====\n\n" +
                " 选择API:\n" +
                $"  [1] Google API (谷歌翻译)\n" +
                $"  [2] Youdao API (有道翻译)\n" +
                $"  [3] 修改全局设置 (快捷键等)\n\n>"
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
        case Stage.API_MENU:
            {
                if (api == null) { Tools.ShowError("无效的API[2301291854]", false); stage = Stage.MAIN; continue; }
                Console.Write(
                "\n\n==== API ====\n" +
                $"  [0] 返回主菜单\n" +
                $"  [1] 翻译文本\n" +
                $"  [2] 修改设置\n\n>"
                );
                var input = ConsoleColors.ReadLineWithTempColors();

                // 无效的选择
                if (input == null) { Tools.ShowError("无效的输入[2301291855]", false); stage = Stage.MAIN; continue; }

                // 使用 API 进行操作 或 返回主菜单
                switch (input.Trim())
                {
                    case "0": stage = Stage.MAIN; continue;     // 返回主菜单
                    case "1": stage = Stage.TRANS; continue;    // 翻译
                    case "2": api.Config(); continue;           // 修改API设置
                    default: Tools.ShowError("无效的输入[2301291904]", false); continue;
                }
            }
    }
    Thread.Sleep(1000); // DEBUG
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