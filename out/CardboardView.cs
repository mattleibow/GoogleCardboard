using System;
using System.Threading;

/*    1:     */ namespace com.google.vrtoolkit.cardboard
 {
	/*    2:     */ 
	/*    3:     */	 using Context = android.content.Context;
	/*    4:     */	 using GLES20 = android.opengl.GLES20;
	/*    5:     */	 using GLSurfaceView = android.opengl.GLSurfaceView;
	/*    6:     */	 using Renderer = android.opengl.GLSurfaceView.Renderer;
	/*    7:     */	 using Matrix = android.opengl.Matrix;
	/*    8:     */	 using AttributeSet = android.util.AttributeSet;
	/*    9:     */	 using Log = android.util.Log;
	/*   10:     */	 using WindowManager = android.view.WindowManager;
	/*   11:     */	 using HeadTracker = com.google.vrtoolkit.cardboard.sensors.HeadTracker;
	/*   12:     */	/*   13:     */	/*   14:     */ 
	/*   15:     */	 public class CardboardView : GLSurfaceView
	/*   16:     */
	/*   17:     */
	 {	/*   18:     */	   private const string TAG = "CardboardView";
	/*   19:     */	   private const float DEFAULT_Z_NEAR = 0.1F;
	/*   20:     */	   private const float DEFAULT_Z_FAR = 100.0F;
	/*   21:     */	   private RendererHelper mRendererHelper;
	/*   22:     */	   private HeadTracker mHeadTracker;
	/*   23:     */	   private HeadMountedDisplay mHmd;
	/*   24:     */	   private DistortionRenderer mDistortionRenderer;
	/*   25:     */	   private CardboardDeviceParamsObserver mCardboardDeviceParamsObserver;
	/*   26:  65 */	   private bool mVRMode = true;
	/*   27:  66 */	   private volatile bool mDistortionCorrectionEnabled = true;
	/*   28:  67 */	   private volatile float mDistortionCorrectionScale = 1.0F;
	/*   29:  68 */	   private float mZNear = 0.1F;
	/*   30:  69 */	   private float mZFar = 100.0F;
	/*   31:     */   
	/*   32:     */	   public CardboardView(Context context) : base(context)
	/*   33:     */
	   {	/*   34: 268 */	/*   35: 269 */		 init(context);
	/*   36:     */
	   }	/*   37:     */   
	/*   38:     */	   public CardboardView(Context context, AttributeSet attrs) : base(context, attrs)
	/*   39:     */
	   {	/*   40: 273 */	/*   41: 274 */		 init(context);
	/*   42:     */
	   }	/*   43:     */   
	/*   44:     */
	   public virtual GLSurfaceView.Renderer Renderer
	   {
		   set
		/*   45:     */
		   {		/*   46: 285 */			 this.mRendererHelper = (value != null ? new RendererHelper(this, value) : null);
		/*   47: 286 */			 base.Renderer = this.mRendererHelper;
		/*   48:     */
		   }
	   }	/*   49:     */   
	/*   50:     */
	   public virtual StereoRenderer Renderer
	   {
		   set
		/*   51:     */
		   {		/*   52: 297 */			 Renderer = value != null ? new StereoRendererHelper(this, value) : (GLSurfaceView.Renderer)null;
		/*   53:     */
		   }
	   }	/*   54:     */   
	/*   55:     */
	   public virtual bool VRModeEnabled
	   {
		   set
		/*   56:     */
		   {		/*   57: 317 */			 this.mVRMode = value;
		/*   58: 319 */			 if (this.mRendererHelper != null)
			 {
		/*   59: 320 */			   this.mRendererHelper.VRModeEnabled = value;
		/*   60:     */
			 }		/*   61:     */
		   }
	   }	/*   62:     */   
	/*   63:     */
	   public virtual bool VRMode
	   {
		   get
		/*   64:     */
		   {		/*   65: 331 */			 return this.mVRMode;
		/*   66:     */
		   }
	   }	/*   67:     */   
	/*   68:     */
	   public virtual HeadMountedDisplay HeadMountedDisplay
	   {
		   get
		/*   69:     */
		   {		/*   70: 349 */			 return this.mHmd;
		/*   71:     */
		   }
	   }	/*   72:     */   
	/*   73:     */	   public virtual void updateCardboardDeviceParams(CardboardDeviceParams cardboardDeviceParams)
	/*   74:     */
	   {	/*   75: 361 */		 if ((cardboardDeviceParams == null) || (cardboardDeviceParams.Equals(this.mHmd.Cardboard)))
		 {
	/*   76: 362 */		   return;
	/*   77:     */
		 }	/*   78: 365 */		 if (this.mCardboardDeviceParamsObserver != null)
		 {
	/*   79: 366 */		   this.mCardboardDeviceParamsObserver.onCardboardDeviceParamsUpdate(cardboardDeviceParams);
	/*   80:     */
		 }	/*   81: 369 */		 this.mHmd.Cardboard = cardboardDeviceParams;
	/*   82: 371 */		 if (this.mRendererHelper != null)
		 {
	/*   83: 372 */		   this.mRendererHelper.CardboardDeviceParams = cardboardDeviceParams;
	/*   84:     */
		 }	/*   85:     */
	   }	/*   86:     */   
	/*   87:     */
	   public virtual CardboardDeviceParamsObserver CardboardDeviceParamsObserver
	   {
		   set
		/*   88:     */
		   {		/*   89: 387 */			 this.mCardboardDeviceParamsObserver = value;
		/*   90:     */
		   }
	   }	/*   91:     */   
	/*   92:     */
	   public virtual CardboardDeviceParams CardboardDeviceParams
	   {
		   get
		/*   93:     */
		   {		/*   94: 404 */			 return this.mHmd.Cardboard;
		/*   95:     */
		   }		   set
		/*  333:     */
		   {		/*  334: 747 */	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final CardboardDeviceParams deviceParams = new CardboardDeviceParams(value);
			   CardboardDeviceParams deviceParams = new CardboardDeviceParams(value);
		/*  335: 748 */			   outerInstance.queueEvent(new RunnableAnonymousInnerClassHelper2(this, deviceParams));
		/*  343:     */
		   }
	   }	/*   96:     */   
	/*   97:     */	   public virtual void updateScreenParams(ScreenParams screenParams)
	/*   98:     */
	   {	/*   99: 416 */		 if ((screenParams == null) || (screenParams.Equals(this.mHmd.Screen)))
		 {
	/*  100: 417 */		   return;
	/*  101:     */
		 }	/*  102: 420 */		 this.mHmd.Screen = screenParams;
	/*  103: 422 */		 if (this.mRendererHelper != null)
		 {
	/*  104: 423 */		   this.mRendererHelper.ScreenParams = screenParams;
	/*  105:     */
		 }	/*  106:     */
	   }	/*  107:     */   
	/*  108:     */
	   public virtual ScreenParams ScreenParams
	   {
		   get
		/*  109:     */
		   {		/*  110: 440 */			 return this.mHmd.Screen;
		/*  111:     */
		   }		   set
		/*  346:     */
		   {		/*  347: 758 */	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final ScreenParams screenParams = new ScreenParams(value);
			   ScreenParams screenParams = new ScreenParams(value);
		/*  348: 759 */			   outerInstance.queueEvent(new RunnableAnonymousInnerClassHelper3(this, screenParams));
		/*  356:     */
		   }
	   }	/*  112:     */   
	/*  113:     */
	   public virtual float InterpupillaryDistance
	   {
		   set
		/*  114:     */
		   {		/*  115: 453 */			 this.mHmd.Cardboard.InterpupillaryDistance = value;
		/*  116: 455 */			 if (this.mRendererHelper != null)
			 {
		/*  117: 456 */			   this.mRendererHelper.InterpupillaryDistance = value;
		/*  118:     */
			 }		/*  119:     */
		   }		   get
		/*  122:     */
		   {		/*  123: 466 */			 return this.mHmd.Cardboard.InterpupillaryDistance;
		/*  124:     */
		   }
	   }	/*  125:     */   
	/*  126:     */
	   public virtual float FovY
	   {
		   set
		/*  127:     */
		   {		/*  128: 487 */			 this.mHmd.Cardboard.FovY = value;
		/*  129: 489 */			 if (this.mRendererHelper != null)
			 {
		/*  130: 490 */			   this.mRendererHelper.FOV = value;
		/*  131:     */
			 }		/*  132:     */
		   }		   get
		/*  135:     */
		   {		/*  136: 508 */			 return this.mHmd.Cardboard.FovY;
		/*  137:     */
		   }
	   }	/*  138:     */   
	/*  139:     */	   public virtual void setZPlanes(float zNear, float zFar)
	/*  140:     */
	   {	/*  141: 521 */		 this.mZNear = zNear;
	/*  142: 522 */		 this.mZFar = zFar;
	/*  143: 524 */		 if (this.mRendererHelper != null)
		 {
	/*  144: 525 */		   this.mRendererHelper.setZPlanes(zNear, zFar);
	/*  145:     */
		 }	/*  146:     */
	   }	/*  147:     */   
	/*  148:     */
	   public virtual float ZNear
	   {
		   get
		/*  149:     */
		   {		/*  150: 535 */			 return this.mZNear;
		/*  151:     */
		   }
	   }	/*  152:     */   
	/*  153:     */
	   public virtual float ZFar
	   {
		   get
		/*  154:     */
		   {		/*  155: 544 */			 return this.mZFar;
		/*  156:     */
		   }
	   }	/*  157:     */   
	/*  158:     */
	   public virtual bool DistortionCorrectionEnabled
	   {
		   set
		/*  159:     */
		   {		/*  160: 555 */			 this.mDistortionCorrectionEnabled = value;
		/*  161: 557 */			 if (this.mRendererHelper != null)
			 {
		/*  162: 558 */			   this.mRendererHelper.DistortionCorrectionEnabled = value;
		/*  163:     */
			 }		/*  164:     */
		   }		   get
		/*  167:     */
		   {		/*  168: 568 */			 return this.mDistortionCorrectionEnabled;
		/*  169:     */
		   }
	   }	/*  170:     */   
	/*  171:     */
	   public virtual float DistortionCorrectionScale
	   {
		   set
		/*  172:     */
		   {		/*  173: 591 */			 this.mDistortionCorrectionScale = value;
		/*  174: 593 */			 if (this.mRendererHelper != null)
			 {
		/*  175: 594 */			   this.mRendererHelper.DistortionCorrectionScale = value;
		/*  176:     */
			 }		/*  177:     */
		   }		   get
		/*  180:     */
		   {		/*  181: 614 */			 return this.mDistortionCorrectionScale;
		/*  182:     */
		   }
	   }	/*  183:     */   
	/*  184:     */	   public virtual void onResume()
	/*  185:     */
	   {	/*  186: 624 */		 if (this.mRendererHelper == null)
		 {
	/*  187: 625 */		   return;
	/*  188:     */
		 }	/*  189: 628 */		 base.onResume();
	/*  190: 629 */		 this.mHeadTracker.startTracking();
	/*  191:     */
	   }	/*  192:     */   
	/*  193:     */	   public virtual void onPause()
	/*  194:     */
	   {	/*  195: 638 */		 if (this.mRendererHelper == null)
		 {
	/*  196: 639 */		   return;
	/*  197:     */
		 }	/*  198: 642 */		 base.onPause();
	/*  199: 643 */		 this.mHeadTracker.stopTracking();
	/*  200:     */
	   }	/*  201:     */   
	/*  202:     */
	   public virtual GLSurfaceView.Renderer Renderer
	   {
		   set
		/*  203:     */
		   {		/*  204: 649 */			 throw new Exception("Please use the CardboardView renderer interfaces");
		/*  205:     */
		   }
	   }	/*  206:     */   
	/*  207:     */	   public virtual void onDetachedFromWindow()
	/*  208:     */
	   {	/*  209: 655 */		 if (this.mRendererHelper != null)
		 {
	/*  210: 656 */		   lock (this.mRendererHelper)
	/*  211:     */
		   {	/*  212: 657 */			 this.mRendererHelper.shutdown();
	/*  213:     */			 try
	/*  214:     */
			 {	/*  215: 659 */			   Monitor.Wait(this.mRendererHelper);
	/*  216:     */
			 }	/*  217:     */			 catch (InterruptedException e)
	/*  218:     */
			 {	/*  219: 661 */			   Log.e("CardboardView", "Interrupted during shutdown: " + e.ToString());
	/*  220:     */
			 }	/*  221:     */
		   }	/*  222:     */
		 }	/*  223: 666 */		 base.onDetachedFromWindow();
	/*  224:     */
	   }	/*  225:     */   
	/*  226:     */	   private void init(Context context)
	/*  227:     */
	   {	/*  228: 672 */		 EGLContextClientVersion = 2;
	/*  229: 673 */		 PreserveEGLContextOnPause = true;
	/*  230:     */     
	/*  231: 675 */		 WindowManager windowManager = (WindowManager)context.getSystemService("window");
	/*  232:     */     
	/*  233:     */ 
	/*  234: 678 */		 this.mHeadTracker = new HeadTracker(context);
	/*  235: 679 */		 this.mHmd = new HeadMountedDisplay(windowManager.DefaultDisplay);
	/*  236:     */
	   }	/*  237:     */   
	/*  238:     */	   public abstract interface Renderer
	/*  239:     */
	   {	/*  240:     */		 void onDrawFrame(HeadTransform paramHeadTransform, EyeParams paramEyeParams1, EyeParams paramEyeParams2);
	/*  241:     */     
	/*  242:     */		 void onFinishFrame(Viewport paramViewport);
	/*  243:     */     
	/*  244:     */		 void onSurfaceChanged(int paramInt1, int paramInt2);
	/*  245:     */     
	/*  246:     */		 void onSurfaceCreated(EGLConfig paramEGLConfig);
	/*  247:     */     
	/*  248:     */		 void onRendererShutdown();
	/*  249:     */
	   }	/*  250:     */   
	/*  251:     */	   public abstract interface StereoRenderer
	/*  252:     */
	   {	/*  253:     */		 void onNewFrame(HeadTransform paramHeadTransform);
	/*  254:     */     
	/*  255:     */		 void onDrawEye(EyeTransform paramEyeTransform);
	/*  256:     */     
	/*  257:     */		 void onFinishFrame(Viewport paramViewport);
	/*  258:     */     
	/*  259:     */		 void onSurfaceChanged(int paramInt1, int paramInt2);
	/*  260:     */     
	/*  261:     */		 void onSurfaceCreated(EGLConfig paramEGLConfig);
	/*  262:     */     
	/*  263:     */		 void onRendererShutdown();
	/*  264:     */
	   }	/*  265:     */   
	/*  266:     */	   public abstract interface CardboardDeviceParamsObserver
	/*  267:     */
	   {	/*  268:     */		 void onCardboardDeviceParamsUpdate(CardboardDeviceParams paramCardboardDeviceParams);
	/*  269:     */
	   }	/*  270:     */   
	/*  271:     */	   private class RendererHelper : GLSurfaceView.Renderer
	/*  272:     */
	/*  273:     */
	   {		   private readonly CardboardView outerInstance;

	/*  274:     */		 internal readonly HeadTransform mHeadTransform;
	/*  275:     */		 internal readonly EyeParams mMonocular;
	/*  276:     */		 internal readonly EyeParams mLeftEye;
	/*  277:     */		 internal readonly EyeParams mRightEye;
	/*  278:     */		 internal readonly float[] mLeftEyeTranslate;
	/*  279:     */		 internal readonly float[] mRightEyeTranslate;
	/*  280:     */		 internal readonly CardboardView.Renderer mRenderer;
	/*  281:     */		 internal bool mShuttingDown;
	/*  282:     */		 internal HeadMountedDisplay mHmd;
	/*  283:     */		 internal bool mVRMode;
	/*  284:     */		 internal bool mDistortionCorrectionEnabled;
	/*  285:     */		 internal float mDistortionCorrectionScale;
	/*  286:     */		 internal float mZNear;
	/*  287:     */		 internal float mZFar;
	/*  288:     */		 internal bool mProjectionChanged;
	/*  289:     */		 internal bool mInvalidSurfaceSize;
	/*  290:     */     
	/*  291:     */		 public RendererHelper(CardboardView outerInstance, CardboardView.Renderer renderer)
	/*  292:     */
		 {			 this.outerInstance = outerInstance;
	/*  293: 710 */		   this.mRenderer = renderer;
	/*  294: 711 */		   this.mHmd = new HeadMountedDisplay(outerInstance.mHmd);
	/*  295: 712 */		   this.mHeadTransform = new HeadTransform();
	/*  296: 713 */		   this.mMonocular = new EyeParams(0);
	/*  297: 714 */		   this.mLeftEye = new EyeParams(1);
	/*  298: 715 */		   this.mRightEye = new EyeParams(2);
	/*  299: 716 */		   updateFieldOfView(this.mLeftEye.Fov, this.mRightEye.Fov);
	/*  300: 717 */		   outerInstance.mDistortionRenderer = new DistortionRenderer();
	/*  301:     */       
	/*  302: 719 */		   this.mLeftEyeTranslate = new float[16];
	/*  303: 720 */		   this.mRightEyeTranslate = new float[16];
	/*  304:     */       
	/*  305:     */ 
	/*  306: 723 */		   this.mVRMode = outerInstance.mVRMode;
	/*  307: 724 */		   this.mDistortionCorrectionEnabled = outerInstance.mDistortionCorrectionEnabled;
	/*  308: 725 */		   this.mDistortionCorrectionScale = outerInstance.mDistortionCorrectionScale;
	/*  309: 726 */		   this.mZNear = outerInstance.mZNear;
	/*  310: 727 */		   this.mZFar = outerInstance.mZFar;
	/*  311:     */       
	/*  312:     */ 
	/*  313: 730 */		   this.mProjectionChanged = true;
	/*  314:     */
		 }	/*  315:     */     
	/*  316:     */		 public virtual void shutdown()
	/*  317:     */
		 {	/*  318: 734 */		   outerInstance.queueEvent(new RunnableAnonymousInnerClassHelper(this));
	/*  330:     */
		 }
		 private class RunnableAnonymousInnerClassHelper : Runnable
		 {
			 private readonly RendererHelper outerInstance;

			 public RunnableAnonymousInnerClassHelper(RendererHelper outerInstance)
			 {
				 this.outerInstance = outerInstance;
			 }

		 /*  319:     */		 /*  320:     */			 public virtual void run()
		 /*  321:     */
			 {		 /*  322: 737 */			   lock (outerInstance.outerInstance)
		 /*  323:     */
			   {		 /*  324: 738 */				 outerInstance.outerInstance.mShuttingDown = true;
		 /*  325: 739 */				 outerInstance.outerInstance.mRenderer.onRendererShutdown();
		 /*  326: 740 */				 Monitor.PulseAll(outerInstance.outerInstance);
		 /*  327:     */
			   }		 /*  328:     */
			 }		 /*  329:     */
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

		 /*  336:     */		 /*  337:     */			 public virtual void run()
		 /*  338:     */
			 {		 /*  339: 751 */			   outerInstance.outerInstance.mHmd.Cardboard = deviceParams;
		 /*  340: 752 */			   outerInstance.outerInstance.mProjectionChanged = true;
		 /*  341:     */
			 }		 /*  342:     */
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

		 /*  349:     */		 /*  350:     */			 public virtual void run()
		 /*  351:     */
			 {		 /*  352: 762 */			   outerInstance.outerInstance.mHmd.Screen = screenParams;
		 /*  353: 763 */			   outerInstance.outerInstance.mProjectionChanged = true;
		 /*  354:     */
			 }		 /*  355:     */
		 }	/*  357:     */     
	/*  358:     *///JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setInterpupillaryDistance(final float interpupillaryDistance)
		 public virtual float InterpupillaryDistance
		 {
			 set
		/*  359:     */
			 {		/*  360: 769 */			   outerInstance.queueEvent(new RunnableAnonymousInnerClassHelper4(this, value));
		/*  368:     */
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

		 /*  361:     */		 /*  362:     */			 public virtual void run()
		 /*  363:     */
			 {		 /*  364: 772 */			   outerInstance.outerInstance.mHmd.Cardboard.InterpupillaryDistance = interpupillaryDistance;
		 /*  365: 773 */			   outerInstance.outerInstance.mProjectionChanged = true;
		 /*  366:     */
			 }		 /*  367:     */
		 }	/*  369:     */     
	/*  370:     *///JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setFOV(final float fovY)
		 public virtual float FOV
		 {
			 set
		/*  371:     */
			 {		/*  372: 779 */			   outerInstance.queueEvent(new RunnableAnonymousInnerClassHelper5(this, value));
		/*  380:     */
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

		 /*  373:     */		 /*  374:     */			 public virtual void run()
		 /*  375:     */
			 {		 /*  376: 782 */			   outerInstance.outerInstance.mHmd.Cardboard.FovY = fovY;
		 /*  377: 783 */			   outerInstance.outerInstance.mProjectionChanged = true;
		 /*  378:     */
			 }		 /*  379:     */
		 }	/*  381:     */     
	/*  382:     *///JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setZPlanes(final float zNear, final float zFar)
		 public virtual void setZPlanes(float zNear, float zFar)
	/*  383:     */
		 {	/*  384: 789 */		   outerInstance.queueEvent(new RunnableAnonymousInnerClassHelper6(this, zNear, zFar));
	/*  393:     */
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

		 /*  385:     */		 /*  386:     */			 public virtual void run()
		 /*  387:     */
			 {		 /*  388: 792 */			   outerInstance.outerInstance.mZNear = zNear;
		 /*  389: 793 */			   outerInstance.outerInstance.mZFar = zFar;
		 /*  390: 794 */			   outerInstance.outerInstance.mProjectionChanged = true;
		 /*  391:     */
			 }		 /*  392:     */
		 }	/*  394:     */     
	/*  395:     *///JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setDistortionCorrectionEnabled(final boolean enabled)
		 public virtual bool DistortionCorrectionEnabled
		 {
			 set
		/*  396:     */
			 {		/*  397: 800 */			   outerInstance.queueEvent(new RunnableAnonymousInnerClassHelper7(this, value));
		/*  405:     */
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

		 /*  398:     */		 /*  399:     */			 public virtual void run()
		 /*  400:     */
			 {		 /*  401: 803 */			   outerInstance.outerInstance.mDistortionCorrectionEnabled = enabled;
		 /*  402: 804 */			   outerInstance.outerInstance.mProjectionChanged = true;
		 /*  403:     */
			 }		 /*  404:     */
		 }	/*  406:     */     
	/*  407:     *///JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setDistortionCorrectionScale(final float scale)
		 public virtual float DistortionCorrectionScale
		 {
			 set
		/*  408:     */
			 {		/*  409: 810 */			   outerInstance.queueEvent(new RunnableAnonymousInnerClassHelper8(this, value));
		/*  417:     */
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

		 /*  410:     */		 /*  411:     */			 public virtual void run()
		 /*  412:     */
			 {		 /*  413: 813 */			   outerInstance.outerInstance.mDistortionCorrectionScale = scale;
		 /*  414: 814 */			   outerInstance.outerInstance.mDistortionRenderer.ResolutionScale = scale;
		 /*  415:     */
			 }		 /*  416:     */
		 }	/*  418:     */     
	/*  419:     *///JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setVRModeEnabled(final boolean enabled)
		 public virtual bool VRModeEnabled
		 {
			 set
		/*  420:     */
			 {		/*  421: 820 */			   outerInstance.queueEvent(new RunnableAnonymousInnerClassHelper9(this, value));
		/*  438:     */
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

		 /*  422:     */		 /*  423:     */			 public virtual void run()
		 /*  424:     */
			 {		 /*  425: 823 */			   if (outerInstance.outerInstance.mVRMode == enabled)
			   {
		 /*  426: 824 */				 return;
		 /*  427:     */
			   }		 /*  428: 827 */			   outerInstance.outerInstance.mVRMode = enabled;
		 /*  429: 830 */			   if ((outerInstance.outerInstance.mRenderer is CardboardView.StereoRendererHelper))
		 /*  430:     */
			   {		 /*  431: 831 */				 CardboardView.StereoRendererHelper stereoHelper = (CardboardView.StereoRendererHelper)CardboardView.RendererHelper.this.mRenderer;
		 /*  432: 832 */				 stereoHelper.VRModeEnabled = enabled;
		 /*  433:     */
			   }		 /*  434: 837 */			   outerInstance.outerInstance.mProjectionChanged = true;
		 /*  435: 838 */			   outerInstance.outerInstance.onSurfaceChanged((GL10)null, outerInstance.outerInstance.mHmd.Screen.Width, outerInstance.outerInstance.mHmd.Screen.Height);
		 /*  436:     */
			 }		 /*  437:     */
		 }	/*  439:     */     
	/*  440:     */		 public virtual void onDrawFrame(GL10 gl)
	/*  441:     */
		 {	/*  442: 847 */		   if ((this.mShuttingDown) || (this.mInvalidSurfaceSize))
		   {
	/*  443: 848 */			 return;
	/*  444:     */
		   }	/*  445: 851 */		   ScreenParams screen = this.mHmd.Screen;
	/*  446: 852 */		   CardboardDeviceParams cdp = this.mHmd.Cardboard;
	/*  447:     */       
	/*  448:     */ 
	/*  449: 855 */		   outerInstance.mHeadTracker.getLastHeadView(this.mHeadTransform.HeadView, 0);
	/*  450:     */       
	/*  451:     */ 
	/*  452: 858 */		   float halfInterpupillaryDistance = cdp.InterpupillaryDistance * 0.5F;
	/*  453: 861 */		   if (this.mVRMode)
	/*  454:     */
		   {	/*  455: 863 */			 Matrix.setIdentityM(this.mLeftEyeTranslate, 0);
	/*  456: 864 */			 Matrix.setIdentityM(this.mRightEyeTranslate, 0);
	/*  457:     */         
	/*  458: 866 */			 Matrix.translateM(this.mLeftEyeTranslate, 0, halfInterpupillaryDistance, 0.0F, 0.0F);
	/*  459:     */         
	/*  460: 868 */			 Matrix.translateM(this.mRightEyeTranslate, 0, -halfInterpupillaryDistance, 0.0F, 0.0F);
	/*  461:     */         
	/*  462:     */ 
	/*  463:     */ 
	/*  464: 872 */			 Matrix.multiplyMM(this.mLeftEye.Transform.EyeView, 0, this.mLeftEyeTranslate, 0, this.mHeadTransform.HeadView, 0);
	/*  465:     */         
	/*  466:     */ 
	/*  467: 875 */			 Matrix.multiplyMM(this.mRightEye.Transform.EyeView, 0, this.mRightEyeTranslate, 0, this.mHeadTransform.HeadView, 0);
	/*  468:     */
		   }	/*  469:     */		   else
	/*  470:     */
		   {	/*  471: 881 */			 Array.Copy(this.mHeadTransform.HeadView, 0, this.mMonocular.Transform.EyeView, 0, this.mHeadTransform.HeadView.length);
	/*  472:     */
		   }	/*  473: 887 */		   if (this.mProjectionChanged)
	/*  474:     */
		   {	/*  475: 890 */			 this.mMonocular.Viewport.setViewport(0, 0, screen.Width, screen.Height);
	/*  476: 892 */			 if (!this.mVRMode)
	/*  477:     */
			 {	/*  478: 894 */			   float aspectRatio = screen.Width / screen.Height;
	/*  479: 895 */			   Matrix.perspectiveM(this.mMonocular.Transform.Perspective, 0, cdp.FovY, aspectRatio, this.mZNear, this.mZFar);
	/*  480:     */
			 }	/*  481: 897 */			 else if (this.mDistortionCorrectionEnabled)
	/*  482:     */
			 {	/*  483: 898 */			   updateFieldOfView(this.mLeftEye.Fov, this.mRightEye.Fov);
	/*  484: 899 */			   outerInstance.mDistortionRenderer.onProjectionChanged(this.mHmd, this.mLeftEye, this.mRightEye, this.mZNear, this.mZFar);
	/*  485:     */
			 }	/*  486:     */			 else
	/*  487:     */
			 {	/*  488: 921 */			   float distEyeToScreen = cdp.VisibleViewportSize / 2.0F / (float)Math.Tan(Math.toRadians(cdp.FovY) / 2.0D);
	/*  489:     */           
	/*  490:     */ 
	/*  491:     */ 
	/*  492: 925 */			   float left = screen.WidthMeters / 2.0F - halfInterpupillaryDistance;
	/*  493: 926 */			   float right = halfInterpupillaryDistance;
	/*  494: 927 */			   float bottom = cdp.VerticalDistanceToLensCenter - screen.BorderSizeMeters;
	/*  495:     */           
	/*  496: 929 */			   float top = screen.BorderSizeMeters + screen.HeightMeters - cdp.VerticalDistanceToLensCenter;
	/*  497:     */           
	/*  498:     */ 
	/*  499: 932 */			   FieldOfView leftEyeFov = this.mLeftEye.Fov;
	/*  500: 933 */			   leftEyeFov.Left = (float)Math.toDegrees(Math.Atan2(left, distEyeToScreen));
	/*  501:     */           
	/*  502: 935 */			   leftEyeFov.Right = (float)Math.toDegrees(Math.Atan2(right, distEyeToScreen));
	/*  503:     */           
	/*  504: 937 */			   leftEyeFov.Bottom = (float)Math.toDegrees(Math.Atan2(bottom, distEyeToScreen));
	/*  505:     */           
	/*  506: 939 */			   leftEyeFov.Top = (float)Math.toDegrees(Math.Atan2(top, distEyeToScreen));
	/*  507:     */           
	/*  508:     */ 
	/*  509:     */ 
	/*  510: 943 */			   FieldOfView rightEyeFov = this.mRightEye.Fov;
	/*  511: 944 */			   rightEyeFov.Left = leftEyeFov.Right;
	/*  512: 945 */			   rightEyeFov.Right = leftEyeFov.Left;
	/*  513: 946 */			   rightEyeFov.Bottom = leftEyeFov.Bottom;
	/*  514: 947 */			   rightEyeFov.Top = leftEyeFov.Top;
	/*  515:     */           
	/*  516: 949 */			   leftEyeFov.toPerspectiveMatrix(this.mZNear, this.mZFar, this.mLeftEye.Transform.Perspective, 0);
	/*  517:     */           
	/*  518: 951 */			   rightEyeFov.toPerspectiveMatrix(this.mZNear, this.mZFar, this.mRightEye.Transform.Perspective, 0);
	/*  519:     */           
	/*  520:     */ 
	/*  521: 954 */			   this.mLeftEye.Viewport.setViewport(0, 0, screen.Width / 2, screen.Height);
	/*  522:     */           
	/*  523: 956 */			   this.mRightEye.Viewport.setViewport(screen.Width / 2, 0, screen.Width / 2, screen.Height);
	/*  524:     */
			 }	/*  525: 960 */			 this.mProjectionChanged = false;
	/*  526:     */
		   }	/*  527: 963 */		   if (this.mVRMode)
	/*  528:     */
		   {	/*  529: 964 */			 if (this.mDistortionCorrectionEnabled)
	/*  530:     */
			 {	/*  531: 965 */			   outerInstance.mDistortionRenderer.beforeDrawFrame();
	/*  532: 967 */			   if (this.mDistortionCorrectionScale == 1.0F)
	/*  533:     */
			   {	/*  534: 968 */				 this.mRenderer.onDrawFrame(this.mHeadTransform, this.mLeftEye, this.mRightEye);
	/*  535:     */
			   }	/*  536:     */			   else
	/*  537:     */
			   {	/*  538: 971 */				 int leftX = this.mLeftEye.Viewport.x;
	/*  539: 972 */				 int leftY = this.mLeftEye.Viewport.y;
	/*  540: 973 */				 int leftWidth = this.mLeftEye.Viewport.width;
	/*  541: 974 */				 int leftHeight = this.mLeftEye.Viewport.height;
	/*  542: 975 */				 int rightX = this.mRightEye.Viewport.x;
	/*  543: 976 */				 int rightY = this.mRightEye.Viewport.y;
	/*  544: 977 */				 int rightWidth = this.mRightEye.Viewport.width;
	/*  545: 978 */				 int rightHeight = this.mRightEye.Viewport.height;
	/*  546:     */             
	/*  547:     */ 
	/*  548: 981 */				 this.mLeftEye.Viewport.setViewport((int)(leftX * this.mDistortionCorrectionScale), (int)(leftY * this.mDistortionCorrectionScale), (int)(leftWidth * this.mDistortionCorrectionScale), (int)(leftHeight * this.mDistortionCorrectionScale));
	/*  549:     */             
	/*  550:     */ 
	/*  551:     */ 
	/*  552:     */ 
	/*  553: 986 */				 this.mRightEye.Viewport.setViewport((int)(rightX * this.mDistortionCorrectionScale), (int)(rightY * this.mDistortionCorrectionScale), (int)(rightWidth * this.mDistortionCorrectionScale), (int)(rightHeight * this.mDistortionCorrectionScale));
	/*  554:     */             
	/*  555:     */ 
	/*  556:     */ 
	/*  557:     */ 
	/*  558:     */ 
	/*  559:     */ 
	/*  560: 993 */				 this.mRenderer.onDrawFrame(this.mHeadTransform, this.mLeftEye, this.mRightEye);
	/*  561:     */             
	/*  562:     */ 
	/*  563: 996 */				 this.mLeftEye.Viewport.setViewport(leftX, leftY, leftWidth, leftHeight);
	/*  564:     */             
	/*  565: 998 */				 this.mRightEye.Viewport.setViewport(rightX, rightY, rightWidth, rightHeight);
	/*  566:     */
			   }	/*  567:1002 */			   outerInstance.mDistortionRenderer.afterDrawFrame();
	/*  568:     */
			 }	/*  569:     */			 else
	/*  570:     */
			 {	/*  571:1004 */			   this.mRenderer.onDrawFrame(this.mHeadTransform, this.mLeftEye, this.mRightEye);
	/*  572:     */
			 }	/*  573:     */
		   }	/*  574:     */		   else
		   {
	/*  575:1007 */			 this.mRenderer.onDrawFrame(this.mHeadTransform, this.mMonocular, null);
	/*  576:     */
		   }	/*  577:1010 */		   this.mRenderer.onFinishFrame(this.mMonocular.Viewport);
	/*  578:     */
		 }	/*  579:     */     
	/*  580:     */		 public virtual void onSurfaceChanged(GL10 gl, int width, int height)
	/*  581:     */
		 {	/*  582:1015 */		   if (this.mShuttingDown)
		   {
	/*  583:1016 */			 return;
	/*  584:     */
		   }	/*  585:1020 */		   ScreenParams screen = this.mHmd.Screen;
	/*  586:1021 */		   if ((width != screen.Width) || (height != screen.Height))
	/*  587:     */
		   {	/*  588:1022 */			 if (!this.mInvalidSurfaceSize)
	/*  589:     */
			 {	/*  590:1023 */			   GLES20.glClear(16384);
	/*  591:1024 */			   Log.w("CardboardView", "Surface size " + width + "x" + height + " does not match the expected screen size " + screen.Width + "x" + screen.Height + ". Rendering is disabled.");
	/*  592:     */
			 }	/*  593:1030 */			 this.mInvalidSurfaceSize = true;
	/*  594:     */
		   }	/*  595:     */		   else
	/*  596:     */
		   {	/*  597:1032 */			 this.mInvalidSurfaceSize = false;
	/*  598:     */
		   }	/*  599:1037 */		   this.mRenderer.onSurfaceChanged(width, height);
	/*  600:     */
		 }	/*  601:     */     
	/*  602:     */		 public virtual void onSurfaceCreated(GL10 gl, EGLConfig config)
	/*  603:     */
		 {	/*  604:1042 */		   if (this.mShuttingDown)
		   {
	/*  605:1043 */			 return;
	/*  606:     */
		   }	/*  607:1046 */		   this.mRenderer.onSurfaceCreated(config);
	/*  608:     */
		 }	/*  609:     */     
	/*  610:     */		 internal virtual void updateFieldOfView(FieldOfView leftEyeFov, FieldOfView rightEyeFov)
	/*  611:     */
		 {	/*  612:1050 */		   CardboardDeviceParams cdp = this.mHmd.Cardboard;
	/*  613:1051 */		   ScreenParams screen = this.mHmd.Screen;
	/*  614:1052 */		   Distortion distortion = cdp.Distortion;
	/*  615:     */       
	/*  616:     */ 
	/*  617:     */ 
	/*  618:     */ 
	/*  619:     */ 
	/*  620:     */ 
	/*  621:1059 */		   float idealFovAngle = (float)Math.toDegrees(Math.Atan2(cdp.LensDiameter / 2.0F, cdp.EyeToLensDistance));
	/*  622:     */       
	/*  623:     */ 
	/*  624:     */ 
	/*  625:     */ 
	/*  626:     */ 
	/*  627:     */ 
	/*  628:     */ 
	/*  629:     */ 
	/*  630:     */ 
	/*  631:     */ 
	/*  632:     */ 
	/*  633:1071 */		   float eyeToScreenDist = cdp.EyeToLensDistance + cdp.ScreenToLensDistance;
	/*  634:     */       
	/*  635:     */ 
	/*  636:1074 */		   float outerDist = (screen.WidthMeters - cdp.InterpupillaryDistance) / 2.0F;
	/*  637:     */       
	/*  638:1076 */		   float innerDist = cdp.InterpupillaryDistance / 2.0F;
	/*  639:1077 */		   float bottomDist = cdp.VerticalDistanceToLensCenter - screen.BorderSizeMeters;
	/*  640:     */       
	/*  641:1079 */		   float topDist = screen.HeightMeters + screen.BorderSizeMeters - cdp.VerticalDistanceToLensCenter;
	/*  642:     */       
	/*  643:     */ 
	/*  644:     */ 
	/*  645:1083 */		   float outerAngle = (float)Math.toDegrees(Math.Atan2(distortion.distort(outerDist), eyeToScreenDist));
	/*  646:     */       
	/*  647:1085 */		   float innerAngle = (float)Math.toDegrees(Math.Atan2(distortion.distort(innerDist), eyeToScreenDist));
	/*  648:     */       
	/*  649:1087 */		   float bottomAngle = (float)Math.toDegrees(Math.Atan2(distortion.distort(bottomDist), eyeToScreenDist));
	/*  650:     */       
	/*  651:1089 */		   float topAngle = (float)Math.toDegrees(Math.Atan2(distortion.distort(topDist), eyeToScreenDist));
	/*  652:     */       
	/*  653:     */ 
	/*  654:1092 */		   leftEyeFov.Left = Math.Min(outerAngle, idealFovAngle);
	/*  655:1093 */		   leftEyeFov.Right = Math.Min(innerAngle, idealFovAngle);
	/*  656:1094 */		   leftEyeFov.Bottom = Math.Min(bottomAngle, idealFovAngle);
	/*  657:1095 */		   leftEyeFov.Top = Math.Min(topAngle, idealFovAngle);
	/*  658:     */       
	/*  659:1097 */		   rightEyeFov.Left = Math.Min(innerAngle, idealFovAngle);
	/*  660:1098 */		   rightEyeFov.Right = Math.Min(outerAngle, idealFovAngle);
	/*  661:1099 */		   rightEyeFov.Bottom = Math.Min(bottomAngle, idealFovAngle);
	/*  662:1100 */		   rightEyeFov.Top = Math.Min(topAngle, idealFovAngle);
	/*  663:     */
		 }	/*  664:     */
	   }	/*  665:     */   
	/*  666:     */	   private class StereoRendererHelper : CardboardView.Renderer
	/*  667:     */
	/*  668:     */
	   {		   private readonly CardboardView outerInstance;

	/*  669:     */		 internal readonly CardboardView.StereoRenderer mStereoRenderer;
	/*  670:     */		 internal bool mVRMode;
	/*  671:     */     
	/*  672:     */		 public StereoRendererHelper(CardboardView outerInstance, CardboardView.StereoRenderer stereoRenderer)
	/*  673:     */
		 {			 this.outerInstance = outerInstance;
	/*  674:1114 */		   this.mStereoRenderer = stereoRenderer;
	/*  675:1115 */		   this.mVRMode = outerInstance.mVRMode;
	/*  676:     */
		 }	/*  677:     */     
	/*  678:     *///JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setVRModeEnabled(final boolean enabled)
		 public virtual bool VRModeEnabled
		 {
			 set
		/*  679:     */
			 {		/*  680:1119 */			   outerInstance.queueEvent(new RunnableAnonymousInnerClassHelper(this, value));
		/*  687:     */
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

		 /*  681:     */		 /*  682:     */			 public virtual void run()
		 /*  683:     */
			 {		 /*  684:1122 */			   outerInstance.outerInstance.mVRMode = enabled;
		 /*  685:     */
			 }		 /*  686:     */
		 }	/*  688:     */     
	/*  689:     */		 public virtual void onDrawFrame(HeadTransform head, EyeParams leftEye, EyeParams rightEye)
	/*  690:     */
		 {	/*  691:1129 */		   this.mStereoRenderer.onNewFrame(head);
	/*  692:1130 */		   GLES20.glEnable(3089);
	/*  693:     */       
	/*  694:1132 */		   leftEye.Viewport.setGLViewport();
	/*  695:1133 */		   leftEye.Viewport.setGLScissor();
	/*  696:1134 */		   this.mStereoRenderer.onDrawEye(leftEye.Transform);
	/*  697:1137 */		   if (rightEye == null)
		   {
	/*  698:1138 */			 return;
	/*  699:     */
		   }	/*  700:1141 */		   rightEye.Viewport.setGLViewport();
	/*  701:1142 */		   rightEye.Viewport.setGLScissor();
	/*  702:1143 */		   this.mStereoRenderer.onDrawEye(rightEye.Transform);
	/*  703:     */
		 }	/*  704:     */     
	/*  705:     */		 public virtual void onFinishFrame(Viewport viewport)
	/*  706:     */
		 {	/*  707:1148 */		   viewport.setGLViewport();
	/*  708:1149 */		   viewport.setGLScissor();
	/*  709:1150 */		   this.mStereoRenderer.onFinishFrame(viewport);
	/*  710:     */
		 }	/*  711:     */     
	/*  712:     */		 public virtual void onSurfaceChanged(int width, int height)
	/*  713:     */
		 {	/*  714:1155 */		   if (this.mVRMode)
		   {
	/*  715:1158 */			 this.mStereoRenderer.onSurfaceChanged(width / 2, height);
	/*  716:     */
		   }		   else
		   {
	/*  717:1160 */			 this.mStereoRenderer.onSurfaceChanged(width, height);
	/*  718:     */
		   }	/*  719:     */
		 }	/*  720:     */     
	/*  721:     */		 public virtual void onSurfaceCreated(EGLConfig config)
	/*  722:     */
		 {	/*  723:1166 */		   this.mStereoRenderer.onSurfaceCreated(config);
	/*  724:     */
		 }	/*  725:     */     
	/*  726:     */		 public virtual void onRendererShutdown()
	/*  727:     */
		 {	/*  728:1171 */		   this.mStereoRenderer.onRendererShutdown();
	/*  729:     */
		 }	/*  730:     */
	   }	/*  731:     */
	 }

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     com.google.vrtoolkit.cardboard.CardboardView
	 * JD-Core Version:    0.7.0.1
	 */
 }