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
        public Dictionary<string, int> GetBigrams => _bigrams;

        public BigramGenerator(string dataPath, string analysisPath)
        {
            _bigramsFilePath = dataPath;
            _analysisFilePath = analysisPath;
            DataBigramsReader();
        }

        public void GenerateBigramsData(string outputPath, int textLength = 1000)
        {
            string generatedText = GenerateText(textLength);
            File.WriteAllText(outputPath, generatedText, Encoding.UTF8);

            using (StreamWriter writer = new StreamWriter(_analysisFilePath, false, Encoding.UTF8))
            {
                foreach (KeyValuePair<string, int> pair in _generatedFrequency)
                    writer.WriteLine($"{pair.Key} {Math.Round((double)pair.Value / textLength, 5)} {Math.Round((double)_bigrams[pair.Key] / _totalFrequencySum, 5)}");
            }
        }

        private void DataBigramsReader()
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

        public string GenerateText(int textLength)
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

        public string NextRandomBigram()
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
    }


    public class WordGenerator
    {
        private readonly Dictionary<string, int> _wordFrequencies = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _generatedFrequency = new Dictionary<string, int>();
        private readonly Random _random = new Random();
        private readonly string _wordsFilePath;
        private readonly string _analysisFilePath;
        private readonly int _wordCount = 1000;
        private int _totalFrequencySum;

        public Dictionary<string, int> WordFrequencies => _wordFrequencies;
        public Dictionary<string, int> GeneratedFrequency => _generatedFrequency;
        public int TotalFrequencySum => _totalFrequencySum;

        public WordGenerator(string wordsFilePath, string analysisFilePath)
        {
            _wordsFilePath = wordsFilePath;
            _analysisFilePath = analysisFilePath;
            DataWordsReader();
        }

        public void GenerateWordsData(string resultFilePath)
        {
            string generatedText = GenerateText();
            File.WriteAllText(resultFilePath, generatedText, Encoding.UTF8);

            using StreamWriter writer = new StreamWriter(_analysisFilePath, false, Encoding.UTF8);

            foreach (KeyValuePair<string, int> pair in _generatedFrequency)
                writer.WriteLine($"{pair.Key} {Math.Round((double)pair.Value / _wordCount, 5)} {Math.Round((double)_wordFrequencies[pair.Key] / _totalFrequencySum, 5)}");
        }

        private void DataWordsReader()
        {
            if (!File.Exists(_wordsFilePath))
                throw new FileNotFoundException("Words file not found", _wordsFilePath);

            foreach (string line in File.ReadLines(_wordsFilePath))
            {
                string[] parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 5) continue;

                string word = parts[1].Trim();
                string frequencyStr = parts[4].Trim();

                if (double.TryParse(frequencyStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double frequency))
                {
                    int intFrequency = (int)(frequency * 100);
                    _wordFrequencies[word] = intFrequency;
                }
            }

            _totalFrequencySum = _wordFrequencies.Values.Sum();
        }

        public string GenerateText()
        {
            if (_wordFrequencies.Count == 0)
                throw new InvalidOperationException("Word frequencies not loaded");

            StringBuilder result = new StringBuilder();

            for (int i = 0; i < _wordCount; i++)
            {
                string currentWord = NextRandomWord();
                _generatedFrequency[currentWord] = _generatedFrequency.TryGetValue(currentWord, out int count) ? count + 1 : 1;

                result.Append(currentWord + ' ');
            }

            return result.ToString();
        }

        public string NextRandomWord()
        {
            int randomValue = _random.Next(_totalFrequencySum);
            int cumulativeSum = 0;

            foreach (KeyValuePair<string, int> pair in _wordFrequencies)
            {
                cumulativeSum += pair.Value;
                if (randomValue < cumulativeSum)
                    return pair.Key;
            }

            return _wordFrequencies.Keys.First();
        }
    }

    class Program
    {
        private const string ResultsDirectory = "../Results";
        private const string InputDataDirectory = "Data";
        static void Main(string[] args)
        {
            string bigramsDataPath = Path.Combine(InputDataDirectory, "bigrams_input.txt");
            // string bigramsDataPath = @"C:\Users\vniki\source\repos\Yurlova-mod-lab05\mod-lab05-gen\ProjCharGenerator\Data\bigrams_input.txt";
            string wordsDataPath = Path.Combine(InputDataDirectory, "words_input.txt");
            // string wordsDataPath = @"C:\Users\vniki\source\repos\Yurlova-mod-lab05\mod-lab05-gen\ProjCharGenerator\Data\words_input.txt";

            string bigramsOutPath = Path.Combine(ResultsDirectory, "gen-1.txt");
            string wordsOutPath = Path.Combine(ResultsDirectory, "gen-2.txt");

            string bigramsStatPath = Path.Combine(ResultsDirectory, "gen-1-stat.txt");
            string wordsStatPath = Path.Combine(ResultsDirectory, "gen-2-stat.txt");

            var bigramGenerator = new BigramGenerator(bigramsDataPath, bigramsStatPath);
            bigramGenerator.GenerateBigramsData(bigramsOutPath);

            var wordsGenerator = new WordGenerator(wordsDataPath, wordsStatPath);
            wordsGenerator.GenerateWordsData(wordsOutPath);
        }
    }
}

