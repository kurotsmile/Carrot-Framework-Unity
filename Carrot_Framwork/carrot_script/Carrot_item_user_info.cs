using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
namespace Carrot
{
    public class Carrot_item_user_info : MonoBehaviour
    {
        public Image img_icon;
        public Text txt_title;
        public Text txt_value;
        public string type;
        public string act;
        public void set_val_en(string s_data, string s_val)
        {
            IList list_data = (IList)Json.Deserialize(s_data);
            int index_val = int.Parse(s_val);
            this.txt_value.text = list_data[index_val].ToString();
        }

        public void click()
        {
            if (this.act!="")
            {
                GameObject.Find("Carrot").GetComponent<Carrot>().user.act_btn_filed_info_user(this.act);
            }
            else
            {
                if (type == "5") Application.OpenURL("mailto:"+txt_value.text);
                if (type == "7") Application.OpenURL(txt_value.text);
                if (type == "8") Application.OpenURL("tel:" + txt_value.text);
                if (type == "9") Application.OpenURL("https://maps.google.com/maps?q="+ UnityWebRequest.EscapeURL(txt_value.text));
            }
        }

    }
}
