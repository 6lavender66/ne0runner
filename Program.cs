using System;
using System.Collections.Generic;
using System.Threading;

class Logic
{
    public static void HandleInput(ref int playerPosition, List<int[]> bulletPositions)
    {
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
    }

    public static void UpdateGame(ref List<int[]> bulletPositions)
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
}

class Render
{
    public static void RenderGame(int playerPosition, int oldPlayerPosition, List<int[]> bulletPositions, List<int[]> oldBulletPositions)
    {
        if (playerPosition != oldPlayerPosition)
        {
            Console.SetCursorPosition(oldPlayerPosition, Console.WindowHeight - 1);
            Console.Write(" ");
            Console.SetCursorPosition(playerPosition, Console.WindowHeight - 1);
            Console.Write("A"); // pomyśleć o jakims lepszym ASCII 
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
            if (bulletPos[1] >= 0 && bulletPos[1] < Console.WindowHeight)
            {
                Console.SetCursorPosition(bulletPos[0], bulletPos[1]);
                Console.Write("^");
            }
        }   // dodać aktualizację pozycje przeciwników    }
    }
}

class Game
{
    static bool isGameRunning = true;
    static int playerPosition = Console.WindowWidth / 2;
    static List<int[]> bulletPositions = new List<int[]>();
    static List<int[]> oldBulletPositions = new List<int[]>();

    static void Main()
    {
        Console.CursorVisible = false;
        playerPosition = Console.WindowWidth / 2;

        while (isGameRunning)
        {
            int oldPlayerPosition = playerPosition;
            oldBulletPositions = bulletPositions.ConvertAll(bullet => new int[] { bullet[0], bullet[1] });

            Logic.HandleInput(ref playerPosition, bulletPositions);
            Logic.UpdateGame(ref bulletPositions);
            Render.RenderGame(playerPosition, oldPlayerPosition, bulletPositions, oldBulletPositions);

            oldBulletPositions = bulletPositions;

            Thread.Sleep(20);
        }
    }
}