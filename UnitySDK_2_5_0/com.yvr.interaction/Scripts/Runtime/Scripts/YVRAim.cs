using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;
using UnityEngine.XR;
using YVR.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif
using CommonUsages = UnityEngine.InputSystem.CommonUsages;

namespace YVR.Interaction
{
    /// <summary>
    ///     The flags in the extension for each hand that can be read from
    ///     <see cref="YVRAimHandDevice.aimFlags" /> and casting to this type.
    /// </summary>
    [Flags]
    [Preserve]
    public enum YVRAimFlags : ulong
    {
        /// <summary>
        ///     No flags are valid.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Data for this hand has been computed.
        /// </summary>
        Computed = 1 << 0,

        /// <summary>
        ///     The aim pose is valid. Retrieve this data from
        ///     <see cref="TrackedDevice.devicePosition" /> and
        ///     <see cref="TrackedDevice.deviceRotation" /> that
        ///     <see cref="YVRAimHand" /> inherits.
        /// </summary>
        Valid = 1 << 1,

        /// <summary>
        ///     Indicates whether the index finger is pinching with the thumb.
        ///     Only valid when the pinch strength retrieved from
        ///     <see cref="YVRAimHand.pinchStrengthIndex" /> is
        ///     at a full strength of <c>1.0</c>.
        /// </summary>
        IndexPinching = 1 << 2,

        /// <summary>
        ///     Indicates whether the middle finger is pinching with the thumb.
        ///     Only valid when the pinch strength retrieved from
        ///     <see cref="YVRAimHand.pinchStrengthMiddle" /> is
        ///     at a full strength of <c>1.0</c>.
        /// </summary>
        MiddlePinching = 1 << 3,

        /// <summary>
        ///     Indicates whether the ring finger is pinching with the thumb.
        ///     Only valid when the pinch strength retrieved from
        ///     <see cref="YVRAimHand.pinchStrengthRing" /> is
        ///     at a full strength of <c>1.0</c>.
        /// </summary>
        RingPinching = 1 << 4,

        /// <summary>
        ///     Indicates whether the little finger is pinching with the thumb.
        ///     Only valid when the pinch strength retrieved from
        ///     <see cref="YVRAimHand.pinchStrengthLittle" /> is
        ///     at a full strength of <c>1.0</c>.
        /// </summary>
        LittlePinching = 1 << 5,

        /// <summary>
        ///     Indicates whether a system gesture is being performed (when the
        ///     palm of the hand is facing the headset).
        /// </summary>
        SystemGesture = 1 << 6,

        /// <summary>
        ///     Indicates whether the hand these flags were retrieved from is
        ///     the dominant hand.
        /// </summary>
        DominantHand = 1 << 7,

        /// <summary>
        ///     Indicates whether the menu gesture button is pressed.
        /// </summary>
        MenuPressed = 1 << 8
    }

    /// <summary>
    ///     A <see cref="TrackedDevice" /> based off the data exposed in the
    ///     <see href="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#XR_FB_hand_tracking_aim">
    ///         YVR Hand Tracking Aim extension
    ///     </see>
    ///     . Enabled through <see cref="YVRHandTrackingAim" />
    ///     back-end is enabled.
    /// </summary>
    /// <remarks>
    ///     For this type to function, you must enable hand-tracking and be running
    /// </remarks>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [Preserve]
    [InputControlLayout(displayName = "YVR Aim Hand", commonUsages = new[] { "LeftHand", "RightHand" })]
    public class YVRAimHand : TrackedDevice
    {
        /// <summary>
        ///     The pinch amount required to register as being pressed for the
        ///     purposes of <see cref="indexPressed" />, <see cref="middlePressed" />,
        ///     <see cref="ringPressed" />, and <see cref="littlePressed" />.
        /// </summary>
        public const float pressThreshold = 0.8f;


        private const string k_YVRAimHandDeviceProductName = "YVR Aim Hand Tracking";
        private const string k_YVRAimHandDeviceManufacturerName = "YVR Aim";

        private HandType m_Handedness;
        private YVRAimFlags m_PreviousFlags;
        private bool m_WasIndexPressed;
        private bool m_WasLittlePressed;
        private bool m_WasMiddlePressed;
        private bool m_WasRingPressed;
        private bool m_WasTracked;

        static YVRAimHand()
        {
            RegisterLayout();
        }

        /// <summary>
        ///     The left-hand <see cref="UnityEngine.InputSystem.InputDevice" /> that contains
        ///     <see cref="InputControl" />s that surface data in the YVR Hand
        ///     Tracking Aim extension.
        /// </summary>
        /// <remarks>
        ///     It is recommended that you treat this as read-only, and do not set
        ///     it yourself. It will be set for you if hand-tracking has been
        ///     plug-in.
        /// </remarks>
        public static YVRAimHand left { get; set; }

