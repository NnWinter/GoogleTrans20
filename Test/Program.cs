using EO.WebBrowser;

//Thread th = new Thread(
//() => {
//    Form f1 = new Form();
//    WebBrowser web = new WebBrowser();
//    web.ScriptErrorsSuppressed = true;
//    f1.SetBounds(0, 0, 500, 500);
//    f1.Controls.Add(web);
//    web.Size = f1.ClientSize;
//    web.Navigate("http://www.google.com");
//    web.Refresh();
//    web.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler((sender, e) =>
//    {
//        if (web.ReadyState == WebBrowserReadyState.Complete)
//        {
//            web.Document.
//        }
//    });
//    Application.Run(f1);
//}
//);

//th.SetApartmentState(ApartmentState.STA);
//th.IsBackground = true;
//th.Start();

//Console.ReadLine();

//th.Interrupt();

//Create a ThreadRunner object
ThreadRunner threadRunner = new ThreadRunner();

//Create a WebView through the ThreadRunner
WebView webView = threadRunner.CreateWebView();

threadRunner.Send(() =>
{
    //Load Google's home page
    webView.LoadUrlAndWait("https://fanyi.youdao.com");

    // 对网页进行操作的测试
    //object obj = webView.EvalScript("document.getElementsByName('btnI')[0].value");
    //object obj2 = webView.EvalScript("document.getElementsByName('btnI')[0].Click");

    /*

    webView.EvalScript("document.getElementById('js_fanyi_input').innerText = 'Hello World';");

    var doc = webView.GetDOMWindow().document;
    var input = doc.getElementById("js_fanyi_input");
    var text = input.innerText;
    //input.InvokeFunction("input");

    var bstr = "transBtn";
    var transButton = doc.getElementsByTagName("a"); //.First(x=>x.className.Contains(bstr));

    foreach(var a in transButton)
    {
        Console.WriteLine("\n" + a.outerHTML + "\n");
    }
    //Console.WriteLine(transButton.outerHTML);

    Console.WriteLine(text);
    //button.InvokeFunction("click");
    //Capture screens-hot and save it to a file

    */

    webView.Capture().Save("WebScreenShot.png");
});