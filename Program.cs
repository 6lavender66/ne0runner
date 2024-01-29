using System;
using System.Collections.Generic;
using System.Threading;

class SpaceInvaders
{
    static void Main()
    {
        bool isGameRunning = true;
        int playerPosition = Console.WindowWidth / 2;
        List<int[]> bulletPositions = new List<int[]>(); // pociski jako lista tablic, powinno działać wydajniej

        while (isGameRunning)
        {
            // obsługa klawiatury, 3 klawisze 
            // !!! ZMIENIĆ SPOSÓB WPROWADZANIA Z HOLD NA CLICK !!!

            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.LeftArrow:
                        playerPosition = Math.Max(0, playerPosition - 1);
                        break;
                    case ConsoleKey.RightArrow:
                        playerPosition = Math.Min(Console.WindowWidth - 1, playerPosition + 1);
                        break;
                    case ConsoleKey.UpArrow:
                        bulletPositions.Add(new int[] { playerPosition, Console.WindowHeight - 2 });
                        break;
                }
            }

            // Aktualizacja gry
            UpdateGame(ref bulletPositions);

            // Renderowanie
            Console.Clear();
            RenderGame(playerPosition, bulletPositions);

            Thread.Sleep(100);
        }
    }

    // odświeżanie
    static void UpdateGame(ref List<int[]> bulletPositions)
    {
        for (int i = bulletPositions.Count - 1; i >= 0; i--)
        {
            bulletPositions[i][1]--; // wprowadzić stałą szybkość dla pocisków

            if (bulletPositions[i][1] < 0)
            {
                bulletPositions.RemoveAt(i); // oszczędzanie pamięci
            }
        }
        // dodać aktualizację pozycje przeciwników
    }

    // renderowanie
    static void RenderGame(int playerPosition, List<int[]> bulletPositions)
    {
           Console.SetCursorPosition(playerPosition, Console.WindowHeight - 1);
        Console.Write("A"); // pomyśleć o jakims lepszym ASCII 

        foreach (var bulletPos in bulletPositions)
        {
            Console.SetCursorPosition(bulletPos[0], bulletPos[1]);
            Console.Write("^");
        }

    }
}

// napisz więcej funkcji, jeśli chcesz żeby ta gra jakkolwiek była grywalna