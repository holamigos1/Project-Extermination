using GameAnimation.Data;
using GameObjects.Base;
using UnityEngine;

namespace Characters
{
    public class PlayerCharacter : Unit
    {
        [SerializeField] private AnimationControllerState _state;
    }
}