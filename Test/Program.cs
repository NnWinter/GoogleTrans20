using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

var result = "[[[\"Hello\",\"你好\",null,null,10]],null,\"zh-CN\",null,null,null,null,[]]";

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

Console.WriteLine(trans);