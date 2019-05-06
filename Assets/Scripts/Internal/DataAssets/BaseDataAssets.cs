using System;
using UnityEngine;

namespace DataAssets
{
	public abstract class BaseDataAssets<SELF, Data> : ScriptableObject, IAssets<Data> where SELF : UnityEngine.Object, IAssets<Data>
	{
		public const string ASSETS_RESOURCE_PATH = "DataAssets/";

		public static SELF Instance
		{
			get
			{
				if(_cachedAssets == null)
				{
					if(_cachedAssets == null)
					{
						_cachedAssets = ResourceLocator.Locate<SELF>("", ASSETS_RESOURCE_PATH);
					}

					if(_cachedAssets == null)
					{
						throw new Exception($"No `{typeof(SELF).Name}` Assets object found at Resource location: " + ASSETS_RESOURCE_PATH);
					}
					else
					{
						if(!_cachedAssets.RefreshCache(out string message))
						{
							throw new Exception(message);
						}
					}
				}

				return _cachedAssets;
			}
		}

		private static SELF _cachedAssets = null;

		public abstract bool TryGetAsset(string assetID, out Data data);
		public abstract bool HasAsset(string assetID);
		public virtual bool RefreshCache(out string message)
		{
			message = string.Empty;
			return true;
		}
	}

	public interface IAssets<Data>
	{
		bool TryGetAsset(string assetID, out Data data);
		bool HasAsset(string assetID);
		bool RefreshCache(out string message);
	}
}