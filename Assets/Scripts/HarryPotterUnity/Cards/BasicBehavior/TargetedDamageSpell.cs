﻿using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Cards.PlayRequirements;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.BasicBehavior
{
    [RequireComponent(typeof(InputRequirement))]
    public class TargetedDamageSpell : BaseSpell, IDamageSpell
    {
        [Header("Targeted Damage Spell Settings"), Space(10)]
        [UsedImplicitly, SerializeField]
        private int _damageAmount;

        [UsedImplicitly, SerializeField]
        protected bool _canTargetPlayer;

        public int DamageAmount
        {
            get { return this._damageAmount; }
            set { this._damageAmount = value; }
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            var target = targets.Single();

            if (target is BaseCreature)
            {
                (target as BaseCreature).TakeDamage(this._damageAmount);
            }
            else
            {
                this.Player.OppositePlayer.TakeDamage(this, this._damageAmount);
            }
                
        }

        public override List<BaseCard> GetFromHandActionTargets()
        {
            var targets = new List<BaseCard>();

            if (this._canTargetPlayer) targets.Add(this.Player.OppositePlayer.Deck.StartingCharacter);

            targets.AddRange(this.Player.OppositePlayer.InPlay.Creatures.Concat(this.Player.InPlay.Creatures));

            return targets;
        }
    }
}
