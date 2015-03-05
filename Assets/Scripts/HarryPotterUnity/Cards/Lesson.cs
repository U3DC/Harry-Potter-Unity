﻿namespace HarryPotterUnity.Cards
{
    public class Lesson : GenericCard, IPersistentCard {

        public enum LessonTypes
        {
            Creatures = 0, Charms, Transfiguration, Potions, Quidditch
        }

        public LessonTypes LessonType;

        public override void OnClickAction()
        {
            Player.Hand.Remove(this);
            Player.InPlay.Add(this);            
        }

        public override bool MeetsAdditionalPlayRequirements()
        {
            return true;
        }

        public void OnEnterInPlayAction()
        {
            if (!Player.LessonTypesInPlay.Contains(LessonType))
            {
                Player.LessonTypesInPlay.Add(LessonType);
            }
        
            Player.AmountLessonsInPlay++;

            State = CardStates.InPlay;
        }

        public void OnExitInPlayAction()
        {
            Player.AmountLessonsInPlay--;
            Player.UpdateLessonTypesInPlay();
        }

        //Lesson Cards don't implement these methods
        public void OnInPlayBeforeTurnAction() { }
        public void OnInPlayAfterTurnAction() { }
        public void OnSelectedAction() { }
    }
}
