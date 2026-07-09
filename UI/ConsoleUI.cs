using ConsoleGameFramework.Common;
using System.Text;
using ConsoleGameFramework.Models;
using static ConsoleGameFramework.Common.Enums;
using static ConsoleGameFramework.Common.Constants;
using static ConsoleGameFramework.Common.UtilityExtension;
using static ConsoleGameFramework.Common.Utility;
using ConsoleGameFramework.Core;
namespace ConsoleGameFramework.UI;

/// <summary>
/// 콘솔 출력과 입력을 담당하는 공용 UI 클래스입니다.
///
/// 핵심 설계:
/// 1. 각 화면은 ConsoleUI.Clear()를 호출하면서 새 프레임을 시작합니다.
/// 2. WriteTitle, WriteBox, WriteMenu 같은 출력 함수는 곧바로 콘솔에 쓰지 않고 내부 버퍼에 쌓습니다.
/// 3. ConsoleUI.Present()가 호출되면 버퍼에 쌓인 내용을 콘솔 화면에 한 번에 반영합니다.
///
/// 이 방식은 Console.Clear()를 매 화면마다 직접 호출하는 방식보다 깜빡임이 적습니다.
/// 특히 미로처럼 키 입력마다 화면을 다시 그리는 장면에서 더 부드럽게 보입니다.
/// </summary>
public static class ConsoleUI
{
    private const int DefaultWidth = 80;
    private const int MinWidth = 60;
    private const int MaxWidth = 120;
    private const int MapCellWidth = 2;

    // 프레임 버퍼는 화면에 출력할 줄과 색상 조각을 메모리에 먼저 모아두는 공간입니다.
    private static readonly List<List<ConsoleSpan>> _frameLines = new List<List<ConsoleSpan>>();
    private static List<ConsoleSpan> _currentLine = new List<ConsoleSpan>();
    private static bool _isFrameOpen;
    private static int _lastPresentedLineCount;
    private static bool _isInitialized;

    private readonly record struct ConsoleSpan(string Text, ConsoleColor? Foreground, ConsoleColor? Background);

    /// <summary>
    /// 콘솔 기본 상태를 초기화합니다.
    /// </summary>
    public static void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;

