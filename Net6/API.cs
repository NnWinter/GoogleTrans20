using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Net6
{
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
        public abstract List<Language>? Languages { get; set; }
        /// <summary>
        /// 使用 API 进行翻译
        /// </summary>
        /// <param name="fromLanguage">源语言</param>
        /// <param name="toLanguage">目标语言</param>
        /// <param name="text">要翻译的文本</param>
        /// <returns>翻译后的文本</returns>
        public abstract string? Translate(string fromLanguage, string toLanguage, string text);
    }
    /// <summary>
    /// 某 Google API
    /// </summary>
    public class GoogleTransAPI : API
    {
        public override string Name { get; init; } = "GoogleApi";
        public override string ApiUri { get; init; } = "https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}";
        public override List<Language>? Languages { get; set; }
        public GoogleTransAPI()
        {
            Languages = Language.ReadLanguagesFromFile(new FileInfo(@"APIs\GoogleApi\Languages.txt"));
            if(Languages == null)
            {
                Tools.ShowError($"加载 {Name} 的语言列表时发生了 \"语言列表为 null\" 的致命错误[2301290255]", true);
            }
        }

        public override string? Translate(string fromLanguage, string toLanguage, string text)
        {
            try
            {
                string uri = string.Format(ApiUri, fromLanguage, toLanguage, Uri.EscapeDataString(text));

                /* 部分内容为旧版代码，虽然做了部分修改，但还可以进一步优化 */

                HttpClient httpClient = new HttpClient();
                string result = httpClient.GetStringAsync(uri).Result;
                var jsonData = JsonSerializer.Deserialize<List<object>>(result);

                /* 这里可以加个异常处理，但是，咕咕咕 */
                if (!jsonData[0].GetType().Equals(typeof(JsonElement)))
                {
                    string eMsg = string.Format("GoogleAPI 翻译失败。[4.1] 源语言：{0:G} 目标语言：{1:G}\n{2}\n", fromLanguage, toLanguage,
                        "API 返回的结果不是有效的 JsonElement");
                    Tools.ShowError(eMsg, false);
                    return null;
                }
                if(((JsonElement)jsonData[0]).ValueKind != JsonValueKind.Array)
                {
                    string eMsg = string.Format("GoogleAPI 翻译失败。[4.2] 源语言：{0:G} 目标语言：{1:G}\n{2}\n", fromLanguage, toLanguage,
                        "JsonElement 不是 Array 类型");
                    Tools.ShowError(eMsg, false);
                    return null;
                }

                var transItem = ((JsonElement)jsonData[0])[0][1];

                return transItem.ToString();
            }
            catch (Exception ex)
            {
                string eMsg = string.Format("GoogleAPI 翻译失败。[4.3] 源语言：{0:G} 目标语言：{1:G}\n{2}\n", fromLanguage, toLanguage, ex.Message);
                Tools.ShowError(eMsg, false);
                return null;
            }
        }
    }
    /// <summary>
    /// 语言相关的类
    /// </summary>
    public class Language
    {
        /// <summary>
        /// 缩写 如: zh
        /// </summary>
        public string ShortName { get; init; }
        /// <summary>
        /// 全称 如: 简体中文
        /// </summary>
        public string? FullName { get; init; }
        /// <summary>
        /// 定义语言
        /// </summary>
        /// <param name="shortName">缩写</param>
        /// <param name="fullName">全称</param>
        public Language(string shortName, string? fullName)
        {
            ShortName = shortName;
            FullName = fullName;
        }
        /// <summary>
        /// 从文件中读取语言列表
        /// </summary>
        /// <param name="file">文件</param>
        /// <returns></returns>
        public static List<Language>? ReadLanguagesFromFile(FileInfo file)
        {
            try
            {
                var sr = file.OpenText();
                var languages = new List<Language>();
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("//")) { continue; } // 忽略注释行
                    try
                    {
                        if (!line.Contains(',')){ languages.Add(new Language(line, null)); }
                        else
                        {
                            var index = line.IndexOf(',');
                            var shortName = line.Substring(0, index);
                            var fullName = line.Substring(index+1);
                            languages.Add (new Language(shortName, fullName));
                        }
                    }
                    catch 
                    { 
                        continue; 
                    }
                }
                return languages;
            }
            catch (Exception ex)
            {
                string eMsg = string.Format("读取语言时产生了一个错误[2301290237]\n{0}\n", ex.Message);
                Tools.ShowError(eMsg, false);
                return null;
            }
        }
        /// <summary>
        /// 获得语言列表的纯文本
        /// </summary>
        /// <param name="languages">语言列表</param>
        /// <returns>文本化的语言列表</returns>
        public static string LanguageListSt(List<Language> languages)
        {
            var lanstr = new StringBuilder();
            foreach (var language in languages)
            {
                lanstr.AppendLine(
                    language.ShortName.PadRight(8) +
                    (language.FullName == null ? "" : " = " + language.FullName)
                );
            }
            return lanstr.ToString();
        }
        public override string ToString()
        {
            return $"缩写: {ShortName}, 全称: {(FullName == null ? "" : FullName)}";
        }
    }
}