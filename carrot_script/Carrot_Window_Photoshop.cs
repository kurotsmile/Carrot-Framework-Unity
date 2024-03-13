using System;
using UnityEngine;
using UnityEngine.UI;

namespace Carrot
{
    public class Carrot_Window_Photoshop : MonoBehaviour
    {
        public Carrot_UI UI;
        [Header("Edit Photo")]
        public GameObject panel_edit_photo_tool_menu;
        public GameObject panel_edit_photo_tool_val;
        public RawImage rawImage_Photo;
        public Slider slider_edit_photo;
        private Texture2D tex_original_photo;
        private Texture2D tex_original_view;
        public Sprite[] icon_func_photo_tool;
        public Image img_icon_tool_photo;
        private int index_photo_edit;
        private int sel_tool_edit_photo = 0;

        public void show(Texture2D photo_Texture,float scale)
        {
            if (scale != 0f)
            {
                RectTransform component = this.rawImage_Photo.rectTransform;

                float aspectRatio = (float)photo_Texture.width / (float)photo_Texture.height;

                if (aspectRatio >= 1f)
                {
                    float newHeight = component.sizeDelta.x / aspectRatio;
                    component.sizeDelta = new Vector2(component.sizeDelta.x, newHeight);
                }
                else
                {
                    float newWidth = component.sizeDelta.y * aspectRatio;
                    component.sizeDelta = new Vector2(newWidth, component.sizeDelta.y);
                }

                float y = component.sizeDelta.x * (float)photo_Texture.height / (float)photo_Texture.width;
                float x = component.sizeDelta.y * (float)photo_Texture.width / (float)photo_Texture.height;
                component.sizeDelta = new Vector2(y / scale, x / scale);
            }
            this.rawImage_Photo.texture = photo_Texture;
            this.panel_edit_photo_tool_val.SetActive(false);
            this.panel_edit_photo_tool_menu.SetActive(true);
            this.tex_original_photo = photo_Texture;
        }

        public void btn_sel_tool_photo(int index_tool)
        {
            this.sel_tool_edit_photo = index_tool;
            this.tex_original_view = this.tex_original_photo;
            this.panel_edit_photo_tool_val.SetActive(true);
            this.panel_edit_photo_tool_menu.SetActive(false);
            this.img_icon_tool_photo.sprite = this.icon_func_photo_tool[index_tool];
            if (index_tool == 0) { this.slider_edit_photo.minValue = 30; this.slider_edit_photo.maxValue = 60; this.slider_edit_photo.wholeNumbers = true; this.slider_edit_photo.value = 43; }
            if (index_tool <= 1) { this.slider_edit_photo.minValue = 0f; this.slider_edit_photo.maxValue = 1f; this.slider_edit_photo.wholeNumbers = false; this.slider_edit_photo.value = 0.5f; }
        }

        public void btn_save_tool_photo()
        {
            this.tex_original_photo = (Texture2D)this.rawImage_Photo.texture;
            this.panel_edit_photo_tool_val.SetActive(false);
            this.panel_edit_photo_tool_menu.SetActive(true);
            GameObject.Find("Carrot").GetComponent<Carrot>().get_tool().save_file("img_photo_" + this.index_photo_edit, this.tex_original_photo.EncodeToPNG());
        }

        public void btn_cancel_tool_photo()
        {
            this.tex_original_photo = this.tex_original_view;
            this.rawImage_Photo.texture = this.tex_original_view;
            this.panel_edit_photo_tool_val.SetActive(false);
            this.panel_edit_photo_tool_menu.SetActive(true);
        }

        public void close()
        {
            this.UI.close();
        }

        public void change_val_tool_edit()
        {
            if (this.sel_tool_edit_photo == 0) this.rawImage_Photo.texture = this.AdjustBrightness(this.tex_original_photo, slider_edit_photo.value.ToString());
            if (this.sel_tool_edit_photo == 1) this.rawImage_Photo.texture = this.Saturation(this.tex_original_photo, slider_edit_photo.value);
            if (this.sel_tool_edit_photo == 2) this.rawImage_Photo.texture = this.color_text(this.tex_original_photo, slider_edit_photo.value, Color.red);
            if (this.sel_tool_edit_photo == 3) this.rawImage_Photo.texture = this.color_text(this.tex_original_photo, slider_edit_photo.value, Color.green);
            if (this.sel_tool_edit_photo == 4) this.rawImage_Photo.texture = this.color_text(this.tex_original_photo, slider_edit_photo.value, Color.blue);
        }

        private Texture2D AdjustBrightness(Texture2D textPhoto,string brightness)
        {
            int brightnessInt = Convert.ToInt32(brightness);
            float mappedBrightness = (51 * brightnessInt) / 10 - 255;
            Texture2D bitmapImage = new Texture2D(textPhoto.width, textPhoto.height);
            if (mappedBrightness < -255) mappedBrightness = -255;
            if (mappedBrightness > 255) mappedBrightness = 255;
            Color color;
            for (int i = 0; i < bitmapImage.width; i++)
            {
                for (int j = 0; j < bitmapImage.height; j++)
                {
                    color = textPhoto.GetPixel(i, j);
                    float cR;
                    float cG;
                    float cB;
                    float cA;
                    if (color.a == 1f)
                    {
                        cR = color.r + (mappedBrightness / 255);
                        cG = color.g + (mappedBrightness / 255);
                        cB = color.b + (mappedBrightness / 255);
                        cA = color.a;
                        if (cR < 0) cR = 0;
                        if (cR > 255) cR = 255;
                        if (cG < 0) cG = 0;
                        if (cG > 255) cG = 255;
                        if (cB < 0) cB = 0;
                        if (cB > 255) cB = 255;
                    }
                    else
                    {
                        cR = color.r;
                        cG = color.g;
                        cB = color.b;
                        cA = color.a;
                    }
                    bitmapImage.SetPixel(i, j, new Color(cR, cG, cB, cA));
                }
            }
            bitmapImage.Apply();
            return bitmapImage;
        }

        private Texture2D Saturation(Texture2D inTex, float angle)
        {
            Texture2D outTex = new Texture2D(inTex.width, inTex.height);
            Color[] pix = inTex.GetPixels(0, 0, inTex.width, inTex.height);
            for (int i = 0; i < pix.Length; i++)
            {
                HSBColor hsb = HSBColor.FromColor(pix[i]);
                hsb.s = hsb.s + angle;
                pix[i] = hsb.ToColor();
            }
            outTex.SetPixels(0, 0, inTex.width, inTex.height, pix);
            outTex.Apply();
            return outTex;
        }

        private Texture2D color_text(Texture2D inTex, float angle, Color color_to)
        {
            Texture2D outTex = new Texture2D(inTex.width, inTex.height);
            Color[] pix = inTex.GetPixels(0, 0, inTex.width, inTex.height);
            for (int i = 0; i < pix.Length; i++)
            {
                if (i % 3 == 0)
                {
                    HSBColor hsb = HSBColor.Lerp(HSBColor.FromColor(pix[i]), new HSBColor(color_to), angle);
                    pix[i] = hsb.ToColor();
                }
            }
            outTex.SetPixels(0, 0, inTex.width, inTex.height, pix);
            outTex.Apply();
            return outTex;
        }
    }
}
