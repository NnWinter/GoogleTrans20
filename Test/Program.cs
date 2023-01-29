
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

string json = "                          {\"type\":\"ZH_CN2EN\",\"errorCode\":0,\"elapsedTime\":0,\"translateResult\":[[{\"src\":\"你好\",\"tgt\":\"hello\"}]]}\r\n";


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





var s = ReadYoudaoJson(json);

Console.WriteLine(s);