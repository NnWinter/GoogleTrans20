using Net6;

GlobalOptions.Load();


API api = new YoudaoAPI();

var str = api.Translate("ZH_CN", "en", "你好");

Console.WriteLine(str);