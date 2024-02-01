using System;
using System.Collections.Generic;
using System.Threading;

class Logic
{
    static int speed = 3; // prędkość poruszania się statku
    static Random rand = new Random();
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

    public static void UpdateGame(ref List<int[]> bulletPositions, ref List<int[]> enemyPositions)
    {
        for (int i = bulletPositions.Count - 1; i >= 0; i--)
        {
            bulletPositions[i][1]--; // wprowadzić stałą szybkość dla pocisków

            if (bulletPositions[i][1] < 0)
            {
                bulletPositions.RemoveAt(i); // oszczędzanie pamięci
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
        if (rand.Next(100) < 10) // 10% szans na generowanie nowego przeciwnika w każdej klatce
        {
            char[] enemyTypes = { '&', '%', '#' };
            int x = rand.Next(Console.WindowWidth);
            int y = 0; // zawsze na górze ekranu
            char type = enemyTypes[rand.Next(enemyTypes.Length)];
            enemyPositions.Add(new int[] { x, y, type });
        }
    }
}

class Render
{
    public static void RenderGame(int playerPosition, int oldPlayerPosition, List<int[]> bulletPositions, List<int[]> oldBulletPositions, List<int[]> enemyPositions, List<int[]> oldEnemyPositions)
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
        }

        // czyszczenie starych przeciwników
        foreach (var enemy in enemyPositions)
        {
            Console.SetCursorPosition(enemy[0], enemy[1]);
            Console.Write((char)enemy[2]); // Konwersja typu przeciwnika na znak
        }

        // rysowanie nowych przeciwników
        foreach (var oldEnemy in oldEnemyPositions)
        {
            if (oldEnemy[1] >= 0 && oldEnemy[1] < Console.WindowHeight)
            {
                Console.SetCursorPosition(oldEnemy[0], oldEnemy[1]);
                Console.Write(" ");
            }
        }
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

    static void Main()
    {
        Console.CursorVisible = false;
        playerPosition = Console.WindowWidth / 2;

        while (isGameRunning)
        {
            int oldPlayerPosition = playerPosition;
            oldBulletPositions = bulletPositions.ConvertAll(bullet => new int[] { bullet[0], bullet[1] });
            oldEnemyPositions = enemyPositions.ConvertAll(enemy => new int[] { enemy[0], enemy[1] });


            Logic.HandleInput(ref playerPosition, bulletPositions);
            Logic.UpdateGame(ref bulletPositions, ref enemyPositions);
            Render.RenderGame(playerPosition, oldPlayerPosition, bulletPositions, oldBulletPositions, enemyPositions, oldEnemyPositions);

            oldBulletPositions = bulletPositions;

            Thread.Sleep(20);
        }
    }
}

//
