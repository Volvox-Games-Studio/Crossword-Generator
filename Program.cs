using Crossword_Generator;

var words = new []{"Boğa", "Boza", "Bağ", "Boz", "Baz", "Oba", "Boğaz"};

var start = DateTime.Now;

var success = CrosswordGenerator.TryGenerateFromWords(words, out var grid);

var time = (DateTime.Now - start).TotalSeconds;

if (success)
{
    Console.WriteLine("===================== BEST LEVEL ===================");
    grid.Print();
}

Console.WriteLine($"Took {time:f2} seconds.");