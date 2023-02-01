using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

HttpClient httpClient = new HttpClient();

var url = "http://fanyi.youdao.com/translate";

var text = "text";

var data = new Dictionary<string, string>{
        { "i", text },
        {"from", "AUTO"},
        {"to", "AUTO"},
        {"smartresult", "dict"},
        {"client", "fanyideskweb"},
        {"salt", "16081210430989"},
        {"doctype", "json"},
        {"version", "2.1"},
        {"keyfrom", "fanyi.web"},
        {"action", "FY_BY_CLICKBUTTION"}
    };

var jsonDic = JsonConvert.SerializeObject(data);

var httpcontent = new StringContent(jsonDic, Encoding.UTF8, "application/json");

var response = await httpClient.PostAsync(url, httpcontent);

var content = response.Content;

var bytes = await content.ReadAsByteArrayAsync();

var result = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

Console.WriteLine(result);