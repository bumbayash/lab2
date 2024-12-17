using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Patterns
{
    // Одиночка
    public class GameLogger
    {

        // единственный экземпляр класса
        private static GameLogger instance;

        // конструктор приватный - самостоятельно создать экземпляр класса нельзя
        private GameLogger() { }

        // метод для получения одиночки
        public static GameLogger getInstance()
        {
            if (instance == null)
            {
                instance = new GameLogger();
            }

            return instance;
        }

        // поведение одиночки
        public void log(string message)
        {
            Console.WriteLine("[GAME LOG]: " + message);
        }

    }
    public abstract class Enemy
    {
        protected string name;
        protected int health;
        protected int damage;

        public virtual string getName()
        {
            return name;
        }

        public virtual int getHealth()
        {
            return health;
        }

        public abstract void takeDamage(int damage);

        public abstract void attack(PlayableCharacter player);

        public virtual bool isAlive()
        {
            return health > 0;
        }
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

    public class CharacterClass
    {
        public static CharacterClass WARRIOR = new CharacterClass(100);
        public static CharacterClass THIEF = new CharacterClass(90);
        public static CharacterClass MAGE = new CharacterClass(80);


        private int startingHealth;

        public CharacterClass(int startingHealth)
        {
            this.startingHealth = startingHealth;
        }

        public int getStartingHealth()
        {
            return startingHealth;
        }
        public static string values()
        {
            return "WARRIOR" + ", " + "THIEF" + ", " + "MAGE";
        }
        static public CharacterClass CharacterSelect(string character)
        {
            if (character == "WARRIOR")
            {
                return WARRIOR;
            }
            else if (character == "THIEF")
            {
                return THIEF;
            }
            else if (character == "MAGE")
            {
                return MAGE;
            }
            else 
            {
                Console.WriteLine("Некорректный ввод");
                return CharacterSelect(Console.ReadLine());
            }
        }
            
    }

public class PlayableCharacter
    {

        protected GameLogger logger;
        protected string name;
        protected CharacterClass characterClass;
        protected Weapon weapon;
        protected Armor armor;
        protected int health;

        // конструктор принимает в себя билдера из которого и берет значения полей
        private PlayableCharacter(Builder builder)
        {
            this.logger = GameLogger.getInstance();
            this.name = builder.Name;
            this.characterClass = builder.CharacterClass;
            this.weapon = builder.Weapon;
            this.armor = builder.Armor;
            this.health = characterClass.getStartingHealth();
        }

        // внутренний класс билдер - чтобы удобно создавать персонажей
    public class Builder
    {

            private string name;
            private CharacterClass characterClass;
            private Weapon weapon;
            private Armor armor;

            public string Name { get => name; set => name = value; }
            public CharacterClass CharacterClass { get => characterClass; set => characterClass = value; }
            public Weapon Weapon { get => weapon; set => weapon = value; }
            public Armor Armor { get => armor; set => armor = value; }

            public Builder setName(string name)
        {
            this.name = name;
            return this;
        }

        public Builder setCharacterClass(CharacterClass characterClass)
        {
            this.characterClass = characterClass;
            return this;
        }

        public Builder setWeapon(Weapon weapon)
        {
            this.weapon = weapon;
            return this;
        }

        public Builder setArmor(Armor armor)
        {
            this.armor = armor;
            return this;
        }

        // перенос полей билдера в нового персонажа
        public PlayableCharacter build()
        {
            return new PlayableCharacter(this);
        }

        }

        // поведение персонажа
        public void takeDamage(int damage)
        {
            int reducedDamage = Convert.ToInt32(Math.Round(damage * (1 - armor.getDefense()), 0));

            if (reducedDamage < 0)
                reducedDamage = 0;

            health -= reducedDamage;
            armor.use();
            logger.log(name + " получил урон: " + reducedDamage);

            if (health > 0)
            {
                logger.log(($"У {name} осталось {health} здоровья"));
            }
        }

        // здесь персонаж атакует абстрактного врага
        public void attack(Enemy enemy)
        {
            logger.log(($"{name} атакует врага {enemy.getName()}"));
            weapon.use();
            enemy.takeDamage(weapon.getDamage());
        }

        public bool isAlive()
        {
            return health > 0;
        }

        public string getName()
        {
            return name;
        }

    }
    public class Sword : Weapon
    {

        private int damage;
        private GameLogger logger;

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
            if (roll >= criticalChance)
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

            return Convert.ToInt32(Math.Round(damage * factor, 0));
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

        private float defense;
        private GameLogger logger;

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

        public override void takeDamage(int damage)
        {
            logger.log($"{name} получает {damage} урона!");
            health -= damage;
            if (health > 0)
                logger.log($"У {name} осталось {health} здоровья");
        }

        public override void attack(PlayableCharacter player)
        {
            logger.log($"{name} атакует {player.getName()}!");
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

        public override void takeDamage(int damage)
        {
            damage = Convert.ToInt32(Math.Round(damage * (1 - resistance), 0));
            gameLogger.log($"{name} получает {damage} урона!");
            health -= damage;
            if (health > 0)
                gameLogger.log($"У {name} осталось {health} здоровья");
        }

        public override void attack(PlayableCharacter player)
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
    public class main
    {

        public static void Main(string[] args)
        {
            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine("----------ТЕСТОВЫЙ ПРОГОН ЦЕПОЧКИ ОБЯЗАННОСТЕЙ----------");
            int incomingDamage = 100;
            float debuff = 1.5f;
            bool hasInvulnerability = true;
            int barrierHealth = 100;
            Console.WriteLine(
                $"Тестовые данные: {incomingDamage} входящий урон; {debuff} множитель; {hasInvulnerability} неуязвим; ед. здоровья у щита {barrierHealth}"
            );

            DamageHandler chainStart = new BuffDebuffDamageHandler(debuff);
            DamageHandler chainFinish = chainStart;

            if (hasInvulnerability)
            {
                chainFinish = chainFinish.setNext(new InvulnerabilityDamageHandler());
            }

            if (barrierHealth > 0)
            {
                chainFinish = chainFinish.setNext(new BarrierDamageHandler(barrierHealth));
            }

            incomingDamage = chainStart.handle(incomingDamage);
            GameLogger.getInstance().log($"Итоговый урон: {incomingDamage}");
            Console.WriteLine("----------------ТЕСТОВЫЙ ПРОГОН ЗАВЕРШЕН----------------");
            Console.WriteLine("--------------------------------------------------------");

            GameEventPublisher gameEventPublisher = new GameEventPublisher();

            GameEventListener consoleListener = new GameConsoleEventListener();
            gameEventPublisher.subscribe(GameEvent.GAME_START, consoleListener);
            gameEventPublisher.subscribe(GameEvent.GAME_OVER, consoleListener);
            gameEventPublisher.subscribe(GameEvent.GAME_VICTORY, consoleListener);


            // для работы с очками игрока в качестве имплементации используется кеширующая прокси
            PlayerProfileRepository repository = new PlayerProfileCacheRepository();

            GameEventListener updaterListener = new GameUpdaterEventListener(repository);
            gameEventPublisher.subscribe(GameEvent.GAME_OVER, updaterListener);

            Console.WriteLine("Создайте своего персонажа:");

            Console.WriteLine("Введите имя:");
            string name = Console.ReadLine();

            PlayerProfile playerProfile = repository.getProfile(name);
            Console.WriteLine($"Текущий счет игрока {playerProfile.getName()}: {playerProfile.getScore()}");

            Console.WriteLine("Выберите класс из списка: " + CharacterClass.values());
            CharacterClass characterClass = CharacterClass.CharacterSelect(Console.ReadLine());

            EquipmentChest startingEquipmentChest = getChest(characterClass);
            Armor startingArmor = startingEquipmentChest.getArmor();
            Weapon startingWeapon = startingEquipmentChest.getWeapon();

            PlayableCharacter player =
                new PlayableCharacter.Builder()
                    .setName(name)
                    .setCharacterClass(characterClass)
                    .setArmor(startingArmor)
                    .setWeapon(startingWeapon)
                    .build();

            gameEventPublisher.notifyAll(GameEvent.GAME_START, playerProfile);

            GameLogger gameLogger = GameLogger.getInstance();
            gameLogger.log($"{player.getName()} очнулся на распутье!");

            gameLogger.log($"К {player.getName()} присоединяется компаньон!!!");
            Companion companion = new Companion(characterClass);

            Console.WriteLine("Куда вы двинетесь? Выберите локацию: (мистический лес, проклятый особняк, логово дракона)");
            string locationName = Console.ReadLine();
            Location location = getLocation(locationName);

            gameLogger.log($"{player.getName()} отправился в {locationName}");
            Enemy enemy = location.spawnEnemy();
            // с шансом в 50% игрок встречает сильного врага
            double strongEnemyCurse = new Random().NextDouble();
            if (strongEnemyCurse > 0.5)
            {
                gameLogger.log($"Боги особенно немилостивы к {name}, сегодня его ждет страшная битва...");
                enemy = addEnemyModifiers(enemy);
            }

            gameLogger.log($"У {player.getName()} на пути возникает {enemy.getName()}, начинается бой!");

            Random random = new Random();
            while (player.isAlive() && enemy.isAlive())
            {
                Console.WriteLine("Введите что-нибудь чтобы атаковать!");
                Console.ReadLine();
                player.attack(enemy);
                companion.attack(enemy);
                double stunned = random.NextDouble();
                if (stunned > 0.5)
                {
                    gameLogger.log($"{enemy.getName()} был оглушен атакой {player.getName()}!");
                    continue;
                }
                enemy.attack(player);
            }

            Console.WriteLine();

            if (!player.isAlive())
            {
                gameLogger.log($"{player.getName()} был убит...");

                gameEventPublisher.notifyAll(GameEvent.GAME_OVER, playerProfile);

                repository.updateHighScore(name, 0);
                playerProfile = repository.getProfile(name);
                Console.WriteLine($"Новый счет игрока {playerProfile.getName()}: {playerProfile.getScore()}");
                return;
            }

            gameLogger.log($"Злой {enemy.getName()} был побежден! {player.getName()} отправился дальше по тропе судьбы...");

            // обновляет счет игрока в зависимости от локации и модификаторов врага,
            // после чего выводит актуальный счет
            int score = getScore(locationName, strongEnemyCurse);
            repository.updateHighScore(name, score);
            playerProfile = repository.getProfile(name);
            Console.WriteLine($"Новый счет игрока {playerProfile.getName()}: {playerProfile.getScore()}");

            gameEventPublisher.notifyAll(GameEvent.GAME_VICTORY, playerProfile);
        }

        private static Enemy addEnemyModifiers(Enemy enemy)
        {
            BaseEnemyDecorator decorator = new BaseEnemyDecorator(enemy);

            // с вероятностью 30% на врага накладывается оба модификатора
            double secondModifierProbablity = 0.3;
            bool secondModiferProc = secondModifierProbablity.CompareTo(new Random().NextDouble()) <= 0;
            if (new Random().NextDouble() > 0.5)
            {
                decorator = new LegendaryEnemyDecorator(decorator);
                if (secondModiferProc)
                    decorator = new WindfuryEnemyDecorator(decorator);
            }
            else
            {
                decorator = new WindfuryEnemyDecorator(decorator);
                if (secondModiferProc)
                    decorator = new LegendaryEnemyDecorator(decorator);
            }

            return decorator;
        }

        // получаем стартовый сундук в зависимости от класса персонажа
        private static EquipmentChest getChest(CharacterClass characterClass)
        {
            if (characterClass == CharacterClass.WARRIOR)
            {
                return new WarriorEquipmentChest();
            }
            if (characterClass == CharacterClass.THIEF)
            {
                return new ThiefEquipmentChest();
            }
            if (characterClass == CharacterClass.MAGE)
            {
                return new MagicalEquipmentChest();
            }
            throw new Exception();
        }

        // получаем локацию в зависимости от выбора игрока
        private static Location getLocation(string locationName)
        {
            switch (locationName.ToLower())
            {
                case "мистический лес":
                    return new Forest();
                case "проклятый особняк":
                    return new HauntedManor();
                case "логово дракона":
                    return new DragonBarrow();
                default:
                    throw new Exception();
            }
        }

        private static int getScore(string locationName, double strongEnemy)
        {
            int baseScore = 0;
            switch (locationName.ToLower())
            {
                case "мистический лес":
                    baseScore = 10;
                    break;
                case "проклятый особняк":
                    baseScore = 50;
                    break;
                case "логово дракона":
                    baseScore = 100;
                    break;
                default:
                    throw new Exception();
            }

            if (strongEnemy > 0.5)
                return baseScore * 2;

            return baseScore;
        }

    }


    // Декораторы
    public class BaseEnemyDecorator : Enemy
    {

        // оборачиваемый объект, может быть как конкретным врагом, так и другим декоратором
        private Enemy wrapee;

        protected GameLogger logger;

        public BaseEnemyDecorator(Enemy wrapee)
        {
            this.wrapee = wrapee;
            this.logger = GameLogger.getInstance();
        }

        // все методы абстрактного класса перезаписываются -
        // их поведение берется из другого объекта;
        // при этом в абстрактном декораторе новое поведение не добавляется, он
        // лишь определяет структуру компоновки объектов
        public override string getName()
        {
            return wrapee.getName();
        }

        public override int getHealth()
        {
            return wrapee.getHealth();
        }

        public override bool isAlive()
        {
            return wrapee.isAlive();
        }

        public override void takeDamage(int damage)
        {
            wrapee.takeDamage(damage);
        }

        public override void attack(PlayableCharacter player)
        {
            wrapee.attack(player);
        }

    }

    public class LegendaryEnemyDecorator : BaseEnemyDecorator
    {

        private static int ADDITIONAL_DAMAGE = 20;

        public LegendaryEnemyDecorator(Enemy wrapee) : base(wrapee) { }

        // данный декоратор добавляет легендарный модификатор к имени врага
       public override string getName()
        {
            return ($"Легендарный {base.getName()}");
        }

        // основа нового поведения - нанесение дополнительного урона
       public override void attack(PlayableCharacter player)
        {
            // здесь вызов отправляется базовому классу
            base.attack(player);

            // а вот здесь добавляется поведение к методу
            logger.log("Враг легендарный и наносит дополнительный урон!!!");
            player.takeDamage(ADDITIONAL_DAMAGE);
        }

    }

    public class WindfuryEnemyDecorator : BaseEnemyDecorator
    {

        public WindfuryEnemyDecorator(Enemy wrapee) : base(wrapee) { }

        public override string getName()
        {
            return ($"Обладающий Неистовством Ветра {base.getName()}");
        }

        public override void attack(PlayableCharacter player)
        {
            base.attack(player);

            logger.log("Неистовство ветра позволяет врагу атаковать второй раз!!!");
            base.attack(player);
        }

    }

    // Адаптер
    public class WeaponToEnemyAdapter : Enemy
    {

        private static float DISPEL_PROBABILITY = 0.2f;

        private GameLogger logger;
        private Weapon weapon;

        public WeaponToEnemyAdapter(Weapon weapon)
        {
            this.logger = GameLogger.getInstance();
            this.name = "Магическое оружие";
            this.health = 50;
            this.weapon = weapon;
        }

        public override void takeDamage(int damage)
        {
            logger.log(($"{name} получает {damage} урона!"));
            health -= damage;

            double dispelRoll = new Random().NextDouble();
            if (dispelRoll.CompareTo(DISPEL_PROBABILITY) <= 0)
            {
                logger.log("Атака рассеяла заклятие с оружия!");
                this.health = 0;
            }

            if (health > 0)
                logger.log(($"У {name} осталось {health} здоровья"));
        }

        public override void attack(PlayableCharacter player)
        {
            logger.log(($"{name} атакует {player.getName()}!"));
            player.takeDamage(damage);
        }

    }

    // Фасад
    public class WeaponEquipmentFacade
    {

        private EquipmentChest equipmentChest;

        public WeaponEquipmentFacade(CharacterClass characterClass)
        {
            if (characterClass == CharacterClass.MAGE)
            {
                equipmentChest = new MagicalEquipmentChest();
            }
            else if (characterClass == CharacterClass.WARRIOR)
            {
                equipmentChest = new WarriorEquipmentChest();
            }
            else if (characterClass == CharacterClass.THIEF)
            {
                equipmentChest = new ThiefEquipmentChest();
            }
            else
            {
                Console.WriteLine("Ошибка в фасаде");
            }
        }

        public Weapon getWeapon()
        {
            return equipmentChest.getWeapon();
        }

    }

    // в проклятом особняке в качестве врага выступает случайное зачарованное оружие
    // при этом для получения оружия используется паттерн фасад,
    // а для работы с оружием как с врагом - паттерн адаптер
    public class HauntedManor : Location
        {

        private WeaponEquipmentFacade weaponEquipmentFacade;

        public HauntedManor()
        {
            int random = new Random().Next(CharacterClass.values().Split().Length);
            CharacterClass randomClass = CharacterClass.CharacterSelect(CharacterClass.values().Split()[random]);
            this.weaponEquipmentFacade = new WeaponEquipmentFacade(randomClass);
        }

        public Enemy spawnEnemy()
        {
            Weapon weapon = weaponEquipmentFacade.getWeapon();
            //noinspection UnnecessaryLocalVariable
            Enemy enchantedWeapon = new WeaponToEnemyAdapter(weapon);
            return enchantedWeapon;
        }

    }

    public class PlayerProfile // Serializable
    {

        private static long serialVersionUID = 1540076508561334783L;

        private string name;
        private int score;

        public PlayerProfile(string name, int score)
        {
            this.name = name;
            this.score = score;
        }

        public string getName()
        {
            return name;
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public int getScore()
        {
            return score;
        }

        public void setScore(int score)
        {
            this.score = score;
        }

    }

    public interface PlayerProfileRepository
    {
        PlayerProfile getProfile(string name);

        void updateHighScore(string name, int score);

    }

    // репозиторий для работы с профилями игрока - в качестве БД выступает простой файл
    public class PlayerProfileDBRepository : PlayerProfileRepository
    {
        private static string SCORE_FILENAME = @"score.txt";

        public PlayerProfileDBRepository()
        {
            try
            {
                if (File.Exists(SCORE_FILENAME))
                    return;

                //File.CreateText(SCORE_FILENAME);
                Dictionary<string, PlayerProfile> emptyDb = new Dictionary<string, PlayerProfile>();
                update(emptyDb);
            }
            catch (IOException rethrow)
            {
                throw new Exception(rethrow.ToString());
            }
        }

        public PlayerProfile getProfile(string name)
        {
            Console.WriteLine("Из базы данных достается информация о профилях игроков..");
            Dictionary<string, PlayerProfile> playerProfileMap = findAll();
            if (!playerProfileMap.Keys.Contains(name))
            {
                Console.WriteLine("В базе данных создается новый профиль...");
                playerProfileMap.Add(name, new PlayerProfile(name, 0));
            }
            update(playerProfileMap);
            return playerProfileMap[name];
        }

        public void updateHighScore(string name, int score)
        {
            Console.WriteLine("В базе данных обновляются очки игрока...");
            Dictionary<string, PlayerProfile> playerProfileMap = findAll();
            if (!playerProfileMap.Keys.Contains(name))
            {
                Console.WriteLine("В базе данных создается новый профиль...");
                playerProfileMap.Add(name, new PlayerProfile(name, 0));

            }
            playerProfileMap[name].setScore(score);
            update(playerProfileMap);
        }

        private Dictionary<string, PlayerProfile> findAll()
        {
            try
            {
                StreamReader sr = new StreamReader(SCORE_FILENAME);
                Dictionary<string, PlayerProfile> scores = new Dictionary<string, PlayerProfile>();
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    scores.Add(s.Split()[0], new PlayerProfile(s.Split()[1], Convert.ToInt32(s.Split()[2])));
                }
                sr.Close();
                return scores;
            } catch (Exception rethrow)
            {
                throw new Exception(rethrow.ToString());
            }
        }

        private void update(Dictionary<string, PlayerProfile> scores)
        {
            try 
            {
                StreamWriter sw = new StreamWriter(SCORE_FILENAME);
                int l = scores.Count;
                List<string> keys = scores.Keys.ToList();
                for (int i = 0; i < l; i++)
                { sw.WriteLine(keys[i].ToString() + " " + scores[keys[i]].getName() + " " + scores[keys[i]].getScore().ToString());}
                sw.Close();
            } catch (Exception rethrow)
            {
                throw new Exception(rethrow.ToString());
            }
        }

    }

    // кеширующая прокси - методы базы данных вызываются только в случае, если
    // профиль не был закеширован
    public class PlayerProfileCacheRepository : PlayerProfileRepository
    {

        private Dictionary<string, PlayerProfile> cachedProfiles;
        private PlayerProfileDBRepository database;

        public PlayerProfileCacheRepository()
        {
            cachedProfiles = new Dictionary<string, PlayerProfile>();
            this.database = new PlayerProfileDBRepository();
        }

        public PlayerProfile getProfile(string name)
        {
            // если профиль не закеширован - достает из базы и кеширует
            if (!cachedProfiles.Keys.Contains(name))
            {
                Console.WriteLine("Профиль игрока не найден в кеше...");
                PlayerProfile playerProfileFromDatabase = database.getProfile(name);
                cachedProfiles.Add(name, playerProfileFromDatabase);
            }

            // достает закешированный профиль
            Console.WriteLine("Профиль игрока достается из кеша...");
            return cachedProfiles[name];
        }

        public void updateHighScore(string name, int score)
        {
            if (!cachedProfiles.Keys.Contains(name))
            {
                Console.WriteLine("Профиль игрока не найден в кеше...");
                database.updateHighScore(name, score);
                PlayerProfile playerProfileFromDatabase = database.getProfile(name);
                cachedProfiles.Add(name, playerProfileFromDatabase);
                return;
            }

            // write-through кеш - пишет сначала в кеш, а потом в БД
            PlayerProfile cachedProfile = cachedProfiles[name];
            cachedProfile.setScore(score);
            database.updateHighScore(name, score);
        }

    }

    public interface AttackStrategy
    {

        void execute(Enemy enemy);

    }

    public class MeleeAttackStrategy : AttackStrategy
    {
        public void execute(Enemy enemy)
        {
            GameLogger.getInstance().log("Компаньон атакует в ближнем бою!");
            enemy.takeDamage(10);
        }

    }

    public class RangedAttackStrategy : AttackStrategy
    {
        public void execute(Enemy enemy)
        {
            GameLogger.getInstance().log("Компаньон атакует издалека!");
            enemy.takeDamage(7);
        }

    }

    public class Companion
    {

        private AttackStrategy attackStrategy;

        public Companion(CharacterClass companionToClass)
        {
            if (companionToClass == CharacterClass.MAGE)
            {
                attackStrategy = new MeleeAttackStrategy();
            }
            else if (companionToClass == CharacterClass.WARRIOR)
            {
                attackStrategy = new RangedAttackStrategy();
            }
            else if (companionToClass == CharacterClass.THIEF)
            {
                if (new Random().NextDouble() >= 0.5)
                {
                    attackStrategy = new RangedAttackStrategy();
                }
                else
                {
                    attackStrategy = new MeleeAttackStrategy();
                }
            }
            else
            {
                Console.WriteLine("Ошибка в выборе стратегии компаньона");
            }
        }

        public void attack(Enemy enemy)
        {
            this.attackStrategy.execute(enemy);
        }

    }

    public class GameEvent
    {
        public static GameEvent GAME_START = new GameEvent();
        public static GameEvent GAME_OVER = new GameEvent();
        public static GameEvent GAME_VICTORY = new GameEvent();
    }

    public class GameEventPublisher
    {

        private Dictionary<GameEvent, List<GameEventListener>> gameEventsListeners = new Dictionary<GameEvent, List<GameEventListener>>();

        public void subscribe(GameEvent typeToSubscribeTo, GameEventListener subscriber)
            {
            if (!gameEventsListeners.Keys.Contains(typeToSubscribeTo))
                {
                    gameEventsListeners[typeToSubscribeTo] =  new List<GameEventListener>();
                }

                List<GameEventListener> eventListeners = gameEventsListeners[typeToSubscribeTo];

                eventListeners.Add(subscriber);
            }

            public void notifyAll(GameEvent notifyEventType, PlayerProfile playerProfile)
            {
                List<GameEventListener> eventListeners = gameEventsListeners[notifyEventType];
                if (eventListeners.Count() == 0)
                {
                    return;
                }

                foreach (GameEventListener listener in eventListeners)
                {
                    listener.update(notifyEventType, playerProfile);
                }
            }

    }

    public interface GameEventListener
    {
        void update(GameEvent g_event, PlayerProfile playerProfile);

    }

    public class GameConsoleEventListener : GameEventListener
    {
        public void update(GameEvent g_event, PlayerProfile playerProfile)
        {
            if (g_event == GameEvent.GAME_START)
            {
                Console.WriteLine($"В игре произошло событие: GAME_START, для игрока {playerProfile.getName()}");
            }
            else if (g_event == GameEvent.GAME_VICTORY)
            {
                Console.WriteLine($"В игре произошло событие: GAME_VICTORY, для игрока {playerProfile.getName()}");
            }
            else if (g_event == GameEvent.GAME_OVER)
            {
                Console.WriteLine($"В игре произошло событие: GAME_OVER, для игрока {playerProfile.getName()}");
            }
            else
            {
                Console.WriteLine("Ошибка в событии");
            }
        }

    }

    public class GameUpdaterEventListener : GameEventListener
    {

        private PlayerProfileRepository playerProfileRepository;

        public GameUpdaterEventListener(PlayerProfileRepository playerProfileRepository)
        {
            this.playerProfileRepository = playerProfileRepository;
        }

        public void update(GameEvent g_event, PlayerProfile playerProfile)
        {
            if (g_event == GameEvent.GAME_OVER)
                playerProfileRepository.updateHighScore(playerProfile.getName(), 0);
        }

    }

    public interface DamageHandler
    {

        /**
         * Указывает для текущего обработчика следующего.
         *
         * @param nextHandler обработчик, следующий для текущего
         * @return nextHandler
         */
        DamageHandler setNext(DamageHandler nextHandler);

        int handle(int incomingDamage);

    }

    public class AbstractDamageHandler : DamageHandler
    {

        private DamageHandler nextHandler;

        public DamageHandler setNext(DamageHandler nextHandler)
        {
            this.nextHandler = nextHandler;
            return nextHandler;
        }

        public virtual int handle(int incomingDamage)
        {
            if (nextHandler is null)
                return incomingDamage;

            return nextHandler.handle(incomingDamage);
        }

    }

    public class BarrierDamageHandler : AbstractDamageHandler
    {

        private int barrierHealth;

        public BarrierDamageHandler(int barrierHealth)
        {
            this.barrierHealth = barrierHealth;
        }

        public override int handle(int incomingDamage)
        {
            if (barrierHealth == 0)
            {
                GameLogger.getInstance().log("Барьер персонажа не может заблокировать урон");
                return base.handle(incomingDamage);
            }

            GameLogger.getInstance().log($"У персонажа есть барьер с прочностью {barrierHealth}");
            if (barrierHealth >= incomingDamage)
            {
                barrierHealth -= incomingDamage;
                GameLogger.getInstance().log("Барьер полностью поглотил урон");
                GameLogger.getInstance().log($"Урон стал {0}");
                return base.handle(0);
            }

            incomingDamage = incomingDamage - barrierHealth;
            barrierHealth = 0;
            GameLogger.getInstance().log("Барьер персонажа был рассеян");
            GameLogger.getInstance().log($"Урон стал {incomingDamage}");
            return base.handle(incomingDamage);
        }

    }

    public class BuffDebuffDamageHandler : AbstractDamageHandler
    {

        private double multiplier;

        public BuffDebuffDamageHandler(float multiplier)
        {
            this.multiplier = multiplier;
        }
        public override int handle(int incomingDamage)
        {
            GameLogger.getInstance().log($"На персонажа наложен эффект который модифицирует урон в {multiplier} раз!");

            incomingDamage = Convert.ToInt32(Math.Round(incomingDamage * multiplier, 0));

            GameLogger.getInstance().log($"Урон стал {incomingDamage}");
            return base.handle(incomingDamage);
        }

    }

    public class InvulnerabilityDamageHandler : AbstractDamageHandler
    {

        public override int handle(int incomingDamage)
        {
            GameLogger.getInstance().log("На персонажа наложена неуязвимость!!!");
            return 0;
        }

    }
}

