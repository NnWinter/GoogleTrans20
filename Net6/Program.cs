using Net6;

API api = new GoogleTransAPI();

var str = api.Translate("zh", "en", "你好");

Console.WriteLine(str);