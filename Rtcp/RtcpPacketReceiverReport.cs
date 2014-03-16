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
using System.Text;

namespace SatIp.Library.Rtcp
{
    public class RtcpPacketReceiverReport : RtcpPacket
    {
        #region Fields

        private int _version = 2;
        private uint _senderSource;
        private readonly List<RtcpPacketReportBlock> _rtcpReportBlocks;

        #endregion

        #region Constructor

        internal RtcpPacketReceiverReport()
        {
            _rtcpReportBlocks = new List<RtcpPacketReportBlock>();
        }

        #endregion

        #region Overrides

        protected override void ParseInternal(byte[] buffer, ref int offset)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (offset < 0)
            {
                throw new ArgumentException("Argument 'offset' value must be >= 0.");
            }
            _version = buffer[offset] >> 6;
            bool isPadded = Convert.ToBoolean((buffer[offset] >> 5) & 0x1);
            int reportBlockCount = buffer[offset++] & 0x1F;
            int type = buffer[offset++];
            int length = buffer[offset++] << 8 | buffer[offset++];
            if (isPadded)
            {
                PaddBytesCount = buffer[offset + length];
            }
            _senderSource = (uint)(buffer[offset++] << 24 | buffer[offset++] << 16 | buffer[offset++] << 8 | buffer[offset++]);
            for (int i = 0; i < reportBlockCount; i++)
            {
                var reportBlock = new RtcpPacketReportBlock();
                reportBlock.Parse(buffer, offset);
                _rtcpReportBlocks.Add(reportBlock);
                offset += 24;
            }
        }
        public override void ToByte(byte[] buffer, ref int offset)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (offset < 0)
            {
                throw new ArgumentException("Argument 'offset' value must be >= 0.");
            }
            int length = (4 + (_rtcpReportBlocks.Count * 24)) / 4;
            buffer[offset++] = (byte)(2 << 6 | 0 << 5 | (_rtcpReportBlocks.Count & 0x1F));
            buffer[offset++] = 201;
            buffer[offset++] = (byte)((length >> 8) & 0xFF);
            buffer[offset++] = (byte)((length) & 0xFF);
            buffer[offset++] = (byte)((_senderSource >> 24) & 0xFF);
            buffer[offset++] = (byte)((_senderSource >> 16) & 0xFF);
            buffer[offset++] = (byte)((_senderSource >> 8) & 0xFF);
            buffer[offset++] = (byte)((_senderSource) & 0xFF);
            foreach (var block in _rtcpReportBlocks)
            {
                block.ToByte(buffer, ref offset);
            }
        }
        public override string ToString()
        {
            var retVal = new StringBuilder();
            retVal.AppendLine("Type: RR");
            retVal.AppendLine("Version: " + _version);
            retVal.AppendLine("SSRC: " + _senderSource);
            retVal.AppendLine("Report blocks: " + _rtcpReportBlocks.Count);

            return retVal.ToString();
        }

        #endregion

        #region Properties
        
        public override int Version
        {
            get { return _version; }
        }
        public override RtcpPacketType Type
        {
            get { return RtcpPacketType.ReceiverReport; }
        }
        public uint SenderSource
        {
            get { return _senderSource; }

            set { _senderSource = value; }
        }
        public List<RtcpPacketReportBlock> ReportBlocks
        {
            get { return _rtcpReportBlocks; }
        }
        public override int Size
        {
            get { return 8 + (24 * _rtcpReportBlocks.Count); }
        }

        #endregion
    }
}
