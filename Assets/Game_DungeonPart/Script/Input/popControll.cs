﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class popControll : EventTrigger
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        GameObject.Find("ItemPanel").GetComponent<ItemButtonManager>().BlockkingOnClick();
    }
}
