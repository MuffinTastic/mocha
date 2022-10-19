﻿namespace Mocha.Renderer;

/*
 * Generators for internal textures (missing texture etc.)
 */
public partial class TextureBuilder
{
	private static Texture? zero;
	public static Texture Zero
	{
		get
		{
			if ( zero == null )
				CreateZeroTexture();

			return zero;
		}
	}
	private static Texture? one;
	public static Texture One
	{
		get
		{
			if ( one == null )
				CreateOneTexture();

			return one;
		}
	}


	private static Texture? missingTexture;
	public static Texture MissingTexture
	{
		get
		{
			if ( missingTexture == null )
				CreateMissingTexture();

			return missingTexture;
		}
	}

	public static void CreateOneTexture()
	{
		{
			var missingTextureData = new byte[]
			{
				255, 255, 255, 255,
			};

			one = new TextureBuilder()
				.FromData( missingTextureData, 1, 1 )
				.WithName( "internal:one" )
				.Build();
		}
	}


	public static void CreateZeroTexture()
	{
		{
			var missingTextureData = new byte[]
			{
				0, 0, 0, 255,
			};

			zero = new TextureBuilder()
				.FromData( missingTextureData, 1, 1 )
				.WithName( "internal:zero" )
				.Build();
		}
	}

	public static void CreateMissingTexture()
	{
		//
		// Missing texture
		//
		{
			var missingTextureData = new byte[]
			{
				//
				0, 0, 0, 255,		// B
				255, 0, 255, 255,	// P
				0, 0, 0, 255,		// B
				255, 0, 255, 255,	// P

				//
				255, 0, 255, 255,	// P
				0, 0, 0, 255,		// B
				255, 0, 255, 255,	// P
				0, 0, 0, 255,		// B

				//
				0, 0, 0, 255,		// B
				255, 0, 255, 255,	// P
				0, 0, 0, 255,		// B
				255, 0, 255, 255,	// P

				//
				255, 0, 255, 255,	// P
				0, 0, 0, 255,		// B
				255, 0, 255, 255,	// P
				0, 0, 0, 255,		// B
			};

			missingTexture = new TextureBuilder()
				.FromData( missingTextureData, 4, 4 )
				.WithName( "internal:missing" )
				.Build();
		}
	}
}
