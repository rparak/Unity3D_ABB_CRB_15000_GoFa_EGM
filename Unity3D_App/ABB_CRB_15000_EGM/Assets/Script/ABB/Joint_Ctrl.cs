// System
using System;
// Unity 
using UnityEngine;
using Debug = UnityEngine.Debug;

using static Robot_Ctrl;
public class Joint_Ctrl : MonoBehaviour
{
    // Controlled joint index
    public int index;
    // Orientation of the joint before the start
    private Vector3 init_joint_orientation = new Vector3(0.0f, 0.0f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        init_joint_orientation = transform.localEulerAngles;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        try
        {
            transform.localEulerAngles = new Vector3(init_joint_orientation[0], 
                                                     init_joint_orientation[1], 
                                                     init_joint_orientation[2] + (float)((-1) * ABB_EGM_Control.J_Orientation[index]));
        }
        catch (Exception e)
        {
            Debug.Log("Exception:" + e);
        }
    }
    void OnApplicationQuit()
    {
        try
        {
            Destroy(this);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
