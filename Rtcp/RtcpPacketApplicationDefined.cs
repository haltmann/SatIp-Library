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
using System.Globalization;

namespace SatIp.Library.Rtcp
{
    public class RtcpPacketApplicationDefined : RtcpPacket
    {
        #region Fields

        private int _version = 2;
        private int _subType;
        private uint _source;
        private string _name = "";
        private byte[] _data;

        #endregion

        #region Constructor

        internal RtcpPacketApplicationDefined()
        {
            _name = "xxxx";
            _data = new byte[0];
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

            _version = buffer[offset++] >> 6;
            bool isPadded = Convert.ToBoolean((buffer[offset] >> 5) & 0x1);
            int subType = buffer[offset++] & 0x1F;
            int type = buffer[offset++];
            int length = buffer[offset++] << 8 | buffer[offset++];
            if (isPadded)
            {
                PaddBytesCount = buffer[offset + length];
            }

            _subType = subType;
            _source = (uint)(buffer[offset++] << 24 | buffer[offset++] << 16 | buffer[offset++] << 8 | buffer[offset++]);
            _name = ((char)buffer[offset++]).ToString(CultureInfo.InvariantCulture) + ((char)buffer[offset++]).ToString(CultureInfo.InvariantCulture) + ((char)buffer[offset++]).ToString(CultureInfo.InvariantCulture) + ((char)buffer[offset++]).ToString(CultureInfo.InvariantCulture);
            _data = new byte[length - 8];
            Array.Copy(buffer, offset, _data, 0,_data.Length);
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
            int length = 8 + _data.Length;
            buffer[offset++] = (byte)(2 << 6 | 0 << 5 | _subType & 0x1F);
            buffer[offset++] = 204;
            buffer[offset++] = (byte)((length >> 8) | 0xFF);
            buffer[offset++] = (byte)((length) | 0xFF);
            buffer[offset++] = (byte)((_source >> 24) | 0xFF);
            buffer[offset++] = (byte)((_source >> 16) | 0xFF);
            buffer[offset++] = (byte)((_source >> 8) | 0xFF);
            buffer[offset++] = (byte)((_source) | 0xFF);
            buffer[offset++] = (byte)_name[0];
            buffer[offset++] = (byte)_name[1];
            buffer[offset++] = (byte)_name[2];
            buffer[offset++] = (byte)_name[2];
            Array.Copy(_data, 0, buffer, offset, _data.Length);
            offset += _data.Length;
        }

        #endregion

        #region Properties
        
        public override int Version
        {
            get { return _version; }
        }
        public override RtcpPacketType Type
        {
            get { return RtcpPacketType.ApplicationDefined; }
        }
        public int SubType
        {
            get { return _subType; }
        }
        public uint Source
        {
            get { return _source; }

            set { _source = value; }
        }
        public string Name
        {
            get { return _name; }
        }
        public byte[] Data
        {
            get { return _data; }
        }
        public override int Size
        {
            get { return 12 + _data.Length; }
        }
        #endregion
    }
}
