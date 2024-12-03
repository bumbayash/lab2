using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Lab_4
{
    public static class RandomExtensions
    {
        public static bool NextBool(this Random random)
        {
            return random.Next(0, 2) == 1;
        }
    }


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
            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine("----------ТЕСТОВЫЙ ПРОГОН ЦЕПОЧКИ ОБЯЗАННОСТЕЙ----------");
            int incomingDamage = 100;
            float debuff = 1.5f;
            bool hasInvulnerability = true;
            int barrierHealth = 100;
            Console.WriteLine($"Тестовые данные:\n Входящий урон [{incomingDamage}];\n Множитель [{debuff}];\n Неуязвим [{hasInvulnerability}];\n Ед. здоровья у щита [{barrierHealth}]");

            IDamageHandler chainStart = new BuffDebuffDamageHandler(debuff);
            IDamageHandler chainFinish = chainStart;

            if (hasInvulnerability)
            {
                chainFinish = chainFinish.SetNext(new InvulnerabilityDamageHandler());
            }

            if (barrierHealth > 0)
            {
                chainFinish = chainFinish.SetNext(new BarrierDamageHandler(barrierHealth));
            }

            incomingDamage = chainStart.Handle(incomingDamage);
            GameLogger.getInstance().log($"Итоговый урон: {incomingDamage}");
            Console.WriteLine("----------------ТЕСТОВЫЙ ПРОГОН ЗАВЕРШЕН----------------");
            Console.WriteLine("--------------------------------------------------------");

            GameEventPublisher gameEventPublisher = new GameEventPublisher();

            GameEventListener consoleListener = new GameConsoleEventListener();
            gameEventPublisher.Subscribe(GameEvent.GAME_START, consoleListener);
            gameEventPublisher.Subscribe(GameEvent.GAME_OVER, consoleListener);
            gameEventPublisher.Subscribe(GameEvent.GAME_VICTORY, consoleListener);

            // Для работы с очками игрока в качестве имплементации используется кеширующая прокси
            IPlayerProfileRepository repository = new PlayerProfileCacheRepository();

            GameEventListener updaterListener = new GameUpdaterEventListener(repository);
            gameEventPublisher.Subscribe(GameEvent.GAME_OVER, updaterListener);

            Console.WriteLine("Создайте своего персонажа:");

            Console.Write("Введите имя: ");
            string name = Console.ReadLine();

            PlayerProfile playerProfile = repository.GetProfile(name);
            Console.WriteLine($"Текущий счёт игрока {name}: {playerProfile.Score}");

            Console.Write("Выберите класс из списка: " + string.Join(", ", Enum.GetNames(typeof(CharacterClass))) + "\n");
            CharacterClass characterClass = (CharacterClass)Enum.Parse(typeof(CharacterClass), Console.ReadLine());

            EquipmentChest startingEquipmentChest = getChest(characterClass);
            Armor startingArmor = startingEquipmentChest.getArmor();
            Weapon startingWeapon = startingEquipmentChest.getWeapon();

            PlayableCharacter player = new PlayableCharacter.Builder()
                .SetName(name)
                .SetCharacterClass(characterClass)
                .SetArmor(startingArmor)
                .SetWeapon(startingWeapon)
                .Build();

            gameEventPublisher.NotifyAll(GameEvent.GAME_START, playerProfile);

            GameLogger gameLogger = GameLogger.getInstance();
            gameLogger.log($"{name} очнулся на распутье!");

            gameLogger.log($"{name} присоединился компаньон!!!");
            Companion companion = new Companion(characterClass);

            Console.Write("Куда вы двинетесь? Выберите локацию: (мистический лес, проклятый особняк, логово дракона) ");
            string locationName = Console.ReadLine();
            Location location = getLocation(locationName);

            gameLogger.log($"{name} отправился в {locationName}");

            Enemy enemy = location.spawnEnemy();

            // С шансом в 50% игрок встречает сильного врага
            bool strongEnemyCurse = new Random().NextBool();
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
