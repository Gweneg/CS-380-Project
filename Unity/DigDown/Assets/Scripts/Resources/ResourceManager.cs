using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Resources
{
	/// <summary>
	///	Loads and holds references to all art and sound assets that require swapping at run-time (e.g. tile pooling).
	/// </summary>
	public class ResourceManager : MonoBehaviour
	{
		/// <summary>
		/// The singleton instance of the resource manager.
		/// </summary>
		public static ResourceManager Instance;
		
		/// <summary>
		/// All loaded sprites, arranged in order of arbitrary TextureID.
		/// </summary>
		[SerializeField]
		private Sprite[] sprites;

		/// <summary>
		/// Gets and returns the sprite with the given ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static Sprite GetSprite(ushort id)
		{
			return Instance.sprites[id];
		}

		private void Awake()
		{
			// Assign the singleton, catching when there has already been one set.
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Debug.LogError("");
			}
			
		}
	}

}