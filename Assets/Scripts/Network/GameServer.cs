using LibCSharp.Network.ClientStatic;
using System;
using System.Collections;
using UnityEngine;

static class GameServer
{
	static GameServer()
	{
		//Host = "127.0.0.1";
		Host = "server.partisanwars.net";
		Port = 3726;

		TCPPktClient.onConnect = onConnect;
		TCPPktClient.onDisconnect = onDisconnect;
		TCPPktClient.onMessage = onMessage;
	}

	public static string Host;
	public static int Port;

	static bool isInUse = false;
	static bool isRequestSent = false;
	static string Authorization;
	static string Data;

	static void onMessage(string json)
	{ Data = json; }

	static void onDisconnect()
	{
		isRequestSent = false; // auto resend last request
		log.Warn("GameServer connection lost");
	}

	static void onConnect()
	{
		log.Debug("GameServer connected to: {0}:{1}", Host, Port);
		TCPPktClient.Send("connect", Authorization);
	}

	public static void Connect(string host, int port)
	{
		if (TCPPktClient.state == TCPPktClient.State.CONNECTED)
		{ log.Warn("GameServer already connected!"); }
		else if (TCPPktClient.state == TCPPktClient.State.CONNECTING)
		{ log.Warn("GameServer connecting already in progress!"); }
		else
		{
			log.Debug("GameServer start connecting to: {0}:{1}", host, port);
			TCPPktClient.Connect(host, port);

			Host = host;
			Port = port;
		}
	}

	static void Reconnect()
	{ Connect(Host, Port); }

	const string LOG_LOAD_DATA = "GameServer::LoadData{0} URL: {1} Auth: {2} Body: {3}";

	public static IEnumerator LoadData(string url, Func<string, int> worker, string Authorization, string body = "", bool handleError = false)
	{
		// prevents multiple requesting
		if (isInUse)
		{
			log.Debug(LOG_LOAD_DATA, " [PENDING]", url, Authorization, body);

			while (isInUse)
				yield return null;

			log.Debug(LOG_LOAD_DATA, " [RESUMED]", url, Authorization, body);
		}
		else
		{ log.Debug(LOG_LOAD_DATA, "", url, Authorization, body); }

		isInUse = true;
		isRequestSent = false;
		Data = null;

		GameServer.Authorization = Authorization;

		// first run
		if (TCPPktClient.state == TCPPktClient.State.OFFLINE)
		{
			log.Warn("GameServer not connected");
			Reconnect();
			yield return null;
		}

		while (true)
		{
			TCPPktClient.Poll();

			if (TCPPktClient.state == TCPPktClient.State.CONNECTING)
			{
				yield return null;
				continue;
			}

			// connection lost
			if (TCPPktClient.state == TCPPktClient.State.OFFLINE)
			{
				yield return new WaitForSeconds(1); // pause 1 sec
				Reconnect();
				continue;
			}

			// connected

			if (Data != null) // answer received
			{
				isInUse = false;
				worker(Data);
				break; // request completed
			}

			if (!isRequestSent)
			{
				TCPPktClient.Send(url, body);
				isRequestSent = true;
			}

			yield return null;
		}
	}

	static LibCSharp.Logger log = LibCSharp.Logger.GetCurrentClassLogger();
}