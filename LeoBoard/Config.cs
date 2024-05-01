using System.Runtime.CompilerServices;

namespace LeoBoard;

internal sealed record Config(
    string Title,
    int Rows,
    int Columns,
    int CellSize,
    int FontSize,
    int ExtraXTextOffset,
    bool DrawGridNumbers,
    bool TestMode,
    Action<int, int, bool, bool>? ClickHandler)
{
    public const int MinWidth = 100;
    public const int MinHeight = 60;
    
    public void EnsureValid()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            throw new ArgumentException("Title must not be empty");
        }
        
        EnsureMinSize(Rows);
        EnsureMinSize(Columns);
        EnsureMinSize(CellSize);
        EnsureMinSize(FontSize);
        
        return;
        
        static void EnsureMinSize(int value, [CallerArgumentExpression(nameof(value))] string valueName = "")
        {
            const int MinSize = 1;
            
            if (value >= MinSize)
            {
                return;
            }
            
            throw new ArgumentException($"{valueName} must be at least {MinSize}");
        }
    }
}
