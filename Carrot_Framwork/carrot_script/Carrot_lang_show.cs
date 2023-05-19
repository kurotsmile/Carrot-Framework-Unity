using UnityEngine;
using UnityEngine.UI;

namespace Carrot
{
    public class Carrot_lang_show : MonoBehaviour
    {
        public string[] key;
        public Text[] emp;
        [Tooltip("Các đối tượng ảnh muốn thay đổi biểu tượng quốc gia")]
        public Image[] List_img_change_icon_lang;

        public void load_lang_emp()
        {
            for (int i = 0; i < this.emp.Length; i++) 
                if (PlayerPrefs.GetString(this.key[i], "") != "") 
                    this.emp[i].text = PlayerPrefs.GetString(this.key[i]);
        }

        public void load_lang_emp(Sprite sp_lang_cur)
        {
            this.load_lang_emp();
            for (int i = 0; i < this.List_img_change_icon_lang.Length; i++)
            {
                List_img_change_icon_lang[i].sprite = sp_lang_cur;
            }
        }
    }


}
