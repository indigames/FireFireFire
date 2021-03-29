using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachmentUtil : MonoBehaviour
{
    public GameObject attachmentTopRope, attachmentSideRopes;
    static AttachmentUtil instance = null;

    private void Awake()
    {
        instance = this;
    }

    public static GameObject GetAttachmentTemplate(StageTarget.Attachment type)
    {
        if (type == StageTarget.Attachment.NONE) return null;
        if (type == StageTarget.Attachment.TOP_ROPE) return instance.attachmentTopRope;
        if (type == StageTarget.Attachment.SIDE_ROPES) return instance.attachmentSideRopes;
        return null;
    }
}
