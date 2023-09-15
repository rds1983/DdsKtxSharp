using System;
using System.Runtime.InteropServices;
using static DdsKtxSharp.DdsKtx;

namespace DdsKtxSharp
{
#if !DDSKTXSHARP_INTERNAL
	public
#else
	internal
#endif
	unsafe class DdsKtxParser
	{
		private readonly byte[] _data;

		public readonly ddsktx_texture_info Info;

		private DdsKtxParser(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			_data = data;

			fixed (ddsktx_texture_info* infoPtr = &Info)
			fixed (byte* dataPtr = data)
			{
				if (!ddsktx_parse(infoPtr, dataPtr, data.Length))
				{
					throw new Exception(LastError);
				}
			}
		}

		public byte[] GetSubData(int arrayIndex, int sliceFaceIndex, int mipIndex, out ddsktx_sub_data sub_data)
		{
			fixed (byte* dataPtr = _data)
			fixed (ddsktx_texture_info* infoPtr = &Info)
			fixed (ddsktx_sub_data* subDataPtr = &sub_data)
			{
				ddsktx_get_sub(infoPtr, subDataPtr, dataPtr, _data.Length, arrayIndex, sliceFaceIndex, mipIndex);

				var result = new byte[sub_data.size_bytes];
				Marshal.Copy(new IntPtr(sub_data.buff), result, 0, result.Length);
				return result;
			}

		}

		public static DdsKtxParser FromMemory(byte[] data)
		{
			return new DdsKtxParser(data);
		}
	}
}
