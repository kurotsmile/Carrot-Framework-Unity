using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Carrot
{
    public class Carrot_Box_Item_group : MonoBehaviour
    {
        [Header("Emp UI")]
        public Image img_icon;
        public Image img_icon_collect;

        [Header("Asset Icon")]
        public Sprite sp_icon_collect_true;
        public Sprite sp_icon_collect_false;

        private bool is_collect_true = true;
        private List<Carrot_Box_Item> list_box_item;
        private int index;

        public void on_load()
        {
            this.list_box_item = new List<Carrot_Box_Item>();
        }

        public void btn_collect()
        {
            if (this.is_collect_true)
            {
                this.is_collect_true = false;
                this.img_icon_collect.sprite = this.sp_icon_collect_true;
                this.act_show_all_child(false);
            }
            else
            {
                this.is_collect_true = true;
                this.img_icon_collect.sprite = this.sp_icon_collect_false;
                this.act_show_all_child(true);
            }
        }

        public void add_item(Carrot_Box_Item box_item)
        {
            this.list_box_item.Add(box_item);
        }

        private void act_show_all_child(bool is_show)
        {
            for(int i = 0; i < this.list_box_item.Count; i++)
            {
                this.list_box_item[i].gameObject.SetActive(is_show);
            }
        }

        public void set_icon(Sprite sp_icon)
        {
            this.img_icon.sprite = sp_icon;
        }
    }
}
