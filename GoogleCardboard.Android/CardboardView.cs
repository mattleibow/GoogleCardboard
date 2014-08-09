using System;
using System.Threading;
using Android.Content;
using Android.Util;
using Java.Interop;
using Java.Lang;
using Google.Cardboard.Sensors;
using Math = Java.Lang.Math;
using Android.Views;
using AMatrix = Android.Opengl.Matrix;
using Android.Opengl;
using IGL10 = Javax.Microedition.Khronos.Opengles.IGL10;
using EGLConfig = Javax.Microedition.Khronos.Egl.EGLConfig;
using Object = Java.Lang.Object;

namespace Google.Cardboard
{
	public class CardboardView : GLSurfaceView
	{
		private static string TAG = "CardboardView";
		private static float DEFAULT_Z_NEAR = 0.1F;
		private static float DEFAULT_Z_FAR = 100.0F;
		private RendererHelper mRendererHelper;
		private HeadTracker mHeadTracker;
		private DistortionRenderer mDistortionRenderer;
		private ICardboardDeviceParamsObserver mCardboardDeviceParamsObserver;
		private bool mVRMode = true;
		private volatile bool mDistortionCorrectionEnabled = true;
		private volatile float mDistortionCorrectionScale = 1.0F;
		private float mZNear = 0.1F;
		private float mZFar = 100.0F;

		private const float EPSILON = 000.1f;

		public CardboardView(Context context) : base(context)
		{
			Init(context);
		}

		public CardboardView(Context context, IAttributeSet attrs) : base(context, attrs)
		{
			Init(context);
		}

		public void SetRenderer(IRenderer renderer)
		{
			mRendererHelper = (renderer != null ? new RendererHelper(this, renderer) : null);
			base.SetRenderer(mRendererHelper);
		}

		public void SetRenderer(IStereoRenderer renderer)
		{
			SetRenderer(renderer != null ? new StereoRendererHelper(this, renderer) : (IRenderer) null);
		}

		public bool VRModeEnabled
		{
			set
			{
				mVRMode = value;
				if (mRendererHelper != null)
				{
					mRendererHelper.SetVRModeEnabled(value);
				}
			}
		}

		public bool VRMode
		{
			get { return mVRMode; }
		}

		public HeadMountedDisplay HeadMountedDisplay { get; private set; }

		public void SetCardboardDeviceParamsObserver(ICardboardDeviceParamsObserver observer)
		{
			mCardboardDeviceParamsObserver = observer;
		}

		public CardboardDeviceParams CardboardDeviceParams
		{
			get { return HeadMountedDisplay.Cardboard; }
			set
			{
				if ((value == null) || (value.Equals(HeadMountedDisplay.Cardboard)))
				{
					return;
				}
				if (mCardboardDeviceParamsObserver != null)
				{
					mCardboardDeviceParamsObserver.OnCardboardDeviceParamsUpdate(value);
				}
				HeadMountedDisplay.Cardboard = (value);
				if (mRendererHelper != null)
				{
					mRendererHelper.SetCardboardDeviceParams(value);
				}
			}
		}

		public ScreenParams ScreenParams
		{
			set
			{
				if ((value == null) || (value.Equals(HeadMountedDisplay.Screen)))
				{
					return;
				}
				HeadMountedDisplay.Screen = (value);
				if (mRendererHelper != null)
				{
					mRendererHelper.SetScreenParams(value);
				}
			}
			get { return HeadMountedDisplay.Screen; }
		}

		public float InterpupillaryDistance
		{
			get { return HeadMountedDisplay.Cardboard.InterpupillaryDistance; }
			set
			{
				HeadMountedDisplay.Cardboard.InterpupillaryDistance = (value);
				if (mRendererHelper != null)
				{
					mRendererHelper.SetInterpupillaryDistance(value);
				}
			}
		}

		public float FovY
		{
			set
			{
				HeadMountedDisplay.Cardboard.FovY = (value);
				if (mRendererHelper != null)
				{
					mRendererHelper.SetFOV(value);
				}
			}
			get { return HeadMountedDisplay.Cardboard.FovY; }
		}

		public void SetZPlanes(float zNear, float zFar)
		{
			mZNear = zNear;
			mZFar = zFar;
			if (mRendererHelper != null)
			{
				mRendererHelper.SetZPlanes(zNear, zFar);
			}
		}

		public float ZNear
		{
			get { return mZNear; }
		}

		public float ZFar
		{
			get { return mZFar; }
		}

