using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Server
{
    class Program
    {
        static Socket srv;
        static IPEndPoint srv_endp;
        static int srv_port = 12345;
        private UdpClient udpClient;

       
        static void Main(string[] args)
        {
            srv = new Socket(SocketType.Dgram, ProtocolType.Udp);
            srv_endp = new IPEndPoint(IPAddress.Any, srv_port);

        }



        int countErorr = 0;
        private void AsyncReceiver()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Loopback, 0);

            while (true)
            {
                try
                {
                    MemoryStream memoryStream = new MemoryStream();
                    byte[] bytes = udpClient.Receive(ref ep);
                    memoryStream.Write(bytes, 2, bytes.Length - 2);

                    int countMsg = bytes[0] - 1;
                    if (countMsg > 10)
                        throw new Exception("Потеря первого пакета");
                    for (int i = 0; i < countMsg; i++)
                    {
                        byte[] bt = udpClient.Receive(ref ep);
                        memoryStream.Write(bt, 0, bt.Length);
                    }

                    GetData(memoryStream.ToArray());
                    memoryStream.Close();
                }
                catch (Exception ex)
                {
                    countErorr++;
                }
            }
        }
}