        /// <summary>
        ///     The right-hand <see cref="UnityEngine.InputSystem.InputDevice" /> that contains
        ///     <see cref="InputControl" />s that surface data in the YVR Hand
        ///     Tracking Aim extension.
        /// </summary>
        /// <remarks>
        ///     It is recommended that you treat this as read-only, and do not set
        ///     it yourself. It will be set for you if hand-tracking has been
        ///     plug-in.
        /// </remarks>
        public static YVRAimHand right { get; set; }

        /// <summary>
        ///     A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl)
        ///     that represents whether the pinch between the index finger and
        ///     the thumb is mostly pressed (greater than a threshold of <c>0.8</c>
        ///     contained in <see cref="pressThreshold" />).
        /// </summary>
        [Preserve]
        [InputControl(offset = 0)]
        public ButtonControl indexPressed { get; private set; }

        /// <summary>
        ///     A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl)
        ///     that represents whether the pinch between the middle finger and
        ///     the thumb is mostly pressed (greater than a threshold of <c>0.8</c>
        ///     contained in <see cref="pressThreshold" />).
        /// </summary>
        [Preserve]
        [InputControl(offset = 1)]
        public ButtonControl middlePressed { get; private set; }

        /// <summary>
        ///     A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl)
        ///     that represents whether the pinch between the ring finger and
        ///     the thumb is mostly pressed (greater than a threshold of <c>0.8</c>
        ///     contained in <see cref="pressThreshold" />).
        /// </summary>
        [Preserve]
        [InputControl(offset = 2)]
        public ButtonControl ringPressed { get; private set; }

        /// <summary>
        ///     A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl)
        ///     that represents whether the pinch between the little finger and
        ///     the thumb is mostly pressed (greater than a threshold of <c>0.8</c>
        ///     contained in <see cref="pressThreshold" />).
        /// </summary>
        [Preserve]
        [InputControl(offset = 3)]
        public ButtonControl littlePressed { get; private set; }

        /// <summary>
        ///     Cast the result of reading this to <see cref="YVRAimFlags" /> to examine the value.
        /// </summary>
        [Preserve]
        [InputControl]
        public IntegerControl aimFlags { get; private set; }

        /// <summary>
        ///     An [AxisControl](xref:UnityEngine.InputSystem.Controls.AxisControl)
        ///     that represents the pinch strength between the index finger and
        ///     the thumb.
        /// </summary>
        /// <remarks>
        ///     A value of <c>0</c> denotes no pinch at all, while a value of
        ///     <c>1</c> denotes a full pinch.
        /// </remarks>
        [Preserve]
        [InputControl]
        public AxisControl pinchStrengthIndex { get; private set; }

        /// <summary>
        ///     An [AxisControl](xref:UnityEngine.InputSystem.Controls.AxisControl)
        ///     that represents the pinch strength between the middle finger and
        ///     the thumb.
        /// </summary>
        /// <remarks>
        ///     A value of <c>0</c> denotes no pinch at all, while a value of
        ///     <c>1</c> denotes a full pinch.
        /// </remarks>
        [Preserve]
        [InputControl]
        public AxisControl pinchStrengthMiddle { get; private set; }

        /// <summary>
        ///     An [AxisControl](xref:UnityEngine.InputSystem.Controls.AxisControl)
        ///     that represents the pinch strength between the ring finger and
        ///     the thumb.
        /// </summary>
        /// <remarks>
        ///     A value of <c>0</c> denotes no pinch at all, while a value of
        ///     <c>1</c> denotes a full pinch.
        /// </remarks>
        [Preserve]
        [InputControl]
        public AxisControl pinchStrengthRing { get; private set; }

        /// <summary>
        ///     An [AxisControl](xref:UnityEngine.InputSystem.Controls.AxisControl)
        ///     that represents the pinch strength between the little finger and
        ///     the thumb.
        /// </summary>
        /// <remarks>
        ///     A value of <c>0</c> denotes no pinch at all, while a value of
        ///     <c>1</c> denotes a full pinch.
        /// </remarks>
        [Preserve]
        [InputControl]
        public AxisControl pinchStrengthLittle { get; private set; }

