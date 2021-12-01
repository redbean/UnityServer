using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Listener
{
    SocketAsyncEventArgs acceptArgs;
    Socket listenSocket;
    AutoResetEvent flowCtrlEvent;

    public delegate void NewClientHandler(Socket clientSoc, object token);
    public NewClientHandler OnNewClientCallback;

    public Listener()
    {
        this.OnNewClientCallback = null;
    }

    public void Start(string host, int port, int backLog)
    {
        listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPAddress address;
        if (host == "0.0.0.0")
            address = IPAddress.Any;
        else
            address = IPAddress.Parse(host);


        //클라이언트가 당도할 엔드 포인트(== 서버)
        IPEndPoint endPoint = new IPEndPoint(address, port);

        try
        {
            //Bind, Listen, Accept Process

            this.listenSocket.Bind(endPoint);
            this.listenSocket.Listen(backLog);

            this.acceptArgs = new SocketAsyncEventArgs();
            this.acceptArgs.Completed += new System.EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);


            //비동기 대기 블로킹 방지
            this.listenSocket.AcceptAsync(this.acceptArgs);

            Thread listenThread = new Thread(doListen);
            listenThread.Start();

        } 
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private void doListen()
    {
        this.flowCtrlEvent = new AutoResetEvent(false);
        while (true)
        {
            this.acceptArgs.AcceptSocket = null;
            bool pending = true;
            try
            {
                pending = listenSocket.AcceptAsync(this.acceptArgs);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                continue;
            }

            if(!pending)
            {
                OnAcceptCompleted(null, this.acceptArgs);
            }
            flowCtrlEvent.WaitOne();
        }
        flowCtrlEvent = new AutoResetEvent(false);
    }

    private void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
    {
        if(e.SocketError == SocketError.Success)
        {
            Socket clientSoc = e.AcceptSocket;
            this.flowCtrlEvent.Set();
            if(this.OnNewClientCallback != null)
            {
                OnNewClientCallback(clientSoc, e.UserToken);
            }
            return;
        }
        else
        {
            Console.WriteLine("Failed to accept client");
        }
        this.flowCtrlEvent.Set();

    }
}