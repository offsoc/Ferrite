//  <auto-generated>
//  This file was auto-generated by the Ferrite TL Generator.
//  Please do not modify as all changes will be lost.
//  <auto-generated/>

#nullable enable

using System.Buffers;
using System.Runtime.InteropServices;
using Ferrite.Utils;
using DotNext.Buffers;

namespace Ferrite.TL.slim.mtproto;

public readonly ref struct rpc_result
{
    private readonly Span<byte> _buff;
    private readonly IMemoryOwner<byte>? _memory;
    public rpc_result(long req_msg_id, ReadOnlySpan<byte> result)
    {
        var length = GetRequiredBufferSize(result.Length);
        _memory = UnmanagedMemoryPool<byte>.Shared.Rent(length);
        _memory.Memory.Span.Clear();
        _buff = _memory.Memory.Span[..length];
        SetConstructor(unchecked((int)0xf35c6d01));
        Set_req_msg_id(req_msg_id);
        Set_result(result);
    }
    public rpc_result(Span<byte> buff)
    {
        _buff = buff;
    }
    
    public readonly int Constructor => MemoryMarshal.Read<int>(_buff);

    private void SetConstructor(int constructor)
    {
        MemoryMarshal.Write(_buff.Slice(0, 4), ref constructor);
    }
    public int Length => _buff.Length;
    public ReadOnlySpan<byte> ToReadOnlySpan() => _buff;
    public TLBytes? TLBytes => _memory != null ? new TLBytes(_memory, 0, _buff.Length) : null;
    public static Span<byte> Read(Span<byte> data, int offset)
    {
        var bytesRead = GetOffset(3, data[offset..]);
        if (bytesRead > data.Length + offset)
        {
            return Span<byte>.Empty;
        }
        return data.Slice(offset, bytesRead);
    }

    public static int GetRequiredBufferSize(int len_result)
    {
        return 4 + 8 + len_result;
    }
    public static int ReadSize(Span<byte> data, int offset)
    {
        return GetOffset(3, data[offset..]);
    }
    public readonly long req_msg_id => MemoryMarshal.Read<long>(_buff[GetOffset(1, _buff)..]);
    private void Set_req_msg_id(long value)
    {
        MemoryMarshal.Write(_buff[GetOffset(1, _buff)..], ref value);
    }
    public Span<byte> result => ObjectReader.Read(_buff);
    private void Set_result(ReadOnlySpan<byte> value)
    {
        value.CopyTo(_buff[GetOffset(2, _buff)..]);
    }
    private static int GetOffset(int index, Span<byte> buffer)
    {
        int offset = 4;
        if(index >= 2) offset += 8;
        if(index >= 3) offset += ObjectReader.ReadSize(buffer[offset..], unchecked((int)0xf35c6d01));
        return offset;
    }
    public ref struct TLObjectBuilder
    {
        private long _req_msg_id;
        public TLObjectBuilder with_req_msg_id(long value)
        {
            _req_msg_id = value;
            return this;
        }
        private ReadOnlySpan<byte> _result;
        public TLObjectBuilder with_result(ReadOnlySpan<byte> value)
        {
            _result = value;
            return this;
        }
        public rpc_result Build()
        {
            return new rpc_result(_req_msg_id, _result);
        }
    }

    public static TLObjectBuilder Builder()
    {
        return new TLObjectBuilder();
    }
    public void Dispose()
    {
        _memory?.Dispose();
    }
}
