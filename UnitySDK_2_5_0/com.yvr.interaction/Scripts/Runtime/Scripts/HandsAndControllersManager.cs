using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using YVR.Core;

namespace YVR.Interaction
{
    public class HandsAndControllersManager : MonoBehaviour
    {
        public GameObject leftHandInteraction;
        public GameObject rightHandInteraction;
        public InputAction leftHandAction;
        public InputAction rightHandAction;

        private InputTrackingState m_LeftHandTrackingState;
        private InputTrackingState m_RightHandTrackingState;

        public void Awake()
        {
            CreateHandDevices();
            leftHandAction.Enable();
            rightHandAction.Enable();
        }

        public void Update()
        {
            m_LeftHandTrackingState = (InputTrackingState)leftHandAction.ReadValue<int>();
            m_RightHandTrackingState = (InputTrackingState)rightHandAction.ReadValue<int>();
            LeftHandActionPerformed(m_LeftHandTrackingState);
            RightHandActionPerformed(m_RightHandTrackingState);
        }

        private void OnDestroy()
        {
            if (YVRAimHand.left != null)
            {
                InputSystem.RemoveDevice(YVRAimHand.left);
                YVRAimHand.left.OnDestory();
                YVRAimHand.left = null;
            }

            if (YVRAimHand.right != null)
            {
                InputSystem.RemoveDevice(YVRAimHand.right);
                YVRAimHand.right.OnDestory();
                YVRAimHand.right = null;
            }

            if (YVRHandDevice.leftHand != null)
            {
                InputSystem.RemoveDevice(YVRHandDevice.leftHand);
                YVRHandDevice.leftHand.OnDestory();
                YVRHandDevice.leftHand = null;
            }

            if (YVRHandDevice.rightHand != null)
            {
                InputSystem.RemoveDevice(YVRHandDevice.rightHand);
                YVRHandDevice.rightHand.OnDestory();
                YVRHandDevice.rightHand = null;
            }
        }

        private void CreateHandDevices()
        {
            if (YVRAimHand.left == null) YVRAimHand.left = YVRAimHand.CreateHand(InputDeviceCharacteristics.Left);
            if (YVRAimHand.right == null) YVRAimHand.right = YVRAimHand.CreateHand(InputDeviceCharacteristics.Right);

            if (YVRHandDevice.leftHand == null) YVRHandDevice.leftHand = YVRHandDevice.Create(HandType.HandLeft);

            if (YVRHandDevice.rightHand == null) YVRHandDevice.rightHand = YVRHandDevice.Create(HandType.HandRight);
        }

        private void LeftHandActionPerformed(InputTrackingState value)
        {
            var isTracking = value != InputTrackingState.None;
            leftHandInteraction.SetActive(isTracking);
        }

        private void RightHandActionPerformed(InputTrackingState value)
        {
            var isTracking = value != InputTrackingState.None;
            rightHandInteraction.SetActive(isTracking);
        }
    }
}