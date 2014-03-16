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
using System.Text;

namespace SatIp.Library.Rtcp
{
    public class RtcpPacketSourceDescriptionChunk
    {
        #region Fields

        private uint _source;
        private string _cName;
        private string _name;
        private string _email;
        private string _phone;
        private string _location;
        private string _tool;
        private string _note;

        #endregion

        #region Constructor

        public RtcpPacketSourceDescriptionChunk(uint source, string cname)
        {
            if (source == 0)
            {
                throw new ArgumentException("Argument 'source' value must be > 0.");
            }
            if (string.IsNullOrEmpty(cname))
            {
                throw new ArgumentException("Argument 'cname' value may not be null or empty.");
            }

            _source = source;
            _cName = cname;
        }

        internal RtcpPacketSourceDescriptionChunk()
        {
        }

        #endregion

        #region method Parse

        
        public void Parse(byte[] buffer, ref int offset)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (offset < 0)
            {
                throw new ArgumentException("Argument 'offset' value must be >= 0.");
            }
            int startOffset = offset;
            _source = (uint)(buffer[offset++] << 24 | buffer[offset++] << 16 | buffer[offset++] << 8 | buffer[offset++]);
            while (offset < buffer.Length && buffer[offset] != 0)
            {
                int type = buffer[offset++];
                int length = buffer[offset++];
                if (type == 1)
                {
                    _cName = Encoding.UTF8.GetString(buffer, offset, length);
                }
                else if (type == 2)
                {
                    _cName = Encoding.UTF8.GetString(buffer, offset, length);
                }
                else if (type == 3)
                {
                    _email = Encoding.UTF8.GetString(buffer, offset, length);
                }
                else if (type == 4)
                {
                    _phone = Encoding.UTF8.GetString(buffer, offset, length);
                }
                else if (type == 5)
                {
                    _location = Encoding.UTF8.GetString(buffer, offset, length);
                }
                else if (type == 6)
                {
                    _tool = Encoding.UTF8.GetString(buffer, offset, length);
                }
                else if (type == 7)
                {
                    _note = Encoding.UTF8.GetString(buffer, offset, length);
                }
                else if (type == 8)
                {
                    // TODO:
                }
                offset += length;
            }
            offset++;
            offset += (offset - startOffset) % 4;
        }

        #endregion

        #region method ToByte
        public void ToByte(byte[] buffer, ref int offset)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (offset < 0)
            {
                throw new ArgumentException("Argument 'offset' value must be >= 0.");
            }

