using System;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace YVR.Interaction
{
    public enum InputType
    {
        None,
        LeftController,
        RightController,
        AllController,
        LeftHand,
        RightHand,
        AllHand,
        HMD,
    }

    public class XRInputManager : MonoBehaviorSingleton<XRInputManager>
    {
        public InputType inputType { get; private set; }
        public XRBaseController leftController;
        public XRBaseController rightController;
        public XRBaseController leftHand;
        public XRBaseController rightHand;
        public XRBaseController head;
        public Action<InputType> onInputTypeChanged;

        private XRControllerState m_LeftControllerState;
        private XRControllerState m_RightControllerState;
        private XRControllerState m_LeftHandState;
        private XRControllerState m_RightHandState;

        private bool leftControllerTracking => (m_LeftControllerState?.inputTrackingState & InputTrackingState.Rotation) != 0;
        private bool rightControllerTracking => (m_RightControllerState?.inputTrackingState & InputTrackingState.Rotation) != 0;
        private bool leftHandTracking => (m_LeftHandState?.inputTrackingState & InputTrackingState.Rotation) != 0;
        private bool rightHandTracking => (m_RightHandState?.inputTrackingState & InputTrackingState.Rotation) != 0;

        protected override void Init()
        {
            m_LeftControllerState =
                leftController == null ? new XRControllerState() : leftController.currentControllerState;
            m_RightControllerState = rightController == null
                ? new XRControllerState()
                : rightController.currentControllerState;
            m_LeftHandState = leftHand == null ? new XRControllerState() : leftHand.currentControllerState;
            m_RightHandState = rightHand == null ? new XRControllerState() : rightHand.currentControllerState;
            onInputTypeChanged += SwitchInputModeActive;
        }

        private void Update()
        {
            UpdateInputType();
        }

        public XRBaseController GetXRControllerOfType(InputType inputType)
        {
            switch (inputType)
            {
                case InputType.LeftController:
                    return leftController;
                case InputType.RightController:
                    return rightController;
                case InputType.LeftHand:
                    return leftHand;
                case InputType.RightHand:
                    return rightHand;
                case InputType.HMD:
                    return head;
                default:
                    return head;
            }
        }

        public void UpdateInputType()
        {
            InputType currentInputType = inputType;

            if (leftControllerTracking && rightControllerTracking)
                currentInputType = InputType.AllController;
            else if (leftControllerTracking)
                currentInputType = InputType.LeftController;
            else if (rightControllerTracking)
                currentInputType = InputType.RightController;
            else if (leftHandTracking && rightHandTracking)
                currentInputType = InputType.AllHand;
            else if (leftHandTracking)
                currentInputType = InputType.LeftHand;
            else if (rightHandTracking)
                currentInputType = InputType.RightHand;
            else
                currentInputType = InputType.HMD;

            if (inputType != currentInputType)
            {
                inputType = currentInputType;
                onInputTypeChanged?.SafeInvoke(inputType);
            }
        }

        private void SwitchInputModeActive(InputType changedType)
        {
            leftController?.model.gameObject.SetActive(leftControllerTracking);
            rightController?.model.gameObject.SetActive(rightControllerTracking);
            leftHand?.gameObject.SetActive(leftHandTracking);
            rightHand?.gameObject.SetActive(rightHandTracking);
            head?.gameObject.SetActive(changedType == InputType.HMD);
        }
    }
}