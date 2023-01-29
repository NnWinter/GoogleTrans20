using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

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
    /// Google API
    /// </summary>
    public class GoogleAPI : API
    {
        public override string Name { get; init; } = "GoogleApi";
        public override string ApiUri { get; init; } = "https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}";
        public override List<Language>? Languages { get; set; }
        public GoogleAPI()
        {
            Languages = Language.ReadLanguagesFromFile(new FileInfo(@$"APIs\{Name}\Languages.txt"));
            if (Languages == null)
            {
                Tools.ShowError($"加载 {Name} 的语言列表时发生了 \"语言列表为 null\" 的致命错误[2301291205]", true);
            }
        }

        public override string? Translate(string fromLanguage, string toLanguage, string text)
        {
            try
            {
                string uri = string.Format(ApiUri, fromLanguage, toLanguage, Uri.EscapeDataString(text));

                // 使用 API 读取翻译结果
                HttpClient httpClient = new HttpClient();
                string result = httpClient.GetStringAsync(uri).Result;

                // 检查 API 的返回值
                if (string.IsNullOrWhiteSpace(result))
                {
                    string eMsg =
                        $"{Name} 翻译失败。[2301291159] 源语言：{fromLanguage} 目标语言：{toLanguage}\n" +
                        $"API 传回了空 数据，应确认是否能正常访问该网站，如 VPN 代理问题，网络连接等\n若问题依旧存在请向作者反馈\n";
                    Tools.ShowError(eMsg, false);
                    return null;
                }

                // 用于解析 有道的 Json 的函数
                Func<string, string?> ReadYoudaoJson = (json) =>
                {
                    try
                    {
                        var jsonData = (JArray?)JsonConvert.DeserializeObject(json);

#pragma warning disable CS8602, CS8604 // ↓↓↓ 这里可能会有 null，抛异常就行了，不用管警告
                        return jsonData[0][0][0].ToString();
#pragma warning restore CS8602, CS8604 // ↑↑↑

                    }
                    catch { return null; }
                };
                var trans = ReadYoudaoJson(result);

                // 检查翻译结果
                if (trans == null)
                {
                    string eMsg =
                        $"{Name} 翻译失败。[2301291155] 源语言：{fromLanguage} 目标语言：{toLanguage}\n" +
                        $"无法解析 API 传回的 JSON 数据 (遇到这个问题请向作者反馈)\n";
                    Tools.ShowError(eMsg, false);
                    return null;
                }

                return trans;
            }
            catch (Exception ex)
            {
                string eMsg = $"{Name} 翻译失败。[2301291208] 源语言：{fromLanguage} 目标语言：{toLanguage}\n{ex.Message}\n";
                Tools.ShowError(eMsg, false);
                return null;
            }
        }
    }
    /// <summary>
    /// 有道 API
    /// </summary>
    public class YoudaoAPI : API
    {
        public override string Name { get; init; } = "YoudaoApi";
        public override string ApiUri { get; init; } = "http://fanyi.youdao.com/translate?&doctype=json&type={0}2{1}&i={2}";
        public override List<Language>? Languages { get; set; }
        public YoudaoAPI()
        {
            Languages = Language.ReadLanguagesFromFile(new FileInfo(@$"APIs\{Name}\Languages.txt"));
            if (Languages == null)
            {
                Tools.ShowError($"加载 {Name} 的语言列表时发生了 \"语言列表为 null\" 的致命错误[2301290255]", true);
            }
        }
        public override string? Translate(string fromLanguage, string toLanguage, string text)
        {
            try
            {
                string uri = string.Format(ApiUri, fromLanguage, toLanguage, Uri.EscapeDataString(text));

                // 使用 API 读取翻译结果
                HttpClient httpClient = new HttpClient();
                string result = httpClient.GetStringAsync(uri).Result;

                // 检查 API 的返回值
                if(string.IsNullOrWhiteSpace(result))
                {
                    string eMsg =
                        $"Youdao 翻译失败。[2301291159] 源语言：{fromLanguage} 目标语言：{toLanguage}\n" +
                        $"API 传回了空 数据，应确认是否能正常访问该网站，如 VPN 代理问题，网络连接等\n若问题依旧存在请向作者反馈\n";
                    Tools.ShowError(eMsg, false);
                    return null;
                }

                // 用于解析 有道的 Json 的函数
                Func<string, string?> ReadYoudaoJson = (json) =>
                {
                    try
                    {
                        var jsonData = (JObject?)JsonConvert.DeserializeObject(json);

#pragma warning disable CS8602, CS8604 // ↓↓↓ 这里可能会有 null，抛异常就行了，不用管警告
                        return jsonData["translateResult"].ToArray()[0][0]["tgt"].ToString();
#pragma warning restore CS8602, CS8604 // ↑↑↑

                    }
                    catch { return null; }
                };
                var trans = ReadYoudaoJson(result);

                // 检查翻译结果
                if(trans == null)
                {
                    string eMsg = 
                        $"{Name} 翻译失败。[2301291155] 源语言：{fromLanguage} 目标语言：{toLanguage}\n" +
                        $"无法解析 API 传回的 JSON 数据 (遇到这个问题请向作者反馈)\n";
                    Tools.ShowError(eMsg, false);
                    return null;
                }

                return trans;
            }
            catch (Exception ex)
            {
                string eMsg = $"{Name} 翻译失败。[2301291206] 源语言：{fromLanguage} 目标语言：{toLanguage}\n{ex.Message}\n";
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
                        if (!line.Contains(',')) { languages.Add(new Language(line, null)); }
                        else
                        {
                            var index = line.IndexOf(',');
                            var shortName = line.Substring(0, index).Trim();
                            var fullName = line.Substring(index + 1).Trim();
                            languages.Add(new Language(shortName, fullName));
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