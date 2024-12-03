using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_4
{
    public abstract class Enemy
    {
        protected string name;
        protected int health;
        protected int damage;

        public string getName() { return name; }
        public int getHealth() { return health; }

        public abstract void takeDamage(int damage);

        public abstract void attack(PlayableCharacter player);

        public Boolean isAlive() { return health > 0; }
    }
    public class Goblin : Enemy
    {
        private GameLogger logger;
        public Goblin()
        {
            this.logger = GameLogger.getInstance();
            this.name = "Гоблин";
            this.health = 50;
            this.damage = 10;
        }

        override public void takeDamage(int damage)
        {
            logger.log(String.Format($"У {name}%s осталось {damage} здоровья"));
            health -= damage;
            if (health > 0)
                logger.log(String.Format($"У {name} осталось {health} здоровья"));
        }

        override public void attack(PlayableCharacter player)
        {
            logger.log(String.Format($"{name} атакует врага {player.getName()}"));
            player.takeDamage(damage);
        }
    }

    public class Dragon : Enemy
    {
        private GameLogger gameLogger;
        private float resistance;

        public Dragon()
        {
            this.gameLogger = GameLogger.getInstance();
            this.name = "Дракон";
            this.resistance = 0.2f;
            this.health = 100;
            this.damage = 30;
        }

        override public void takeDamage(int damage)
        {
            damage = (int)Math.Round(damage * (1 - resistance));
            gameLogger.log(String.Format($"У {name} осталось {damage} здоровья"));
            health -= damage;
            if (health > 0)
                gameLogger.log(String.Format($"У {name} осталось {health} здоровья"));
        }

        override public void attack(PlayableCharacter player)
        {
            gameLogger.log("Дракон дышит огнем!");
            player.takeDamage(damage);
        }
    }
    public class BaseEnemyDecorator : Enemy
    {
        private Enemy wrapee;
        protected GameLogger logger;

        public BaseEnemyDecorator(Enemy wrapee)
        {
            this.wrapee = wrapee;
            this.logger = GameLogger.getInstance();
        }

        public string getName() { return wrapee.getName(); }
        public override void takeDamage(int damage)
        {
            wrapee.takeDamage(damage);
        }

        public int getHealth()
        {
            return wrapee.getHealth();
        }
        public Boolean isAlive()
        {
            return wrapee.isAlive();
        }
        public override void attack(PlayableCharacter player)
        {
            wrapee.attack(player);
        }
    }
    public class LegendaryEnemyDecorator : BaseEnemyDecorator
    {
        private const int ADDITIONAL_DAMAGE = 20;

        public LegendaryEnemyDecorator(Enemy wrapee) : base(wrapee)
        {
        }
        public string getName()
        {
            return $"обладающим Неистовством ветра {base.getName()}";
        }
        override public void attack(PlayableCharacter player)
        {
            base.attack(player);

            logger.log("Враг легендарный и наносит дополнительный урон!!!");
            player.takeDamage(ADDITIONAL_DAMAGE);
        }
    }

    public class WindfuryEnemyDecorator : BaseEnemyDecorator
    {
        public WindfuryEnemyDecorator(Enemy wrapee) : base(wrapee)
        { }

        public string getName()
        {
            return $"Обладающий Неистовством Ветра {base.getName()} ";
        }

        public override void attack(PlayableCharacter player)
        {
            base.attack(player);
            logger.log("Неистовство ветра позволяет врагу атаковать второй раз!!!");
            base.attack(player);

        }
    }
    public class WeaponToEnemyAdapter : Enemy
    {
        private const float DISPEL_PROBABILITY = 0.2f;
        private GameLogger logger;
        private Weapon weapon;
        private string name = "Магическое оружие";

        public WeaponToEnemyAdapter(Weapon weapon)
        {
            this.weapon = weapon;
            this.health = 50;
            this.name = "Магическое оружие";
            this.logger = GameLogger.getInstance();
        }

        override public void takeDamage(int damage)
        {
            logger.log($"{name} получает {damage} урона!");
            health -= damage;

            double dispelRoll = new Random().NextDouble();
            if (dispelRoll <= DISPEL_PROBABILITY)
            {
                logger.log("Атака рассеяла заклятие с оружия!");
                this.health = 0;
            }

            if (health > 0) { logger.log($"У {name} осталось {health} здоровья"); }
        }
        override public void attack(PlayableCharacter player)
        {
            logger.log($"{name} атакует {player.getName()}!");
            player.takeDamage(damage);
        }

    }
}
