using UnityEngine.U2D;
using UnityEngine;
using UnityEngine.AddressableAssets;

// A set of useful functions


[System.Serializable]
public class SceneAssetReference : AssetReference
{
    public SceneAssetReference(string guid) : base(guid)
    {
    }

    public override bool ValidateAsset(string path)
    {
        return path.EndsWith(".unity");
    }
}

[System.Serializable]
public class SOAssetReference<T> : AssetReferenceT<T> where T : ScriptableObject
{
    public SOAssetReference(string guid) : base(guid)
    {
    }

    public void TryLoadAsync(System.Action<T> callback)
    {
        if (this.IsValid())
        {
            if (this.Asset != null)
            {
                callback?.Invoke((T)this.Asset);
                return;
            }
            else
            {
                this.ReleaseAsset();
            }
        }

        this.LoadAssetAsync().Completed += (handle) => callback(handle.Result);
    }
}

[System.Serializable]
public class AudioAssetReference : AssetReferenceT<AudioClip>
{
    public AudioAssetReference(string guid) : base(guid)
    {
    }

    public void TryLoadAsync(System.Action<AudioClip> callback)
    {
        if (this.IsValid())
        {
            if (this.Asset != null)
            {
                callback?.Invoke((AudioClip)this.Asset);
                return;
            }
            else
            {
                this.ReleaseAsset();
            }
        }

        this.LoadAssetAsync().Completed += (handle) => callback(handle.Result);
    }
}

[System.Serializable]
public class CustomAssetReference : AssetReferenceGameObject
{
    public CustomAssetReference(string guid) : base(guid)
    {
    }

    public void TryLoadAsync(System.Action<GameObject> callback)
    {
        if (this.IsValid())
        {
            if (this.Asset != null)
            {
                callback?.Invoke((GameObject)this.Asset);
                return;
            }
            else
            {
                this.ReleaseAsset();
            }
        }

        this.LoadAssetAsync().Completed += (handle) => callback(handle.Result);
    }
}

[System.Serializable]
public class SpriteAtlasReference : AssetReferenceT<SpriteAtlas>
{
    public SpriteAtlasReference(string guid) : base(guid)
    {
    }

    public void TryLoadAsync(System.Action<SpriteAtlas> callback)
    {
        if (this.IsValid())
        {
            if (this.Asset != null)
            {
                callback?.Invoke((SpriteAtlas)this.Asset);
                return;
            }
            else
            {
                this.ReleaseAsset();
            }
        }

        this.LoadAssetAsync().Completed += (handle) => callback(handle.Result);
    }
}