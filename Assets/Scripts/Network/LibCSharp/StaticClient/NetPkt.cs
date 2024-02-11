using System;
using System.Collections.Generic;
using System.IO;

namespace LibCSharp.Network.ClientStatic
{
	static partial class TCPPktClient
	{
		// служебный класс NetPkt скрыт внутри TCPPktClient, что бы не загрязнять интерфейс реализацией
		class NetPkt
		{
			const int HDR_DATA_SIZE_LEN = 3;
			const int MAX_DATA_LEN = 256 * 256 * 256;

			internal readonly MemoryStream ms;
			internal readonly BinaryReader br;
			internal readonly BinaryWriter bw;

			internal NetPkt(int size = 0)
			{
				ms = size > 0 ? new MemoryStream(size) : new MemoryStream();
				br = new BinaryReader(ms);
				bw = new BinaryWriter(ms);
			}

			internal void PrepareToReceive(int size)
			{
				ms.Position = 0;
				ms.SetLength(size);
			}

			internal void PrepareToSend()
			{
				ms.Position = 0;
				var len = ms.Length - HDR_DATA_SIZE_LEN;
				if (len > MAX_DATA_LEN)
					throw new Exception(string.Format("Maximum outgoing pkt size exceeded: {0} > {1}", len, MAX_DATA_LEN));

				ms.WriteByte((byte)len);
				ms.WriteByte((byte)(len >> 8));
				ms.WriteByte((byte)(len >> 16));

				ms.Position = 0;
			}

			internal void PrepareToRead()
			{
				ms.SetLength(ms.Position);
				ms.Position = HDR_DATA_SIZE_LEN;
			}

			internal void PrepareToWrite()
			{
				ms.SetLength(HDR_DATA_SIZE_LEN);
				ms.Position = HDR_DATA_SIZE_LEN;
			}

			/// <summary>Merge another outgoing packet</summary>
			internal void Append(NetPkt pkt2)
			{
				int old_len = (int)ms.Length;
				ms.SetLength(old_len + pkt2.ms.Length);
				Buffer.BlockCopy(pkt2.ms.GetBuffer(), 0, ms.GetBuffer(), old_len, (int)pkt2.ms.Length);
			}

			internal bool TryParseLength(out int full_length)
			{
				if (ms.Position >= HDR_DATA_SIZE_LEN)
				{
					long old_pos = ms.Position;
					ms.Position = 0;
					full_length = (ms.ReadByte() | (ms.ReadByte() << 8) | (ms.ReadByte() << 16)) + HDR_DATA_SIZE_LEN;
					ms.Position = old_pos;
					return true;
				}

				full_length = 0;
				return false;
			}
		}
	}
}