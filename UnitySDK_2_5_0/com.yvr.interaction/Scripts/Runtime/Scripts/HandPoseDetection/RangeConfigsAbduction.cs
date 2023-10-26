using System;

namespace YVR.Interaction
{
    [Serializable]
    public class RangeConfigsAbduction
    {
        public float mid;
        public float width;
        public RangeConfigsAbduction(float m, float w)
        {
            mid = m;
            width = w;
        }
    }
}