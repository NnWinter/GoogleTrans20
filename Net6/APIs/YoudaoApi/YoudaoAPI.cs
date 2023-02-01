using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System;

namespace Net6.APIs.YoudaoApi
{
    /// <summary>
    /// 有道 API
    /// </summary>
    public class YoudaoAPI : API
    {
        public override string Name { get; init; } = "YoudaoApi";
        public override string ApiUri { get; init; } = "http://fanyi.youdao.com/translate?&doctype=json&type={0}2{1}&i={2}";
        public override Dictionary<string, string?> Languages { get; init; } = new Dictionary<string, string?>();
        public override string DirectoryPath { get; init; }
        public override ApiOption ApiOption { get; init; }
        public YoudaoAPI()
        {
            DirectoryPath = @$"APIs\{Name}";
            ApiOption = new YoudaoApiOption(this);

            var lanTemp = Language.ReadLanguagesFromFile(DirectoryPath + @"\Languages.txt");
            if (lanTemp == null)
            {
                Tools.ShowError($"加载 {Name} 的语言列表时发生了 \"语言列表为 null\" 的致命错误[2301301031]", true);
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
                        $"Youdao 翻译失败。[2302010508] 源语言：{fromLanguage} 目标语言：{toLanguage}\n" +
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
                string eMsg = $"{Name} 翻译失败。[2301291206] 源语言：{fromLanguage} 目标语言：{toLanguage}\n{ex.Message}\n";
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

                // 将字典转换为数组以进行随机 (不含中文)
                var zh = "ZH_CN";
                var dicArray_NoZh = Languages.Where(x=>x.Key!=zh).ToArray();

                // 列表添加起始语言
                var lan_list = new Queue<string>();
                lan_list.Enqueue(ApiOption.Lan_Start);
                var count = 0;

                // 列表添加中间语言
                var prev_index = -1; // 用于记录上一个语言非中文的序号，避免重复
                var isZh = ApiOption.Lan_Start == zh; //上一个语言是否为中文

                while (count < ApiOption.ExecuteTimes - 1)
                {
                    // 如果是中文，就从非中文语言中随机一个
                    if (isZh)
                    {
                        var index = random.Next() % dicArray_NoZh.Length;
                        if (index == prev_index) { continue; }

                        lan_list.Enqueue(dicArray_NoZh[index].Key);
                        isZh = false;
                        count++;
                    }
                    // 如果不是中文，就添加中文
                    else
                    {
                        lan_list.Enqueue(zh);
                        isZh = true;
                        count++;
                    }
                }

                // 列表添加结尾语言
                var isEndZh = ApiOption.Lan_End == zh; // 结尾语言是否为中文
                if(isZh ^ isEndZh) // 异或 -> 中中=false, 中外=true, 外外=false
                {
                    lan_list.Enqueue(ApiOption.Lan_End); 
                }
                else // 这表示中间语言的结尾和结束语言相同了
                {
                    Tools.ShowWarning(
                        "\n翻译次数与YoudaoAPI的规则不符[2302020404]\n" +
                        "中间语言的结尾与结束语言不可同为中文或外语\n" +
                        "将额外添加一个中间层\n");
                    // 如果是以非中文结尾，转换为中文再转换为外语
                    if (!isZh) 
                    { 
                        lan_list.Enqueue(zh);
                        lan_list.Enqueue(ApiOption.Lan_End);
                    }
                    // 如果不是以中文结尾，转换为随机外语再转换为中文
                    else
                    {
                        var index = random.Next() % dicArray_NoZh.Length;
                        lan_list.Enqueue(dicArray_NoZh[index].Key);
                    }
                }

                // 显示随机翻译的语言顺序
                Console.WriteLine("随机翻译语言顺序: ");
                var lans = lan_list.ToArray();
                for (int i = 0; i < lans.Length; i++) { Language.Print(lans[i], Languages); if (i != lans.Length - 1) { Console.Write(", "); } }

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
        /// 根据语言队列进行翻译 [啊没错，这个也是直接从 GoogleAPI 那里搬过来的，诶嘿]
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
            Console.WriteLine();
            while (queue.TryDequeue(out next))
            {
                var text_temp = Translate(prev, next, text);
                if (string.IsNullOrEmpty(text_temp)) { Tools.ShowError($"{Name} 翻译文本时返回了空文本[2302010633]\n源语言 = {prev}, 目标语言 = {next}", false); return null; }
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
