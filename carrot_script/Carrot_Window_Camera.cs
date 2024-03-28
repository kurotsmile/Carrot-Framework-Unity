using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Carrot
{
    public class Carrot_Window_Camera : MonoBehaviour
    {
        private Carrot carrot;
        private WebCamTexture tex = null;
        public Carrot_UI UI;
        public RawImage _rawImage_photo;
        private bool isFrontFacing = false;
        private bool isCameraRotation = false;
        private bool isCameraMirror = false;

        public AudioSource sound_camera;
        public GameObject btn_photo_libary;
        private int leng_img_photo = 0;
        private UnityAction<Texture2D> act_after_take_photo;

        public void load(Carrot carrot)
        {
            this.carrot = carrot;
            this.check_show_btn_libary();
            this.load_device_camera();
        }

        private void load_device_camera()
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            string name_device_cam = "";
            for (int i = 0; i < devices.Length; i++) if (devices[i].isFrontFacing == this.isFrontFacing) name_device_cam = devices[i].name;
            tex = new WebCamTexture(name_device_cam);
            this._rawImage_photo.rectTransform.sizeDelta = new Vector2(tex.width, tex.height);
            this._rawImage_photo.texture = tex;
            tex.Play();
        }

        private void check_show_btn_libary()
        {
            this.leng_img_photo = PlayerPrefs.GetInt("leng_img_photo", 0);
            if (this.leng_img_photo > 0)
                this.btn_photo_libary.SetActive(true);
            else
                this.btn_photo_libary.SetActive(false);
        }

        public void btn_take_photo_camera()
        {
            this.sound_camera.Play();
            StartCoroutine(TakePhoto());
        }

        IEnumerator TakePhoto()
        {
            while (this.sound_camera.isPlaying)  yield return new WaitForEndOfFrame();

            Texture2D photo = new Texture2D(tex.width, tex.height);
            photo.SetPixels(tex.GetPixels());
            if (this.isCameraRotation) photo.SetPixels(ImageRotator.RotateImage(photo, 90).GetPixels());
            if (this.isCameraMirror) photo.SetPixels(this.FlipTexture(photo).GetPixels());
            photo.Apply();
            this.carrot.camera_pro.Add_photo(photo);
            this.check_show_btn_libary();
            if (this.act_after_take_photo != null) this.act_after_take_photo(photo);
            this.close();
        }

        public void btn_show_list_photo()
        {
            this.carrot.camera_pro.Show_list_img(this.act_done_select_photo_in_list);
        }

        private void act_done_select_photo_in_list(Texture2D photo)
        {
            this.act_after_take_photo(photo);
            this.act_after_take_photo = null;
            this.close();
        }

        public void btn_rotate_camera_photo()
        {
            this.close();
            if (this.isFrontFacing) this.isFrontFacing = false; else this.isFrontFacing = true;
            this.load_device_camera();
        }

        public void btn_rotation_camera_photo()
        {
            if (this.isCameraRotation)
            {
                this._rawImage_photo.rectTransform.rotation = Quaternion.Euler(Vector3.zero);
                this.isCameraRotation = false;
            }
            else
            {
                this._rawImage_photo.rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 90f));
                this.isCameraRotation = true;
            }
        }

        public void btn_mirror_camera_photo()
        {
            if (this.isCameraMirror)
            {
                this._rawImage_photo.rectTransform.localScale = new Vector3(1f, 1f, 1f);
                this.isCameraMirror = false;
            }
            else
            {
                this._rawImage_photo.rectTransform.localScale = new Vector3(1f, -1f, 1f);
                this.isCameraMirror = true;
            }
        }

        private void Update()
        {
            if(tex!=null) this._rawImage_photo.rectTransform.sizeDelta = new Vector2(tex.width, tex.height);
        }

        public void close()
        {
            if (tex != null) { tex.Stop(); tex = null; }
            this.UI.close();
        }

        public void set_act_after_take_photo(UnityAction<Texture2D> act)
        {
            this.act_after_take_photo = act;
        }

        Texture2D FlipTexture(Texture2D original)
        {
            int width = original.width;
            int height = original.height;
            Texture2D snap = new Texture2D(width, height);
            Color[] pixels = original.GetPixels();
            Color[] pixelsFlipped = new Color[pixels.Length];
            for (int i = 0; i < height; i++) Array.Copy(pixels, i * width, pixelsFlipped, (height - i - 1) * width, width);
            snap.SetPixels(pixelsFlipped);
            snap.Apply();
            return snap;
        }
    }

    public class ImageRotator
    {
        public static Texture2D RotateImage(Texture2D originTexture, int angle)
        {
            Texture2D result;
            result = new Texture2D(originTexture.width, originTexture.height);
            Color32[] pix1 = result.GetPixels32();
            Color32[] pix2 = originTexture.GetPixels32();
            int W = originTexture.width;
            int H = originTexture.height;
            int x = 0;
            int y = 0;
            Color32[] pix3 = rotateSquare(pix2, (Math.PI / 180 * (double)angle), originTexture);
            for (int j = 0; j < H; j++)
            {
                for (var i = 0; i < W; i++)
                {
                    //pix1[result.width/2 - originTexture.width/2 + x + i + result.width*(result.height/2-originTexture.height/2+j+y)] = pix2[i + j*originTexture.width];
                    pix1[result.width / 2 - W / 2 + x + i + result.width * (result.height / 2 - H / 2 + j + y)] = pix3[i + j * W];
                }
            }
            result.SetPixels32(pix1);
            result.Apply();
            return result;
        }
        static Color32[] rotateSquare(Color32[] arr, double phi, Texture2D originTexture)
        {
            int x;
            int y;
            int i;
            int j;
            double sn = Math.Sin(phi);
            double cs = Math.Cos(phi);
            Color32[] arr2 = originTexture.GetPixels32();
            int W = originTexture.width;
            int H = originTexture.height;
            int xc = W / 2;
            int yc = H / 2;
            for (j = 0; j < H; j++)
            {
                for (i = 0; i < W; i++)
                {
                    arr2[j * W + i] = new Color32(0, 0, 0, 0);
                    x = (int)(cs * (i - xc) + sn * (j - yc) + xc);
                    y = (int)(-sn * (i - xc) + cs * (j - yc) + yc);
                    if ((x > -1) && (x < W) && (y > -1) && (y < H))
                    {
                        arr2[j * W + i] = arr[y * W + x];
                    }
                }
            }
            return arr2;
        }
    }
}
