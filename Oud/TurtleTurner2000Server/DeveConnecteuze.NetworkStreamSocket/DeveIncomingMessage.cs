using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeveConnecteuze.Network
{
    public class DeveIncomingMessageStreamSocket
    {
        private byte[] bytes;
        private int m_readPosition = 0;

        private DeveConnectionStreamSocket sender;
        public DeveConnectionStreamSocket Sender
        {
            get { return sender; }
        }

        private DeveMessageTypeStreamSocket messageType;

        public DeveMessageTypeStreamSocket MessageType
        {
            get { return messageType; }
        }

        public DeveIncomingMessageStreamSocket(DeveConnectionStreamSocket sender, byte[] bytes)
        {
            this.bytes = bytes;
            this.sender = sender;
            messageType = (DeveMessageTypeStreamSocket)ReadByte();
        }

        public byte ReadByte()
        {
            byte retval = bytes[m_readPosition];
            m_readPosition += 1;
            return retval;
        }

        public Int32 ReadInt32()
        {
            Int32 retval = BitConverter.ToInt32(bytes, m_readPosition);
            m_readPosition += 4;
            return retval;
        }

        public String ReadString()
        {
            int stringLength = ReadInt32();
            ASCIIEncoding encoder = new ASCIIEncoding();
            String retval = encoder.GetString(bytes, m_readPosition, stringLength);
            m_readPosition += stringLength;
            return retval;
        }

        public byte[] ReadBytes(int amount)
        {
            byte[] retval = new byte[amount];
            Array.Copy(bytes, m_readPosition, retval, 0, amount);
            m_readPosition += amount;
            return retval;
        }
    }
}
