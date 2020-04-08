using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordGuesser
{
    class Program
    {
        enum GameStateType
        {
            askForWordLength,
            quit,
            help,
            GuessCharacter,
            FilterOnUserResponse,
            FinishWord
        };

        static GameStateType gameState = GameStateType.askForWordLength;
        static List<char> currentGuessWord;
        static List<string> possibleWords;
        static List<char> guessedCharacters = new List<char>();
        static char currentGuessChar;

        static void Main(string[] args)
        {
            while (
                GameEngine()
            );
        }

        private static void PrintAskForWordLenght()
        {
            Console.WriteLine("Think of a word, enter its length of characters: ");
        }
        private static void PrintGameHeader()
        {
            Console.WriteLine("WordGuesser\n");
        }
        private static void PrintInstructionLine()
        {
            Console.WriteLine("(h) --> Help (q) --> Quit (n) --> New game\n\n");
        }
        private static void PrintGuessedCharacter()
        {

            foreach(var ch in currentGuessWord)
            {
                Console.Write($"{ ch } ");
                
            }
            Console.WriteLine("\n");

            if (gameState == GameStateType.FinishWord)
            {
                Console.WriteLine("Nice, word solved!\n");

                gameState = GameStateType.FinishWord;
            }
            else
            {
                Console.WriteLine($"Is there any \"{currentGuessChar}\", enter all positions (1-X) seperated with comma (,) or (0) if not present");
                gameState = GameStateType.FilterOnUserResponse;
            }
        }

        private static void GuessCharacter()
        {
            List<char> possibleCharacters = new List<char>();

            foreach(var word in possibleWords)
            {
                possibleCharacters.AddRange(word.ToCharArray());
            }
            possibleCharacters = possibleCharacters.Distinct().
                Where(x => false == guessedCharacters.Contains(x)).ToList();


            if(possibleCharacters.Count == 1)
            {
                currentGuessChar = possibleCharacters.ElementAt(0);

                for(int i = 0; i < currentGuessWord.Count; i++)
                {
                    if(currentGuessWord[i] == '_')
                    {
                        currentGuessWord[i] = currentGuessChar;
                    }
                }
                gameState = GameStateType.FinishWord;
                return;
            }

            var guessedChar = new Random().Next(0, possibleCharacters.Count);
            currentGuessChar = possibleCharacters[guessedChar];
        }

        private static void ReadWordList()
        {
             possibleWords = new List<string>()
                {
                    "klient",
                    "biljardbord",
                    "ninja",
                    "banjo",
                    "boll",
                    "biljardkö",
                    "kö"

                };
        }
        private static void FilterWordList()
        {
            for(var i = 0; i < currentGuessWord.Count; i++)
            {
                if(currentGuessWord[i] != '_')
                {
                    possibleWords = possibleWords.Where(x => x[i] == currentGuessWord[i]).ToList();
                }
            }
        }
        private static void FilterWordLength(int length)
        {
            // filter length
            possibleWords = possibleWords.Where(x => x.Length == length).ToList();
        }


        private static bool GameEngine()
        {
            PrintCurrentGameScreen();

            HandleUserInput();
            
            return gameState != GameStateType.quit;
        }

        private static void PrintCurrentGameScreen()
        {
            Console.Clear();
            PrintGameHeader();
            PrintInstructionLine();
            switch (gameState)
            {
                case GameStateType.askForWordLength:
                    ReadWordList();
                    PrintAskForWordLenght();
                    break;
                case GameStateType.GuessCharacter:
                    FilterWordList();
                    GuessCharacter();
                    PrintGuessedCharacter();
                    break;                
                default:
                    break;
            }

        }
        private static void HandleUserInput()
        {
            string userEntry = Console.ReadLine();

            if(userEntry ==  "q")
            {
                gameState = GameStateType.quit;
                return;
            }
            else if(userEntry == "h")
            {
                gameState = GameStateType.help;
                return;
            }

            switch (gameState)
            {
                case GameStateType.FinishWord:
                    if(userEntry == "n")
                    {
                        gameState = GameStateType.askForWordLength;
                    }
                    break;
                case GameStateType.askForWordLength:
                    try
                    {
                        var wordLength = Convert.ToInt32(userEntry);
                        currentGuessWord = new List<char>();


                        // tood, simpler ?
                        for(var i = 0; i < wordLength; i++){
                            currentGuessWord.Add('_');
                        }

                        FilterWordLength(wordLength);
                        gameState = GameStateType.GuessCharacter;
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("not a valid number of characters");
                    }
                    break;
                case GameStateType.FilterOnUserResponse:
                    if(userEntry == "0")
                    {
                        gameState = GameStateType.GuessCharacter; 
                    }
                    try
                    {
                        var positionsStr = userEntry.Split(',');
                        var positions = new int[positionsStr.Count()];
                        var i = 0;

                        foreach (var pos in positionsStr)
                        {
                            positions[i] = Convert.ToInt32(pos)-1; // eftersom användare skriver in 1 som första pos
                            if (positions[i] >= currentGuessWord.Count)
                            {
                                throw new Exception();
                            }
                            i++;
                        }
                        
                        for(var j = 0; j < i; j++)
                        {
                            currentGuessWord[positions[j]] = currentGuessChar;
                        }
                        guessedCharacters.Add(currentGuessChar);
                    } 
                    catch(Exception ex)
                    {
                        Console.WriteLine("Not correct positions");
                    }

                    gameState = GameStateType.GuessCharacter;
                    break;
            }
        }
    }
}
