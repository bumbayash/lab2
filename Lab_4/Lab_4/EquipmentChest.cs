using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_4
{
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
    public class WeaponEquipmentFacade
    {
        private EquipmentChest equipmentChest;
        public WeaponEquipmentFacade(CharacterClass characterClass)
        {
            switch (characterClass)
            {
                case CharacterClass.MAGE:
                    equipmentChest = new MagicalEquipmentChest();
                    break;
                case CharacterClass.WARRIOR:
                    equipmentChest = new WarriorEquipmentChest();
                    break;
                case CharacterClass.THIEF:
                    equipmentChest = new ThiefEquipmentChest();
                    break;
                default:
                    Console.WriteLine(characterClass.ToString());
                    throw new ArgumentException("Неизвестный класс");
            }
        }
        public Weapon getWeapon()
        {
            return equipmentChest.getWeapon();
        }

    }
}
