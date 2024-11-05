using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TP_Lab3.Bow;

namespace TP_Lab3
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

    public interface Weapon
    {
        int getDamage();
        void use();
    }

    public interface Armor
    {
        float getDefense();
        void use();
    }

    public enum CharacterClass
    {
        WARRIOR = 100,
        THIEF = 90,
        MAGE = 80,
    }
    public class PlayableCharacter
    {
        private GameLogger logger;
        private String name;
        private CharacterClass characterClass;
        private Weapon weapon;
        private Armor armor;
        private int health;

        protected internal PlayableCharacter(Builder builder)
        {
            this.logger = GameLogger.getInstance();
            this.name = builder.Name;
            this.characterClass = builder.CharacterClass;
            this.weapon = builder.Weapon;
            this.armor = builder.Armor;
            this.health = (int)builder.CharacterClass;
        }
        public class Builder
        {

            internal string Name { get; set; }
            internal CharacterClass CharacterClass { get; set; }
            internal Weapon Weapon { get; set; }
            internal Armor Armor { get; set; }

            public Builder SetName(string name)
            {
                this.Name = name;
                return this;
            }

            public Builder SetCharacterClass(CharacterClass characterClass)
            {
                this.CharacterClass = characterClass;
                return this;
            }

            public Builder SetWeapon(Weapon weapon)
            {
                this.Weapon = weapon;
                return this;
            }

            public Builder SetArmor(Armor armor)
            {
                this.Armor = armor;
                return this;
            }

            public PlayableCharacter Build()
            {
                return new PlayableCharacter(this);
            }
        }

        public void takeDamage(int damage)
        {
            int reducedDamage = (int)Math.Round(damage * (1 - armor.getDefense()));

            if (reducedDamage < 0)
                reducedDamage = 0;

            health -= reducedDamage;
            armor.use();
            logger.log(name + " получил урон: " + reducedDamage);

            if (health > 0)
            {
                logger.log(String.Format($"У {name} осталось {health}здоровья"));
            }
        }
        public void attack(Enemy enemy)
        {
            logger.log(String.Format($"{name} атакует врага {enemy.getName()}" ));
            weapon.use();
            enemy.takeDamage(weapon.getDamage());
        }
        public Boolean isAlive()
        {
            return health > 0;
        }

        public String getName()
        {
            return name;
        }
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
        private  GameLogger logger;

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
            if (roll <= criticalChance) {
                logger.log("Критический урон!");
                return damage * criticalModifier;
            }
            return damage;
        }

        public void use()
        {
            logger.log("Выстрел из лука!");
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
            public class HeavyArmor : Armor
            {
                private float defense;
                private GameLogger logger;

                public HeavyArmor()
                {
                    this.logger = GameLogger.getInstance();
                    this.defense = 0.3f;
                }

                public float getDefense()
                {
                    return defense;
                }

                public void use()
                {
                    logger.log("Тяжелая броня блокирует значительную часть урона");
                }
            }
            public class LightArmor : Armor
            {
                private float defense;
                private GameLogger logger;

                public LightArmor()
                {
                    this.logger = GameLogger.getInstance();
                    this.defense = 0.2f;
                }

                public float getDefense()
                {
                    return defense;
                }

                public void use()
                {
                    logger.log("Легкая броня блокирует урон");
                }
            }
            public class Robe : Armor
            {
                private float defense; private GameLogger logger;

                public Robe()
                {
                    this.logger = GameLogger.getInstance();
                    this.defense = 0.1f;
                }

                public float getDefense()
                {
                    return defense;
                }

                public void use()
                {
                    logger.log("Роба блокирует немного урона");
                }
            }
            public interface EquipmentChest
            {

                // возвращает некое оружие
                Weapon getWeapon();

                // возвращает некую броню
                Armor getArmor();

            }

            public class WarriorEquipmentChest : EquipmentChest
            {
                public Weapon getWeapon()
                {
                    return new Sword();
                }
                public Armor getArmor()
                {
                    return new HeavyArmor();
                }
            }

            public class MagicalEquipmentChest : EquipmentChest
            {
                public Weapon getWeapon()
                {
                    return new Staff();
                }

                public Armor getArmor()
                {
                    return new Robe();
                }
            }

            public class ThiefEquipmentChest : EquipmentChest
            {
                public Weapon getWeapon()
                {
                return new Bow();
                }

            public Armor getArmor()
            {
                return new LightArmor();
            }
        }
            public interface Location
            {

                Enemy spawnEnemy();

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

            public class Forest : Location
            {
                public Enemy spawnEnemy()
                {
                    return new Goblin();
                }
            }

        public class DragonBarrow : Location
        {
            public Enemy spawnEnemy()
            {
                return new Dragon();
            }
        }
        
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Создайте своего персонажа:");

            Console.WriteLine("Введите имя:");
            string name = Console.ReadLine();

            Console.WriteLine("Выберите класс из списка: " + string.Join(", ", Enum.GetNames(typeof(CharacterClass))));
            CharacterClass characterClass = (CharacterClass)Enum.Parse(typeof(CharacterClass), Console.ReadLine().ToUpperInvariant());

            EquipmentChest startingEquipmentChest = getChest(characterClass);
            Armor startingArmor = startingEquipmentChest.getArmor();
            Weapon startingWeapon = startingEquipmentChest.getWeapon();

            PlayableCharacter player = new PlayableCharacter.Builder()
            .SetName(name)
            .SetCharacterClass(characterClass)
            .SetArmor(startingArmor)
            .SetWeapon(startingWeapon)
            .Build();

            GameLogger gameLogger = GameLogger.getInstance();
            gameLogger.log($"{name} очнулся на распутье!");

            Console.WriteLine("Куда вы двинетесь? Выберите локацию: (мистический лес, логово дракона)");
            string locationName = Console.ReadLine();
            Location location = getLocation(locationName);

            gameLogger.log($"{name} отправился в {locationName}");
            Enemy enemy = location.spawnEnemy();
            gameLogger.log($"{name} столкнулся с {enemy.getName()}, начинается бой!");

            Random random = new Random();
            while (player.isAlive() && enemy.isAlive())
            {
                Console.WriteLine("Введите что-нибудь чтобы атаковать!");
                Console.ReadLine();
                player.attack(enemy);
                bool stunned = random.NextDouble() > 0.5;
                if (stunned)
                {
                    gameLogger.log($"{enemy.getName()} был оглушен атакой {name}!");
                    continue;
                }
                enemy.attack(player);
            }
            Console.WriteLine();

            if (!player.isAlive())
            {
                gameLogger.log($"{name} был убит...");
                return;
            }
            gameLogger.log($"Злой {enemy.getName()} был побеждён! {name} отправился дальше по тропе судьбы...");


        }
        private static EquipmentChest getChest(CharacterClass characterClass)
        {
            switch (characterClass)
            {
                case CharacterClass.MAGE:
                    return new MagicalEquipmentChest();
                case CharacterClass.WARRIOR:
                    return new WarriorEquipmentChest();
                case CharacterClass.THIEF:
                    return new ThiefEquipmentChest();
                default:
                    throw new ArgumentException(nameof(characterClass));
            }
        }
        private static Location getLocation(string locationName)
        {
            switch (locationName.ToLower())
            {
                case "мистический лес":
                    return new Forest();
                case "логово дракона":
                    return new DragonBarrow();
                default:
                    throw new ArgumentException(nameof(locationName));
            }
        }
    }

    

    // Получаем локацию в зависимости от выбора игрока
    
}

