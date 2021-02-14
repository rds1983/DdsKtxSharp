﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sichem;

namespace DdsKtxSharp.Generator
{
	class Program
	{
		private static void Write(Dictionary<string, string> input, StringBuilder output)
		{
			foreach (var pair in input)
			{
				output.Append(pair.Value);
			}
		}

		private static string PostProcess(string data)
		{
			data = Utility.ReplaceNativeCalls(data);

			data = data.Replace("((void*)(0))", "null");
			data = data.Replace("((void *)(0))", "null");
			data = data.Replace("(bool)(0)", "false");
			data = data.Replace("(bool)(1)", "true");
			data = data.Replace("bit_mask = {", "bit_mask = new uint[] {");

			data = data.Replace("(sizeof((k__translate_ktx_fmt)) / sizeof(ddsktx__ktx_format_info))",
				"k__translate_ktx_fmt.Length");
			data = data.Replace("(int)(sizeof((k__translate_ktx_fmt2)) / sizeof(ddsktx__ktx_format_info2))",
				"k__translate_ktx_fmt2.Length");
			data = data.Replace("(int)(sizeof((k__translate_dds_fourcc)) / sizeof(ddsktx__dds_translate_fourcc_format))",
				"k__translate_dds_fourcc.Length");
			data = data.Replace("(int)(sizeof((k__translate_dds_pixel)) / sizeof(ddsktx__dds_translate_pixel_format))",
				"k__translate_dds_pixel.Length");
			data = data.Replace("(int)(sizeof((k__translate_dxgi)) / sizeof(ddsktx__dds_translate_fourcc_format))",
				"k__translate_dxgi.Length");

			data = data.Replace("((cubemap) != 0)", "(cubemap)");
			data = data.Replace("((has_alpha) != 0)", "(has_alpha)");
			data = data.Replace("((srgb) != 0)", "(srgb)");

			data = data.Replace("(header.caps1 & 0x00400000)?", "(header.caps1 & 0x00400000) != 0?");
			data = data.Replace("(k__formats_info[format].has_alpha) != 0", "(k__formats_info[format].has_alpha)");

			return data;
		}

		static void Process()
		{
			var parameters = new ConversionParameters
			{
				InputPath = @"dds-ktx.h",
				Defines = new[]
				{
					"DDSKTX_IMPLEMENT"
				},
				SkipStructs = new string[]
				{
					"stbir__filter_info",
					"stbir__info",
					"stbir__FP32",
				},
				SkipGlobalVariables = new string[]
				{
					"stbir__filter_info_table"
				},
				SkipFunctions = new[]
				{
					"dds_ktx_err"
				},
				Classes = new string[]
				{
					"stbir__info",
					"ddsktx__dds_translate_pixel_format"
				},
				GlobalArrays = new string[]
				{
				}
			};

			var cp = new ClangParser();

			var result = cp.Process(parameters);

			// Write output
			var sb = new StringBuilder();
			sb.AppendLine(string.Format("// Generated by Sichem at {0}", DateTime.Now));
			sb.AppendLine();

			sb.AppendLine("using System;");
			sb.AppendLine("using System.Runtime.InteropServices;");

			sb.AppendLine();

			sb.Append("namespace DdsKtxSharp\n{\n\t");
			sb.AppendLine("unsafe partial class DdsKtx\n\t{");

			Write(result.Constants, sb);
			Write(result.GlobalVariables, sb);
			Write(result.Enums, sb);
			Write(result.Structs, sb);
			Write(result.Methods, sb);

			sb.Append("}\n}");
			var data = sb.ToString();

			// Post processing
			Logger.Info("Post processing...");
			data = PostProcess(data);

			File.WriteAllText(@"..\..\..\..\..\src\DdsKtxSharp.Generated.cs", data);
		}

		static void Main(string[] args)
		{
			try
			{
				Process();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}

			Console.WriteLine("Finished. Press any key to quit.");
			Console.ReadKey();
		}
	}
}