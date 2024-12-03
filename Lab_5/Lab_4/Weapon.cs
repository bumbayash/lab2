using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_4
{
    public interface Weapon
    {
        int getDamage();
        void use();
    }
    public class Sword : Weapon
    {
        private int damage;
        private readonly GameLogger logger;

        public Sword()
        {
            this.damage = 20;
            this.logger = GameLogger.getInstance();
        }

        public int getDamage()
        {
            return damage;
        }

        public void use()
        {
            logger.log("Удар мечом!");
        }

    }
    public class Bow : Weapon
    {
        private int damage;
        private double criticalChance;
        private int criticalModifier;
        private GameLogger logger;

        public Bow()
        {
            this.damage = 15;
            this.criticalChance = 0.3;
            this.criticalModifier = 2;
            this.logger = GameLogger.getInstance();
        }

        public int getDamage()
        {
            double roll = new Random().NextDouble();
            if (roll <= criticalChance)
            {
                logger.log("Критический урон!");
                return damage * criticalModifier;
            }
            return damage;
        }

        public void use()
        {
            logger.log("Выстрел из лука!");
        }
    }
    public class Staff : Weapon
    {
        private int damage;
        private double scatter;
        private GameLogger logger;

        public Staff()
        {
            this.damage = 25;
            this.scatter = 0.2;
            this.logger = GameLogger.getInstance();
        }

        public int getDamage()
        {
            double roll = new Random().NextDouble();
            double factor = 1 + (roll * 2 * scatter - scatter);

            return (int)Math.Round(damage * factor);
        }
        public void use()
        {
            logger.log("Воздух накаляется, из посоха вылетает огненный шар!");
        }
    }
}
