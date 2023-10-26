using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine.XR;
using YVR.Core;

namespace YVR.Interaction.Editor
{
    [CustomEditor(typeof(YVRHandPoseGenerator), true)]
    public class YVRHandPoseGeneratorEditor : UnityEditor.Editor
    {
        private bool shapesRecognizer = true;
        private bool bonesRecognizer = true;
        private bool transRecognizer = true;
        private bool thumb = true;
        private bool index = true;
        private bool middle = true;
        private bool ring = true;
        private bool pinky = true;

        private YVRHandPoseGenerator m_Target;
        private YVRHandPoseConfig config;

        private void OnEnable()
        {
            m_Target = (YVRHandPoseGenerator)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Hand Pose Config");
            m_Target.config = (YVRHandPoseConfig)EditorGUILayout.ObjectField(m_Target.config, typeof(YVRHandPoseConfig), false);

            if (GUILayout.Button("New"))
            {
                m_Target.config = CreateInstance<YVRHandPoseConfig>();
                AssetDatabase.CreateAsset(m_Target.config, string.Format("Assets/{0}.asset", typeof(YVRHandPoseConfig).Name+"_"+DateTime.Now.ToString("MMddhhmmss")));
            }

            if (GUILayout.Button("Save"))
            {
                try
                {
                    ConfigSave();

                    Debug.Log("HandPose Config Saved.");
                }
                catch (Exception e)
                {
                    Debug.LogError("Save Error "+e.ToString());
                }
                AssetDatabase.Refresh();
            }

            GUILayout.EndHorizontal();

            if (m_Target.config != null)
            {
                if (config != m_Target.config)
                {
                    config = m_Target.config;
                    ConfigRead();
                }

                shapesRecognizer = EditorGUILayout.Foldout(shapesRecognizer, "Shapes Recognizer");
                if (shapesRecognizer)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    using (new GUILayout.VerticalScope())
                    {
                        thumb = EditorGUILayout.Foldout(thumb, "Thumb");
                        if (thumb)
                        {
                            FingerConfig(m_Target.thumb);
                        }
                        index = EditorGUILayout.Foldout(index, "Index");
                        if (index)
                        {
                            FingerConfig(m_Target.index);
                        }
                        middle = EditorGUILayout.Foldout(middle, "Middle");
                        if (middle)
                        {
                            FingerConfig(m_Target.middle);
                        }
                        ring = EditorGUILayout.Foldout(ring, "Ring");
                        if (ring)
                        {
                            FingerConfig(m_Target.ring);
                        }
                        pinky = EditorGUILayout.Foldout(pinky, "Pinky");
                        if (pinky)
                        {
                            FingerConfig(m_Target.pinky);
                        }
                        EditorGUILayout.Space(5);
                        serializedObject.FindProperty("shapesholdDuration").floatValue = EditorGUILayout.FloatField("Hold Duration", Mathf.Max(0, serializedObject.FindProperty("shapesholdDuration").floatValue));
                    }
                    EditorGUILayout.EndHorizontal();

                }

                bonesRecognizer = EditorGUILayout.Foldout(bonesRecognizer, "Bones Recognizer");
                if (bonesRecognizer)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    using (new GUILayout.VerticalScope())
                    {
                        BonesConfig(m_Target.Bones);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                transRecognizer = EditorGUILayout.Foldout(transRecognizer, "Transform Recognizer");
                if (transRecognizer)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    using (new GUILayout.VerticalScope())
                    {
                        TransConfig();
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorUtility.SetDirty(m_Target.config);
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void ConfigSave()
        {
            m_Target.config.shapesRecognizer.holdDuration = m_Target.shapesholdDuration;
            m_Target.config.shapesRecognizer.thumb = m_Target.thumb;
            m_Target.config.shapesRecognizer.index = m_Target.index;
            m_Target.config.shapesRecognizer.middle = m_Target.middle;
            m_Target.config.shapesRecognizer.ring = m_Target.ring;
            m_Target.config.shapesRecognizer.pinky = m_Target.pinky;

            m_Target.config.bonesRecognizer.holdDuration = m_Target.bonesHoldDuration;
            m_Target.config.bonesRecognizer.Bones = m_Target.Bones;

            m_Target.config.transRecognizer.holdDuration = m_Target.transHoldDuration;
            m_Target.config.transRecognizer.trackAxis = m_Target.trackAxis;
            m_Target.config.transRecognizer.spaceType = m_Target.spaceType;
            m_Target.config.transRecognizer.trackTarget = m_Target.trackTarget;
            m_Target.config.transRecognizer.angleThreshold = m_Target.angleThreshold;
            m_Target.config.transRecognizer.thresholdWidth = m_Target.thresholdWidth;
        }

        private void ConfigRead()
        {
            m_Target.shapesholdDuration = m_Target.config.shapesRecognizer.holdDuration;
            m_Target.thumb = m_Target.config.shapesRecognizer.thumb;
            m_Target.index = m_Target.config.shapesRecognizer.index;
            m_Target.middle = m_Target.config.shapesRecognizer.middle;
            m_Target.ring = m_Target.config.shapesRecognizer.ring;
            m_Target.pinky = m_Target.config.shapesRecognizer.pinky;

            m_Target.bonesHoldDuration = m_Target.config.bonesRecognizer.holdDuration;
            m_Target.Bones = m_Target.config.bonesRecognizer.Bones;

            m_Target.transHoldDuration = m_Target.config.transRecognizer.holdDuration;
            m_Target.trackAxis = m_Target.config.transRecognizer.trackAxis;
            m_Target.spaceType = m_Target.config.transRecognizer.spaceType;
            m_Target.trackTarget = m_Target.config.transRecognizer.trackTarget;
            m_Target.angleThreshold = m_Target.config.transRecognizer.angleThreshold;
            m_Target.thresholdWidth = m_Target.config.transRecognizer.thresholdWidth;
        }

        private void TransConfig()
        {
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.Space(5);
            using (new GUILayout.VerticalScope())
            {
                EditorGUILayout.Space(5);
                m_Target.trackAxis = (TransRecognizer.TrackAxis)EditorGUILayout.EnumPopup("Track Axis", m_Target.trackAxis);
                m_Target.trackTarget = (TransRecognizer.TrackTarget)EditorGUILayout.EnumPopup("Track Target", m_Target.trackTarget);
                m_Target.angleThreshold = EditorGUILayout.FloatField("Angle Threshold", m_Target.angleThreshold);
                m_Target.thresholdWidth = EditorGUILayout.FloatField("Threshold Width", m_Target.thresholdWidth);
                EditorGUILayout.Space(5);
            }
            EditorGUILayout.Space(5);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
            m_Target.transHoldDuration = EditorGUILayout.FloatField("Hold Duration", m_Target.transHoldDuration);
        }

        private void BonesConfig(List<BonesRecognizer.BonesGroup> listBones)
        {
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.Space(5);
            using (new GUILayout.VerticalScope())
            {
                EditorGUILayout.Space(5);

                GUI.changed = false;
                int count = EditorGUILayout.IntField("Size", listBones.Count);
                if (GUI.changed)
                {
                    if (count >= 0 && count != listBones.Count)
                    {
                        if (count > listBones.Count)
                        {
                            count = count - listBones.Count;
                            for (int i = 0; i < count; i++)
                            {
                                listBones.Add(new BonesRecognizer.BonesGroup());
                            }
                        }
                        else
                        {
                            count = listBones.Count - count;
                            for (int i = 0; i < count; i++)
                            {
                                listBones.Remove(listBones[listBones.Count-1]);
                            }
                        }
                    }
                }

                foreach (var bones in listBones)
                {
                    EditorGUILayout.LabelField("Element "+listBones.IndexOf(bones));
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    using (new GUILayout.VerticalScope())
                    {
                        bones.A_Bone = (HandJoint)EditorGUILayout.EnumPopup("Bone1", bones.A_Bone);
                        bones.B_Bone = (HandJoint)EditorGUILayout.EnumPopup("Bone2", bones.B_Bone);
                        bones.distance = EditorGUILayout.FloatField("Distance", bones.distance);
                        bones.thresholdWidth = EditorGUILayout.FloatField("Width", bones.thresholdWidth);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space(5);
            }
            EditorGUILayout.Space(5);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
            serializedObject.FindProperty("bonesHoldDuration").floatValue = EditorGUILayout.FloatField("Hold Duration", Mathf.Max(0, serializedObject.FindProperty("bonesHoldDuration").floatValue));
        }

        private void FingerConfig(Finger finger)
        {
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.Space(5);
            using (new GUILayout.VerticalScope())
            {
                EditorGUILayout.Space(5);
                FlexionConfig(finger, finger.fingerConfigs.flexionConfigs);
                CurlConfig(finger, finger.fingerConfigs.curlConfigs);
                AbductionConfig(finger, finger.fingerConfigs.abductionConfigs);
                EditorGUILayout.Space(5);
            }
            EditorGUILayout.Space(5);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
        }

        private void FlexionConfig(Finger finger, RangeConfigs flexionConfigs)
        {
            finger.flexion = (Flexion)EditorGUILayout.EnumPopup("Flexion", finger.flexion);
            Vector2 defaultVal = new Vector2();
            switch (finger.flexion)
            {
                case Flexion.Any:
                    return;
                case Flexion.Open:
                    defaultVal = GetDefaultShapeVal(finger.handFinger, ShapeType.flexion, true);
                    flexionConfigs.min = defaultVal.x;
                    flexionConfigs.max = defaultVal.y;
                    break;
                case Flexion.Close:
                    defaultVal = GetDefaultShapeVal(finger.handFinger, ShapeType.flexion, false);
                    flexionConfigs.min = defaultVal.x;
                    flexionConfigs.max = defaultVal.y;
                    break;
                case Flexion.Custom:
                    EditorGUILayout.MinMaxSlider("Custom Range",
                        ref flexionConfigs.min,
                        ref flexionConfigs.max,
                        ShapesRecognizer.k_FlexionMin,
                        ShapesRecognizer.k_FlexionMax);
                    break;
                default:
                    break;
            }
            flexionConfigs.width = EditorGUILayout.Slider("Width", flexionConfigs.width, 0,
                ShapesRecognizer.k_FlexionMax - ShapesRecognizer.k_FlexionMin);
            EditorGUILayout.LabelField(new GUIContent("Flexion Range"),
                new GUIContent($"[{flexionConfigs.min+" - "+flexionConfigs.width}, {flexionConfigs.max+" + "+flexionConfigs.width}]"));
        }

        private void CurlConfig(Finger finger, RangeConfigs curlConfigs)
        {
            finger.curl = (Curl)EditorGUILayout.EnumPopup("Curl", finger.curl);
            Vector2 defaultVal = new Vector2();
            switch (finger.curl)
            {
                case Curl.Any:
                    return;
                case Curl.Open:
                    defaultVal = GetDefaultShapeVal(finger.handFinger, ShapeType.curl, true);
                    curlConfigs.min = defaultVal.x;
                    curlConfigs.max = defaultVal.y;
                    break;
                case Curl.Close:
                    defaultVal = GetDefaultShapeVal(finger.handFinger, ShapeType.curl, false);
                    curlConfigs.min = defaultVal.x;
                    curlConfigs.max = defaultVal.y;
                    break;
                case Curl.Custom:
                    EditorGUILayout.MinMaxSlider("Custom Range",
                        ref curlConfigs.min,
                        ref curlConfigs.max,
                        finger.handFinger == HandFinger.Thumb ? ShapesRecognizer.k_CurlThumbMin : ShapesRecognizer.k_CurlMin,
                        finger.handFinger == HandFinger.Thumb ? ShapesRecognizer.k_CurlThumbMax : ShapesRecognizer.k_CurlMax);
                    break;
                default:
                    break;
            }
            curlConfigs.width = EditorGUILayout.Slider("Width", curlConfigs.width, 0,
                 ShapesRecognizer.k_CurlMax- ShapesRecognizer.k_CurlMin);
            EditorGUILayout.LabelField(new GUIContent("Curl Range"),
                new GUIContent($"[{curlConfigs.min+" - "+curlConfigs.width}, {curlConfigs.max+" + "+curlConfigs.width}]"));
        }

        private void AbductionConfig(Finger finger, RangeConfigsAbduction abductionConfigs)
        {
            if (finger.handFinger == HandFinger.Pinky) return;

            finger.abduction = (Abduction)EditorGUILayout.EnumPopup("Abduction", finger.abduction);
            Vector2 defaultVal = new Vector2();
            defaultVal = GetDefaultShapeVal(finger.handFinger, ShapeType.abduction);
            abductionConfigs.mid = defaultVal.x;
            switch (finger.abduction)
            {
                case Abduction.Any:
                    return;
                case Abduction.Open:
                    break;
                case Abduction.Close:
                    break;
                default:
                    break;
            }
            abductionConfigs.width = EditorGUILayout.Slider("Width", abductionConfigs.width, 0,
                ShapesRecognizer.k_AbductionMax - ShapesRecognizer.k_AbductionMin);
            EditorGUILayout.LabelField(new GUIContent("Abduction Range"),
                new GUIContent($"[{abductionConfigs.mid+" ± "+abductionConfigs.width+"/2"}]"));
        }

        private Vector2 GetDefaultShapeVal(HandFinger finger, ShapeType shapeType, bool isOpen = true)
        {
            Vector2 val = new Vector2();
            switch (shapeType)
            {
                case ShapeType.flexion:
                    val.x = finger == HandFinger.Thumb ? (isOpen ? ShapesRecognizer.k_FlexionThumbOpenMin : ShapesRecognizer.k_FlexionThumbCloseMin) :
                        (isOpen ? ShapesRecognizer.k_FlexionOpenMin : ShapesRecognizer.k_FlexionCloseMin);
                    val.y = finger == HandFinger.Thumb ? (isOpen ? ShapesRecognizer.k_FlexionThumbOpenMax : ShapesRecognizer.k_FlexionThumbCloseMax) :
                        (isOpen ? ShapesRecognizer.k_FlexionOpenMax : ShapesRecognizer.k_FlexionCloseMax);
                    break;
                case ShapeType.curl:
                    val.x = finger == HandFinger.Thumb ? (isOpen ? ShapesRecognizer.k_CurlThumbOpenMin : ShapesRecognizer.k_CurlThumbCloseMin) :
                        (isOpen ? ShapesRecognizer.k_CurlOpenMin : ShapesRecognizer.k_CurlCloseMin);
                    val.y = finger == HandFinger.Thumb ? (isOpen ? ShapesRecognizer.k_CurlThumbOpenMax : ShapesRecognizer.k_CurlThumbCloseMax) :
                        (isOpen ? ShapesRecognizer.k_CurlOpenMax : ShapesRecognizer.k_CurlCloseMax);
                    break;
                case ShapeType.abduction:
                    val.x = finger == HandFinger.Thumb ? ShapesRecognizer.k_AbductionThumbMid : ShapesRecognizer.k_AbductionMid;
                    val.y = finger == HandFinger.Thumb ? ShapesRecognizer.k_AbductionThumbWidth : ShapesRecognizer.k_AbductionWidth;
                    break;
            }
            return val;
        }
    }

    [CustomPropertyDrawer(typeof(DisplayOnly))]
    public class DisplayOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}