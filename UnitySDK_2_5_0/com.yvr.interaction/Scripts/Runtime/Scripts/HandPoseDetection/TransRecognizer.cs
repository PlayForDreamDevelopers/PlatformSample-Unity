using System;

namespace YVR.Interaction
{
    [Serializable]
    public class TransRecognizer
    {
        public TrackAxis trackAxis;
        public SpaceType spaceType;
        public TrackTarget trackTarget;

        public enum SpaceType
        {
            WorldSpace,
            LocalXY,
            LocalYZ,
            LocalXZ
        }

        public enum TrackAxis
        {
            Fingers, Palm, Thumb
        }

        public enum TrackTarget
        {
            TowardsFace,
            AwayFromFace,
            WorldUp,
            WorldDown,
        }

        public float angleThreshold = 35f;
        public float thresholdWidth = 10f;
        public float holdDuration = 0.022f;
    }
}
