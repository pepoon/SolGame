using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace DOTSNET
{
    public static class Utils
    {
        // pretty print bytes as KB/MB/GB/etc.
        // long to support > 2GB
        // divides by floats to return "2.5MB" etc.
        public static string PrettyBytes(long bytes)
        {
            // bytes
            if (bytes < 1024)
                return $"{bytes} B";
            // kilobytes
            else if (bytes < 1024L * 1024L)
                return $"{(bytes / 1024f):F2} KB";
            // megabytes
            else if (bytes < 1024 * 1024L * 1024L)
                return $"{(bytes / (1024f * 1024f)):F2} MB";
            // gigabytes
            return $"{(bytes / (1024f * 1024f * 1024f)):F2} GB";
        }

        // helper function to swap int bytes endianness
        public static uint SwapBytes(uint value)
        {
            return (value & 0x000000FFu) << 24 |
                   (value & 0x0000FF00u) << 8 |
                   (value & 0x00FF0000u) >> 8 |
                   (value & 0xFF000000u) >> 24;
        }

        // RoundToLong for huge double can overflow from long.max to long.min
        // Convert.ToInt64 throws exception if it would overflow.
        // we need one that properly Clamps.
        internal static long RoundAndClampToLong(double value)
        {
            // this would fail the test because rounding a int.max float overflows!
            //return Mathf.RoundToInt(Mathf.Clamp(value, int.MinValue, int.MaxValue));

            // this works perfectly!
            if (value >= long.MaxValue)
                return long.MaxValue;
            if (value <= long.MinValue)
                return long.MinValue;

            // Convert.ToInt64 so we don't need to depend on Unity.Mathf!
            return Convert.ToInt64(value);
        }

        // need to round bits to minimum amount of bytes they fit into
        public static int RoundBitsToFullBytes(int bits)
        {
            // special case: for 1 bit we need 1 byte.
            // for 0 bits we need 0 bytes.
            // the calculation below would give
            //   0 - 1 = -1 then / 8 = 0 then + 1 = 1
            // for 0 byte.
            if (bits == 0) return 0;

            // calculation example for up to 9 bits:
            //   1 - 1 =  0 then / 8 = 0 then + 1 = 1
            //   2 - 1 =  1 then / 8 = 0 then + 1 = 1
            //   3 - 1 =  2 then / 8 = 0 then + 1 = 1
            //   4 - 1 =  3 then / 8 = 0 then + 1 = 1
            //   5 - 1 =  4 then / 8 = 0 then + 1 = 1
            //   6 - 1 =  5 then / 8 = 0 then + 1 = 1
            //   7 - 1 =  6 then / 8 = 0 then + 1 = 1
            //   8 - 1 =  7 then / 8 = 0 then + 1 = 1
            //   9 - 1 =  8 then / 8 = 1 then + 1 = 2
            return ((bits-1) / 8) + 1;
        }

        // calculate bits needed for a value range
        // largest type we support is ulong, so use that as parameters
        // min, max are both INCLUSIVE
        //   min=0, max=7 means 0..7 = 8 values in total = 3 bits required
        public static int BitsRequired(ulong min, ulong max)
        {
            // make sure value is within range
            // => throws exception because the developer should fix it immediately
            if (min > max)
                throw new ArgumentOutOfRangeException($"{nameof(BitsRequired)} min={min} needs to be <= max={max}");

            // if min == max then we need 0 bits because it is only ever one value
            if (min == max)
                return 0;

            // normalize from min..max to 0..max-min
            // example:
            //   min = 0, max = 7 => 7-0 = 7 (0..7 = 8 values needed)
            //   min = 4, max = 7 => 7-4 = 3 (0..3 = 4 values needed)
            //
            // CAREFUL: DO NOT ADD ANYTHING TO THIS VALUE.
            //          if min=0 and max=ulong.max then normalized = ulong.max,
            //          adding anything to it would make it overflow!
            //          (see tests!)
            ulong normalized = max - min;
            //UnityEngine.Debug.Log($"min={min} max={max} normalized={normalized}");

            // .Net Core 3.1 has BitOperations.Log2(x)
            // Unity doesn't, so we could use one of a dozen weird tricks:
            // https://stackoverflow.com/questions/15967240/fastest-implementation-of-log2int-and-log2float
            // including lookup tables, float exponent tricks for little endian,
            // etc.
            //
            // ... or we could just hard code!
            if (normalized < 2) return 1;
            if (normalized < 4) return 2;
            if (normalized < 8) return 3;
            if (normalized < 16) return 4;
            if (normalized < 32) return 5;
            if (normalized < 64) return 6;
            if (normalized < 128) return 7;
            if (normalized < 256) return 8;
            if (normalized < 512) return 9;
            if (normalized < 1024) return 10;
            if (normalized < 2048) return 11;
            if (normalized < 4096) return 12;
            if (normalized < 8192) return 13;
            if (normalized < 16384) return 14;
            if (normalized < 32768) return 15;
            if (normalized < 65536) return 16;
            if (normalized < 131072) return 17;
            if (normalized < 262144) return 18;
            if (normalized < 524288) return 19;
            if (normalized < 1048576) return 20;
            if (normalized < 2097152) return 21;
            if (normalized < 4194304) return 22;
            if (normalized < 8388608) return 23;
            if (normalized < 16777216) return 24;
            if (normalized < 33554432) return 25;
            if (normalized < 67108864) return 26;
            if (normalized < 134217728) return 27;
            if (normalized < 268435456) return 28;
            if (normalized < 536870912) return 29;
            if (normalized < 1073741824) return 30;
            if (normalized < 2147483648) return 31;
            if (normalized < 4294967296) return 32;
            if (normalized < 8589934592) return 33;
            if (normalized < 17179869184) return 34;
            if (normalized < 34359738368) return 35;
            if (normalized < 68719476736) return 36;
            if (normalized < 137438953472) return 37;
            if (normalized < 274877906944) return 38;
            if (normalized < 549755813888) return 39;
            if (normalized < 1099511627776) return 40;
            if (normalized < 2199023255552) return 41;
            if (normalized < 4398046511104) return 42;
            if (normalized < 8796093022208) return 43;
            if (normalized < 17592186044416) return 44;
            if (normalized < 35184372088832) return 45;
            if (normalized < 70368744177664) return 46;
            if (normalized < 140737488355328) return 47;
            if (normalized < 281474976710656) return 48;
            if (normalized < 562949953421312) return 49;
            if (normalized < 1125899906842624) return 50;
            if (normalized < 2251799813685248) return 51;
            if (normalized < 4503599627370496) return 52;
            if (normalized < 9007199254740992) return 53;
            if (normalized < 18014398509481984) return 54;
            if (normalized < 36028797018963968) return 55;
            if (normalized < 72057594037927936) return 56;
            if (normalized < 144115188075855872) return 57;
            if (normalized < 288230376151711744) return 58;
            if (normalized < 576460752303423488) return 59;
            if (normalized < 1152921504606846976) return 60;
            if (normalized < 2305843009213693952) return 61;
            if (normalized < 4611686018427387904) return 62;
            if (normalized < 9223372036854775808) return 63;
            return 64;
        }

        // same as Unity's Quaternion.IsEqualUsingDot, which is private
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool IsEqualUsingDot(float dot) => dot > 0.9999989867210388;

        // DOTS quaternion doesn't have .Angle() yet
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float QuaternionAngle(quaternion a, quaternion b)
        {
            // same as Unity's Quaternion.Angle, but for DOTS
            float num = math.min(math.abs(math.dot(a, b)), 1f);
            return IsEqualUsingDot(num) ? 0 : (float) (math.acos(num) * 2.0 * 57.295780181884766);
        }

        // like MemCmp but on a bit size basis
        // compares the lowest (most right) bits first.
        public static unsafe bool BitCmp(byte* a, byte* b, int bitSize)
        {
            // calculate full bytes to compare (without remaining bits)
            // bits.
            // calculation example for up to 9 bits:
            //   1 - 1 =  0 then / 8 = 0
            //   2 - 1 =  1 then / 8 = 0
            //   3 - 1 =  2 then / 8 = 0
            //   4 - 1 =  3 then / 8 = 0
            //   5 - 1 =  4 then / 8 = 0
            //   6 - 1 =  5 then / 8 = 0
            //   7 - 1 =  6 then / 8 = 0
            //   8 - 1 =  7 then / 8 = 0
            //   9 - 1 =  8 then / 8 = 1
            int fullBytes = (bitSize-1) / 8;

            // memcmp the full bytes
            if (UnsafeUtility.MemCmp(a, b, fullBytes) != 0)
                return false;

            // bit compare the remaining bits
            int remainingBits = bitSize - fullBytes * 8;
            if (remainingBits > 0)
            {
                // create a mask with 'n' remaining bits all set to '1'
                // for example, for 8 bits it is 0x000000FF
                // need ulong so we can shift left 32 for 32 bits
                // (would be too much for uint)
                ulong mask = (1ul << remainingBits) - 1;

                // compare remaining bytes, masked
                // (ignore everything beyond remaining bits)
                byte lastA = a[fullBytes];
                byte lastB = b[fullBytes];
                return (lastA & mask) == (lastB & mask);
            }

            return true;
        }
    }
}