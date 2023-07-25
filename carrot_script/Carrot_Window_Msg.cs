using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Carrot
{
    public enum Msg_Icon { Alert, Error, Success, Question };

    public class Carrot_Window_Msg : MonoBehaviour
    {
        public Carrot_UI UI;
        public Carrot carrot;

        [Header("Msg Obj")]
        public Sprite icon_msg_alert;
        public Sprite icon_msg_error;
        public Sprite icon_msg_success;
        public Sprite icon_msg_question;
        public Image icon_msg_img;
        public Text msg_title;
        public Text msg_txt;
        
        public GameObject panel_msg_btn;
        public GameObject item_btn_msg_prefab;

        public void load(Carrot carrot)
        {
            this.carrot = carrot;
            this.UI.set_theme(this.carrot.color_highlight);
        }

        public void close()
        {
            UI.close();
        }

        public GameObject add_btn_msg(string txt, UnityAction act)
        {
            this.panel_msg_btn.SetActive(true);
            GameObject btn_item_msg = Instantiate(this.item_btn_msg_prefab);
            btn_item_msg.transform.SetParent(this.panel_msg_btn.transform);
            btn_item_msg.transform.localPosition = new Vector3(btn_item_msg.transform.localPosition.x, btn_item_msg.transform.localPosition.y, 0f);
            btn_item_msg.transform.localScale = new Vector3(1f, 1f, 1f);
            btn_item_msg.transform.localRotation = Quaternion.identity;
            btn_item_msg.GetComponentInChildren<Text>().text = txt;
            btn_item_msg.GetComponent<Button>().onClick.RemoveAllListeners();
            btn_item_msg.GetComponent<Button>().onClick.AddListener(act);
            return btn_item_msg;
        }

        public void set_title(string s)
        {
            this.msg_title.text = s;
            this.msg_title.gameObject.SetActive(true);
        }

        public void set_msg(string s)
        {
            this.msg_txt.text = s;
        }

        public void set_icon(Msg_Icon icon)
        {
            if (icon == Msg_Icon.Error) this.icon_msg_img.sprite = this.icon_msg_error;
            if (icon == Msg_Icon.Success) this.icon_msg_img.sprite = this.icon_msg_success;
            if (icon == Msg_Icon.Question) this.icon_msg_img.sprite = this.icon_msg_question;
        }

        public void set_icon_customer(Sprite sp)
        {
            this.icon_msg_img.sprite = sp;
        }

        public void update_btns_gamepad_console()
        {
            if(this.carrot.type_control!=TypeControl.None) this.carrot.game.set_list_button_gamepad_console(UI.get_list_btn());

        }
    }
}
