using System.Collections;
using UnityEngine;
using UnityEngine.Events;
namespace Carrot
{
	public class Carrot_DeviceOrientationChange : MonoBehaviour
	{
		public UnityEvent OnResolutionChange = new();
		public UnityEvent OnOrientationChange = new();
		public static float CheckDelay = 1f;

		public Vector2 resolution;            
		public DeviceOrientation orientation;     
		private bool isAlive = false;
		public GameObject[] emp_portrait;
		public GameObject[] emp_Landscape;
		public bool is_load_on_start=true;

		private bool is_portrait=true;

		void Start()
		{
			if (this.is_load_on_start)
			{
				this.Start_check();
			}
        }

		public void Start_check()
		{
			this.isAlive = true;
            this.Check_show_emp_by_resolution();
            StartCoroutine(CheckForChange());
        }

		public void Stop_check()
		{
			this.isAlive = false;
			this.StopAllCoroutines();
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
                    OnResolutionChange?.Invoke();
                    this.Check_show_emp_by_resolution();
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
                            OnOrientationChange?.Invoke();
                            if (orientation == DeviceOrientation.Portrait) this.Act_portrait();
							if (orientation == DeviceOrientation.LandscapeLeft) this.Act_Landscape();
							if (orientation == DeviceOrientation.LandscapeRight) this.Act_Landscape();
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
		public void Act_portrait()
        {
			for (int i = 0; i < this.emp_portrait.Length; i++) this.emp_portrait[i].SetActive(true);
			for (int i = 0; i < this.emp_Landscape.Length; i++) this.emp_Landscape[i].SetActive(false);
			this.is_portrait = true;
		}

		[ContextMenu("Act Landspace")]
		public void Act_Landscape()
        {
			for (int i = 0; i < this.emp_portrait.Length; i++) this.emp_portrait[i].SetActive(false);
			for (int i = 0; i < this.emp_Landscape.Length; i++) this.emp_Landscape[i].SetActive(true);
			this.is_portrait = false;
		}

		public void Check_show_emp_by_resolution()
        {
			if (Screen.width > Screen.height) this.Act_Landscape();
			else this.Act_portrait();
		}

		public bool Get_status_portrait()
        {
			return this.is_portrait;
        }
    }
}