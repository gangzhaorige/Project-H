using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using ProjectH.Models;

public class ChampionAssetManager : MonoBehaviour
{
    private static ChampionAssetManager _instance;
    public static ChampionAssetManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<ChampionAssetManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("ChampionAssetManager");
                    _instance = go.AddComponent<ChampionAssetManager>();
                }
            }
            return _instance;
        }
    }

    private Dictionary<int, ChampionSO> championSOCache = new Dictionary<int, ChampionSO>();
    private Dictionary<string, AsyncOperationHandle<Sprite>> spriteHandles = new Dictionary<string, AsyncOperationHandle<Sprite>>();
    private List<AsyncOperationHandle> allHandles = new List<AsyncOperationHandle>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void OnDestroy()
    {
        foreach (var handle in allHandles)
        {
            if (handle.IsValid()) Addressables.Release(handle);
        }
        allHandles.Clear();
        spriteHandles.Clear();
        championSOCache.Clear();
    }

    public void GetChampionSO(int id, System.Action<ChampionSO> callback)
    {
        if (championSOCache.TryGetValue(id, out ChampionSO so))
        {
            callback?.Invoke(so);
            return;
        }

        string address = $"Data/Champions/{id}_champ_data";
        var handle = Addressables.LoadAssetAsync<ChampionSO>(address);
        allHandles.Add(handle);
        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                championSOCache[id] = op.Result;
                callback?.Invoke(op.Result);
            }
        };
    }

    public void GetSprite(AssetReferenceSprite assetRef, System.Action<Sprite> callback)
    {
        if (assetRef == null || !assetRef.RuntimeKeyIsValid())
        {
            callback?.Invoke(null);
            return;
        }

        string key = assetRef.AssetGUID;
        if (spriteHandles.TryGetValue(key, out AsyncOperationHandle<Sprite> handle))
        {
            if (handle.IsDone)
            {
                callback?.Invoke(handle.Result);
            }
            else
            {
                handle.Completed += (op) => callback?.Invoke(op.Result);
            }
            return;
        }

        var newHandle = Addressables.LoadAssetAsync<Sprite>(assetRef);
        spriteHandles[key] = newHandle;
        allHandles.Add(newHandle);
        newHandle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                callback?.Invoke(op.Result);
            }
        };
    }
}
