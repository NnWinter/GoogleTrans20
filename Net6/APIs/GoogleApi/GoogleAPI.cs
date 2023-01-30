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
        public override Language[] Languages { get; init; }
        public override string DirectoryPath { get; init; }
        public override ApiOption ApiOption { get; init; }
        public GoogleAPI()
        {
            DirectoryPath = @$"APIs\{Name}";
            ApiOption = new GoogleApiOption(this);

            var lanTemp = Language.ReadLanguagesFromFile(DirectoryPath + @"\Languages.txt");
            if (lanTemp == null)
            {
                Tools.ShowError($"加载 {Name} 的语言列表时发生了 \"语言列表为 null\" 的致命错误[2301291205]", true);
                // 虽然已经退出了，但是用来消除编译器警告
                Languages = Array.Empty<Language>(); return;
            }
            Languages = lanTemp.ToArray();
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
        public override string? TranslateByConfig(string text)
        {
            if (ApiOption.UseRandom)
            {
                // 创建随机语言列表
                var random = new Random((int)(DateTime.Now.Ticks % int.MaxValue));

                var lan_list = new Queue<Language>();
                lan_list.Enqueue(ApiOption.Lan_Start);
                var count = 0;

                var prev_index = -1; // 用于记录上一个语言的序号，避免重复
                while (count < ApiOption.ExecuteTimes - 1)
                {
                    var index = random.Next() % Languages.Length;
                    if (index == prev_index) { continue; }
                    lan_list.Enqueue(Languages[index]);
                    count++;
                }

                lan_list.Enqueue(ApiOption.Lan_End);
#if DEBUG
                var lans = lan_list.ToArray();
                foreach(var l in lans){ l.Print(); Console.Write(","); }
#endif
                return TranslateByLanQueue(lan_list, text);
            }
            else
            {
                var lan_list = new Queue<Language>();
                lan_list.Enqueue(ApiOption.Lan_Start);
                var count = 0;

                var prev = ApiOption.Lan_Start;
                var lan_array = ApiOption.Lan_List.ToArray();
                while (count < ApiOption.ExecuteTimes - 1)
                {
                    lan_list.Enqueue(lan_array[count % lan_array.Length]);
                    count++;
                }

                lan_list.Enqueue(ApiOption.Lan_End);
#if DEBUG
                var lans = lan_list.ToArray();
                foreach (var l in lans) { l.Print(); Console.Write(","); }
#endif
                return TranslateByLanQueue(lan_list, text);
            }
        }
        /// <summary>
        /// 根据语言队列进行翻译
        /// </summary>
        /// <param name="queue">语言队列</param>
        /// <param name="text">要翻译的文本</param>
        /// <returns></returns>
        private string? TranslateByLanQueue(Queue<Language> queue, string text)
        {
            Language prev = queue.Dequeue();
            Language? next;
            while (queue.TryDequeue(out next))
            {
                var text_temp = Translate(prev.ShortName, next.ShortName, text);
                if (string.IsNullOrEmpty(text_temp)) { Tools.ShowError($"{Name} 翻译文本时返回了空文本[2301301124]\n源语言 = {prev.ShortName}, 目标语言 = {next.ShortName}", false); return null; }
                text = text_temp;
                prev = next;
                if (GlobalOptions.ShowProcess) { Console.WriteLine("\n---- 翻译过程 ----\n" + text); } // 是否显示翻译过程
                Thread.Sleep(ApiOption.Interval);
            }
            return text;
        }
    }
}
