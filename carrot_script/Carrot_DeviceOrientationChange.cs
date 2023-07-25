using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
namespace Carrot
{
	public class Carrot_DeviceOrientationChange : MonoBehaviour
	{
		public UnityEvent OnResolutionChange = new UnityEvent();
		public UnityEvent OnOrientationChange = new UnityEvent();
		public static float CheckDelay = 1f;

		public static Vector2 resolution;            
		public static DeviceOrientation orientation;     
		static bool isAlive = true;
		public GameObject[] emp_portrait;
		public GameObject[] emp_Landscape;
		public bool is_load_on_start=true;

		private bool is_portrait=true;

		void Start()
		{
			if(this.is_load_on_start) this.check_show_emp_by_resolution();
			StartCoroutine(CheckForChange());
		}

		IEnumerator CheckForChange()
		{
			resolution = new Vector2(Screen.width, Screen.height);
			orientation = Input.deviceOrientation;
	
			while (isAlive)
			{
				if (resolution.x != Screen.width || resolution.y != Screen.height)
				{
					resolution = new Vector2(Screen.width, Screen.height);
					if(OnResolutionChange!=null)OnResolutionChange.Invoke();
					this.check_show_emp_by_resolution();
				}

				switch (Input.deviceOrientation)
				{
					case DeviceOrientation.Unknown:
					case DeviceOrientation.FaceUp:
					case DeviceOrientation.FaceDown:
						break;
					default:
						if (orientation != Input.deviceOrientation)
						{
							orientation = Input.deviceOrientation;
							if(OnOrientationChange!=null)OnOrientationChange.Invoke();
							if (orientation == DeviceOrientation.Portrait) this.act_portrait();
							if (orientation == DeviceOrientation.LandscapeLeft) this.act_Landscape();
							if (orientation == DeviceOrientation.LandscapeRight) this.act_Landscape();
						}
						break;
				}

				yield return new WaitForSeconds(CheckDelay);
			}
		}

		void OnDestroy()
		{
			isAlive = false;
		}

		[ContextMenu("Act Portrait")]
		public void act_portrait()
        {
			for (int i = 0; i < this.emp_portrait.Length; i++) this.emp_portrait[i].SetActive(true);
			for (int i = 0; i < this.emp_Landscape.Length; i++) this.emp_Landscape[i].SetActive(false);
			this.is_portrait = true;
		}

		[ContextMenu("Act Landspace")]
		public void act_Landscape()
        {
			for (int i = 0; i < this.emp_portrait.Length; i++) this.emp_portrait[i].SetActive(false);
			for (int i = 0; i < this.emp_Landscape.Length; i++) this.emp_Landscape[i].SetActive(true);
			this.is_portrait = false;
		}

		public void check_show_emp_by_resolution()
        {
			if (Screen.width > Screen.height) this.act_Landscape();
			else this.act_portrait();
		}

		public bool get_status_portrait()
        {
			return this.is_portrait;
        }
	}

}