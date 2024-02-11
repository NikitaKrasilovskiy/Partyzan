using System;

namespace LibCSharp.Network.ClientStatic
{
	static partial class TCPPktClient
	{
		/// <summary>Вызывается при приходе сообщения от сервера</summary>
		/// <param name="opcode">Код команды</param>
		/// <param name="json">Текст команды</param>
		public static OnMessage onMessage;

		/// <summary>Вызывается при установке соединения с сервером</summary>
		public static OnConnect onConnect;

		/// <summary>Вызывается при потере соединения с сервером</summary>
		public static OnDisconnect onDisconnect;

		public delegate void OnMessage(string json);

		public delegate void OnConnect();
		public delegate void OnDisconnect();

		public enum State
		{
			OFFLINE,
			CONNECTING,
			CONNECTED,
		}

		public static State state { get; private set; }

		public static void Connect(string host, int port)
		{
			if (host == null)
				throw new ArgumentNullException("host");

			if (state != State.OFFLINE)
				throw new Exception("Already connecting!");

			BeginConnecting(host, port);
		}

		/// <summary>Послать сообщение на сервер</summary>
		public static void Send(string request, string body)
		{
			if (state != State.CONNECTED)
				return;

			SendRequest(request, body);
		}

		/// <summary>Обновление состояния сети. Функцию необходимо периодически вызывать из Update или FixedUpdate</summary> 
		public static void Poll()
		{
			switch (state)
			{
				case State.OFFLINE:
					break;

				case State.CONNECTING:
					PollConnecting();
					break;

				case State.CONNECTED:
					PollConnected();
					break;
			}
		}
	}
}