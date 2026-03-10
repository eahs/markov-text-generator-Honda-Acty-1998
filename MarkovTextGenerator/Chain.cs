using System.Text;

namespace MarkovTextGenerator;

public class Chain
{
    public Dictionary<string, List<Word>> Words { get; set; } = new();
    private readonly Dictionary<string, int> _sums = new();
    private readonly Random _rand = new();

    // NEW: A list to keep track of words that actually start a sentence
    private List<string> _startingWords = new List<string>();

    public string GetRandomStartingWord()
    {
        //Pick from the starting words instead of all words
        if (_startingWords.Count == 0) return "";
        return _startingWords[_rand.Next(_startingWords.Count)];
    }

    public void AddSentence(string? sentence)
    {
        if (string.IsNullOrWhiteSpace(sentence)) return;
        string[] words = sentence.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // 2. Save the first word so we know it's a good place to start
        _startingWords.Add(words[0]);

        for (int i = 0; i < words.Length - 1; i++)
        {
            AddPair(words[i], words[i + 1]);
        }
        AddPair(words[words.Length - 1], "");
    }

    public void AddPair(string word, string word2)
    {
        if (!Words.ContainsKey(word))
        {
            Words[word] = new List<Word>();
            _sums[word] = 0;
        }

        _sums[word]++;

        foreach (var w in Words[word])
        {
            if (w.Value == word2)
            {
                w.Count++;
                return;
            }
        }

        Words[word].Add(new Word(word2));
    }

    public void UpdateProbabilities()
    {
        foreach (var word in Words.Keys)
        {
            foreach (var next in Words[word])
            {
                next.Probability = (double)next.Count / _sums[word];
            }
        }
    }

    public string GetNextWord(string word)
    {
        if (!Words.ContainsKey(word)) return "";

        double roll = _rand.NextDouble();
        double cumulative = 0;

        foreach (var w in Words[word])
        {
            cumulative += w.Probability;
            if (roll <= cumulative)
                return w.Value;
        }

        return "";
    }

    public string GenerateSentence(string startingWord)
    {
        if (string.IsNullOrEmpty(startingWord)) return "";

        StringBuilder sentence = new StringBuilder(startingWord);
        string currentWord = startingWord;

        // Safety break at 50 words so it doesn't loop forever if your data is circular
        for (int i = 0; i < 50; i++)
        {
            string nextWord = GetNextWord(currentWord);

            if (string.IsNullOrEmpty(nextWord))
                break;

            sentence.Append(" " + nextWord);
            currentWord = nextWord;
        }

        return sentence.ToString();
    }
}

