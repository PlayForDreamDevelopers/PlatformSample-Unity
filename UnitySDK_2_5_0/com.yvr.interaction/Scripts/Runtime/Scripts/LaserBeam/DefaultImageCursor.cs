using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.UI;

namespace YVR.Interaction
{
    [ExcludeFromDocs]
    public class DefaultImageCursor : CursorBase
    {
        public Image dot;

        private new GameObject gameObject;
        public override GameObject cursorGameObject => this.gameObject;

        private new Transform transform;
        public override Transform cursorTransform => this.transform;

        void Awake()
        {
            this.transform = base.transform;
            this.gameObject = base.gameObject;
        }

        public override void UpdateEffect(CursorConfiguration configuration, float distance, Vector3 normal, GameObject hitGameObject)
        {
            float scaleParam = Mathf.Max(configuration.cursorMinScale, distance) * 0.1f;
            transform.localScale = new Vector3(scaleParam, scaleParam, transform.localScale.z);
            transform.localPosition = Vector3.forward * distance;
            transform.forward = normal;
            if (dot)
                dot.color = configuration.cursorDotColor;
        }

        public override void Show(bool display)
        {
            if (gameObject) gameObject.SetActive(display);
        }
    }
}
