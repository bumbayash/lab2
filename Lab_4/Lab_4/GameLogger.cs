using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_4
{
    public class GameLogger
    {
        private static GameLogger instance;
        private GameLogger() { }
        public static GameLogger getInstance()
        {
            if (instance == null)
            {
                instance = new GameLogger();
            }
            return instance;
        }

        internal void log(string v)
        {
            Console.WriteLine(v);
        }
    }
}
