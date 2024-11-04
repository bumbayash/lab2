using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TP_Lab2
{
    public class Person
    {

        private string name;
        private  Animal companion;

    public Person(string name, Animal companion)
        {
            this.name = name;
            this.companion = companion;
        }

        public void getCompanionInfo()
        {
            Console.WriteLine("--------------------");
            Console.WriteLine($"Companion for {name} is named {companion.Name}");
            Console.WriteLine($"It is {companion.Age} years old");
            Console.WriteLine("And it sounds like this:");
            companion.makeSound();
            Console.WriteLine("--------------------");
        }

    }
    public interface IAlive
    {
        void makeSound();
        int GetAge();
        string GetName();
    }
    public abstract class Animal
    {
        private string shape;
        private int age;
        private string name;

        public Animal()
        {
        }

        public Animal(string shape, int age, string name)
        {
            if (age < 0) { age = 0; }
            else { this.age = age; }
            this.shape = shape;
            this.name = name;
        }

        public virtual void makeSound()
        {
            Console.WriteLine(shape);
        }

        public string Shape { get { return shape; } set { shape = value; } }
        public int Age { get { return age; } set { age = value; } }
        public string Name { get { return name; } set { name = value; } }
    }
    public class Dog : Animal
    {
        public Dog(int age, string name)
        {
            if (age < 0) { age = 0; }
            else { this.Age = age; }
            this.Name = name;
        }
        public override void makeSound()
        {
            Console.WriteLine(
                "\n" +
                "           ^\\\n" +
                " /        //o__o\n" +
                "/\\       /  __/\n" +
                "\\ \\______\\  /     -ARF!\n" +
                " \\         /\n" +
                "  \\ \\----\\ \\\n" +
                "   \\_\\_   \\_\\_ \n" 
                );
        }

    }
    public class  Cat : Animal
    {
        public Cat(int age, string name)
        {
            if (age < 0) { age = 0; }
            else { this.Age = age; }
            this.Name = name;
        }
        public override void makeSound()
        {
            Random rnd = new Random();
            int value = rnd.Next();
            if (value % 2 == 0)
            {
                Console.WriteLine(
                    "" +
                    "    /\\___/\\\n" +
                    "   /       \\\n" +
                    "  l  u   u  l\n" +
                    "--l----*----l--\n" +
                    "   \\   w   /     - Meow!\n" +
                    "     ======\n" +
                    "   /       \\ __\n" +
                    "   l        l\\ \\\n" +
                    "   l        l/ /\n" +
                    "   l  l l   l /\n" +
                    "   \\ ml lm /_/ \n");
            }
            else
            {
                Console.WriteLine(
                    "" +
                "      /\\_/\\\n" +
                " /\\  / o o \\\n" +
                "//\\\\ \\~(*)~/\n" +
                "`  \\/   ^ /\n" +
                "   | \\|| ||\n" +
                "   \\ '|| ||\n" +
                "    \\)()-()) [the cat is not in the mood to meow]\n");
            }
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Animal sally = new Dog(10, "Sally");
            Animal george = new Cat(8, "George");

            Person billy = new Person("Bill", sally);
            Person jessica = new Person("Jessica", george);

            billy.getCompanionInfo();
            jessica.getCompanionInfo();
        }
    }
}
