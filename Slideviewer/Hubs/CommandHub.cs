using ControlHub.Common;
using ControlHub.Model;
using ControlHub.Model.ValueObject;
using ControlHub.Repository;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web.Script.Serialization;


namespace ControlHub.Hubs
{
    public class CommandHub : Hub
    {
        /* 
         * add validate viewcommand
         */
        public static ObjectCache Cache = MemoryCache.Default;

        #region properties

        public IList<ClientConnection> ClientList
        {
            get
            {
                if (Cache[CacheItems.Clients.ToString()] == null)
                    return new List<ClientConnection>();

                return (List<ClientConnection>)Cache[CacheItems.Clients.ToString()];
            }
            set
            {
                Cache.Set(CacheItems.Clients.ToString(), value, DateTimeOffset.MaxValue);
            }
        }


        public IList<ClientConnection> ManagerList
        {
            get
            {
                if (Cache["Managers"] == null)
                    return new List<ClientConnection>();

                return (List<ClientConnection>)Cache["Managers"];
            }
            set
            {
                Cache.Set("Managers", value, DateTimeOffset.MaxValue);
            }
        }

        #endregion

        public override Task OnDisconnected(bool stopCalled)
        {
            if (ClientList.Any(x => x.ConnectionId == Context.ConnectionId))
            {
                var dummy = ClientList.First(x => x.ConnectionId == Context.ConnectionId);
                PublishLogEvent("client ", string.Format("Client: '{0}' disconnected. Id: '{1}'.", dummy.Name, dummy.ConnectionId));

                ClientList.Remove(ClientList.First(x => x.ConnectionId == Context.ConnectionId));                
            }

            if (ManagerList.Any(x => x.ConnectionId == Context.ConnectionId))
            {
                var dummy = ManagerList.First(x => x.ConnectionId == Context.ConnectionId);
                PublishLogEvent("manager", string.Format("Manager: '{0}' disconnected. Id: '{1}'.", dummy.Name, dummy.ConnectionId));

                ManagerList.Remove(ManagerList.First(x => x.ConnectionId == Context.ConnectionId));
            }

            //Push to any manager client disconnected            
            Task.Run(() => PubishClientListChanged());
            
            return base.OnDisconnected(stopCalled);
        }


        #region manager methods
              

        public void GetClients()
        {   
            Task.Run(() => Clients.Client(Context.ConnectionId).clientListChanges(
                JsonHelper<ClientConnection[]>.ToJson(ClientList.OrderBy(x => x.Name).ToArray())));
        }

        public void PubishClientListChanged()
        {
            foreach (var manager in ManagerList)
            {
                Task.Run(() => Clients.Client(manager.ConnectionId).clientListChanges(
                    JsonHelper<ClientConnection[]>.ToJson(ClientList.OrderBy(x => x.Name).ToArray())));
            }            
        }

        public void PublishLogEvent(string level, string message)
        {
            var log = new LogRow
            {
                Level = level,
                Message = message,
                TimeStamp = DateTime.Now.ToString()
            };

            foreach (var manager in ManagerList)
            {
                Task.Run(() => Clients.Client(manager.ConnectionId).logEvent(
                    JsonHelper<LogRow>.ToJson(log)));
            }
        }
        
        public void RegisterManager(string name)
        {
            var obj = ManagerList;

            if (obj.All(x => x.ConnectionId != Context.ConnectionId))
            {
                //Add connection
                obj.Add(new ClientConnection
                {
                    ConnectionId = Context.ConnectionId,
                    Name = name
                });

                //Update cache
                ManagerList = obj;

                PublishLogEvent("connect", string.Format("manager: '{0}' connected, id: '{1}'.", name, Context.ConnectionId));
            }

            //give the manager the client list
            GetClients();
        }

        #endregion


        #region client methods

        public void PushMessageToClients(string[] connectionIds, string message)
        {
            foreach (var connectionId in connectionIds)
            {
                Task.Run(() => Clients.Client(connectionId).NewMessage(message)); 
            }

            PublishLogEvent("message", string.Format("Message: '{0}' sent.", message));
        }

        public void PushCommandToClients(string[] connectionIds, ViewCommand cmd)
        {
            var json = JsonHelper<ViewCommand>.ToJson(cmd);

            foreach (var connectionId in connectionIds)
            {
                Task.Run(() => Clients.Client(connectionId).NewCommand(json));
            }

            PublishLogEvent("command", string.Format("Command: '{0}' executed.", json));
        }
        
        public void BroadcastMessage(ViewCommand cmd)
        {
            Task.Run(() => Clients.All.NewCommand(JsonHelper<ViewCommand>.ToJson(cmd)));

            PublishLogEvent("command", string.Format("Broadcast: '{0}' executed.", cmd));            
        }


        public void RegisterToEvents(string name)
        {
            var obj = ClientList;
            
            if (obj.All(x => x.ConnectionId != Context.ConnectionId))
            {
                //Add connection
                obj.Add(new ClientConnection
                {
                    ConnectionId = Context.ConnectionId,
                    Name = name
                });

                //Update cache
                ClientList = obj;


                //Only when new
                PubishClientListChanged();

                PublishLogEvent("connect", string.Format("Client: '{0}' connected, id: '{1}'.", name, Context.ConnectionId));
            }
            
            PushMessageToClients(new[] { Context.ConnectionId }, string.Format("Hi, {0}", name));
        }

        #endregion

        #region slideshow methods

        public void GetSlideshow(string slideShowName)
        {
            PushSlidesToClients(new[] { Context.ConnectionId }, slideShowName);
        }

        public void PushSlidesToClients(string[] connectionIds, string slideShowName)
        {
            try
            {
                var json = JsonHelper<Slide[]>.ToJson(SlideShowFactory.GetSlideShow(slideShowName).ToArray());

                foreach (var connectionId in connectionIds)
                {
                    Task.Run(() => Clients.Client(connectionId).SlidesChanged(json));
                }

                PublishLogEvent("command", string.Format("Command: Slideshow published.", json));
            }
            catch
            { /* probably path not found */ }
        }


        #endregion
    }
}