		public bool DistortionCorrectionEnabled
		{
			set
			{
				mDistortionCorrectionEnabled = value;
				if (mRendererHelper != null)
				{
					mRendererHelper.SetDistortionCorrectionEnabled(value);
				}
			}
			get { return mDistortionCorrectionEnabled; }
		}

		public float DistortionCorrectionScale
		{
			set
			{
				mDistortionCorrectionScale = value;
				if (mRendererHelper != null)
				{
					mRendererHelper.SetDistortionCorrectionScale(value);
				}
			}
			get { return mDistortionCorrectionScale; }
		}

		public override void OnResume()
		{
			if (mRendererHelper == null)
			{
				return;
			}
			base.OnResume();
			mHeadTracker.StartTracking();
		}

		public override void OnPause()
		{
			if (mRendererHelper == null)
			{
				return;
			}
			base.OnPause();
			mHeadTracker.StopTracking();
		}

		public override void SetRenderer(GLSurfaceView.IRenderer renderer)
		{
			throw new RuntimeException("Please use the CardboardView renderer interfaces");
		}

		protected override void OnDetachedFromWindow()
		{
			if (mRendererHelper != null)
			{
				lock (mRendererHelper)
				{
					mRendererHelper.Shutdown();
					try
					{
						Monitor.Wait(mRendererHelper);
					}
					catch (InterruptedException e)
					{
						Log.Error("CardboardView", "Interrupted during shutdown: " + e.ToString());
					}
				}
			}
			base.OnDetachedFromWindow();
		}

		private void Init(Context context)
		{
			SetEGLContextClientVersion(2);
			PreserveEGLContextOnPause = (true);

			IWindowManager windowManager = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

			mHeadTracker = new HeadTracker(context);
			HeadMountedDisplay = new HeadMountedDisplay(windowManager.DefaultDisplay);
		}

		public interface IRenderer
		{
			void OnDrawFrame(HeadTransform head, EyeParams leftEyes, EyeParams rightEye);

			void OnFinishFrame(Viewport viewport);

			void OnSurfaceChanged(int width, int height);

			void OnSurfaceCreated(EGLConfig config);

			void OnRendererShutdown();
		}

		public interface IStereoRenderer
		{
			void OnNewFrame(HeadTransform transform);

			void OnDrawEye(EyeTransform transform);

			void OnFinishFrame(Viewport viewport);

			void OnSurfaceChanged(int width, int height);

			void OnSurfaceCreated(EGLConfig config);

			void OnRendererShutdown();
		}

		public interface ICardboardDeviceParamsObserver
		{
			void OnCardboardDeviceParamsUpdate(CardboardDeviceParams paramCardboardDeviceParams);
		}

		private class RendererHelper : Object, GLSurfaceView.IRenderer
		{
			private HeadTransform mHeadTransform;
			private EyeParams mMonocular;
			private EyeParams mLeftEye;
			private EyeParams mRightEye;
			private float[] mLeftEyeTranslate;
			private float[] mRightEyeTranslate;
			private IRenderer mRenderer;
			private bool mShuttingDown;
			private HeadMountedDisplay mHmd;
			private bool mVRMode;
			private bool mDistortionCorrectionEnabled;
			private float mDistortionCorrectionScale;
			private float mZNear;
			private float mZFar;
			private bool mProjectionChanged;
			private bool mInvalidSurfaceSize;

			private readonly CardboardView parentCardboardView;

			public RendererHelper(CardboardView cardboardView, IRenderer renderer)
			{
				parentCardboardView = cardboardView;

				mRenderer = renderer;
				mHmd = new HeadMountedDisplay(parentCardboardView.HeadMountedDisplay);
				mHeadTransform = new HeadTransform();
				mMonocular = new EyeParams(0);
				mLeftEye = new EyeParams(1);
				mRightEye = new EyeParams(2);
				UpdateFieldOfView(mLeftEye.Fov, mRightEye.Fov);
				parentCardboardView.mDistortionRenderer = new DistortionRenderer();

				mLeftEyeTranslate = new float[16];
				mRightEyeTranslate = new float[16];


				mVRMode = parentCardboardView.mVRMode;
				mDistortionCorrectionEnabled = parentCardboardView.mDistortionCorrectionEnabled;
				mDistortionCorrectionScale = parentCardboardView.mDistortionCorrectionScale;
				mZNear = parentCardboardView.mZNear;
				mZFar = parentCardboardView.mZFar;


				mProjectionChanged = true;
			}

			public void Shutdown()
			{
				parentCardboardView.QueueEvent(new Runnable(() =>
				{
					lock (this)
					{
						mShuttingDown = true;
						mRenderer.OnRendererShutdown();
						Monitor.PulseAll(this);
					}
				}));
			}

