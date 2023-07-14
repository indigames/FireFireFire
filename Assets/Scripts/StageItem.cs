using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageItem : MonoBehaviour
{
    [Range(1, 5)]
    public int spreadSpeed = 3;
    public StageItemInfo info;


#if UNITY_EDITOR
    void OnValidate()
    {
        if (!this.info)
        {
            StageItemInfo info = this.GetComponent<StageItemInfo>();
            if (!info)
            {
                this.info = this.gameObject.AddComponent<StageItemInfo>();
            }
            else
            {
                this.info = info;
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}
