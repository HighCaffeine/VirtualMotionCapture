using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Collections;

using VMSServerHeader;
using System.Diagnostics;

public class ServerManager : GenericSingleton<ServerManager>
{
    private UdpClient client;
    IPEndPoint serverEP;

    private Stopwatch stopwatch = new Stopwatch();
    [Header("Server Port(XsensClient)")][SerializeField] private int port = 12345;

    void Start()
    {
        client = new UdpClient();
        serverEP = new IPEndPoint(IPAddress.Loopback, port);

        client.BeginReceive(OnReceive, null);
    }

    public void SendData(string sendData, eClientType clientType)
    {
        //데이터 임의 생성 (1MB)
        byte[] data = new byte[1024 * 1024];
        stopwatch.Restart();
        client.Send(data, data.Length, serverEP);
        UnityEngine.Debug.Log("1MB 전송");
    }

    public void OnReceive(System.IAsyncResult ar)
    {
        IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
        byte[] recvBytes = client.EndReceive(ar, ref ep);
        float mb = (float)recvBytes.Length / (1024 * 1024);

        float rtt = (float)stopwatch.Elapsed.Milliseconds;
        UnityEngine.Debug.Log($"받은 데이터 : {recvBytes.Length} bytes from {ep} ({mb:F1} MB({rtt:F0})ms)");

        client.BeginReceive(OnReceive, null);
    }

}
