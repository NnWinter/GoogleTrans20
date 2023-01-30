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
        public bool Flag = true;
        /// <summary>
        /// 读取用户的输入信息(多行)
        /// </summary>
        /// <returns>用户输入的文本</returns>
        public string? ReadWithTextView()
        {
            var input = new StringBuilder();
            while (Flag)
            {
                var line = Console.ReadLine();
                if (line == null) { input.AppendLine(); continue; }
                if (line.Trim() == GlobalOptions.ExitStr) { break; }
                input.AppendLine(line);
            }
            return input.ToString();
        }
        private async Task CheckCloseKey()
        {
            await new Task(() => { 
                while(Flag)
                {
                    if(Console.ReadKey().Key == GlobalOptions.ExitKey)
                    {
                        Flag = false;
                    }
                }
            });
        }
    }
}
