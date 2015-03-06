﻿using System;
using System.Collections;
using System.Collections.Generic;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;

using LessonTypes = HarryPotterUnity.Cards.Lesson.LessonTypes;

namespace HarryPotterUnity.Cards
{
    public abstract class GenericSpell : GenericCard {

        [UsedImplicitly, SerializeField]
        private LessonTypes _costType;

        [UsedImplicitly, SerializeField]
        private int _costAmount;

        [UsedImplicitly, SerializeField]
        private int _inputRequired;

        private static readonly Vector3 SpellOffset = new Vector3(0f, 0f, -400f);

        protected sealed override void OnClickAction()
        {
            AnimateAndDiscard();
            Player.Hand.Remove(this);
        }

        protected sealed override bool MeetsAdditionalPlayRequirements()
        {
            return Player.AmountLessonsInPlay >= _costAmount &&
                   Player.LessonTypesInPlay.Contains(_costType) &&
                   MeetsAdditionalInputRequirements();
        }

        private void AnimateAndDiscard()
        {
            //TODO: Rotate if it's being played by the opponent
            UtilManager.AddTweenToQueue(this, SpellOffset, 0.5f, 0f, State, false, false);
            Invoke("ExecuteActionAndDiscard", 0.9f);
        }

        [UsedImplicitly]
        protected void ExecuteActionAndDiscard()
        {
            Player.Discard.Add(this);
            if (_inputRequired == 0)
            {
                OnPlayAction();
                Player.UseActions(ActionCost); //If the card requires input, the action will be used after the input is selected.
            }
            else
            {
                BeginWaitForInput();
            }
        }

        private void BeginWaitForInput()
        {
            //Move ALL invalid colliders to ignoreraycast layer
            Player.DisableAllCards();
            Player.OppositePlayer.DisableAllCards();

            var validCards = GetValidCards();

            //place valid cards in valid layer
            foreach (var card in validCards)
            {
                card.Enable();
                card.gameObject.layer = UtilManager.ValidChoiceLayer;
            }

            StartCoroutine(WaitForPlayerInput());
        }

        private IEnumerator WaitForPlayerInput()
        {
            if (_inputRequired == 0) throw new Exception("This card does not require input!");

            var selectedCards = new List<GenericCard>();

            while (selectedCards.Count < _inputRequired)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 1000f, 1 << 11))
                    {
                        var target = hit.transform.gameObject.GetComponent<GenericCard>();
                        selectedCards.Add(target);

                        target.SetSelected();

                        if (selectedCards.Count == _inputRequired)
                        {
                            AfterInputAction(selectedCards);

                            Player.EnableAllCards();
                            Player.OppositePlayer.EnableAllCards();

                            Player.UseActions(ActionCost);
                        }
                    }
                }
                yield return null;
            }
        }

        protected virtual bool MeetsAdditionalInputRequirements()
        {
            return true;
        }

        protected virtual IEnumerable<GenericCard> GetValidCards()
        {
            return null;
        }

        protected virtual void OnPlayAction() { }
        protected virtual void AfterInputAction(List<GenericCard> input) { }


    }
}