// 
// Project Ferrite is an Implementation of the Telegram Server API
// Copyright 2022 Aykut Alparslan KOC <aykutalparslan@msn.com>
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
// 

using System.Buffers.Binary;
using DotNext.Buffers;
using Ferrite.Transport;

namespace Ferrite.Core.Features;

public class QuickAckFeature : IQuickAckFeature
{
    public void Send(int ack, SparseBufferWriter<byte> writer,
        IFrameEncoder encoder, WebSocketHandler? webSocketHandler,
        MTProtoConnection connection)
    {
        writer.Clear();
        ack |= 1 << 31;
        if (encoder is AbridgedFrameEncoder)
        {
            ack = BinaryPrimitives.ReverseEndianness(ack);
        }
        writer.WriteInt32(ack, true);
        var msg = writer.ToReadOnlySequence();
        var encoded = encoder.EncodeBlock(msg);
        if (webSocketHandler != null)
        {
            webSocketHandler.WriteHeaderTo(connection.TransportConnection.Transport.Output, 4);
        }

        connection.TransportConnection.Transport.Output.Write(encoded);
    }
}