using UnityEngine;

namespace YVR.Interaction
{
    /// <summary>
    /// The base class of the cursor.
    /// </summary>
    public abstract class CursorBase:MonoBehaviour
    {
        /// <summary>
        /// The gameobject of cursor
        /// </summary>
        public abstract GameObject cursorGameObject { get; }

        /// <summary>
        /// The transform of cursor
        /// </summary>
        public abstract Transform cursorTransform { get; }

        /// <summary>
        /// Change the cursor effect according to the configuration and raycast results.
        /// </summary>
        /// <param name="configuration">The configuration of cursor</param>
        /// <param name="distance">The distace of raycast result</param>
        /// <param name="normal">The normal of raycast gameobject</param>
        /// <param name="hitGameObject">The raycast gameobject</param>
        public abstract void UpdateEffect(CursorConfiguration configuration, float distance, Vector3 normal,GameObject hitGameObject);

        /// <summary>
        /// Control the display and close of the cursor.
        /// </summary>
        /// <param name="display">Whether to show cursor</param>
        public abstract void Show(bool display);
    }
}
