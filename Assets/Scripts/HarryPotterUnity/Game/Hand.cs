﻿using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Game
{
    [UsedImplicitly]
    public class Hand : MonoBehaviour {
        
        public List<GenericCard> Cards { get; private set; }

        private Player _player;

        private static readonly Vector3 HandPreviewPosition = new Vector3(-80f, -13f, -336f);

        public static readonly Vector3 HandCardsOffset = new Vector3(-240f, -200f, 0f);

        public const float Spacing = 55f;

        public Hand()
        {
            Cards = new List<GenericCard>();
        }

        [UsedImplicitly]
        public void Awake()
        {
            _player = transform.GetComponentInParent<Player>();
        }

        public void Add(GenericCard card, bool preview = true)
        {
            var shouldFlip = card.FlipState == GenericCard.FlipStates.FaceUp && !_player.IsLocalPlayer || 
                             card.FlipState == GenericCard.FlipStates.FaceDown && _player.IsLocalPlayer;

            card.transform.parent = transform;

            if (Cards.Count == 12) AdjustHandSpacing();
            AnimateCardToHand(card, shouldFlip, preview);

            Cards.Add(card);
        }

        public void Remove(GenericCard card)
        {
            Cards.Remove(card);

            AdjustHandSpacing();
        }

        private void AdjustHandSpacing()
        {
            var shrinkFactor = Cards.Count >= 12 ? 0.5f : 1f;
        
            for (var i = 0; i < Cards.Count; i++)
            {
                var cardPosition = HandCardsOffset;

                cardPosition.x += i * Spacing * shrinkFactor;
                cardPosition.z -= i;

                TweenQueue.MoveCardWithoutQueue(Cards[i], cardPosition, GenericCard.CardStates.InHand);
            }
        }

        private void AnimateCardToHand(GenericCard card, bool flip = true, bool preview = true)
        {
            var cardPosition = HandCardsOffset;

            var shrinkFactor = Cards.Count >= 12 ? 0.5f : 1f;

            cardPosition.x += Cards.Count * Spacing * shrinkFactor;
            cardPosition.z -= Cards.Count;

            if (preview)
            {
                UtilManager.TweenQueue.AddTweenToQueue(card, HandPreviewPosition, 0.5f, card.State, flip, TweenQueue.RotationType.NoRotate);
            }

            UtilManager.TweenQueue.AddTweenToQueue(card, cardPosition, 0.3f, GenericCard.CardStates.InHand, !preview && flip, TweenQueue.RotationType.NoRotate);
        }
    }
}
