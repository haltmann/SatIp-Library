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
using System.Globalization;
using System.Text;

namespace SatIp.Library.Rtp
{
    /// <summary>
    /// A data packet consisting of the fixed RTP header, a possibly empty list of contributing 
    /// sources (see below), and the payload data. Some underlying protocols may require an 
    /// encapsulation of the RTP packet to be defined. Typically one packet of the underlying 
    /// protocol contains a single RTP packet, but several RTP packets MAY be contained if 
    /// permitted by the encapsulation method (see Section 11).
    /// </summary>
    public class RtpPacket
    {
        #region Fields

        private int _version = 2;
        private bool _marker;
        private int _payloadType;
        private ushort _sequenceNumber;
        private uint _timestamp;
        private uint _senderSource;
        private uint[] _contributingSources;
        private byte[] _data;

        #endregion

        #region Statics

        /// <summary>
        /// Parses RTP packet.
        /// </summary>
        /// <param name="buffer">Buffer containing RTP packet.</param>
        /// <param name="size">Number of bytes used in buffer.</param>
        /// <returns>Returns parsed RTP packet.</returns>
        public static RtpPacket Parse(byte[] buffer, int size)
        {
            var packet = new RtpPacket();
            packet.ParseInternal(buffer, size);
            return packet;
        }

        #endregion

        #region Overrides 

        /// <summary>
        /// Returns this packet info as string.
        /// </summary>
        /// <returns>Returns packet info.</returns>
        public override string ToString()
        {
            var retVal = new StringBuilder();
            retVal.Append("----- RTP Packet\r\n");
            retVal.Append("Version: " + _version.ToString(CultureInfo.InvariantCulture) + "\r\n");
            retVal.Append("IsMaker: " + _marker.ToString() + "\r\n");
            retVal.Append("PayloadType: " + _payloadType.ToString(CultureInfo.InvariantCulture) + "\r\n");
            retVal.Append("SeqNo: " + _sequenceNumber.ToString(CultureInfo.InvariantCulture) + "\r\n");
            retVal.Append("Timestamp: " + _timestamp.ToString(CultureInfo.InvariantCulture) + "\r\n");
            retVal.Append("SSRC: " + _senderSource.ToString(CultureInfo.InvariantCulture) + "\r\n");
            retVal.Append("Data: " + _data.Length + " bytes.\r\n");
            return retVal.ToString();
        }

        #endregion

        #region Methods 

        /// <summary>
        /// Parses RTP packet from the specified buffer.
        /// </summary>
        /// <param name="buffer">Buffer containing RTP packet.</param>
        /// <param name="size">Number of bytes used in buffer.</param>
        private void ParseInternal(byte[] buffer, int size)
        {
            int offset = 0;
            _version = buffer[offset] >> 6;
            bool isPadded = Convert.ToBoolean((buffer[offset] >> 5) & 0x1);
            bool hasExtention = Convert.ToBoolean((buffer[offset] >> 4) & 0x1);
            int csrcCount = buffer[offset++] & 0xF;
            _marker = Convert.ToBoolean(buffer[offset] >> 7);
            _payloadType = buffer[offset++] & 0x7F;
            _sequenceNumber = (ushort)(buffer[offset++] << 8 | buffer[offset++]);
            _timestamp =(uint) (buffer[offset++] << 24 | buffer[offset++] << 16 | buffer[offset++] << 8 | buffer[offset++]);
            _senderSource = (uint)(buffer[offset++] << 24 | buffer[offset++] << 16 | buffer[offset++] << 8 | buffer[offset++]);
            _contributingSources = new uint[csrcCount];
            for (int i = 0; i < csrcCount; i++)
            {
                _contributingSources[i] = (uint)(buffer[offset++] << 24 | buffer[offset++] << 16 | buffer[offset++] << 8 | buffer[offset++]);
            }
            if (hasExtention)
            {
                offset++;
                offset += buffer[offset];
            }
            _data = new byte[size - offset];
            Array.Copy(buffer, offset, _data, 0, _data.Length);
        }
        /// <summary>
        /// Stores this packet to the specified buffer.
        /// </summary>
        /// <param name="buffer">Buffer where to store packet.</param>
        /// <param name="offset">Offset in buffer.</param>
        public void ToByte(byte[] buffer, ref int offset)
        {
            int cc = 0;
            if (_contributingSources != null)
            {
                cc = _contributingSources.Length;
            }
            buffer[offset++] = (byte)(_version << 6 | 0 << 5 | cc & 0xF);
            buffer[offset++] = (byte)(Convert.ToInt32(_marker) << 7 | _payloadType & 0x7F);
            buffer[offset++] = (byte)(_sequenceNumber >> 8);
            buffer[offset++] = (byte)(_sequenceNumber & 0xFF);
            buffer[offset++] = (byte)((_timestamp >> 24) & 0xFF);
            buffer[offset++] = (byte)((_timestamp >> 16) & 0xFF);
            buffer[offset++] = (byte)((_timestamp >> 8) & 0xFF);
            buffer[offset++] = (byte)(_timestamp & 0xFF);
            buffer[offset++] = (byte)((_senderSource >> 24) & 0xFF);
            buffer[offset++] = (byte)((_senderSource >> 16) & 0xFF);
            buffer[offset++] = (byte)((_senderSource >> 8) & 0xFF);
            buffer[offset++] = (byte)(_senderSource & 0xFF);
            if (_contributingSources != null)
            {
                foreach (var contributingSource in _contributingSources)
                {
                    buffer[offset++] = (byte)((contributingSource >> 24) & 0xFF);
                    buffer[offset++] = (byte)((contributingSource >> 16) & 0xFF);
                    buffer[offset++] = (byte)((contributingSource >> 8) & 0xFF);
                    buffer[offset++] = (byte)(contributingSource & 0xFF);
                }
            }
            Array.Copy(_data, 0, buffer, offset, _data.Length);
            offset += _data.Length;
        }
        #endregion

