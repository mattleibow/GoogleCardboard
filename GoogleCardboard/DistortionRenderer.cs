using System;

 namespace com.google.vrtoolkit.cardboard
 {
	 
		 using GLES20 = android.opengl.GLES20;
		 using Log = android.util.Log;
					 
		 public class DistortionRenderer
	
	 {		   private const string TAG = "DistortionRenderer";
		   private int mTextureId;
		   private int mRenderbufferId;
		   private int mFramebufferId;
		   private IntBuffer mOriginalFramebufferId;
		   private IntBuffer mCullFaceEnabled;
		   private IntBuffer mScissorTestEnabled;
		   private IntBuffer mViewport;
		   private float mResolutionScale;
		   private DistortionMesh mLeftEyeDistortionMesh;
		   private DistortionMesh mRightEyeDistortionMesh;
		   private HeadMountedDisplay mHmd;
		   private FieldOfView mLeftEyeFov;
		   private FieldOfView mRightEyeFov;
		   private ProgramHolder mProgramHolder;
		   private readonly string VERTEX_SHADER = "attribute vec2 aPosition;\nattribute float aVignette;\nattribute vec2 aTextureCoord;\nvarying vec2 vTextureCoord;\nvarying float vVignette;\nuniform float uTextureCoordScale;\nvoid main() {\n    gl_Position = vec4(aPosition, 0.0, 1.0);\n    vTextureCoord = aTextureCoord.xy * uTextureCoordScale;\n    vVignette = aVignette;\n}\n";
		   private readonly string FRAGMENT_SHADER = "precision mediump float;\nvarying vec2 vTextureCoord;\nvarying float vVignette;\nuniform sampler2D uTextureSampler;\nvoid main() {\n    gl_FragColor = vVignette * texture2D(uTextureSampler, vTextureCoord);\n}\n";
	   
		   public DistortionRenderer()
	
	   {			 this.mTextureId = -1;
			 this.mRenderbufferId = -1;
			 this.mFramebufferId = -1;
			 this.mOriginalFramebufferId = IntBuffer.allocate(1);
			 this.mCullFaceEnabled = IntBuffer.allocate(1);
			 this.mScissorTestEnabled = IntBuffer.allocate(1);
			 this.mViewport = IntBuffer.allocate(4);
	     
			 this.mResolutionScale = 1.0F;
	     
	 
	 
	 
	 
	 
	 
	 
	 
	 
			 this.VERTEX_SHADER = "attribute vec2 aPosition;\nattribute float aVignette;\nattribute vec2 aTextureCoord;\nvarying vec2 vTextureCoord;\nvarying float vVignette;\nuniform float uTextureCoordScale;\nvoid main() {\n    gl_Position = vec4(aPosition, 0.0, 1.0);\n    vTextureCoord = aTextureCoord.xy * uTextureCoordScale;\n    vVignette = aVignette;\n}\n";
	     
	 
	 
	 
	 
	 
	 
	 
	 
	 
	 
	 
			 this.FRAGMENT_SHADER = "precision mediump float;\nvarying vec2 vTextureCoord;\nvarying float vVignette;\nuniform sampler2D uTextureSampler;\nvoid main() {\n    gl_FragColor = vVignette * texture2D(uTextureSampler, vTextureCoord);\n}\n";
	
	   }	   
		   private class ProgramHolder
	
	   {		   private readonly DistortionRenderer outerInstance;

			 public int program;
			 public int aPosition;
			 public int aVignette;
			 public int aTextureCoord;
			 public int uTextureCoordScale;
			 public int uTextureSampler;
	     
			 internal ProgramHolder(DistortionRenderer outerInstance)
		 {
			 this.outerInstance = outerInstance;
		 }
	
	   }	   
		   private class EyeViewport
	
	   {		   private readonly DistortionRenderer outerInstance;

			 public float x;
			 public float y;
			 public float width;
			 public float height;
			 public float eyeX;
			 public float eyeY;
	     
			 internal EyeViewport(DistortionRenderer outerInstance)
		 {
			 this.outerInstance = outerInstance;
		 }
	     
			 public override string ToString()
	
		 {			   return "EyeViewport {x:" + this.x + " y:" + this.y + " width:" + this.width + " height:" + this.height + " eyeX: " + this.eyeX + " eyeY: " + this.eyeY + "}";
	
		 }	
	   }	   
		   public virtual void beforeDrawFrame()
	
	   {			 GLES20.glGetIntegerv(36006, this.mOriginalFramebufferId);
			 GLES20.glBindFramebuffer(36160, this.mFramebufferId);
	
	   }	   
		   public virtual void afterDrawFrame()
	
	   {			 GLES20.glBindFramebuffer(36160, this.mOriginalFramebufferId.array()[0]);
			 GLES20.glViewport(0, 0, this.mHmd.Screen.Width, this.mHmd.Screen.Height);
	     
			 GLES20.glGetIntegerv(2978, this.mViewport);
			 GLES20.glGetIntegerv(2884, this.mCullFaceEnabled);
			 GLES20.glGetIntegerv(3089, this.mScissorTestEnabled);
			 GLES20.glDisable(3089);
			 GLES20.glDisable(2884);
	     
			 GLES20.glClearColor(0.0F, 0.0F, 0.0F, 1.0F);
			 GLES20.glClear(16640);
	     
			 GLES20.glUseProgram(this.mProgramHolder.program);
	     
			 GLES20.glEnable(3089);
			 GLES20.glScissor(0, 0, this.mHmd.Screen.Width / 2, this.mHmd.Screen.Height);
	     
	 
	 
			 renderDistortionMesh(this.mLeftEyeDistortionMesh);
	     
			 GLES20.glScissor(this.mHmd.Screen.Width / 2, 0, this.mHmd.Screen.Width / 2, this.mHmd.Screen.Height);
	     
	 
	 
			 renderDistortionMesh(this.mRightEyeDistortionMesh);
	     
			 GLES20.glDisableVertexAttribArray(this.mProgramHolder.aPosition);
			 GLES20.glDisableVertexAttribArray(this.mProgramHolder.aVignette);
			 GLES20.glDisableVertexAttribArray(this.mProgramHolder.aTextureCoord);
			 GLES20.glUseProgram(0);
			 GLES20.glBindBuffer(34962, 0);
			 GLES20.glBindBuffer(34963, 0);
			 GLES20.glDisable(3089);
			 if (this.mCullFaceEnabled.array()[0] == 1)
		 {
			   GLES20.glEnable(2884);
	
		 }			 if (this.mScissorTestEnabled.array()[0] == 1)
		 {
			   GLES20.glEnable(3089);
	
		 }			 GLES20.glViewport(this.mViewport.array()[0], this.mViewport.array()[1], this.mViewport.array()[2], this.mViewport.array()[3]);
	
	   }	   
	
	   public virtual float ResolutionScale
	   {
		   set
		
		   {					 this.mResolutionScale = value;
		
		   }
	   }	   
		   public virtual void onProjectionChanged(HeadMountedDisplay hmd, EyeParams leftEye, EyeParams rightEye, float zNear, float zFar)
	
	   {			 this.mHmd = new HeadMountedDisplay(hmd);
			 this.mLeftEyeFov = new FieldOfView(leftEye.Fov);
			 this.mRightEyeFov = new FieldOfView(rightEye.Fov);
	     
			 ScreenParams screen = this.mHmd.Screen;
			 CardboardDeviceParams cdp = this.mHmd.Cardboard;
			 if (this.mProgramHolder == null)
		 {
			   this.mProgramHolder = createProgramHolder();
	
		 }			 EyeViewport leftEyeViewport = initViewportForEye(leftEye, 0.0F);
			 EyeViewport rightEyeViewport = initViewportForEye(rightEye, leftEyeViewport.width);
	     
			 leftEye.Fov.toPerspectiveMatrix(zNear, zFar, leftEye.Transform.Perspective, 0);
	     
			 rightEye.Fov.toPerspectiveMatrix(zNear, zFar, rightEye.Transform.Perspective, 0);
	     
	 
			 float textureWidthM = leftEyeViewport.width + rightEyeViewport.width;
			 float textureHeightM = Math.Max(leftEyeViewport.height, rightEyeViewport.height);
			 float xPxPerM = screen.Width / screen.WidthMeters;
			 float yPxPerM = screen.Height / screen.HeightMeters;
			 int textureWidthPx = Math.Round(textureWidthM * xPxPerM);
			 int textureHeightPx = Math.Round(textureHeightM * yPxPerM);
	     
			 float xEyeOffsetMScreen = screen.WidthMeters / 2.0F - cdp.InterpupillaryDistance / 2.0F;
			 float yEyeOffsetMScreen = cdp.VerticalDistanceToLensCenter - screen.BorderSizeMeters;
	     
	 
			 this.mLeftEyeDistortionMesh = createDistortionMesh(leftEye, leftEyeViewport, textureWidthM, textureHeightM, xEyeOffsetMScreen, yEyeOffsetMScreen);
	     
	 
	 
			 xEyeOffsetMScreen = screen.WidthMeters - xEyeOffsetMScreen;
			 this.mRightEyeDistortionMesh = createDistortionMesh(rightEye, rightEyeViewport, textureWidthM, textureHeightM, xEyeOffsetMScreen, yEyeOffsetMScreen);
	     
	 
	 
	 
			 setupRenderTextureAndRenderbuffer(textureWidthPx, textureHeightPx);
	
	   }	   
		   private EyeViewport initViewportForEye(EyeParams eye, float xOffsetM)
	
	   {			 ScreenParams screen = this.mHmd.Screen;
			 CardboardDeviceParams cdp = this.mHmd.Cardboard;
	     
			 float eyeToScreenDistanceM = cdp.EyeToLensDistance + cdp.ScreenToLensDistance;
			 float leftM = (float)Math.Tan(Math.toRadians(eye.Fov.Left)) * eyeToScreenDistanceM;
	     
			 float rightM = (float)Math.Tan(Math.toRadians(eye.Fov.Right)) * eyeToScreenDistanceM;
	     
			 float bottomM = (float)Math.Tan(Math.toRadians(eye.Fov.Bottom)) * eyeToScreenDistanceM;
	     
			 float topM = (float)Math.Tan(Math.toRadians(eye.Fov.Top)) * eyeToScreenDistanceM;
	     
	 
			 EyeViewport vp = new EyeViewport(this, null);
			 vp.x = xOffsetM;
			 vp.y = 0.0F;
			 vp.width = (leftM + rightM);
			 vp.height = (bottomM + topM);
			 vp.eyeX = (leftM + xOffsetM);
			 vp.eyeY = bottomM;
	     
			 float xPxPerM = screen.Width / screen.WidthMeters;
			 float yPxPerM = screen.Height / screen.HeightMeters;
			 eye.Viewport.x = Math.Round(vp.x * xPxPerM);
			 eye.Viewport.y = Math.Round(vp.y * xPxPerM);
			 eye.Viewport.width = Math.Round(vp.width * xPxPerM);
			 eye.Viewport.height = Math.Round(vp.height * xPxPerM);
	     
			 return vp;
	
	   }	   
		   private DistortionMesh createDistortionMesh(EyeParams eye, EyeViewport eyeViewport, float textureWidthM, float textureHeightM, float xEyeOffsetMScreen, float yEyeOffsetMScreen)
	
	   {			 return new DistortionMesh(this, eye, this.mHmd.Cardboard.Distortion, this.mHmd.Screen.WidthMeters, this.mHmd.Screen.HeightMeters, xEyeOffsetMScreen, yEyeOffsetMScreen, textureWidthM, textureHeightM, eyeViewport.eyeX, eyeViewport.eyeY, eyeViewport.x, eyeViewport.y, eyeViewport.width, eyeViewport.height);
	
	   }	   
		   private void renderDistortionMesh(DistortionMesh mesh)
	
	   {			 GLES20.glBindBuffer(34962, mesh.mArrayBufferId);
			 mesh.GetType();
		 mesh.GetType();
		 GLES20.glVertexAttribPointer(this.mProgramHolder.aPosition, 3, 5126, false, 20, 0 * 4);
	     
	 
	 
	 
	 
	 
			 GLES20.glEnableVertexAttribArray(this.mProgramHolder.aPosition);
	     
			 mesh.GetType();
		 mesh.GetType();
		 GLES20.glVertexAttribPointer(this.mProgramHolder.aVignette, 1, 5126, false, 20, 2 * 4);
	     
	 
	 
	 
	 
	 
			 GLES20.glEnableVertexAttribArray(this.mProgramHolder.aVignette);
	     
			 mesh.GetType();
		 mesh.GetType();
		 GLES20.glVertexAttribPointer(this.mProgramHolder.aTextureCoord, 2, 5126, false, 20, 3 * 4);
	     
	 
	 
	 
	 
	 
			 GLES20.glEnableVertexAttribArray(this.mProgramHolder.aTextureCoord);
	     
			 GLES20.glActiveTexture(33984);
			 GLES20.glBindTexture(3553, this.mTextureId);
			 GLES20.glUniform1i(this.mProgramHolder.uTextureSampler, 0);
			 GLES20.glUniform1f(this.mProgramHolder.uTextureCoordScale, this.mResolutionScale);
	     
	 
			 GLES20.glBindBuffer(34963, mesh.mElementBufferId);
			 GLES20.glDrawElements(5, mesh.nIndices, 5125, 0);
	
	   }	   
		   private float computeDistortionScale(Distortion distortion, float screenWidthM, float interpupillaryDistanceM)
	
	   {			 return distortion.distortionFactor((screenWidthM / 2.0F - interpupillaryDistanceM / 2.0F) / (screenWidthM / 4.0F));
	
	   }	   
		   private int createTexture(int width, int height)
	
	   {			 int[] textureIds = new int[1];
			 GLES20.glGenTextures(1, textureIds, 0);
	     
			 GLES20.glBindTexture(3553, textureIds[0]);
			 GLES20.glTexParameteri(3553, 10242, 33071);
	     
			 GLES20.glTexParameteri(3553, 10243, 33071);
	     
			 GLES20.glTexParameteri(3553, 10240, 9729);
	     
			 GLES20.glTexParameteri(3553, 10241, 9729);
	     
	 
	 
			 GLES20.glTexImage2D(3553, 0, 6407, width, height, 0, 6407, 33635, null);
	     
	 
	 
	 
	 
	 
	 
	 
	 
	 
			 return textureIds[0];
	
	   }	   
		   private int setupRenderTextureAndRenderbuffer(int width, int height)
	
	   {			 if (this.mTextureId != -1)
		 {
			   GLES20.glDeleteTextures(1, new int[] {this.mTextureId}, 0);
	
		 }			 if (this.mRenderbufferId != -1)
		 {
			   GLES20.glDeleteRenderbuffers(1, new int[] {this.mRenderbufferId}, 0);
	
		 }			 if (this.mFramebufferId != -1)
		 {
			   GLES20.glDeleteFramebuffers(1, new int[] {this.mFramebufferId}, 0);
	
		 }			 this.mTextureId = createTexture(width, height);
			 checkGlError("setupRenderTextureAndRenderbuffer: create texture");
	     
	 
			 int[] renderbufferIds = new int[1];
			 GLES20.glGenRenderbuffers(1, renderbufferIds, 0);
			 GLES20.glBindRenderbuffer(36161, renderbufferIds[0]);
			 GLES20.glRenderbufferStorage(36161, 33189, width, height);
	     
			 this.mRenderbufferId = renderbufferIds[0];
			 checkGlError("setupRenderTextureAndRenderbuffer: create renderbuffer");
	     
			 int[] framebufferIds = new int[1];
			 GLES20.glGenFramebuffers(1, framebufferIds, 0);
			 GLES20.glBindFramebuffer(36160, framebufferIds[0]);
			 this.mFramebufferId = framebufferIds[0];
	     
			 GLES20.glFramebufferTexture2D(36160, 36064, 3553, this.mTextureId, 0);
	     
	 
			 GLES20.glFramebufferRenderbuffer(36160, 36096, 36161, renderbufferIds[0]);
	     
	 
	 
			 int status = GLES20.glCheckFramebufferStatus(36160);
			 if (status != 36053)
		 {
			   throw new Exception("Framebuffer is not complete: " + status.ToString("x"));
	
		 }			 GLES20.glBindFramebuffer(36160, 0);
	     
			 return framebufferIds[0];
	
	   }	   
		   private int loadShader(int shaderType, string source)
	
	   {			 int shader = GLES20.glCreateShader(shaderType);
			 if (shader != 0)
	
		 {			   GLES20.glShaderSource(shader, source);
			   GLES20.glCompileShader(shader);
			   int[] compiled = new int[1];
			   GLES20.glGetShaderiv(shader, 35713, compiled, 0);
			   if (compiled[0] == 0)
	
		   {				 Log.e("DistortionRenderer", "Could not compile shader " + shaderType + ":");
				 Log.e("DistortionRenderer", GLES20.glGetShaderInfoLog(shader));
				 GLES20.glDeleteShader(shader);
				 shader = 0;
	
		   }	
		 }			 return shader;
	
	   }	   
		   private int createProgram(string vertexSource, string fragmentSource)
	
	   {			 int vertexShader = loadShader(35633, vertexSource);
			 if (vertexShader == 0)
		 {
			   return 0;
	
		 }			 int pixelShader = loadShader(35632, fragmentSource);
			 if (pixelShader == 0)
		 {
			   return 0;
	
		 }			 int program = GLES20.glCreateProgram();
			 if (program != 0)
	
		 {			   GLES20.glAttachShader(program, vertexShader);
			   checkGlError("glAttachShader");
			   GLES20.glAttachShader(program, pixelShader);
			   checkGlError("glAttachShader");
			   GLES20.glLinkProgram(program);
			   int[] linkStatus = new int[1];
			   GLES20.glGetProgramiv(program, 35714, linkStatus, 0);
			   if (linkStatus[0] != 1)
	
		   {				 Log.e("DistortionRenderer", "Could not link program: ");
				 Log.e("DistortionRenderer", GLES20.glGetProgramInfoLog(program));
				 GLES20.glDeleteProgram(program);
				 program = 0;
	
		   }	
		 }			 return program;
	
	   }	   
		   private ProgramHolder createProgramHolder()
	
	   {			 ProgramHolder holder = new ProgramHolder(this, null);
			 holder.program = createProgram("attribute vec2 aPosition;\nattribute float aVignette;\nattribute vec2 aTextureCoord;\nvarying vec2 vTextureCoord;\nvarying float vVignette;\nuniform float uTextureCoordScale;\nvoid main() {\n    gl_Position = vec4(aPosition, 0.0, 1.0);\n    vTextureCoord = aTextureCoord.xy * uTextureCoordScale;\n    vVignette = aVignette;\n}\n", "precision mediump float;\nvarying vec2 vTextureCoord;\nvarying float vVignette;\nuniform sampler2D uTextureSampler;\nvoid main() {\n    gl_FragColor = vVignette * texture2D(uTextureSampler, vTextureCoord);\n}\n");
			 if (holder.program == 0)
		 {
			   throw new Exception("Could not create program");
	
		 }			 holder.aPosition = GLES20.glGetAttribLocation(holder.program, "aPosition");
			 checkGlError("glGetAttribLocation aPosition");
			 if (holder.aPosition == -1)
		 {
			   throw new Exception("Could not get attrib location for aPosition");
	
		 }			 holder.aVignette = GLES20.glGetAttribLocation(holder.program, "aVignette");
			 checkGlError("glGetAttribLocation aVignette");
			 if (holder.aVignette == -1)
		 {
			   throw new Exception("Could not get attrib location for aVignette");
	
		 }			 holder.aTextureCoord = GLES20.glGetAttribLocation(holder.program, "aTextureCoord");
	     
			 checkGlError("glGetAttribLocation aTextureCoord");
			 if (holder.aTextureCoord == -1)
		 {
			   throw new Exception("Could not get attrib location for aTextureCoord");
	
		 }			 holder.uTextureCoordScale = GLES20.glGetUniformLocation(holder.program, "uTextureCoordScale");
	     
			 checkGlError("glGetUniformLocation uTextureCoordScale");
			 if (holder.uTextureCoordScale == -1)
		 {
			   throw new Exception("Could not get attrib location for uTextureCoordScale");
	
		 }			 holder.uTextureSampler = GLES20.glGetUniformLocation(holder.program, "uTextureSampler");
	     
			 checkGlError("glGetUniformLocation uTextureSampler");
			 if (holder.uTextureSampler == -1)
		 {
			   throw new Exception("Could not get attrib location for uTextureSampler");
	
		 }			 return holder;
	
	   }	   
		   private void checkGlError(string op)
	
	   {			 int error;
			 if ((error = GLES20.glGetError()) != 0)
	
		 {			   Log.e("DistortionRenderer", op + ": glError " + error);
			   throw new Exception(op + ": glError " + error);
	
		 }	
	   }	   
		   private static float clamp(float val, float min, float max)
	
	   {			 return Math.Max(min, Math.Min(max, val));
	
	   }	   
		   private class DistortionMesh
	
	   {		   private readonly DistortionRenderer outerInstance;

			 internal const string TAG = "DistortionMesh";
			 public const int BYTES_PER_FLOAT = 4;
			 public const int BYTES_PER_INT = 4;
			 public readonly int COMPONENTS_PER_VERT = 5;
			 public readonly int DATA_STRIDE_BYTES = 20;
			 public readonly int DATA_POS_OFFSET = 0;
			 public readonly int DATA_VIGNETTE_OFFSET = 2;
			 public readonly int DATA_UV_OFFSET = 3;
			 public readonly int ROWS = 40;
			 public readonly int COLS = 40;
			 public readonly float VIGNETTE_SIZE_M_SCREEN = 0.002F;
			 public int nIndices;
			 public int mArrayBufferId = -1;
			 public int mElementBufferId = -1;
	     
			 public DistortionMesh(DistortionRenderer outerInstance, EyeParams eye, Distortion distortion, float screenWidthM, float screenHeightM, float xEyeOffsetMScreen, float yEyeOffsetMScreen, float textureWidthM, float textureHeightM, float xEyeOffsetMTexture, float yEyeOffsetMTexture, float viewportXMTexture, float viewportYMTexture, float viewportWidthMTexture, float viewportHeightMTexture)
	
		 {			 this.outerInstance = outerInstance;
			   float mPerUScreen = screenWidthM;
			   float mPerVScreen = screenHeightM;
			   float mPerUTexture = textureWidthM;
			   float mPerVTexture = textureHeightM;
	       
			   float[] vertexData = new float[8000];
			   int vertexOffset = 0;
			   for (int row = 0; row < 40; row++)
		   {
				 for (int col = 0; col < 40; col++)
	
			 {				   float uTexture = col / 39.0F * (viewportWidthMTexture / textureWidthM) + viewportXMTexture / textureWidthM;
	           
	 
				   float vTexture = row / 39.0F * (viewportHeightMTexture / textureHeightM) + viewportYMTexture / textureHeightM;
	           
	 
	 
				   float xTexture = uTexture * mPerUTexture;
				   float yTexture = vTexture * mPerVTexture;
				   float xTextureEye = xTexture - xEyeOffsetMTexture;
				   float yTextureEye = yTexture - yEyeOffsetMTexture;
				   float rTexture = (float)Math.Sqrt(xTextureEye * xTextureEye + yTextureEye * yTextureEye);
	           
				   float textureToScreen = rTexture > 0.0F ? distortion.distortInverse(rTexture) / rTexture : 1.0F;
	           
	 
				   float xScreen = xTextureEye * textureToScreen + xEyeOffsetMScreen;
				   float yScreen = yTextureEye * textureToScreen + yEyeOffsetMScreen;
				   float uScreen = xScreen / mPerUScreen;
				   float vScreen = yScreen / mPerVScreen;
				   float vignetteSizeMTexture = 0.002F / textureToScreen;
	           
	 
				   float dxTexture = xTexture - DistortionRenderer.clamp(xTexture, viewportXMTexture + vignetteSizeMTexture, viewportXMTexture + viewportWidthMTexture - vignetteSizeMTexture);
	           
	 
	 
				   float dyTexture = yTexture - DistortionRenderer.clamp(yTexture, viewportYMTexture + vignetteSizeMTexture, viewportYMTexture + viewportHeightMTexture - vignetteSizeMTexture);
	           
	 
	 
				   float drTexture = (float)Math.Sqrt(dxTexture * dxTexture + dyTexture * dyTexture);
	           
				   float vignette = 1.0F - DistortionRenderer.clamp(drTexture / vignetteSizeMTexture, 0.0F, 1.0F);
	           
				   vertexData[(vertexOffset + 0)] = (2.0F * uScreen - 1.0F);
				   vertexData[(vertexOffset + 1)] = (2.0F * vScreen - 1.0F);
				   vertexData[(vertexOffset + 2)] = vignette;
				   vertexData[(vertexOffset + 3)] = uTexture;
				   vertexData[(vertexOffset + 4)] = vTexture;
	           
				   vertexOffset += 5;
	
			 }	
		   }			   this.nIndices = 3158;
			   int[] indexData = new int[this.nIndices];
			   int indexOffset = 0;
			   vertexOffset = 0;
			   for (int row = 0; row < 39; row++)
	
		   {				 if (row > 0)
	
			 {				   indexData[indexOffset] = indexData[(indexOffset - 1)];
				   indexOffset++;
	
			 }				 for (int col = 0; col < 40; col++)
	
			 {				   if (col > 0)
			   {
					 if (row % 2 == 0)
				 {
					   vertexOffset++;
	
				 }				 else
				 {
					   vertexOffset--;
	
				 }	
			   }				   indexData[(indexOffset++)] = vertexOffset;
				   indexData[(indexOffset++)] = (vertexOffset + 40);
	
			 }				 vertexOffset += 40;
	
		   }			   FloatBuffer vertexBuffer = ByteBuffer.allocateDirect(vertexData.Length * 4).order(ByteOrder.nativeOrder()).asFloatBuffer();
	       
	 
			   vertexBuffer.put(vertexData).position(0);
	       
			   IntBuffer indexBuffer = ByteBuffer.allocateDirect(indexData.Length * 4).order(ByteOrder.nativeOrder()).asIntBuffer();
	       
	 
			   indexBuffer.put(indexData).position(0);
	       
			   int[] bufferIds = new int[2];
			   GLES20.glGenBuffers(2, bufferIds, 0);
			   this.mArrayBufferId = bufferIds[0];
			   this.mElementBufferId = bufferIds[1];
	       
			   GLES20.glBindBuffer(34962, this.mArrayBufferId);
			   GLES20.glBufferData(34962, vertexData.Length * 4, vertexBuffer, 35044);
	       
	 
			   GLES20.glBindBuffer(34963, this.mElementBufferId);
			   GLES20.glBufferData(34963, indexData.Length * 4, indexBuffer, 35044);
	       
	 
			   GLES20.glBindBuffer(34962, 0);
			   GLES20.glBindBuffer(34963, 0);
	
		 }	
	   }	
	 }

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     com.google.vrtoolkit.cardboard.DistortionRenderer
	 * JD-Core Version:    0.7.0.1
	 */
 }