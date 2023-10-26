using System;
using UnityEngine;

namespace YVR.Interaction
{
    /// <summary>
    /// The configuration of cursor
    /// </summary>
    [Serializable]
    public class CursorConfiguration
    {
        /// <summary>
        /// The min scale of the cursor.
        /// </summary>
        [Header("Cursor")]
        public float cursorMinScale = 1f;
        /// <summary>
        /// The color of the cursor dot.
        /// </summary>
        public Color cursorDotColor = Color.white;
    }
}
