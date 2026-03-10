namespace MarkovTextGenerator;

public class Program
{
    static void Main(string[] args)
    {
        Chain chain = new Chain();

        // 1. Altered portion: Read all lines from text files in the "Data" folder
        // Make sure you have a folder named "Data" in your project directory with .txt files!
        if (Directory.Exists("Data"))
        {
            foreach (string file in Directory.GetFiles("Data", "*.txt"))
            {
                Console.WriteLine($"Learning from {Path.GetFileName(file)}...");
                LoadText(file, chain);
            }
        }

        Console.WriteLine("\nWelcome to Marky Markov's Random Text Generator!");
        Console.WriteLine("Enter some text I can learn from (enter single ! to finish): ");

        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine();
            if (line == "!")
                break;

            chain.AddSentence(line);
        }

        // Update all the probabilities with the new data
        chain.UpdateProbabilities();

        // 2. Predict next word based on user input
        Console.WriteLine("\nPart 1: Single Word Prediction");
        Console.WriteLine("Give me a word and I'll tell you what typically comes next.");
        Console.Write("> ");
        var word = Console.ReadLine() ?? string.Empty;
        var nextWord = chain.GetNextWord(word);

        if (nextWord == "")
            Console.WriteLine("I haven't seen that word before, or it always ends a sentence.");
        else
            Console.WriteLine("I predict the next word will be: " + nextWord);

        // 3. Generate a completely random sentence
        Console.WriteLine("\nPart 2: Random Sentence Generation");
        string startWord = chain.GetRandomStartingWord();
        if (!string.IsNullOrEmpty(startWord))
        {
            string sentence = chain.GenerateSentence(startWord);
            Console.WriteLine("Generated Sentence: " + sentence);
        }
        else
        {
            Console.WriteLine("I don't have enough data to generate a sentence yet.");
        }
    }

    static void LoadText(string path, Chain chain)
    {
        // Read all lines from the file and train the chain
        var lines = File.ReadAllLines(path);
        foreach (var line in lines)
        {
            chain.AddSentence(line);
        }
    }
}

