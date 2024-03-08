using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
namespace Carrot
{
    public class Carrot_location : MonoBehaviour
    {
        [Header("Obj main")]
        public Carrot carrot;
        [Header("Obj Location")]
        public float latA, lonA;
        private int count_check = 0;

        private Carrot_Window_Loading box_loading;

        public void get_location(UnityAction<LocationInfo> act_done, UnityAction<string> act_fail = null)
        {
            this.count_check++;
            if (box_loading != null) this.box_loading.close();
            this.box_loading = this.carrot.show_loading(get_location_data(act_done, act_fail));
        }

        IEnumerator get_location_data(UnityAction<LocationInfo> act_done, UnityAction<string> act_fail)
        {
            if (!Input.location.isEnabledByUser)
            {
                Permission.RequestUserPermission(Permission.FineLocation);
                if (this.count_check < 2)
                {
                    this.carrot.delay_function(5f, () => this.get_location(act_done, act_fail));
                }
                else
                {
                    this.StopAllCoroutines();
                    this.carrot.stop_all_act();
                }
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
                act_fail?.Invoke("Timed out");
                yield break;
            }

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                if (box_loading != null) this.box_loading.close();
                act_fail?.Invoke("Unable to determine device location");
                yield break;
            }
            else
            {
                lonA = Input.location.lastData.longitude;
                latA = Input.location.lastData.latitude;
                if (box_loading != null) this.box_loading.close();
                act_done?.Invoke(Input.location.lastData);
            }
        }

        public void show_map_locations()
        {
            Application.OpenURL("https://maps.google.com?q=" + this.latA.ToString().Replace(",", ".") + "," + this.lonA.ToString().Replace(",", ".") + "");
        }

        public string get_address()
        {
            string s_url = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + this.lonA + "," + this.latA + "&key=" + this.carrot.key_api_google_location_map;
            return s_url;
        }
    }
}