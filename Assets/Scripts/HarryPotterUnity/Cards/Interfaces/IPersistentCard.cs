﻿namespace HarryPotterUnity.Cards.Interfaces
{
    public interface IPersistentCard {

        void OnInPlayBeforeTurnAction();
        void OnInPlayAfterTurnAction();

        bool CanPerformInPlayAction();
        void OnSelectedAction(); 

        void OnEnterInPlayAction();
        void OnExitInPlayAction();
    }
}