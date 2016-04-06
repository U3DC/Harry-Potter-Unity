﻿using HarryPotterUnity.Cards.BasicBehavior;

namespace HarryPotterUnity.Cards.Charms.Items
{
    public class BrokenWand : ItemLessonProvider
    {
        public override void OnSelectedAction()
        {
            var card = Player.Deck.TakeTopCard();

            if (card is BaseLesson)
            {
                Player.InPlay.Add(card);
            }
            else
            {
                Player.Discard.Add(card);
            }

            Player.UseActions();
        }

        public override bool CanPerformInPlayAction()
        {
            return Player.CanUseActions() && Player.IsLocalPlayer;
        }
    }
}
