using System.Runtime.InteropServices;

namespace DdsKtxSharp
{
#if !DDSKTXSHARP_INTERNAL
	public
#else
	internal
#endif
	static unsafe partial class DdsKtx
	{
		public static string LastError;

		private static unsafe void dds_ktx_err(string error)
		{
			ddsktx__dds_translate_pixel_format f = new ddsktx__dds_translate_pixel_format { bit_mask = new uint[] { 1, 2, 3, 4 } };

			LastError = error;
		}
	}
}
