using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Server.Aspects;
using Server.Interfaces;
using Server.Proxy;

namespace Server.Hubs
{
    [HubName("chat")]
    public class ChatHub : Hub
    {
        private static List<string> _bannedWords = new List<string>();
        private static List<User> _users = new List<User>();
        private static Hashtable _channels = new Hashtable();
        private static Hashtable _bannedUsers = new Hashtable();
        private readonly IWordProcesser _wordProcesser;

        public ChatHub(IWordProcesser wordProcesser)
        {
            this._wordProcesser = wordProcesser;
        }

        [LogAspect("User logged in")]
        public void Login(string name)
        {
            if (_users.Any(m => m.Name == name))
                throw new HubException("USED_NAME");

            _users.Add(new User()
            {
                ConnectionId = Context.ConnectionId,
                Name = name
            });

            Clients.Caller.loginSuccess(_channels.Keys);
        }

        [LogAspect("Channel created")]
        public void CreateChannel(string channel)
        {
            if (_channels.ContainsKey(channel))
                throw new HubException("This channel is already created");

            _channels.Add(channel, new List<string>() { });

            Groups.Add(Context.ConnectionId, channel);

            Clients.All.channels(_channels.Keys);
        }

        [HubMethodName("join")]
        [LogAspect("Joined to channel")]
        public List<string> JoinChannel(string channel, string name)
        {
            if (!_channels.ContainsKey(channel))
                throw new HubException("Channel does not exsist!");

            List<string> users = (List<string>)_channels[channel];
            if (!users.Contains(name))
            {
                users.Add(name);
                Groups.Add(Context.ConnectionId, channel);
                Clients.OthersInGroup(channel).users(users, name);
            }

            return users;
        }

        public string Say(Conversation conversation)
        {
            if (!_channels.ContainsKey(conversation.Channel))
                throw new HubException("Channel does not exsist!");

            // try to throw only an Exception and see diff. 
            if (_wordProcesser.CheckBannedWord(_bannedWords, conversation.Message))
                throw new HubException("BANNED_WORD");

            Clients.OthersInGroup(conversation.Channel).conversation(conversation);

            return conversation.Message;
        }

        [HubMethodName("addBannedWord")]
        public string AddBannedWord(string word)
        {
            if (!_bannedWords.Contains(word))
                _bannedWords.Add(word);

            return $"{word} is added to banned list";
        }

        // without HubMethodName this function can be called by listBannedWords
        public List<string> ListBannedWords()
        {
            return _bannedWords;
        }

        public string RemoveBannedWord(string word)
        {
            if (!_bannedWords.Contains(word))
                throw new HubException("Word is not in banned list");

            _bannedWords.Remove(word);

            return $"{word} is removed from list";
        }

        [HubMethodName("banUser")]
        public string BanUserFromChannel(string channel, string name)
        {
            if (!_channels.ContainsKey(channel))
                throw new HubException("Channel does not exsist!");

            var users = (List<string>)_channels[channel];
            if (!users.Contains(name))
                throw new HubException("User could not found in channel you specified!");

            users.Remove(name);

            if (!_bannedUsers.ContainsKey(name))
                _bannedUsers.Add(name, new List<string>());

            var bannedChannels = (List<string>)_bannedUsers[name];
            bannedChannels.Add(channel);

            Clients.Group(channel).userBanned(new UserBannedResult() { Users = users, BannedUser = name });

            return $"{name} is banned from {channel}";
        }

        public string BanUserFromServer(string name)
        {
            if (_users.Any(u => u.Name == name))
            {
                User user = _users.Find(u => u.Name == name);

                // we are informing everyone except banned user
                Clients.AllExcept(user.ConnectionId).userBannedFromServer(name);

                // we are inforing banned user
                Clients.AllExcept(_users.Where(u => u.Name != name).Select(s => s.ConnectionId).ToArray()).bannedFromServer();

                return $"{name} is banned from server";
            }
            else
            {
                return $"{name} could not found in user list";
            }
        }

        public override Task OnConnected()
        {
            Console.WriteLine($"User is connected with {Context.ConnectionId} connection id");

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var user = _users.Find(u => u.ConnectionId == Context.ConnectionId);
            if (user != null)
            {
                _users.Remove(user);

                List<string> _usersInChannel;
                List<string> _channelGroups = new List<string>();
                foreach (string channel in _channels.Keys)
                {
                    _usersInChannel = (List<string>)_channels[channel];
                    if (_usersInChannel.Contains(user.Name))
                        _channelGroups.Add(channel);
                }

                Clients.OthersInGroups(_channelGroups).userDisconnected(user.Name);
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}
