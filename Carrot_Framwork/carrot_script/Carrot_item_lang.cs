using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Carrot
{
    public class Carrot_item_lang : MonoBehaviour
    {
        public Text txt_name;
        public Image img_icon;
        public GameObject obj_data_offline;
        public GameObject obj_suggestions;
        public string s_key_lang;
        public int index;

        public void click()
        {
            GameObject.Find("Carrot").GetComponent<Carrot_lang>().load_data_lang(this);
        }
    }
}
