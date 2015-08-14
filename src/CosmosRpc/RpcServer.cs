﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.34209
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using NLog;

namespace Cosmos.Rpc
{
    public class RpcServer : BaseNetMqServer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private IRpcService _rpcService;
        
        public RpcServer(IRpcService rpcService, string host = "0.0.0.0", int responsePort = -1) : base(responsePort, 0, host)
        {
            _rpcService = rpcService;
        }

        protected override async Task<byte[]> ProcessRequest(byte[] reqData)
        {
            return await Task.Run(() =>
            {
                var requestMsg = MsgPackTool.GetMsg<RequestMsg>(reqData);

                var resMsg = new ResponseMsg();
                resMsg.IsError = false;

                var method = _rpcService.GetType().GetMethod(requestMsg.FuncName);
                object executeResult = null;

                if (method != null)
                {
                    var arguments = MsgPackTool.ConvertMsgPackObjectArray(requestMsg.Arguments);

                    try
                    {
                        var result = method.Invoke(_rpcService, arguments);
                        executeResult = result;
                    }
                    catch (Exception e)
                    {
                        resMsg.IsError = true;
                        resMsg.ErrorMessage = string.Format("[ERROR]Method '{0}' Exception: {1}", requestMsg.FuncName, e);
                        Logger.Error(resMsg.ErrorMessage);
                    }
                }
                else
                {
                    resMsg.IsError = true;
                    resMsg.ErrorMessage = string.Format("[ERROR]Not found method: {0}", requestMsg.FuncName);
                    Logger.Error(resMsg.ErrorMessage);
                    Thread.Sleep(1);
                }

                resMsg.Value = executeResult;
                return MsgPackTool.GetBytes(resMsg);
            });
        }

    }
    /// <summary>
    /// Any call RPC Fucntion must in this class
    /// </summary>
    public interface IRpcService
    {
    }

    /// <summary>
    /// 使用ZeroMQ进行RPC
    /// </summary>
    //public class RpcServer : IDisposable
    //{
    //    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    //    public Poller Poller;
    //    private Task _pollerTask;

    //    internal NetMQContext _context;
    //    private ResponseSocket _server;
    //    public int requestPort { get; private set; }
    //    public string Host { get; private set; }

    //    object RpcInstace;

    //    public RpcServer(rpcService rpcInstance, string host = "0.0.0.0")
    //    {
    //        RpcInstace = rpcInstance;
    //        Poller = new Poller();
    //        Host = host;

    //        _context = NetMQContext.Create();
    //        _server = _context.CreateResponseSocket();

    //        Poller.AddSocket(_server);

    //        requestPort = _server.BindRandomPort("tcp://" + host);
    //        _server.ReceiveReady += OnReceiveReady;

    //        _pollerTask = Task.Run(() =>
    //        {
    //            Poller.Start();
    //        });
    //    }

    //    private void OnReceiveReady(object sender, NetMQSocketEventArgs e)
    //    {
    //        var data = _server.Receive();
    //        var req = RpcShare.RequestSerializer.UnpackSingleObject(data);

    //        ProcessRequest(req);
    //    }

    //    async void ProcessRequest(RequestMsg requestMsg)
    //    {
    //        var method = RpcInstace.GetType().GetMethod(requestMsg.FuncName);
    //        object executeResult = null;

    //        if (method != null)
    //        {
    //            var arguments = new object[requestMsg.Arguments.Length];
    //            for (var i = 0; i < arguments.Length; i++) // MsgPack.MessagePackObject arg in requestProto.Arguments)
    //            {
    //                MsgPack.MessagePackObject arg = (MsgPack.MessagePackObject)requestMsg.Arguments[i];
    //                arguments[i] = arg.ToObject();
    //            }
    //            var result = method.Invoke(RpcInstace, arguments);

    //            if (result is Task)
    //            {
    //                executeResult = await (result as Task<object>);
    //            }
    //            else
    //            {
    //                executeResult = result;
    //            }

    //        }
    //        else
    //        {
    //            Logger.Error("[ERROR]Not found method: {0}", requestMsg.FuncName);
    //            Thread.Sleep(1);
    //        }

    //        var data = RpcShare.ResponseSerializer.PackSingleObject(new ResponseMsg {
    //            RequestId = requestMsg.RequestId,
    //            Value = executeResult,
    //        });
    //        _server.Send(data);
    //    }


    //    public void Dispose()
    //    {
    //        Poller.RemoveSocket(_server);
    //        _server.Close();
    //        _context.Dispose();

    //        Poller.Stop();
    //        Poller.Dispose();
    //        _pollerTask.Dispose();
    //    }
    //}
}

