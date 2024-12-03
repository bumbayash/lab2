using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_4
{
    // Интерфейс стратегии атаки
    public interface IAttackStrategy
    {
        void Execute(Enemy enemy);
    }

    // Класс для ближней атаки
    public class MeleeAttackStrategy : IAttackStrategy
    {
        public void Execute(Enemy enemy)
        {
            GameLogger.getInstance().log("Компаньон атакует в ближнем бою!");
            enemy.takeDamage(10);
        }
    }

    // Класс для дальней атаки
    public class RangedAttackStrategy : IAttackStrategy
    {
        public void Execute(Enemy enemy)
        {
            GameLogger.getInstance().log("Компаньон атакует издалека!");
            enemy.takeDamage(7);
        }
    }

    // Класс Компаньона
    public class Companion
    {
        private readonly IAttackStrategy _attackStrategy;

        public Companion(CharacterClass companionToClass)
        {
            switch (companionToClass)
            {
                case CharacterClass.MAGE:
                    _attackStrategy = new MeleeAttackStrategy();
                    break;
                case CharacterClass.WARRIOR:
                    _attackStrategy = new RangedAttackStrategy();
                    break;
                case CharacterClass.THIEF:
                    var random = new Random();
                    bool roll = random.Next(0, 2) == 0;                    // NextBool() - гипотетический метод, который возвращает true/false
                    if (roll)
                    {
                        _attackStrategy = new MeleeAttackStrategy();
                    }
                    else
                    {
                        _attackStrategy = new RangedAttackStrategy();
                    }
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public void Attack(Enemy enemy)
        {
            _attackStrategy.Execute(enemy);
        }
    }

}
