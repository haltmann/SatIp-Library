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
using System.Net.Sockets;

namespace SatIp.Library.Rtsp
{
    /// <summary>
  /// A simple implementation of an RTSP client.
  /// </summary>
  public class RtspClient
  {
    #region variables

    private readonly string _serverHost;
    private TcpClient _client;
    private int _cseq = 1;
    private readonly object _lockObject = new object();

    #endregion
   
    public RtspClient(string serverHost)
    {
      _serverHost = serverHost;
      _client = new TcpClient(serverHost, 554);
    }

    ~RtspClient()
    {
      lock (_lockObject)
      {
        if (_client != null)
        {
          _client.Close();
          _client = null;
        }
      }
    }
    
    public RtspStatusCode SendRequest(RtspRequest request, out RtspResponse response)
    {
      response = null;
      lock (_lockObject)
      {
        NetworkStream stream = null;
        try
        {
            stream = _client.GetStream();
            if (stream == null)
            {
                throw new Exception();
            }
        }
        catch
        {
            _client.Close();
        }

        try
        {
          if (_client == null)
          {
            _client = new TcpClient(_serverHost, 554);
          }
          request.Headers.Add("CSeq", _cseq.ToString(CultureInfo.InvariantCulture));
          _cseq++;
          byte[] requestBytes = request.Serialise();
            
          stream.Write(requestBytes, 0, requestBytes.Length);
          var responseBytes = new byte[_client.ReceiveBufferSize];
          int byteCount = stream.Read(responseBytes, 0, responseBytes.Length);
          response = RtspResponse.Deserialise(responseBytes, byteCount);
          string contentLengthString;
          if (response.Headers.TryGetValue("Content-Length", out contentLengthString))
          {
              int contentLength = int.Parse(contentLengthString);
              if ((string.IsNullOrEmpty(response.Body) && contentLength > 0) || response.Body.Length < contentLength)
              {
                  if (response.Body == null)
                  {
                    response.Body = string.Empty;
                  }
                  while (byteCount > 0 && response.Body.Length < contentLength)
                  {
                    byteCount = stream.Read(responseBytes, 0, responseBytes.Length);
                    response.Body += System.Text.Encoding.UTF8.GetString(responseBytes, 0, byteCount);
                  }
              }
          }
            return response.StatusCode;
        }
        finally
        {
          stream.Close();
        }
      }
    }
  }
}
