using System.Collections.Generic;
using UnityEngine;

namespace DataAssets
{
	public abstract class BaseListedDataAssets<SELF, Data> : BaseDataAssets<SELF, Data> where Data : IListedAssetData where SELF : Object, IAssets<Data>
	{
		[SerializeField]
		protected List<Data> _assetsData = null;

		private Dictionary<string, Data> _assetsCached;

		public override bool RefreshCache(out string message)
		{
			_assetsCached = new Dictionary<string, Data>();
			message = string.Empty;
			for(int i = 0; i < _assetsData.Count; i++)
			{
				Data data = _assetsData[i];
				if(_assetsCached.ContainsKey(data.AssetID))
				{
					message = $"Already detected Asset with ID {data.AssetID}! Process canceled!";
					return false;
				}

				_assetsCached[data.AssetID] = data;
			}
			return true;
		}

		public Data[] AssetsData
		{
			get
			{
				return _assetsData.ToArray();
			}
		}

		public override bool TryGetAsset(string assetID, out Data data)
		{
			return _assetsCached.TryGetValue(assetID, out data);
		}

		public override bool HasAsset(string assetID)
		{
			return _assetsCached.ContainsKey(assetID);
		}
	}

	public interface IListedAssetData
	{
		string AssetID
		{
			get;
		}
	}
}