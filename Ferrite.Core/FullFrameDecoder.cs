﻿/*
 *   Project Ferrite is an Implementation Telegram Server API
 *   Copyright 2022 Aykut Alparslan KOC <aykutalparslan@msn.com>
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU Affero General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU Affero General Public License for more details.
 *
 *   You should have received a copy of the GNU Affero General Public License
 *   along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Buffers;
using DotNext.Buffers;
using DotNext.IO;
using Ferrite.Crypto;
using Ferrite.Data;
using Ferrite.TL.currentLayer;

namespace Ferrite.Core;

public class FullFrameDecoder : IFrameDecoder
{
    private readonly byte[] _lengthBytes = new byte[4];
    private const int StreamChunkSize = 1024;
    private int _length;
    private int _remaining;
    private bool _isStream;
    private int _sequence;
    private Aes256Ctr? _decryptor;
    private readonly IDistributedCache _cache;
    private readonly IPersistentStore _db;

    public FullFrameDecoder(IDistributedCache cache, IPersistentStore db)
    {
        _cache = cache;
        _db = db;
    }

    public FullFrameDecoder(Aes256Ctr decryptor, IDistributedCache cache, IPersistentStore db)
    {
        _decryptor = decryptor;
        _cache = cache;
        _db = db;
    }

    public bool Decode(ref SequenceReader<byte> reader, out ReadOnlySequence<byte> frame, out bool isStream)
    {
        isStream = _isStream;
        if (_length == 0)
        {
            if (reader.Remaining < 4)
            {
                frame = new ReadOnlySequence<byte>();
                return false;
            }
            else
            {
                reader.TryCopyTo(_lengthBytes);
                if (_decryptor != null)
                {
                    _decryptor.Transform(_lengthBytes);
                    _length = (_lengthBytes[0]) |
                        (_lengthBytes[1] << 8) |
                        (_lengthBytes[2] << 16) |
                        (_lengthBytes[3] << 24);
                }
                reader.Advance(4);
            }
        }
        if (reader.Remaining >= 72 && !_isStream)
        {
            _isStream = IsStream(reader.UnreadSequence.Slice(0, 72));
            isStream = _isStream;
        }

        if (_isStream && reader.Remaining > 0)
        {
            int toBeWritten = Math.Min(_remaining, StreamChunkSize);
            ReadOnlySequence<byte> chunk = reader.UnreadSequence.Slice(0, toBeWritten);
            reader.Advance(toBeWritten);
            _remaining -= toBeWritten;
            if (_decryptor != null)
            {
                var chunkDecrypted = new byte[toBeWritten];
                _decryptor.Transform(chunk, chunkDecrypted);
                frame = new ReadOnlySequence<byte>(chunkDecrypted);
            }
            else
            {
                frame = chunk;
            }
            if (_remaining == 0)
            {
                _length = 0;
                _isStream = false;
                Array.Clear(_lengthBytes);
                return false;
            }

            return true;
        }
        if (reader.Remaining < _length)
        {
            frame = new ReadOnlySequence<byte>();
            return false;
        }
        var data = new byte[_length];
        Array.Copy(_lengthBytes, data, 4);
        reader.UnreadSequence.Slice(0, _length - 4).CopyTo(data.AsSpan().Slice(4));
        reader.Advance(_length-4);
        _length = 0;
        Array.Clear(_lengthBytes);
        if (_decryptor != null)
        {
            _decryptor.Transform(data.AsSpan().Slice(4));
        }
        uint crc32 = data.AsSpan().GetCrc32();
        _sequence = (data[4]) |
                        (data[5] << 8) |
                        (data[6] << 16) |
                        (data[7] << 24);

        int checkSum = (data[data.Length-4]) |
                        (data[data.Length - 3] << 8) |
                        (data[data.Length - 2] << 16) |
                        (data[data.Length - 1] << 24);

        if (crc32 == (uint)checkSum)
        {
            frame = new ReadOnlySequence<byte>(data.AsMemory().Slice(8,data.Length-12));
        }
        else
        {
            frame = new ReadOnlySequence<byte>();
        }
        if (reader.Remaining != 0)
        {
            return false;
        }
        return false;
    }
    private bool IsStream(ReadOnlySequence<byte> header)
    {
        SequenceReader reader = IAsyncBinaryReader.Create(header);
        long authKeyId = reader.ReadInt64(true);
        var authKey = (_cache.GetAuthKey(authKeyId) ?? 
                       _cache.GetTempAuthKey(authKeyId)) ?? 
                      _db.GetAuthKey(authKeyId);
        if (authKey is { Length: > 0 })
        {
            _ = _cache.PutAuthKeyAsync(authKeyId, authKey);
            Span<byte> messageKey = stackalloc byte[16];
            reader.Read(messageKey);
            AesIge aesIge = new AesIge(authKey, messageKey);
            Span<byte> messageHeader = stackalloc byte[48];
            reader.Read(messageHeader);
            aesIge.Decrypt(messageHeader);
            SpanReader<byte> sr = new SpanReader<byte>(messageHeader);
            sr.Advance(32);
            int construstor = sr.ReadInt32(true);
            if (construstor == TLConstructor.Upload_SaveFilePart ||
                construstor == TLConstructor.Upload_SaveBigFilePart)
            {
                return true;
            }
        }
        return false;
    }
}