            int startOffset = offset;
            buffer[offset++] = (byte)((_source >> 24) & 0xFF);
            buffer[offset++] = (byte)((_source >> 16) & 0xFF);
            buffer[offset++] = (byte)((_source >> 8) & 0xFF);
            buffer[offset++] = (byte)((_source) & 0xFF);
            if (!string.IsNullOrEmpty(_cName))
            {
                byte[] b = Encoding.UTF8.GetBytes(_cName);
                buffer[offset++] = 1;
                buffer[offset++] = (byte)b.Length;
                Array.Copy(b, 0, buffer, offset, b.Length);
                offset += b.Length;
            }
            if (!string.IsNullOrEmpty(_name))
            {
                byte[] b = Encoding.UTF8.GetBytes(_name);
                buffer[offset++] = 2;
                buffer[offset++] = (byte)b.Length;
                Array.Copy(b, 0, buffer, offset, b.Length);
                offset += b.Length;
            }
            if (!string.IsNullOrEmpty(_email))
            {
                byte[] b = Encoding.UTF8.GetBytes(_email);
                buffer[offset++] = 3;
                buffer[offset++] = (byte)b.Length;
                Array.Copy(b, 0, buffer, offset, b.Length);
                offset += b.Length;
            }
            if (!string.IsNullOrEmpty(_phone))
            {
                byte[] b = Encoding.UTF8.GetBytes(_phone);
                buffer[offset++] = 4;
                buffer[offset++] = (byte)b.Length;
                Array.Copy(b, 0, buffer, offset, b.Length);
                offset += b.Length;
            }
            if (!string.IsNullOrEmpty(_location))
            {
                byte[] b = Encoding.UTF8.GetBytes(_location);
                buffer[offset++] = 5;
                buffer[offset++] = (byte)b.Length;
                Array.Copy(b, 0, buffer, offset, b.Length);
                offset += b.Length;
            }
            if (!string.IsNullOrEmpty(_tool))
            {
                byte[] b = Encoding.UTF8.GetBytes(_tool);
                buffer[offset++] = 6;
                buffer[offset++] = (byte)b.Length;
                Array.Copy(b, 0, buffer, offset, b.Length);
                offset += b.Length;
            }
            if (!string.IsNullOrEmpty(_note))
            {
                byte[] b = Encoding.UTF8.GetBytes(_note);
                buffer[offset++] = 7;
                buffer[offset++] = (byte)b.Length;
                Array.Copy(b, 0, buffer, offset, b.Length);
                offset += b.Length;
            }
            buffer[offset++] = 0;
            while ((offset - startOffset) % 4 > 0)
            {
                buffer[offset++] = 0;
            }
        }

        #endregion

        #region Properties
        public uint Source
        {
            get { return _source; }
        }
        public string CName
        {
            get { return _cName; }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                if (Encoding.UTF8.GetByteCount(value) > 255)
                {
                    throw new ArgumentException("Property 'Name' value must be <= 255 bytes.");
                }
                _name = value;
            }
        }
        public string Email
        {
            get { return _email; }
            set
            {
                if (Encoding.UTF8.GetByteCount(value) > 255)
                {
                    throw new ArgumentException("Property 'Email' value must be <= 255 bytes.");
                }
                _email = value;
            }
        }
        public string Phone
        {
            get { return _phone; }

            set
            {
                if (Encoding.UTF8.GetByteCount(value) > 255)
                {
                    throw new ArgumentException("Property 'Phone' value must be <= 255 bytes.");
                }

                _phone = value;
            }
        }
        public string Location
        {
            get { return _location; }
            set
            {
                if (Encoding.UTF8.GetByteCount(value) > 255)
                {
                    throw new ArgumentException("Property 'Location' value must be <= 255 bytes.");
                }
                _location = value;
            }
        }
        public string Tool
        {
            get { return _tool; }
            set
            {
                if (Encoding.UTF8.GetByteCount(value) > 255)
                {
                    throw new ArgumentException("Property 'Tool' value must be <= 255 bytes.");
                }
                _tool = value;
            }
        }
        public string Note
        {
            get { return _note; }
            set
            {
                if (Encoding.UTF8.GetByteCount(value) > 255)
                {
                    throw new ArgumentException("Property 'Note' value must be <= 255 bytes.");
                }

                _note = value;
            }
        }
        public int Size
        {
            get
            {
                int size = 4;
                if (!string.IsNullOrEmpty(_cName))
                {
                    size += 2;
                    size += Encoding.UTF8.GetByteCount(_cName);
                }
                if (!string.IsNullOrEmpty(_name))
                {
                    size += 2;
                    size += Encoding.UTF8.GetByteCount(_name);
                }
                if (!string.IsNullOrEmpty(_email))
                {
                    size += 2;
                    size += Encoding.UTF8.GetByteCount(_email);
                }
                if (!string.IsNullOrEmpty(_phone))
                {
                    size += 2;
                    size += Encoding.UTF8.GetByteCount(_phone);
                }
                if (!string.IsNullOrEmpty(_location))
                {
                    size += 2;
                    size += Encoding.UTF8.GetByteCount(_location);
                }
                if (!string.IsNullOrEmpty(_tool))
                {
                    size += 2;
                    size += Encoding.UTF8.GetByteCount(_tool);
                }
                if (!string.IsNullOrEmpty(_note))
                {
                    size += 2;
                    size += Encoding.UTF8.GetByteCount(_note);
                }
                size++;
                while ((size % 4) > 0)
                {
                    size++;
                }
                return size;
            }
        }

        #endregion
    }
}
