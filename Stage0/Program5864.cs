using System;
namespace Stage0
{
    partial class Program
    {
        private static void Main(string[] args)
        {
            welcome5864();
            Welcome8145();
            Console.ReadKey();
        }

        static partial void Welcome8145();

        private static void welcome5864()
        {
            Console.WriteLine("Enter your name: ");
            string name = Console.ReadLine();
            Console.WriteLine("{0}, welcome to my first console aplication", name);
        }

}

}
