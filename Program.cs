using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using static System.Formats.Asn1.AsnWriter;

class Logic
{
    static int speed = 1; // prędkość poruszania się statku, w sumie lekko bezsensu, chyba że da sie zniwelować efekt skakania xD
    static Random rand = new Random();
    public static int score = 0;

    public static void HandleInput(ref int playerPosition, List<int[]> bulletPositions)
    {
 
        if (Console.KeyAvailable)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    playerPosition -= speed;
                    if (playerPosition < 0)
                    {
                        playerPosition = Console.WindowWidth - 1;
                    }
                    break;
                case ConsoleKey.RightArrow:
                    playerPosition += speed;
                    if (playerPosition > Console.WindowWidth - 1) 
                    {
                        playerPosition = 0;
                    }
                    break;
                case ConsoleKey.UpArrow:
                    bulletPositions.Add(new int[] { playerPosition, Console.WindowHeight - 5 });
                    break;
            }
        }
    }

    public static void UpdateGame(ref List<int[]> bulletPositions, ref List<int[]> enemyPositions, int playerPosition, ref bool isGameRunning)
    {
        for (int i = bulletPositions.Count - 1; i >= 0; i--)
        {
            bulletPositions[i][1]--; 

            if (bulletPositions[i][1] < 0)
            {
                bulletPositions.RemoveAt(i);
            }
        }

        // aktualizacja pozycji przeciwników
        for (int i = enemyPositions.Count - 1; i >= 0; i--)
        {
            enemyPositions[i][1]++;
            if (enemyPositions[i][1] >= Console.WindowHeight)
            {
                enemyPositions.RemoveAt(i);
            }
        }

        // generowanie nowych przeciwników
        if (rand.Next(100) < 55) // 55% szans na generowanie nowego przeciwnika w każdej klatce
        {
            char[] enemyTypes = { '&', '%', '#' };
            int x = rand.Next(Console.WindowWidth);
            int y = 0; // zawsze na górze ekranu
            char type = enemyTypes[rand.Next(enemyTypes.Length)];
            enemyPositions.Add(new int[] { x, y, type });
        }

        // sprawdzanie kolizji pocisków z przeciwnikami
        for (int i = bulletPositions.Count - 1; i >= 0; i--)
        {
            for (int j = enemyPositions.Count - 1; j >= 0; j--)
            {
                if (bulletPositions[i][0] == enemyPositions[j][0] && bulletPositions[i][1] == enemyPositions[j][1])
                {
                    bulletPositions.RemoveAt(i);
                    enemyPositions.RemoveAt(j);
                    score++; 
                    break;
                }
            }
        }

        for (int i = enemyPositions.Count - 1; i >= 0; i--)
        {
            int[] enemy = enemyPositions[i];
            if (enemy[1] == Console.WindowHeight - 2 && enemy[0] == playerPosition)
            {
                Game.playerHP -= 25f; // odejmowanie HP
                enemyPositions.RemoveAt(i); 

                if (Game.playerHP <= 0)
                {
                    isGameRunning = false; // HP spada do 0
                    return;
                }

            }
        }
    }
}

class Render
{
    public static void RenderGame(int playerPosition, int oldPlayerPosition, List<int[]> bulletPositions, List<int[]> oldBulletPositions, List<int[]> enemyPositions, List<int[]> oldEnemyPositions)
    {
        if (playerPosition != oldPlayerPosition)
        {
            Console.SetCursorPosition(oldPlayerPosition, Console.WindowHeight - 5);
            Console.Write(" ");
            Console.SetCursorPosition(playerPosition, Console.WindowHeight - 5);
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("A"); // pomyśleć o jakims lepszym ASCII - dalej myśle
            Console.ResetColor();
        }

        // czyszczenie starych pozycji pocisków
        foreach (var oldBulletPos in oldBulletPositions)
        {
            if (oldBulletPos[1] >= 0 && oldBulletPos[1] < Console.WindowHeight)
            {
                Console.SetCursorPosition(oldBulletPos[0], oldBulletPos[1]);
                Console.Write(" ");
            }
        }

        // rysowanie nowych pozycji pocisków
        foreach (var bulletPos in bulletPositions)
        {
            if (bulletPos[1] >= 0 && bulletPos[1] < Console.WindowHeight + 1)
            {
                Console.SetCursorPosition(bulletPos[0], bulletPos[1]);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("^");
                Console.ResetColor();
            }
        }

        // rysowanie nowych przeciwników
        foreach (var enemy in enemyPositions)
        {
            Console.SetCursorPosition(enemy[0], enemy[1]);
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write((char)enemy[2]); // konwersja typu przeciwnika na znak
            Console.ResetColor();
        }

        // czyszczenie starych przeciwników
        foreach (var oldEnemy in oldEnemyPositions)
        {
            if (oldEnemy[1] >= 0 && oldEnemy[1] < Console.WindowHeight)
            {
                Console.SetCursorPosition(oldEnemy[0], oldEnemy[1]);
                Console.Write(" ");
            }
        }


        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        Console.Write(new string(' ', Console.WindowWidth)); // czyszczenie lini zapobiega bugowi w HP
        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        Console.Write($"Zestrzelenia: {Logic.score}   Pilot: {Game.playerName}   Pochodzenie: {Game.playerOrigin}   HP: {Game.playerHP}");

    }
}

