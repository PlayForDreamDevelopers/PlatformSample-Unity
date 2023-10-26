using System;
using UnityEngine.XR;

namespace YVR.Interaction
{
    [Serializable]
    public class ShapesRecognizer
    {
        public Finger thumb = new Finger(HandFinger.Thumb);
        public Finger index = new Finger(HandFinger.Index);
        public Finger middle = new Finger(HandFinger.Middle);
        public Finger ring = new Finger(HandFinger.Ring);
        public Finger pinky = new Finger(HandFinger.Pinky);
        public float holdDuration = 0.09f;

        public const float k_DefaultFlexionWidth = 10f;
        public const float k_DefaultCurlWidth = 20f;
        public const float k_FlexionThumbOpenMin = 155f;
        public const float k_FlexionThumbOpenMax = 180f;
        public const float k_FlexionThumbCloseMin = 90f;
        public const float k_FlexionThumbCloseMax = 120f;

        public const float k_FlexionOpenMin = 144f;
        public const float k_FlexionOpenMax = 180f;
        public const float k_FlexionCloseMin = 90f;
        public const float k_FlexionCloseMax = 126f;
        public const float k_FlexionMin = 90f;
        public const float k_FlexionMax = 180f;

        public const float k_CurlThumbOpenMin = 90f;
        public const float k_CurlThumbOpenMax = 180f;
        public const float k_CurlThumbCloseMin = 45f;
        public const float k_CurlThumbCloseMax = 90f;
        public const float k_CurlThumbMin = 45f;
        public const float k_CurlThumbMax = 180f;

        public const float k_CurlOpenMin = 107f;
        public const float k_CurlOpenMax = 180f;
        public const float k_CurlCloseMin = 0f;
        public const float k_CurlCloseMax = 73f;
        public const float k_CurlMin = 0f;
        public const float k_CurlMax = 180f;

        public const float k_AbductionThumbMid = 13f;
        public const float k_AbductionThumbWidth = 6f;

        public const float k_AbductionMid = 10f;
        public const float k_AbductionWidth = 6f;
        public const float k_AbductionMin = 0f;
        public const float k_AbductionMax = 90f;
    }

    public enum ShapeType
    {
        flexion,
        curl,
        abduction
    }

    public enum Flexion
    {
        Any,
        Open,
        Close,
        Custom
    }

    public enum Curl
    {
        Any,
        Open,
        Close,
        Custom
    }

    public enum Abduction
    {
        Any,
        Open,
        Close,
    }
}