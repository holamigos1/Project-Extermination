using System;
using System.Collections;
using GameItems.Base;
using Misc;
using Misc.Extensions;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Characters.Humanoid.Base
{
    public class HumanRigsHandler
    {
        public HumanRigsHandler(Transform aimRoot, HumanRigsSettings rigSettingsSettings)
        {
            _rigSettings = rigSettingsSettings;
            _aimRoot = aimRoot;
        }
        
        public GameItem ItemInRightHand => _rigSettings._itemRootRightHand.HasChild() ? 
                                            _rigSettings._itemRootRightHand.GetFirstChild().GetComponent<GameItem>() 
                                            : null;
        public GameItem ItemInLeftHand => _rigSettings._itemRootLeftHand.HasChild() ? 
                                            _rigSettings._itemRootLeftHand.GetFirstChild().GetComponent<GameItem>() 
                                            : null;
        
        private HumanRigsSettings _rigSettings;
        private Transform _aimRoot;

        public IEnumerator PickUpItem(GameItem nearItem, Action<GameItem> itemPickUpped) //TODO жирно
        {
            Debug.Log("Pickuping..");
            float distanceToItem = Vector3.Distance(_aimRoot.position, nearItem.ItemTransform.position);

            if (_rigSettings.PickUpDistance < distanceToItem)
            {
                itemPickUpped?.Invoke(null);
                yield break;
            }

            _rigSettings.HandsRigLayer.weight = 1;

            float timePassed = 0;
            while (timePassed < _rigSettings.PickUpTime)
            {
                yield return null;
                timePassed += Time.smoothDeltaTime;
                _rigSettings.RHandPickupConstraint.weight = timePassed / _rigSettings.PickUpTime;
            }
            
            _rigSettings.LeftPalmConstraint.weight = 1;
            
            nearItem.ItemRigidbody.useGravity = false;
            nearItem.ItemRigidbody.isKinematic = true;
            nearItem.ItemTransform.parent = _rigSettings._itemRootRightHand;
            nearItem.ItemTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);


            while (_rigSettings.RHandPickupConstraint.weight > 0)
            {
                yield return null;
                timePassed -= Time.smoothDeltaTime;
                _rigSettings.RHandPickupConstraint.weight = timePassed / _rigSettings.PickUpTime;
            }

            yield return null;
            
            itemPickUpped?.Invoke(nearItem);
        }
        
        [Serializable]
        public struct HumanRigsSettings
        {
            public Transform _itemRootRightHand;
            public Transform _itemRootLeftHand;
            
            public float PickUpDistance;
            public float PickUpTime;
            
            public Rig HeadRigLayer;
            public Rig FeetRigLayer;
            public Rig HandsRigLayer;

            public MultiAimConstraint HeadAimConstraint;
            
            public TwoBoneIKConstraint RFootConstraint;
            public TwoBoneIKConstraint LFootConstraint;
            
            public TwoBoneIKConstraint RHandPickupConstraint;
            public TwoBoneIKConstraint LHandConstraint;
            public MultiParentConstraint RightPalmConstraint;
            public MultiParentConstraint LeftPalmConstraint;
            public MultiAimConstraint HandAimConstraint;
        }
    }
}