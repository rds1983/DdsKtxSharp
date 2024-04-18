using System;

namespace DdsKtxSharp
{
	internal static unsafe class CRuntime
	{
		public static void memcpy(void* a, void* b, long size)
		{
			Buffer.MemoryCopy(b, a, size, size);
		}

		public static void memcpy(void* a, void* b, ulong size)
		{
			memcpy(a, b, (long)size);
		}

		public static int memcmp(void* a, void* b, long size)
		{
			var result = 0;
			var ap = (byte*)a;
			var bp = (byte*)b;
			for (long i = 0; i < size; ++i)
			{
				if (*ap != *bp)
					result += 1;

				ap++;
				bp++;
			}

			return result;
		}

		public static int memcmp(void* a, void* b, ulong size)
		{
			return memcmp(a, b, (long)size);
		}

		public static int memcmp(byte* a, byte[] b, ulong size)
		{
			fixed (void* bptr = b)
			{
				return memcmp(a, bptr, (long)size);
			}
		}

		public static void memset(void* ptr, int value, long size)
		{
			var bptr = (byte*)ptr;
			var bval = (byte)value;
			for (long i = 0; i < size; ++i)
				*bptr++ = bval;
		}

		public static void memset(void* ptr, int value, ulong size)
		{
			memset(ptr, value, (long)size);
		}
	}
}