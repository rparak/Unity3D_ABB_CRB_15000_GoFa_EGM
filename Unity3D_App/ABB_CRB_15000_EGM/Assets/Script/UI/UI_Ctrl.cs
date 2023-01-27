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
File Name: UI_Ctrl.cs
****************************************************************************/

// System 
using System;
using System.Text;
// Unity
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;
// TM 
using TMPro;

using static Robot_Ctrl;

public class UI_Ctrl : MonoBehaviour
{
    // TMP_InputField 
    public TMP_InputField ip_address_txt;
    // Image
    public Image connection_info_img;
    // TextMeshProUGUI
    public TextMeshProUGUI position_x_txt, position_y_txt, position_z_txt;
    public TextMeshProUGUI rotation_x_txt, rotation_y_txt, rotation_z_txt;
    public TextMeshProUGUI connectionInfo_txt;
    // Slider
    public Slider[] C_Position = new Slider[3];
    public Slider[] C_Orientation = new Slider[3];
    // Toggle
    public Toggle Viewpoint_Visibility;
    // GameObject
    public GameObject Viewpoint_EE_0;
    public GameObject Viewpoint_EE_SCHUNK;
    public GameObject EE_SCHUNK;
    // Dropdown
    public TMP_Dropdown EE_Configuration;
    // Float
    //  Velocity of each robot control parameter: value is modified
    //  by the function (Mathf.SmoothDamp)
    private float[] C_Pos_Current_Vel = new float[3];
    private float[] C_Orient_Current_Vel = new float[3];

    // Other variables
    private double[] C_Orientation_tmp = new double[3];
    private static readonly double[] C_Orientation_Offset = new double[3] { 180.0, 0.0, 180.0};
    private static readonly float smooth_time = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        // Connection information {image} -> Connect/Disconnect
        connection_info_img.GetComponent<Image>().color = new Color32(255, 0, 48, 50);
        // Connection information {text} -> Connect/Disconnect
        connectionInfo_txt.text = "Disconnected";

        // Position {Cartesian} -> X..Z
        position_x_txt.text = "0.00";
        position_y_txt.text = "0.00";
        position_z_txt.text = "0.00";
        // Rotation {Euler Angles} -> RX..RZ
        rotation_x_txt.text = "0.00";
        rotation_y_txt.text = "0.00";
        rotation_z_txt.text = "0.00";

        // Robot IP Address
        ip_address_txt.text = "127.0.0.1";

        // Visibility of the robot's end effector viewpoint
        Viewpoint_Visibility.isOn = false;

        // End-Effector Configuration:
        //  0 - without end-effector
        //  1 - with predefined end-effector
        EE_Configuration.value = 0;

        // Slider:
        //  Set Min/Max limits: Position in metres
        C_Position[0].minValue = GlobalVariables_Main_Control.C_Position_Limit[0, 0]; C_Position[0].maxValue = GlobalVariables_Main_Control.C_Position_Limit[0, 1];
        C_Position[1].minValue = GlobalVariables_Main_Control.C_Position_Limit[1, 0]; C_Position[1].maxValue = GlobalVariables_Main_Control.C_Position_Limit[1, 1];
        C_Position[2].minValue = GlobalVariables_Main_Control.C_Position_Limit[2, 0]; C_Position[2].maxValue = GlobalVariables_Main_Control.C_Position_Limit[2, 1];
        //  Set Min/Max limits: Orientation in degrees
        C_Orientation[0].minValue = GlobalVariables_Main_Control.C_Orientation_Limit[0, 0]; C_Orientation[0].maxValue = GlobalVariables_Main_Control.C_Orientation_Limit[0, 1];
        C_Orientation[1].minValue = GlobalVariables_Main_Control.C_Orientation_Limit[1, 0]; C_Orientation[1].maxValue = GlobalVariables_Main_Control.C_Orientation_Limit[1, 1];
        C_Orientation[2].minValue = GlobalVariables_Main_Control.C_Orientation_Limit[2, 0]; C_Orientation[2].maxValue = GlobalVariables_Main_Control.C_Orientation_Limit[2, 1];
        //  Reset Values
        C_Position[0].value = (C_Position[0].minValue + C_Position[0].maxValue) / 2.0f; 
        C_Position[1].value = (C_Position[1].minValue + C_Position[1].maxValue) / 2.0f;
        C_Position[2].value = (C_Position[2].minValue + C_Position[2].maxValue) / 2.0f;
        C_Orientation[0].value = (C_Orientation[0].minValue + C_Orientation[0].maxValue) / 2.0f;
        C_Orientation[1].value = (C_Orientation[1].minValue + C_Orientation[1].maxValue) / 2.0f;
        C_Orientation[2].value = (C_Orientation[2].minValue + C_Orientation[2].maxValue) / 2.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Read Variables:
        //  IP Address of the robot
        ABB_EGM_Control.ip_address = ip_address_txt.text;
        ABB_TCP_Control.ip_address = ip_address_txt.text;
        //  End-Effector Configuration
        ABB_TCP_Control.EE_Config = EE_Configuration.value;

