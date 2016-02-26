using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using SocketLibrary;

namespace ServerTest
{

    class ClientThread
    {
        private TcpClient client;

        private static int counter = 0;

        public ClientThread(TcpClient client)
        {
            this.client = client;
            counter++;

            Interlocked.Increment(ref counter);
        }

        // ajetaan omassa säikeessään
        public void ServeClient()
        {
            SocketHelper socketHelper = new SocketHelper(client);
            socketHelper.Open();

            DateTime t = DateTime.Now;

            //luetaan ja käsitellään komennot
            bool ok = true;
            while (ok)
            {
                // katsotaan, onko uutta dataa
                if (socketHelper.DataAvailable())
                {
                    string komento = socketHelper.Read();
                    string vastaus = "";
                    //päivitetään aika
                    t = DateTime.Now;
                    switch (komento)
                    {
                        case COMMANDS.TIME:
                            vastaus = DateTime.Now.ToString();
                            break;
                        case COMMANDS.NUMBER_OF_REQUEST:
                            vastaus = "1"; // TODO
                            break;
                        case COMMANDS.QUIT:
                            vastaus = "Lopetus";
                            ok = false;
                            break;
                    }
                    socketHelper.Write(vastaus);
                    Console.WriteLine(vastaus);
                }
                else
                {
                    TimeSpan sp = DateTime.Now - t;
                    if(sp.TotalSeconds > 60)
                    {// lopetetaan tämän asiakkaan palveleminen
                        Console.WriteLine("Auto Stop!");
                        ok = false;
                    }
                    Thread.Sleep(100);
                }
            }
            //suljetaan yhteydet
            socketHelper.Close();
        }
    }
}
