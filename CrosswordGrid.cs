namespace Crossword_Generator;

public class CrosswordGrid
{
    private readonly List<CrosswordWord> m_Words = new();
    private int m_SizeX;
    private int m_SizeY;
    
    
    public int GetArea()
    {
        return m_SizeX * m_SizeY;
    }

    public int GetAbsEdgeDifference()
    {
        return Math.Abs(m_SizeX - m_SizeY);
    }
    
    public void Print()
    {
        var matrix = new char[m_SizeX, m_SizeY];

        for (var y = 0; y < m_SizeY; y++)
        {
            for (var x = 0; x < m_SizeX; x++)
            {
                matrix[x, y] = ' ';
            }
        }
        
        foreach (var word in m_Words)
        {
            for (var i = 0; i < word.Text.Length; i++)
            {
                var x = word.X + (word.IsVertical ? 0 : i);
                var y = word.Y + (word.IsVertical ? i : 0);

                matrix[x, y] = word.Text[i];
            }
        }

        var output = "============== WORDS ==============\n";

        foreach (var word in m_Words)
        {
            output += $"{word}\n";
        }
        
        output += "============== MAP ==============\n";
        
        for (var y = 0; y < m_SizeY; y++)
        {
            output += "|";
            
            for (var x = 0; x < m_SizeX; x++)
            {
                output += $"{matrix[x, y]}|";
            }

            output += "\n";
        }

        output += $"Size: [{m_SizeX}x{m_SizeY}]";
        
        Console.WriteLine(output);
    }

    public void FixPositions()
    {
        var xMin = int.MaxValue;
        var xMax = int.MinValue;
        var yMin = int.MaxValue;
        var yMax = int.MinValue;

        foreach (var word in m_Words)
        {
            var wordXMin = word.GetXMin();
            var wordXMax = word.GetXMax();
            var wordYMin = word.GetYMin();
            var wordYMax = word.GetYMax();
            
            if (wordXMin < xMin)
            {
                xMin = wordXMin;
            }

            if (wordXMax > xMax)
            {
                xMax = wordXMax;
            }

            if (wordYMin < yMin)
            {
                yMin = wordYMin;
            }

            if (wordYMax > yMax)
            {
                yMax = wordYMax;
            }
        }

        for (var i = 0; i < m_Words.Count; i++)
        {
            var word = m_Words[i];

            word.X -= xMin;
            word.Y -= yMin;

            m_Words[i] = word;
        }

        m_SizeX = xMax - xMin + 1;
        m_SizeY = yMax - yMin + 1;
    }
    
    public void PushWord(CrosswordWord word)
    {
        m_Words.Add(word);
    }

    public void PopWord()
    {
        m_Words.RemoveAt(m_Words.Count - 1);
    }

    public CrosswordGrid Clone()
    {
        var grid = new CrosswordGrid();

        foreach (var word in m_Words)
        {
            grid.m_Words.Add(word);
        }

        grid.m_SizeX = m_SizeX;
        grid.m_SizeY = m_SizeY;

        return grid;
    }
    
    public List<CrosswordWord> GetPossibilities(string word)
    {
        var allPossibilities = new List<CrosswordWord>();

        if (m_Words.Count == 0)
        {
            allPossibilities.Add(new CrosswordWord
            {
                Text = word,
                IsVertical = false,
                X = 0,
                Y = 0
            });

            return allPossibilities;
        }

        foreach (var otherWords in m_Words)
        {
            var possibilities = GetCrossingPossibilities(otherWords, word);
            
            allPossibilities.AddRange(possibilities);
        }

        return allPossibilities;
    }


    private bool IsCompatible(CrosswordWord word)
    {
        return m_Words.All(other => word.IsCompatible(other));
    }
    
    private List<CrosswordWord> GetCrossingPossibilities(CrosswordWord a, string b)
    {
        var possibilities = new List<CrosswordWord>();
        
        for (var i = 0; i < a.Text.Length; i++)
        {
            var letterA = a.Text[i];
            
            for (var j = 0; j < b.Length; j++)
            {
                var letterB = b[j];

                if (letterA != letterB) continue;

                var possibility = new CrosswordWord
                {
                    Text = b,
                    IsVertical = !a.IsVertical,
                    X = a.X + (a.IsVertical ? -j : i),
                    Y = a.Y + (a.IsVertical ? i : -j)
                };

                if (!IsCompatible(possibility)) continue;
                
                possibilities.Add(possibility);
            }
        }

        return possibilities;
    }
}