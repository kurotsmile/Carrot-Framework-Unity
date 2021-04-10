using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Carrot_item_user_info : MonoBehaviour
{
    public Image img_icon;
    public Text txt_title;
    public Text txt_value;
    public string act;

    public void click()
    {
        if (this.act =="2")
        {
            Application.OpenURL(txt_value.text);
        }
        else
        {
            GameObject.Find("Carrot").GetComponent<Carrot.Carrot>().act_btn_filed_info_user(this.act);
        }
    }

}
