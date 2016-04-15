﻿using System.Collections.Generic;

namespace HarryPotterUnity.Cards.Characters
{
    public class MadamPomfrey : BaseCharacter
    {
        private bool HasUsedAbility { get; set; }

        public override bool CanPerformInPlayAction()
        {
            return HasUsedAbility == false 
                && Player.CanUseActions()
                && Player.IsLocalPlayer;
        }

        public override void OnSelectedAction(List<BaseCard> targets = null)
        {
            HasUsedAbility = true;

            var cards = Player.Discard.GetHealableCards(12);

            Player.Deck.AddAll(cards);

            Player.Deck.Shuffle();
        }
    }
}