using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace YVR.Interaction
{
    public class YVRHandPoseGenerator : MonoBehaviour
    {
        public YVRHandPoseConfig config;

        //Shapes
        public Finger thumb = new Finger(HandFinger.Thumb);
        public Finger index = new Finger(HandFinger.Index);
        public Finger middle = new Finger(HandFinger.Middle);
        public Finger ring = new Finger(HandFinger.Ring);
        public Finger pinky = new Finger(HandFinger.Pinky);

        public float shapesholdDuration = 0.09f;

        //Bones
        public List<BonesRecognizer.BonesGroup> Bones = new List<BonesRecognizer.BonesGroup>();

        public float bonesHoldDuration = 0.022f;

        //Trans
        public TransRecognizer.TrackAxis trackAxis;
        public TransRecognizer.SpaceType spaceType;
        public TransRecognizer.TrackTarget trackTarget;

        public float angleThreshold = 35f;
        public float thresholdWidth = 10f;
        public float transHoldDuration = 0.022f;
    }
}