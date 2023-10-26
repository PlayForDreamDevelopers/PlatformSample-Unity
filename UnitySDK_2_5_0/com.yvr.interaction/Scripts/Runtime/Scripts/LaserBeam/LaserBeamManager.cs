using UnityEngine;

namespace YVR.Interaction
{
    public class LaserBeamManager : MonoBehaviour
    {
        public DefaultLaserBeamPointer leftCrontrollerLaserBeam;
        public DefaultLaserBeamPointer rightCrontrollerLaserBeam;

        private void Start()
        {
            XRInputManager.instance.onInputTypeChanged += SwitchLaserBeam;
        }

        private void OnDestroy()
        {
            XRInputManager.instance.onInputTypeChanged -= SwitchLaserBeam;
        }

        private void SwitchLaserBeam(InputType inputType)
        {
            leftCrontrollerLaserBeam.enabled = (inputType == InputType.LeftController || inputType == InputType.AllController);
            rightCrontrollerLaserBeam.enabled = (inputType == InputType.RightController || inputType == InputType.AllController);
        }
    }
}
