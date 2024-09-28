namespace Crossword_Generator;

public struct CrosswordWord
{
    public string Text { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsVertical { get; set; }


    public bool IsCompatible(CrosswordWord other)
    {
        if (IsColliding(other))
        {
            return IsCrossing(other);
        }

        return !IsTooClose(other);
    }
    

    public override string ToString()
    {
        return $"[{Text}] ({X}, {Y}) {(IsVertical ? "Vertical" : "Horizontal")}";
    }


    public int GetXMin()
    {
        return X;
    }

    public int GetXMax()
    {
        return IsVertical
            ? X
            : X + Text.Length - 1;
    }
    
    public int GetYMin()
    {
        return Y;
    }

    public int GetYMax()
    {
        return IsVertical
            ? Y + Text.Length - 1
            : Y;
    }


    private void GetLetterPosition(int index, out int x, out int y)
    {
        x = X + (IsVertical ? 0 : index);
        y = Y + (IsVertical ? index : 0);
    }
    
    private void GetBeforeStartPosition(out int x, out int y)
    {
        GetLetterPosition(-1, out x, out y);
    }

    private void GetAfterEndPosition(out int x, out int y)
    {
        GetLetterPosition(Text.Length, out x, out y);
    }

    private bool IsTooClose(CrosswordWord other)
    {
        if (IsVertical && other.IsVertical)
        {
            for (var i = 0; i < Text.Length; i++)
            {
                for (var j = 0; j < other.Text.Length; j++)
                {
                    GetLetterPosition(i, out var xA, out var yA);
                    other.GetLetterPosition(j, out var xB, out var yB);
                    
                    if (yA != yB) continue;

                    if (Math.Abs(xA - xB) == 1) return true;
                }
            }

            return false;
        }

        if (!IsVertical && !other.IsVertical)
        {
            for (var i = 0; i < Text.Length; i++)
            {
                for (var j = 0; j < other.Text.Length; j++)
                {
                    GetLetterPosition(i, out var xA, out var yA);
                    other.GetLetterPosition(j, out var xB, out var yB);
                    
                    if (xA != xB) continue;
                    
                    if (Math.Abs(yA - yB) == 1) return true;
                }
            }

            return false;
        }
        
        GetBeforeStartPosition(out var beforeX, out var beforeY);
        GetAfterEndPosition(out var afterX, out var afterY);

        for (var i = 0; i < other.Text.Length; i++)
        {
            other.GetLetterPosition(i, out var otherX, out var otherY);

            if (beforeX == otherX && beforeY == otherY) return true;
            
            if (afterX == otherX && afterY == otherY) return true;
        }
        
        other.GetBeforeStartPosition(out beforeX, out beforeY);
        other.GetAfterEndPosition(out afterX, out afterY);

        for (var i = 0; i < Text.Length; i++)
        {
            GetLetterPosition(i, out var otherX, out var otherY);

            if (beforeX == otherX && beforeY == otherY) return true;
            
            if (afterX == otherX && afterY == otherY) return true;
        }

        return false;
    }

    private bool IsCrossing(CrosswordWord other)
    {
        if (IsVertical == other.IsVertical) return false;
        
        for (var i = 0; i < Text.Length; i++)
        {
            for (var j = 0; j < other.Text.Length; j++)
            {
                GetLetterPosition(i, out var xA, out var yA);
                other.GetLetterPosition(j, out var xB, out var yB);
                    
                if (xA != xB) continue;
                    
                if (yA != yB) continue;

                if (Text[i] == other.Text[j]) return true;
            }
        }

        return false;
    }

    private bool IsColliding(CrosswordWord other)
    {
        var xMinA = GetXMin();
        var xMaxA = GetXMax();
        var yMinA = GetYMin();
        var yMaxA = GetYMax();

        var xMinB = other.GetXMin();
        var xMaxB = other.GetXMax();
        var yMinB = other.GetYMin();
        var yMaxB = other.GetYMax();
        
        if (xMaxA < xMinB || xMinA > xMaxB) return false;
        
        if (yMaxA < yMinB || yMinA > yMaxB) return false;
        
        return true;
    }
}