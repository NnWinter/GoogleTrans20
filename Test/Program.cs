using PuppeteerSharp;
using static System.Net.Mime.MediaTypeNames;

using var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
var browser = await Puppeteer.LaunchAsync(new LaunchOptions
{
    Headless = true
});
var page = await browser.NewPageAsync();
await page.GoToAsync("https://fanyi.youdao.com/index.html");

// 先要点击选择语言，让语言变为2个




/*

var inpu = "div[id='js_fanyi_input']";

// 尝试清除内容并重写


string text1 = "哦哦哦哦哦哦哦哦哦哦~~~~~";
string text2 = "啊啊啊啊啊啊啊啊啊啊啊！！！！";

page.TypeAsync(inpu, text1).Wait();

Thread.Sleep(2000);

var content = await page.GetContentAsync();
Console.WriteLine(content);


// 尝试清空文本

var input = await page.QuerySelectorAsync(inpu);
await input.EvaluateFunctionAsync($"e => e.innerText = ' '");
Thread.Sleep(100);

//


page.TypeAsync(inpu, text2).Wait();

Thread.Sleep(2000);

*/

var content = await page.GetContentAsync();
Console.WriteLine(content);

Console.ReadLine();