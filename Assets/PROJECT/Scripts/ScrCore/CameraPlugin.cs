using System;
using System.Linq;
using UnityEngine;

public class CameraPlugin : MonoBehaviour
{
	private WebCamTexture webcamTexture;

	private string _deviceName;

	private Texture2D texture;

	private Texture2D gallery;

	private bool isAvalibleCam;

	private WebCamDevice[] devices;

	private int indexCam;

	private int orient;

	public Action<Texture2D, float> OnUpdateTexture;

	public Action<bool> InitComplete;

	private bool m_force;

	public float Quality { get; set; }

	public WebCamDevice CurrentDevice
	{
		get
		{
			return this.devices[this.indexCam];
		}
	}
    private void Start()
    {
		 
    }
    public void Initilized()
	{
		//if (!UniAndroidPermission.IsPermitted(AndroidPermission.CAMERA))
		//{
		//	UniAndroidPermission.RequestPermission(AndroidPermission.CAMERA, delegate
		//	{
		//		DebugLogger.Log("[CameraPlugin] RequestPremission complete");
		//		this.Init();
		//	}, delegate
		//	{
		//		DebugLogger.LogError("[CameraPlugin] RequestPremission Error");
		//		this.InitComplete.SafeInvoke(false);
		//	});
		//}
		//else
		{
			this.Init();
		}
	}

	private void Init()
	{
		this.isAvalibleCam = true;
		if (this.webcamTexture != null && this.webcamTexture.isPlaying)
		{
			this.webcamTexture.Stop();
		}
		this.devices = WebCamTexture.devices;
		if (this.devices == null || this.devices.Length == 0)
		{
			Debug.Log ("[CameraPlugin] devices == null || devices.Length == 0");
		}
		else
		{
			this._deviceName = this.devices[this.indexCam].name;
			if (this._deviceName != null)
			{
				this.webcamTexture = new WebCamTexture(this._deviceName);
				try
				{
					this.webcamTexture.Play();
				}
				catch (Exception ex)
				{
					Debug.Log (ex.Message);
					Debug.Log ("[CameraPlugin] webcamTexture.Play");
					return;
				}
				Debug.Log("[CameraPlugin] Init Camera");
				this.InitComplete.SafeInvoke(true);
			}
			else
			{
				Debug.Log("[CameraPlugin] _deviceName == null");
			}
		}
	}

	public Texture2D TakeSnapshot()
	{
		if (this.isAvalibleCam)
		{
			int num = Mathf.Min(this.webcamTexture.width, this.webcamTexture.height);
			this.texture = new Texture2D(num, num, TextureFormat.RGB24, false)
			{
				filterMode = FilterMode.Point
			};
			this.texture.SetPixels(this.webcamTexture.GetPixels((this.webcamTexture.width - num) / 2, (this.webcamTexture.height - num) / 2, num, num));
			this.texture.Apply();
			this.isAvalibleCam = false;
			if (this.webcamTexture != null)
			{
				this.webcamTexture.Stop();
				Destroy(this.webcamTexture);
			}
			Debug.Log(this.orient);
			switch (this.orient)
			{
				case 90:
					this.texture = ActionHelper.Rotate(this.texture, false);
					break;
				case 180:
					this.texture = ActionHelper.Rotate(this.texture, false);
					this.texture = ActionHelper.Rotate(this.texture, false);
					break;
				case 270:
					this.texture = ActionHelper.Rotate(this.texture, true);
					break;
				case -90:
					this.texture = ActionHelper.Rotate(this.texture, true);
					break;
				case -180:
					this.texture = ActionHelper.Rotate(this.texture, true);
					this.texture = ActionHelper.Rotate(this.texture, true);
					break;
				case -270:
					this.texture = ActionHelper.Rotate(this.texture, false);
					break;
			}
			return this.texture;
		}
		return null;
	}

	public Texture2D GetTextureByQuality()
	{
		return this.texture;
	}

	public void ForceUpdate()
	{
		this.m_force = true;
		this.Update();
	}

	public void ChangeCamera()
	{
		if (this.webcamTexture != null)
		{
			this.webcamTexture.Stop(); 
			Destroy(this.webcamTexture);
		}
		this.indexCam++;
		if (this.indexCam >= this.devices.Count())
		{
			this.indexCam = 0;
		}
		this.Init();
	}

	private void Update()
	{
		if (this.isAvalibleCam)
		{
			if (this.webcamTexture == null)
			{
				//UnityEngine.Debug.LogError("[CameraPlugin] webcamTexture is null");
			}
			else
			{
				int num = Mathf.Min(this.webcamTexture.width, this.webcamTexture.height);
				this.orient = -this.webcamTexture.videoRotationAngle;
				this.texture = new Texture2D(num, num, TextureFormat.ARGB32, false)
				{
					filterMode = FilterMode.Point
				};
				this.texture.SetPixels(this.webcamTexture.GetPixels((this.webcamTexture.width - num) / 2, (this.webcamTexture.height - num) / 2, num, num));
				this.texture.Apply();
				this.OnUpdateTexture.SafeInvoke(this.texture, (float)this.orient);
			}
		}
	}

	public void Stop()
	{
		this.isAvalibleCam = false;
		if (this.webcamTexture != null)
		{
			this.webcamTexture.Stop();
			 Destroy(this.webcamTexture);
			this.webcamTexture = null;
		}
	}

	private void OnDestroy()
	{
		if (this.webcamTexture != null)
		{
			this.webcamTexture.Stop();
			 Destroy(this.webcamTexture);
			this.webcamTexture = null;
		}
	}
}


