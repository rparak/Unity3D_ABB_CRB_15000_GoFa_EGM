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

    // Other variables
    public double[] C_Orientation_tmp = new double[3];
    public static readonly double[] C_Orientation_Offset = new double[3] { 180.0, 0.0, 180.0};

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
        // Read IP Address of the robot
        ABB_EGM_Control.ip_address = ip_address_txt.text;

        // ------------------------ Connection Information ------------------------//
        // If the button (connect/disconnect) is pressed, change the color and text
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
        ABB_EGM_Control.C_Position[0] = Math.Round(C_Position[0].value, 0);
        ABB_EGM_Control.C_Position[1] = Math.Round(C_Position[1].value, 0);
        ABB_EGM_Control.C_Position[2] = Math.Round(C_Position[2].value, 0);
        // Rotation {Euler Angles} -> RX..RZ
        C_Orientation_tmp[0] = Math.Round(C_Orientation[0].value, 0);
        ABB_EGM_Control.C_Orientation[0] = C_Orientation_tmp[0] > 0.0 ? (-1) * (C_Orientation_Offset[0] - C_Orientation_tmp[0]) : C_Orientation_Offset[0] + C_Orientation_tmp[0];
        C_Orientation_tmp[1] = Math.Round(C_Orientation[1].value, 0);
        ABB_EGM_Control.C_Orientation[1] = C_Orientation_tmp[1] > 0.0 ? (-1) * (C_Orientation_Offset[1] - C_Orientation_tmp[1]) : C_Orientation_Offset[1] + C_Orientation_tmp[1];
        C_Orientation_tmp[2] = Math.Round(C_Orientation[2].value, 0);
        ABB_EGM_Control.C_Orientation[2] = C_Orientation_tmp[2] > 0.0 ? (-1) * (C_Orientation_Offset[2] - C_Orientation_tmp[2]) : C_Orientation_Offset[2] + C_Orientation_tmp[2];

        // Cyclic read-write parameters to text info
        // Position {Cartesian} -> X..Z
        position_x_txt.text = ABB_EGM_Control.C_Position[0].ToString();
        position_y_txt.text = ABB_EGM_Control.C_Position[1].ToString();
        position_z_txt.text = ABB_EGM_Control.C_Position[2].ToString();
        // Rotation {Euler Angles} -> RX..RZ
        rotation_x_txt.text = ABB_EGM_Control.C_Orientation[0].ToString();
        rotation_y_txt.text = ABB_EGM_Control.C_Orientation[1].ToString();
        rotation_z_txt.text = ABB_EGM_Control.C_Orientation[2].ToString();
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
}
