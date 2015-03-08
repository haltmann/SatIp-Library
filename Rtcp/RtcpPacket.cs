/*  
    Copyright (C) <2007-2015>  <Kay Diefenthal>

    SatIp.Library is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    SatIp.Library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with SatIp.Library.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;

namespace SatIp.Library.Rtcp
{
    public abstract class RtcpPacket
    {
        #region Fields

        private int _paddingBytesCount;

        #endregion

        #region Statics
        
        public static RtcpPacket Parse(byte[] buffer,ref int offset)
        {
            return Parse(buffer, ref offset,false);
        }
       
        public static RtcpPacket Parse(byte[] buffer,ref int offset,bool noException)
        {
            if(buffer == null){
                throw new ArgumentNullException("buffer");
            }
            if(offset < 0){
                throw new ArgumentException("Argument 'offset' value must be >= 0.");
            }
            var type =(RtcpPacketType)buffer[offset + 1];
            if (type == RtcpPacketType.SenderReport)
            {
                var packet = new RtcpPacketSenderReport();
                packet.ParseInternal(buffer,ref offset);
                return packet;
            }
            if(type == RtcpPacketType.ReceiverReport)
            {
                var packet = new RtcpPacketReceiverReport();
                packet.ParseInternal(buffer,ref offset);
                return packet;
            }
            if(type == RtcpPacketType.SourceDescription)
            {
                var packet = new RtcpPacketSourceDescription();
                packet.ParseInternal(buffer,ref offset);
                return packet;
            }
            if(type == RtcpPacketType.GoodBye)
            {
                var packet = new RtcpPacketGoodbye();
                packet.ParseInternal(buffer,ref offset);
                return packet;
            }
            if(type == RtcpPacketType.ApplicationDefined)
            {
                var packet = new RtcpPacketApplicationDefined();
                packet.ParseInternal(buffer,ref offset);
                return packet;
            }
            offset += 2;
            int length = buffer[offset++] << 8 | buffer[offset++];
            offset += length;
            if(noException)
            {
                return null;
            }
            throw new ArgumentException("Unknown RTCP packet type '" + type + "'.");
        }

        #endregion

        #region Abstracts

        public abstract void ToByte(byte[] buffer,ref int offset);
       
        protected abstract void ParseInternal(byte[] buffer,ref int offset);

        #endregion

        #region Properties

        public abstract int Version
        {
            get;
        }

        public bool IsPadded
        {
            get
            {
                if(_paddingBytesCount > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public abstract RtcpPacketType Type
        {
            get;
        }

        public int PaddBytesCount
        {
            get { return _paddingBytesCount; }

            set{
                if(value < 0){
                    throw new ArgumentException("Property 'PaddBytesCount' value must be >= 0.");
                }

                _paddingBytesCount = value;
            }            
        }

        public abstract int Size
        {
            get;
        }

        #endregion
    }
}
