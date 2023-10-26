using System;
using UnityEngine;
using UnityEngine.XR;

namespace YVR.Interaction
{
    [Serializable]
    public class Finger
    {
        [HideInInspector]
        public HandFinger handFinger;
        public Flexion flexion;
        public Curl curl;
        public Abduction abduction;
        public FingerConfigs fingerConfigs;

        public Finger(HandFinger finger)
        {
            handFinger = finger;
            flexion = Flexion.Any;
            curl = Curl.Any;
            abduction = Abduction.Any;
            fingerConfigs = new FingerConfigs(finger);
        }
    }
}