        #region Properties 

        /// <summary>
        /// Gets RTP version.
        /// </summary>
        public int Version
        {
            get { return _version; }
        }

        /// <summary>
        /// Gets if packet is padded to some bytes boundary.
        /// </summary>
        public bool IsPadded
        {
            get { return false; }
        }

        /// <summary>
        /// Gets marker bit. The usage of this bit depends on payload type.
        /// </summary>
        public bool Marker
        {
            get { return _marker; }
            set { _marker = value; }
        }

        /// <summary>
        /// Gets payload type.
        /// </summary>
        /// <exception cref="ArgumentException">Is raised when invalid value is passed.</exception>
        public int PayloadType
        {
            get { return _payloadType; }
            set
            {
                if (value < 0 || value > 128)
                {
                    throw new ArgumentException("Payload value must be >= 0 and <= 128.");
                }
                _payloadType = value;
            }
        }

        /// <summary>
        /// Gets or sets RTP packet sequence number.
        /// </summary>
        /// <exception cref="ArgumentException">Is raised when invalid value is passed.</exception>
        public ushort SequenceNumber
        {
            get { return _sequenceNumber; }
            set { _sequenceNumber = value; }
        }

        /// <summary>
        /// Gets sets packet timestamp. 
        /// </summary>
        /// <exception cref="ArgumentException">Is raised when invalid value is passed.</exception>
        public uint Timestamp
        {
            get { return _timestamp; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("Timestamp value must be >= 1.");
                }
                _timestamp = value;
            }
        }

        /// <summary>
        /// Gets or sets synchronization source ID.
        /// </summary>
        /// <exception cref="ArgumentException">Is raised when invalid value is passed.</exception>
        public uint SenderSource
        {
            get { return _senderSource; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("SSRC value must be >= 1.");
                }
                _senderSource = value;
            }
        }

        /// <summary>
        /// Gets or sets the scontributing source for the payload contained in this packet.
        /// Value null means none.
        /// </summary>
        public uint[] ContributingSources
        {
            get { return _contributingSources; }
            set { _contributingSources = value; }
        }

        /// <summary>
        /// Gets SSRC + CSRCs as joined array.
        /// </summary>
        public uint[] Sources
        {
            get
            {
                var retVal = new uint[1];
                if (_contributingSources != null)
                {
                    retVal = new uint[1 + _contributingSources.Length];
                }
                retVal[0] = _senderSource;
                Array.Copy(_contributingSources, retVal, _contributingSources.Length);
                return retVal;
            }
        }

        /// <summary>
        /// Gets or sets RTP data. Data must be encoded with PayloadType encoding.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised when null value is passed.</exception>
        public byte[] Data
        {
            get { return _data; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Data");
                }
                _data = value;
            }
        }

        #endregion
    }
}
