using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Net.IO
{
    class PacketBuilder
    {
        private readonly MemoryStream ms;

        public PacketBuilder()
        {
            ms = new MemoryStream();

        }

        public void WriteOpCode(byte opcode)
        {
            ms.WriteByte(opcode);
        }

        public void WriteMessage(string msg)
        {
            ms.Write(BitConverter.GetBytes(msg.Length));
            ms.Write(Encoding.ASCII.GetBytes(msg));
        }

        public byte[] GetPacketBytes()
        {
            return ms.ToArray();
        }
    }
}