        /// <summary>
        ///     Perform final initialization tasks after the control hierarchy has been put into place.
        /// </summary>
        protected override void FinishSetup()
        {
            base.FinishSetup();

            indexPressed = GetChildControl<ButtonControl>(nameof(indexPressed));
            middlePressed = GetChildControl<ButtonControl>(nameof(middlePressed));
            ringPressed = GetChildControl<ButtonControl>(nameof(ringPressed));
            littlePressed = GetChildControl<ButtonControl>(nameof(littlePressed));
            aimFlags = GetChildControl<IntegerControl>(nameof(aimFlags));
            pinchStrengthIndex = GetChildControl<AxisControl>(nameof(pinchStrengthIndex));
            pinchStrengthMiddle = GetChildControl<AxisControl>(nameof(pinchStrengthMiddle));
            pinchStrengthRing = GetChildControl<AxisControl>(nameof(pinchStrengthRing));
            pinchStrengthLittle = GetChildControl<AxisControl>(nameof(pinchStrengthLittle));

            var deviceDescriptor = XRDeviceDescriptor.FromJson(description.capabilities);
            if (deviceDescriptor != null)
            {
                if ((deviceDescriptor.characteristics & InputDeviceCharacteristics.Left) != 0)
                    InputSystem.SetDeviceUsage(this,
                        CommonUsages.LeftHand);
                else if ((deviceDescriptor.characteristics & InputDeviceCharacteristics.Right) != 0)
                    InputSystem.SetDeviceUsage(this,
                        CommonUsages.RightHand);
            }
        }

        /// <summary>
        ///     Creates a <see cref="YVRAimHand" /> and adds it to the Input System.
        /// </summary>
        /// <param name="extraCharacteristics">
        ///     Additional characteristics to build the hand device with besides
        ///     <see cref="InputDeviceCharacteristics.HandTracking" />.
        /// </param>
        /// <returns>
        ///     A <see cref="YVRAimHand" /> retrieved from <see cref="InputSystem.AddDevice" />.
        /// </returns>
        /// <remarks>
        ///     It is recommended that you do not call this yourself. It will be
        ///     called for you at the appropriate time if hand-tracking has been
        ///     plug-in.
        /// </remarks>
        public static YVRAimHand CreateHand(InputDeviceCharacteristics extraCharacteristics)
        {
            var desc = new InputDeviceDescription
            {
                product = k_YVRAimHandDeviceProductName,
                manufacturer = k_YVRAimHandDeviceManufacturerName,
                capabilities = new XRDeviceDescriptor
                {
                    characteristics = InputDeviceCharacteristics.HandTracking | extraCharacteristics,
                    inputFeatures = new List<XRFeatureDescriptor>
                    {
                        new()
                        {
                            name = "index_pressed",
                            featureType = FeatureType.Binary
                        },
                        new()
                        {
                            name = "middle_pressed",
                            featureType = FeatureType.Binary
                        },
                        new()
                        {
                            name = "ring_pressed",
                            featureType = FeatureType.Binary
                        },
                        new()
                        {
                            name = "little_pressed",
                            featureType = FeatureType.Binary
                        },
                        new()
                        {
                            name = "aim_flags",
                            featureType = FeatureType.DiscreteStates
                        },
                        new()
                        {
                            name = "aim_pose_position",
                            featureType = FeatureType.Axis3D
                        },
                        new()
                        {
                            name = "aim_pose_rotation",
                            featureType = FeatureType.Rotation
                        },
                        new()
                        {
                            name = "pinch_strength_index",
                            featureType = FeatureType.Axis1D
                        },
                        new()
                        {
                            name = "pinch_strength_middle",
                            featureType = FeatureType.Axis1D
                        },
                        new()
                        {
                            name = "pinch_strength_ring",
                            featureType = FeatureType.Axis1D
                        },
                        new()
                        {
                            name = "pinch_strength_little",
                            featureType = FeatureType.Axis1D
                        }
                    }
                }.ToJson()
            };
            var handAim = InputSystem.AddDevice(desc) as YVRAimHand;
            handAim.m_Handedness = extraCharacteristics == InputDeviceCharacteristics.Left
                ? HandType.HandLeft
                : HandType.HandRight;
            Application.onBeforeRender += handAim.OnUpdateHandAims;
            return handAim;
        }

        private void OnUpdateHandAims()
        {
            UpdateHand(m_Handedness == HandType.HandLeft);
        }

        public void OnDestory()
        {
            Application.onBeforeRender -= OnUpdateHandAims;
        }

