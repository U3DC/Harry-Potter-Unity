﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Cards.PlayRequirements;
using JetBrains.Annotations;
using UnityEngine;
using MonoBehaviour = UnityEngine.MonoBehaviour;

namespace HarryPotterUnity.Game
{
    [UsedImplicitly]
    class InputGatherer : MonoBehaviour
    {
        private BaseCard _cardInfo;
        

        private int _inputRequired;

        [UsedImplicitly]
        private void Start()
        {
            _cardInfo = GetComponent<BaseCard>();
            var requirement = GetComponent<InputRequirement>();

            if (requirement == null)
            {
                Debug.LogError("No Input Requirement Attached to this card");
                return;
            }

            _inputRequired = requirement.InputRequired;

        }

        public void GatherInput()
        {
            _cardInfo.Player.DisableAllCards();
            _cardInfo.Player.OppositePlayer.DisableAllCards();

            var validCards = _cardInfo.GetValidTargets();

            foreach (var card in validCards)
            {
                card.SetAsValidChoice();
                card.Enable();
                card.gameObject.layer = GameManager.VALID_CHOICE_LAYER;
            }

            StartCoroutine(WaitForPlayerInput());
        }

        private IEnumerator WaitForPlayerInput()
        {
            if (_inputRequired <= 0) throw new Exception("_inputRequired field is not set or set to a negative value!");

            var selectedCards = new List<BaseCard>();

            while (selectedCards.Count < _inputRequired)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0) && _cardInfo.Player.IsLocalPlayer)
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 1000f, 1 << 11))
                    {
                        var target = hit.transform.gameObject.GetComponent<BaseCard>();
                        selectedCards.Add(target);

                        target.SetSelected();

                        if (selectedCards.Count == _inputRequired)
                        {
                            var selectedCardIds = selectedCards.Select(c => c.NetworkId).ToArray();
                            _cardInfo.Player.NetworkManager.photonView.RPC("ExecuteInputCardById", PhotonTargets.All, _cardInfo.NetworkId, selectedCardIds);
                        }
                    }
                }
                yield return null;
            }
        }
    }
}
