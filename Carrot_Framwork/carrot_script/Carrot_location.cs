using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

namespace Carrot
{
    public class Carrot_location : MonoBehaviour
    {
        public string key_api;
        public float latA, lonA;

        private Carrot_Window_Loading box_loading;

        public void get_location(UnityAction<LocationInfo> act_done, UnityAction<string> act_fail=null)
        {
            if (box_loading != null) this.box_loading.close();
            this.box_loading = this.GetComponent<Carrot>().show_loading(get_location_data(act_done, act_fail));
        }

        IEnumerator get_location_data(UnityAction<LocationInfo> act_done,UnityAction<string> act_fail)
        {
            if (!Input.location.isEnabledByUser)
            {
                Permission.RequestUserPermission(Permission.FineLocation);
                this.GetComponent<Carrot>().delay_function(5f, ()=>this.get_location(act_done, act_fail));
                yield break;
            }

            Input.location.Start(1, 1);

            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            if (maxWait < 1)
            {
                if (box_loading != null) this.box_loading.close();
                if (act_fail != null) act_fail.Invoke("Timed out");
                yield break;
            }

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                if (box_loading != null) this.box_loading.close();
                if (act_fail != null) act_fail.Invoke("Unable to determine device location");
                yield break;
            }
            else
            {
                lonA = Input.location.lastData.longitude;
                latA = Input.location.lastData.latitude;
                if (box_loading != null) this.box_loading.close();
                if (act_done != null) act_done(Input.location.lastData);
            }

            //Input.location.Stop();
        }

        public void show_map_locations()
        {
            Application.OpenURL("https://maps.google.com?q=" + this.latA.ToString().Replace(",", ".") + "," + this.lonA.ToString().Replace(",", ".") + "");
        }

        public string get_address()
        {
            string s_url = "https://maps.googleapis.com/maps/api/geocode/json?latlng="+this.lonA+","+this.latA+ "&key="+this.key_api;
            WWWForm frm_location = this.GetComponent<Carrot>().frm_act("get_location");
            this.GetComponent<Carrot>().send(frm_location, act_get_location_address);
            return s_url;
        }

        private void act_get_location_address(string s_data)
        {
            this.GetComponent<Carrot>().show_msg(s_data);
        }
    }
}