class Game
{
    static bool isGameRunning = true;
    static int playerPosition = Console.WindowWidth / 2;
    static List<int[]> bulletPositions = new List<int[]>();
    static List<int[]> oldBulletPositions = new List<int[]>();
    static List<int[]> enemyPositions = new List<int[]>();
    static List<int[]> oldEnemyPositions = new List<int[]>();

    public static float playerHP = 100f;
    public static string playerName = "";
    public static string playerOrigin = "";

    static void Main()
    {
        Console.CursorVisible = false;

        UX();
        Menu();

        while (isGameRunning)
        {
            int oldPlayerPosition = playerPosition;
            oldBulletPositions = bulletPositions.ConvertAll(bullet => new int[] { bullet[0], bullet[1] });
            oldEnemyPositions = enemyPositions.ConvertAll(enemy => new int[] { enemy[0], enemy[1] });


            Logic.HandleInput(ref playerPosition, bulletPositions);
            Logic.UpdateGame(ref bulletPositions, ref enemyPositions, playerPosition, ref isGameRunning);
            Render.RenderGame(playerPosition, oldPlayerPosition, bulletPositions, oldBulletPositions, enemyPositions, oldEnemyPositions);

            oldBulletPositions = bulletPositions;

            Thread.Sleep(20);
        }

        TheEnd();
    }

    //     
    //      UX 
    // 


    static void UX()
    {
        string entropyResult = Multiply("6", "9");
        string entropy = $"Współczynnik entriopi wynosi... {entropyResult}!";
        TypeEffect(entropy, 200);

        string greeting = "Witam Pana Profesora w mojej grze!";
        Console.SetCursorPosition(((Console.WindowWidth / 2) - (greeting.Length / 2)), Console.CursorTop);
        TypeEffect(greeting, 50);

        Console.WriteLine();
        Thread.Sleep(1000);

        string logo = @"

                 ██████╗  █████╗ ██╗      █████╗ ██╗  ██╗██╗   ██╗  ██████╗  ██████╗  ██████╗  ██████╗
                ██╔════╝ ██╔══██╗██║     ██╔══██╗╚██╗██╔╝╚██╗ ██╔╝  ╚════██╗██╔═══██╗██╔═══██╗██╔═══██╗
                ██║  ███╗███████║██║     ███████║ ╚███╔╝  ╚████╔╝    █████╔╝ ██████╔╝ ██████╔╝ ██████╔╝
                ██║   ██║██╔══██║██║     ██╔══██║ ██║██═╗  ╚██╔╝     ╚═══██╗ ╚════██╗ ╚════██╗ ╚════██╗    
                ╚██████╔╝██║  ██║███████╗██║  ██║██╔╝ ██║   ██║     ██████╔╝ ██████╔╝ ██████╔╝ ██████╔
                 ╚═════╝ ╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝     ╚═════╝  ╚═════╝  ╚═════╝  ╚═════╝ ";

        string lengthLogo = "███████████████████████████████████████████████████████████████████████████████████████";
        Console.SetCursorPosition(((Console.WindowWidth / 2) - (lengthLogo.Length / 2)), Console.CursorTop);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(logo);
        Console.ResetColor();

        

        Console.WriteLine();
        Console.WriteLine();

        string loading = "Naciśnij dowolny klawiasz aby kontynuować\n";
        Console.SetCursorPosition(((Console.WindowWidth / 2) - (loading.Length / 2)), Console.CursorTop);
        TypeEffect(loading, 50);
        Console.ReadKey();

        string plot = "Dawno, dawno temu w odległej galaktyce... W świecie gdzie sesja zdawała się trwać dłużej niż maraton reżyserskich wersji filmów Peter'a Jackosn'a, jednoosobowa grupa zdeterminowanych studentów borykała się z największym wyzwaniem... \nZdobyciem aprobaty na projekt zaliczeniowy\n";
        TypeEffect(plot, 80);

        string goal = "Twoim celem jest śrubowanie jak najlepszego wyniku, aż do momentu twojej nieuniknionej śmierci \n(z przecieków powiem że Stwórca nie stworzył tutaj kampanii fabularnej [ehhh])\n";
        TypeEffect(goal, 80);

        Console.SetCursorPosition(((Console.WindowWidth / 2) - (loading.Length / 2)), Console.CursorTop);
        TypeEffect(loading, 50);

        Console.ReadKey();
    }

