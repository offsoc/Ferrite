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

public readonly ref struct dh_gen_fail
{
    private readonly Span<byte> _buff;
    private readonly IMemoryOwner<byte>? _memory;
    public dh_gen_fail(ReadOnlySpan<byte> nonce, ReadOnlySpan<byte> server_nonce, ReadOnlySpan<byte> new_nonce_hash3)
    {
        var length = GetRequiredBufferSize();
        _memory = UnmanagedMemoryPool<byte>.Shared.Rent(length);
        _memory.Memory.Span.Clear();
        _buff = _memory.Memory.Span[..length];
        SetConstructor(unchecked((int)0xa69dae02));
        Set_nonce(nonce);
        Set_server_nonce(server_nonce);
        Set_new_nonce_hash3(new_nonce_hash3);
    }
    public dh_gen_fail(Span<byte> buff)
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
        var bytesRead = GetOffset(4, data[offset..]);
        if (bytesRead > data.Length + offset)
        {
            return Span<byte>.Empty;
        }
        return data.Slice(offset, bytesRead);
    }

    public static int GetRequiredBufferSize()
    {
        return 4 + 16 + 16 + 16;
    }
    public static int ReadSize(Span<byte> data, int offset)
    {
        return GetOffset(4, data[offset..]);
    }
    public ReadOnlySpan<byte> nonce => _buff.Slice(GetOffset(1, _buff), 16);
    private void Set_nonce(ReadOnlySpan<byte> value)
    {
        if(value.Length != 16)
        {
            return;
        }
        value.CopyTo(_buff.Slice(GetOffset(1, _buff), 16));
    }
    public ReadOnlySpan<byte> server_nonce => _buff.Slice(GetOffset(2, _buff), 16);
    private void Set_server_nonce(ReadOnlySpan<byte> value)
    {
        if(value.Length != 16)
        {
            return;
        }
        value.CopyTo(_buff.Slice(GetOffset(2, _buff), 16));
    }
    public ReadOnlySpan<byte> new_nonce_hash3 => _buff.Slice(GetOffset(3, _buff), 16);
    private void Set_new_nonce_hash3(ReadOnlySpan<byte> value)
    {
        if(value.Length != 16)
        {
            return;
        }
        value.CopyTo(_buff.Slice(GetOffset(3, _buff), 16));
    }
    private static int GetOffset(int index, Span<byte> buffer)
    {
        int offset = 4;
        if(index >= 2) offset += 16;
        if(index >= 3) offset += 16;
        if(index >= 4) offset += 16;
        return offset;
    }
    public ref struct TLObjectBuilder
    {
        private ReadOnlySpan<byte> _nonce;
        public TLObjectBuilder with_nonce(ReadOnlySpan<byte> value)
        {
            _nonce = value;
            return this;
        }
        private ReadOnlySpan<byte> _server_nonce;
        public TLObjectBuilder with_server_nonce(ReadOnlySpan<byte> value)
        {
            _server_nonce = value;
            return this;
        }
        private ReadOnlySpan<byte> _new_nonce_hash3;
        public TLObjectBuilder with_new_nonce_hash3(ReadOnlySpan<byte> value)
        {
            _new_nonce_hash3 = value;
            return this;
        }
        public dh_gen_fail Build()
        {
            return new dh_gen_fail(_nonce, _server_nonce, _new_nonce_hash3);
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
