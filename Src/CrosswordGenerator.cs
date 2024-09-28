using System.Collections.Generic;

namespace Crossword_Generator
{
    public static class CrosswordGenerator
    {
        public static bool TryGenerateFromWords(string[] words, out CrosswordGrid grid)
        {
            var grids = GenerateAllFromWords(words);

            if (grids.Count <= 0)
            {
                grid = null;
            
                return false;
            }
        
            grid = grids[0];

            for (var i = 1; i < grids.Count; i++)
            {
                if (grids[i].GetArea() >= grid.GetArea()) continue;
            
                if (grids[i].GetAbsEdgeDifference() > grid.GetAbsEdgeDifference()) continue;
            
                grid = grids[i];
            }

            return true;
        }

        public static List<CrosswordGrid> GenerateAllFromWords(string[] words)
        {
            for (var i = 0; i < words.Length; i++)
            {
                words[i] = words[i].ToUpper();
            }
        
            var grids = new List<CrosswordGrid>();
            var permutations = GeneratePermutations(words);

            foreach (var permutation in permutations)
            {
                var candidates = new List<CrosswordGrid>();

                if (!TryGenerateFromWordPermutation(permutation, candidates)) continue;
            
                foreach (var candidate in candidates)
                {
                    candidate.FixPositions();
                    grids.Add(candidate);
                }
            }

            return grids;
        }


        private static bool TryGenerateFromWordPermutation(List<string> permutation, List<CrosswordGrid> grids)
        {
            var grid = new CrosswordGrid();

            return TryPlaceWordToGrid(permutation, 0, grid, grids);
        }

        private static bool TryPlaceWordToGrid(List<string> permutation, int index, CrosswordGrid grid, List<CrosswordGrid> grids)
        {
            if (index >= permutation.Count)
            {
                grids.Add(grid);
            
                return true;
            }
        
            var word = permutation[index];
            var possibilities = grid.GetPossibilities(word);
            var valid = false;

            foreach (var possibility in possibilities)
            {
                grid.PushWord(possibility);
            
                if (TryPlaceWordToGrid(permutation, index + 1, grid, grids))
                {
                    valid = true;
                
                    grid = grid.Clone();
                }
            
                grid.PopWord();
            }

            return valid;
        }
    
        private static List<List<string>> GeneratePermutations(string[] array)
        {
            var size = Factorial(array.Length);
            var permutations = new List<List<string>>(capacity: size);
        
            GeneratePermutationStep(permutations, array, 0, array.Length - 1);

            return permutations;
        }
    
        private static void GeneratePermutationStep(List<List<string>> permutations, string[] array, int start, int end)
        {
            if (start == end)
            {
                var permutation = new List<string>(array);
            
                permutations.Add(permutation);
            
                return;
            }

            for (var i = start; i <= end; i++)
            {
                (array[start], array[i]) = (array[i], array[start]);
                
                GeneratePermutationStep(permutations, array, start + 1, end);
                
                (array[start], array[i]) = (array[i], array[start]);
            }
        }

        private static int Factorial(int x)
        {
            if (x is 0 or 1) return 1;

            var result = 1;
        
            for (var i = 2; i <= x; i++)
            {
                result *= i;
            }

            return result;
        }
    }
}