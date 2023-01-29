using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net6.APIs.GoogleApi
{
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
        private ApiOption Option = new ApiOption();
        public override void Config()
        {
            Option.Modify();
        }
    }
}
