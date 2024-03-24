using UnityEngine;

namespace Carrot
{
    public class Carrot_Box_Btn_Panel : MonoBehaviour
    {
        public GameObject btn_prefab;

        public Carrot_Button_Item create_btn(string s_name="btn_item")
        {
            GameObject bnt_footer = Instantiate(btn_prefab);
            bnt_footer.name = s_name;
            bnt_footer.transform.SetParent(this.transform);
            bnt_footer.transform.localPosition = new Vector3(bnt_footer.transform.position.x, bnt_footer.transform.position.y, 0f);
            bnt_footer.transform.localScale = new Vector3(1f, 1f, 1f);
            bnt_footer.transform.localRotation = Quaternion.Euler(Vector3.zero);

            Carrot_Button_Item btn_item = bnt_footer.GetComponent<Carrot_Button_Item>();
            return btn_item;
        }

    }
}

