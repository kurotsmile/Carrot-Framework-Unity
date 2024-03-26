using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Carrot
{
    interface Carrot_Gamepad_Event
    {
        void gamepad_keydown_down();
        void gamepad_keydown_right();
        void gamepad_keydown_left();
        void gamepad_keydown_up();
        void gamepad_keydown_select();
        void gamepad_keydown_start();
        void gamepad_keydown_y();
        void gamepad_keydown_x();
        void gamepad_keydown_a();
        void gamepad_keydown_b();
    }

    public class Carrot_Gamepad : MonoBehaviour
    {
        private Carrot carrot;
        private string id_gamepad;
        public Carrot_UI UI;
        public GameObject panel_gamepad;
        public GameObject panel_setup_button_gamepad;
        public GameObject obj_btn_edit_all;
        public GameObject obj_btn_edit_all_done;
        public GameObject obj_btn_set_default;
        public Text txt_gamepad_name;
        public Text txt_gamepad_name_func_game;
        public Text txt_gamepad_name_func_menu;
        public string[] s_tip_gamepad_func_game;
        public string[] s_tip_gamepad_func_menu;
        public Button[] btn_gamepad;
        public Image[] img_gamepad_mapping;
        public Sprite[] sp_icon_func_gamepad;
        public Image img_icon_func_gamepad;
        public Text Text_set_gamepad_name;
        public Text Text_id_gamepad;
        public Color32 color_no_set_gamepad;
        public UnityAction[] on_button_gamepad = new UnityAction[10];
        public UnityAction on_joystick_right;
        public UnityAction on_joystick_left;
        public UnityAction on_joystick_up;
        public UnityAction on_joystick_down;
        private KeyCode[] key_code_gamepad;
        private bool is_use_gamepad = true;
        private bool is_setup_gamepad = false;
        private bool is_mapping = false;
        private bool is_joystick = false;
        private bool is_joystick_press = true;
        private bool is_edit_all_gamepad = false;
        private int index_setup_gamepad = 0;
        private int index_button_setup_gamepad = 0;
        private KeyCode[] KeyCode_default = new KeyCode[10];

        public void set_carrot(Carrot carrot)
        {
            this.carrot = carrot;
            KeyCode_default[0] = KeyCode.UpArrow;
            KeyCode_default[1] = KeyCode.RightArrow;
            KeyCode_default[2] = KeyCode.LeftArrow;
            KeyCode_default[3] = KeyCode.DownArrow;
            KeyCode_default[4] = KeyCode.Joystick1Button0;
            KeyCode_default[5] = KeyCode.Return;
            KeyCode_default[6] = KeyCode.W;
            KeyCode_default[7] = KeyCode.D;
            KeyCode_default[8] = KeyCode.S;
            KeyCode_default[9] = KeyCode.A;
        }

        public void set_KeyCode_default(KeyCode[] KeyCode_default)
        {
            this.KeyCode_default = KeyCode_default;
            this.load_gamepad_data();
        }

        public void set_id_gamepad(string s_id)
        {
            this.id_gamepad = s_id;
            this.Text_id_gamepad.text = s_id;
        }

        public void Update()
        {
            if (this.is_use_gamepad)
            {
                if (this.is_joystick && this.is_joystick_press)
                {
                    if (Input.GetAxisRaw("Horizontal") >= 1) if (this.on_joystick_right != null) { this.is_joystick_press = false; this.delay_function(this.on_joystick_right);return; }
                    if (Input.GetAxisRaw("Horizontal") <= -1) if (this.on_joystick_left != null) { this.is_joystick_press = false; this.delay_function(this.on_joystick_left); return; }
                    if (Input.GetAxisRaw("Vertical") >= 1) if (this.on_joystick_up != null) { this.is_joystick_press = false; this.delay_function(this.on_joystick_up); return; }
                    if (Input.GetAxisRaw("Vertical") <= -1) if (this.on_joystick_down != null) { this.is_joystick_press = false; this.delay_function(this.on_joystick_down); return; }
                }

                if (this.is_setup_gamepad)
                {
                    if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
                        {
                            if (Input.GetKeyDown(kcode))
                            {
                                this.check_duplicate_buttons(kcode);
                                this.Text_set_gamepad_name.text = kcode.ToString();
                                this.Text_set_gamepad_name.color = Color.red;
                                this.btn_gamepad[this.index_setup_gamepad].GetComponentInChildren<Text>().text = kcode.ToString();
                                this.key_code_gamepad[this.index_setup_gamepad] = kcode;
                                PlayerPrefs.SetString(this.id_gamepad + "_gamepad_" + this.index_setup_gamepad, kcode.ToString());
                                this.reset_status_gamepad();
                                this.carrot.delay_function(0.3f, this.act_done_edit_gamepad);
                            }
                        }
                    }
                }
                else
                {
                    if (Input.anyKeyDown && this.is_joystick_press)
                    {
                        if (this.is_mapping)
                        {
                            this.reset_img_mapping();
                            for (int i = 0; i < this.img_gamepad_mapping.Length; i++) if (Input.GetKeyDown(key_code_gamepad[i])) this.img_gamepad_mapping[i].color = Color.black;
                        }

                        for (int i = 0; i < this.key_code_gamepad.Length; i++) if (Input.GetKeyDown(key_code_gamepad[i])) if (this.on_button_gamepad[i] != null) this.on_button_gamepad[i]();
                    }
                }
            }
        }

        private void reset_img_mapping()
        {
            for (int i = 0; i < this.img_gamepad_mapping.Length; i++) this.img_gamepad_mapping[i].color = Color.white;
        }

        private void act_done_edit_gamepad()
        {
            this.is_mapping = true;
            this.panel_setup_button_gamepad.SetActive(false);
            if (this.is_edit_all_gamepad)
            {
                this.index_button_setup_gamepad++;
                if (this.index_button_setup_gamepad >= this.key_code_gamepad.Length)
                {
                    this.is_edit_all_gamepad = false;
                    this.is_setup_gamepad = false;
                    this.obj_btn_edit_all.SetActive(true);
                    this.obj_btn_edit_all_done.SetActive(false);
                    this.obj_btn_set_default.SetActive(true);
                }
                else
                    this.setup_button_gamepad(this.index_button_setup_gamepad);
            }
            else
            {
                this.is_setup_gamepad = false;
            }
        }

        private void check_duplicate_buttons(KeyCode key_new)
        {
            for (int i = 0; i < this.key_code_gamepad.Length; i++)
            {
                if (this.key_code_gamepad[i] == key_new)
                {
                    this.key_code_gamepad[i] = KeyCode.None;
                    PlayerPrefs.DeleteKey(this.id_gamepad + "_gamepad_" + i);
                    this.btn_gamepad[i].GetComponentInChildren<Text>().text = this.key_code_gamepad[i].ToString();
                }
            }
        }

        private KeyCode get_key_code_by_name(string s_name_key)
        {
            foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (kcode.ToString() == s_name_key) return kcode;
            }
            return KeyCode.None;
        }

        public void show_setting_gamepad()
        {
            this.is_use_gamepad = true;
            this.transform.SetAsLastSibling();
            this.panel_gamepad.SetActive(true);
            this.panel_setup_button_gamepad.SetActive(false);
            this.reset_status_gamepad();
            for (int i = 0; i < this.key_code_gamepad.Length; i++) this.btn_gamepad[i].GetComponentInChildren<Text>().text = this.key_code_gamepad[i].ToString();
            this.is_setup_gamepad = false;
            this.obj_btn_edit_all.SetActive(true);
            this.obj_btn_edit_all_done.SetActive(false);
            this.obj_btn_set_default.SetActive(true);

            string[] names_gamepad = Input.GetJoystickNames();
            if (names_gamepad.Length > 0)
            {
                foreach (string s_name_gamepad in names_gamepad) this.txt_gamepad_name.text = s_name_gamepad;
                this.txt_gamepad_name.gameObject.SetActive(true);
            }
            else
            {
                this.txt_gamepad_name.gameObject.SetActive(false);
            }
            this.gameObject.name = "Window_Gamepad";
            this.gameObject.transform.localPosition = Vector3.zero;
            this.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            
            this.carrot.add_window(this.gameObject);
            this.is_mapping = true;
            this.carrot.game.set_list_button_gamepad_console(this.UI.get_list_btn());
        }

        public void close_setting_gamepad()
        {
            this.is_mapping = false;
            this.panel_gamepad.SetActive(false);
            UI.close();
        }

        public void setup_button_gamepad(int index_btn)
        {
            this.is_mapping = false;
            this.reset_status_gamepad();
            this.index_setup_gamepad = index_btn;
            this.btn_gamepad[index_btn].GetComponent<Image>().color = this.carrot.color_highlight;
            this.txt_gamepad_name_func_game.text = this.s_tip_gamepad_func_game[index_btn];
            this.txt_gamepad_name_func_menu.text = this.s_tip_gamepad_func_menu[index_btn];
            this.img_icon_func_gamepad.sprite = this.sp_icon_func_gamepad[index_btn];
            this.Text_set_gamepad_name.text = key_code_gamepad[index_btn].ToString();
            this.Text_set_gamepad_name.color = Color.blue;
            this.is_setup_gamepad = true;
            this.panel_setup_button_gamepad.SetActive(true);
        }

        private void reset_status_gamepad()
        {
            for (int i = 0; i < this.btn_gamepad.Length; i++) this.btn_gamepad[i].GetComponent<Image>().color = this.color_no_set_gamepad;
            this.reset_img_mapping();
        }

        public void btn_set_default_gamepad()
        {
            this.reset_status_gamepad();
            this.key_code_gamepad = new KeyCode[10];
            for (int i = 0; i < this.KeyCode_default.Length; i++) this.key_code_gamepad[i] = this.KeyCode_default[i];

            for (int i = 0; i < this.key_code_gamepad.Length; i++)
            {
                PlayerPrefs.DeleteKey("gamepad_" + i);
                this.btn_gamepad[i].GetComponentInChildren<Text>().text = this.key_code_gamepad[i].ToString();
            }
            this.panel_setup_button_gamepad.SetActive(false);
        }

        public void load_gamepad_data()
        {
            this.key_code_gamepad = new KeyCode[10];
            if (PlayerPrefs.GetString(this.id_gamepad + "_gamepad_0", "") != "") this.key_code_gamepad[0] = this.get_key_code_by_name(PlayerPrefs.GetString(this.id_gamepad + "_gamepad_0")); else this.key_code_gamepad[0] = this.KeyCode_default[0];
            if (PlayerPrefs.GetString(this.id_gamepad + "_gamepad_1", "") != "") this.key_code_gamepad[1] = this.get_key_code_by_name(PlayerPrefs.GetString(this.id_gamepad + "_gamepad_1")); else this.key_code_gamepad[1] = this.KeyCode_default[1];
            if (PlayerPrefs.GetString(this.id_gamepad + "_gamepad_2", "") != "") this.key_code_gamepad[2] = this.get_key_code_by_name(PlayerPrefs.GetString(this.id_gamepad + "_gamepad_2")); else this.key_code_gamepad[2] = this.KeyCode_default[2];
            if (PlayerPrefs.GetString(this.id_gamepad + "_gamepad_3", "") != "") this.key_code_gamepad[3] = this.get_key_code_by_name(PlayerPrefs.GetString(this.id_gamepad + "_gamepad_3")); else this.key_code_gamepad[3] = this.KeyCode_default[3];
            if (PlayerPrefs.GetString(this.id_gamepad + "_gamepad_4", "") != "") this.key_code_gamepad[4] = this.get_key_code_by_name(PlayerPrefs.GetString(this.id_gamepad + "_gamepad_4")); else this.key_code_gamepad[4] = this.KeyCode_default[4];
            if (PlayerPrefs.GetString(this.id_gamepad + "_gamepad_5", "") != "") this.key_code_gamepad[5] = this.get_key_code_by_name(PlayerPrefs.GetString(this.id_gamepad + "_gamepad_5")); else this.key_code_gamepad[5] = this.KeyCode_default[5];
            if (PlayerPrefs.GetString(this.id_gamepad + "_gamepad_6", "") != "") this.key_code_gamepad[6] = this.get_key_code_by_name(PlayerPrefs.GetString(this.id_gamepad + "_gamepad_6")); else this.key_code_gamepad[6] = this.KeyCode_default[6];
            if (PlayerPrefs.GetString(this.id_gamepad + "_gamepad_7", "") != "") this.key_code_gamepad[7] = this.get_key_code_by_name(PlayerPrefs.GetString(this.id_gamepad + "_gamepad_7")); else this.key_code_gamepad[7] = this.KeyCode_default[7];
            if (PlayerPrefs.GetString(this.id_gamepad + "_gamepad_8", "") != "") this.key_code_gamepad[8] = this.get_key_code_by_name(PlayerPrefs.GetString(this.id_gamepad + "_gamepad_8")); else this.key_code_gamepad[8] = this.KeyCode_default[8];
            if (PlayerPrefs.GetString(this.id_gamepad + "_gamepad_9", "") != "") this.key_code_gamepad[9] = this.get_key_code_by_name(PlayerPrefs.GetString(this.id_gamepad + "_gamepad_9")); else this.key_code_gamepad[9] = this.KeyCode_default[9];
        }

        public void btn_edit_all_gamepad()
        {
            this.is_edit_all_gamepad = true;
            this.obj_btn_edit_all.SetActive(false);
            this.obj_btn_edit_all_done.SetActive(true);
            this.obj_btn_set_default.SetActive(false);
            this.index_button_setup_gamepad = 0;
            this.setup_button_gamepad(this.index_button_setup_gamepad);
        }

        public void set_gamepad_keydown_up(UnityAction act)
        {
            this.on_button_gamepad[0] = act;
        }

        public void set_gamepad_keydown_right(UnityAction act)
        {
            this.on_button_gamepad[1] = act;
        }

        public void set_gamepad_keydown_left(UnityAction act)
        {
            this.on_button_gamepad[2] = act;
        }

        public void set_gamepad_keydown_down(UnityAction act)
        {
            this.on_button_gamepad[3] = act;
        }

        public void set_gamepad_keydown_select(UnityAction act)
        {
            this.on_button_gamepad[4] = act;
        }

        public void set_gamepad_keydown_start(UnityAction act)
        {
            this.on_button_gamepad[5] = act;
        }

        public void set_gamepad_keydown_y(UnityAction act)
        {
            this.on_button_gamepad[6] = act;
        }

        public void set_gamepad_keydown_x(UnityAction act)
        {
            this.on_button_gamepad[7] = act;
        }

        public void set_gamepad_keydown_a(UnityAction act)
        {
            this.on_button_gamepad[8] = act;
        }

        public void set_gamepad_keydown_b(UnityAction act)
        {
            this.on_button_gamepad[9] = act;
        }

        public void set_gamepad_Joystick_right(UnityAction act)
        {
            this.on_joystick_right = act;
        }

        public void set_gamepad_Joystick_left(UnityAction act)
        {
            this.on_joystick_left = act;
        }

        public void set_gamepad_Joystick_down(UnityAction act)
        {
            this.on_joystick_down = act;
        }

        public void set_gamepad_Joystick_up(UnityAction act)
        {
            this.on_joystick_up = act;
        }

        public void set_list_tip_gamepad(string[] s_list_tip)
        {
            this.s_tip_gamepad_func_game = s_list_tip;
        }

        public void set_joystick_enable_play(bool is_play)
        {
            this.is_joystick = is_play;
            this.is_joystick_press = is_play;
        }

        public string get_id_gamepad()
        {
            return this.id_gamepad;
        }

        public void delay_function(UnityAction act_func, float timer = 0.5f)
        {
            act_func();
            StartCoroutine(act_try_connect(timer));
        }

        private IEnumerator act_try_connect(float timer)
        {
            yield return new WaitForSeconds(timer);
            this.is_joystick_press = true;
        }

        public void change_status_use_gamepad(Carrot_Box_Btn_Item item_btn_extension)
        {
            
            if (this.is_use_gamepad)
            {
                this.is_use_gamepad = false;
                this.carrot.Show_msg("Gamepad", "Disabled and do not use this handle!", Msg_Icon.Alert);
                item_btn_extension.set_icon(this.carrot.icon_carrot_gamepad_on);
            }
            else
            {
                this.is_use_gamepad = true;
                this.carrot.Show_msg("Gamepad", "Enabled and used this gamepad!", Msg_Icon.Alert);
                item_btn_extension.set_icon(this.carrot.icon_carrot_gamepad_off);
            }
        }

        public bool get_status_use_gampead()
        {
            return this.is_use_gamepad;
        }
    }
}
