using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

var edge = new EdgeDriver();
try
{
    edge.Url = "https://fanyi.youdao.com/index.html";
    var input = edge.FindElement(By.Id("js_fanyi_input"));
    input.SendKeys("你好");

    Thread.Sleep(1000);

    var output = edge.FindElement(By.Id("js_fanyi_output"));

    string str = output.Text;
    Console.WriteLine(str);
}
finally
{
    edge.Quit();
}