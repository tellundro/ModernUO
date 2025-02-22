using Server.Mobiles;

namespace Server.Engines.Quests.Samurai
{
    public class FierceDragon : BaseCreature
    {
        [Constructible]
        public FierceDragon() : base(AIType.AI_Melee, FightMode.Aggressor)
        {
            Body = 103;
            BaseSoundID = 362;

            SetSpeed(0.3, 1.0);
            SetStr(6000, 6020);
            SetDex(0);
            SetInt(850, 870);

            SetDamage(50, 80);

            SetDamageType(ResistanceType.Fire, 100);

            SetResistance(ResistanceType.Physical, 95, 98);
            SetResistance(ResistanceType.Fire, 95, 98);
            SetResistance(ResistanceType.Cold, 95, 98);
            SetResistance(ResistanceType.Poison, 95, 98);
            SetResistance(ResistanceType.Energy, 95, 98);

            SetSkill(SkillName.Tactics, 120.0);
            SetSkill(SkillName.Wrestling, 120.0);
            SetSkill(SkillName.Magery, 120.0);

            Fame = 15000;
            Karma = 15000;

            CantWalk = true;
        }

        public FierceDragon(Serial serial) : base(serial)
        {
        }

        public override string DefaultName => "a fierce dragon";

        public override int GetIdleSound() => 0x2C4;

        public override int GetAttackSound() => 0x2C0;

        public override int GetDeathSound() => 0x2C1;

        public override int GetAngerSound() => 0x2C4;

        public override int GetHurtSound() => 0x2C3;

        public override void AggressiveAction(Mobile aggressor, bool criminal)
        {
            base.AggressiveAction(aggressor, criminal);

            if (aggressor is PlayerMobile player)
            {
                var qs = player.Quest;
                if (qs is HaochisTrialsQuest)
                {
                    QuestObjective obj = qs.FindObjective<SecondTrialAttackObjective>();
                    if (obj?.Completed == false)
                    {
                        obj.Complete();
                        qs.AddObjective(new SecondTrialReturnObjective(true));
                    }
                }
            }
        }

        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);

            var version = reader.ReadEncodedInt();
        }
    }
}
