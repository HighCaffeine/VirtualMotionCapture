using System;
namespace VMSServerHeader
{
    public class ServerData
    {
        public static int BufferSize = 512;
        public static char MessageDivideChar = '|';
        public static char ParamsDivideChar = '_';
        public static string[] SendTypeToken = {"Ping",
                                                "Fail", "Success", "Sync", "Connect",
                                                "" };
    }
    public enum eSendType
    {
        // Fail, Suc Only Use (Server -> Client)
        // **Network Message Format** 
        //   ({fail / suc}_){datatype}|message|send id|{valueCount}|{params(Join(''))...}
        // 
        // Message Format Example (Attack (4) 1001 user attack 2001, 2002 monster)
        // *client -> server example 
        // 4|attack(msg)|1001(id)|2(hitted object count)|2001_2002(monster / character ID)
        // 
        // *server -> client example (Success)
        // 1_4|attack(message)|1001(id)|2(hitted object count)|2001_2002(monster / character ID)
        // 0_4|0|0|0|0
        // 
        Ping, Fail, Success, Sync,
    }

    public enum eResultType
    {
        SendType = 0,
        Message = 1,
        AccountID = 2,
        DataCount = 3,
        Datas = 4,
    }

    public enum eClientType
    {
        Broadast,
        MainControl,
        SubControl,

        Count,

        //TEST
        XsensSuit = 0,
        XsensClient,
        UnityVRMClient,
        //TEST
    }
}