        // Connection Information
        //  If the button (connect/disconnect) is pressed, change the color and text
        if (GlobalVariables_Main_Control.Is_Connected == true)
        {
            // green color
            connection_info_img.GetComponent<Image>().color = new Color32(135, 255, 0, 50);
            connectionInfo_txt.text = "Connected";
        }
        else if (GlobalVariables_Main_Control.Is_Disconnected == true)
        {
            // red color
            connection_info_img.GetComponent<Image>().color = new Color32(255, 0, 48, 50);
            connectionInfo_txt.text = "Disconnected";
        }

        // Cyclic read-write parameters to the robot
        // Position {Cartesian} -> X..Z
        ABB_EGM_Control.C_Position[0] = Mathf.SmoothDamp((float)ABB_EGM_Control.C_Position[0], C_Position[0].value, ref C_Pos_Current_Vel[0], smooth_time);
        ABB_EGM_Control.C_Position[1] = Mathf.SmoothDamp((float)ABB_EGM_Control.C_Position[1], C_Position[1].value, ref C_Pos_Current_Vel[1], smooth_time);
        ABB_EGM_Control.C_Position[2] = Mathf.SmoothDamp((float)ABB_EGM_Control.C_Position[2], C_Position[2].value, ref C_Pos_Current_Vel[2], smooth_time);

        // Rotation {Euler Angles} -> RX..RZ
        C_Orientation_tmp[0] = Mathf.SmoothDamp((float)C_Orientation_tmp[0], C_Orientation[0].value, ref C_Orient_Current_Vel[0], smooth_time);
        ABB_EGM_Control.C_Orientation[0] = C_Orientation_tmp[0] > 0.0 ? (-1) * (C_Orientation_Offset[0] - C_Orientation_tmp[0]) : C_Orientation_Offset[0] + C_Orientation_tmp[0];
        C_Orientation_tmp[1] = Mathf.SmoothDamp((float)C_Orientation_tmp[1], C_Orientation[1].value, ref C_Orient_Current_Vel[1], smooth_time);
        ABB_EGM_Control.C_Orientation[1] = C_Orientation_tmp[1] > 0.0 ? (-1) * (C_Orientation_Offset[1] - C_Orientation_tmp[1]) : C_Orientation_Offset[1] + C_Orientation_tmp[1];
        C_Orientation_tmp[2] = Mathf.SmoothDamp((float)C_Orientation_tmp[2], C_Orientation[2].value, ref C_Orient_Current_Vel[2], smooth_time);
        ABB_EGM_Control.C_Orientation[2] = C_Orientation_tmp[2] > 0.0 ? (-1) * (C_Orientation_Offset[2] - C_Orientation_tmp[2]) : C_Orientation_Offset[2] + C_Orientation_tmp[2];

        // Cyclic read-write parameters to text info
        // Position {Cartesian} -> X..Z
        position_x_txt.text = Math.Round(ABB_EGM_Control.C_Position[0], 0).ToString();
        position_y_txt.text = Math.Round(ABB_EGM_Control.C_Position[1], 0).ToString();
        position_z_txt.text = Math.Round(ABB_EGM_Control.C_Position[2], 0).ToString();
        // Rotation {Euler Angles} -> RX..RZ
        rotation_x_txt.text = Math.Round(ABB_EGM_Control.C_Orientation[0], 0).ToString();
        rotation_y_txt.text = Math.Round(ABB_EGM_Control.C_Orientation[1], 0).ToString();
        rotation_z_txt.text = Math.Round(ABB_EGM_Control.C_Orientation[2], 0).ToString();


        // Configuration of the robot end-effector visualization in the scene
        switch (ABB_TCP_Control.EE_Config)
        {
            case 0:
                {
                    // End-Effector Configuration: 
                    //  0 - without end-effector
                    EE_SCHUNK.SetActive(false);

                    if (Viewpoint_Visibility.isOn == true)
                        Viewpoint_EE_0.SetActive(true);
                    else
                        Viewpoint_EE_0.SetActive(false);

                    Viewpoint_EE_SCHUNK.SetActive(false);
                }
                break;

            case 1:
                {
                    // End-Effector Configuration:
                    //  1 - with predefined end-effector
                    EE_SCHUNK.SetActive(true);

                    if (Viewpoint_Visibility.isOn == true)
                        Viewpoint_EE_SCHUNK.SetActive(true);
                    else
                        Viewpoint_EE_SCHUNK.SetActive(false);

                    Viewpoint_EE_0.SetActive(false);
                }
                break;
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

    public void TaskOnClick_ConnectBTN()
    {
        GlobalVariables_Main_Control.Connect = true;
        GlobalVariables_Main_Control.Disconnect = false;
    }

    public void TaskOnClick_DisconnectBTN()
    {
        GlobalVariables_Main_Control.Connect = false;
        GlobalVariables_Main_Control.Disconnect = true;
    }

    public void TaskOnClick_EE_Open_BTN()
    {
        ABB_TCP_Control.EE_Open = 1;
        ABB_TCP_Control.EE_Close = 0;
    }

    public void TaskOnClick_EE_Close_BTN()
    {
        ABB_TCP_Control.EE_Open = 0;
        ABB_TCP_Control.EE_Close = 1;
    }
}
