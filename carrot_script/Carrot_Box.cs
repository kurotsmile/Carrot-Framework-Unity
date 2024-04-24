using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace Carrot
{
    public enum Carrot_Box_Type {List_Box,Grid_Box };
    public class Carrot_Box : MonoBehaviour
    {
        private Carrot carrot;
        [Header("Box obj main")]
        private Carrot_Box_Type box_type = Carrot_Box_Type.List_Box;
        public Text txt_title;
        public Image img_icon;
        public GameObject obj_list;
        public GameObject obj_grid;
        public Transform area_list_contain;
        public Transform area_grid_contain;
        public Transform area_all_item;
        public GameObject box_list_item_prefab;
        public GameObject box_grid_item_prefab;
        public GameObject box_item_group_prefab;
        public GameObject box_item_panel_btn_prefab;
        public Button btn_close;
        public Carrot_UI UI;
        private UnityAction act_close = null;
        private UnityAction<List<string>> act_close_change_parameter = null;

        [Header("Theme Event")]
        public GameObject[] obj_hat_theme;
        public Image img_scroll_handl;

        [Header("Button Header")]
        public Transform area_btn_header;
        public Transform area_btn_header_right;
        public GameObject box_btn_item_prefab;

        public void load(Carrot carrot)
        {
            this.carrot = carrot;
            if (obj_hat_theme.Length > 0)
            {
                int month_cur = DateTime.Now.Month;
                for (int i = 0; i < obj_hat_theme.Length; i++)
                {
                    if(this.obj_hat_theme[i]!=null) this.obj_hat_theme[i].SetActive(false);
                }
                if (this.obj_hat_theme[month_cur] != null)  this.obj_hat_theme[month_cur].SetActive(true);
            }

            this.Check_type_box();
            this.UI.set_theme(this.carrot.color_highlight);
        }

        public void set_type(Carrot_Box_Type type)
        {
            this.box_type = type;
            this.Check_type_box();
        }

        private void Check_type_box()
        {
            this.obj_list.SetActive(false);
            this.obj_grid.SetActive(false);
            
            if (this.box_type == Carrot_Box_Type.List_Box)
            {
                this.obj_list.SetActive(true);
                this.area_all_item = this.area_list_contain;
            }
            else
            {
                this.obj_grid.SetActive(true);
                this.area_all_item = this.area_grid_contain;
            }  
        }

        public void set_item_size(Vector2 size_cell)
        {
            if(this.box_type==Carrot_Box_Type.Grid_Box)
                this.area_grid_contain.GetComponent<GridLayoutGroup>().cellSize= size_cell;
            else
                foreach(Transform tr in this.area_all_item) tr.GetComponent<RectTransform>().sizeDelta = size_cell;
        }

        public void Set_grid_col(int col)
        {
            this.area_grid_contain.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            this.area_grid_contain.GetComponent<GridLayoutGroup>().constraintCount = col;
        }

        public void close()
        {
            this.carrot.play_sound_click();
            if (this.act_close != null) act_close();
            if (this.act_close_change_parameter != null)
            {
                List<string> arr_item_change = new();
                foreach(Transform tr in this.area_all_item)
                {
                    if (tr.GetComponent<Carrot_Box_Item>()) if (tr.GetComponent<Carrot_Box_Item>().get_change_status()) arr_item_change.Add(tr.name);

                }
                this.act_close_change_parameter(arr_item_change);
            }
            this.UI.close();
        }

        public void set_title(string s_title)
        {
            this.txt_title.text = s_title;
        }

        public void set_icon(Sprite sp_icon)
        {
            this.img_icon.sprite = sp_icon;
        }

        public void set_icon_white(Sprite sp_icon)
        {
            this.img_icon.sprite = sp_icon;
            this.img_icon.color = Color.white;
        }

        public GameObject add_item(GameObject item_obj)
        {
            GameObject item_box = Instantiate(item_obj);
            item_box.transform.SetParent(this.area_all_item);
            item_box.transform.localPosition = new Vector3(item_box.transform.position.x, item_box.transform.position.y, 0f);
            item_box.transform.localScale = new Vector3(1f, 1f, 1f);
            item_box.transform.localRotation = Quaternion.Euler(Vector3.zero);
            return item_box;
        }

        public Carrot_Box_Item_group add_item_group(string s_name_group)
        {
            GameObject item_box = Instantiate(box_item_group_prefab);
            item_box.name = s_name_group;
            item_box.transform.SetParent(this.area_all_item);
            item_box.transform.localPosition = new Vector3(item_box.transform.position.x, item_box.transform.position.y, 0f);
            item_box.transform.localScale = new Vector3(1f, 1f, 1f);
            item_box.transform.localRotation = Quaternion.Euler(Vector3.zero);
            Carrot_Box_Item_group item_group = item_box.GetComponent<Carrot_Box_Item_group>();
            item_group.on_load();
            return item_group;
        }

        private Carrot_Box_Btn_Item add_btn_header(GameObject item_obj,bool is_head_left=true)
        {
            GameObject item_btn_header = Instantiate(item_obj);
            if(is_head_left)
                item_btn_header.transform.SetParent(this.area_btn_header);
            else
                item_btn_header.transform.SetParent(this.area_btn_header_right);
            item_btn_header.transform.localPosition = new Vector3(item_btn_header.transform.position.x, item_btn_header.transform.position.y, 0f);
            item_btn_header.transform.localScale = new Vector3(1f, 1f, 1f);
            item_btn_header.transform.localRotation = Quaternion.Euler(Vector3.zero);
            return item_btn_header.GetComponent<Carrot_Box_Btn_Item>();
        }

        public Carrot_Box_Item create_item(string s_name="Item_Box")
        {
            Carrot_Box_Item box_item_new;
            if (this.box_type == Carrot_Box_Type.Grid_Box)
            {
                box_item_new = this.add_item(this.box_grid_item_prefab).GetComponent<Carrot_Box_Item>();
            }
            else
            {
                box_item_new = this.add_item(this.box_list_item_prefab).GetComponent<Carrot_Box_Item>();
                box_item_new.check_type();
                box_item_new.txt_tip.color = this.carrot.color_highlight;
            }
            box_item_new.on_load(this.carrot);
            box_item_new.name = s_name;
            return box_item_new;
        }

        public Carrot_Box_Item create_item_of_top(string s_name = "Item_Box")
        {
            Carrot_Box_Item box_item_new = this.create_item(s_name);
            box_item_new.transform.SetAsFirstSibling();
            return box_item_new;
        }

        public Carrot_Box_Btn_Panel create_panel_btn()
        {
            GameObject obj_panel_btn = this.add_item(this.box_item_panel_btn_prefab);
            Carrot_Box_Btn_Panel panel_btn = obj_panel_btn.GetComponent<Carrot_Box_Btn_Panel>();
            return panel_btn;
        }

        public Carrot_Box_Item create_item_of_index(string s_name = "Item_Box",int index_Sibling=0)
        {
            Carrot_Box_Item box_item_new = this.add_item(this.box_list_item_prefab).GetComponent<Carrot_Box_Item>();
            box_item_new.name = s_name;
            box_item_new.txt_tip.color = this.carrot.color_highlight;
            box_item_new.transform.SetSiblingIndex(index_Sibling);
            return box_item_new;
        }

        public void set_act_before_closing(UnityAction act)
        {
            this.act_close = act;
        }

        public void set_act_before_closing(UnityAction<List<string>> act)
        {
            this.act_close_change_parameter = act;
        }

        public UnityAction get_act_before_closing()
        {
            return this.act_close;
        }

        public UnityAction<List<string>> get_act_before_closing_change()
        {
            return this.act_close_change_parameter;
        }

        public Carrot_Box_Btn_Item create_btn_menu_header(Sprite sp_icon,bool is_left=true)
        {
            Carrot_Box_Btn_Item btn_header_item = this.add_btn_header(this.box_btn_item_prefab,is_left);
            btn_header_item.icon.sprite = sp_icon;
            return btn_header_item;
        }

        public void update_gamepad_cosonle_control()
        {
            if (this.carrot.type_control != TypeControl.None)
            {
                if (UI.get_list_btn() != null) this.carrot.game.set_list_button_gamepad_console(this.UI.get_list_btn());
                if (UI.scrollRect != null) this.carrot.game.set_scrollRect_gamepad_consoles(this.UI.scrollRect);
            }
        }

        public void update_color_table_row()
        {
            int index_row = 0;
            foreach(Transform tr in this.area_all_item)
            {
                if (tr.GetComponent<Button>() && tr.gameObject.activeInHierarchy)
                {
                    index_row++;
                    if (index_row % 2 == 0) {
                        ColorBlock cb = tr.GetComponent<Button>().colors;
                        cb.normalColor = this.carrot.get_color_highlight_blur(200);
                        tr.GetComponent<Button>().colors = cb;
                    }
                }
            }
        }
    }
}


 
