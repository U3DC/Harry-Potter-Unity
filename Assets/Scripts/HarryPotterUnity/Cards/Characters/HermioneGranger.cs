﻿using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Cards.Generic.Interfaces;
using UnityEngine;
using System.Linq;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Characters
{
    [UsedImplicitly]
    public class HermioneGranger : GenericCard, IPersistentCard
    {
        public bool CanPerformInPlayAction()
        {
            return Player.CanUseActions() && 
                   Player.Hand.Cards.Count(c => c.Type == Type.Lesson) >= 2 &&
                   Player.AmountLessonsInPlay >= 2;
        }

        public void OnSelectedAction()
        {
            var firstLesson = Player.Hand.Cards.Find(c => c.Type == Type.Lesson);
            var secondLesson = Player.Hand.Cards.FindLast(c => c.Type == Type.Lesson);

            Player.InPlay.Add(firstLesson);
            Player.InPlay.Add(secondLesson);
            Player.Hand.RemoveAll(new[] {firstLesson, secondLesson});

            Player.UseActions();
        }

        protected override void OnClickAction(List<GenericCard> targets) { }
        public void OnInPlayBeforeTurnAction() { }
        public void OnInPlayAfterTurnAction() { }        
        public void OnEnterInPlayAction() { }
        public void OnExitInPlayAction() { }
    }
}