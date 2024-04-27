using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Carrot
{
    public class Carrot_UI : MonoBehaviour
    {
        [Header("Window Carrot")]
        public int index_window;
        public bool is_not_destroy;

        [Header("Theme")]
        public Image[] emp_theme;
        public Text[] emp_theme_txt;

        [Header("Gamepad and D-pad Carrot")]
        public Transform[] area_btns;
        public GameObject[] obj_gamepad;
        public ScrollRect scrollRect;

        public List<GameObject> get_list_btn()
        {
            List<GameObject> list_btn = new();
            for (int i = 0; i < area_btns.Length; i++)
                foreach (Transform btn in this.area_btns[i])
                    list_btn.Add(btn.gameObject);

            for (int i=0;i<this.obj_gamepad.Length;i++) list_btn.Add(this.obj_gamepad[i]);
            return list_btn;
        }

        public void set_theme(Color32 color)
        {
            for (int i = 0; i < this.emp_theme.Length; i++) this.emp_theme[i].color = color;
            for (int i = 0; i < this.emp_theme_txt.Length; i++) this.emp_theme_txt[i].color = color;
        }

        public void close()
        {
            if (!GameObject.Find("Carrot").GetComponent<Carrot>().close_window(this.index_window))
            {
                if (!this.GetComponent<Carrot_Gamepad>()) Destroy(this.gameObject);
            }
        }
    }
}
