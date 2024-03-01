namespace TextDecorator;

internal static class TextDecorator
{
    /// <summary>
    /// Main program. Initiates the different modes based on the program arguments. If no arguments are made, or its
    /// to few or to many arguments it gives appropriate error- message.
    /// The first character/word in arguments represents the mode, and the one after whitespace is the word/text that
    /// the program should decorate.
    /// <example># Lorem Ipsum is simply dummy text of the printing and typesetting industry.</example>
    /// </summary>
    /// <param name="args">Mode, Text</param>
    private static void Main(string[] args)
    {
        if (args.Length is < 2 or > 3)
        {
            Console.WriteLine("Please enter program arguments: <mode> <text>");
            Console.WriteLine("Modes: #, alt, robber/røver or pig. Use \" \" if multiple words");
            return;
        }

        string mode = args[0].ToLower();
        string text = args[1];

        switch (mode)
        {
            case "#":
                BlockMode(text);
                break;
            case "alt":
                AlternateCaseMode(text);
                break;
            case "røver":
            case "robber":
                RobberMode(text);
                break;
            case "pig":
                PigLatinMode(text);
                break;
            default:
                Console.WriteLine(
                    "Unknown mode. Please use #, alt, robber/røver or pig. The program is not case-sensitive.");
                break;
        }
    }
    
    /// <summary>
    /// It starts with calculating how long the text is. I have made the decision to have a max width of 120 characters
    /// (including 4 "empty" blocks) before jumping to new line. It also adds 4 blocks so that the decor goes around
    /// all the text. Used in  top/bottom border. lineLength  and lastSpace then works out how long the line actually
    /// gets for each when the line cannot have more than 116 characters AND needs to break line before a word gets cut
    /// in half. The .substring method prints one and one line with max 116 characters and ends with whitespace, and
    /// then adds spaces if needed with new string method so that all blocks on each line ends up in same position
    /// (max 120).
    /// </summary>
    /// <param name="text">Input- text from program-arguments</param>
    private static void BlockMode(string text)
    {
        int maxLineWidth = Math.Min(Math.Max(text.Length + 4, 0), 120);
        int pos = 0;
        
        Console.WriteLine(new string('#', maxLineWidth));
        
        while (pos < text.Length)
        {
            int lineLength = Math.Min(maxLineWidth - 4, text.Length - pos);
            int lastWhiteSpace = text.LastIndexOf(' ', pos + lineLength - 1);
            if (lastWhiteSpace != -1 && lastWhiteSpace > pos && pos + lineLength < text.Length)
            {
                lineLength = lastWhiteSpace - pos + 1;
            }
            Console.WriteLine($"# {text.Substring(pos, lineLength)}" +
                              $"{new string(' ', Math.Max(maxLineWidth - 3 - lineLength, 0))}#");
            pos += lineLength;
            while (pos < text.Length && char.IsWhiteSpace(text[pos]))
            {
                pos++;
            }
        }
        Console.WriteLine(new string('#', maxLineWidth));
    }
    
