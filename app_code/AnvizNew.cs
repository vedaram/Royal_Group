using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;


   
    public class AnvizNew
    {
        // Consts
        public const long CKT_ERROR_INVPARAM = -1;
        public const long CKT_ERROR_NETDAEMONREADY = -1;
        public const long CKT_ERROR_CHECKSUMERR = -2;
        public const long CKT_ERROR_MEMORYFULL = -1;
        public const long CKT_ERROR_INVFILENAME = -3;
        public const long CKT_ERROR_FILECANNOTOPEN = -4;
        public const long CKT_ERROR_FILECONTENTBAD = -5;
        public const long CKT_ERROR_FILECANNOTCREATED = -2;
        public const long CKT_ERROR_NOTHISPERSON = -1;

        public const long CKT_RESULT_OK = 1;
        public const long CKT_RESULT_ADDOK = 1;
        public const long CKT_RESULT_HASMORECONTENT = 2;

        // Types
        [StructLayout(LayoutKind.Sequential, Size = 26, CharSet = CharSet.Ansi), Serializable]
        public struct NETINFO
        {
            public int ID;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] IP;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Mask;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Gateway;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] ServerIP;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] MAC;
        }

        [StructLayout(LayoutKind.Sequential, Size = 164), Serializable]
        public struct  CKT_KQState
        {
            public int num;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 160)]
            public byte[] kqmsg;   //0 To 9, 0 To 15
        }

        [StructLayout(LayoutKind.Sequential, Size = 16), Serializable]
        public struct  SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek ;
            public short wDay ;
            public short wHour;
            public short wMinute ;
            public short wSecond ;
            public short wMilliseconds;
        }

        [StructLayout(LayoutKind.Sequential, Size = 18), Serializable]
        public struct DATETIMEINFO
        {
            public int ID;
            public ushort Year;
            public byte Month;
            public byte Day;
            public byte Hour;
            public byte Minute;
            public byte Second;
        }
    
        [StructLayout(LayoutKind.Sequential, Size = 48, CharSet = CharSet.Ansi), Serializable]
        public struct PERSONINFO
        {
            public int PersonID;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Password;
            public int CardNo;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[] Name;
            public int Dept;
            public int Group;
            //public int Auth;
            public int KQOption;
            public int FPMark;
            public int Other;
        }

        [StructLayout(LayoutKind.Sequential, Size = 40, CharSet = CharSet.Ansi), Serializable]
        public struct CLOCKINGRECORD
        {
            public int ID;
            public int PersonID;
            public int Stat;
            public int BackupCode;
            public int WorkType;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] Time;
        }

        

        [StructLayout(LayoutKind.Sequential, Size = 60), Serializable]
        public struct DEVICEINFO
        {
            public int ID;
            public int MajorVersion;
            public int MinorVersion;
            public int SpeakerVolume;
            public int Parameter;
            public int DefaultAuth;
            public int FixWGHead;
            public int WGOption;
            public int AutoUpdateAllow;
            public int KQRepeatTime;
            public int RealTimeAllow;
            public int RingAllow;
            public int LockDelayTime;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] AdminPassword;
        }
  
        [StructLayout(LayoutKind.Sequential, Size = 12, CharSet = CharSet.Ansi), Serializable]
        public struct RINGTIME
        {
            public int hour;
            public int minute;
            public int week;
        }

        [StructLayout(LayoutKind.Sequential, Size = 4), Serializable]
        public struct TIMESECT
        {
            public byte bHour;
            public byte bMinute;
            public byte eHour;
            public byte eMinute;
        }

        [StructLayout(LayoutKind.Sequential, Size = 76), Serializable]
        public struct CKT_MessageInfo
        {
            public int PersonID;
            public int sYear;
            public int sMon;
            public int sDay;
            public int eYear;
            public int eMon;
            public int eDay;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 48)]
            public byte[] msg;
        }

        [StructLayout(LayoutKind.Sequential, Size = 28), Serializable]
        public struct CKT_MessageHead
        {
            public int PersonID;
            public int sYear;
            public int sMon;
            public int sDay;
            public int eYear;
            public int eMon;
            public int eDay;
        }

        // Routines
        [DllImport("tc400.dll")]
        public static extern int CKT_FreeMemory(ref int memory);

        [DllImport("tc400.dll")]
        public static extern int CKT_RegisterSno(int Sno, int ComPort);
               
        [DllImport("tc400.dll")]
        public static extern int CKT_RegisterNet(int Sno, String Addr);

        [DllImport("tc400.dll")]
        public static extern int CKT_RegisterUSB(int Sno, int Index);
        
        [DllImport("tc400.dll")]
        public static extern void CKT_UnregisterSnoNet(int Sno);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_NetDaemon();
        
        [DllImport("tc400.dll")]
        public static extern int CKT_ComDaemon();
        
        [DllImport("tc400.dll")]
        public static extern void CKT_Disconnect();
        
        [DllImport("tc400.dll")]
        public static extern int CKT_ReportConnections(ref int ppSno);

        [DllImport("tc400.dll")]
        public static extern int CKT_GetDeviceNetInfo(int Sno, ref NETINFO pNetInfo);
               
        [DllImport("tc400.dll")]
        public static extern int CKT_SetDeviceIPAddr(int Sno, byte[] IP);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_SetDeviceMask(int Sno, byte[] Mask);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_SetDeviceGateway(int Sno, byte[] Gate);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_SetDeviceServerIPAddr(int Sno, byte[] Svr);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_SetDeviceMAC(int Sno, byte[] MAC);

        [DllImport("tc400.dll")]
        public static extern int CKT_GetDeviceClock(int Sno, ref DATETIMEINFO pDateTimeInfo);

        [DllImport("tc400.dll")]
        public static extern int CKT_SetDeviceClock(int Sno, ref DATETIMEINFO pDateTimeInfo);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_SetDeviceDate(int Sno, short Year_Renamed, short Month_Renamed, short Day_Renamed);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_SetDeviceTime(int Sno, short Hour_Renamed, short Minute_Renamed, short Second_Renamed);

        [DllImport("tc400.dll")]
        public static extern int CKT_GetFPTemplate(int Sno, int PersonID, int FPID, ref int pFPData, ref int FPDataLen);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_PutFPTemplate(int Sno, int PersonID, int FPID, byte[] pFPData, int FPDataLen);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_GetFPTemplateSaveFile(int Sno, int PersonID, int FPID, String FPDataFilename);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_PutFPTemplateLoadFile(int Sno, int PersonID, int FPID, String FPDataFilename);

        [DllImport("tc400.dll")]
        public static extern int CKT_GetFPRawData(int Sno, int PersonID, int FPID, ref byte FPRawData);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_PutFPRawData(int Sno, int PersonID, int FPID, ref byte FPRawData);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_GetFPRawDataSaveFile(int Sno, int PersonID, int FPID, String FPDataFilename);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_PutFPRawDataLoadFile(int Sno, int PersonID, int FPID, String FPDataFilename);

        [DllImport("tc400.dll")]
        public static extern int CKT_ListPersonInfo(int Sno, ref int pRecordCount, ref int ppPersons);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_ModifyPersonInfo(int Sno, ref PERSONINFO person);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_DeletePersonInfo(int Sno, int PersonID, int backupID);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_EraseAllPerson(int Sno);

        [DllImport("tc400.dll")]
        public static extern int CKT_ListPersonInfoEx(int Sno, ref int ppLongRun);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_ListPersonProgress(int pLongRun, ref int pRecCount, ref int pRetCount, ref int ppPersons);

        [DllImport("tc400.dll")]
        public static extern int CKT_GetCounts(int Sno, ref int pPersonCount, ref int pFPCount, ref int pClockingsCount);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_ClearClockingRecord(int Sno, int type, int count);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_GetClockingRecordEx(int Sno, ref int ppLongRun);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_GetClockingNewRecordEx(int Sno, ref int ppLongRun);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_GetClockingRecordProgress(int pLongRun, ref int pRecCount, ref int pRetCount, ref int ppPersons);

        [DllImport("tc400.dll")]
        public static extern int CKT_ResetDevice(int Sno);
				
        [DllImport("tc400.dll")]
        public static extern int CKT_GetDeviceInfo(int Sno, ref DEVICEINFO devinfo);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_SetDefaultAuth(int Sno, int Auth);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_SetDoor(int Sno, int Second_Renamed);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_SetSpeakerVolume(int Sno, int Volume);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_SetDeviceAdminPassword(int Sno, String Password);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_SetRealtimeMode(int Sno, int RealMode);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_SetFixWGHead(int Sno, int WGHead);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_SetWG(int Sno, int WGMode);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_SetRingAllow(int Sno, int type);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_SetRepeatKQ(int Sno, int time);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_SetAutoUpdate(int Sno, int AutoUpdate);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_ForceOpenLock(int Sno);

        [DllImport("tc400.dll")]
        public static extern int CKT_ReadRealtimeClocking(ref int ppClockings);

        [DllImport("tc400.dll")]
        public static extern int CKT_GetTimeSection(int Sno, int ord, TIMESECT[] ts);
				
        [DllImport("tc400.dll")]
        public static extern int CKT_SetTimeSection(int Sno, int ord, TIMESECT[] ts);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_GetGroup(int Sno, int ord, int[] grp);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_SetGroup(int Sno, int ord, int[] grp);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_GetHitRingInfo(int Sno, RINGTIME[] array);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_SetHitRingInfo(int Sno, int ord, ref RINGTIME ring);

        [DllImport("tc400.dll")]
        public static extern int CKT_GetMessageByIndex(int Sno, int idx, ref CKT_MessageInfo msg);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_AddMessage(int Sno, ref CKT_MessageInfo msg);
               
        [DllImport("tc400.dll")]
        public static extern int CKT_GetAllMessageHead(int Sno, CKT_MessageHead[] mh);
        
        [DllImport("tc400.dll")]
        public static extern int CKT_DelMessageByIndex(int Sno, int idx);

        [DllImport("tc400.dll")]
        public static extern int CKT_SetDeviceMode(int Sno, int Mode);

        [DllImport("tc400.dll")]
        public static extern int CKT_ChangeConnectionMode(int Mode);

        [DllImport("tc400.dll")]
        public static extern int CKT_SetWorkCode(int Sno, int Mode);

        [DllImport("tc400.dll")]
        public static extern int CKT_GetMachineNumber(int Sno,Byte[] MMID);

        [DllImport("tc400.dll")]
        public static extern int CKT_GetKQState(int Sno, ref CKT_KQState kqs);

        [DllImport("tc400.dll")]
        public static extern int CKT_SetKQState(int Sno, ref CKT_KQState kqs);

        [DllImport("tc400.dll")]
        public static extern int CKT_SetDateTimeFormat(int Sno, int dateF, int timeF);

        [DllImport("tc400.dll")]
        public static extern int CKT_DeleteAllPersonInfo(int Sno);

        [DllImport("tc400.dll")]
        public static extern int CKT_NetDaemonWithPort(int Portint);
        
    }

