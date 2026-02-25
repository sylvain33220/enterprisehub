using System;
using System.Linq;

class Program
{
    static void Main()
    {
        string key = GenerateKey(32);
        Console.WriteLine($"Clé générée : {key}");
    }

    static string GenerateKey(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
                                    .Select(s => s[random.Next(s.Length)])
                                    .ToArray());
    }
}