﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTarget : StageBase
{
    public enum Attachment
    {
        NONE,
        TOP_ROPE,
        SIDE_ROPES
    }

    public Attachment attachment = Attachment.NONE;
    [Range(1, 5)]
    public int spreadSpeed = 3;

}
