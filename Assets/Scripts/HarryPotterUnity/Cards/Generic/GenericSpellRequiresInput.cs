﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Generic
{
    public abstract class GenericSpellRequiresInput : GenericSpell
    {
        [UsedImplicitly, SerializeField]
        private int _inputRequired;

        public int InputRequired { get { return _inputRequired; } }

        public abstract List<GenericCard> GetValidCards();
      
        public abstract void AfterInputAction(List<GenericCard> input);

        protected sealed override void OnPlayAction() { }

        protected sealed override void ExecuteActionAndDiscard()
        {
            Player.Discard.Add(this);
            BeginWaitForInput();
        }

        private void BeginWaitForInput()
        {
            Player.DisableAllCards();
            Player.OppositePlayer.DisableAllCards();

            var validCards = GetValidCards();

            foreach (var card in validCards)
            {
                card.Enable();
                card.gameObject.layer = UtilManager.ValidChoiceLayer;
            }

            StartCoroutine(WaitForPlayerInput());
        }

        private IEnumerator WaitForPlayerInput()
        {
            if (_inputRequired <= 0) throw new Exception("_inputRequired field is not set or set to a negative value!");

            var selectedCards = new List<GenericCard>();

            while (selectedCards.Count < _inputRequired)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0) && Player.IsLocalPlayer)
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
                            var selectedCardIds = selectedCards.Select(c => c.NetworkId).ToArray();
                            Player.MpGameManager.photonView.RPC("ExecuteInputSpellById", PhotonTargets.All, NetworkId, selectedCardIds);
                        }
                    }
                }
                yield return null;
            }
        }
    }
}
