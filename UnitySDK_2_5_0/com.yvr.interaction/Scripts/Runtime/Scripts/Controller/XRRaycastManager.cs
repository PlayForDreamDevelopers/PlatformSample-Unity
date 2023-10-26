using UnityEngine.XR.Interaction.Toolkit;

namespace YVR.Interaction
{
    public class XRRaycastManager : MonoBehaviorSingleton<XRRaycastManager>
    {
        public XRRayInteractor leftControllerInteractor;
        public XRRayInteractor rightControllerInteractor;
        public XRRayInteractor leftHandInteractor;
        public XRRayInteractor rightHandInteractor;
        public XRRayInteractor headInteractor;

        public XRRayInteractor GetXRRayInteractorOfType(InputType inputType)
        {
            switch (inputType)
            {
                case InputType.LeftController:
                    return leftControllerInteractor;
                case InputType.RightController:
                    return rightControllerInteractor;
                case InputType.LeftHand:
                    return leftHandInteractor;
                case InputType.RightHand:
                    return rightHandInteractor;
                case InputType.HMD:
                    return headInteractor;
                default:
                    return headInteractor;
            }
        }
    }
}