using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_4
{
    public interface Location
    {
        Enemy spawnEnemy();
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
    public class HauntedManor : Location
    {
        private WeaponEquipmentFacade weaponEquipmentFacade;
        public HauntedManor()
        {
            Random Random = new Random();
            //CharacterClass randomClass = (CharacterClass)Random.Next(Enum.GetValues(typeof(CharacterClass)).Length);
            int[] numb = { 80, 90, 100 };
            Random random = new Random();
            int index = random.Next(numb.Length);
            int randomnumber = numb[index];
            CharacterClass randomClass = (CharacterClass)randomnumber;
            this.weaponEquipmentFacade = new WeaponEquipmentFacade(randomClass);
        }
        public Enemy spawnEnemy()
        {
            Weapon weapon = weaponEquipmentFacade.getWeapon();
            Enemy enchantedWeapon = new WeaponToEnemyAdapter(weapon);
            return enchantedWeapon;
        }
       
    }
}