			public void SetCardboardDeviceParams(CardboardDeviceParams newParams)
			{
				CardboardDeviceParams deviceParams = new CardboardDeviceParams(newParams);
				parentCardboardView.QueueEvent(new Runnable(() =>
				{
					mHmd.Cardboard = (deviceParams);
					mProjectionChanged = true;
				}));
			}

			public void SetScreenParams(ScreenParams newParams)
			{
				ScreenParams screenParams = new ScreenParams(newParams);
				parentCardboardView.QueueEvent(new Runnable(() =>
				{
					mHmd.Screen = (screenParams);
					mProjectionChanged = true;
				}));
			}

			public void SetInterpupillaryDistance(float interpupillaryDistance)
			{
				parentCardboardView.QueueEvent(new Runnable(() =>
				{
					mHmd.Cardboard.InterpupillaryDistance = (interpupillaryDistance);
					mProjectionChanged = true;
				}));
			}

			public void SetFOV(float fovY)
			{
				parentCardboardView.QueueEvent(new Runnable(() =>
				{
					mHmd.Cardboard.FovY = (fovY);
					mProjectionChanged = true;
				}));
			}

			public void SetZPlanes(float zNear, float zFar)
			{
				parentCardboardView.QueueEvent(new Runnable(() =>
				{
					mZNear = zNear;
					mZFar = zFar;
					mProjectionChanged = true;
				}));
			}

			public void SetDistortionCorrectionEnabled(bool enabled)
			{
				parentCardboardView.QueueEvent(new Runnable(() =>
				{
					mDistortionCorrectionEnabled = enabled;
					mProjectionChanged = true;
				}));
			}

			public void SetDistortionCorrectionScale(float scale)
			{
				parentCardboardView.QueueEvent(new Runnable(() =>
				{
					mDistortionCorrectionScale = scale;
					parentCardboardView.mDistortionRenderer.ResolutionScale = (scale);
				}));
			}

			public void SetVRModeEnabled(bool enabled)
			{
				parentCardboardView.QueueEvent(new Runnable(() =>
				{
					if (mVRMode == enabled)
					{
						return;
					}
					mVRMode = enabled;
					if ((mRenderer is StereoRendererHelper))
					{
						StereoRendererHelper stereoHelper = (StereoRendererHelper) mRenderer;
						stereoHelper.SetVRModeEnabled(enabled);
					}
					mProjectionChanged = true;
					OnSurfaceChanged((IGL10) null, mHmd.Screen.Width, mHmd.Screen.Height);
				}));
			}

