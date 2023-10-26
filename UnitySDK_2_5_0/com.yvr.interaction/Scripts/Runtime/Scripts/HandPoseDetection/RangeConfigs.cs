using System;

namespace YVR.Interaction
{
    [Serializable]
    public class RangeConfigs
    {
        public float min;
        public float max;
        public float width;
        public RangeConfigs(float n, float m, float w)
        {
            min = n;
            max = m;
            width =w;
        }
    }
}
