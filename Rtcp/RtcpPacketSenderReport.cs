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

namespace SatIp.Library.Rtcp
{
    public class RtcpPacketSenderReport : RtcpPacket
    {
        #region Fields

        private uint _senderOctetCount;
        private ulong _ntpTimestamp;
        private int _version;
        private uint _senderSource;
        private uint _rtpTimestamp;
        private uint _senderPacketCount;
        private readonly List<RtcpPacketReportBlock> _rtcpReportBlocks;

        public RtcpPacketSenderReport()
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
            _ntpTimestamp = (ulong)(buffer[offset++] << 56 | buffer[offset++] << 48 | buffer[offset++] << 40 | buffer[offset++] << 32 | buffer[offset++] << 24 | buffer[offset++] << 16 | buffer[offset++] << 8 | buffer[offset++]);
            _rtpTimestamp = (uint)(buffer[offset++] << 24 | buffer[offset++] << 16 | buffer[offset++] << 8 | buffer[offset++]);
            _senderPacketCount = (uint)(buffer[offset++] << 24 | buffer[offset++] << 16 | buffer[offset++] << 8 | buffer[offset++]);
            _senderOctetCount = (uint)(buffer[offset++] << 24 | buffer[offset++] << 16 | buffer[offset++] << 8 | buffer[offset++]);
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
            int length = (24 + (_rtcpReportBlocks.Count * 24)) / 4;
            buffer[offset++] = (byte)(2 << 6 | 0 << 5 | (_rtcpReportBlocks.Count & 0x1F));
            buffer[offset++] = 200;
            buffer[offset++] = (byte)((length >> 8) & 0xFF);
            buffer[offset++] = (byte)((length) & 0xFF);
            buffer[offset++] = (byte)((_senderSource >> 24) & 0xFF);
            buffer[offset++] = (byte)((_senderSource >> 16) & 0xFF);
            buffer[offset++] = (byte)((_senderSource >> 8) & 0xFF);
            buffer[offset++] = (byte)((_senderSource) & 0xFF);
            buffer[offset++] = (byte)((_ntpTimestamp >> 56) & 0xFF);
            buffer[offset++] = (byte)((_ntpTimestamp >> 48) & 0xFF);
            buffer[offset++] = (byte)((_ntpTimestamp >> 40) & 0xFF);
            buffer[offset++] = (byte)((_ntpTimestamp >> 32) & 0xFF);
            buffer[offset++] = (byte)((_ntpTimestamp >> 24) & 0xFF);
            buffer[offset++] = (byte)((_ntpTimestamp >> 16) & 0xFF);
            buffer[offset++] = (byte)((_ntpTimestamp >> 8) & 0xFF);
            buffer[offset++] = (byte)((_ntpTimestamp) & 0xFF);
            buffer[offset++] = (byte)((_rtpTimestamp >> 24) & 0xFF);
            buffer[offset++] = (byte)((_rtpTimestamp >> 16) & 0xFF);
            buffer[offset++] = (byte)((_rtpTimestamp >> 8) & 0xFF);
            buffer[offset++] = (byte)((_rtpTimestamp) & 0xFF);
            buffer[offset++] = (byte)((_senderPacketCount >> 24) & 0xFF);
            buffer[offset++] = (byte)((_senderPacketCount >> 16) & 0xFF);
            buffer[offset++] = (byte)((_senderPacketCount >> 8) & 0xFF);
            buffer[offset++] = (byte)((_senderPacketCount) & 0xFF);
            buffer[offset++] = (byte)((_senderOctetCount >> 24) & 0xFF);
            buffer[offset++] = (byte)((_senderOctetCount >> 16) & 0xFF);
            buffer[offset++] = (byte)((_senderOctetCount >> 8) & 0xFF);
            buffer[offset++] = (byte)((_senderOctetCount) & 0xFF);
            foreach (var block in _rtcpReportBlocks)
            {
                block.ToByte(buffer, ref offset);
            }
        }

        #endregion

        #region Properties
        public override int Version
        {
            get { return _version; }
        }
        public override RtcpPacketType Type
        {
            get { return RtcpPacketType.SenderReport; }
        }
        public uint SenderSource
        {
            get { return _senderSource; }
        }
        public ulong NtpTimestamp
        {
            get { return _ntpTimestamp; }

            set { _ntpTimestamp = value; }
        }
        public uint RtpTimestamp
        {
            get { return _rtpTimestamp; }

            set { _rtpTimestamp = value; }
        }
        public uint SenderPacketCount
        {
            get { return _senderPacketCount; }

            set { _senderPacketCount = value; }
        }
        public uint SenderOctetCount
        {
            get { return _senderOctetCount; }

            set { _senderOctetCount = value; }
        }
        public List<RtcpPacketReportBlock> ReportBlocks
        {
            get { return _rtcpReportBlocks; }
        }
        public override int Size
        {
            get { return 28 + (24 * _rtcpReportBlocks.Count); }
        }
        #endregion
    }
}
