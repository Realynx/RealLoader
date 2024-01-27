using System;
using System.Runtime.InteropServices;

public class App
{
    public static void Main(string[] args)
    {
        Console.WriteLine($"{nameof(App)} started - args = [ {string.Join(", ", args)} ]");
    }
}
