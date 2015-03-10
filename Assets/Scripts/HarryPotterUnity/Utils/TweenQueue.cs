﻿using HarryPotterUnity.Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HarryPotterUnity.Utils
{
    internal class TweenObject
    {
        public GameObject Target;
        public Vector3 Position;
        public iTween.EaseType EaseType;
        public float Time;
        public float Delay;
        public bool Flip;
        public bool Rotate;
        public GenericCard.CardStates StateAfterAnimation;
    }

    public class TweenQueue
    {
        private readonly Queue<TweenObject> _queue;

        private static bool _tweenQueueRunning;
        public bool TweenQueueIsEmpty { get; private set; }

        public TweenQueue()
        {
            _queue = new Queue<TweenObject>();    
        }

        /// <summary>
        /// Add a tween to the queue, the tween will not execute until all other tweens in the queue have finished executing.
        /// </summary>
        /// <param name="target">The target card</param>
        /// <param name="position">The target position</param>
        /// <param name="time">Time in seconds the tween should take to complete</param>
        /// <param name="stateAfterAnimation">The CardState of the card after the animation has finished</param>
        /// <param name="flip">Whether the card should be flipped from FaceUp to FaceDown or vice-versa</param>
        /// <param name="rotate">Whether the card should be rotated from vertical to horizontal or vice-versa</param>
        public void AddTweenToQueue(GenericCard target, Vector3 position, float time, GenericCard.CardStates stateAfterAnimation, bool flip, bool rotate)
        {
            var newTween = new TweenObject
            {
                Target = target.gameObject,
                Position = position,
                Time = time,
                Delay = 0.1f,
                EaseType = iTween.EaseType.EaseInOutSine,
                StateAfterAnimation = stateAfterAnimation,
                Flip = flip,
                Rotate = rotate
            };

            _queue.Enqueue(newTween);
            TweenQueueIsEmpty = false;

            if (_tweenQueueRunning) return;

            _tweenQueueRunning = true;
            StaticCoroutine.DoCoroutine(RunTweenQueue());
        }

        private IEnumerator RunTweenQueue()
        {
            while (true)
            {
                if (_queue.Count == 0)
                {
                    TweenQueueIsEmpty = true;
                    yield return null;
                }
                else
                {
                    var tween = _queue.Dequeue();

                    iTween.MoveTo(tween.Target, iTween.Hash(
                        "time", tween.Time,
                        "delay", tween.Delay,
                        "position", tween.Position,
                        "easetype", tween.EaseType,
                        "islocal", true,
                        "oncomplete", "SwitchState",
                        "oncompletetarget", tween.Target,
                        "oncompleteparams", tween.StateAfterAnimation
                        ));

                    if (tween.Flip || tween.Rotate)
                    {
                        RotateAndFlipCard(tween.Target, tween.Time, tween.Flip, tween.Rotate);
                    }

                    yield return new WaitForSeconds(tween.Time + tween.Delay);
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private static void RotateAndFlipCard(GameObject card, float time, bool flip, bool rotate)
        {
            //TODO: IDEA: bool rotate -> rotate type enum? rotate90, rotate180, noRotate?
            var cardRotation = card.transform.localRotation.eulerAngles;
            var targetFlip = flip ? (cardRotation.y > 20f ? 0f : 180f) : cardRotation.y;
            var targetRotate = rotate ? (cardRotation.z > 20f ? 0f : 270f) : cardRotation.z;

            if (flip) card.GetComponent<GenericCard>().SwitchFlipState();

            iTween.RotateTo(card, iTween.Hash("time", time,
                "y", targetFlip,
                "z", targetRotate,
                "easetype", iTween.EaseType.EaseInOutSine,
                "islocal", true
                ));
        }

        /// <summary>
        /// Move a card to a target position without placing it into the Tween Queue
        /// </summary>
        /// <param name="card">The target card</param>
        /// <param name="cardPosition">The target position</param>
        /// <param name="stateAfterAnimation">The CardState of the card after the animation has finished</param>
        public static void MoveCardWithoutQueue(GenericCard card, Vector3 cardPosition, GenericCard.CardStates stateAfterAnimation)
        {
            iTween.MoveTo(card.gameObject, iTween.Hash("time", 0.5f,
                "position", cardPosition,
                "easetype", iTween.EaseType.EaseInOutSine,
                "islocal", true,
                "oncomplete", "SwitchState",
                "oncompletetarget", card.gameObject,
                "oncompleteparams", stateAfterAnimation
                ));
        }
    }
}