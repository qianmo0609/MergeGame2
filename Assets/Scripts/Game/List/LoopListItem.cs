using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopListItem : MonoBehaviour
{
    [SerializeField] UISprite icon;
    [SerializeField] UILabel numLabel;
    
    public void onInitInfo(string name,int num)
    {
        this.icon.spriteName = name;
        this.numLabel.text = num.ToString();
    }
}
