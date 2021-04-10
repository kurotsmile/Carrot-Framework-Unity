using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Carrot_item_user_edit : MonoBehaviour
{
    public string s_name;
    public string s_type;
    public Text txt_title;
    public InputField inp_val;
    public Dropdown dropdown_val;
    public GameObject panel_avatar;
    public void set_data(string s_type_field,string s_data_val,string s_val)
    {
        this.s_type = s_type_field;
        this.inp_val.gameObject.SetActive(false);
        this.dropdown_val.gameObject.SetActive(false);
        this.panel_avatar.SetActive(false);
        if (this.s_type == "2")
        {
            this.dropdown_val.gameObject.SetActive(true);
            IList list_data_val = (IList)Carrot.Json.Deserialize(s_data_val);
            dropdown_val.ClearOptions();
            int sel_index_val = 0;
            for (int i = 0; i < list_data_val.Count; i++)
            {
                dropdown_val.options.Add(new Dropdown.OptionData() { text = list_data_val[i].ToString() });
                if (s_val == list_data_val[i].ToString()) sel_index_val = i;
            }

            this.dropdown_val.value = sel_index_val;
            this.dropdown_val.RefreshShownValue();
        }
        else if (this.s_type == "4")
        {
            this.inp_val.gameObject.SetActive(true);
            this.inp_val.contentType = InputField.ContentType.DecimalNumber;
            this.inp_val.text = s_val;
        }
        else if (this.s_type == "5")
        {
            this.inp_val.gameObject.SetActive(true);
            this.inp_val.contentType = InputField.ContentType.EmailAddress;
            this.inp_val.text = s_val;
        }
        else if (this.s_type == "3")
        {
            this.inp_val.gameObject.SetActive(true);
            this.inp_val.contentType = InputField.ContentType.Password;
            this.inp_val.text = s_val;
        }
        else if (this.s_type == "6")
        {
            this.panel_avatar.SetActive(true);
        }
        else
        {
            this.inp_val.gameObject.SetActive(true);
            this.inp_val.contentType = InputField.ContentType.Standard;
            this.inp_val.text = s_val;
        }

    }

    public string  get_val()
    {
        if (this.s_type == "2")
        {
            return this.dropdown_val.value.ToString();
        }
        else
        {
            return this.inp_val.text;
        }
    }
}
