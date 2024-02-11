using System;
using System.IO;
using System.Net.Sockets;

namespace LibCSharp.Network.ClientStatic
{
	static partial class TCPPktClient
	{
		static bool m_EventConnectingSuccess = false; // atomic
		static bool m_EventConnectingFailed = false; // atomic
		static string m_ConnectErrorMessage;
		static DateTime m_ConnectStartTime;

		static void BeginConnecting(string host, int port)
		{
			log.Debug("Connecting to {0}:{1}", host, port);

			m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			//m_socket.ReceiveBufferSize = ?;
			//m_socket.SendBufferSize = ?;

			ResetRXPkt();
			m_pkt_tx = null;

			state = State.CONNECTING;

			m_EventConnectingSuccess = false;
			m_EventConnectingFailed = false;
			m_ConnectErrorMessage = null;
			m_ConnectStartTime = DateTime.Now;

			try
			{ m_socket.BeginConnect(host, port, new AsyncCallback(AsyncConnectCallback), m_socket); }
			catch (Exception e)
			{
				log.Error("Connect exception: '{0}'", e.Message);
				state = State.OFFLINE;
				onDisconnect?.Invoke();
			}
		}

		// Executes in other thread!
		static void AsyncConnectCallback(IAsyncResult res)
		{
			try
			{ m_socket.EndConnect(res); }
			catch (Exception e)
			{
				m_ConnectErrorMessage = e.Message;
				m_EventConnectingFailed = true;

				return;
			}

			m_socket.Blocking = false;
			m_socket.NoDelay = true;

			m_EventConnectingSuccess = true;
		}

		static void PollConnecting()
		{
			if (m_EventConnectingSuccess)
			{
				state = State.CONNECTED;
				onConnect?.Invoke();
			}
			else if (m_EventConnectingFailed)
			{
				log.Warn("AsyncConnectCallback EndConnect exception: {0}", m_ConnectErrorMessage);
				state = State.OFFLINE;
				onDisconnect?.Invoke();
			}
			else if ((DateTime.Now - m_ConnectStartTime).TotalMilliseconds > 5000)
			{
				log.Warn("Connecting attempt timed out, closing...");
				m_socket.Close();
				m_ConnectStartTime = DateTime.Now; // avoid repeating
			}
		}

		static void PollConnected()
		{
			if (ReadData()
				&& WriteData())
				return;

			// disconnected

			state = State.OFFLINE;
			onDisconnect?.Invoke();
		}

		static void ResetRXPkt()
		{
			if (m_pkt_rx == null)
				m_pkt_rx = new NetPkt(m_socket.ReceiveBufferSize);

			m_pkt_rx.PrepareToReceive(m_socket.ReceiveBufferSize);
		}

		static bool ReadData()
		{
			MemoryStream ms = m_pkt_rx.ms;
			SocketError err;
			long size = ms.Length - ms.Position;

			int received_bytes = m_socket.Receive(ms.GetBuffer(), (int)ms.Position, (int)size, SocketFlags.None, out err);

			if (err == SocketError.WouldBlock) // no data in RX buffer
				return true; // still connected

			if (err != SocketError.Success) // TODO: switch error types
			{
				log.Warn("ReadData error {0}", err.ToString());
				return false; // disconnected
			}

			log.Trace("ReadData len: {0}", received_bytes);

			if (received_bytes == 0) // disconnected
			{
				log.Debug("Remote peer closed connection");
				return false; // disconnected
			}

			ms.Position = ms.Position + received_bytes;

			// circle buffer

			while (true)
			{
				int pkt_full_len;

				// incomplete packet received
				if (!m_pkt_rx.TryParseLength(out pkt_full_len))
					return true; // still connected

				// incomplete packet received
				if (pkt_full_len > ms.Position)
				{
					if (pkt_full_len > ms.Length)
						ms.SetLength(pkt_full_len); // resize buffer

					return true; // still connected
				}

				// 1 full packet at buffer
				if (pkt_full_len == ms.Position)
				{
					Parse(m_pkt_rx);
					ResetRXPkt();

					return true; // still connected
				}

				// more than 1 full packet received
				// копируем лишние данные в новый пакет

				int tailLength = (int)ms.Position - pkt_full_len;
				var next_pkt = new NetPkt(tailLength); // TODO: set size to MAX(next pkt data size, ReceiveBufferSize)
				next_pkt.ms.SetLength(tailLength);

				Buffer.BlockCopy(ms.GetBuffer(), pkt_full_len, next_pkt.ms.GetBuffer(), 0, tailLength);
				next_pkt.ms.Position = tailLength;

				Parse(m_pkt_rx);

				m_pkt_rx = next_pkt;
				ms = m_pkt_rx.ms;
			}
		}

		static void Parse(NetPkt pkt)
		{
			// set for read by consumer
			pkt.PrepareToRead();

			onMessage?.Invoke(pkt.br.ReadString());

			// reset packet

			pkt.ms.Position = 0;
			pkt.ms.SetLength(pkt.ms.Capacity);
		}

		static bool WriteData()
		{
			if (m_pkt_tx == null)
				return true; // still connected

			MemoryStream ms = m_pkt_tx.ms;
			SocketError err;
			long size = ms.Length - ms.Position;

			log.Trace("WriteData len: {0}", size);

			int sended_bytes = m_socket.Send(ms.GetBuffer(), (int)ms.Position, (int)size, SocketFlags.None, out err);

			if (err == SocketError.WouldBlock) // cannot send (TX buffer full)
			{
				log.Warn("WriteData - cannot send - buffer full");
				return true; // still connected
			}

			if (err != SocketError.Success) // TODO: switch error types
			{
				log.Warn("WriteData error {0}", err.ToString());
				return false; // disconnected
			}

			log.Trace("WriteData len: {0}/{1}", sended_bytes, size);

			ms.Position += sended_bytes;

			if (ms.Position == ms.Length) // all data sent
				m_pkt_tx = null;

			return true;
		}

		static void SendRequest(string request, string body)
		{
			log.Debug("SendRequest: '{0}' '{1}'", request, body);

			NetPkt pkt = new NetPkt(m_socket.SendBufferSize);
			pkt.PrepareToWrite();

			pkt.bw.Write(request);
			pkt.bw.Write(body);

			pkt.PrepareToSend();

			if (m_pkt_tx != null)
				m_pkt_tx.Append(pkt);
			else
				m_pkt_tx = pkt;

			WriteData();
		}

		static NetPkt m_pkt_rx;
		static NetPkt m_pkt_tx;

		static Socket m_socket;

		static LibCSharp.Logger log = LibCSharp.Logger.GetCurrentClassLogger();
	}
}