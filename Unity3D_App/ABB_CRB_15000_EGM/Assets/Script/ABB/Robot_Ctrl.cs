// System 
using System;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
// Unity
using UnityEngine;
using Debug = UnityEngine.Debug;
// ABB EGM Lib.
using abb.egm;

public class Robot_Ctrl : MonoBehaviour
{
    public static class GlobalVariables_Main_Control
    {
        public static bool Connect, Disconnect;
        public static bool Is_Connected, Is_Disconnected;
        // Cartesian Space Limit:
        //  Position {X, Y, Z} (mm)
        public static float[,] C_Position_Limit = new float[3, 2];
        //  Orientation (Euler Angles) {X, Y, Z} (°)
        public static float[,] C_Orientation_Limit = new float[3, 2];
    }

    public static class ABB_EGM_Control
    {
        // IP Port Number and IP Address
        public static string ip_address;
        // IP Port Number
        public static int port_number = 6511;
        // Joint Space:
        //  Orientation {J1 .. J6} (°)
        public static double[] J_Orientation = new double[6];
        // Cartesian Space:
        //  Position {X, Y, Z} (mm)
        public static double[] C_Position = new double[3];
        //  Orientation (Euler Angles) {X, Y, Z} (°)
        public static double[] C_Orientation = new double[3];
        // Class thread information (is alive or not)
        public static bool is_alive = false;
    }

    // Class Control Robot {ABB Externally Guided Motion - EGM}
    private Egm_Control ABB_EGM_Control_Cls = new Egm_Control();

