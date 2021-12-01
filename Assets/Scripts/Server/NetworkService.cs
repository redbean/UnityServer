using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class NetworkService
{
    //접속 확인 
    Listener clientListener;

    //소켓 메서드 호출 시 필요, 인터페이스 안생성 해도됨
    SocketAsyncEventArgsPool receiveEventArgsPool;
    SocketAsyncEventArgsPool sendEventArgsPool;


    //데이터 송수신 버퍼 관리 매니저 객체
    BufferManager bufferMgr;

    //클라 접속시 통보 수단
    public delegate void SessionHandler(UserToken token);
    public SessionHandler sessionCallback { get; set; }


    public void Listen(string host, int port, int backlog)
    {
        Listener listener = new Listener();
        listener.OnNewClientCallback += OnNewCliConnected;
        listener.Start(host, port, backlog);
    }

    private void OnNewCliConnected(Socket clientSoc, object token)
    {
        
    }
}
