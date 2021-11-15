// helper class to copy Unity.Collections.Bytes30 (etc.) into byte[]
// -> ArraySegments can use those functions too!
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace DOTSNET
{
    public static class FlatByteArrays
    {
        // copy Bytes16 struct to byte[]
        public static bool Bytes16ToArray(Bytes16 value, byte[] array, int arrayOffset)
        {
            // enough space in array?
            // => check total size before any writes to make it atomic!
            if (array != null &&
                arrayOffset + 16 <= array.Length)
            {
                unsafe
                {
                    // for large structures, memcpy is 10x faster than manual!
                    fixed (byte* ptr = &array[arrayOffset])
                        UnsafeUtility.CopyStructureToPtr(ref value, ptr);
                }
                return true;
            }
            // not enough space
            return false;
        }

        // copy Bytes30 struct to byte[]
        public static bool Bytes30ToArray(Bytes30 value, byte[] array, int arrayOffset)
        {
            // enough space in array?
            // => check total size before any writes to make it atomic!
            if (array != null &&
                arrayOffset + 30 <= array.Length)
            {
                unsafe
                {
                    // for large structures, memcpy is 10x faster than manual!
                    fixed (byte* ptr = &array[arrayOffset])
                        UnsafeUtility.CopyStructureToPtr(ref value, ptr);
                }
                return true;
            }
            // not enough space
            return false;
        }

        // copy Bytes62 struct to byte[]
        public static bool Bytes62ToArray(Bytes62 value, byte[] array, int arrayOffset)
        {
            // enough space in array?
            // => check total size before any writes to make it atomic!
            if (array != null &&
                arrayOffset + 62 <= array.Length)
            {
                unsafe
                {
                    // for large structures, memcpy is 10x faster than manual!
                    fixed (byte* ptr = &array[arrayOffset])
                        UnsafeUtility.CopyStructureToPtr(ref value, ptr);
                }
                return true;
            }
            // not enough space to write
            return false;
        }

        // copy Bytes126 struct to byte[]
        public static bool Bytes126ToArray(Bytes126 value, byte[] array, int arrayOffset)
        {
            // enough space in array?
            // => check total size before any writes to make it atomic!
            if (array != null &&
                arrayOffset + 126 <= array.Length)
            {
                unsafe
                {
                    // for large structures, memcpy is 10x faster than manual!
                    fixed (byte* ptr = &array[arrayOffset])
                        UnsafeUtility.CopyStructureToPtr(ref value, ptr);
                }
                return true;
            }
            // not enough space to write
            return false;
        }

        // copy Bytes510 struct to byte[]
        public static bool Bytes510ToArray(Bytes510 value, byte[] array, int arrayOffset)
        {
            // enough space in array?
            // => check total size before any writes to make it atomic!
            if (array != null &&
                arrayOffset + 510 <= array.Length)
            {
                unsafe
                {
                    // for large structures, memcpy is 10x faster than manual!
                    fixed (byte* ptr = &array[arrayOffset])
                        UnsafeUtility.CopyStructureToPtr(ref value, ptr);
                }
                return true;
            }
            // not enough space to write
            return false;
        }

        // create Bytes16 struct from byte[]
        public static bool ArrayToBytes16(byte[] array, int arrayOffset, out Bytes16 value)
        {
            // enough data to read?
            // => check total size before any reads to make it atomic!
            if (array != null && arrayOffset + 16 <= array.Length)
            {
                unsafe
                {
                    // for large structures, memcpy is 10x faster than manual!
                    fixed (byte* ptr = &array[arrayOffset])
                        UnsafeUtility.CopyPtrToStructure(ptr, out value);
                }
                return true;
            }
            // not enough data to read
            value = new Bytes16();
            return false;
        }

        // create Bytes30 struct from byte[]
        public static bool ArrayToBytes30(byte[] array, int arrayOffset, out Bytes30 value)
        {
            // enough data to read?
            // => check total size before any reads to make it atomic!
            if (array != null && arrayOffset + 30 <= array.Length)
            {
                unsafe
                {
                    // for large structures, memcpy is 10x faster than manual!
                    fixed (byte* ptr = &array[arrayOffset])
                        UnsafeUtility.CopyPtrToStructure(ptr, out value);
                }
                return true;
            }
            value = new Bytes30();
            return false;
        }

        // create Bytes62 struct from byte[]
        public static bool ArrayToBytes62(byte[] array, int arrayOffset, out Bytes62 value)
        {
            // enough data to read?
            // => check total size before any reads to make it atomic!
            if (array != null && arrayOffset + 62 <= array.Length)
            {
                unsafe
                {
                    // for large structures, memcpy is 10x faster than manual!
                    fixed (byte* ptr = &array[arrayOffset])
                        UnsafeUtility.CopyPtrToStructure(ptr, out value);
                }
                return true;
            }
            value = new Bytes62();
            return false;
        }

        // create Bytes126 struct from byte[]
        public static bool ArrayToBytes126(byte[] array, int arrayOffset, out Bytes126 value)
        {
            // enough data to read?
            // => check total size before any reads to make it atomic!
            if (array != null && arrayOffset + 126 <= array.Length)
            {
                unsafe
                {
                    // for large structures, memcpy is 10x faster than manual!
                    fixed (byte* ptr = &array[arrayOffset])
                        UnsafeUtility.CopyPtrToStructure(ptr, out value);
                }
                return true;
            }
            value = new Bytes126();
            return false;
        }

        // create Bytes510 struct from byte[]
        public static bool ArrayToBytes510(byte[] array, int arrayOffset, out Bytes510 value)
        {
            // enough data to read?
            // => check total size before any reads to make it atomic!
            if (array != null && arrayOffset + 510 <= array.Length)
            {
                unsafe
                {
                    // for large structures, memcpy is 10x faster than manual!
                    fixed (byte* ptr = &array[arrayOffset])
                        UnsafeUtility.CopyPtrToStructure(ptr, out value);
                }
                return true;
            }
            value = new Bytes510();
            return false;
        }

        // compare two Bytes16 structs without .Equals() boxing allocations.
        public static bool CompareBytes16(Bytes16 a, Bytes16 b) =>
            a.byte0000 == b.byte0000 &&
            a.byte0001 == b.byte0001 &&
            a.byte0002 == b.byte0002 &&
            a.byte0003 == b.byte0003 &&
            a.byte0004 == b.byte0004 &&
            a.byte0005 == b.byte0005 &&
            a.byte0006 == b.byte0006 &&
            a.byte0007 == b.byte0007 &&
            a.byte0008 == b.byte0008 &&
            a.byte0009 == b.byte0009 &&
            a.byte0010 == b.byte0010 &&
            a.byte0011 == b.byte0011 &&
            a.byte0012 == b.byte0012 &&
            a.byte0013 == b.byte0013 &&
            a.byte0014 == b.byte0014 &&
            a.byte0015 == b.byte0015;
    }
}