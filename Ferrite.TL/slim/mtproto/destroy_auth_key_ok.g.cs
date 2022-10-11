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

public readonly ref struct destroy_auth_key_ok
{
    private readonly Span<byte> _buff;
    private readonly IMemoryOwner<byte>? _memory;
    public destroy_auth_key_ok()
    {
        var length = GetRequiredBufferSize();
        _memory = UnmanagedMemoryPool<byte>.Shared.Rent(length);
        _memory.Memory.Span.Clear();
        _buff = _memory.Memory.Span[..length];
        SetConstructor(unchecked((int)0xf660e1d4));
    }
    public destroy_auth_key_ok(Span<byte> buff)
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
        var bytesRead = GetOffset(1, data[offset..]);
        if (bytesRead > data.Length + offset)
        {
            return Span<byte>.Empty;
        }
        return data.Slice(offset, bytesRead);
    }

    public static int GetRequiredBufferSize()
    {
        return 4;
    }
    public static int ReadSize(Span<byte> data, int offset)
    {
        return GetOffset(1, data[offset..]);
    }
    private static int GetOffset(int index, Span<byte> buffer)
    {
        int offset = 4;
        return offset;
    }
    public ref struct TLObjectBuilder
    {
        public destroy_auth_key_ok Build()
        {
            return new destroy_auth_key_ok();
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
