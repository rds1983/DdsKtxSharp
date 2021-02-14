using System;
using System.IO;
using DdsKtxSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static DdsKtxSharp.DdsKtx;

namespace StbImageSharp.Samples.MonoGame
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class ViewerGame : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private readonly string _filePath;
		private Texture2D _texture;

		public ViewerGame(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				throw new ArgumentNullException(nameof(filePath));
			}

			_filePath = filePath;

			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1400,
				PreferredBackBufferHeight = 960
			};

			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			Window.AllowUserResizing = true;
		}
		
		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			// Load image data into memory
			var data = File.ReadAllBytes(_filePath);

			var parser = DdsKtxParser.FromMemory(data);

			var format = SurfaceFormat.Color;

			var info = parser.Info;
			Console.WriteLine("Format: " + info.format);
			Console.WriteLine("Flags: " + info.flags);
			Console.WriteLine("Width: " + info.width);
			Console.WriteLine("Height: " + info.height);
			Console.WriteLine("Bpp: " + info.bpp);

			ddsktx_sub_data sub_data;
			var imageData = parser.GetSubData(0, 0, 0, out sub_data);

			switch (info.format)
			{
				case DdsKtx.ddsktx_format.DDSKTX_FORMAT_BC1:
					format = SurfaceFormat.Dxt1;
					break;

				case DdsKtx.ddsktx_format.DDSKTX_FORMAT_BC2:
					format = SurfaceFormat.Dxt3;
					break;

				case DdsKtx.ddsktx_format.DDSKTX_FORMAT_BC3:
					format = SurfaceFormat.Dxt5;
					break;

				case DdsKtx.ddsktx_format.DDSKTX_FORMAT_BGRA8:
					// Switch B and R
					for (var i = 0; i < imageData.Length / 4; ++i)
					{
						var temp = imageData[i * 4];
						imageData[i * 4] = imageData[i * 4 + 2];
						imageData[i * 4 + 2] = temp;
						imageData[i * 4 + 3] = 255;
					}

					break;

				case DdsKtx.ddsktx_format.DDSKTX_FORMAT_RGB8:
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

			_texture = new Texture2D(GraphicsDevice, info.width, info.height, false, format);
			_texture.SetData(imageData);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
			_spriteBatch.Begin();

			_spriteBatch.Draw(_texture, Vector2.Zero, Color.White);

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}