using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_4
{
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
            logger.log(String.Format($"{name} атакует врага {enemy.getName()}"));
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
}
