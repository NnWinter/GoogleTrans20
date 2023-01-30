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
        public static List<Language>? ReadLanguagesFromFile(string path)
        {
            try
            {
                var sr = new StreamReader(path);
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
        /// <summary>
        /// 按照预设颜色和格式输出短文本以说明本语言<br />
        /// (不含换行)
        /// </summary>
        public void Print()
        {
            Console.Write(ShortName);
            if (FullName != null)
            {
                ConsoleColors.WriteWithTempColors($"({FullName})", ConsoleColor.DarkGray, ConsoleColor.Black);
            }
        }
    }
}
