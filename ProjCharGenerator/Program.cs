using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;

namespace generator
{
    class CharGenerator 
    {
        private string syms = "абвгдеёжзийклмнопрстуфхцчшщьыъэюя"; 
        private char[] data;
        private int size;
        private Random random = new Random();
        public CharGenerator() 
        {
           size = syms.Length;
           data = syms.ToCharArray(); 
        }
        public char getSym() 
        {
           return data[random.Next(0, size)]; 
        }
    }
    
    public class BigramGenerator
    {
        private readonly Dictionary<string, int> _bigrams = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _generatedFrequency = new Dictionary<string, int>();
        private readonly Random _random = new Random();
        private readonly string _bigramsFilePath;
        private readonly string _analysisFilePath;
        private int _totalFrequencySum;

        public BigramGenerator(string dataPath, string analysisPath)
        {
            _bigramsFilePath = dataPath;
            _analysisFilePath = analysisPath;
            LoadBigrams();
        }

        public void GenerateAndSave(string outputPath, int textLength = 1000)
        {
            string generatedText = GenerateText(textLength);
            File.WriteAllText(outputPath, generatedText, Encoding.UTF8);
            MakeDataForPlot(textLength);
        }

        private void LoadBigrams()
        {
            if (!File.Exists(_bigramsFilePath))
                throw new FileNotFoundException("File Bigrams not found!!!", _bigramsFilePath);

            foreach (string line in File.ReadLines(_bigramsFilePath))
            {
                string[] parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 3) continue;

                string bigram = parts[1].Trim().ToLower();
                if (bigram.Length != 2) continue;

                if (int.TryParse(parts[2].Trim(), out int frequency))
                {
                    _bigrams[bigram] = frequency;
                }
            }

            _totalFrequencySum = _bigrams.Values.Sum();
        }

        private string GenerateText(int textLength)
        {
            if (_bigrams.Count == 0)
                throw new InvalidOperationException("Files not data!");

            StringBuilder result = new StringBuilder();

            for (int i = 0; i < textLength; i++)
            {
                string nextBigram = NextRandomBigram();
                _generatedFrequency[nextBigram] = _generatedFrequency.TryGetValue(nextBigram, out int count) ? count + 1 : 1;
                result.Append(nextBigram);
            }

            return result.ToString();
        }

        private string NextRandomBigram()
        {
            int randomValue = _random.Next(_totalFrequencySum);
            int cumulativeSum = 0;

            foreach (KeyValuePair<string, int> pair in _bigrams)
            {
                cumulativeSum += pair.Value;
                if (randomValue < cumulativeSum)
                    return pair.Key;
            }

            return _bigrams.Keys.First();
        }

        private void MakeDataForPlot(int textLength)
        {
            using (StreamWriter writer = new StreamWriter(_analysisFilePath, false, Encoding.UTF8))
            {
                foreach (KeyValuePair<string, int> pair in _generatedFrequency)
                    writer.WriteLine($"{pair.Key} {Math.Round((double)pair.Value / textLength, 5)} {Math.Round((double)_bigrams[pair.Key] / _totalFrequencySum, 5)}");
            }
        }
    }
    
    class Program
    {
        private const string ResultsDirectory = "../Results";
        private const string InputDataDirectory = "Data";
        static void Main(string[] args)
        {
            string bigramsPath = Path.Combine(InputDataDirectory, "bigrams_input.txt");

            string bigramsOutputPath = Path.Combine(ResultsDirectory, "gen-1.txt");

            string bigramsAnalysisPath = Path.Combine(ResultsDirectory, "gen-1-analysis.txt");

            var bigramGenerator = new BigramGenerator(bigramsPath, bigramsAnalysisPath);
            bigramGenerator.GenerateAndSave(bigramsOutputPath);
        }
    }
}

