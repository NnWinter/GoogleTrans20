using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace Net6
{
    internal class InputWindow
    {
        /// <summary>
        /// 在独立界面中读取用户的输入信息(多行)
        /// </summary>
        /// <param name="message">提示文本</param>
        /// <returns>用户输入的文本</returns>
        public static string? ReadWithTextView(string message)
        {
            string? result = null;
            //Application.UseSystemConsole = true;    // 不加的话中文文本会显示一个重叠框框 (可能是由于中文字符占位导致的)
            Application.Init();
            Colors.Base.Normal = Application.Driver.MakeAttribute(Color.White, Color.Black);    // 修改默认样式为白黑 (原本是白蓝)
            var inputWindow = new NnInputWindow(message);
            Application.Run(inputWindow);
            result = ((NnInputWindow)Application.Top).InputBox.Text.ToString();
            Application.Shutdown(); // 退出 Terminal 并还原之前的内容
            inputWindow.Dispose();
            return result;
        }
    }
    /// <summary>
    /// 使用 Terminal.Gui 这个 Nuget 实现在新界面让用户输入的功能<br/>
    /// 主要是能在运行结束后还原原本的内容，不影响程序使用
    /// 并且能监听按钮事件，不需要自己写监听器
    /// </summary>
    public class NnInputWindow : Window
    {
        public TextView InputBox;

        public NnInputWindow(string message)
        {
            // 标题
            Title = "输入框 (鼠标右键显示菜单, Paste = 粘贴, PageUp/PageDown 翻页)";

            var MessageLabel = new Label
            {
                X = Pos.Center(),
                Y = 1,
                Text = message
            };

            // 输入框
            InputBox = new TextView()
            {
                X = 2,
                Y = 2,
                // Fill remaining horizontal space
                Width = Dim.Fill() - 2,
                Height = Dim.Fill() - 4
            };

            // 确认按钮
            var SubmitButton = new Button()
            {
                Text = "确认",
                Y = Pos.Bottom(InputBox) + 1,
                X = Pos.Center(),   // 垂直居中
                IsDefault = true,
            };

            // 响应按钮点击事件 - 点击时退出
            SubmitButton.Clicked += () =>
            {
                Application.RequestStop();
            };

            // 添加组件到窗体
            Add(InputBox, SubmitButton);
        }
    }
}
