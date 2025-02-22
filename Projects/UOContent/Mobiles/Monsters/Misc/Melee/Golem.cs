using System;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    public class Golem : BaseCreature
    {
        private bool m_Stunning;

        [Constructible]
        public Golem(bool summoned = false, double scalar = 1.0) : base(AIType.AI_Melee)
        {
            Body = 752;

            if (summoned)
            {
                Hue = 2101;
            }

            SetSpeed(0.9, 1.5);

            SetStr((int)(251 * scalar), (int)(350 * scalar));
            SetDex((int)(76 * scalar), (int)(100 * scalar));
            SetInt((int)(101 * scalar), (int)(150 * scalar));

            SetHits((int)(151 * scalar), (int)(210 * scalar));

            SetDamage((int)(13 * scalar), (int)(24 * scalar));

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, (int)(35 * scalar), (int)(55 * scalar));

            if (summoned)
            {
                SetResistance(ResistanceType.Fire, (int)(50 * scalar), (int)(60 * scalar));
            }
            else
            {
                SetResistance(ResistanceType.Fire, (int)(100 * scalar));
            }

            SetResistance(ResistanceType.Cold, (int)(10 * scalar), (int)(30 * scalar));
            SetResistance(ResistanceType.Poison, (int)(10 * scalar), (int)(25 * scalar));
            SetResistance(ResistanceType.Energy, (int)(30 * scalar), (int)(40 * scalar));

            SetSkill(SkillName.MagicResist, 150.1 * scalar, 190.0 * scalar);
            SetSkill(SkillName.Tactics, 60.1 * scalar, 100.0 * scalar);
            SetSkill(SkillName.Wrestling, 60.1 * scalar, 100.0 * scalar);

            if (summoned)
            {
                Fame = 10;
                Karma = 10;
            }
            else
            {
                Fame = 3500;
                Karma = -3500;
            }

            if (!summoned)
            {
                PackItem(new IronIngot(Utility.RandomMinMax(13, 21)));

                if (Utility.RandomDouble() < 0.1)
                {
                    PackItem(new PowerCrystal());
                }

                if (Utility.RandomDouble() < 0.15)
                {
                    PackItem(new ClockworkAssembly());
                }

                if (Utility.RandomDouble() < 0.2)
                {
                    PackItem(new ArcaneGem());
                }

                if (Utility.RandomDouble() < 0.25)
                {
                    PackItem(new Gears());
                }
            }

            ControlSlots = 3;
        }

        public Golem(Serial serial) : base(serial)
        {
        }

        public override string CorpseName => "a golem corpse";

        public override bool IsScaredOfScaryThings => false;
        public override bool IsScaryToPets => true;

        public override bool IsBondable => false;

        public override FoodType FavoriteFood => FoodType.None;

        public override bool CanBeDistracted => false;

        public override string DefaultName => "a golem";

        public override bool DeleteOnRelease => true;

        public override bool AutoDispel => !Controlled;
        public override bool BleedImmune => true;

        public override bool BardImmune => !Core.AOS || Controlled;
        public override Poison PoisonImmune => Poison.Lethal;

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.05)
            {
                if (!IsParagon)
                {
                    if (Utility.RandomDouble() < 0.75)
                    {
                        c.DropItem(DawnsMusicGear.RandomCommon);
                    }
                    else
                    {
                        c.DropItem(DawnsMusicGear.RandomUncommon);
                    }
                }
                else
                {
                    c.DropItem(DawnsMusicGear.RandomRare);
                }
            }
        }

        public override int GetAngerSound() => 541;

        public override int GetIdleSound()
        {
            if (!Controlled)
            {
                return 542;
            }

            return base.GetIdleSound();
        }

        public override int GetDeathSound()
        {
            if (!Controlled)
            {
                return 545;
            }

            return base.GetDeathSound();
        }

        public override int GetAttackSound() => 562;

        public override int GetHurtSound()
        {
            if (Controlled)
            {
                return 320;
            }

            return base.GetHurtSound();
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (!m_Stunning && Utility.RandomDouble() < 0.3)
            {
                m_Stunning = true;

                defender.Animate(21, 6, 1, true, false, 0);
                PlaySound(0xEE);
                defender.LocalOverheadMessage(
                    MessageType.Regular,
                    0x3B2,
                    false,
                    "You have been stunned by a colossal blow!"
                );

                if (Weapon is BaseWeapon weapon)
                {
                    weapon.OnHit(this, defender);
                }

                if (defender.Alive)
                {
                    defender.Frozen = true;
                    Timer.StartTimer(TimeSpan.FromSeconds(5.0), () => Recover_Callback(defender));
                }
            }
        }

        private void Recover_Callback(Mobile defender)
        {
            defender.Frozen = false;
            defender.Combatant = null;
            defender.LocalOverheadMessage(MessageType.Regular, 0x3B2, false, "You recover your senses.");
            m_Stunning = false;
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (Controlled || Summoned)
            {
                var master = ControlMaster ?? SummonMaster;

                if (master?.Player == true && master.Map == Map && master.InRange(Location, 20))
                {
                    if (master.Mana >= amount)
                    {
                        master.Mana -= amount;
                    }
                    else
                    {
                        amount -= master.Mana;
                        master.Mana = 0;
                        master.Damage(amount);
                    }
                }
            }

            base.OnDamage(amount, from, willKill);
        }

        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);
            var version = reader.ReadInt();
        }
    }
}
