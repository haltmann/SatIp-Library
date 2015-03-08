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
using System.Text;

namespace SatIp.Library.Rtcp
{
    public class RtcpPacketGoodbye : RtcpPacket
    {
        #region Fields

        private int _version = 2;
        private uint[] _sources;
        private string _leavingReason = "";

        #endregion

        #region Constructor

        internal RtcpPacketGoodbye()
        {
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
            int sourceCount = buffer[offset++] & 0x1F;
            int type = buffer[offset++];
            int length = buffer[offset++] << 8 | buffer[offset++];
            if (isPadded)
            {
                PaddBytesCount = buffer[offset + length];
            }
            _sources = new uint[sourceCount];
            for (int i = 0; i < sourceCount; i++)
            {
                _sources[i] = (uint)(buffer[offset++] << 24 | buffer[offset++] << 16 | buffer[offset++] << 8 | buffer[offset++]);
            }
            if (length > _sources.Length * 4)
            {
                int reasonLength = buffer[offset++];
                _leavingReason = Encoding.UTF8.GetString(buffer, offset, reasonLength);
                offset += reasonLength;
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
            int length = 0;
            length += _sources.Length * 4;
            if (!string.IsNullOrEmpty(_leavingReason))
            {
                length++;
                length += Encoding.UTF8.GetByteCount(_leavingReason);
            }
            buffer[offset++] = (byte)(2 << 6 | 0 << 5 | _sources.Length & 0x1F);
            buffer[offset++] = 203;
            buffer[offset++] = (byte)((length >> 8) & 0xFF);
            buffer[offset++] = (byte)(length & 0xFF);
            foreach (var source in _sources)
            {
                buffer[offset++] = (byte)((source & 0xFF000000) >> 24);
                buffer[offset++] = (byte)((source & 0x00FF0000) >> 16);
                buffer[offset++] = (byte)((source & 0x0000FF00) >> 8);
                buffer[offset++] = (byte)((source & 0x000000FF));
            }
            if (!string.IsNullOrEmpty(_leavingReason))
            {
                byte[] reasonBytes = Encoding.UTF8.GetBytes(_leavingReason);
                buffer[offset++] = (byte)reasonBytes.Length;
                Array.Copy(reasonBytes, 0, buffer, offset, reasonBytes.Length);
                offset += reasonBytes.Length;
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
            get { return RtcpPacketType.GoodBye; }
        }
        public uint[] Sources
        {
            get { return _sources; }
            set
            {
                if (value.Length > 31)
                {
                    throw new ArgumentException("Property 'Sources' can accomodate only 31 entries.");
                }
                _sources = value;
            }
        }
        public string LeavingReason
        {
            get { return _leavingReason; }

            set { _leavingReason = value; }
        }
        public override int Size
        {
            get
            {
                int size = 4;
                if (_sources != null)
                {
                    size += 4 * _sources.Length;
                }
                if (!string.IsNullOrEmpty(_leavingReason))
                {
                    size++;
                    size += Encoding.UTF8.GetByteCount(_leavingReason);
                }
                return size;
            }
        }
        #endregion
    }
}
