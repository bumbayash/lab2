using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_4
{
    public interface Armor
    {
        float getDefense();
        void use();
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
}
