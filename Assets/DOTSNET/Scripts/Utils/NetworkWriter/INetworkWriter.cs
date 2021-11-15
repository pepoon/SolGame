// NetworkWriter interface
// -> NetworkMessage/NetworkComponent.Serialize can use the interface
// -> different systems can use different writers depending on the use case
//
// writers might work with byte[], NativeArray, fixed byte[] for burst, etc.
using Unity.Collections;
using Unity.Mathematics;

namespace DOTSNET
{
    // NOTE: casting to INetworkWriter allocates because it's an interface.
    // use the explicit types directly where possible.
    public interface INetworkWriter
    {
        // postion & space /////////////////////////////////////////////////////
        int Position { get; set; }
        int Space { get; }

        // end result //////////////////////////////////////////////////////////
        // we need a way to get the writer's internal content after writing.
        // not all buffer types can give a NativeSlice (i.e. if fixed buffer).
        // CopyTo() works in all cases though.
        //
        // returns bytes written
        int CopyTo(byte[] destination);
        unsafe int CopyTo(byte* destination, int destinationLength);

        // simple types ////////////////////////////////////////////////////////
        bool WriteByte(byte value);
        bool WriteBool(bool value);
        bool WriteShort(short value);
        bool WriteUShort(ushort value);
        bool WriteInt(int value);
        bool WriteInt2(int2 value);
        bool WriteInt3(int3 value);
        bool WriteInt4(int4 value);
        bool WriteUInt(uint value);
        bool WriteLong(long value);
        bool WriteULong(ulong value);
        bool WriteFloat(float value);
        bool WriteFloat2(float2 value);
        bool WriteFloat3(float3 value);
        bool WriteDouble(double value);
        bool WriteDouble2(double2 value);
        bool WriteDouble3(double3 value);
        bool WriteDouble4(double4 value);
        bool WriteDecimal(decimal value);
        bool WriteQuaternion(quaternion value);
        // write quaternion with smallest-three compression
        // see also: https://gafferongames.com/post/snapshot_compression/
        //
        // reuses our smallest three compression for quaternion->uint 32 bit.
        //
        // IMPORTANT: assumes normalized quaternion!
        //            we also normalize when decompressing.
        bool WriteQuaternionSmallestThree(quaternion value);

        bool WriteBytes16(Bytes16 value);
        bool WriteBytes30(Bytes30 value);
        bool WriteBytes62(Bytes62 value);
        bool WriteBytes126(Bytes126 value);
        bool WriteBytes510(Bytes510 value);

        bool WriteFixedString32(FixedString32 value);
        bool WriteFixedString64(FixedString64 value);
        bool WriteFixedString128(FixedString128 value);
        bool WriteFixedString512(FixedString512 value);

        // arrays //////////////////////////////////////////////////////////////
        // byte* useful for writing fixed byte[]s
        unsafe bool WriteBytes(byte* bytes, int bytesLength, int offset, int size);
        bool WriteBytes(NativeSlice<byte> bytes);
        bool WriteBytesAndSize(NativeSlice<byte> bytes);
        bool WriteNativeArray(NativeArray<byte> array, int offset, int size);
        bool WriteNativeArray<T>(NativeArray<T> array, int offset, int size)
            where T : unmanaged;
    }
}
