using UnityEngine;
using UnityEngine.UI;
using YVR.Interaction;

public class HandPoseTest : MonoBehaviour
{
    public string poseName;
    public YVRHandPose pose;

    public Text poseResult;
    // Start is called before the first frame update
    void Start()
    {
        pose.handPoseStart.AddListener(OnHandPoseStart);
        pose.handPoseUpdate.AddListener(OnHandPoseUpdate);
        pose.handPoseEnd.AddListener(OnHandPoseEnd);
    }

    public void OnHandPoseStart()
    {
        Debug.Log("OnHandPoseStart");
        ClearText();
        poseResult.text =  poseName+": Start" + "\n";
    }

    public void OnHandPoseUpdate(float value)
    {
        Debug.Log($"OnHandPoseUpdate value:{value}");
        ClearText();
        poseResult.text = $"{poseName}: Update value:{value}" + "\n";
    }

    public void OnHandPoseEnd()
    {
        Debug.Log("OnHandPoseEnd");
        ClearText();
        poseResult.text = $"{poseName}: End" + "\n";
    }

    private void ClearText()
    {
        if (poseResult.text.Length>=300)
        {
            poseResult.text = "";
        }
    }
}
