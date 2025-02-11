using System;
using System.Collections.Generic;
using System.Linq;

namespace PacmanGame
{
    class Program
    {
        static void Main(string[] args)
        {
            // Размер карты
            int width = 10;
            int height = 10;

            while (true)
            {
                // Создание лабиринта
                char[,] maze = CreateMaze(width, height);

                // Генерация начальной позиции игрока
                int playerX = 1, playerY = 1;

                // Генерация монеток
                List<(int x, int y)> coins = GenerateCoins(maze, 10, playerX, playerY);

                // Счетчик монеток
                int coinCount = 0;

                // Основной игровой цикл
                while (true)
                {
                    // Очистка экрана
                    Console.Clear();

                    // Отображение лабиринта
                    DrawMaze(maze, playerX, playerY, coins);

                    // Проверка победы
                    if (coins.Count == 0)
                    {
                        Console.WriteLine("Вы победили! Все монетки собраны!");
                        Console.WriteLine("Нажмите 'R', чтобы начать заново, или любую другую клавишу, чтобы выйти.");
                        ConsoleKeyInfo restartKey = Console.ReadKey(true);

                        if (restartKey.Key == ConsoleKey.R)
                        {
                            break; // Перезапуск игры
                        }
                        else
                        {
                            Console.WriteLine("Спасибо за игру! До свидания!");
                            return; // Выход из игры
                        }
                    }

                    // Обработка ввода
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    (int newX, int newY) = GetNewPosition(playerX, playerY, key.Key);

                    // Проверка, можно ли переместиться в новую позицию
                    if (newX >= 0 && newX < width && newY >= 0 && newY < height && maze[newY, newX] != '#')
                    {
                        playerX = newX;
                        playerY = newY;

                        // Проверка, находится ли игрок на монетке
                        if (coins.Any(c => c.x == playerX && c.y == playerY))
                        {
                            coins.RemoveAll(c => c.x == playerX && c.y == playerY);
                            coinCount++;
                            Console.WriteLine($"Монетка собрана! Всего монеток: {coinCount}");
                        }
                    }
                }
            }
        }

        // Функция создания лабиринта
        static char[,] CreateMaze(int width, int height)
        {
            char[,] maze = new char[height, width];
            Random random = new Random();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    {
                        maze[y, x] = '#'; // Стены по периметру
                    }
                    else
                    {
                        maze[y, x] = random.Next(3) == 0 ? '#' : ' '; // Случайные стены внутри
                    }
                }
            }

            return maze;
        }

        // Функция проверки достижимости клетки
        static bool IsReachable(char[,] maze, int startX, int startY, int targetX, int targetY)
        {
            int width = maze.GetLength(1);
            int height = maze.GetLength(0);

            bool[,] visited = new bool[height, width];
            Stack<(int x, int y)> stack = new Stack<(int x, int y)>();
            stack.Push((startX, startY));
            visited[startY, startX] = true;

            while (stack.Count > 0)
            {
                var (x, y) = stack.Pop();

                if (x == targetX && y == targetY)
                {
                    return true;
                }

                (int, int)[] directions = { (-1, 0), (1, 0), (0, -1), (0, 1) };
                foreach (var (dx, dy) in directions)
                {
                    int newX = x + dx;
                    int newY = y + dy;

                    if (newX >= 0 && newX < width && newY >= 0 && newY < height &&
                        maze[newY, newX] != '#' && !visited[newY, newX])
                    {
                        stack.Push((newX, newY));
                        visited[newY, newX] = true;
                    }
                }
            }

            return false;
        }

        // Функция генерации монеток
        static List<(int x, int y)> GenerateCoins(char[,] maze, int count, int playerX, int playerY)
        {
            Random random = new Random();
            List<(int x, int y)> coins = new List<(int x, int y)>();

            while (coins.Count < count)
            {
                int x = random.Next(maze.GetLength(1));
                int y = random.Next(maze.GetLength(0));

                if (maze[y, x] == ' ' && !coins.Any(c => c.x == x && c.y == y) && IsReachable(maze, playerX, playerY, x, y))
                {
                    coins.Add((x, y));
                }
            }

            return coins;
        }

        // Функция отрисовки лабиринта
        static void DrawMaze(char[,] maze, int playerX, int playerY, List<(int x, int y)> coins)
        {
            for (int y = 0; y < maze.GetLength(0); y++)
            {
                for (int x = 0; x < maze.GetLength(1); x++)
                {
                    if (x == playerX && y == playerY)
                    {
                        Console.Write('@'); // Игрок
                    }
                    else if (coins.Any(c => c.x == x && c.y == y))
                    {
                        Console.Write('.'); // Монетка
                    }
                    else
                    {
                        Console.Write(maze[y, x]); // Лабиринт
                    }
                }
                Console.WriteLine();
            }
        }

        // Функция получения новой позиции игрока
        static (int, int) GetNewPosition(int x, int y, ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    return (x, y - 1); // Вверх
                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    return (x, y + 1); // Вниз
                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    return (x - 1, y); // Влево
                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    return (x + 1, y); // Вправо
                default:
                    return (x, y); // Нет движения
            }
        }
    }
}