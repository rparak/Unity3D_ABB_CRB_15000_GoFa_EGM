/****************************************************************************
MIT License
Copyright(c) 2023 Roman Parak
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*****************************************************************************
Author   : Roman Parak
Email    : Roman.Parak @outlook.com
Github   : https://github.com/rparak
File Name: Joint_Ctrl.cs
****************************************************************************/

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
