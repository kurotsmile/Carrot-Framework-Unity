using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Carrot_item_app : MonoBehaviour
{
    public Text txt_name;
    public Image img_icon;
    public string link;

    public void click()
    {
        Application.OpenURL(this.link);
    }
}
