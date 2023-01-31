using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net6
{
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
        public static Dictionary<string, string?>? ReadLanguagesFromFile(string path)
        {
            try
            {
                var sr = new StreamReader(path);
                var languages = new Dictionary<string, string?>();
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("//")) { continue; } // 忽略注释行
                    try
                    {
                        if (!line.Contains(',')) { languages.Add(line, null); }
                        else
                        {
                            var index = line.IndexOf(',');
                            var shortName = line.Substring(0, index).Trim();
                            var fullName = line.Substring(index + 1).Trim();
                            languages.Add(shortName, fullName);
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
        [Obsolete]
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
        /// <summary>
        /// 按照预设颜色和格式输出短文本以说明本语言<br />
        /// (不含换行)
        /// </summary>
        [Obsolete]
        public void Print()
        {
            Console.Write(ShortName);
            if (FullName != null)
            {
                ConsoleColors.WriteWithTempColors($"({FullName})", ConsoleColor.DarkGray, ConsoleColor.Black);
            }
        }
        /// <summary>
        /// 含颜色的输出语言功能
        /// </summary>
        /// <param name="language">缩写</param>
        /// <param name="fullname">语言全名</param>
        public static void Print(string language, string? fullname)
        {
            Console.Write(language);
            if (fullname != null)
            {
                ConsoleColors.WriteWithTempColors($"({fullname})", ConsoleColor.DarkGray, ConsoleColor.Black);
            }
        }
        /// <summary>
        /// 含颜色的输出语言功能(含查找)
        /// </summary>
        /// <param name="language">要输出的语言</param>
        /// <param name="languages">查找语言的列表</param>
        public static void Print(string language, Dictionary<string, string> languages)
        {
            Print(language, GetLanguageFullname(language, languages));
        }
        /// <summary>
        /// 从语言列表中查找语言缩写并返回对应的语言<br/>
        /// 如果语言不存在则提示警告并以 FullName = null 返回新的 Language
        /// </summary>
        /// <param name="language">要查找的语言</param>
        /// <param name="languages">语言列表</param>
        /// <returns>找到/创建的语言</returns>
        public static string? GetLanguageFullname(string language, Dictionary<string, string> languages)
        {
            bool success = languages.TryGetValue(language, out string? lan);
            if (!success || lan == null)
            {
                Tools.ShowWarning($"语言 {language} 不存在于 Languages.txt，请注意 API 是否支持该语言");
            }
            return lan;
        }

        /// <summary>
        /// 将语言列表转换为字符串
        /// </summary>
        /// <param name="list">要转换的语言列表</param>
        /// <returns>转换结果 如 "zh,en,fr,ja"</returns>
        public static string LanListToString(List<string> list)
        {
            var result = new StringBuilder();
            foreach (var language in list) { result.Append(language + ","); }
            result.Remove(result.Length-1,1); // 返回结果是引用类型，不用管
            return result.ToString();
        }
    }
}
