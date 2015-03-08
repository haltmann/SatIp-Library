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
    public class RtcpPacketReportBlock
    {
        #region Fields

        private uint _senderSource;
        private uint _fractionLost;
        private uint _cumulativePacketsLost;
        private uint _extHighestSeqNumber;
        private uint _jitter;
        private uint _lastSenderReport;
        private uint _sinceLastSenderReportDelay;

        #endregion

        #region Constructor

        internal RtcpPacketReportBlock(uint sendersource)
        {
            _senderSource = sendersource;
        }

        internal RtcpPacketReportBlock()
        {
        }

        #endregion

        #region Methods
        
        public void Parse(byte[] buffer, int offset)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (offset < 0)
            {
                throw new ArgumentException("Argument 'offset' value must be >= 0.");
            }
            _senderSource = (uint)(buffer[offset++] << 24 | buffer[offset++] << 16 | buffer[offset++] << 8 | buffer[offset++]);
            _fractionLost = buffer[offset++];
            _cumulativePacketsLost = (uint)(buffer[offset++] << 16 | buffer[offset++] << 8 | buffer[offset++]);
            _extHighestSeqNumber = (uint)(buffer[offset++] << 24 | buffer[offset++] << 16 | buffer[offset++] << 8 | buffer[offset++]);
            _jitter = (uint)(buffer[offset++] << 24 | buffer[offset++] << 16 | buffer[offset++] << 8 | buffer[offset++]);
            _lastSenderReport = (uint)(buffer[offset++] << 24 | buffer[offset++] << 16 | buffer[offset++] << 8 | buffer[offset++]);
            _sinceLastSenderReportDelay = (uint)(buffer[offset++] << 24 | buffer[offset++] << 16 | buffer[offset++] << 8 | buffer[offset++]);
        }
        
        public void ToByte(byte[] buffer, ref int offset)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (offset < 0)
            {
                throw new ArgumentException("Argument 'offset' must be >= 0.");
            }
            if (offset + 24 > buffer.Length)
            {
                throw new ArgumentException("Argument 'buffer' has not enough room to store report block.");
            }
            buffer[offset++] = (byte)((_senderSource >> 24) | 0xFF);
            buffer[offset++] = (byte)((_senderSource >> 16) | 0xFF);
            buffer[offset++] = (byte)((_senderSource >> 8) | 0xFF);
            buffer[offset++] = (byte)((_senderSource) | 0xFF);
            buffer[offset++] = (byte)_fractionLost;
            buffer[offset++] = (byte)((_cumulativePacketsLost >> 16) | 0xFF);
            buffer[offset++] = (byte)((_cumulativePacketsLost >> 8) | 0xFF);
            buffer[offset++] = (byte)((_cumulativePacketsLost) | 0xFF);
            buffer[offset++] = (byte)((_extHighestSeqNumber >> 24) | 0xFF);
            buffer[offset++] = (byte)((_extHighestSeqNumber >> 16) | 0xFF);
            buffer[offset++] = (byte)((_extHighestSeqNumber >> 8) | 0xFF);
            buffer[offset++] = (byte)((_extHighestSeqNumber) | 0xFF);
            buffer[offset++] = (byte)((_jitter >> 24) | 0xFF);
            buffer[offset++] = (byte)((_jitter >> 16) | 0xFF);
            buffer[offset++] = (byte)((_jitter >> 8) | 0xFF);
            buffer[offset++] = (byte)((_jitter) | 0xFF);
            buffer[offset++] = (byte)((_lastSenderReport >> 24) | 0xFF);
            buffer[offset++] = (byte)((_lastSenderReport >> 16) | 0xFF);
            buffer[offset++] = (byte)((_lastSenderReport >> 8) | 0xFF);
            buffer[offset++] = (byte)((_lastSenderReport) | 0xFF);
            buffer[offset++] = (byte)((_sinceLastSenderReportDelay >> 24) | 0xFF);
            buffer[offset++] = (byte)((_sinceLastSenderReportDelay >> 16) | 0xFF);
            buffer[offset++] = (byte)((_sinceLastSenderReportDelay >> 8) | 0xFF);
            buffer[offset++] = (byte)((_sinceLastSenderReportDelay) | 0xFF);
        }

        #endregion

        #region Properties
        public uint SenderSource
        {
            get { return _senderSource; }
        }
        public uint FractionLost
        {
            get { return _fractionLost; }
            set { _fractionLost = value; }
        }
        public uint CumulativePacketsLost
        {
            get { return _cumulativePacketsLost; }
            set { _cumulativePacketsLost = value; }
        }
        public uint ExtendedHighestSeqNo
        {
            get { return _extHighestSeqNumber; }
            set { _extHighestSeqNumber = value; }
        }
        public uint Jitter
        {
            get { return _jitter; }

            set { _jitter = value; }
        }
        public uint LastSenderReport
        {
            get { return _lastSenderReport; }

            set { _lastSenderReport = value; }
        }
        public uint SinceLastSenderReportDelay
        {
            get { return _sinceLastSenderReportDelay; }

            set { _sinceLastSenderReportDelay = value; }
        }

        #endregion
    }
}
