﻿using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.PlayRequirements
{
    public class UniqueTagRequirement : MonoBehaviour, ICardPlayRequirement
    {
        private Player _player;

        [SerializeField, UsedImplicitly]
        private Tag _tag;

        [UsedImplicitly]
        void Start()
        {
            _player = GetComponent<BaseCard>().Player;
        }
        public bool MeetsRequirement()
        {
            return true;
        }

        public void OnRequirementMet()
        {
            if (!_player.InPlay.Cards.Exists(c => c.Tags.Contains(_tag))) return;

            var card =_player.InPlay.Cards.Find(c => c.Tags.Contains(_tag));

            _player.Discard.Add(card);
        }
    }
}
