# Unity3D Industrial Robotics: ABB CRB 15000 EGM (Externally Guided Motion)

## Requirements:

**Software:**
```bash
ABB RobotStudio, Blender, Unity3D 2021.3.16f1 (LTS), Visual Studio 2019/2022
```

**Supported on the following operating systems:**
```bash
Universal Windows Platform, Android
```

| Software/Package      | Link                                                                                  |
| --------------------- | ------------------------------------------------------------------------------------- |
| Blender               | https://www.blender.org/download/                                                     |
| Unity3D               | https://unity3d.com/get-unity/download/archive                                        |
| ABB RobotStudio       | https://new.abb.com/products/robotics/robotstudio/downloads                           |
| Visual Studio         | https://visualstudio.microsoft.com/downloads/                                         |
| NuGetForUnity         | https://github.com/GlitchEnzo/NuGetForUnity                                           |

## Project Description:

The project focuses on a simple demonstration of client-server communication via User Datagram Protocol (UDP), which is implemented in Unity3D. More precisely, it is the control of the robot by EGM (Externally Guided Motion). The main idea is to control the Tool Center Point (TCP) of the robot (Translation, Rotation - Quaternion / Euler Angles) and collect data from the ABB into the Unity3D simulation to visualize the robot motion. An additional feature of the project is the control of the SCHUNK end-effector via TCP/IP, which is also implemented in Unity3D.

**WARNING: RobotWare version 7.6.1 or lower must be used.**