    /// <summary>
    /// A simple method that writes every other character lowercase and uppercase.
    /// </summary>
    /// <improvement> I considered to make the code start each sentence with upper-letter, but the specs did not
    /// imply that this is necessary here.</improvement>
    /// <param name="text">Input- text from program-arguments</param>
    private static void AlternateCaseMode(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            Console.Write(i % 2 == 0 ? char.ToUpper(text[i]) : char.ToLower(text[i]));
        }
        Console.WriteLine();
    }
    
    /// <summary>
    /// Somewhat same as previous method. It uses the bool-method IsVowel to check if character is a vowel. If it is
    /// not a vowel then letter + "o" pluss letter again. If it is, then just the letter.
    /// </summary>
    /// <param name="text">Input- text from program-arguments</param>
    private static void RobberMode(string text) 
    {
        foreach (char c in text)
        {
            if (char.IsLetter(c))
            {
                char letter = char.ToLower(c);
                Console.Write(!IsVowel(letter) ? $"{c}o{letter}" : $"{c}");
            }
            else
            {
                Console.Write(c);
            }
        }
        Console.WriteLine();
    }
    
    /// <summary>
    /// Starts by splitting whole text into an array of words. Then checks for punctuations (period, comma etc) after
    /// every word. If it is a punctuation then it remembers that punctuation for that word and then strips it away.
    /// It then uses ConvertToPigLatin() to translate the word. It then checks whenever there is a punctuation if it
    /// is to end a sentence ( . or ? or ! ). If that is true then the next word will be in a new sentence and then
    /// calls CapitalizeFirstLetter() which sets this word to have uppercase first character, and then rest of them
    /// lowercase.
    /// <improvement>Could have added prefixes(?) with words that starts with e.g "ch", "qu", "th", "rh". And some
    /// other rules. There are many variants I believe.</improvement>
    /// </summary>
    /// <param name="text">Input- text from program-arguments</param>
    private static void PigLatinMode(string text)
    {
        bool isNewSentence = true;
        string[] words = text.Split(' ');
        
        for (int i = 0; i < words.Length; i++)
        {
            string word = words[i];
            string punctuation = "";
            
            if (char.IsPunctuation(word[^1]))
            {
                punctuation = word[^1].ToString();
                word = word[..^1];
            }
            
            string pigLatinWord = ConvertToPigLatin(word, isNewSentence);

            isNewSentence = punctuation is "." or "?" or "!";
            Console.Write($"{pigLatinWord}{punctuation}{(i < words.Length - 1 ? " " : "")}");

        }
        Console.WriteLine();
    }

    /// <summary>
    /// Starts to check the input-word if it is first word in a new sentence. If bool is false then it makes sure that
    /// the word is all lowercase. It then do checks if first cha   r is vowel and then returns the word if it is capital
    /// or lower based on the bool. If word does not start with vowel then it calls the FindFirstVowelIndex() to find
    /// index of first vowel in a word. It then capitalizes or not, again based on the bool and returns this word to
    /// PigLatinMode() </summary>
    /// <param name="word">Word from PigLatinMode from text program-arguments</param>
    /// <param name="isNewSentence">Bool that checks for new sentence or not</param>
    /// <returns>Word, rearranged or not, capitalized or not</returns>
    private static string ConvertToPigLatin(string word, bool isNewSentence)
    {
        if (isNewSentence && string.IsNullOrWhiteSpace(word))
        {
            return string.Empty;
        }
        
        string caseCheck = isNewSentence ? word : word.ToLower();
        if (IsVowel(caseCheck[0]))
        {
            return isNewSentence ? CapitalizeFirstLetter($"{caseCheck}yay") : $"{caseCheck}yay";
        }

        int firstVowelIndex = FindFirstVowelIndex(caseCheck);
        if (firstVowelIndex == caseCheck.Length)
        {
            return caseCheck;
        }
        string pigLatin = $"{caseCheck[firstVowelIndex..]}{caseCheck[..firstVowelIndex]}ay";
        return isNewSentence ? CapitalizeFirstLetter(pigLatin) : pigLatin;
    }

    /// <summary>
    /// If called it checks that it is an actual word. Ut then returns the word with uppercase first letter, and
    /// lowercase the rest of the letters
    /// </summary>
    /// <param name="word">Word from PigLatinMode from text program-arguments</param>
    /// <returns>Capitalized word as long it is not NullOrEmpty</returns>
    private static string CapitalizeFirstLetter(string word)
    {
        return string.IsNullOrEmpty(word) ? word : char.ToUpper(word[0]) + word[1..].ToLower();
    }

    /// <summary>
    /// Method to out the index of the first vowel in a word is. Used in pig latin.
    /// </summary>
    /// <param name="word">Word from pig latin</param>
    /// <returns>integer with the index of first vowel in the word. If no vowel it returns the highest index</returns>
    private static int FindFirstVowelIndex(string word)
    {
        for (int i = 0; i < word.Length; i++)
        {
            if (IsVowel(word[i]))
            {
                return i;
            }
        }
        return word.Length;
    }
    
    /// <summary>
    /// A simple method that checks incoming letters if they are vowels or not.
    /// </summary>
    /// <param name="letter">Checks every character</param>
    /// <returns>True if it is vowel, False if not</returns>
    private static bool IsVowel(char letter)
    {
        return "aeiouAEIOU".Contains(letter);
    }
}