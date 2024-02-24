﻿using System.Net.WebSockets;

namespace FortBackend.src.App.XMPP.Helpers.Resources
{
    public class GlobalData
    {
        public static List<AccessToken> AccessToken { get; set; } = new List<AccessToken>();
        public static List<RefreshToken> RefreshToken { get; set; } = new List<RefreshToken>();
        public static List<ExchangeCode> ExchangeCode { get; set; } = new List<ExchangeCode>();
        public static List<ClientToken> ClientToken { get; set; } = new List<ClientToken>();

        public static List<Clients> Clients { get; set; } = new List<Clients>();
        public static Dictionary<string, RoomsLessDyanmic> Rooms { get; set; } = new Dictionary<string, RoomsLessDyanmic>();
    }

    public class AccessToken
    {
        public string token { get; set; } = string.Empty;
        public string accountId { get; set; } = string.Empty;
    }

    public class ExchangeCode
    {
        public string token { get; set; } = string.Empty;
        public string accountId { get; set; } = string.Empty;
    }

    public class RefreshToken
    {
        public string token { get; set; } = string.Empty;
        public string accountId { get; set; } = string.Empty;
    }

    public class ClientToken
    {
        public string token { get; set; } = string.Empty;
        public string accountId { get; set; } = string.Empty;
    }

    public class Clients
    {
        public WebSocket Client { get; set; }
        public string displayName { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;
        public string jid { get; set; } = string.Empty;
        public string resource { get; set; } = string.Empty;
        public lastPresenceUpdate lastPresenceUpdate { get; set; }
        public string accountId { get; set; } = string.Empty;
    }


    public class lastPresenceUpdate
    {
        public bool away = false;
        public string presence = "{}";
    }

    public class MoreINfoINsideMembers
    {
        public string accountId { get; set; } = string.Empty;
    }
    public class RoomsLessDyanmic
    {
        public List<MoreINfoINsideMembers> members = new List<MoreINfoINsideMembers>();
    }
}