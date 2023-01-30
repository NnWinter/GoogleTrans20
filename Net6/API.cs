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
        public abstract List<Language> Languages { get; set; }
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
        public abstract string? TranslateByConfig();
    }
}