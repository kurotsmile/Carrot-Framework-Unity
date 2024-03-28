using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Carrot
{
    public class Carrot_camera : MonoBehaviour
    {
        public Carrot carrot;
        public GameObject window_camera_prefab;
        public GameObject window_photoshop_prefab;
        private Carrot_Box box_grid_list_photo;
        int leng_img_photo = 0;

        public void On_load()
        {
            this.leng_img_photo = PlayerPrefs.GetInt("leng_img_photo", 0);
        }

        [ContextMenu("Show Camera")]
        public Carrot_Window_Camera Show_camera()
        {
            GameObject camera_window = this.carrot.create_window(this.window_camera_prefab);
            Carrot_Window_Camera window_cam = camera_window.GetComponent<Carrot_Window_Camera>();
            window_cam.load(this.carrot);
            Carrot_UI camera_ui = camera_window.GetComponent<Carrot_UI>();
            if (this.carrot.type_control != TypeControl.None) this.carrot.game.set_list_button_gamepad_console(camera_ui.get_list_btn());
            return window_cam;
        }

        public void Show_camera(UnityAction<Texture2D> act_after_take_photo)
        {
            this.Show_camera().set_act_after_take_photo(act_after_take_photo);
        }

        public void Show_list_img(UnityAction<Texture2D> act_after_take_photo)
        {
            this.box_grid_list_photo = this.carrot.show_grid();
            this.box_grid_list_photo.set_title(this.carrot.lang.Val("list_photo_camera", "List Photo Cameras"));
            this.box_grid_list_photo.set_item_size(new Vector2(100f, 130f));
            for (int i = 0; i < this.leng_img_photo; i++)
            {
                if (PlayerPrefs.GetString("carrot_img_photo_" + i, "") != "")
                {
                    var index_photo = i;
                    Carrot_Box_Item box_item = this.box_grid_list_photo.create_item();
                    box_item.img_icon.sprite = this.carrot.get_tool().get_sprite_to_playerPrefs("carrot_img_photo_" + i);
                    box_item.set_act(() => this.Act_show_photo_in_list(index_photo));

                    Carrot_Box_Btn_Item btn_select = box_item.create_item();
                    btn_select.set_icon(this.carrot.user.icon_user_done);
                    btn_select.set_color(this.carrot.color_highlight);
                    btn_select.set_act(() => this.Act_select_photo_in_list(act_after_take_photo, index_photo));

                    Carrot_Box_Btn_Item btn_view = box_item.create_item();
                    btn_view.set_icon(this.carrot.icon_carrot_all_category);
                    btn_view.set_color(this.carrot.color_highlight);
                    Destroy(btn_view.GetComponent<Button>());

                    Carrot_Box_Btn_Item btn_delete = box_item.create_item();
                    btn_delete.set_icon(this.carrot.sp_icon_del_data);
                    btn_delete.set_color(this.carrot.color_highlight);
                    btn_delete.set_act(() => this.Act_delete_photo_in_list(box_item.gameObject, index_photo));
                }
            }

            if (this.carrot.type_control != TypeControl.None)
            {
                this.carrot.game.set_list_button_gamepad_console(box_grid_list_photo.UI.get_list_btn());
                this.carrot.game.set_scrollRect_gamepad_consoles(box_grid_list_photo.UI.scrollRect);
            }
        }

        private void Act_show_photo_in_list(int index_photo)
        {
            Texture2D photo = this.carrot.get_tool().get_texture2D_to_playerPrefs("carrot_img_photo_" + index_photo);
            this.Show_photoshop(photo);
        }

        private void Act_select_photo_in_list(UnityAction<Texture2D> act_after_take_photo,int index_photo)
        {
            this.box_grid_list_photo.close();
            Texture2D photo=this.carrot.get_tool().get_texture2D_to_playerPrefs("carrot_img_photo_" + index_photo);
            act_after_take_photo(photo);
        }

        private void Act_delete_photo_in_list(GameObject obj,int index_photo)
        {
            PlayerPrefs.DeleteKey("carrot_name_photo_" + index_photo);
            PlayerPrefs.DeleteKey("carrot_img_photo_" + index_photo);
            Destroy(obj);
        }

        public void Add_photo(Texture2D photo_Texture)
        {
            this.carrot.get_tool().PlayerPrefs_Save_texture2D("carrot_img_photo_" + this.leng_img_photo, photo_Texture);
            PlayerPrefs.SetString("carrot_name_photo_" + this.leng_img_photo, System.DateTime.Now.ToString());
            this.leng_img_photo++;
            PlayerPrefs.SetInt("leng_img_photo", this.leng_img_photo);
        }

        public void Show_photoshop(Texture2D photo,float scale=0f)
        {
            GameObject window_photoshop=this.carrot.create_window(this.window_photoshop_prefab);
            Carrot_Window_Photoshop photoshop = window_photoshop.GetComponent<Carrot_Window_Photoshop>();
            photoshop.show(photo,scale);
            photoshop.UI.set_theme(this.carrot.color_highlight);
            if (this.carrot.type_control != TypeControl.None) this.carrot.game.set_list_button_gamepad_console(photoshop.UI.get_list_btn());
        }

    }
}
