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
        public override Dictionary<string, string?> Languages { get; init; } = new Dictionary<string, string?>();
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
                return; // 虽然已经退出了，但是用来消除编译器警告
            }
            Languages = lanTemp;
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
                        var lines = jsonData[0];
                        var result = new StringBuilder();
                        foreach (var line in lines) {
                            result.Append(line[0].ToString()); // 这里似乎自带换行符，如果用AppendLine会导致多次换行
                        }
                        return result.ToString();
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
                // 将字典转换为数组以进行随机
                var dicArray = Languages.ToArray();
                
                // 列表添加起始语言
                var lan_list = new Queue<string>();
                lan_list.Enqueue(ApiOption.Lan_Start);
                var count = 0;

                // 列表添加中间语言
                var prev_index = -1; // 用于记录上一个语言的序号，避免重复
                while (count < ApiOption.ExecuteTimes - 1)
                {
                    var index = random.Next() % dicArray.Length;
                    if (index == prev_index) { continue; }
                    lan_list.Enqueue(dicArray[index].Key);
                    count++;
                }

                // 列表添加结尾语言
                lan_list.Enqueue(ApiOption.Lan_End);

                // 显示随机翻译的语言顺序
                Console.Write("随机翻译语言顺序: ");
                var lans = lan_list.ToArray();
                for (int i = 0; i < lans.Length; i++) { Language.Print(lans[i], Languages); if (i != lans.Length - 1) { Console.Write(", "); } }
                Console.WriteLine();

                // 翻译并返回结果
                return TranslateByLanQueue(lan_list, text);
            }
            else
            {
                // 列表添加起始语言
                var lan_list = new Queue<string>();
                lan_list.Enqueue(ApiOption.Lan_Start);
                var count = 0;

                // 列表添加中间语言
                var lan_array = ApiOption.Lan_List.ToArray();
                while (count < ApiOption.ExecuteTimes - 1)
                {
                    lan_list.Enqueue(lan_array[count % lan_array.Length]);
                    count++;
                }

                // 列表添加结尾语言
                lan_list.Enqueue(ApiOption.Lan_End);

                // 翻译并返回结果
                return TranslateByLanQueue(lan_list, text);
            }
        }
        /// <summary>
        /// 根据语言队列进行翻译
        /// </summary>
        /// <param name="queue">语言队列</param>
        /// <param name="text">要翻译的文本</param>
        /// <returns></returns>
        private string? TranslateByLanQueue(Queue<string> queue, string text)
        {
            string prev = queue.Dequeue();
            string? next;
            int count = 1;
            AppendProcessToFile(text, true);// 保存原文到本地过程
            while (queue.TryDequeue(out next))
            {
                var text_temp = Translate(prev, next, text);
                if (string.IsNullOrEmpty(text_temp)) { Tools.ShowError($"{Name} 翻译文本时返回了空文本[2301301124]\n源语言 = {prev}, 目标语言 = {next}", false); return null; }
                text = text_temp;
                AppendProcessToFile(text);// 保存过程到本地
                prev = next;
                if (GlobalOptions.ShowProcess) { Console.WriteLine("---- 翻译过程 ----\n" + text); } // 是否显示翻译过程
                else { Console.Write($"第 {count++} 次... "); }
                Thread.Sleep(ApiOption.Interval);
            }
            AppendResultToFile(text);// 保存结果到本地
            return text;
        }
    }
}
