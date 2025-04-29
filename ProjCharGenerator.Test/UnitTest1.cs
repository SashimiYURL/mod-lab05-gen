using generator;

namespace ProjCharGenerator.Test;

public class BigramGeneratorTests
{
    private readonly string _tempBigramsFile = "bigrams_test.txt";
    private readonly string _tempOutputFile = "output_test.txt";
    private readonly string _tempAnalysisFile = "stat_test.txt";


    [Fact]
    public void BigramGenerator_LoadsDataFromFile()
    {
        File.WriteAllText(_tempBigramsFile, "1 аа 100\n2 аб 50\n3 ба 30\n4 бб 20");

        var generator = new BigramGenerator(_tempBigramsFile, _tempAnalysisFile);
        Assert.Equal(4, generator.GetBigrams.Count);

        File.Delete(_tempBigramsFile);
    }

    [Fact]
    public void GenerateText_ProducesCorrectLength()
    {
        File.WriteAllText(_tempBigramsFile, "1 аа 100\n2 аб 50\n3 ба 30\n4 бб 20");

        var generator = new BigramGenerator(_tempBigramsFile, _tempAnalysisFile);
        string text = generator.GenerateText(100);
        Assert.Equal(200, text.Length);

        File.Delete(_tempBigramsFile);
    }

    [Fact]
    public void GenerateText_EmptyDataFile()
    {
        File.WriteAllText(_tempBigramsFile, "");
        var generator = new BigramGenerator(_tempBigramsFile, _tempAnalysisFile);

        var exception = Assert.Throws<InvalidOperationException>(() => generator.GenerateText(10));
        Assert.Equal("Files not data!", exception.Message);

        File.Delete(_tempBigramsFile);
    }

    [Fact]
    public void GenerateBigramsData_CreatesOutputFile()
    {
        File.WriteAllText(_tempBigramsFile, "1 аа 100\n2 аб 50\n3 ба 30\n4 бб 20");

        var generator = new BigramGenerator(_tempBigramsFile, _tempAnalysisFile);
        generator.GenerateBigramsData(_tempOutputFile, 50);
        Assert.True(File.Exists(_tempOutputFile));

        File.Delete(_tempBigramsFile);
        File.Delete(_tempOutputFile);
        File.Delete(_tempAnalysisFile);
    }

    [Fact]
    public void NextRandomBigram_ReturnsValidBigram()
    {
        File.WriteAllText(_tempBigramsFile, "1 аа 100\n2 аб 50\n3 ба 30\n4 бб 20");

        var generator = new BigramGenerator(_tempBigramsFile, _tempAnalysisFile);
        string bigram = generator.NextRandomBigram();
        Assert.Equal(2, bigram.Length);
        Assert.Contains(bigram, generator.GetBigrams.Keys);

        File.Delete(_tempBigramsFile);
    }
}

public class WordGeneratorTests
{
    private readonly string _tempWordsFile = "bigrams_test.txt";
    private readonly string _tempOutputFile = "output_test.txt";
    private readonly string _tempAnalysisFile = "stat_test.txt";


    [Fact]
    public void GenerateText_ContainsExpectedWords()
    {
        File.WriteAllText(_tempWordsFile, "1 слово 0.1 0.1 0.1\n2 тест 0.05 0.05 0.05\n3 пример 0.02 0.02 0.02");

        var generator = new WordGenerator(_tempWordsFile, _tempAnalysisFile);
        string text = generator.GenerateText();
        Assert.Contains("слово", text);
        Assert.Contains("тест", text);
        Assert.Contains("пример", text);

        File.Delete(_tempWordsFile);
    }

     [Fact]
    public void WordGenerator_LoadsDataFromFile()
    {
        File.WriteAllText(_tempWordsFile, "1 слово 0.1 0.1 100\n2 тест 0.05 0.05 0.05\n3 пример 0.02 0.02 100");

        var generator = new WordGenerator(_tempWordsFile, _tempAnalysisFile);
        Assert.Equal(3, generator.WordFrequencies.Count);

        File.Delete(_tempWordsFile);
    }

    [Fact]
    public void GenerateWordsData_CreatesOutputFile()
    {
        File.WriteAllText(_tempWordsFile, "1 слово 0.1 0.1 0.1\n2 тест 0.05 0.05 0.05\n3 пример 0.02 0.02 0.02");

        var generator = new WordGenerator(_tempWordsFile, _tempAnalysisFile);
        generator.GenerateWordsData(_tempOutputFile);
        Assert.True(File.Exists(_tempOutputFile));

        File.Delete(_tempWordsFile);
        File.Delete(_tempOutputFile);
        File.Delete(_tempAnalysisFile);
    }

    [Fact]
    public void NextRandomWord_ReturnsValidWord()
    {
        File.WriteAllText(_tempWordsFile, "1 слово 0.1 0.1 0.1\n2 тест 0.05 0.05 0.05\n3 пример 0.02 0.02 0.02");

        var generator = new WordGenerator(_tempWordsFile, _tempAnalysisFile);
        string word = generator.NextRandomWord();
        Assert.Contains(word, generator.WordFrequencies.Keys);

        File.Delete(_tempWordsFile);
    }

    [Fact]
    public void DataWordsReader_ThrowsOnMissingFile()
    {
        File.WriteAllText(_tempWordsFile, "1 слово 0.1 0.1 0.1\n2 тест 0.05 0.05 0.05\n3 пример 0.02 0.02 0.02");

        string missingFile = Path.GetTempFileName();
        File.Delete(missingFile);
        Assert.Throws<FileNotFoundException>(() => new WordGenerator(missingFile, _tempAnalysisFile));

        File.Delete(_tempWordsFile);
    }

    [Fact]
    public void GenerateText_EmptyDataFile()
    {
        File.WriteAllText(_tempWordsFile, "");
        var generator = new WordGenerator(_tempWordsFile, _tempAnalysisFile);

        var exception = Assert.Throws<InvalidOperationException>(() => generator.GenerateText());
        Assert.Equal("Word frequencies not loaded", exception.Message);

        File.Delete(_tempWordsFile);
    }
}