    // Other variables
    private int main_state_ctrl_egm = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Cartesian Space Limit:
        //  Set Min/Max limits: Position in metres
        GlobalVariables_Main_Control.C_Position_Limit[0, 0] = 300.0f; GlobalVariables_Main_Control.C_Position_Limit[0, 1] = 700.0f;
        GlobalVariables_Main_Control.C_Position_Limit[1, 0] = -200.0f; GlobalVariables_Main_Control.C_Position_Limit[1, 1] = 200.0f;
        GlobalVariables_Main_Control.C_Position_Limit[2, 0] = 200.0f; GlobalVariables_Main_Control.C_Position_Limit[2, 1] = 600.0f;
        //  Set Min/Max limits: Orientation in degrees
        GlobalVariables_Main_Control.C_Orientation_Limit[0, 0] = -30.0f; GlobalVariables_Main_Control.C_Orientation_Limit[0, 1] = 30.0f;
        GlobalVariables_Main_Control.C_Orientation_Limit[1, 0] = -30.0f; GlobalVariables_Main_Control.C_Orientation_Limit[1, 1] = 30.0f;
        GlobalVariables_Main_Control.C_Orientation_Limit[2, 0] = -30.0f; GlobalVariables_Main_Control.C_Orientation_Limit[2, 1] = 30.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (main_state_ctrl_egm)
        {
            case 0:
                {
                    GlobalVariables_Main_Control.Is_Connected = false;
                    GlobalVariables_Main_Control.Is_Disconnected = true;

                    // ------------------------ Wait State {Disconnect State} ------------------------//
                    if (GlobalVariables_Main_Control.Connect == true)
                    {
                        //Start Stream {ABB Externally Guided Motion - EGM}
                        ABB_EGM_Control_Cls.Start();

                        // go to connect state
                        main_state_ctrl_egm = 1;
                    }
                }
                break;
            case 1:
                {
                    GlobalVariables_Main_Control.Is_Connected = true;
                    GlobalVariables_Main_Control.Is_Disconnected = false;

                    // ------------------------ Data Processing State {Connect State} ------------------------//
                    if (GlobalVariables_Main_Control.Disconnect == true)
                    {
                        // Stop threading block {ABB Externally Guided Motion - EGM}
                        if (ABB_EGM_Control.is_alive == true)
                        {
                            ABB_EGM_Control_Cls.Stop();
                        }

                        if (ABB_EGM_Control.is_alive == false)
                        {
                            // go to initialization state {wait state -> disconnect state}
                            main_state_ctrl_egm = 0;
                        }
                    }
                }
                break;
        }
    }

    void OnApplicationQuit()
    {
        try
        {
            // Destroy Control Robot {ABB Externally Guided Motion - EGM}
            ABB_EGM_Control_Cls.Destroy();

            Destroy(this);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    class Egm_Control
    {
        private Thread sensor_thread = null;
        private UdpClient udp_client = null;
        private bool exit_thread = false;
        private uint sequence_number = 0;
        public void Egm_Control_Thread()
        {
            // Create an udp server and listen on any address and the port
            // {ABB Robot Port is set from the RobotStudio ABB}
            udp_client = new UdpClient(ABB_EGM_Control.port_number);

            // IPAddress.Parse(ABB_EGM_Control.ip_address)
            // IPAddress.Any
            var end_point = new IPEndPoint(IPAddress.Parse(ABB_EGM_Control.ip_address), ABB_EGM_Control.port_number);

            while (exit_thread == false)
            {
                // Get the data from the robot
                var data = udp_client.Receive(ref end_point);

                if (data != null)
                {
                    // Initialization ABB Robot {EGM READ data (position, rotation)}
                    EgmRobot robot_msg = EgmRobot.CreateBuilder().MergeFrom(data).Build();
                    // Read robot joint orientation
                    ABB_EGM_Control.J_Orientation[0] = robot_msg.FeedBack.Joints.GetJoints(0);
                    ABB_EGM_Control.J_Orientation[1] = robot_msg.FeedBack.Joints.GetJoints(1);
                    ABB_EGM_Control.J_Orientation[2] = robot_msg.FeedBack.Joints.GetJoints(2);
                    ABB_EGM_Control.J_Orientation[3] = robot_msg.FeedBack.Joints.GetJoints(3);
                    ABB_EGM_Control.J_Orientation[4] = robot_msg.FeedBack.Joints.GetJoints(4);
                    ABB_EGM_Control.J_Orientation[5] = robot_msg.FeedBack.Joints.GetJoints(5);

                    // Create a new EGM sensor message
                    EgmSensor.Builder egm_sensor = EgmSensor.CreateBuilder();

                    // Create a sensor message to send to the robot
                    EMG_Sensor_Message(egm_sensor);

                    using (MemoryStream memory_stream = new MemoryStream())
                    {
                        // Sensor Message
                        EgmSensor sensor_message = egm_sensor.BuildPartial();
                        sensor_message.WriteTo(memory_stream);

                        // Send message to the ABB ROBOT {UDP}
                        int bytes_sent = udp_client.Send(memory_stream.ToArray(), (int)memory_stream.Length, end_point);
                        // Check sent data
                        if (bytes_sent < 0)
                        {
                            Debug.Log("Error send to robot");
                        }
                    }
                }
            }
        }
        void EMG_Sensor_Message(EgmSensor.Builder egm_s)
        {
            // create a header
            EgmHeader.Builder egm_hdr = new EgmHeader.Builder();
            /*
             * SetTm: Timestamp in milliseconds (can be used for monitoring delays)
             * SetMtype: Sent by sensor, MSGTYPE_DATA if sent from robot controller
             */
            egm_hdr.SetSeqno(sequence_number++).SetTm((uint)DateTime.Now.Ticks)
                .SetMtype(EgmHeader.Types.MessageType.MSGTYPE_CORRECTION);

            egm_s.SetHeader(egm_hdr);

            // Create EGM Sensor Data
            EgmPlanned.Builder planned = new EgmPlanned.Builder();
            EgmPose.Builder cartesian = new EgmPose.Builder();
            EgmEuler.Builder orientation = new EgmEuler.Builder();
            EgmCartesian.Builder position = new EgmCartesian.Builder();

            // Set data {Position / Orientation}
            position.SetX(ABB_EGM_Control.C_Position[0])
                    .SetY(ABB_EGM_Control.C_Position[1])
                    .SetZ(ABB_EGM_Control.C_Position[2]);
            orientation.SetX(ABB_EGM_Control.C_Orientation[0])
                       .SetY(ABB_EGM_Control.C_Orientation[1])
                       .SetZ(ABB_EGM_Control.C_Orientation[2]);

            // Set data {Cartesian}
            cartesian.SetPos(position).SetEuler(orientation);

            // Bind position object to planned
            planned.SetCartesian(cartesian);
            // Bind planned to sensor object
            egm_s.SetPlanned(planned);

            return;
        }
        public void Start()
        {
            exit_thread = false;
            // Start a thread and listen to incoming messages
            sensor_thread = new Thread(new ThreadStart(Egm_Control_Thread));
            sensor_thread.IsBackground = true;
            sensor_thread.Start();
            // Thread is active
            ABB_EGM_Control.is_alive = true;
        }
        public void Stop()
        {
            // Stop and exit thread
            exit_thread = true;
            sensor_thread.Abort();
            Thread.Sleep(100);
            ABB_EGM_Control.is_alive = sensor_thread.IsAlive;
            sensor_thread.Abort();
        }
        public void Destroy()
        {
            // Stop a thread (Robot Web Services communication)
            Stop();
            Thread.Sleep(100);
        }
    }
}
