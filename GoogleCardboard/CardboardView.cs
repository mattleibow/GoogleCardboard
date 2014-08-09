using System;
using System.Threading;

 namespace com.google.vrtoolkit.cardboard
 {
	 
		 using Context = android.content.Context;
		 using GLES20 = android.opengl.GLES20;
		 using GLSurfaceView = android.opengl.GLSurfaceView;
		 using Renderer = android.opengl.GLSurfaceView.Renderer;
		 using Matrix = android.opengl.Matrix;
		 using AttributeSet = android.util.AttributeSet;
		 using Log = android.util.Log;
		 using WindowManager = android.view.WindowManager;
		 using HeadTracker = com.google.vrtoolkit.cardboard.sensors.HeadTracker;
			 
		 public class CardboardView : GLSurfaceView
	
	
	 {		   private const string TAG = "CardboardView";
		   private const float DEFAULT_Z_NEAR = 0.1F;
		   private const float DEFAULT_Z_FAR = 100.0F;
		   private RendererHelper mRendererHelper;
		   private HeadTracker mHeadTracker;
		   private HeadMountedDisplay mHmd;
		   private DistortionRenderer mDistortionRenderer;
		   private CardboardDeviceParamsObserver mCardboardDeviceParamsObserver;
	/*   26:  65 */	   private bool mVRMode = true;
	/*   27:  66 */	   private volatile bool mDistortionCorrectionEnabled = true;
	/*   28:  67 */	   private volatile float mDistortionCorrectionScale = 1.0F;
	/*   29:  68 */	   private float mZNear = 0.1F;
	/*   30:  69 */	   private float mZFar = 100.0F;
	   
		   public CardboardView(Context context) : base(context)
	
	   {				 init(context);
	
	   }	   
		   public CardboardView(Context context, AttributeSet attrs) : base(context, attrs)
	
	   {				 init(context);
	
	   }	   
	
	   public virtual GLSurfaceView.Renderer Renderer
	   {
		   set
		
		   {					 this.mRendererHelper = (value != null ? new RendererHelper(this, value) : null);
					 base.Renderer = this.mRendererHelper;
		
		   }
	   }	   
	
	   public virtual StereoRenderer Renderer
	   {
		   set
		
		   {					 Renderer = value != null ? new StereoRendererHelper(this, value) : (GLSurfaceView.Renderer)null;
		
		   }
	   }	   
	
	   public virtual bool VRModeEnabled
	   {
		   set
		
		   {					 this.mVRMode = value;
					 if (this.mRendererHelper != null)
			 {
					   this.mRendererHelper.VRModeEnabled = value;
		
			 }		
		   }
	   }	   
	
	   public virtual bool VRMode
	   {
		   get
		
		   {					 return this.mVRMode;
		
		   }
	   }	   
	
	   public virtual HeadMountedDisplay HeadMountedDisplay
	   {
		   get
		
		   {					 return this.mHmd;
		
		   }
	   }	   
		   public virtual void updateCardboardDeviceParams(CardboardDeviceParams cardboardDeviceParams)
	
	   {			 if ((cardboardDeviceParams == null) || (cardboardDeviceParams.Equals(this.mHmd.Cardboard)))
		 {
			   return;
	
		 }			 if (this.mCardboardDeviceParamsObserver != null)
		 {
			   this.mCardboardDeviceParamsObserver.onCardboardDeviceParamsUpdate(cardboardDeviceParams);
	
		 }			 this.mHmd.Cardboard = cardboardDeviceParams;
			 if (this.mRendererHelper != null)
		 {
			   this.mRendererHelper.CardboardDeviceParams = cardboardDeviceParams;
	
		 }	
	   }	   
	
	   public virtual CardboardDeviceParamsObserver CardboardDeviceParamsObserver
	   {
		   set
		
		   {					 this.mCardboardDeviceParamsObserver = value;
		
		   }
	   }	   
	
	   public virtual CardboardDeviceParams CardboardDeviceParams
	   {
		   get
		
		   {					 return this.mHmd.Cardboard;
		
		   }		   set
		
		   {			//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final CardboardDeviceParams deviceParams = new CardboardDeviceParams(value);
			   CardboardDeviceParams deviceParams = new CardboardDeviceParams(value);
					   outerInstance.queueEvent(new RunnableAnonymousInnerClassHelper2(this, deviceParams));
		
		   }
	   }	   
		   public virtual void updateScreenParams(ScreenParams screenParams)
	
	   {			 if ((screenParams == null) || (screenParams.Equals(this.mHmd.Screen)))
		 {
			   return;
	
		 }			 this.mHmd.Screen = screenParams;
			 if (this.mRendererHelper != null)
		 {
			   this.mRendererHelper.ScreenParams = screenParams;
	
		 }	
	   }	   
	
	   public virtual ScreenParams ScreenParams
	   {
		   get
		
		   {					 return this.mHmd.Screen;
		
		   }		   set
		
		   {			//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final ScreenParams screenParams = new ScreenParams(value);
			   ScreenParams screenParams = new ScreenParams(value);
					   outerInstance.queueEvent(new RunnableAnonymousInnerClassHelper3(this, screenParams));
		
		   }
	   }	   
	
	   public virtual float InterpupillaryDistance
	   {
		   set
		
		   {					 this.mHmd.Cardboard.InterpupillaryDistance = value;
					 if (this.mRendererHelper != null)
			 {
					   this.mRendererHelper.InterpupillaryDistance = value;
		
			 }		
		   }		   get
		
		   {					 return this.mHmd.Cardboard.InterpupillaryDistance;
		
		   }
	   }	   
	
	   public virtual float FovY
	   {
		   set
		
		   {					 this.mHmd.Cardboard.FovY = value;
					 if (this.mRendererHelper != null)
			 {
					   this.mRendererHelper.FOV = value;
		
			 }		
		   }		   get
		
		   {					 return this.mHmd.Cardboard.FovY;
		
		   }
	   }	   
		   public virtual void setZPlanes(float zNear, float zFar)
	
	   {			 this.mZNear = zNear;
			 this.mZFar = zFar;
			 if (this.mRendererHelper != null)
		 {
			   this.mRendererHelper.setZPlanes(zNear, zFar);
	
		 }	
	   }	   
	
	   public virtual float ZNear
	   {
		   get
		
		   {					 return this.mZNear;
		
		   }
	   }	   
	
	   public virtual float ZFar
	   {
		   get
		
		   {					 return this.mZFar;
		
		   }
	   }	   
	
	   public virtual bool DistortionCorrectionEnabled
	   {
		   set
		
		   {					 this.mDistortionCorrectionEnabled = value;
					 if (this.mRendererHelper != null)
			 {
					   this.mRendererHelper.DistortionCorrectionEnabled = value;
		
			 }		
		   }		   get
		
		   {					 return this.mDistortionCorrectionEnabled;
		
		   }
	   }	   
	
	   public virtual float DistortionCorrectionScale
	   {
		   set
		
		   {					 this.mDistortionCorrectionScale = value;
					 if (this.mRendererHelper != null)
			 {
					   this.mRendererHelper.DistortionCorrectionScale = value;
		
			 }		
		   }		   get
		
		   {					 return this.mDistortionCorrectionScale;
		
		   }
	   }	   
		   public virtual void onResume()
	
	   {			 if (this.mRendererHelper == null)
		 {
			   return;
	
		 }			 base.onResume();
			 this.mHeadTracker.startTracking();
	
	   }	   
		   public virtual void onPause()
	
	   {			 if (this.mRendererHelper == null)
		 {
			   return;
	
		 }			 base.onPause();
			 this.mHeadTracker.stopTracking();
	
	   }	   
	
	   public virtual GLSurfaceView.Renderer Renderer
	   {
		   set
		
		   {					 throw new Exception("Please use the CardboardView renderer interfaces");
		
		   }
	   }	   
		   public virtual void onDetachedFromWindow()
	
	   {			 if (this.mRendererHelper != null)
		 {
			   lock (this.mRendererHelper)
	
		   {				 this.mRendererHelper.shutdown();
				 try
	
			 {				   Monitor.Wait(this.mRendererHelper);
	
			 }				 catch (InterruptedException e)
	
			 {				   Log.e("CardboardView", "Interrupted during shutdown: " + e.ToString());
	
			 }	
		   }	
		 }			 base.onDetachedFromWindow();
	
	   }	   
		   private void init(Context context)
	
	   {			 EGLContextClientVersion = 2;
			 PreserveEGLContextOnPause = true;
	     
			 WindowManager windowManager = (WindowManager)context.getSystemService("window");
	     
	 
			 this.mHeadTracker = new HeadTracker(context);
			 this.mHmd = new HeadMountedDisplay(windowManager.DefaultDisplay);
	
	   }	   
		   public abstract interface Renderer
	
	   {			 void onDrawFrame(HeadTransform paramHeadTransform, EyeParams paramEyeParams1, EyeParams paramEyeParams2);
	     
			 void onFinishFrame(Viewport paramViewport);
	     
			 void onSurfaceChanged(int paramInt1, int paramInt2);
	     
			 void onSurfaceCreated(EGLConfig paramEGLConfig);
	     
			 void onRendererShutdown();
	
	   }	   
		   public abstract interface StereoRenderer
	
	   {			 void onNewFrame(HeadTransform paramHeadTransform);
	     
			 void onDrawEye(EyeTransform paramEyeTransform);
	     
			 void onFinishFrame(Viewport paramViewport);
	     
			 void onSurfaceChanged(int paramInt1, int paramInt2);
	     
			 void onSurfaceCreated(EGLConfig paramEGLConfig);
	     
			 void onRendererShutdown();
	
	   }	   
		   public abstract interface CardboardDeviceParamsObserver
	
	   {			 void onCardboardDeviceParamsUpdate(CardboardDeviceParams paramCardboardDeviceParams);
	
	   }	   
		   private class RendererHelper : GLSurfaceView.Renderer
	
	
	   {		   private readonly CardboardView outerInstance;

			 internal readonly HeadTransform mHeadTransform;
			 internal readonly EyeParams mMonocular;
			 internal readonly EyeParams mLeftEye;
			 internal readonly EyeParams mRightEye;
			 internal readonly float[] mLeftEyeTranslate;
			 internal readonly float[] mRightEyeTranslate;
			 internal readonly CardboardView.Renderer mRenderer;
			 internal bool mShuttingDown;
			 internal HeadMountedDisplay mHmd;
			 internal bool mVRMode;
			 internal bool mDistortionCorrectionEnabled;
			 internal float mDistortionCorrectionScale;
			 internal float mZNear;
			 internal float mZFar;
			 internal bool mProjectionChanged;
			 internal bool mInvalidSurfaceSize;
	     
			 public RendererHelper(CardboardView outerInstance, CardboardView.Renderer renderer)
	
		 {			 this.outerInstance = outerInstance;
			   this.mRenderer = renderer;
			   this.mHmd = new HeadMountedDisplay(outerInstance.mHmd);
			   this.mHeadTransform = new HeadTransform();
			   this.mMonocular = new EyeParams(0);
			   this.mLeftEye = new EyeParams(1);
			   this.mRightEye = new EyeParams(2);
			   updateFieldOfView(this.mLeftEye.Fov, this.mRightEye.Fov);
			   outerInstance.mDistortionRenderer = new DistortionRenderer();
	       
			   this.mLeftEyeTranslate = new float[16];
			   this.mRightEyeTranslate = new float[16];
	       
	 
			   this.mVRMode = outerInstance.mVRMode;
			   this.mDistortionCorrectionEnabled = outerInstance.mDistortionCorrectionEnabled;
			   this.mDistortionCorrectionScale = outerInstance.mDistortionCorrectionScale;
			   this.mZNear = outerInstance.mZNear;
			   this.mZFar = outerInstance.mZFar;
	       
	 
			   this.mProjectionChanged = true;
	
		 }	     
			 public virtual void shutdown()
	
		 {			   outerInstance.queueEvent(new RunnableAnonymousInnerClassHelper(this));
	
		 }
		 private class RunnableAnonymousInnerClassHelper : Runnable
		 {
			 private readonly RendererHelper outerInstance;

			 public RunnableAnonymousInnerClassHelper(RendererHelper outerInstance)
			 {
				 this.outerInstance = outerInstance;
			 }

		 		 			 public virtual void run()
		 
			 {		 			   lock (outerInstance.outerInstance)
		 
			   {		 				 outerInstance.outerInstance.mShuttingDown = true;
		 				 outerInstance.outerInstance.mRenderer.onRendererShutdown();
		 				 Monitor.PulseAll(outerInstance.outerInstance);
		 
			   }		 
			 }		 
		 }
		 private class RunnableAnonymousInnerClassHelper2 : Runnable
		 {
			 private readonly RendererHelper outerInstance;

			 private CardboardDeviceParams deviceParams;

			 public RunnableAnonymousInnerClassHelper2(RendererHelper outerInstance, CardboardDeviceParams deviceParams)
			 {
				 this.outerInstance = outerInstance;
				 this.deviceParams = deviceParams;
			 }

		 		 			 public virtual void run()
		 
			 {		 			   outerInstance.outerInstance.mHmd.Cardboard = deviceParams;
		 			   outerInstance.outerInstance.mProjectionChanged = true;
		 
			 }		 
		 }
		 private class RunnableAnonymousInnerClassHelper3 : Runnable
		 {
			 private readonly RendererHelper outerInstance;

			 private ScreenParams screenParams;

			 public RunnableAnonymousInnerClassHelper3(RendererHelper outerInstance, ScreenParams screenParams)
			 {
				 this.outerInstance = outerInstance;
				 this.screenParams = screenParams;
			 }

		 		 			 public virtual void run()
		 
			 {		 			   outerInstance.outerInstance.mHmd.Screen = screenParams;
		 			   outerInstance.outerInstance.mProjectionChanged = true;
		 
			 }		 
		 }	     
	//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setInterpupillaryDistance(final float interpupillaryDistance)
		 public virtual float InterpupillaryDistance
		 {
			 set
		
			 {					   outerInstance.queueEvent(new RunnableAnonymousInnerClassHelper4(this, value));
		
			 }
		 }
		 private class RunnableAnonymousInnerClassHelper4 : Runnable
		 {
			 private readonly RendererHelper outerInstance;

			 private float interpupillaryDistance;

			 public RunnableAnonymousInnerClassHelper4(RendererHelper outerInstance, float interpupillaryDistance)
			 {
				 this.outerInstance = outerInstance;
				 this.interpupillaryDistance = interpupillaryDistance;
			 }

		 		 			 public virtual void run()
		 
			 {		 			   outerInstance.outerInstance.mHmd.Cardboard.InterpupillaryDistance = interpupillaryDistance;
		 			   outerInstance.outerInstance.mProjectionChanged = true;
		 
			 }		 
		 }	     
	//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setFOV(final float fovY)
		 public virtual float FOV
		 {
			 set
		
			 {					   outerInstance.queueEvent(new RunnableAnonymousInnerClassHelper5(this, value));
		
			 }
		 }
		 private class RunnableAnonymousInnerClassHelper5 : Runnable
		 {
			 private readonly RendererHelper outerInstance;

			 private float fovY;

			 public RunnableAnonymousInnerClassHelper5(RendererHelper outerInstance, float fovY)
			 {
				 this.outerInstance = outerInstance;
				 this.fovY = fovY;
			 }

		 		 			 public virtual void run()
		 
			 {		 			   outerInstance.outerInstance.mHmd.Cardboard.FovY = fovY;
		 			   outerInstance.outerInstance.mProjectionChanged = true;
		 
			 }		 
		 }	     
	//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setZPlanes(final float zNear, final float zFar)
		 public virtual void setZPlanes(float zNear, float zFar)
	
		 {			   outerInstance.queueEvent(new RunnableAnonymousInnerClassHelper6(this, zNear, zFar));
	
		 }
		 private class RunnableAnonymousInnerClassHelper6 : Runnable
		 {
			 private readonly RendererHelper outerInstance;

			 private float zNear;
			 private float zFar;

			 public RunnableAnonymousInnerClassHelper6(RendererHelper outerInstance, float zNear, float zFar)
			 {
				 this.outerInstance = outerInstance;
				 this.zNear = zNear;
				 this.zFar = zFar;
			 }

		 		 			 public virtual void run()
		 
			 {		 			   outerInstance.outerInstance.mZNear = zNear;
		 			   outerInstance.outerInstance.mZFar = zFar;
		 			   outerInstance.outerInstance.mProjectionChanged = true;
		 
			 }		 
		 }	     
	//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setDistortionCorrectionEnabled(final boolean enabled)
		 public virtual bool DistortionCorrectionEnabled
		 {
			 set
		
			 {					   outerInstance.queueEvent(new RunnableAnonymousInnerClassHelper7(this, value));
		
			 }
		 }
		 private class RunnableAnonymousInnerClassHelper7 : Runnable
		 {
			 private readonly RendererHelper outerInstance;

			 private bool enabled;

			 public RunnableAnonymousInnerClassHelper7(RendererHelper outerInstance, bool enabled)
			 {
				 this.outerInstance = outerInstance;
				 this.enabled = enabled;
			 }

		 		 			 public virtual void run()
		 
			 {		 			   outerInstance.outerInstance.mDistortionCorrectionEnabled = enabled;
		 			   outerInstance.outerInstance.mProjectionChanged = true;
		 
			 }		 
		 }	     
	//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setDistortionCorrectionScale(final float scale)
		 public virtual float DistortionCorrectionScale
		 {
			 set
		
			 {					   outerInstance.queueEvent(new RunnableAnonymousInnerClassHelper8(this, value));
		
			 }
		 }
		 private class RunnableAnonymousInnerClassHelper8 : Runnable
		 {
			 private readonly RendererHelper outerInstance;

			 private float scale;

			 public RunnableAnonymousInnerClassHelper8(RendererHelper outerInstance, float scale)
			 {
				 this.outerInstance = outerInstance;
				 this.scale = scale;
			 }

		 		 			 public virtual void run()
		 
			 {		 			   outerInstance.outerInstance.mDistortionCorrectionScale = scale;
		 			   outerInstance.outerInstance.mDistortionRenderer.ResolutionScale = scale;
		 
			 }		 
		 }	     
	//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setVRModeEnabled(final boolean enabled)
		 public virtual bool VRModeEnabled
		 {
			 set
		
			 {					   outerInstance.queueEvent(new RunnableAnonymousInnerClassHelper9(this, value));
		
			 }
		 }
		 private class RunnableAnonymousInnerClassHelper9 : Runnable
		 {
			 private readonly RendererHelper outerInstance;

			 private bool enabled;

			 public RunnableAnonymousInnerClassHelper9(RendererHelper outerInstance, bool enabled)
			 {
				 this.outerInstance = outerInstance;
				 this.enabled = enabled;
			 }

		 		 			 public virtual void run()
		 
			 {		 			   if (outerInstance.outerInstance.mVRMode == enabled)
			   {
		 				 return;
		 
			   }		 			   outerInstance.outerInstance.mVRMode = enabled;
		 			   if ((outerInstance.outerInstance.mRenderer is CardboardView.StereoRendererHelper))
		 
			   {		 				 CardboardView.StereoRendererHelper stereoHelper = (CardboardView.StereoRendererHelper)CardboardView.RendererHelper.this.mRenderer;
		 				 stereoHelper.VRModeEnabled = enabled;
		 
			   }		 			   outerInstance.outerInstance.mProjectionChanged = true;
		 			   outerInstance.outerInstance.onSurfaceChanged((GL10)null, outerInstance.outerInstance.mHmd.Screen.Width, outerInstance.outerInstance.mHmd.Screen.Height);
		 
			 }		 
		 }	     
			 public virtual void onDrawFrame(GL10 gl)
	
		 {			   if ((this.mShuttingDown) || (this.mInvalidSurfaceSize))
		   {
				 return;
	
		   }			   ScreenParams screen = this.mHmd.Screen;
			   CardboardDeviceParams cdp = this.mHmd.Cardboard;
	       
	 
			   outerInstance.mHeadTracker.getLastHeadView(this.mHeadTransform.HeadView, 0);
	       
	 
			   float halfInterpupillaryDistance = cdp.InterpupillaryDistance * 0.5F;
			   if (this.mVRMode)
	
		   {				 Matrix.setIdentityM(this.mLeftEyeTranslate, 0);
				 Matrix.setIdentityM(this.mRightEyeTranslate, 0);
	         
				 Matrix.translateM(this.mLeftEyeTranslate, 0, halfInterpupillaryDistance, 0.0F, 0.0F);
	         
				 Matrix.translateM(this.mRightEyeTranslate, 0, -halfInterpupillaryDistance, 0.0F, 0.0F);
	         
	 
	 
				 Matrix.multiplyMM(this.mLeftEye.Transform.EyeView, 0, this.mLeftEyeTranslate, 0, this.mHeadTransform.HeadView, 0);
	         
	 
				 Matrix.multiplyMM(this.mRightEye.Transform.EyeView, 0, this.mRightEyeTranslate, 0, this.mHeadTransform.HeadView, 0);
	
		   }			   else
	
		   {				 Array.Copy(this.mHeadTransform.HeadView, 0, this.mMonocular.Transform.EyeView, 0, this.mHeadTransform.HeadView.length);
	
		   }			   if (this.mProjectionChanged)
	
		   {				 this.mMonocular.Viewport.setViewport(0, 0, screen.Width, screen.Height);
				 if (!this.mVRMode)
	
			 {				   float aspectRatio = screen.Width / screen.Height;
				   Matrix.perspectiveM(this.mMonocular.Transform.Perspective, 0, cdp.FovY, aspectRatio, this.mZNear, this.mZFar);
	
			 }				 else if (this.mDistortionCorrectionEnabled)
	
			 {				   updateFieldOfView(this.mLeftEye.Fov, this.mRightEye.Fov);
				   outerInstance.mDistortionRenderer.onProjectionChanged(this.mHmd, this.mLeftEye, this.mRightEye, this.mZNear, this.mZFar);
	
			 }				 else
	
			 {				   float distEyeToScreen = cdp.VisibleViewportSize / 2.0F / (float)Math.Tan(Math.toRadians(cdp.FovY) / 2.0D);
	           
	 
	 
				   float left = screen.WidthMeters / 2.0F - halfInterpupillaryDistance;
				   float right = halfInterpupillaryDistance;
				   float bottom = cdp.VerticalDistanceToLensCenter - screen.BorderSizeMeters;
	           
				   float top = screen.BorderSizeMeters + screen.HeightMeters - cdp.VerticalDistanceToLensCenter;
	           
	 
				   FieldOfView leftEyeFov = this.mLeftEye.Fov;
				   leftEyeFov.Left = (float)Math.toDegrees(Math.Atan2(left, distEyeToScreen));
	           
				   leftEyeFov.Right = (float)Math.toDegrees(Math.Atan2(right, distEyeToScreen));
	           
				   leftEyeFov.Bottom = (float)Math.toDegrees(Math.Atan2(bottom, distEyeToScreen));
	           
				   leftEyeFov.Top = (float)Math.toDegrees(Math.Atan2(top, distEyeToScreen));
	           
	 
	 
				   FieldOfView rightEyeFov = this.mRightEye.Fov;
				   rightEyeFov.Left = leftEyeFov.Right;
				   rightEyeFov.Right = leftEyeFov.Left;
				   rightEyeFov.Bottom = leftEyeFov.Bottom;
				   rightEyeFov.Top = leftEyeFov.Top;
	           
				   leftEyeFov.toPerspectiveMatrix(this.mZNear, this.mZFar, this.mLeftEye.Transform.Perspective, 0);
	           
				   rightEyeFov.toPerspectiveMatrix(this.mZNear, this.mZFar, this.mRightEye.Transform.Perspective, 0);
	           
	 
				   this.mLeftEye.Viewport.setViewport(0, 0, screen.Width / 2, screen.Height);
	           
				   this.mRightEye.Viewport.setViewport(screen.Width / 2, 0, screen.Width / 2, screen.Height);
	
			 }				 this.mProjectionChanged = false;
	
		   }			   if (this.mVRMode)
	
		   {				 if (this.mDistortionCorrectionEnabled)
	
			 {				   outerInstance.mDistortionRenderer.beforeDrawFrame();
				   if (this.mDistortionCorrectionScale == 1.0F)
	
			   {					 this.mRenderer.onDrawFrame(this.mHeadTransform, this.mLeftEye, this.mRightEye);
	
			   }				   else
	
			   {					 int leftX = this.mLeftEye.Viewport.x;
					 int leftY = this.mLeftEye.Viewport.y;
					 int leftWidth = this.mLeftEye.Viewport.width;
					 int leftHeight = this.mLeftEye.Viewport.height;
					 int rightX = this.mRightEye.Viewport.x;
					 int rightY = this.mRightEye.Viewport.y;
					 int rightWidth = this.mRightEye.Viewport.width;
					 int rightHeight = this.mRightEye.Viewport.height;
	             
	 
					 this.mLeftEye.Viewport.setViewport((int)(leftX * this.mDistortionCorrectionScale), (int)(leftY * this.mDistortionCorrectionScale), (int)(leftWidth * this.mDistortionCorrectionScale), (int)(leftHeight * this.mDistortionCorrectionScale));
	             
	 
	 
	 
					 this.mRightEye.Viewport.setViewport((int)(rightX * this.mDistortionCorrectionScale), (int)(rightY * this.mDistortionCorrectionScale), (int)(rightWidth * this.mDistortionCorrectionScale), (int)(rightHeight * this.mDistortionCorrectionScale));
	             
	 
	 
	 
	 
	 
					 this.mRenderer.onDrawFrame(this.mHeadTransform, this.mLeftEye, this.mRightEye);
	             
	 
					 this.mLeftEye.Viewport.setViewport(leftX, leftY, leftWidth, leftHeight);
	             
					 this.mRightEye.Viewport.setViewport(rightX, rightY, rightWidth, rightHeight);
	
			   }				   outerInstance.mDistortionRenderer.afterDrawFrame();
	
			 }				 else
	
			 {				   this.mRenderer.onDrawFrame(this.mHeadTransform, this.mLeftEye, this.mRightEye);
	
			 }	
		   }			   else
		   {
				 this.mRenderer.onDrawFrame(this.mHeadTransform, this.mMonocular, null);
	
		   }			   this.mRenderer.onFinishFrame(this.mMonocular.Viewport);
	
		 }	     
			 public virtual void onSurfaceChanged(GL10 gl, int width, int height)
	
		 {			   if (this.mShuttingDown)
		   {
				 return;
	
		   }			   ScreenParams screen = this.mHmd.Screen;
			   if ((width != screen.Width) || (height != screen.Height))
	
		   {				 if (!this.mInvalidSurfaceSize)
	
			 {				   GLES20.glClear(16384);
				   Log.w("CardboardView", "Surface size " + width + "x" + height + " does not match the expected screen size " + screen.Width + "x" + screen.Height + ". Rendering is disabled.");
	
			 }				 this.mInvalidSurfaceSize = true;
	
		   }			   else
	
		   {				 this.mInvalidSurfaceSize = false;
	
		   }			   this.mRenderer.onSurfaceChanged(width, height);
	
		 }	     
			 public virtual void onSurfaceCreated(GL10 gl, EGLConfig config)
	
		 {			   if (this.mShuttingDown)
		   {
				 return;
	
		   }			   this.mRenderer.onSurfaceCreated(config);
	
		 }	     
			 internal virtual void updateFieldOfView(FieldOfView leftEyeFov, FieldOfView rightEyeFov)
	
		 {			   CardboardDeviceParams cdp = this.mHmd.Cardboard;
			   ScreenParams screen = this.mHmd.Screen;
			   Distortion distortion = cdp.Distortion;
	       
	 
	 
	 
	 
	 
			   float idealFovAngle = (float)Math.toDegrees(Math.Atan2(cdp.LensDiameter / 2.0F, cdp.EyeToLensDistance));
	       
	 
	 
	 
	 
	 
	 
	 
	 
	 
	 
			   float eyeToScreenDist = cdp.EyeToLensDistance + cdp.ScreenToLensDistance;
	       
	 
			   float outerDist = (screen.WidthMeters - cdp.InterpupillaryDistance) / 2.0F;
	       
			   float innerDist = cdp.InterpupillaryDistance / 2.0F;
			   float bottomDist = cdp.VerticalDistanceToLensCenter - screen.BorderSizeMeters;
	       
			   float topDist = screen.HeightMeters + screen.BorderSizeMeters - cdp.VerticalDistanceToLensCenter;
	       
	 
	 
			   float outerAngle = (float)Math.toDegrees(Math.Atan2(distortion.distort(outerDist), eyeToScreenDist));
	       
			   float innerAngle = (float)Math.toDegrees(Math.Atan2(distortion.distort(innerDist), eyeToScreenDist));
	       
			   float bottomAngle = (float)Math.toDegrees(Math.Atan2(distortion.distort(bottomDist), eyeToScreenDist));
	       
			   float topAngle = (float)Math.toDegrees(Math.Atan2(distortion.distort(topDist), eyeToScreenDist));
	       
	 
			   leftEyeFov.Left = Math.Min(outerAngle, idealFovAngle);
			   leftEyeFov.Right = Math.Min(innerAngle, idealFovAngle);
			   leftEyeFov.Bottom = Math.Min(bottomAngle, idealFovAngle);
			   leftEyeFov.Top = Math.Min(topAngle, idealFovAngle);
	       
			   rightEyeFov.Left = Math.Min(innerAngle, idealFovAngle);
			   rightEyeFov.Right = Math.Min(outerAngle, idealFovAngle);
			   rightEyeFov.Bottom = Math.Min(bottomAngle, idealFovAngle);
			   rightEyeFov.Top = Math.Min(topAngle, idealFovAngle);
	
		 }	
	   }	   
		   private class StereoRendererHelper : CardboardView.Renderer
	
	
	   {		   private readonly CardboardView outerInstance;

			 internal readonly CardboardView.StereoRenderer mStereoRenderer;
			 internal bool mVRMode;
	     
			 public StereoRendererHelper(CardboardView outerInstance, CardboardView.StereoRenderer stereoRenderer)
	
		 {			 this.outerInstance = outerInstance;
			   this.mStereoRenderer = stereoRenderer;
			   this.mVRMode = outerInstance.mVRMode;
	
		 }	     
	//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setVRModeEnabled(final boolean enabled)
		 public virtual bool VRModeEnabled
		 {
			 set
		
			 {					   outerInstance.queueEvent(new RunnableAnonymousInnerClassHelper(this, value));
		
			 }
		 }
		 private class RunnableAnonymousInnerClassHelper : Runnable
		 {
			 private readonly StereoRendererHelper outerInstance;

			 private bool enabled;

			 public RunnableAnonymousInnerClassHelper(StereoRendererHelper outerInstance, bool enabled)
			 {
				 this.outerInstance = outerInstance;
				 this.enabled = enabled;
			 }

		 		 			 public virtual void run()
		 
			 {		 			   outerInstance.outerInstance.mVRMode = enabled;
		 
			 }		 
		 }	     
			 public virtual void onDrawFrame(HeadTransform head, EyeParams leftEye, EyeParams rightEye)
	
		 {			   this.mStereoRenderer.onNewFrame(head);
			   GLES20.glEnable(3089);
	       
			   leftEye.Viewport.setGLViewport();
			   leftEye.Viewport.setGLScissor();
			   this.mStereoRenderer.onDrawEye(leftEye.Transform);
			   if (rightEye == null)
		   {
				 return;
	
		   }			   rightEye.Viewport.setGLViewport();
			   rightEye.Viewport.setGLScissor();
			   this.mStereoRenderer.onDrawEye(rightEye.Transform);
	
		 }	     
			 public virtual void onFinishFrame(Viewport viewport)
	
		 {			   viewport.setGLViewport();
			   viewport.setGLScissor();
			   this.mStereoRenderer.onFinishFrame(viewport);
	
		 }	     
			 public virtual void onSurfaceChanged(int width, int height)
	
		 {			   if (this.mVRMode)
		   {
				 this.mStereoRenderer.onSurfaceChanged(width / 2, height);
	
		   }		   else
		   {
				 this.mStereoRenderer.onSurfaceChanged(width, height);
	
		   }	
		 }	     
			 public virtual void onSurfaceCreated(EGLConfig config)
	
		 {			   this.mStereoRenderer.onSurfaceCreated(config);
	
		 }	     
			 public virtual void onRendererShutdown()
	
		 {			   this.mStereoRenderer.onRendererShutdown();
	
		 }	
	   }	
	 }

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     com.google.vrtoolkit.cardboard.CardboardView
	 * JD-Core Version:    0.7.0.1
	 */
 }