    public static void TheEnd()
    {

        Console.Clear();

        string end = @"
                                                                 
                                 ███████╗██╗  ██╗███████╗  ███████╗███╗   ██╗██████╗ 
                                 ╚═███╔═╝██║  ██║██╔════╝  ██╔════╝████╗  ██║██╔══██╗
                                   ███║  ███████║█████╗    █████╗  ██╔██╗ ██║██║  ██║
                                   ███║  ██╔══██║██╔══╝    ██╔══╝  ██║╚██╗██║██║  ██║
                                 ███████╗██║  ██║███████╗  ███████╗██║ ╚████║██████╔╝
                                 ╚══════╝╚═╝  ╚═╝╚══════╝  ╚══════╝╚═╝  ╚═══╝╚═════╝ ";
        

        int lengthEnd = 61;
        Console.SetCursorPosition(((Console.WindowWidth / 2) - (lengthEnd / 2)), Console.CursorTop);
        Thread.Sleep(1000);
        Console.WriteLine(end);

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Thread.Sleep(1000);

        string goodBye = $"Nie możesz się jeszcze poddać {playerName}, pozostań zdeterminowany!";
        Console.SetCursorPosition(((Console.WindowWidth / 2) - (goodBye.Length / 2)), Console.CursorTop);
        TypeEffect(goodBye, 100);

        Console.WriteLine();

        string result = $"Twój wynik: {Logic.score}";
        Console.SetCursorPosition(((Console.WindowWidth / 2) - (result.Length / 2)), Console.CursorTop);
        TypeEffect(result, 50);

        Console.ReadKey();
    }

    public static void TypeEffect(string text, int typingSpeed)
    {
        foreach (char c in text)
        {
            Console.Write(c);
            Thread.Sleep(typingSpeed);
        }

        Console.WriteLine();
    }

    static void Menu()
    {
        string[] names = { "Issac", "Ulisses", "Clark", "Müller", "Artem", "Sasza" };
        string[] origins = { "Hanza", "ZSGR", "Republika Chińska", "Nieznane" };

        int nameIndex = 0;
        int originIndex = 0;

        // wybór imienia
        ChooseOption("Wybierz imię pilota: ", names, out nameIndex);

        // wybór pochodzenia
        ChooseOption("Wybierz pochodzenie: ", origins, out originIndex);

        playerName = names[nameIndex];
        playerOrigin = origins[originIndex];

        Console.Clear();

    }

    static void ChooseOption(string prompt, string[] options, out int selectedIndex)
    {
        ConsoleKeyInfo key;
        selectedIndex = 0; // startujemy od pierwszej opcji

        do
        {
            Console.Clear();
            Console.WriteLine(prompt);
            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.Write("-> ");
                }
                else
                {
                    Console.Write("   ");
                }
                Console.WriteLine(options[i]);
            }

            key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.LeftArrow)
            {
                selectedIndex = (selectedIndex - 1 + options.Length) % options.Length; // zapewnia cykliczność wyboru
            }
            else if (key.Key == ConsoleKey.RightArrow)
            {
                selectedIndex = (selectedIndex + 1) % options.Length; // zapewnia cykliczność wyboru
            }
        }
        while (key.Key != ConsoleKey.UpArrow);
    }

    // smaczek dla dataminerów
    static string Multiply(string num1, string num2)
    {
        var base13ToDecimal = new Dictionary<char, int>
        {
            {'0', 0}, {'1', 1}, {'2', 2}, {'3', 3}, {'4', 4}, {'5', 5},
            {'6', 6}, {'7', 7}, {'8', 8}, {'9', 9}, {'A', 10}, {'B', 11}, {'C', 12}
        };
        var decimalToBase13 = new Dictionary<int, char>();
        foreach (var pair in base13ToDecimal)
        {
            decimalToBase13[pair.Value] = pair.Key;
        }

        int decNum1 = base13ToDecimal[num1[0]];
        int decNum2 = base13ToDecimal[num2[0]];
        int decimalResult = decNum1 * decNum2;

        if (decimalResult < 13)
        {
            return decimalToBase13[decimalResult].ToString();
        }
        else
        {
            return decimalToBase13[decimalResult / 13].ToString() + decimalToBase13[decimalResult % 13].ToString();
        }
    }
}


