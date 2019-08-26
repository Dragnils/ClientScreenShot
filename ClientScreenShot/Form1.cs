using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace ClientScreenShot
{
    public partial class Form1 : Form
    {
        private IPEndPoint ipEndPoint;
        private UdpClient udpClient;
        private int width;
        private int height;
        Socket srv_soc;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            srv_soc = new Socket(AddressFamily.InterNetwork,
                           SocketType.Dgram,
                           ProtocolType.Udp);
            try
            {
                int port = int.Parse(txtPort.Text);
                srv_soc.Connect(txtIPAdr.Text, port);
                if (srv_soc.Connected)
                { // OK
                    txtNotif.Text += "Connect success!\r\n";
                    // Получение приветствия от серевера
                    
                }
            }
            catch (Exception ex)
            {
                txtNotif.Text +=
                  "ОШИБКА: " + ex.Message + "\r\n";
            }

           
        }

        private void btnScreen_Click(object sender, EventArgs e)
        {
            Run();
        }

        public void Run()
        {
            width = Screen.PrimaryScreen.Bounds.Width;
            height = Screen.PrimaryScreen.Bounds.Height; 

            udpClient = new UdpClient();
            Bitmap BackGround = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(BackGround);

            while (true)
            {
                graphics.CopyFromScreen(0, 0, 0, 0, new Size(width, height));   // Получаем снимок экрана
                byte[] bytes = ConvertToByte(BackGround);                      // Получаем изображение в виде массива байтов
                List<byte[]> lst = CutMsg(bytes);
                for (int i = 0; i < lst.Count; i++)
                {
                    udpClient.Send(lst[i], lst[i].Length, ipEndPoint);                // Отправляем картинку клиенту
                }
            }
        }
        private byte[] ConvertToByte(Bitmap bmp)
        {
            MemoryStream memoryStream = new MemoryStream();
            bmp.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            return memoryStream.ToArray();
        }

        private List<byte[]> CutMsg(byte[] bt)
        {
            int Lenght = bt.Length;
            byte[] temp;
            List<byte[]> msg = new List<byte[]>();

            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(BitConverter.GetBytes((short)((Lenght / 65500) + 1)), 0, 2);
            memoryStream.Write(bt, 0, bt.Length);
            memoryStream.Position = 0;
            while (Lenght > 0)
            {
                temp = new byte[65500];
                memoryStream.Read(temp, 0, 65500);
                msg.Add(temp);
                Lenght -= 65500;
            }

            return msg;
        }

    }
}
