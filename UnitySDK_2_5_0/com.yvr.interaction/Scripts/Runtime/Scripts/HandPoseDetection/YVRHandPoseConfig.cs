using System;
using UnityEngine;

namespace YVR.Interaction
{
   [Serializable]
    public class YVRHandPoseConfig : ScriptableObject
    {
        [DisplayOnly]
        public ShapesRecognizer shapesRecognizer;
        [DisplayOnly]
        public BonesRecognizer bonesRecognizer;
        [DisplayOnly]
        public TransRecognizer transRecognizer;
    }

    public class DisplayOnly : PropertyAttribute { }
}