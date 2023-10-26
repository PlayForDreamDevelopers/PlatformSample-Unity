using System;
using System.Collections.Generic;
using UnityEngine;
using YVR.Core;

namespace YVR.Interaction
{
    [Serializable]
    public class BonesRecognizer
    {
        public List<BonesGroup> Bones = new List<BonesGroup>();

        public float holdDuration = 0.022f;
        [Serializable]
        public class BonesGroup
        {
            public HandJoint A_Bone = HandJoint.JointWrist;
            public HandJoint B_Bone = HandJoint.JointWrist;
            public float distance = 0.025f;
            public float thresholdWidth = 0.003f;

            [HideInInspector]
            public bool activeState;
        }
    }
}