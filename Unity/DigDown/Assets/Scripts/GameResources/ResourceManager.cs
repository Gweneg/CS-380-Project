using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameResources
{
	/// <summary>
	/// The unique name/ID of the sprite.
	/// </summary>
	public enum SpriteName : ushort
	{
		None,
		GrassToDirt,
		Dirt,
		DirtToDirter,
		Dirter,
		DirterToDirtest,
		Dirtest,
		DirtestToStone,
		Stone,
		MoltenRock,
		Lava,
		Void,
		IronOreOverlay,
		CopperOreOverlay,
		SilverOreOverlay,
		GoldOreOverlay,
		DiamondOreOverlay,
		AzuriteOreOverlay,
		CloudOneOverlay,
		CloudTwoOverlay,
		CloudThreeOverlay,
		SkyBackground,
		DirtBackground,
		DirterBackground,
		DirtestBackground,
		StoneBackground,
		DeepStoneBackground,
		LavaBackground
	}

	/// <summary>
	///	Loads and holds references to all art and sound assets that require swapping at run-time (e.g. tile pooling).
	/// </summary>
	public class ResourceManager : MonoBehaviour
	{
		/// <summary>
		/// The singleton instance of the resource manager.
		/// </summary>
		public static ResourceManager Instance;

		// SPRITE DICTIONARY INITIALIZATION
		// Todo: Awful hacky workaround to Unity not serializing dictionaries.
		[Serializable]
		private struct SpriteDictionaryPair
		{
			public SpriteName id;
			public Sprite sprite;
		}
		[SerializeField]
		private List<SpriteDictionaryPair> spritesInit;
		
		/// <summary>
		/// All loaded sprites, arranged in order of arbitrary TextureID.
		/// </summary>
		private Dictionary<SpriteName, Sprite> _sprites;

		/// <summary>
		/// Gets and returns the sprite with the given ID.
		/// </summary>
		/// <param name="id">The ID of the sprite to get.</param>
		/// <returns>The sprite of given ID.</returns>
		public static Sprite GetSprite(SpriteName id)
		{
			if (!Instance._sprites.ContainsKey(id)) throw new ArgumentOutOfRangeException();
			
			return Instance._sprites[id];
		}

		private void Awake()
		{
			// Initialize if this is the first singleton.
			if (Instance == null)
			{
				Instance = this;
				
				// Convert the serialized values to be the dictionary.
				_sprites = spritesInit.ToDictionary(entry => entry.id, entry => entry.sprite);
			}
			else
			{
				Debug.LogError("Attempted to make second instance.");
			}
		}
	}

}