			public void OnDrawFrame(IGL10 gl)
			{
				if ((mShuttingDown) || (mInvalidSurfaceSize))
				{
					return;
				}
				ScreenParams screen = mHmd.Screen;
				CardboardDeviceParams cdp = mHmd.Cardboard;

				parentCardboardView.mHeadTracker.GetLastHeadView(mHeadTransform.HeadView, 0);

				float halfInterpupillaryDistance = cdp.InterpupillaryDistance*0.5F;
				if (mVRMode)
				{
					AMatrix.SetIdentityM(mLeftEyeTranslate, 0);
					AMatrix.SetIdentityM(mRightEyeTranslate, 0);
					AMatrix.TranslateM(mLeftEyeTranslate, 0, halfInterpupillaryDistance, 0.0F, 0.0F);
					AMatrix.TranslateM(mRightEyeTranslate, 0, -halfInterpupillaryDistance, 0.0F, 0.0F);
					AMatrix.MultiplyMM(mLeftEye.Transform.EyeView, 0, mLeftEyeTranslate, 0, mHeadTransform.HeadView, 0);
					AMatrix.MultiplyMM(mRightEye.Transform.EyeView, 0, mRightEyeTranslate, 0, mHeadTransform.HeadView, 0);
				}
				else
				{
					Array.Copy(mHeadTransform.HeadView, 0, mMonocular.Transform.EyeView, 0,
						mHeadTransform.HeadView.Length);
				}
				if (mProjectionChanged)
				{
					mMonocular.Viewport.SetViewport(0, 0, screen.Width, screen.Height);
					if (!mVRMode)
					{
						float aspectRatio = screen.Width/screen.Height;
						AMatrix.PerspectiveM(mMonocular.Transform.Perspective, 0, cdp.FovY, aspectRatio, mZNear, mZFar);
					}
					else if (mDistortionCorrectionEnabled)
					{
						UpdateFieldOfView(mLeftEye.Fov, mRightEye.Fov);
						parentCardboardView.mDistortionRenderer.onProjectionChanged(mHmd, mLeftEye, mRightEye,
							mZNear, mZFar);
					}
					else
					{
						float distEyeToScreen = cdp.VisibleViewportSize/2.0F/(float) Math.Tan(Math.ToRadians(cdp.FovY)/2.0D);

						float left = screen.WidthMeters/2.0F - halfInterpupillaryDistance;
						float right = halfInterpupillaryDistance;
						float bottom = cdp.VerticalDistanceToLensCenter - screen.BorderSizeMeters;

						float top = screen.BorderSizeMeters + screen.HeightMeters - cdp.VerticalDistanceToLensCenter;


						FieldOfView leftEyeFov = mLeftEye.Fov;
						leftEyeFov.Left = ((float) Math.ToDegrees(Math.Atan2(left, distEyeToScreen)));
						leftEyeFov.Right = ((float) Math.ToDegrees(Math.Atan2(right, distEyeToScreen)));
						leftEyeFov.Bottom = ((float) Math.ToDegrees(Math.Atan2(bottom, distEyeToScreen)));
						leftEyeFov.Top = ((float) Math.ToDegrees(Math.Atan2(top, distEyeToScreen)));

						FieldOfView rightEyeFov = mRightEye.Fov;
						rightEyeFov.Left = (leftEyeFov.Right);
						rightEyeFov.Right = (leftEyeFov.Left);
						rightEyeFov.Bottom = (leftEyeFov.Bottom);
						rightEyeFov.Top = (leftEyeFov.Top);

						leftEyeFov.toPerspectiveMatrix(mZNear, mZFar, mLeftEye.Transform.Perspective, 0);

						rightEyeFov.toPerspectiveMatrix(mZNear, mZFar, mRightEye.Transform.Perspective, 0);


						mLeftEye.Viewport.SetViewport(0, 0, screen.Width/2, screen.Height);

						mRightEye.Viewport.SetViewport(screen.Width/2, 0, screen.Width/2, screen.Height);
					}
					mProjectionChanged = false;
				}
				if (mVRMode)
				{
					if (mDistortionCorrectionEnabled)
					{
						parentCardboardView.mDistortionRenderer.beforeDrawFrame();
						if (System.Math.Abs(mDistortionCorrectionScale - 1.0F) < EPSILON)
						{
							mRenderer.OnDrawFrame(mHeadTransform, mLeftEye, mRightEye);
						}
						else
						{
							int leftX = mLeftEye.Viewport.x;
							int leftY = mLeftEye.Viewport.y;
							int leftWidth = mLeftEye.Viewport.width;
							int leftHeight = mLeftEye.Viewport.height;
							int rightX = mRightEye.Viewport.x;
							int rightY = mRightEye.Viewport.y;
							int rightWidth = mRightEye.Viewport.width;
							int rightHeight = mRightEye.Viewport.height;


							mLeftEye.Viewport.SetViewport((int) (leftX*mDistortionCorrectionScale),
								(int) (leftY*mDistortionCorrectionScale), (int) (leftWidth*mDistortionCorrectionScale),
								(int) (leftHeight*mDistortionCorrectionScale));


							mRightEye.Viewport.SetViewport((int) (rightX*mDistortionCorrectionScale),
								(int) (rightY*mDistortionCorrectionScale), (int) (rightWidth*mDistortionCorrectionScale),
								(int) (rightHeight*mDistortionCorrectionScale));


							mRenderer.OnDrawFrame(mHeadTransform, mLeftEye, mRightEye);


							mLeftEye.Viewport.SetViewport(leftX, leftY, leftWidth, leftHeight);

							mRightEye.Viewport.SetViewport(rightX, rightY, rightWidth, rightHeight);
						}
						parentCardboardView.mDistortionRenderer.afterDrawFrame();
					}
					else
					{
						mRenderer.OnDrawFrame(mHeadTransform, mLeftEye, mRightEye);
					}
				}
				else
				{
					mRenderer.OnDrawFrame(mHeadTransform, mMonocular, null);
				}
				mRenderer.OnFinishFrame(mMonocular.Viewport);
			}

			public void OnSurfaceChanged(IGL10 gl, int width, int height)
			{
				if (mShuttingDown)
				{
					return;
				}
				ScreenParams screen = mHmd.Screen;
				if ((width != screen.Width) || (height != screen.Height))
				{
					if (!mInvalidSurfaceSize)
					{
						GLES20.GlClear(16384);
						Log.Warn("CardboardView",
							"Surface size " + width + "x" + height + " does not match the expected screen size " + screen.Width + "x" +
							screen.Height + ". Rendering is disabled.");
					}
					mInvalidSurfaceSize = true;
				}
				else
				{
					mInvalidSurfaceSize = false;
				}
				mRenderer.OnSurfaceChanged(width, height);
			}

