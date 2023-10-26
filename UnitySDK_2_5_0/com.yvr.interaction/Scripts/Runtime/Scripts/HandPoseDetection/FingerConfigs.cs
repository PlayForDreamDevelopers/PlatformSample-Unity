using System;
using UnityEngine.XR;

namespace YVR.Interaction
{
    [Serializable]
    public class FingerConfigs
    {
        public RangeConfigs flexionConfigs;
        public RangeConfigs curlConfigs;
        public RangeConfigsAbduction abductionConfigs;

        public FingerConfigs(HandFinger finger)
        {
            flexionConfigs = new RangeConfigs(ShapesRecognizer.k_FlexionMin, ShapesRecognizer.k_FlexionMax, ShapesRecognizer.k_DefaultFlexionWidth);
            if (finger == HandFinger.Thumb)
            {
                curlConfigs = new RangeConfigs(ShapesRecognizer.k_CurlThumbMin, ShapesRecognizer.k_CurlThumbMax, ShapesRecognizer.k_DefaultCurlWidth);
                abductionConfigs = new RangeConfigsAbduction( ShapesRecognizer.k_AbductionThumbMid, ShapesRecognizer.k_AbductionThumbWidth);
            }
            else
            {
                curlConfigs = new RangeConfigs(ShapesRecognizer.k_CurlMin, ShapesRecognizer.k_CurlMax, ShapesRecognizer.k_DefaultCurlWidth);
                abductionConfigs = new RangeConfigsAbduction(ShapesRecognizer.k_AbductionMid, ShapesRecognizer.k_AbductionWidth);
            }
        }
    }
}
