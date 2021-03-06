﻿
using NUnit.Framework;
using System;
using Cosmos.Rpc;
using System.Threading.Tasks;
using System.IO;
using Cosmos.Actor;
using MsgPack.Serialization;
using NLog;
using etcetera;

namespace Cosmos.Test
{
    [TestFixture()]
    public class TestEtcd
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private EtcdClient etcd;

        [SetUp]
        public void Init()
        {
            etcd = new EtcdClient(new Uri("http://localhost:4001/v2/keys/"));
            
        }

        //[TearDown]
        //void UnInit()
        //{
        //    etcd = null;
        //}

        /// <summary>
        /// 务必Etcd在执行中
        /// </summary>
        /// <returns></returns>
        bool CheckEtcdRunning()
        {
            try
            {
                var leader = etcd.Statistics.Leader();
                return leader != null;
            }
            catch (Exception)
            {
                Logger.Warn("Etcd is not running !! Pass the test!!");
                return false;
            }
        }

        [Test]
        public void TestEtcdSimpleSetAndAdd()
        {
            if (!CheckEtcdRunning())
            {
                Assert.Ignore();
                return;
            }
            var respSet = etcd.Set("csharp-etcetera-test", "123");
            Assert.AreEqual(respSet.Node.Value, "123");
            var respGet = etcd.Get("csharp-etcetera-test");
            Assert.AreEqual(respGet.Node.Value, "123");
        }
        [Test]
        public void TestEtcdDirSetAndAdd()
        {
            if (!CheckEtcdRunning())
            {
                Assert.Ignore();
                return;
            }
            
            var respSet = etcd.Set("csharp-etcetera-test-dir/sample-key", "123");
            Assert.AreEqual(respSet.Node.Value, "123");
            var respGet = etcd.Get("csharp-etcetera-test-dir/sample-key");
            Assert.AreEqual(respGet.Node.Value, "123");

            var respGetDir = etcd.Get("csharp-etcetera-test-dir");
            Assert.True(respGetDir.Node.Dir);

            var deleteDir = etcd.DeleteDir("csharp-etcetera-test-dir");
            Assert.AreEqual(deleteDir.ErrorCode, 108); // not empty dir cannot delete

            var deleteDirAll = etcd.DeleteDir("csharp-etcetera-test-dir", true);
            Assert.AreEqual(deleteDirAll.ErrorCode, null);

            var respGetDir2 = etcd.Get("csharp-etcetera-test-dir");
            Assert.AreEqual(respGetDir2.Node, null);

        }

        [Test]
        public async void TestWatch()
        {
            var watchTask = Task.Run(() =>
            {
                etcd.Watch("test-watch", (response) =>
                {
                    Assert.AreEqual(response.Node.Value, "NewKey");
                });
            });

            var setRes = etcd.Set("test-watch", "NewKey");
            Assert.AreEqual(setRes.Node.Value, "NewKey");

            await watchTask;
            Assert.Pass();
        }

        [Test]
        public void TestJsonDiscoveryMode()
        {
            var dis = new JsonDiscoveryMode("config/actors.json");
            Assert.AreEqual(3, dis.Nodes.Count);
            Assert.AreEqual("http-fronend-actor", dis.Nodes[0].Name);

        }

        [Test]
        public async void TestEtcdDiscoveryMode()
        {
            var dis = new EtcdDiscoveryMode("testTopTopKey", "http://localhost:2379");
            var res = await dis.SetAsync("testasynckey", "okvalue");
            Assert.AreEqual(res.Node.Value, "okvalue");
            res = await dis.GetAsync("testasynckey");
            Assert.AreEqual(res.Node.Value, "okvalue");
        }

        [Test]
        public async void RegisterEtcdNode()
        {
            var dis = new EtcdDiscoveryMode("testTopKey", "http://localhost:2379");
            var testNode = new ActorNodeConfig();
            testNode.Name = "TestNodeDir/TestNodeKey1";
            testNode.Host = "TestHost";

            var result = await dis.RegisterActor(testNode);
            Assert.IsTrue(result);
            var nodes = await dis.GetNodes();
            Assert.AreEqual(nodes["TestNodeDir/TestNodeKey1"].Host, "TestHost");
        }
    }
}