			public void OnSurfaceCreated(IGL10 gl, EGLConfig config)
			{
				if (mShuttingDown)
				{
					return;
				}
				mRenderer.OnSurfaceCreated(config);
			}

			private void UpdateFieldOfView(FieldOfView leftEyeFov, FieldOfView rightEyeFov)
			{
				CardboardDeviceParams cdp = mHmd.Cardboard;
				ScreenParams screen = mHmd.Screen;
				Distortion distortion = cdp.Distortion;

				float idealFovAngle = (float) Math.ToDegrees(Math.Atan2(cdp.LensDiameter/2.0F, cdp.EyeToLensDistance));
				float eyeToScreenDist = cdp.EyeToLensDistance + cdp.ScreenToLensDistance;
				float outerDist = (screen.WidthMeters - cdp.InterpupillaryDistance)/2.0F;
				float innerDist = cdp.InterpupillaryDistance/2.0F;
				float bottomDist = cdp.VerticalDistanceToLensCenter - screen.BorderSizeMeters;
				float topDist = screen.HeightMeters + screen.BorderSizeMeters - cdp.VerticalDistanceToLensCenter;
				float outerAngle = (float) Math.ToDegrees(Math.Atan2(distortion.distort(outerDist), eyeToScreenDist));
				float innerAngle = (float) Math.ToDegrees(Math.Atan2(distortion.distort(innerDist), eyeToScreenDist));
				float bottomAngle = (float) Math.ToDegrees(Math.Atan2(distortion.distort(bottomDist), eyeToScreenDist));
				float topAngle = (float) Math.ToDegrees(Math.Atan2(distortion.distort(topDist), eyeToScreenDist));

				leftEyeFov.Left = (Math.Min(outerAngle, idealFovAngle));
				leftEyeFov.Right = (Math.Min(innerAngle, idealFovAngle));
				leftEyeFov.Bottom = (Math.Min(bottomAngle, idealFovAngle));
				leftEyeFov.Top = (Math.Min(topAngle, idealFovAngle));

				rightEyeFov.Left = (Math.Min(innerAngle, idealFovAngle));
				rightEyeFov.Right = (Math.Min(outerAngle, idealFovAngle));
				rightEyeFov.Bottom = (Math.Min(bottomAngle, idealFovAngle));
				rightEyeFov.Top = (Math.Min(topAngle, idealFovAngle));
			}
		}

		private class StereoRendererHelper : IRenderer
		{
			private IStereoRenderer mStereoRenderer;
			private bool mVRMode;

			private readonly CardboardView parentCardboardView;

			public StereoRendererHelper(CardboardView cardboardView, IStereoRenderer stereoRenderer)
			{
				parentCardboardView = cardboardView;
				mStereoRenderer = stereoRenderer;
				mVRMode = parentCardboardView.mVRMode;
			}

			public void SetVRModeEnabled(bool enabled)
			{
				parentCardboardView.QueueEvent(new Runnable(() => { mVRMode = enabled; }));
			}

			public void OnDrawFrame(HeadTransform head, EyeParams leftEye, EyeParams rightEye)
			{
				mStereoRenderer.OnNewFrame(head);
				GLES20.GlEnable(3089);

				leftEye.Viewport.SetGlViewport();
				leftEye.Viewport.SetGlScissor();
				mStereoRenderer.OnDrawEye(leftEye.Transform);
				if (rightEye == null)
				{
					return;
				}
				rightEye.Viewport.SetGlViewport();
				rightEye.Viewport.SetGlScissor();
				mStereoRenderer.OnDrawEye(rightEye.Transform);
			}

			public void OnFinishFrame(Viewport viewport)
			{
				viewport.SetGlViewport();
				viewport.SetGlScissor();
				mStereoRenderer.OnFinishFrame(viewport);
			}

			public void OnSurfaceChanged(int width, int height)
			{
				if (mVRMode)
				{
					mStereoRenderer.OnSurfaceChanged(width/2, height);
				}
				else
				{
					mStereoRenderer.OnSurfaceChanged(width, height);
				}
			}

			public void OnSurfaceCreated(EGLConfig config)
			{
				mStereoRenderer.OnSurfaceCreated(config);
			}

			public void OnRendererShutdown()
			{
				mStereoRenderer.OnRendererShutdown();
			}
		}
	}
}