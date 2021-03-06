﻿using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;
using UnityLogWrapper;
using Random = UnityEngine.Random;
using Type = HarryPotterUnity.Enums.Type;

namespace HarryPotterUnity.DeckGeneration
{
    [UsedImplicitly]
    public static class DeckGenerator
    {
        private static List<BaseCard> _allStartingCharacters;
        private static List<BaseCard> _cardLibrary;
        private static List<BaseCard> _availableStartingCharacters;

        private static List<BaseCard> CardLibrary
        {
            get
            {
                if (_cardLibrary == null)
                {
                    var cards = Resources.LoadAll("Cards/")
                        .Cast<GameObject>()
                        .Select( o => o.GetComponent<BaseCard>() );

                    _cardLibrary = new List<BaseCard>(cards);
                }

                return _cardLibrary;
            }
        }

        private static List<BaseCard> AllStartingCharacters
        {
            get
            {
                if (_allStartingCharacters == null)
                {
                    _allStartingCharacters = new List<BaseCard>();
                    _allStartingCharacters.AddRange(CardLibrary.Where(c => c.Type == Type.Character));
                }

                return _allStartingCharacters;
            }
        }
        
        private static List<BaseCard> AvailableStartingCharacters
        {
            get
            {
                if (_availableStartingCharacters == null)
                {
                    _availableStartingCharacters = new List<BaseCard>();
                    _availableStartingCharacters.AddRange( AllStartingCharacters );
                }

                return _availableStartingCharacters;
            }
        }
        
        public static BaseCard GetRandomCharacter()
        {
            var character = AvailableStartingCharacters.Skip(Random.Range(0, AvailableStartingCharacters.Count)).First();

            AvailableStartingCharacters.Remove(character);

            return character;
        }

        public static List<BaseCard> GenerateDeck(List<LessonTypes> types)
        {
            var deck = new List<BaseCard>();

            BuildDeck(ref deck, types);

            if (deck.Count != 60)
            {
                Log.Error("Deck did not initialize with the correct amount of cards, deck.count = {0}", deck.Count);
            }
            return deck;
        }

        public static List<BaseCard> GenerateDeck(List<BaseCard> prebuiltDeck, List<LessonTypes> types)
        {
            var deck = new List<BaseCard>();

            BuildDeck(ref deck, types);

            if (deck.Count != 60)
            {
                Log.Warning("Generate Deck with Prebuilt Deck finished with CardCount = {0}", deck.Count);
            }

            deck.AddRange(prebuiltDeck);

            return deck;
        }

        private static void BuildDeck(ref List<BaseCard> deck, List<LessonTypes> types)
        {
            switch (types.Count)
            {
                case 2:
                    AddLessonsToDeck(ref deck, types[0], 13);
                    AddLessonsToDeck(ref deck, types[1], 10);

                    AddSupportCharacters(ref deck, 1);
                    AddAdventures(ref deck, 3);
                    AddCardsToDeck(ref deck, types[0].ToClassification(), 17);
                    AddCardsToDeck(ref deck, types[1].ToClassification(), 16);
                    break;
                case 3:
                    AddLessonsToDeck(ref deck, types[0], 12);
                    AddLessonsToDeck(ref deck, types[1], 6);
                    AddLessonsToDeck(ref deck, types[2], 5);

                    AddSupportCharacters(ref deck, 1);
                    AddAdventures(ref deck, 3);
                    AddCardsToDeck(ref deck, types[0].ToClassification(), 11);
                    AddCardsToDeck(ref deck, types[1].ToClassification(), 11);
                    AddCardsToDeck(ref deck, types[2].ToClassification(), 11);
                    break;
                default:
                    throw new Exception(types.Count + " type(s) sent to GenerateDeck, unsupported");
            }
        }

        private static void AddAdventures(ref List<BaseCard> deck, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                deck.Add(GetRandomAdventure());
            }
        }

        private static BaseCard GetRandomAdventure()
        {
            var adventures = CardLibrary.Where(c => c.Classification == ClassificationTypes.Adventure).ToList();
            var card = adventures.Skip(Random.Range(0, adventures.Count)).First();

            return card;
        }

        private static void AddSupportCharacters(ref List<BaseCard> deck, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                deck.Add( GetRandomCharacter() );
            }
        }

        private static void AddLessonsToDeck(ref List<BaseCard> deck, LessonTypes lessonType, int amount)
        {
            var card = CardLibrary
                .Where(c => c.Classification == ClassificationTypes.Lesson)
                .First(l => ((ILessonProvider) l).LessonType == lessonType);

            for (int i = 0; i < amount; i++)
            {
                deck.Add(card);
            }
        }

        private static void AddCardsToDeck(ref List<BaseCard> deck, ClassificationTypes classification, int amount)
        {
            var potentialCards = CardLibrary.Where(c => c.Classification == classification).ToList();

            int cardsAdded = 0;

            while (cardsAdded < amount)
            {
                int selected = Random.Range(0, potentialCards.Count);
                var card = potentialCards[selected];

                var deckCopy = deck.ToList();

                bool canBeAdded = (card.DeckGenerationRequirements.Count == 0 || 
                                   card.DeckGenerationRequirements.All(req => req.MeetsRequirement(deckCopy))) && 
                                   card.MeetsRarityRequirements() &&
                                   deck.Count(c => c.CardName.Equals(card.CardName)) < 4;
                
                if (canBeAdded)
                {
                    deck.Add(card);
                    cardsAdded++;
                }
            }
        }

        private static bool MeetsRarityRequirements(this BaseCard card)
        {
            float chanceToAdd;

            float rng = Random.Range(0f, 1f);

            switch (card.Rarity)
            {
                case Rarity.Common:
                    chanceToAdd = 1f;
                    break;
                case Rarity.Uncommon:
                    chanceToAdd = 0.7f;
                    break;
                case Rarity.Rare:
                    chanceToAdd = 0.5f;
                    break;
                case Rarity.UltraRare:
                    chanceToAdd = 0.3f;
                    break;
                default:
                    chanceToAdd = 1f;
                    break;
            }

            return rng <= chanceToAdd;
        }

        public static void ResetStartingCharacterPool()
        {
            _availableStartingCharacters = new List<BaseCard>();
            _availableStartingCharacters.AddRange(AllStartingCharacters);
        }
    }
}