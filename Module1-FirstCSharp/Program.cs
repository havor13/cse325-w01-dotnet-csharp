// --- Display console output ---
Console.WriteLine("Hello, world!");
Console.Write("Hello");
Console.Write(" ");
Console.Write("world!\n");
Console.WriteLine("Name:\tAlex");
Console.WriteLine("She said, \"Hi!\"");
Console.WriteLine("Path: C:\\Users");
Console.WriteLine("{0} {1}", "Hello", "world!");

// --- Accept user input ---
Console.WriteLine();
Console.Write("What is your favorite color? ");
string color = Console.ReadLine() ?? "";
Console.WriteLine("Oh, I love " + color + " too!");

// --- Manipulate strings ---
Console.Write("What's your name? ");
string name = Console.ReadLine() ?? "";
Console.WriteLine($"Welcome, {name.ToUpper()}!");
Console.WriteLine($"Your name has {name.Length} characters.");
Console.WriteLine("cat".Replace("c", "b"));

// --- Challenge: name and age ---
Console.Write("How old are you? ");
string age = Console.ReadLine() ?? "";
Console.WriteLine($"Hello, {name}! You are {age} years old.");