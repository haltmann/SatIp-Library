/*  
    Copyright (C) <2007-2014>  <Kay Diefenthal>

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
using System.Collections.Generic;
using System.Linq;

namespace SatIp.Library.Rtcp
{
    public class RtcpPacketSourceDescription : RtcpPacket
    {
        #region Fields

        private int _version = 2;
        private readonly List<RtcpPacketSourceDescriptionChunk> _rtcpSourceDescriptionsChunks;

        #endregion

        #region Constructor

        internal RtcpPacketSourceDescription()
        {
            _rtcpSourceDescriptionsChunks = new List<RtcpPacketSourceDescriptionChunk>();
        }

        #endregion

        #region Overrides
        
        protected override void ParseInternal(byte[] buffer, ref int offset)
        {
            _version = buffer[offset] >> 6;
            bool isPadded = Convert.ToBoolean((buffer[offset] >> 5) & 0x1);
            int sourceCount = buffer[offset++] & 0x1F;
            int type = buffer[offset++];
            int length = buffer[offset++] << 8 | buffer[offset++];
            if (isPadded)
            {
                PaddBytesCount = buffer[offset + length];
            }
            for (int i = 0; i < sourceCount; i++)
            {
                var chunk = new RtcpPacketSourceDescriptionChunk();
                chunk.Parse(buffer, ref offset);
                _rtcpSourceDescriptionsChunks.Add(chunk);
            }
        }

        public override void ToByte(byte[] buffer, ref int offset)
        {
            buffer[offset++] = (byte)(2 << 6 | 0 << 5 | _rtcpSourceDescriptionsChunks.Count & 0x1F);
            buffer[offset++] = 202;
            int lengthOffset = offset;
            buffer[offset++] = 0; 
            buffer[offset++] = 0; 
            int chunksStartOffset = offset;
            foreach (RtcpPacketSourceDescriptionChunk chunk in _rtcpSourceDescriptionsChunks)
            {
                chunk.ToByte(buffer, ref offset);
            }
            int length = (offset - chunksStartOffset) / 4;
            buffer[lengthOffset] = (byte)((length >> 8) & 0xFF);
            buffer[lengthOffset + 1] = (byte)((length) & 0xFF);
        }

        #endregion

        #region Properties

        public override int Version
        {
            get { return _version; }
        }
        public override RtcpPacketType Type
        {
            get { return RtcpPacketType.SourceDescription; }
        }
        public List<RtcpPacketSourceDescriptionChunk> Chunks
        {
            get { return _rtcpSourceDescriptionsChunks; }
        }
        public override int Size
        {
            get
            {
                return 4 + _rtcpSourceDescriptionsChunks.Sum(chunk => chunk.Size);
            }
        }
        #endregion
    }
}