        try
        {
            Console.CursorVisible = false;
        }
        catch
        {
            // 일부 실행 환경에서는 커서 상태 변경이 지원되지 않을 수 있습니다.
        }
    }

    /// <summary>
    /// 프로그램 종료 시 콘솔 상태를 복구합니다.
    /// </summary>
    public static void Shutdown()
    {
        Present();

        try
        {
            Console.ResetColor();
            Console.CursorVisible = true;
        }
        catch
        {
            // 콘솔 복구에 실패해도 프로그램 종료는 계속 진행합니다.
        }
    }

    /// <summary>
    /// 콘솔 창 너비를 안전하게 가져옵니다.
    /// 일부 실행 환경에서는 Console.WindowWidth 접근이 실패할 수 있으므로 예외 처리를 합니다.
    /// </summary>
    public static int SafeWidth
    {
        get
        {
            try
            {
                return Math.Clamp(Console.WindowWidth - 1, MinWidth, MaxWidth);
            }
            catch
            {
                return DefaultWidth;
            }
        }
    }

    /// <summary>
    /// 새 화면 프레임을 시작합니다.
    /// 이름은 Clear이지만 실제로 즉시 화면을 지우지 않고 버퍼만 비웁니다.
    /// 화면은 Present()에서 한 번에 갱신됩니다.
    /// </summary>
    public static void Clear()
    {
        BeginFrame();
    }

    /// <summary>
    /// 더블 버퍼링용 새 프레임을 엽니다.
    /// Clear()와 동일하지만 의미를 명확히 하고 싶을 때 사용할 수 있습니다.
    /// </summary>
    public static void BeginFrame()
    {
        _frameLines.Clear();
        _currentLine = new List<ConsoleSpan>();
        _isFrameOpen = true;
    }

    /// <summary>
    /// 버퍼에서 일부분을 삭제, 범위가 버퍼 크기보다 클 경우 전체 삭제
    /// </summary>
    /// <param name="range">삭제 범위</param>
    public static void ClearRange(int range)
    {
        if (range >= _frameLines.Count)
        {
            _frameLines.Clear();
            return;
        }

        int startIndex = _frameLines.Count - range;

        _frameLines.RemoveRange(startIndex, range);
    }
    /// <summary>
    /// 버퍼에 쌓인 한 프레임을 콘솔 화면에 반영합니다.
    /// Console.Clear() 대신 커서를 맨 위로 옮긴 뒤 줄 단위로 덮어써서 깜빡임을 줄입니다.
    /// </summary>
    public static void Present()
    {
        if (!_isFrameOpen)
        {
            return;
        }

        if (_currentLine.Count > 0)
        {
            CommitCurrentLine();
        }

        int width = SafeWidth;
        int lineCount = _frameLines.Count;
        int occupiedLineCount = Math.Max(_lastPresentedLineCount, GetCursorTopOrZero() + 1);
        int linesToClear = Math.Max(0, occupiedLineCount - lineCount);

        try
        {
            Console.SetCursorPosition(0, 0);
        }
        catch
        {
            try
            {
                Console.Clear();
            }
            catch
            {
                Console.WriteLine();
            }
        }

        for (int i = 0; i < lineCount; i++)
        {
            WriteLineToConsole(_frameLines[i], width);
        }

        // 이전 프레임보다 현재 프레임이 짧으면 아래쪽에 남은 글자를 공백으로 덮어씁니다.
        for (int i = 0; i < linesToClear; i++)
        {
            Console.Write(new string(' ', width));
            Console.WriteLine();
        }

        try
        {
            Console.SetCursorPosition(0, lineCount);
            Console.CursorVisible = false;
        }
        catch
        {
            // 커서 이동 실패는 출력 자체에는 영향을 주지 않습니다.
        }

        Console.ResetColor();
        _lastPresentedLineCount = lineCount;
        _isFrameOpen = false;
    }

    private static int GetCursorTopOrZero()
    {
        try
        {
            return Console.CursorTop;
        }
        catch
        {
            return 0;
        }
    }

    private static void WriteLineToConsole(IReadOnlyList<ConsoleSpan> line, int width)
    {
        int displayWidth = 0;

        foreach (ConsoleSpan span in line)
        {
            WriteDirect(span.Text, span.Foreground, span.Background);
            displayWidth += GetDisplayWidth(span.Text);
        }

        int padding = Math.Max(0, width - displayWidth);
        Console.ResetColor();
        Console.Write(new string(' ', padding));
        Console.WriteLine();
    }

    private static void WriteDirect(string text, ConsoleColor? foreground = null, ConsoleColor? background = null)
    {
        ConsoleColor oldForeground = Console.ForegroundColor;
        ConsoleColor oldBackground = Console.BackgroundColor;

        try
        {
            if (foreground.HasValue)
            {
                Console.ForegroundColor = foreground.Value;
            }

            if (background.HasValue)
            {
                Console.BackgroundColor = background.Value;
            }

            Console.Write(text);
        }
        finally
        {
            Console.ForegroundColor = oldForeground;
            Console.BackgroundColor = oldBackground;
        }
    }

    private static void CommitCurrentLine()
    {
        _frameLines.Add(_currentLine);
        _currentLine = new List<ConsoleSpan>();
    }

    private static void Append(string text, ConsoleColor? foreground = null, ConsoleColor? background = null)
    {
        string[] parts = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].Length > 0)
            {
                _currentLine.Add(new ConsoleSpan(parts[i], foreground, background));
            }

            if (i < parts.Length - 1)
            {
                CommitCurrentLine();
            }
        }
    }

    /// <summary>
    /// 일반 문자열을 출력합니다.
    /// 프레임이 열려 있으면 버퍼에 쌓고, 프레임이 닫혀 있으면 즉시 출력합니다.
    /// </summary>
    public static void Write(string text)
    {
        if (_isFrameOpen)
        {
            Append(text);
            return;
        }

        Console.Write(text);
    }

    /// <summary>
    /// 일반 문자열을 출력하고 줄을 바꿉니다.
    /// </summary>
    public static void WriteLine(string text = "")
    {
        if (_isFrameOpen)
        {
            Append(text);
            CommitCurrentLine();
            return;
        }

        Console.WriteLine(text);
    }

    /// <summary>
    /// 색상을 적용한 뒤 출력하고, 출력이 끝나면 원래 색상으로 복구합니다.
    /// </summary>
    public static void WriteColored(string text, ConsoleColor? foreground = null, ConsoleColor? background = null, bool newLine = true)
    {
        if (_isFrameOpen)
        {
            Append(text, foreground, background);

            if (newLine)
            {
                CommitCurrentLine();
            }

            return;
        }

        WriteDirect(text, foreground, background);

        if (newLine)
        {
            Console.WriteLine();
        }
    }

    public static void WriteTitle(string title, string subtitle = "")
    {
        WriteRule('═', ConsoleColor.DarkCyan);
        WriteCentered(title, ConsoleColor.Cyan);

        if (!string.IsNullOrWhiteSpace(subtitle))
        {
            WriteCentered(subtitle, ConsoleColor.DarkGray);
        }

        WriteRule('═', ConsoleColor.DarkCyan);
        WriteLine();
    }

    public static void WriteSubtitle(string text)
    {
        WriteLine();
        WriteColored($"▶ {text}", ConsoleColor.Yellow);
        WriteRule('─', ConsoleColor.DarkGray, 60);
    }

    public static void WriteRule(char character = '─', ConsoleColor color = ConsoleColor.DarkGray, int maxWidth = 100)
    {
        int width = Math.Min(SafeWidth, maxWidth);
        WriteColored(new string(character, width), color);
    }

    public static void WriteCentered(string text, ConsoleColor color = ConsoleColor.White)
    {
        int textWidth = GetDisplayWidth(text);
        int padding = Math.Max(0, (SafeWidth - textWidth) / 2);
        WriteColored(new string(' ', padding) + text, color);
    }

    /// <summary>
    /// 여러 줄을 테두리 박스로 출력합니다.
    /// 안내문, 상태창, 퀘스트 설명 등에 사용하기 좋습니다.
    /// </summary>
    public static void WriteBox(IEnumerable<string> lines, string title = "", ConsoleColor borderColor = ConsoleColor.DarkGray)
    {
        List<string> lineList = lines.ToList();
        int maxLineWidth = lineList.Count == 0 ? 0 : lineList.Max(GetDisplayWidth);
        int titleWidth = GetDisplayWidth(title);
        int contentWidth = Math.Max(maxLineWidth, titleWidth);

        // 너무 좁거나 너무 넓은 박스가 되지 않도록 범위를 제한합니다.
        contentWidth = Math.Clamp(contentWidth, 20, Math.Min(90, SafeWidth - 4));

        WriteColored($"┌{new string('─', contentWidth + 2)}┐", borderColor);

        if (!string.IsNullOrWhiteSpace(title))
        {
            WriteColored("│ ", borderColor, null, false);
            WriteColored(Fit(title, contentWidth), ConsoleColor.Cyan, null, false);
            WriteColored(" │", borderColor);
            WriteColored($"├{new string('─', contentWidth + 2)}┤", borderColor);
        }

        if (lineList.Count == 0)
        {
            WriteColored("│ ", borderColor, null, false);
            Write(Fit("내용이 없습니다.", contentWidth));
            WriteColored(" │", borderColor);
        }
        else
        {
            foreach (string line in lineList)
            {
                WriteColored("│ ", borderColor, null, false);
                Write(Fit(line, contentWidth));
                WriteColored(" │", borderColor);
            }
        }

        WriteColored($"└{new string('─', contentWidth + 2)}┘", borderColor);
        WriteLine();
    }

    /// <summary>
    /// 메뉴를 보기 좋은 형식으로 출력합니다.
    /// </summary>
    public static void WriteMenu(IEnumerable<MenuOption> options, string title = "메뉴")
    {
        WriteSubtitle(title);

        foreach (MenuOption option in options)
        {
            ConsoleColor color = option.IsEnabled ? ConsoleColor.White : ConsoleColor.DarkGray;
            string label = $" {option.Number,2}. {option.Label}";

            WriteColored(label, color, null, false);

            if (!string.IsNullOrWhiteSpace(option.Description))
            {
                WriteColored($" - {option.Description}", ConsoleColor.DarkGray, null, false);
            }

            WriteLine();
        }

        WriteLine();
    }

    /// <summary>
    /// 메뉴 번호를 입력받습니다.
    /// 잘못된 입력은 다시 입력하게 만들어 프로그램이 멈추지 않도록 합니다.
    /// </summary>
    public static int ReadMenuChoice(IEnumerable<MenuOption> options, string prompt = "선택")
    {
        Present();

        List<MenuOption> enabledOptions = options.Where(option => option.IsEnabled).ToList();
        HashSet<int> validNumbers = enabledOptions.Select(option => option.Number).ToHashSet();

        while (true)
        {
            WriteColored($"{prompt} > ", ConsoleColor.Green, null, false);
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int number) && validNumbers.Contains(number))
            {
                return number;
            }

            WriteToast("메뉴에 있는 번호를 입력하세요.", ToastType.Warning);
        }
    }

    public static string ReadString(string prompt, string defaultValue = "")
    {
        Present();
        WriteColored(prompt, ConsoleColor.Green, null, false);

        if (!string.IsNullOrWhiteSpace(defaultValue))
        {
            WriteColored($" [{defaultValue}]", ConsoleColor.DarkGray, null, false);
        }

        WriteColored(" > ", ConsoleColor.Green, null, false);
        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            return defaultValue;
        }

        return input.Trim();
    }

    public static int ReadInt(string prompt, int min, int max)
    {
        Present();

        while (true)
        {
            WriteColored($"{prompt} ({min}~{max}) > ", ConsoleColor.Green, null, false);
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int value) && value >= min && value <= max)
            {
                return value;
            }

            WriteToast($"{min}부터 {max} 사이의 숫자를 입력하세요.", ToastType.Warning);
        }
    }

    public static bool Confirm(string prompt)
    {
        Present();

        while (true)
        {
            WriteColored($"{prompt} (Y/N) > ", ConsoleColor.Green, null, false);
            string? input = Console.ReadLine()?.Trim().ToUpperInvariant();

            if (input == "Y" || input == "YES")
            {
                return true;
            }

            if (input == "N" || input == "NO")
            {
                return false;
            }

            WriteToast("Y 또는 N을 입력하세요.", ToastType.Warning);
        }
    }

    public static void Pause(string message = "계속하려면 아무 키나 누르세요...")
    {
        Present();
        Console.WriteLine();
        WriteColored(message, ConsoleColor.DarkGray, null, false);
        Console.ReadKey(true);
    }

    public static void WriteToast(string message, ToastType type = ToastType.Info)
    {
        ConsoleColor color = type switch
        {
            ToastType.Success => ConsoleColor.Green,
            ToastType.Warning => ConsoleColor.Yellow,
            ToastType.Error => ConsoleColor.Red,
            _ => ConsoleColor.Cyan
        };

        string prefix = type switch
        {
            ToastType.Success => "[성공]",
            ToastType.Warning => "[주의]",
            ToastType.Error => "[오류]",
            _ => "[정보]"
        };

        WriteColored($"{prefix} {message}", color);
    }

    /// <summary>
    /// HP, 경험치 같은 값을 막대 그래프로 보여줍니다.
    /// </summary>
    public static void WriteStatusBar(string label, int current, int max, int barWidth = 24, ConsoleColor fillColor = ConsoleColor.Green)
    {
        max = Math.Max(1, max);
        current = Math.Clamp(current, 0, max);

        double ratio = current / (double)max;
        int filled = (int)Math.Round(ratio * barWidth);
        int empty = barWidth - filled;

        Write(Fit(label, 10));
        Write(" [");
        WriteColored(new string('█', filled), fillColor, null, false);
        WriteColored(new string('░', empty), ConsoleColor.DarkGray, null, false);
        Write("] ");
        WriteColored($"{current}/{max}", ConsoleColor.White);
    }

    public static void WriteStatusBar(Character chara, string label,
        int sanity, int current, int max, int barWidth = 24, ConsoleColor fillColor = ConsoleColor.Green)
    {
        max = Math.Max(1, max);
        current = Math.Clamp(current, 0, max);

        double ratio = current / (double)max;
        int filled = (int)Math.Round(ratio * barWidth);
        int empty = barWidth - filled;

        Write(Fit(label, 10));
        Write(" [");
        WriteColored(new string('█', filled), fillColor, null, false);
        WriteColored(new string('░', empty), ConsoleColor.DarkGray, null, false);
        Write("] ");
        WriteColored($"체력 : {current}/{max} | 정신력 : {sanity}", ConsoleColor.White);
        StringBuilder sb = new();

        if (chara.BuffList.Count > 0)
        {
            sb.Append("버프 : ");

            chara.BuffList.ForEach(x => {
                    sb.Append($"{x.Name}");
                sb.Append($" {x.Coefficient}");
                
                if (x.Duration == -1) sb.Append($" - 소멸시까지 지속 \n");
                else sb.Append($" - {x.Duration} 턴 지속 \n");
            });
        }

        if(chara.DebuffList.Count > 0)
        {
            sb.Append("디버프 : ");

            chara.DebuffList.ForEach(x => {
                sb.Append($"{x.Name}");
                sb.Append($" {x.Coefficient}");

                if (x.Duration == -1) sb.Append($" - 소멸시까지 지속 \n");
                else sb.Append($" - {x.Duration} 턴 지속 \n");
            });
        }
        
        WriteLine($"{sb}");
    }

    /// <summary>
    /// key-value 형태의 정보를 출력합니다.
    /// 상태창에서 능력치 목록을 보여줄 때 사용합니다.
    /// </summary>
    public static void WriteKeyValue(string key, string value, int keyWidth = 12)
    {
        WriteColored(Fit(key, keyWidth), ConsoleColor.DarkGray, null, false);
        Write(" : ");
        WriteColored(value, ConsoleColor.White);
    }

    /// <summary>
    /// 표 형태로 데이터를 출력합니다.
    /// 인벤토리, 상점, 랭킹, 퀘스트 목록에 사용할 수 있습니다.
    /// </summary>
    public static void WriteTable(IReadOnlyList<string> headers, IReadOnlyList<IReadOnlyList<string>> rows)
    {
        if (headers.Count == 0)
        {
            return;
        }

        int columnCount = headers.Count;
        int[] widths = new int[columnCount];

        for (int i = 0; i < columnCount; i++)
        {
            widths[i] = GetDisplayWidth(headers[i]);
        }

        foreach (IReadOnlyList<string> row in rows)
        {
            for (int i = 0; i < columnCount && i < row.Count; i++)
            {
                widths[i] = Math.Max(widths[i], GetDisplayWidth(row[i]));
            }
        }

        for (int i = 0; i < widths.Length; i++)
        {
            widths[i] = Math.Clamp(widths[i], 4, 26);
        }

        string border = "+" + string.Join("+", widths.Select(width => new string('-', width + 2))) + "+";

        WriteColored(border, ConsoleColor.DarkGray);
        WriteTableRow(headers, widths, ConsoleColor.Cyan);
        WriteColored(border, ConsoleColor.DarkGray);

        foreach (IReadOnlyList<string> row in rows)
        {
            WriteTableRow(row, widths, ConsoleColor.White);
        }

        WriteColored(border, ConsoleColor.DarkGray);
        WriteLine();
    }
    public static void WriteLoad(int type,
        string first = "", 
        string second = "", 
        string third = "", 
        string fourth = "", 
        string fifth = "",
        string sixth = "",
        string seventh = "",
        string eighth = "",
        string ninth = "",
        bool clearActive = true
        )
    {
        
        type = Math.Clamp(type, 1, 5);
        if(clearActive) Clear();
        
        StringBuilder sb = new();
        sb.Append(first);
        first = first == "" || first == null ?
            DEFAULT_STRING : sb.StrLengExtend(LOAD_TEXT_LENG, first);
        sb.Clear();

        sb.Append(second);
        second = second == "" || second == null ?
            DEFAULT_STRING : sb.StrLengExtend(LOAD_TEXT_LENG, second);
        sb.Clear();

        sb.Append(third);
        third = third == "" || third == null ?
            DEFAULT_STRING : sb.StrLengExtend(LOAD_TEXT_LENG, third);
        sb.Clear();

        sb.Append(fourth);
        fourth = fourth == "" || fourth == null ?
            DEFAULT_STRING : sb.StrLengExtend(LOAD_TEXT_LENG, fourth);
        sb.Clear();

        sb.Append(fifth);
        fifth = fifth == "" || fifth == null ?
            DEFAULT_STRING : sb.StrLengExtend(LOAD_TEXT_LENG, fifth);
        sb.Clear();

        sb.Append(sixth);
        sixth = sixth == "" || sixth == null ?
            DEFAULT_STRING : sb.StrLengExtend(LOAD_TEXT_LENG, sixth);
        sb.Clear();

        sb.Append(seventh);
        seventh = seventh == "" || seventh == null ?
            DEFAULT_STRING : sb.StrLengExtend(LOAD_TEXT_LENG, seventh);
        sb.Clear();

        sb.Append(eighth);
        eighth = eighth == "" || eighth == null ?
            DEFAULT_STRING : sb.StrLengExtend(LOAD_TEXT_LENG, eighth);
        sb.Clear();

        sb.Append(ninth);
        ninth = ninth == "" || ninth == null ?
            DEFAULT_STRING : sb.StrLengExtend(LOAD_TEXT_LENG, ninth);
        sb.Clear();



        // 여유칸 43칸
        switch (type)
        {
            case 1:
                WriteLine($"        /|{first}|\\");
                WriteLine($"       / |{second}| \\");
                WriteLine($"      /  |{third}|  \\");
                WriteLine($"     /   |{fourth}|   \\");
                WriteLine($"    /    |{fifth}|    \\");
                WriteLine($"   /|    |{sixth}|    |\\");
                WriteLine($"  / |    |{seventh}|    | \\");
                WriteLine($" /  |    |{eighth}|    |  \\");
                WriteLine($"/   |     {ninth}     |   \\");
                break;
            case 2:
                WriteLine($"        / {first} \\");
                WriteLine($"       /| {second} |\\");
                WriteLine($"      / | {third} | \\");
                WriteLine($"     /  | {fourth} |  \\");
                WriteLine($"    /   | {fifth} |   \\");
                WriteLine($"   /    | {sixth} |    \\");
                WriteLine($"  /|    | {seventh} |    |\\");
                WriteLine($" / |    | {eighth} |    | \\");
                WriteLine($"/  |      {ninth}      |  \\");
                break;
            case 3:
                WriteLine($"        / {first} \\");
                WriteLine($"       /  {second}  \\");
                WriteLine($"      /|  {third}  |\\");
                WriteLine($"     / |  {fourth}  | \\");
                WriteLine($"    /  |  {fifth}  |  \\");
                WriteLine($"   /   |  {sixth}  |   \\");
                WriteLine($"  /    |  {seventh}  |    \\");
                WriteLine($" /|    |  {eighth}  |    |\\");
                WriteLine($"/ |       {ninth}       | \\");

                break;
            case 4:
                WriteLine($"        / {first} \\");
                WriteLine($"       /  {second}  \\");
                WriteLine($"      /   {third}   \\");
                WriteLine($"     /|   {fourth}   |\\");
                WriteLine($"    / |   {fifth}   | \\");
                WriteLine($"   /  |   {sixth}   |  \\");
                WriteLine($"  /   |   {seventh}   |   \\");
                WriteLine($" /    |   {eighth}   |    \\");
                WriteLine($"/|    |   {ninth}   |    |\\");
                break;
            case 5:
                WriteLine($"        / {first} \\");
                WriteLine($"       /  {second}  \\");
                WriteLine($"      /   {third}   \\");
                WriteLine($"     /    {fourth}    \\");
                WriteLine($"    /|    {fifth}    |\\");
                WriteLine($"   / |    {sixth}    | \\");
                WriteLine($"  /  |    {seventh}    |  \\");
                WriteLine($" /   |    {eighth}    |   \\");
                WriteLine($"/    |    {ninth}    |    \\");
                break;
            default:break;
        }
        if (clearActive) Present();
    }


    public static void WriteTown()
    {
        // 메인 길이
        //WriteLine($"|                                                      |");

        //메인 높이
        WriteLine($"|  ┌──────────────┐                                    |");
        WriteLine($"|  │  ┌────────┐  │                                    |");
        WriteLine($"|  │  │ 훈련장 │  │       ___ ___                      |");
        WriteLine($"|  │  └────────┘  │      /\\       \\                    |");
        WriteLine($"|  │              │     /  \\       \\     ┌───────────┐ |");
        WriteLine($"|  ├──────────────┤    /상점\\       \\    │ 던전 입구 │ |");
        WriteLine($"|  │              │    ├────┴ - - - ┤    └───────────┘ |");
        WriteLine($"|  │     ┌┬┬┐     │    │ ┌┐ │       │     ││       ││  |");
        WriteLine($"|  │     ││││     │    │ ││ │       │     ││       ││  |");
    }
    public static void WriteTrainingHall()
    {
        WriteLine($"|     ┌┐  │    │            ┌─────┐       ┌─────┐      |");
        WriteLine($"|     ││  ├┬───┘            │     │       │     │      |");
        WriteLine($"|   ─┬┴┴┬─├┤  ┼┼  ┌┐        └─────┘       └─────┘      |");
        WriteLine($"|    │  │ ││  ││ ─┼┼─          │             │         |");
        WriteLine($"|  │ │  │ ││  ││  ││ │     ────┼────     ────┼────     |");
        WriteLine($"|  ├─┤  ├─┤├──┤├──┤├─┤         │             │         |");
        WriteLine($"|  │ │  │ ││  ││  ││ │         │             │         |");
        WriteLine($"|  │ \\  / ││ /  \\ ││ │         │             │         |");
        WriteLine($"|  │  \\/  ││ \\  / ││ │         │             │         |");
    }

    // 완성 후 생각
    /*public static void WriteInTrainingHall(int type)
    {
        if (type == 1)
        {
            WriteLine($"|  │                   │          /   \\            \\   |");
            WriteLine($"|  ├───────────────────┤         /     \\            \\  |");
            WriteLine($"|  │                   │        / 상 점 \\            \\ |");
            WriteLine($"|  │                   │       /         \\           │\\|");
            WriteLine($"|  │                   │        ├─────────┴ --- --- -┤   |");
            WriteLine($"|  │      ┌─┬─┬─┐      │        │         │          │ |");
            WriteLine($"|  │      │ │ │ │      │        │ ┌───┐   │          │ |");
            WriteLine($"|  │      │ │ │ │      │        │ │   │   │          │ |");
            WriteLine($"|  │      │ │ │ │      │        │ │   │   │          │ |");

        }
        else
        {
        WriteLine($"|  │       ┌────┬────┬────┐       │        │           |");
        WriteLine($"|  │                              │        ├────────── |");
        WriteLine($"|  │       ├────┼────┼────┤       │        │           |");
        WriteLine($"|  │       │    │    │    │       │        │    ┌─────┬|");
        WriteLine($"|  │       │    │    │    │       │        │    │     │|");
        WriteLine($"|  │       │    │    │    │       │        │    │     │|");
        WriteLine($"|  │       │    │    │    │       │        │    │     │|");
        WriteLine($"|  │       │    │    │    │       │        │    │     │|");
        WriteLine($"|  │       │    │    │    │       │        │    │     │|");

        }

    }*/


    private static void WriteTableRow(IReadOnlyList<string> cells, int[] widths, ConsoleColor color)
    {
        WriteColored("|", ConsoleColor.DarkGray, null, false);

        for (int i = 0; i < widths.Length; i++)
        {
            string value = i < cells.Count ? cells[i] : string.Empty;
            Write(" ");
            WriteColored(Fit(value, widths[i]), color, null, false);
            Write(" ");
            WriteColored("|", ConsoleColor.DarkGray, null, false);
        }

        WriteLine();
    }

    /// <summary>
    /// 미로 또는 타일맵을 콘솔에 그립니다.
    /// 각 셀을 ASCII 2글자 폭으로 고정해 벽 위치가 흔들리지 않게 합니다.
    /// 한글 문자나 일부 특수문자는 콘솔/폰트에 따라 폭이 다르게 보일 수 있으므로 맵 내부에는 사용하지 않습니다.
    /// </summary>
    public static void WriteMap(IReadOnlyList<string> rows)
    {
        foreach (string row in rows)
        {
            foreach (char tile in row)
            {
                switch (tile)
                {
                    case '#':
                        WriteColored("##", ConsoleColor.DarkGray, null, false);
                        break;
                    case 'P':
                        WriteColored("@ ", ConsoleColor.Cyan, null, false);
                        break;
                    case 'E':
                        WriteColored("E ", ConsoleColor.Yellow, null, false);
                        break;
                    case '.':
                        Write(new string(' ', MapCellWidth));
                        break;
                    default:
                        Write(new string(' ', MapCellWidth));
                        break;
                }
            }

            WriteLine();
        }

        WriteLine();
    }

    public static void WriteLog(IEnumerable<string> logs, int maxLines = LOG_LINE_LIMIT)
    {
        List<string> logLines = logs.TakeLast(maxLines).ToList();

        if (logLines.Count == 0)
        {
            logLines.Add("아직 로그가 없습니다.");
        }

        WriteBox(logLines, "최근 로그", ConsoleColor.DarkGray);
    }

    /// <summary>
    /// 한글은 콘솔에서 보통 영문보다 넓게 표시됩니다.
    /// 박스와 표의 정렬을 맞추기 위해 표시 폭을 직접 계산합니다.
    /// </summary>
    public static int GetDisplayWidth(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        int width = 0;

        foreach (char character in text)
        {
            if (char.IsControl(character))
            {
                continue;
            }

            width += IsWideCharacter(character) ? 2 : 1;
        }

        return width;
    }

    public static string Fit(string text, int displayWidth)
    {
        string trimmed = TrimToDisplayWidth(text, displayWidth);
        int padding = Math.Max(0, displayWidth - GetDisplayWidth(trimmed));
        return trimmed + new string(' ', padding);
    }

    private static string TrimToDisplayWidth(string text, int maxWidth)
    {
        if (GetDisplayWidth(text) <= maxWidth)
        {
            return text;
        }

        int width = 0;
        List<char> characters = new List<char>();

        foreach (char character in text)
        {
            int charWidth = IsWideCharacter(character) ? 2 : 1;

            if (width + charWidth > Math.Max(0, maxWidth - 1))
            {
                break;
            }

            characters.Add(character);
            width += charWidth;
        }

        return new string(characters.ToArray()) + "…";
    }

    private static bool IsWideCharacter(char character)
    {
        int code = character;

        return
            code >= 0x1100 && code <= 0x115F ||
            code >= 0x2E80 && code <= 0xA4CF ||
            code >= 0xAC00 && code <= 0xD7A3 ||
            code >= 0xF900 && code <= 0xFAFF ||
            code >= 0xFE10 && code <= 0xFE19 ||
            code >= 0xFE30 && code <= 0xFE6F ||
            code >= 0xFF00 && code <= 0xFF60 ||
            code >= 0xFFE0 && code <= 0xFFE6;
    }
}
