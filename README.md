# DdsKtxSharp
[![NuGet](https://img.shields.io/nuget/v/DdsKtxSharp.svg)](https://www.nuget.org/packages/DdsKtxSharp/) 
![Build & Publish](https://github.com/rds1983/DdsKtxSharp/workflows/Build%20&%20Publish/badge.svg)
[![Chat](https://img.shields.io/discord/628186029488340992.svg)](https://discord.gg/ZeHxhCY)

DdsKtxSharp is C# port of the https://github.com/septag/dds-ktx, which is C library to parse images in DDS and KTX formats.

# Adding Reference
There are two ways of referencing DdsKtxSharp in the project:
1. Through nuget: https://www.nuget.org/packages/DdsKtxSharp/
2. As submodule:
    
    a. `git submodule add https://github.com/rds1983/DdsKtxSharp.git`
    
    b. Now there are two options:
       
      * Add src/DdsKtxSharp.csproj to the solution
       
      * Include *.cs from folder "src" directly in the project. In this case, it might make sense to add DDSKTXSHARP_INTERNAL build compilation symbol to the project, so DdsKtxSharp classes would become internal.
     
# Usage
DdsKtxSharp exposes API similar to dds-ktx.h. However that API is complicated and deals with raw unsafe pointers.

Thus utility class DdsKtxParser had been made to wrap that functionality.

Simple code to load a DDS/KTX image
```c# 
    DdsKtxParser parser = DdsKtxParser.FromMemory(data);

    ddsktx_texture_info info = parser.Info;
    Console.WriteLine("Format: " + info.format);
    Console.WriteLine("Flags: " + info.flags);
    Console.WriteLine("Width: " + info.width);
    Console.WriteLine("Height: " + info.height);
    Console.WriteLine("Bpp: " + info.bpp);

    ddsktx_sub_data sub_data;
    byte[] imageData = parser.GetSubData(0, 0, 0, out sub_data);
```

If you are writing MonoGame application and would like to convert that data to the Texture2D. It could be done following way:
```c#
    SurfaceFormat format = SurfaceFormat.Color;
    switch (info.format)
    {
        case ddsktx_format.DDSKTX_FORMAT_BC1:
            format = SurfaceFormat.Dxt1;
            break;

        case ddsktx_format.DDSKTX_FORMAT_BC2:
            format = SurfaceFormat.Dxt3;
            break;

        case ddsktx_format.DDSKTX_FORMAT_BC3:
            format = SurfaceFormat.Dxt5;
            break;

        case ddsktx_format.DDSKTX_FORMAT_BGRA8:
            // Switch B and R
            for (var i = 0; i < imageData.Length / 4; ++i)
            {
                var temp = imageData[i * 4];
                imageData[i * 4] = imageData[i * 4 + 2];
                imageData[i * 4 + 2] = temp;
                imageData[i * 4 + 3] = 255;
            }

            break;

        case ddsktx_format.DDSKTX_FORMAT_RGB8:
            // Add alpha channel
            var newImageData = new byte[info.width * info.height * 4];
            for (var i = 0; i < newImageData.Length / 4; ++i)
            {
                newImageData[i * 4] = imageData[i * 3 + 2];
                newImageData[i * 4 + 1] = imageData[i * 3 + 1];
                newImageData[i * 4 + 2] = imageData[i * 3];
                newImageData[i * 4 + 3] = 255;
            }

            imageData = newImageData;
            break;

        default:
            throw new Exception("Format " + info.format.ToString() + "isn't supported.");
    }

    Texture2D texture = new Texture2D(GraphicsDevice, info.width, info.height, false, format);
    texture.SetData(imageData);
```

# License
BSD 2-Clause License

# Credits
https://github.com/septag/dds-ktx
