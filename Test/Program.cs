using System.Text;


InputWindow win = new();

var result = win.ReadWithTextView();
Console.WriteLine(result);

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
        while (true)
        {
            var line = Console.ReadLine();
            if (line == null) { input.AppendLine(); continue; }
            if (line.Trim() == "%%") { break; }
            if (Flag)
            {
                input.AppendLine(line);
            }
        }
        return input.ToString();
    }
}