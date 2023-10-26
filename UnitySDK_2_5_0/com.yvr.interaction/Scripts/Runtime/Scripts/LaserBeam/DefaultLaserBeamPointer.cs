using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using YVR.Core;

namespace YVR.Interaction
{
    public class DefaultLaserBeamPointer : MonoBehaviour
    {
        [FormerlySerializedAs("_controllerType")] [SerializeField]
        private InputType m_InputType;

        /// <summary>
        /// X is the min value of the hit distance,Y is the max value of the hit distance
        /// </summary>
        [Tooltip("X is the min value of the hit distance,Y is the max value of the hit distance")]
        public Vector2 hitDistanceRange = new Vector2(0.1f, 1f);

        /// <summary>
        /// The idle configuration of the laser beam and cursor.
        /// </summary>
        public LaserBeamConfiguration idleConfiguration = new LaserBeamConfiguration()
        {
            startWidth = 0.0055f,
            endWidth = 0.0015f,
            startColor = new Color32(255, 255, 255, 205),
            endColor = new Color32(255, 255, 255, 0),
            cursorConfiguration = new CursorConfiguration()
            {
                cursorMinScale = 1f,
                cursorDotColor = Color.white
            }
        };

        /// <summary>
        /// The hold configuration of the laser beam and cursor.
        /// </summary>
        public LaserBeamConfiguration holdConfiguration = new LaserBeamConfiguration()
        {
            startWidth = 0.0041f,
            endWidth = 0.0015f,
            startColor = new Color32(23, 92, 230, 205),
            endColor = new Color32(23, 92, 230, 0),
            cursorConfiguration = new CursorConfiguration
            {
                cursorMinScale = 0.8f,
                cursorDotColor = new Color32(23, 92, 230, 255)
            }
        };

        [SerializeField]
        private GameObject cursorPrefab;

        private CursorBase m_Cursor;
        private LineRenderer m_LaserLine;
        private Transform m_Transform;

        private List<RaycastResult> m_RaycastList = new List<RaycastResult>();

        /// <summary>
        /// The controller type of laser beam.
        /// </summary>
        public InputType controllerType => m_InputType;

        /// <summary>
        /// The cursor of the laser beam.
        /// </summary>
        public CursorBase cursor
        {
            get => m_Cursor;
            set => m_Cursor = value;
        }

        #region Unity Event

        private void OnDisable()
        {
            //move away the line render
            if (m_LaserLine)
            {
                m_LaserLine.SetPosition(0, Vector3.one * 9998);
                m_LaserLine.SetPosition(1, Vector3.one * 9999);
            }
        }

        private void OnEnable()
        {
            m_Transform = XRRaycastManager.instance.GetXRRayInteractorOfType(m_InputType).rayOriginTransform;
        }

        private void Start()
        {
            m_LaserLine = GetComponent<LineRenderer>();
            if (m_LaserLine == null) m_LaserLine = CreateLaserBeam();

            if (cursor == null)
                m_Cursor = CreateDefaultCursor();
        }

        private void LateUpdate() { UpdateRaycast(); }

        #endregion

        #region

        private void UpdateRaycast()
        {
            RaycastResult result;
            XRRaycastManager.instance.GetXRRayInteractorOfType(m_InputType).TryGetCurrentUIRaycastResult(out result);
            var hitSth = result.gameObject != null;
            float distance = hitSth ? Vector3.Distance(result.worldPosition, m_Transform.position) - 0.01f : 0;
            if (cursor) cursor.Show(hitSth);
            Vector3 normal = hitSth ? result.worldNormal * -1 : m_Transform.forward;
            if (normal.Equals(Vector3.zero))
                normal = result.gameObject.transform.forward;

            bool currentControllerIsPressed = XRInputManager.instance.GetXRControllerOfType(m_InputType)
                .uiPressInteractionState.active;
            bool isHandPinch = false;
            if (m_InputType == InputType.LeftHand || m_InputType == InputType.RightHand)
            {
                HandType handType = m_InputType == InputType.LeftHand ? HandType.HandLeft : HandType.HandRight;
                isHandPinch = YVRHandManager.instance.GetPinch(handType, XRHandFingerID.Index);
                HandJointLocations handJointLocations = handType == HandType.HandLeft
                    ? YVRHandManager.instance.leftHandData
                    : YVRHandManager.instance.rightHandData;
                bool isPointerValid = (handJointLocations.aimState.status & HandStatus.InputStateValid) != 0;
                if (cursor) cursor.Show(hitSth && isPointerValid);
            }

            if (currentControllerIsPressed || isHandPinch)
            {
                UpdateLaserBeam(holdConfiguration, distance, normal);
                if (cursor) cursor.UpdateEffect(holdConfiguration.cursorConfiguration, distance, normal, result.gameObject);
            }
            else
            {
                UpdateLaserBeam(idleConfiguration, distance, normal);
                if (cursor) cursor.UpdateEffect(idleConfiguration.cursorConfiguration, distance, normal, result.gameObject);
            }
        }

        private void UpdateLaserBeam(LaserBeamConfiguration configuration, float distance, Vector3 normal)
        {
            if (m_LaserLine)
            {
                m_LaserLine.startColor = configuration.startColor;
                m_LaserLine.endColor = configuration.endColor;
                m_LaserLine.startWidth = configuration.startWidth;
                m_LaserLine.endWidth = configuration.endWidth;

                m_LaserLine.SetPosition(0, m_Transform.position + m_Transform.forward * configuration.startPointOffset);
                float beamLength = Mathf.Clamp(distance, hitDistanceRange.x, hitDistanceRange.y);
                m_LaserLine.SetPosition(1, m_Transform.position + m_Transform.forward * beamLength);
            }
        }

        private LineRenderer CreateLaserBeam()
        {
            var lineRender = gameObject.AddComponent<LineRenderer>();
            return lineRender;
        }

        private CursorBase CreateDefaultCursor()
        {
            if (cursorPrefab && cursorPrefab.GetComponent<CursorBase>())
            {
                var cur = Instantiate<GameObject>(cursorPrefab, m_Transform);
                return cur.GetComponent<CursorBase>();
            }

            return null;
        }

        #endregion
    }
}