        /// <summary>
        ///     Queues update events in the Input System based on the supplied hand.
        ///     It is not recommended that you call this directly. This will be called
        ///     for you when appropriate.
        /// </summary>
        /// <param name="aimFlags">
        ///     The aim flags to update in the Input System.
        /// </param>
        /// <param name="aimPose">
        ///     The aim pose to update in the Input System.
        /// </param>
        /// <param name="pinchIndex">
        ///     The pinch strength for the index finger to update in the Input System.
        /// </param>
        /// <param name="pinchMiddle">
        ///     The pinch strength for the middle finger to update in the Input System.
        /// </param>
        /// <param name="pinchRing">
        ///     The pinch strength for the ring finger to update in the Input System.
        /// </param>
        /// <param name="pinchLittle">
        ///     The pinch strength for the little finger to update in the Input System.
        /// </param>
        public void UpdateHand(
            YVRAimFlags aimFlags,
            Pose aimPose,
            float pinchIndex,
            float pinchMiddle,
            float pinchRing,
            float pinchLittle)
        {
            if (aimFlags != m_PreviousFlags)
            {
                InputSystem.QueueDeltaStateEvent(this.aimFlags, (int)aimFlags);
                m_PreviousFlags = aimFlags;
            }

            var isIndexPressed = pinchIndex > pressThreshold;
            if (isIndexPressed != m_WasIndexPressed)
            {
                InputSystem.QueueDeltaStateEvent(indexPressed, isIndexPressed);
                m_WasIndexPressed = isIndexPressed;
            }

            var isMiddlePressed = pinchMiddle > pressThreshold;
            if (isMiddlePressed != m_WasMiddlePressed)
            {
                InputSystem.QueueDeltaStateEvent(middlePressed, isMiddlePressed);
                m_WasMiddlePressed = isMiddlePressed;
            }

            var isRingPressed = pinchRing > pressThreshold;
            if (isRingPressed != m_WasRingPressed)
            {
                InputSystem.QueueDeltaStateEvent(ringPressed, isRingPressed);
                m_WasRingPressed = isRingPressed;
            }

            var isLittlePressed = pinchLittle > pressThreshold;
            if (isLittlePressed != m_WasLittlePressed)
            {
                InputSystem.QueueDeltaStateEvent(littlePressed, isLittlePressed);
                m_WasLittlePressed = isLittlePressed;
            }

            InputSystem.QueueDeltaStateEvent(pinchStrengthIndex, pinchIndex);
            InputSystem.QueueDeltaStateEvent(pinchStrengthMiddle, pinchMiddle);
            InputSystem.QueueDeltaStateEvent(pinchStrengthRing, pinchRing);
            InputSystem.QueueDeltaStateEvent(pinchStrengthLittle, pinchLittle);

            if ((aimFlags & YVRAimFlags.Computed) == YVRAimFlags.None)
            {
                if (m_WasTracked)
                {
                    InputSystem.QueueDeltaStateEvent(isTracked, false);
                    InputSystem.QueueDeltaStateEvent(trackingState, InputTrackingState.None);
                    m_WasTracked = false;
                }

                return;
            }

            if ((aimFlags & YVRAimFlags.Valid) != YVRAimFlags.None)
            {
                InputSystem.QueueDeltaStateEvent(devicePosition, aimPose.position.FromFlippedZVector3f());
                InputSystem.QueueDeltaStateEvent(deviceRotation, aimPose.rotation.FromFlippedZQuatf());

                if (!m_WasTracked)
                {
                    InputSystem.QueueDeltaStateEvent(trackingState,
                        InputTrackingState.Position | InputTrackingState.Rotation);
                    InputSystem.QueueDeltaStateEvent(isTracked, true);
                }

                m_WasTracked = true;
            }
            else if (m_WasTracked)
            {
                InputSystem.QueueDeltaStateEvent(trackingState, InputTrackingState.None);
                InputSystem.QueueDeltaStateEvent(isTracked, false);
                m_WasTracked = false;
            }
        }

        internal void UpdateHand(bool isLeft)
        {
            UpdateAim(
                isLeft,
                out var aimFlags,
                out var aimPosePosition,
                out var aimPoseRotation,
                out var pinchIndex,
                out var pinchMiddle,
                out var pinchRing,
                out var pinchLittle);

            UpdateHand(
                aimFlags,
                new Pose(aimPosePosition, aimPoseRotation),
                pinchIndex,
                pinchMiddle,
                pinchRing,
                pinchLittle);
        }

        private void UpdateAim(
            bool isLeft,
            out YVRAimFlags aimFlags,
            out Vector3 aimPosePosition,
            out Quaternion aimPoseRotation,
            out float pinchStrengthIndex,
            out float pinchStrengthMiddle,
            out float pinchStrengthRing,
            out float pinchStrengthLittle)
        {
            var aimState = isLeft
                ? YVRHandManager.instance.leftHandData.aimState
                : YVRHandManager.instance.rightHandData.aimState;
            aimFlags = (YVRAimFlags)aimState.status;
            aimPosePosition = aimState.aimPose.position;
            aimPoseRotation = aimState.aimPose.orientation;
            pinchStrengthIndex = aimState.pinchStrengthIndex;
            pinchStrengthMiddle = aimState.pinchStrengthMiddle;
            pinchStrengthRing = aimState.pinchStrengthRing;
            pinchStrengthLittle = aimState.pinchStrengthLittle;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterLayout()
        {
            InputSystem.RegisterLayout<YVRAimHand>(
                matches: new InputDeviceMatcher()
                    .WithProduct(k_YVRAimHandDeviceProductName)
                    .WithManufacturer(k_YVRAimHandDeviceManufacturerName));
        }
    }
}