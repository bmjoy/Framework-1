﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PixelComrades {
    public static class CharacterFactory {
        public static Entity GetBasicCharacterEntity() {
            var entity = Entity.New();
            entity.Add(new GenericStats(StatExtensions.GatherCharacterStats().ToArray()));
            StatExtensions.SetupDefenseStats(entity);
            entity.Add(new VitalStats());
            entity.Add(new LabelComponent(""));
            entity.Add(new DeathStatus());
            entity.Add(new ModifiersContainer(null));
            entity.Add(new StatusContainer());
            entity.Add(new GridPosition());
            entity.Add(new FactionComponent());
            entity.Add(new PronounComponent(PlayerPronouns.They));
            entity.Post(new EntitySetup(entity));
            //entity.Get<EntityCollection<BaseStat>>("Attributes")[Attributes.Agility].AddDerivedStat(0.2f, new BaseStat.DerivedGeneric(100, f => entity.Get<MoveSpeed>().Speed = f, null, "Speed"));
            return entity;
        }
    }
}
