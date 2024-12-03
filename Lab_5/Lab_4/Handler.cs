using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_4
{
    public interface IDamageHandler
    {
        IDamageHandler SetNext(IDamageHandler nextHandler);
        int Handle(int incomingDamage);
    }
    public abstract class AbstractDamageHandler : IDamageHandler
    {
        protected IDamageHandler NextHandler { get; private set; }

        public virtual IDamageHandler SetNext(IDamageHandler nextHandler)
        {
            NextHandler = nextHandler;
            return nextHandler;
        }

        public virtual int Handle(int incomingDamage)
        {
            if (NextHandler != null)
            {
                return NextHandler.Handle(incomingDamage);
            }
            return incomingDamage;
        }
    }
    public class BarrierDamageHandler : AbstractDamageHandler
    {
        private int BarrierHealth { get; set; }

        public BarrierDamageHandler(int barrierHealth)
        {
            BarrierHealth = barrierHealth;
        }

        public override int Handle(int incomingDamage)
        {
            if (BarrierHealth == 0)
            {
                GameLogger.getInstance().log("Барьер персонажа не может заблокировать урон");
                return base.Handle(incomingDamage);
            }

            GameLogger.getInstance().log(string.Format("У персонажа есть барьер с прочностью {0}", BarrierHealth));
            if (BarrierHealth >= incomingDamage)
            {
                BarrierHealth -= incomingDamage;
                GameLogger.getInstance().log("Барьер полностью поглотил урон");
                GameLogger.getInstance().log(string.Format("Урон стал {0}", 0));
                return base.Handle(0);
            }

            incomingDamage = incomingDamage - BarrierHealth;
            BarrierHealth = 0;
            GameLogger.getInstance().log("Барьер персонажа был рассеян");
            GameLogger.getInstance().log(string.Format("Урон стал {0}", incomingDamage));
            return base.Handle(incomingDamage);
        }
    }
    public class BuffDebuffDamageHandler : AbstractDamageHandler
    {
        private readonly float Multiplier;

        public BuffDebuffDamageHandler(float multiplier)
        {
            Multiplier = multiplier;
        }

        public override int Handle(int incomingDamage)
        {
            GameLogger.getInstance().log(string.Format("На персонажа наложен эффект который модифицирует урон в {0} раз!", Multiplier));
            incomingDamage = (int)Math.Round(incomingDamage * Multiplier);
            GameLogger.getInstance().log(string.Format("Урон стал {0}", incomingDamage));
            return base.Handle(incomingDamage);
        }
    }
    public class InvulnerabilityDamageHandler : AbstractDamageHandler
    {
        public override int Handle(int incomingDamage)
        {
            GameLogger.getInstance().log("На персонажа наложена неуязвимость!!!");
            return 0;
        }
    }



}
