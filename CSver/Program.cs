using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace GoogleTrans20CS
{
    class Program
    {
        //语言支持来自：https://cloud.google.com/translate/docs/languages
        static string[] lan = { "af", "sq", "am", "ar", "hy", "az", "eu", "be", "bn", "bs", "bg", "ca", "ceb", "zh", "zh-TW", "co", "hr", "cs", "da", "nl", "en", "eo", "et", "fi", "fr", "fy", "gl", "ka", "de", "el", "gu", "ht", "ha", "haw", "he", "hi", "hmn", "hu", "is", "ig", "id", "ga", "it", "ja", "jv", "kn", "kk", "km", "rw", "ko", "ku", "ky", "lo", "la", "lv", "lt", "lb", "mk", "mg", "ms", "ml", "mt", "mi", "mr", "mn", "my", "ne", "no", "ny", "or", "ps", "fa", "pl", "pt", "pa", "ro", "ru", "sm", "gd", "sr", "st", "sn", "sd", "si", "sk", "sl", "so", "es", "su", "sw", "sv", "tl", "tg", "ta", "tt", "te", "th", "tr", "tk", "uk", "ur", "ug", "uz", "vi", "cy", "xh", "yi", "yo", "zu" };
        static void Main(string[] args)
        {
            //说明
            Console.WriteLine("输入格式：在{}内填入括号内所标注的对应内容注意空格\r\n对应的语言可以到 https://cloud.google.com/translate/docs/languages 查询语言缩写。");
            //输入格式说明
            Console.WriteLine("1.翻译指令：trans from {原文语言} to {目标语言} with {0随机/1固定语种} do {翻译次数}" + Environment.NewLine + "例：trans from zh to en with 0 do 20"); //翻译部分
            Console.WriteLine("2.列出所有语言缩写：list_lan");
            Console.WriteLine();
            string input = Console.ReadLine();
            Random rand = new Random(DateTime.Now.Millisecond);
            //进行判断
            //  翻译功能
            if (input.Contains("trans") && !input.Contains("list_lan"))
            {
                try
                {
                    string[] param = input.Split(' ');
                    Func<string, string> find = (f) =>
                     {
                         for (int i = 0; i < param.Length; i++)
                         {
                             if (param[i] == f) { return param[i + 1]; }
                         }
                         return null;
                     };
                    string from = find("from");
                    string to = find("to");
                    int with = find("with") == "0" ? 0 : (find("with") == "1" ? 1 : -1);
                    int times = int.Parse(find("do"));
                    List<string> trans_his = new List<string>();
                    //随机翻译
                    if (with == 0)
                    {
                        //输入文本
                        Console.WriteLine("输入要翻译的文本，输入q结束");
                        Console.WriteLine("");
                        Console.WriteLine("=====文本区=====");
                        StringBuilder sb = new StringBuilder();
                        string line = "";
                        while ((line = Console.ReadLine()) != "q")
                        {
                            sb.AppendLine(line);
                        }
                        Console.WriteLine("=====文本区=====");
                        string text = sb.ToString();
                        //多次翻译
                        for (int i = 0; i < times - 1; i++)
                        {
                            string rlan = lan[rand.Next(0, lan.Length)];
                            text = TranslateText(text, from, rlan);
                            from = rlan;
                            trans_his.Add(from);
                        }
                        Console.WriteLine("");
                        Console.Write("=====使用语言=====" + Environment.NewLine + "源语言->"); foreach (string l in trans_his) { Console.Write(l + "->"); }; Console.WriteLine("目标语言" + Environment.NewLine + "=====使用语言=====");
                        //最终转换为
                        text = TranslateText(text, from, to);
                        Console.WriteLine("");
                        Console.WriteLine("=====翻译后=====" + Environment.NewLine + text + Environment.NewLine + "=====翻译后=====");
                    }
                    //固定翻译
                    else if (with == 1)
                    {
                        Console.WriteLine("=====固定翻译=====");
                        Console.WriteLine("输入要使用的语言，用空格分割");
                        Console.WriteLine("例：zh en zh-TW");
                        Console.WriteLine("表示翻译过程为 初始语言->简体中文->英文->繁体中文->输出语言");
                        Console.WriteLine("若翻译次数 do 大于语种数量，则会循环切换如： ");
                        Console.WriteLine("初始语言-简体中文->英文->繁体中文->简体中文->英文->输出语言");
                        int count = 0;
                        try
                        {
                            string[] clans = Console.ReadLine().Split(' ');
                            //输入文本
                            Console.WriteLine("输入要翻译的文本，输入q结束");
                            Console.WriteLine("");
                            Console.WriteLine("=====文本区=====");
                            StringBuilder sb = new StringBuilder();
                            string line = "";
                            while ((line = Console.ReadLine()) != "q")
                            {
                                sb.AppendLine(line);
                            }
                            Console.WriteLine("=====文本区=====");
                            string text = sb.ToString();
                            //多次翻译
                            for (int i = 0; i < times - 1; i++)
                            {
                                string clan = clans[i % clans.Length];
                                text = TranslateText(text, from, clan);
                                from = clan;
                                trans_his.Add(from);
                            }
                            Console.WriteLine("");
                            Console.Write("=====使用语言=====" + Environment.NewLine + "源语言->"); foreach (string l in trans_his) { Console.Write(l + "->"); }; Console.WriteLine("目标语言" + Environment.NewLine + "=====使用语言=====");
                            //最终转换为
                            text = TranslateText(text, from, to);
                            Console.WriteLine("");
                            Console.WriteLine("=====翻译后=====" + Environment.NewLine + text + Environment.NewLine + "=====翻译后=====");
                        }
                        catch
                        {
                            Console.WriteLine("在进行第{0:D}次翻译时失败，可能含有无效语种", count);
                        }
                    }
                    else
                    {
                        Console.WriteLine("未知的命令，请输入正确的指令。[3] 未知的翻译方式 with " + find("with"));
                    }
                }
                catch
                {
                    Console.WriteLine("未知的命令，请输入正确的指令。[2] trans指令参数无效");
                }
            }
            //  列出语言
            else if (!input.Contains("trans") && input.Contains("list_lan"))
            {
                Console.WriteLine("=====列表=====");
                foreach (string l in lan)
                {
                    Console.WriteLine(l);
                }
                Console.WriteLine("=====列表=====");
            }
            else
            {
                Console.WriteLine("未知的命令，请输入正确的指令。[1] 未找到有效指令");
            }
            Console.WriteLine("");
            Console.WriteLine("按回车或关闭窗口退出程序");
            Console.ReadLine();
        }
        public static string TranslateText(string input, string from, string to)
        {
            string translation = "";
            try
            {
                string url = String.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}", from, to, Uri.EscapeUriString(input));
                HttpClient httpClient = new HttpClient();
                string result = httpClient.GetStringAsync(url).Result;
                var jsonData = new JavaScriptSerializer().Deserialize<List<dynamic>>(result);
                var translationItems = jsonData[0];
                foreach (object item in translationItems)
                {
                    IEnumerable translationLineObject = item as IEnumerable;
                    IEnumerator translationLineString = translationLineObject.GetEnumerator();
                    translationLineString.MoveNext();
                    translation += string.Format("{0}", Convert.ToString(translationLineString.Current));
                }
                if (translation.Length > 1) { translation = translation.Substring(1); };
            }
            catch { Console.WriteLine("GoogleAPI翻译失败。[4] 源语言：{0:G} 目标语言：{0:G}", from, to); }

            return translation;
        }
    }
}