The 3D parts of the robot were downloaded from the official ABB website here: [ABB Library](https://library.abb.com/)

The 3D model of the end-effector were downloaded from the official SCHUNK website here: [SCHUNK Co-act EGP-C 40-N-N-GoFa](https://schunk.com/vn/en/gripping-systems/parallel-gripper/co-act-egp-c/co-act-egp-c-40-n-n-gofa/p/000000000001468548)

This solution can be used to control a real robot or to simulate one. The Unity3D Digital-Twin application has been tested on the ABB CRB GoFa robotic arm, both on real hardware and in simulation.

The application can be installed on a mobile phone, tablet or computer, but for communication with the robot it is necessary to be in the same network. The application uses performance optimization using multi-threaded programming. The High Definition Render Pipeline (HDRP) technology has been used for better graphics.

The project was realized at the Institute of Automation and Computer Science, Brno University of Technology, Faculty of Mechanical Engineering (NETME Centre - Cybernetics and Robotics Division) in cooperation with Novota Art.

**IP Address Settings:**

|          | ABB RobotStudio (CRB 15000 GoFa) | PC |
| :------: | :-----------: | :-----------: |
| Simulation Control  | 127.0.0.1 | 127.0.0.1 |
| Real - World Control | 192.168.125.1  | 192.168.125.22 |

|          | PORT |
| :------: | :-----------: |
| TCP/IP | 6061 |
| UDPUC | 6511  |

**WARNING: To control the robot in the real world, it is necessary to disable the firewall.**

**Notes:**

EGM (Externally Guided Motion) is an interface for ABB robots that allows smoothless control of the robotic arm from an external application (in our case it is a Unity3D developmentpPlatform ). The EGM can be used to transfer positions to the robot controller in either Joint/ Cartesian space. In our case it is the control of the robot using Cartesian coordinates.

```bash
The file "egm.proto" can be found in the installation folder of RobotWare. For example on Windows with RobotWare 7.6.1:
C:\Users\<user_name>\AppData\Local\ABB Industrial IT\Robotics IT\RobotWare\RobotControl_7.6.1\utility\Template\EGM
```

The Protobuf code generator can be used to generate code from a *.proto file into individual programming languages.

Link: [Protobuf Code Generator and Parser](https://protogen.marcgravell.com)

<p align="center">
<img src="https://github.com/rparak/Unity3D_ABB_CRB_15000_EGM/blob/main/images/ABB_RS_Unity3D.png" width="800" height="500">
</p>

**Unpacking a station (*.rspag):**
1. On the File tab, click Open and then browse to the folder and select the Pack&Go file, the Unpack & Work wizard opens.
2. In the Welcome to the Unpack & Work Wizard page, click Next.
3. In the Select package page, click Browse and then select the Pack & Go file to unpack and the Target folder. Click Next.
4. In the Library handling page select the target library. Two options are available, Load files from local PC or Load files from Pack & Go. Click the option to select the location for loading the required files, and click Next.
5. In the Virtual Controller page, select the RobotWare version and then click Locations to access the RobotWare Add-in and Media pool folders. Optionally, select the check box to automatically restore backup. Click Next.
6. In the Ready to unpack page, review the information and then click Finish.

<p align="center">
  <img src="https://github.com/rparak/Unity3D_ABB_CRB_15000_EGM/blob/main/images/ABB_RobotStudio_1.png" width="800" height="500">
</p>

**ABB RobotStudio Station Logic : Control of the potential end-effector (SCHUNK in our case)**

<p align="center">
  <img src="https://github.com/rparak/Unity3D_ABB_CRB_15000_EGM/blob/main/images/ABB_RobotStudio_2.png" width="800" height="200">
</p>

## Project Hierarchy:

```bash
[/15000-500105-CR-Gofa_BACKUP_2023-02-03/]
Description:
  Backup from real robot CRB 15000 (GoFa).
  
[/CAD/]
Description:
  3D models of the individual parts of the robot and the end-effector.

[/Blender/]
Description:
  The project includes a 3D model of the robot with the origin (centre of rotation) of each joint. The parameters of the robot joints can easily be used to obtain the DH (Denavit–Hartenberg) parameters for the calculation of FK (Forward Kinematics).
  
[/ABB_RS/]
Description:
  The main ABB RobotStudio project for robot control via EGM (with and without end effector control via TCP/IP). The folder also contains the RS project for verification of the robot workspace.
  
[/Unity3D_App/ABB_CRB_15000_EGM]
Description:
  The main Unity3D project with additional dependencies.
  
[/PROTO/]
Description:
  The egm.proto -file.
  
[../Unity3D_App/ABB_CRB_15000_EGM/Assets/Script/ABB/Egm.cs/]
Description:
  Autogenerated code from the egm.proto -file. 
```

<p align="center">
  <img src="https://github.com/rparak/Unity3D_ABB_CRB_15000_EGM/blob/main/images/Unity3D_Scene.png" width="800" height="500">
  <img src="https://github.com/rparak/Unity3D_ABB_CRB_15000_EGM/blob/main/images/Unity3D_UI.png" width="800" height="500">
</p>

## Digital-Twin Application:

<p align="center">
  <img src="https://github.com/rparak/Unity3D_ABB_CRB_15000_EGM/blob/main/images/Unity3D_App_GoFa_1.png" width="800" height="500">
  <img src="https://github.com/rparak/Unity3D_ABB_CRB_15000_EGM/blob/main/images/Unity3D_App_GoFa_2.png" width="800" height="500">
  <img src="https://github.com/rparak/Unity3D_ABB_CRB_15000_EGM/blob/main/images/Unity3D_App_GoFa_3.png" width="800" height="500">
  <img src="https://github.com/rparak/Unity3D_ABB_CRB_15000_EGM/blob/main/images/Unity3D_App_GoFa_4.png" width="800" height="500">
  <img src="https://github.com/rparak/Unity3D_ABB_CRB_15000_EGM/blob/main/images/Unity3D_App_GoFa_5.png" width="800" height="500">
</p>

**Real-World Test:**

<p align="center">
  <img src="https://github.com/rparak/Unity3D_ABB_CRB_15000_EGM/blob/main/images/GoFa_1.png" width="300" height="450">
  <img src="https://github.com/rparak/Unity3D_ABB_CRB_15000_EGM/blob/main/images/GoFa_2.png" width="300" height="450">
</p>

<p align="center">
  <img src="https://github.com/rparak/Unity3D_ABB_CRB_15000_EGM/blob/main/images/GoFa_TV.png" width="700" height="500">
</p>

## Result:

Youtube: https://www.youtube.com/watch?v=hkEybI5IzpE

## Contact Info:
Roman.Parak@outlook.com

## Citation (BibTex)
```bash
@misc{RomanParak_Unity3D,
  author = {Roman Parak},
  title = {A digital-twins in the field of industrial robotics integrated into the unity3d development platform},
  year = {2020-2023},
  publisher = {GitHub},
  journal = {GitHub repository},
  howpublished = {\url{https://github.com/rparak/Unity3D_Robotics_Overview}}
}
```
## License
[MIT](https://choosealicense.com/licenses/mit/)
