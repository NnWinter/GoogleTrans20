using System.Diagnostics;
using System.Text;

using Terminal.Gui;

Console.WriteLine("Hello World");

// 这一行很重要，不加的话中文文本会显示框框 (可能是由于中文字符占位导致的)
Application.UseSystemConsole = true; 

Application.Init();


Colors.Base.Normal = Application.Driver.MakeAttribute(Color.White, Color.Black);

Application.Run< ExampleWindow>();


Console.WriteLine($"Username: {((ExampleWindow)Application.Top).usernameText.Text}");

// Before the application exits, reset Terminal.Gui for clean shutdown
Application.Shutdown();

Console.WriteLine("Hello World");

Console.ReadLine();


// Defines a top-level window with border and title
public class ExampleWindow : Window
{
    public TextView usernameText;

    public ExampleWindow()
    {
        this.
        Title = "输入框 (鼠标右键显示菜单, Paste = 粘贴, PageUp/PageDown 翻页)";

        usernameText = new TextView()
        {
            X = 2,
            Y = 1,
            // Fill remaining horizontal space
            Width = Dim.Fill() - 2,
            Height = Dim.Fill() - 3
        };

        // Create login button
        var btnLogin = new Button()
        {
            Text = "确认",
            Y = Pos.Bottom(usernameText) +1,
            // center the login button horizontally
            X = Pos.Center(),
            IsDefault = true,
        };

        // When login button is clicked display a message popup
        btnLogin.Clicked += () => {
            Console.WriteLine("Test");

            Thread.Sleep(100);

            Application.RequestStop();
            
        };
        // Add the views to the Window
        Add(usernameText, btnLogin);
    }
}