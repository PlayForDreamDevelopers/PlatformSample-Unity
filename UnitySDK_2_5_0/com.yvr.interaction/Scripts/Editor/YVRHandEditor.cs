using UnityEditor;
using UnityEngine;
using YVR.Core;

namespace YVR.Interaction.Editor
{
    [CustomEditor(typeof(YVRHand))]
    public class YVRHandPoseEditors : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();

            YVRHand hand = (YVRHand)target;

            EditorGUILayout.LabelField("Hand Joints", EditorStyles.boldLabel);

            for (int i = 0; i < (int)HandJoint.JointMax; i++)
            {
                string jointName = ((HandJoint)i).ToString();
                hand.handJoints[i] = (Transform)EditorGUILayout.ObjectField(jointName, hand.handJoints[i], typeof(Transform), true);
            }
        }
    }
}