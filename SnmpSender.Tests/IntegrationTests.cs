using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SnmpSender.Tests
{
    public class SnmpEngineFixture : IDisposable
    {
        static SnmpEngine engine;
        
        public SnmpEngineFixture()
        {            
            Task.Run(() =>
            {
                var store = new ObjectStore();

                var getv1 = new GetV1MessageHandler();
                var getv1Mapping = new HandlerMapping("v1", "GET", getv1);

                var getv23 = new GetMessageHandler();
                var getv23Mapping = new HandlerMapping("v2,v3", "GET", getv23);

                var setv1 = new SetV1MessageHandler();
                var setv1Mapping = new HandlerMapping("v1", "SET", setv1);

                var setv23 = new SetMessageHandler();
                var setv23Mapping = new HandlerMapping("v2,v3", "SET", setv23);

                var getnextv1 = new GetNextV1MessageHandler();
                var getnextv1Mapping = new HandlerMapping("v1", "GETNEXT", getnextv1);

                var getnextv23 = new GetNextMessageHandler();
                var getnextv23Mapping = new HandlerMapping("v2,v3", "GETNEXT", getnextv23);

                var getbulk = new GetBulkMessageHandler();
                var getbulkMapping = new HandlerMapping("v2,v3", "GETBULK", getbulk);

                var v1 = new Version1MembershipProvider(new OctetString("public"), new OctetString("public"));
                var v2 = new Version2MembershipProvider(new OctetString("public"), new OctetString("public"));
                var v3 = new Version3MembershipProvider();
                var membership = new ComposedMembershipProvider(new IMembershipProvider[] { v1, v2, v3 });
                var handlerFactory = new MessageHandlerFactory(new[]
                {
                    getv1Mapping,
                    getv23Mapping,
                    setv1Mapping,
                    setv23Mapping,
                    getnextv1Mapping,
                    getnextv23Mapping,
                    getbulkMapping
                });

                var pipelineFactory = new SnmpApplicationFactory(store, membership, handlerFactory);
                engine = new SnmpEngine(pipelineFactory, new Listener { }, new EngineGroup());
                engine.Listener.AddBinding(new IPEndPoint(IPAddress.Any, 161));
                engine.Listener.MessageReceived += IntegrationTests.RequestReceived;
                engine.Start();
            });        
        }
        
        public void Dispose()
        {
            engine.Stop();
            engine.Dispose();
        }
    }
    public class IntegrationTests : IClassFixture<SnmpEngineFixture>
    {
        static CountdownEvent countEventObject;
        static string respose;
        public IntegrationTests(SnmpEngineFixture engine)
        {

        }

        [Fact]
        public void ShouldContainObjectIdOfMessage()
        {
            countEventObject = new CountdownEvent(1);
            GetCommand.Send(161, 2, "public", "127.0.0.1", "1.2.3.4");
            countEventObject.Wait();
            Assert.Equal("1.2.3.4", respose);
        }
        [Fact]
        public void SetShouldContainObjectIdOfMessage()
        {
            countEventObject = new CountdownEvent(1);
            var variable = new List<string>() { "1.2.3.4,s,hello", "1.2.3.4.5,i,100" };
            SetCommand.Send(161, 2, "public", "127.0.0.1", variable);
            countEventObject.Wait();
            Assert.Equal("1.2.3.4", respose);
        }
        [Fact]
        public void TrapShouldContainObjectIdOfMessage()
        {
            countEventObject = new CountdownEvent(1);
            var variable = new List<string>() { "1.2.3.4,s,hello", "1.2.3.4.5,i,100" };
            TrapCommand.Send(161, 2, "public", "127.0.0.1", "1.2.3.4.5", 1, variable);
            countEventObject.Wait();
            Assert.Equal("1.2.3.4", respose);
        }
        internal static void RequestReceived(object sender, MessageReceivedEventArgs e)
        {
            respose = e.Message.Variables().First().Id.ToString();
            countEventObject.Signal();
        }
    }
}
