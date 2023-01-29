using Net6.APIs.GoogleApi;
using Net6.APIs.YoudaoApi;

namespace Net6
{
    /// <summary>
    /// 用于自动化测试的类
    /// </summary>
    internal class Test
    {
        public static void TestAPIs()
        {
            // 测试 Google API
            Console.WriteLine("测试: 创建 Google API");
            API googleApi = new GoogleAPI();
            Console.WriteLine("测试: 创建 Google API - 完成");
            Console.WriteLine("测试: 使用 Google API 翻译 - 你好");
            Console.WriteLine("测试: 翻译结果 - " + googleApi.Translate("zh", "en", "你好"));

            // 测试 Youdao API
            Console.WriteLine("测试: 创建 Youdao API");
            API youdaoApi = new YoudaoAPI();
            Console.WriteLine("测试: 创建 Youdao API - 完成");
            Console.WriteLine("测试: 使用 Youdao API 翻译 - 你好");
            Console.WriteLine("测试: 翻译结果 - " + youdaoApi.Translate("ZH_CN", "EN", "你好"));
            
            
        }
    }
}
