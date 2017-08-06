using UnityEngine;
using Live2D.Cubism.Viewer;
using Live2D.Cubism.Core;
/**
 * Modified version of CameraCapture.cs, originally written by Keijiro Takahashi for FFmpegOut:
 * https://github.com/keijiro/FFmpegOut
 * 
 * Copyright by Keijiro Takahashi, licensed under the MIT license:
 * https://github.com/keijiro/FFmpegOut/blob/master/LICENSE.md
**/
namespace FFmpegOut
{
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("FFmpegOut/Camera Capture")]
	public class CubismCameraCapture : MonoBehaviour
	{

		[SerializeField] public bool _setResolution = true;
		[SerializeField] public int _width = 1280;
		[SerializeField] public int _height = 720;
		[SerializeField] public int _frameRate = 30;
		[SerializeField] public bool _allowSlowDown = true;
		[SerializeField] FFmpegPipe.Codec _codec;
		[SerializeField] public float _startTime = 0;
		[SerializeField] public float _recordLength = 1;
		[SerializeField] public Animation _anim = null;


		[SerializeField] public Shader _shader;
		[SerializeField] public Material _material;

		FFmpegPipe _pipe;
		float _elapsed;

		bool _firstFrame = true;

		RenderTexture _tempTarget;
		GameObject _tempBlitter;

		static int _activePipeCount;


		#region MonoBehavior functions

		void OnEnable()
		{
			if (!FFmpegConfig.CheckAvailable)
			{
				Debug.LogError(
					"ffmpeg.exe is missing. " +
					"Please refer to the installation instruction. " +
					"https://github.com/keijiro/FFmpegOut"
				);
				enabled = false;
			}

		}

		void OnDisable()
		{
			if (_pipe != null) ClosePipe();
		}

		void OnDestroy()
		{
			if (_pipe != null) ClosePipe();
		}




		void Update()
		{
			_elapsed += Time.deltaTime;

			if (_startTime <= _elapsed && _elapsed < _startTime + _recordLength)
			{
				if (_pipe == null) OpenPipe();
			}
			else
			{
				if (_pipe != null) ClosePipe();
			}
		}
			

		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (_pipe != null)
			{
				_startTime = Mathf.Max(_startTime, 0);
				_recordLength = Mathf.Max(_recordLength, 0.01f);

				if (!_anim.isPlaying) {
					return;
				}

				// HACK First frame is from last animation. Skip.
				if (_firstFrame) {
					_firstFrame = false;
					return;
				}

				var tempRT = RenderTexture.GetTemporary(source.width, source.height);
				Graphics.Blit(source, tempRT, _material, 0);

				var tempTex = new Texture2D(source.width, source.height, TextureFormat.RGB24, false);
				tempTex.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0, false);
				tempTex.Apply();

				_pipe.Write(tempTex.GetRawTextureData());

				Destroy(tempTex);
				RenderTexture.ReleaseTemporary(tempRT);
			}

			Graphics.Blit(source, destination);
		}

		#endregion

		#region Private methods

		void OpenPipe()
		{
			if (_pipe != null) return;

			var camera = GetComponent<Camera>();
			var width = _width;
			var height = _height;

			// Apply the screen resolution settings.
			if (_setResolution)
			{
				_tempTarget = RenderTexture.GetTemporary(width, height);
				camera.targetTexture = _tempTarget;
				_tempBlitter = Blitter.CreateGameObject(camera);
			}
			else
			{
				width = camera.pixelWidth;
				height = camera.pixelHeight;
			}

			// Open an output stream.
			_codec = FFmpegPipe.Codec.ProRes;
			_pipe = new FFmpegPipe(_anim.clip.name.Replace(".motion3.json", string.Empty), width, height, _frameRate, _codec);
			_activePipeCount++;

			// Change the application frame rate on the first pipe.
			if (_activePipeCount == 1)
			{
				if (_allowSlowDown)
					Time.captureFramerate = _frameRate;
				else
					Application.targetFrameRate = _frameRate;
			}

			Debug.Log("Capture started (" + _pipe.Filename + ")");
		}

		void ClosePipe()
		{
			var camera = GetComponent<Camera>();

			// Destroy the blitter object.
			if (_tempBlitter != null)
			{
				Destroy(_tempBlitter);
				_tempBlitter = null;
			}

			// Release the temporary render target.
			if (_tempTarget != null && _tempTarget == camera.targetTexture)
			{
				camera.targetTexture = null;
				RenderTexture.ReleaseTemporary(_tempTarget);
				_tempTarget = null;
			}

			// Close the output stream.
			if (_pipe != null)
			{
				Debug.Log("Capture ended (" + _pipe.Filename + ")");

				_pipe.Close();
				_activePipeCount--;

				if (!string.IsNullOrEmpty(_pipe.Error))
				{
					Debug.LogWarning(
						"ffmpeg returned with a warning or an error message. " +
						"See the following lines for details:\n" + _pipe.Error
					);
				}

				_pipe = null;

				// Reset the application frame rate on the last pipe.
				if (_activePipeCount == 0)
				{
					if (_allowSlowDown)
						Time.captureFramerate = 0;
					else
						Application.targetFrameRate = -1;
				}
			}
		}

		#endregion
	}
}
