using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatClient.Net.IO
{
    public class PacketBuilder
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
