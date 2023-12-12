﻿using PowerThreadPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Powork.Model;
using Powork.Repository;
using System.Collections.ObjectModel;
using System.ComponentModel;
using PowerThreadPool.EventArguments;
using Powork.Network;
using System.Net.NetworkInformation;

namespace Powork
{
    static public class GlobalVariables
    {
        static private IPAddress localIP = GetLocalIPAddress();
        static public IPAddress LocalIP { get => localIP; }
        static public int UdpPort { get; } = 1096;
        static public int TcpPort { get; } = 614;
        static public string DbName { get; } = "PoworkDB";
        static public TcpServerClient TcpServerClient { get; set; }
        private static ObservableCollection<User> userList;
        public static ObservableCollection<User> UserList 
        { 
            get => userList;
            set
            {
                userList = value;
                if (UserListChanged != null)
                {
                    UserListChanged.Invoke(userList, new EventArgs());
                }
            }
        }
        public delegate void UserListChangedEventHandler(object sender, EventArgs e);
        static public event UserListChangedEventHandler UserListChanged;
        public delegate void GetMessageEventHandler(object sender, EventArgs e);
        static public event GetMessageEventHandler GetMessage;
        public static void InvokeGetMessageEvent(UserMessage userMessage)
        {
            if (GetMessage != null)
            {
                GetMessage.Invoke(userMessage, new EventArgs());
            }
        }
        static public List<User> SelfInfo 
        { 
            get
            { 
                return UserRepository.SelectUserByIp(LocalIP.ToString());
            }
        }

        public static IPAddress GetLocalIPAddress()
        {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType != NetworkInterfaceType.Loopback && ni.OperationalStatus == OperationalStatus.Up)
                {
                    var ipProperties = ni.GetIPProperties();
                    if (ipProperties.GatewayAddresses.Count > 0)
                    {
                        foreach (var ip in ipProperties.UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                if (!ip.Address.ToString().StartsWith("169.254"))
                                {
                                    return ip.Address;
                                }
                            }
                        }
                    }
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
