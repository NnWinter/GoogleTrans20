using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Net6
{
    /// <summary>
    /// API 接口抽象类
    /// </summary>
    public abstract class API
    {
        /// <summary>
        /// 翻译结果的保存路径 <br/>
        /// [不会吧不会吧，不会2023年了还有操作系统不支持中文路径的吧]
        /// </summary>
        public const string TRANS_RESULT_PATH = "翻译.txt";
        /// <summary>
        /// 翻译过程的保存路径 <br/>
        /// [不会吧不会吧，不会2023年了还有操作系统不支持中文路径的吧]
        /// </summary>
        public const string TRANS_PROCESS_PATH = "过程.txt";
        /// <summary>
        /// API 的名字
        /// </summary>
        public abstract string Name { get; init; }
        /// <summary>
        /// API 的 Uri
        /// </summary>
        public abstract string ApiUri { get; init; }
        /// <summary>
        /// 记录 API 中的语言列表
        /// </summary>
        public abstract Language[] Languages { get; init; }
        /// <summary>
        /// 储存 API 的设置
        /// </summary>
        public abstract ApiOption ApiOption { get; init; }
        /// <summary>
        /// API 的目录
        /// </summary>
        public abstract string DirectoryPath { get; init; }
        /// <summary>
        /// 使用 API 进行翻译
        /// </summary>
        /// <param name="fromLanguage">源语言</param>
        /// <param name="toLanguage">目标语言</param>
        /// <param name="text">要翻译的文本</param>
        /// <returns>翻译后的文本</returns>
        public abstract string? Translate(string fromLanguage, string toLanguage, string text);
        /// <summary>
        /// 根据设置进行翻译
        /// </summary>
        /// <returns>翻译后的文本</returns>
        public abstract string? TranslateByConfig(string text);
        /// <summary>
        /// 将翻译结果保存到本地(添加)
        /// </summary>
        /// <param name="text">要保存的文本</param>
        protected static void AppendResultToFile(string text)
        {
            string datetime = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string output =
                $"    ---- {datetime} ----\n" +
                $"{text}\n";
            try
            {
                File.AppendAllText(TRANS_RESULT_PATH, output);
            }
            catch(Exception ex)
            {
                // 可以不退出程序，虽然不能保存到本地，但控制台里能正常复制的话，就当是个普通的警告
                Tools.ShowError($"未能将翻译结果保存到文件[2301310858]\n{ex.Message}\n", false);
            }
        }
        /// <summary>
        /// 将翻译过程保存到本地(添加)
        /// </summary>
        /// <param name="text">要保存的文本</param>
        protected static void AppendProcessToFile(string text, bool isOrigin = false)
        {
            string datetime = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string output =
                $"    ---- {datetime} {(isOrigin?"原文 ":"")}----\n" +
                $"{text}\n";
            try
            {
                File.AppendAllText(TRANS_PROCESS_PATH, output);
            }
            catch (Exception ex)
            {
                // 可以不退出程序，虽然不能保存到本地，但控制台里能正常复制的话，就当是个普通的警告
                Tools.ShowError($"未能将翻译结果保存到文件[2301310858]\n{ex.Message}\n", false);
            }
        }
    }
}