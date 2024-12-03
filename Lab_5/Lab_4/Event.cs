using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_4
{
    public enum GameEvent
    {
        GAME_START,
        GAME_OVER,
        GAME_VICTORY
    }
    public class GameEventPublisher
    {
        private Dictionary<GameEvent, List<GameEventListener>> gameEventsListeners;

        public GameEventPublisher()
        {
            gameEventsListeners = new Dictionary<GameEvent, List<GameEventListener>>();
        }

        public void Subscribe(GameEvent typeToSubscribeTo, GameEventListener subscriber)
        {
            if (!gameEventsListeners.ContainsKey(typeToSubscribeTo))
            {
                gameEventsListeners[typeToSubscribeTo] = new List<GameEventListener>();
            }

            gameEventsListeners[typeToSubscribeTo].Add(subscriber);
        }

        public void NotifyAll(GameEvent notifyEventType, PlayerProfile playerProfile)
        {
            if (!gameEventsListeners.TryGetValue(notifyEventType, out List<GameEventListener> eventListeners))
            {
                return;
            }

            foreach (GameEventListener listener in eventListeners)
            {
                listener.Update(notifyEventType, playerProfile);
            }
        }
    }
    public interface GameEventListener
    {
        void Update(GameEvent eventType, PlayerProfile playerProfile);
    }
    public class GameConsoleEventListener : GameEventListener
    {
        public void Update(GameEvent eventType, PlayerProfile playerProfile)
        {
            Console.WriteLine($"В игре произошло событие: {eventType}, для игрока {playerProfile}");
        }
    }
    public class GameUpdaterEventListener : GameEventListener
    {
        private readonly IPlayerProfileRepository playerProfileRepository;

        public GameUpdaterEventListener(IPlayerProfileRepository playerProfileRepository)
        {
            this.playerProfileRepository = playerProfileRepository;
        }

        public void Update(GameEvent eventType, PlayerProfile playerProfile)
        {
            if (eventType == GameEvent.GAME_OVER)
            {
                playerProfileRepository.UpdateHighScore(playerProfile.Name, 0);
            }
        }
    }

}
