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

using System.Collections.Generic;
using System.Text;

namespace SatIp.Library.Rtsp
{
    public class RtspRequest
    {
        #region Fields

        private readonly RtspMethod _method;
        private readonly string _uri;
        private readonly int _majorVersion;
        private readonly int _minorVersion;
        private IDictionary<string, string> _headers = new Dictionary<string, string>();
        private string _body = string.Empty;
 
        #endregion

        #region Constructor

        public RtspRequest(RtspMethod method, string uri, int majorVersion, int minorVersion)
        {
            _method = method;
            _uri = uri;
            _majorVersion = majorVersion;
            _minorVersion = minorVersion;
        }

        #endregion

        #region Properties

        public RtspMethod Method
        {
            get { return _method; }
        }

        public string Uri
        {
            get { return _uri; }
        }

        public int MajorVersion
        {
            get { return _majorVersion; }
        }

        public int MinorVersion
        {
            get { return _minorVersion; }
        }

        public IDictionary<string, string> Headers
        {
            get { return _headers; }
            set { _headers = value; }
        }

        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }

        #endregion

        #region Methods

        public byte[] Serialise()
        {
            var request = new StringBuilder();
            request.AppendFormat("{0} {1} RTSP/{2}.{3}\r\n", _method, _uri, _majorVersion, _minorVersion);
            foreach (var header in _headers)
            {
                request.AppendFormat("{0}: {1}\r\n", header.Key, header.Value);
            }
            request.AppendFormat("\r\n{0}", _body);
            return Encoding.UTF8.GetBytes(request.ToString());
        }

        #endregion
    }
}
