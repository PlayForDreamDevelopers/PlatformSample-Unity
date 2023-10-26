using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace YVR.Interaction
{
    public class HMDInputController : XRBaseController
    {
        [SerializeField] InputActionProperty m_HeadPositionAction;

        [SerializeField] InputActionProperty m_HeadRotationAction;

        public InputActionProperty positionAction
        {
            get => m_HeadPositionAction;
            set => SetInputActionProperty(ref m_HeadPositionAction, value);
        }

        public InputActionProperty rotationAction
        {
            get => m_HeadRotationAction;
            set => SetInputActionProperty(ref m_HeadRotationAction, value);
        }

        private void Start()
        {
            XRInputManager.instance.onInputTypeChanged += OnInputTypeChanged;
        }

        private void OnDestroy()
        {
            XRInputManager.instance.onInputTypeChanged -= OnInputTypeChanged;
        }

        private void SetInputActionProperty(ref InputActionProperty property, InputActionProperty value)
        {
            if (Application.isPlaying)
                property.DisableDirectAction();

            property = value;

            if (Application.isPlaying && isActiveAndEnabled)
                property.EnableDirectAction();
        }

        private void EnableAllDirectAction()
        {
            m_HeadPositionAction.EnableDirectAction();
            m_HeadRotationAction.EnableDirectAction();
        }

        private void DisableAllDirectAction()
        {
            m_HeadPositionAction.DisableDirectAction();
            m_HeadRotationAction.DisableDirectAction();
        }

        protected override void UpdateTrackingInput(XRControllerState controllerState)
        {
            UpdateHMDInput(controllerState);
        }

        protected override void UpdateInput(XRControllerState controllerState)
        {
            base.UpdateInput(controllerState);
            if (controllerState == null)
            {
                return;
            }

            controllerState.ResetFrameDependentStates();
            // TODO 需要从 CommonSDK 中获取 HMD 当前按键信息
            // controllerState.selectInteractionState.SetFrameState();
            // controllerState.activateInteractionState.SetFrameState();
            // controllerState.uiPressInteractionState.SetFrameState();
        }

        public void OnInputTypeChanged(InputType inputType)
        {
            if (inputType == InputType.HMD)
            {
                EnableAllDirectAction();
            }
            else
            {
                DisableAllDirectAction();
            }
        }

        public void UpdateHMDInput(XRControllerState controllerState)
        {
            controllerState.inputTrackingState = InputTrackingState.Position | InputTrackingState.Rotation;

            if (m_HeadPositionAction.action != null && m_HeadPositionAction.action.bindings.Count > 0)
            {
                Vector3 headPosition = m_HeadPositionAction.action.ReadValue<Vector3>();
                controllerState.position = headPosition;
            }

            if (m_HeadRotationAction.action != null && m_HeadRotationAction.action.bindings.Count > 0)
            {
                Quaternion headRotation = m_HeadRotationAction.action.ReadValue<Quaternion>();
                controllerState.rotation = headRotation;
            }
        }
    }
}