using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Lab_4
{


    public enum CharacterClass
    {
        WARRIOR = 100,
        THIEF = 90,
        MAGE = 80,
    }
    [Serializable]
    public class PlayerProfile
    {
        public string Name { get; set; }
        public int Score { get; set; }

        public PlayerProfile(string name, int score)
        {
            Name = name;
            Score = score;
        }
    }
    public interface IPlayerProfileRepository
    {
        PlayerProfile GetProfile(string name);
        void UpdateHighScore(string name, int score);
    }
    public class PlayerProfileDBRepository : IPlayerProfileRepository
    {
        private const string SCORE_FILE_NAME = "score.bin";
        private readonly string _filePath = Path.Combine(Environment.CurrentDirectory, SCORE_FILE_NAME);

        public PlayerProfileDBRepository()
        {
            if (!File.Exists(_filePath))
            {
                using (var fs = File.Create(_filePath)) { }
                Update(new Dictionary<string, PlayerProfile>());
            }
        }

        public PlayerProfile GetProfile(string name)
        {
            Console.WriteLine("Из базы данных достается информация о профилях игроков..");
            var playerProfiles = FindAll();
            if (!playerProfiles.ContainsKey(name))
            {
                Console.WriteLine("В базе данных создается новый профиль...");
                playerProfiles[name] = new PlayerProfile(name, 0);
            }
            Update(playerProfiles);
            return playerProfiles[name];
        }

        public void UpdateHighScore(string name, int score)
        {
            Console.WriteLine("В базе данных обновляются очки игрока...");
            var playerProfiles = FindAll();
            if (!playerProfiles.ContainsKey(name))
            {
                Console.WriteLine("В базе данных создается новый профиль...");
                playerProfiles[name] = new PlayerProfile(name, 0);
            }
            playerProfiles[name].Score = score;
            Update(playerProfiles);
        }

        private Dictionary<string, PlayerProfile> FindAll()
        {
            if (!File.Exists(_filePath))
            {
                return new Dictionary<string, PlayerProfile>();
            }

            using (var stream = File.OpenRead(_filePath))
            {
                var formatter = new BinaryFormatter();
                return (Dictionary<string, PlayerProfile>)formatter.Deserialize(stream);
            }
        }

        private void Update(Dictionary<string, PlayerProfile> profiles)
        {
            using (var stream = File.OpenWrite(_filePath))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, profiles);
            }
        }
    }
    public class PlayerProfileCacheRepository : IPlayerProfileRepository
    {
        private readonly IDictionary<string, PlayerProfile> _cachedProfiles;
        private readonly PlayerProfileDBRepository _database;

        public PlayerProfileCacheRepository()
        {
            _cachedProfiles = new Dictionary<string, PlayerProfile>();
            _database = new PlayerProfileDBRepository();
        }

        public PlayerProfile GetProfile(string name)
        {
            // Если профиль не закеширован, достаёт его из базы данных и кеширует
            if (!_cachedProfiles.ContainsKey(name))
            {
                Console.WriteLine("Профиль игрока не найден в кеше...");
                PlayerProfile profileFromDatabase = _database.GetProfile(name);
                _cachedProfiles[name] = profileFromDatabase;
            }

            // Достаёт закешированный профиль
            Console.WriteLine("Профиль игрока достаётся из кеша...");
            return _cachedProfiles[name];
        }

        public void UpdateHighScore(string name, int score)
        {
            if (!_cachedProfiles.ContainsKey(name))
            {
                Console.WriteLine("Профиль игрока не найден в кеше...");
                _database.UpdateHighScore(name, score);
                PlayerProfile profileFromDatabase = _database.GetProfile(name);
                _cachedProfiles[name] = profileFromDatabase;
                return;
            }

            // Write-through cache - сначала записывает в кэш, затем в базу данных
            PlayerProfile cachedProfile = _cachedProfiles[name];
            cachedProfile.Score = score;
            _database.UpdateHighScore(name, score);
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            IPlayerProfileRepository repository = new PlayerProfileCacheRepository();

            Console.WriteLine("Создайте своего персонажа:");

            Console.WriteLine("Введите имя:");
            string name = Console.ReadLine();

            PlayerProfile playerProfile = repository.GetProfile(name);
            Console.WriteLine($"Текущий счет игрока {playerProfile.Name}: {playerProfile.Score}");

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
            gameLogger.log($"{player.getName()} очнулся на распутье!");

            Console.WriteLine("Куда вы двинетесь? Выберите локацию: (мистический лес, проклятый особняк, логово дракона)");
            string locationName = Console.ReadLine();
            Location location = getLocation(locationName);

            gameLogger.log($"{player.getName()} отправился в {locationName}");
            Enemy enemy = location.spawnEnemy();
            bool strongEnemyCurse = new Random().NextDouble() >= 0.5;
            if (strongEnemyCurse)
            {
                gameLogger.log($"Боги особенно немилостивы к {name}, сегодня его ждет страшная битва...");
                enemy = AddEnemyModifiers(enemy);
            }
            gameLogger.log($"{player.getName()} на пути встречает {enemy.getName()}, начинается бой!");

            Random random = new Random();
            while (player.isAlive() && enemy.isAlive())
            {
                Console.WriteLine("Введите что-нибудь чтобы атаковать!");
                Console.ReadLine();
                player.attack(enemy);
                bool stunned = random.NextDouble() > 0.5;
                if (stunned)
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
                repository.UpdateHighScore(name, 0);
                playerProfile = repository.GetProfile(name);
                Console.WriteLine($"Новый счет игрока {playerProfile.Name}: {playerProfile.Score}");
                return;
            }
            gameLogger.log($"Злой {enemy.getName()} был побеждён! {player.getName()} отправился дальше по тропе судьбы...");

            int score = GetScore(locationName, strongEnemyCurse);
            repository.UpdateHighScore(name, score);
            playerProfile = repository.GetProfile(name);
            Console.WriteLine($"Новый счет игрока {playerProfile.Name}: {playerProfile.Score}");
        }
        private static Enemy AddEnemyModifiers(Enemy enemy)
        {
            BaseEnemyDecorator decorator = new BaseEnemyDecorator(enemy);

            // с вероятностью 30% на врага накладывается оба модификатора
            float secondModifierProbability = 0.3f;
            bool secondModifierProc = secondModifierProbability >= new Random().NextDouble();
            if (new Random().NextDouble() >= 0.5)
            {
                decorator = new LegendaryEnemyDecorator(decorator);
                if (secondModifierProc)
                    decorator = new WindfuryEnemyDecorator(decorator);
            }
            else
            {
                decorator = new WindfuryEnemyDecorator(decorator);
                if (secondModifierProc)
                    decorator = new LegendaryEnemyDecorator(decorator);
            }

            return decorator;
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
                case "проклятый особняк":
                    return new HauntedManor();
                default:
                    throw new ArgumentException(nameof(locationName));
            }
        }
        private static int GetScore(string locationName, bool strongEnemy)
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
                    throw new ArgumentException("Неизвестная локация");
            }

            if (strongEnemy)
                return baseScore * 2;

            return baseScore;
        }
    }
}
