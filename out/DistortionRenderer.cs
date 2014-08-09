using System;

/*   1:    */ namespace com.google.vrtoolkit.cardboard
 {
	/*   2:    */ 
	/*   3:    */	 using GLES20 = android.opengl.GLES20;
	/*   4:    */	 using Log = android.util.Log;
	/*   5:    */	/*   6:    */	/*   7:    */	/*   8:    */	/*   9:    */ 
	/*  10:    */	 public class DistortionRenderer
	/*  11:    */
	 {	/*  12:    */	   private const string TAG = "DistortionRenderer";
	/*  13:    */	   private int mTextureId;
	/*  14:    */	   private int mRenderbufferId;
	/*  15:    */	   private int mFramebufferId;
	/*  16:    */	   private IntBuffer mOriginalFramebufferId;
	/*  17:    */	   private IntBuffer mCullFaceEnabled;
	/*  18:    */	   private IntBuffer mScissorTestEnabled;
	/*  19:    */	   private IntBuffer mViewport;
	/*  20:    */	   private float mResolutionScale;
	/*  21:    */	   private DistortionMesh mLeftEyeDistortionMesh;
	/*  22:    */	   private DistortionMesh mRightEyeDistortionMesh;
	/*  23:    */	   private HeadMountedDisplay mHmd;
	/*  24:    */	   private FieldOfView mLeftEyeFov;
	/*  25:    */	   private FieldOfView mRightEyeFov;
	/*  26:    */	   private ProgramHolder mProgramHolder;
	/*  27:    */	   private readonly string VERTEX_SHADER = "attribute vec2 aPosition;\nattribute float aVignette;\nattribute vec2 aTextureCoord;\nvarying vec2 vTextureCoord;\nvarying float vVignette;\nuniform float uTextureCoordScale;\nvoid main() {\n    gl_Position = vec4(aPosition, 0.0, 1.0);\n    vTextureCoord = aTextureCoord.xy * uTextureCoordScale;\n    vVignette = aVignette;\n}\n";
	/*  28:    */	   private readonly string FRAGMENT_SHADER = "precision mediump float;\nvarying vec2 vTextureCoord;\nvarying float vVignette;\nuniform sampler2D uTextureSampler;\nvoid main() {\n    gl_FragColor = vVignette * texture2D(uTextureSampler, vTextureCoord);\n}\n";
	/*  29:    */   
	/*  30:    */	   public DistortionRenderer()
	/*  31:    */
	   {	/*  32: 34 */		 this.mTextureId = -1;
	/*  33: 35 */		 this.mRenderbufferId = -1;
	/*  34: 36 */		 this.mFramebufferId = -1;
	/*  35: 37 */		 this.mOriginalFramebufferId = IntBuffer.allocate(1);
	/*  36: 38 */		 this.mCullFaceEnabled = IntBuffer.allocate(1);
	/*  37: 39 */		 this.mScissorTestEnabled = IntBuffer.allocate(1);
	/*  38: 40 */		 this.mViewport = IntBuffer.allocate(4);
	/*  39:    */     
	/*  40: 42 */		 this.mResolutionScale = 1.0F;
	/*  41:    */     
	/*  42:    */ 
	/*  43:    */ 
	/*  44:    */ 
	/*  45:    */ 
	/*  46:    */ 
	/*  47:    */ 
	/*  48:    */ 
	/*  49:    */ 
	/*  50:    */ 
	/*  51: 53 */		 this.VERTEX_SHADER = "attribute vec2 aPosition;\nattribute float aVignette;\nattribute vec2 aTextureCoord;\nvarying vec2 vTextureCoord;\nvarying float vVignette;\nuniform float uTextureCoordScale;\nvoid main() {\n    gl_Position = vec4(aPosition, 0.0, 1.0);\n    vTextureCoord = aTextureCoord.xy * uTextureCoordScale;\n    vVignette = aVignette;\n}\n";
	/*  52:    */     
	/*  53:    */ 
	/*  54:    */ 
	/*  55:    */ 
	/*  56:    */ 
	/*  57:    */ 
	/*  58:    */ 
	/*  59:    */ 
	/*  60:    */ 
	/*  61:    */ 
	/*  62:    */ 
	/*  63:    */ 
	/*  64: 66 */		 this.FRAGMENT_SHADER = "precision mediump float;\nvarying vec2 vTextureCoord;\nvarying float vVignette;\nuniform sampler2D uTextureSampler;\nvoid main() {\n    gl_FragColor = vVignette * texture2D(uTextureSampler, vTextureCoord);\n}\n";
	/*  65:    */
	   }	/*  66:    */   
	/*  67:    */	   private class ProgramHolder
	/*  68:    */
	   {		   private readonly DistortionRenderer outerInstance;

	/*  69:    */		 public int program;
	/*  70:    */		 public int aPosition;
	/*  71:    */		 public int aVignette;
	/*  72:    */		 public int aTextureCoord;
	/*  73:    */		 public int uTextureCoordScale;
	/*  74:    */		 public int uTextureSampler;
	/*  75:    */     
	/*  76:    */		 internal ProgramHolder(DistortionRenderer outerInstance)
		 {
			 this.outerInstance = outerInstance;
		 }
	/*  77:    */
	   }	/*  78:    */   
	/*  79:    */	   private class EyeViewport
	/*  80:    */
	   {		   private readonly DistortionRenderer outerInstance;

	/*  81:    */		 public float x;
	/*  82:    */		 public float y;
	/*  83:    */		 public float width;
	/*  84:    */		 public float height;
	/*  85:    */		 public float eyeX;
	/*  86:    */		 public float eyeY;
	/*  87:    */     
	/*  88:    */		 internal EyeViewport(DistortionRenderer outerInstance)
		 {
			 this.outerInstance = outerInstance;
		 }
	/*  89:    */     
	/*  90:    */		 public override string ToString()
	/*  91:    */
		 {	/*  92:101 */		   return "EyeViewport {x:" + this.x + " y:" + this.y + " width:" + this.width + " height:" + this.height + " eyeX: " + this.eyeX + " eyeY: " + this.eyeY + "}";
	/*  93:    */
		 }	/*  94:    */
	   }	/*  95:    */   
	/*  96:    */	   public virtual void beforeDrawFrame()
	/*  97:    */
	   {	/*  98:113 */		 GLES20.glGetIntegerv(36006, this.mOriginalFramebufferId);
	/*  99:114 */		 GLES20.glBindFramebuffer(36160, this.mFramebufferId);
	/* 100:    */
	   }	/* 101:    */   
	/* 102:    */	   public virtual void afterDrawFrame()
	/* 103:    */
	   {	/* 104:123 */		 GLES20.glBindFramebuffer(36160, this.mOriginalFramebufferId.array()[0]);
	/* 105:124 */		 GLES20.glViewport(0, 0, this.mHmd.Screen.Width, this.mHmd.Screen.Height);
	/* 106:    */     
	/* 107:126 */		 GLES20.glGetIntegerv(2978, this.mViewport);
	/* 108:127 */		 GLES20.glGetIntegerv(2884, this.mCullFaceEnabled);
	/* 109:128 */		 GLES20.glGetIntegerv(3089, this.mScissorTestEnabled);
	/* 110:129 */		 GLES20.glDisable(3089);
	/* 111:130 */		 GLES20.glDisable(2884);
	/* 112:    */     
	/* 113:132 */		 GLES20.glClearColor(0.0F, 0.0F, 0.0F, 1.0F);
	/* 114:133 */		 GLES20.glClear(16640);
	/* 115:    */     
	/* 116:135 */		 GLES20.glUseProgram(this.mProgramHolder.program);
	/* 117:    */     
	/* 118:137 */		 GLES20.glEnable(3089);
	/* 119:138 */		 GLES20.glScissor(0, 0, this.mHmd.Screen.Width / 2, this.mHmd.Screen.Height);
	/* 120:    */     
	/* 121:    */ 
	/* 122:    */ 
	/* 123:142 */		 renderDistortionMesh(this.mLeftEyeDistortionMesh);
	/* 124:    */     
	/* 125:144 */		 GLES20.glScissor(this.mHmd.Screen.Width / 2, 0, this.mHmd.Screen.Width / 2, this.mHmd.Screen.Height);
	/* 126:    */     
	/* 127:    */ 
	/* 128:    */ 
	/* 129:148 */		 renderDistortionMesh(this.mRightEyeDistortionMesh);
	/* 130:    */     
	/* 131:150 */		 GLES20.glDisableVertexAttribArray(this.mProgramHolder.aPosition);
	/* 132:151 */		 GLES20.glDisableVertexAttribArray(this.mProgramHolder.aVignette);
	/* 133:152 */		 GLES20.glDisableVertexAttribArray(this.mProgramHolder.aTextureCoord);
	/* 134:153 */		 GLES20.glUseProgram(0);
	/* 135:154 */		 GLES20.glBindBuffer(34962, 0);
	/* 136:155 */		 GLES20.glBindBuffer(34963, 0);
	/* 137:156 */		 GLES20.glDisable(3089);
	/* 138:157 */		 if (this.mCullFaceEnabled.array()[0] == 1)
		 {
	/* 139:158 */		   GLES20.glEnable(2884);
	/* 140:    */
		 }	/* 141:160 */		 if (this.mScissorTestEnabled.array()[0] == 1)
		 {
	/* 142:161 */		   GLES20.glEnable(3089);
	/* 143:    */
		 }	/* 144:163 */		 GLES20.glViewport(this.mViewport.array()[0], this.mViewport.array()[1], this.mViewport.array()[2], this.mViewport.array()[3]);
	/* 145:    */
	   }	/* 146:    */   
	/* 147:    */
	   public virtual float ResolutionScale
	   {
		   set
		/* 148:    */
		   {		/* 149:178 */			 this.mResolutionScale = value;
		/* 150:    */
		   }
	   }	/* 151:    */   
	/* 152:    */	   public virtual void onProjectionChanged(HeadMountedDisplay hmd, EyeParams leftEye, EyeParams rightEye, float zNear, float zFar)
	/* 153:    */
	   {	/* 154:199 */		 this.mHmd = new HeadMountedDisplay(hmd);
	/* 155:200 */		 this.mLeftEyeFov = new FieldOfView(leftEye.Fov);
	/* 156:201 */		 this.mRightEyeFov = new FieldOfView(rightEye.Fov);
	/* 157:    */     
	/* 158:203 */		 ScreenParams screen = this.mHmd.Screen;
	/* 159:204 */		 CardboardDeviceParams cdp = this.mHmd.Cardboard;
	/* 160:206 */		 if (this.mProgramHolder == null)
		 {
	/* 161:207 */		   this.mProgramHolder = createProgramHolder();
	/* 162:    */
		 }	/* 163:210 */		 EyeViewport leftEyeViewport = initViewportForEye(leftEye, 0.0F);
	/* 164:211 */		 EyeViewport rightEyeViewport = initViewportForEye(rightEye, leftEyeViewport.width);
	/* 165:    */     
	/* 166:213 */		 leftEye.Fov.toPerspectiveMatrix(zNear, zFar, leftEye.Transform.Perspective, 0);
	/* 167:    */     
	/* 168:215 */		 rightEye.Fov.toPerspectiveMatrix(zNear, zFar, rightEye.Transform.Perspective, 0);
	/* 169:    */     
	/* 170:    */ 
	/* 171:218 */		 float textureWidthM = leftEyeViewport.width + rightEyeViewport.width;
	/* 172:219 */		 float textureHeightM = Math.Max(leftEyeViewport.height, rightEyeViewport.height);
	/* 173:220 */		 float xPxPerM = screen.Width / screen.WidthMeters;
	/* 174:221 */		 float yPxPerM = screen.Height / screen.HeightMeters;
	/* 175:222 */		 int textureWidthPx = Math.Round(textureWidthM * xPxPerM);
	/* 176:223 */		 int textureHeightPx = Math.Round(textureHeightM * yPxPerM);
	/* 177:    */     
	/* 178:225 */		 float xEyeOffsetMScreen = screen.WidthMeters / 2.0F - cdp.InterpupillaryDistance / 2.0F;
	/* 179:226 */		 float yEyeOffsetMScreen = cdp.VerticalDistanceToLensCenter - screen.BorderSizeMeters;
	/* 180:    */     
	/* 181:    */ 
	/* 182:229 */		 this.mLeftEyeDistortionMesh = createDistortionMesh(leftEye, leftEyeViewport, textureWidthM, textureHeightM, xEyeOffsetMScreen, yEyeOffsetMScreen);
	/* 183:    */     
	/* 184:    */ 
	/* 185:    */ 
	/* 186:233 */		 xEyeOffsetMScreen = screen.WidthMeters - xEyeOffsetMScreen;
	/* 187:234 */		 this.mRightEyeDistortionMesh = createDistortionMesh(rightEye, rightEyeViewport, textureWidthM, textureHeightM, xEyeOffsetMScreen, yEyeOffsetMScreen);
	/* 188:    */     
	/* 189:    */ 
	/* 190:    */ 
	/* 191:    */ 
	/* 192:239 */		 setupRenderTextureAndRenderbuffer(textureWidthPx, textureHeightPx);
	/* 193:    */
	   }	/* 194:    */   
	/* 195:    */	   private EyeViewport initViewportForEye(EyeParams eye, float xOffsetM)
	/* 196:    */
	   {	/* 197:244 */		 ScreenParams screen = this.mHmd.Screen;
	/* 198:245 */		 CardboardDeviceParams cdp = this.mHmd.Cardboard;
	/* 199:    */     
	/* 200:247 */		 float eyeToScreenDistanceM = cdp.EyeToLensDistance + cdp.ScreenToLensDistance;
	/* 201:248 */		 float leftM = (float)Math.Tan(Math.toRadians(eye.Fov.Left)) * eyeToScreenDistanceM;
	/* 202:    */     
	/* 203:250 */		 float rightM = (float)Math.Tan(Math.toRadians(eye.Fov.Right)) * eyeToScreenDistanceM;
	/* 204:    */     
	/* 205:252 */		 float bottomM = (float)Math.Tan(Math.toRadians(eye.Fov.Bottom)) * eyeToScreenDistanceM;
	/* 206:    */     
	/* 207:254 */		 float topM = (float)Math.Tan(Math.toRadians(eye.Fov.Top)) * eyeToScreenDistanceM;
	/* 208:    */     
	/* 209:    */ 
	/* 210:257 */		 EyeViewport vp = new EyeViewport(this, null);
	/* 211:258 */		 vp.x = xOffsetM;
	/* 212:259 */		 vp.y = 0.0F;
	/* 213:260 */		 vp.width = (leftM + rightM);
	/* 214:261 */		 vp.height = (bottomM + topM);
	/* 215:262 */		 vp.eyeX = (leftM + xOffsetM);
	/* 216:263 */		 vp.eyeY = bottomM;
	/* 217:    */     
	/* 218:265 */		 float xPxPerM = screen.Width / screen.WidthMeters;
	/* 219:266 */		 float yPxPerM = screen.Height / screen.HeightMeters;
	/* 220:267 */		 eye.Viewport.x = Math.Round(vp.x * xPxPerM);
	/* 221:268 */		 eye.Viewport.y = Math.Round(vp.y * xPxPerM);
	/* 222:269 */		 eye.Viewport.width = Math.Round(vp.width * xPxPerM);
	/* 223:270 */		 eye.Viewport.height = Math.Round(vp.height * xPxPerM);
	/* 224:    */     
	/* 225:272 */		 return vp;
	/* 226:    */
	   }	/* 227:    */   
	/* 228:    */	   private DistortionMesh createDistortionMesh(EyeParams eye, EyeViewport eyeViewport, float textureWidthM, float textureHeightM, float xEyeOffsetMScreen, float yEyeOffsetMScreen)
	/* 229:    */
	   {	/* 230:280 */		 return new DistortionMesh(this, eye, this.mHmd.Cardboard.Distortion, this.mHmd.Screen.WidthMeters, this.mHmd.Screen.HeightMeters, xEyeOffsetMScreen, yEyeOffsetMScreen, textureWidthM, textureHeightM, eyeViewport.eyeX, eyeViewport.eyeY, eyeViewport.x, eyeViewport.y, eyeViewport.width, eyeViewport.height);
	/* 231:    */
	   }	/* 232:    */   
	/* 233:    */	   private void renderDistortionMesh(DistortionMesh mesh)
	/* 234:    */
	   {	/* 235:292 */		 GLES20.glBindBuffer(34962, mesh.mArrayBufferId);
	/* 236:293 */		 mesh.GetType();
		 mesh.GetType();
		 GLES20.glVertexAttribPointer(this.mProgramHolder.aPosition, 3, 5126, false, 20, 0 * 4);
	/* 237:    */     
	/* 238:    */ 
	/* 239:    */ 
	/* 240:    */ 
	/* 241:    */ 
	/* 242:    */ 
	/* 243:300 */		 GLES20.glEnableVertexAttribArray(this.mProgramHolder.aPosition);
	/* 244:    */     
	/* 245:302 */		 mesh.GetType();
		 mesh.GetType();
		 GLES20.glVertexAttribPointer(this.mProgramHolder.aVignette, 1, 5126, false, 20, 2 * 4);
	/* 246:    */     
	/* 247:    */ 
	/* 248:    */ 
	/* 249:    */ 
	/* 250:    */ 
	/* 251:    */ 
	/* 252:309 */		 GLES20.glEnableVertexAttribArray(this.mProgramHolder.aVignette);
	/* 253:    */     
	/* 254:311 */		 mesh.GetType();
		 mesh.GetType();
		 GLES20.glVertexAttribPointer(this.mProgramHolder.aTextureCoord, 2, 5126, false, 20, 3 * 4);
	/* 255:    */     
	/* 256:    */ 
	/* 257:    */ 
	/* 258:    */ 
	/* 259:    */ 
	/* 260:    */ 
	/* 261:318 */		 GLES20.glEnableVertexAttribArray(this.mProgramHolder.aTextureCoord);
	/* 262:    */     
	/* 263:320 */		 GLES20.glActiveTexture(33984);
	/* 264:321 */		 GLES20.glBindTexture(3553, this.mTextureId);
	/* 265:322 */		 GLES20.glUniform1i(this.mProgramHolder.uTextureSampler, 0);
	/* 266:323 */		 GLES20.glUniform1f(this.mProgramHolder.uTextureCoordScale, this.mResolutionScale);
	/* 267:    */     
	/* 268:    */ 
	/* 269:326 */		 GLES20.glBindBuffer(34963, mesh.mElementBufferId);
	/* 270:327 */		 GLES20.glDrawElements(5, mesh.nIndices, 5125, 0);
	/* 271:    */
	   }	/* 272:    */   
	/* 273:    */	   private float computeDistortionScale(Distortion distortion, float screenWidthM, float interpupillaryDistanceM)
	/* 274:    */
	   {	/* 275:337 */		 return distortion.distortionFactor((screenWidthM / 2.0F - interpupillaryDistanceM / 2.0F) / (screenWidthM / 4.0F));
	/* 276:    */
	   }	/* 277:    */   
	/* 278:    */	   private int createTexture(int width, int height)
	/* 279:    */
	   {	/* 280:344 */		 int[] textureIds = new int[1];
	/* 281:345 */		 GLES20.glGenTextures(1, textureIds, 0);
	/* 282:    */     
	/* 283:347 */		 GLES20.glBindTexture(3553, textureIds[0]);
	/* 284:348 */		 GLES20.glTexParameteri(3553, 10242, 33071);
	/* 285:    */     
	/* 286:350 */		 GLES20.glTexParameteri(3553, 10243, 33071);
	/* 287:    */     
	/* 288:352 */		 GLES20.glTexParameteri(3553, 10240, 9729);
	/* 289:    */     
	/* 290:354 */		 GLES20.glTexParameteri(3553, 10241, 9729);
	/* 291:    */     
	/* 292:    */ 
	/* 293:    */ 
	/* 294:358 */		 GLES20.glTexImage2D(3553, 0, 6407, width, height, 0, 6407, 33635, null);
	/* 295:    */     
	/* 296:    */ 
	/* 297:    */ 
	/* 298:    */ 
	/* 299:    */ 
	/* 300:    */ 
	/* 301:    */ 
	/* 302:    */ 
	/* 303:    */ 
	/* 304:    */ 
	/* 305:369 */		 return textureIds[0];
	/* 306:    */
	   }	/* 307:    */   
	/* 308:    */	   private int setupRenderTextureAndRenderbuffer(int width, int height)
	/* 309:    */
	   {	/* 310:374 */		 if (this.mTextureId != -1)
		 {
	/* 311:375 */		   GLES20.glDeleteTextures(1, new int[] {this.mTextureId}, 0);
	/* 312:    */
		 }	/* 313:377 */		 if (this.mRenderbufferId != -1)
		 {
	/* 314:378 */		   GLES20.glDeleteRenderbuffers(1, new int[] {this.mRenderbufferId}, 0);
	/* 315:    */
		 }	/* 316:380 */		 if (this.mFramebufferId != -1)
		 {
	/* 317:381 */		   GLES20.glDeleteFramebuffers(1, new int[] {this.mFramebufferId}, 0);
	/* 318:    */
		 }	/* 319:384 */		 this.mTextureId = createTexture(width, height);
	/* 320:385 */		 checkGlError("setupRenderTextureAndRenderbuffer: create texture");
	/* 321:    */     
	/* 322:    */ 
	/* 323:388 */		 int[] renderbufferIds = new int[1];
	/* 324:389 */		 GLES20.glGenRenderbuffers(1, renderbufferIds, 0);
	/* 325:390 */		 GLES20.glBindRenderbuffer(36161, renderbufferIds[0]);
	/* 326:391 */		 GLES20.glRenderbufferStorage(36161, 33189, width, height);
	/* 327:    */     
	/* 328:393 */		 this.mRenderbufferId = renderbufferIds[0];
	/* 329:394 */		 checkGlError("setupRenderTextureAndRenderbuffer: create renderbuffer");
	/* 330:    */     
	/* 331:396 */		 int[] framebufferIds = new int[1];
	/* 332:397 */		 GLES20.glGenFramebuffers(1, framebufferIds, 0);
	/* 333:398 */		 GLES20.glBindFramebuffer(36160, framebufferIds[0]);
	/* 334:399 */		 this.mFramebufferId = framebufferIds[0];
	/* 335:    */     
	/* 336:401 */		 GLES20.glFramebufferTexture2D(36160, 36064, 3553, this.mTextureId, 0);
	/* 337:    */     
	/* 338:    */ 
	/* 339:404 */		 GLES20.glFramebufferRenderbuffer(36160, 36096, 36161, renderbufferIds[0]);
	/* 340:    */     
	/* 341:    */ 
	/* 342:    */ 
	/* 343:408 */		 int status = GLES20.glCheckFramebufferStatus(36160);
	/* 344:410 */		 if (status != 36053)
		 {
	/* 345:411 */		   throw new Exception("Framebuffer is not complete: " + status.ToString("x"));
	/* 346:    */
		 }	/* 347:416 */		 GLES20.glBindFramebuffer(36160, 0);
	/* 348:    */     
	/* 349:418 */		 return framebufferIds[0];
	/* 350:    */
	   }	/* 351:    */   
	/* 352:    */	   private int loadShader(int shaderType, string source)
	/* 353:    */
	   {	/* 354:422 */		 int shader = GLES20.glCreateShader(shaderType);
	/* 355:423 */		 if (shader != 0)
	/* 356:    */
		 {	/* 357:424 */		   GLES20.glShaderSource(shader, source);
	/* 358:425 */		   GLES20.glCompileShader(shader);
	/* 359:426 */		   int[] compiled = new int[1];
	/* 360:427 */		   GLES20.glGetShaderiv(shader, 35713, compiled, 0);
	/* 361:428 */		   if (compiled[0] == 0)
	/* 362:    */
		   {	/* 363:429 */			 Log.e("DistortionRenderer", "Could not compile shader " + shaderType + ":");
	/* 364:430 */			 Log.e("DistortionRenderer", GLES20.glGetShaderInfoLog(shader));
	/* 365:431 */			 GLES20.glDeleteShader(shader);
	/* 366:432 */			 shader = 0;
	/* 367:    */
		   }	/* 368:    */
		 }	/* 369:435 */		 return shader;
	/* 370:    */
	   }	/* 371:    */   
	/* 372:    */	   private int createProgram(string vertexSource, string fragmentSource)
	/* 373:    */
	   {	/* 374:439 */		 int vertexShader = loadShader(35633, vertexSource);
	/* 375:440 */		 if (vertexShader == 0)
		 {
	/* 376:441 */		   return 0;
	/* 377:    */
		 }	/* 378:443 */		 int pixelShader = loadShader(35632, fragmentSource);
	/* 379:444 */		 if (pixelShader == 0)
		 {
	/* 380:445 */		   return 0;
	/* 381:    */
		 }	/* 382:448 */		 int program = GLES20.glCreateProgram();
	/* 383:449 */		 if (program != 0)
	/* 384:    */
		 {	/* 385:450 */		   GLES20.glAttachShader(program, vertexShader);
	/* 386:451 */		   checkGlError("glAttachShader");
	/* 387:452 */		   GLES20.glAttachShader(program, pixelShader);
	/* 388:453 */		   checkGlError("glAttachShader");
	/* 389:454 */		   GLES20.glLinkProgram(program);
	/* 390:455 */		   int[] linkStatus = new int[1];
	/* 391:456 */		   GLES20.glGetProgramiv(program, 35714, linkStatus, 0);
	/* 392:457 */		   if (linkStatus[0] != 1)
	/* 393:    */
		   {	/* 394:458 */			 Log.e("DistortionRenderer", "Could not link program: ");
	/* 395:459 */			 Log.e("DistortionRenderer", GLES20.glGetProgramInfoLog(program));
	/* 396:460 */			 GLES20.glDeleteProgram(program);
	/* 397:461 */			 program = 0;
	/* 398:    */
		   }	/* 399:    */
		 }	/* 400:464 */		 return program;
	/* 401:    */
	   }	/* 402:    */   
	/* 403:    */	   private ProgramHolder createProgramHolder()
	/* 404:    */
	   {	/* 405:468 */		 ProgramHolder holder = new ProgramHolder(this, null);
	/* 406:469 */		 holder.program = createProgram("attribute vec2 aPosition;\nattribute float aVignette;\nattribute vec2 aTextureCoord;\nvarying vec2 vTextureCoord;\nvarying float vVignette;\nuniform float uTextureCoordScale;\nvoid main() {\n    gl_Position = vec4(aPosition, 0.0, 1.0);\n    vTextureCoord = aTextureCoord.xy * uTextureCoordScale;\n    vVignette = aVignette;\n}\n", "precision mediump float;\nvarying vec2 vTextureCoord;\nvarying float vVignette;\nuniform sampler2D uTextureSampler;\nvoid main() {\n    gl_FragColor = vVignette * texture2D(uTextureSampler, vTextureCoord);\n}\n");
	/* 407:470 */		 if (holder.program == 0)
		 {
	/* 408:471 */		   throw new Exception("Could not create program");
	/* 409:    */
		 }	/* 410:474 */		 holder.aPosition = GLES20.glGetAttribLocation(holder.program, "aPosition");
	/* 411:475 */		 checkGlError("glGetAttribLocation aPosition");
	/* 412:476 */		 if (holder.aPosition == -1)
		 {
	/* 413:477 */		   throw new Exception("Could not get attrib location for aPosition");
	/* 414:    */
		 }	/* 415:479 */		 holder.aVignette = GLES20.glGetAttribLocation(holder.program, "aVignette");
	/* 416:480 */		 checkGlError("glGetAttribLocation aVignette");
	/* 417:481 */		 if (holder.aVignette == -1)
		 {
	/* 418:482 */		   throw new Exception("Could not get attrib location for aVignette");
	/* 419:    */
		 }	/* 420:484 */		 holder.aTextureCoord = GLES20.glGetAttribLocation(holder.program, "aTextureCoord");
	/* 421:    */     
	/* 422:486 */		 checkGlError("glGetAttribLocation aTextureCoord");
	/* 423:487 */		 if (holder.aTextureCoord == -1)
		 {
	/* 424:488 */		   throw new Exception("Could not get attrib location for aTextureCoord");
	/* 425:    */
		 }	/* 426:490 */		 holder.uTextureCoordScale = GLES20.glGetUniformLocation(holder.program, "uTextureCoordScale");
	/* 427:    */     
	/* 428:492 */		 checkGlError("glGetUniformLocation uTextureCoordScale");
	/* 429:493 */		 if (holder.uTextureCoordScale == -1)
		 {
	/* 430:494 */		   throw new Exception("Could not get attrib location for uTextureCoordScale");
	/* 431:    */
		 }	/* 432:496 */		 holder.uTextureSampler = GLES20.glGetUniformLocation(holder.program, "uTextureSampler");
	/* 433:    */     
	/* 434:498 */		 checkGlError("glGetUniformLocation uTextureSampler");
	/* 435:499 */		 if (holder.uTextureSampler == -1)
		 {
	/* 436:500 */		   throw new Exception("Could not get attrib location for uTextureSampler");
	/* 437:    */
		 }	/* 438:503 */		 return holder;
	/* 439:    */
	   }	/* 440:    */   
	/* 441:    */	   private void checkGlError(string op)
	/* 442:    */
	   {	/* 443:    */		 int error;
	/* 444:508 */		 if ((error = GLES20.glGetError()) != 0)
	/* 445:    */
		 {	/* 446:509 */		   Log.e("DistortionRenderer", op + ": glError " + error);
	/* 447:510 */		   throw new Exception(op + ": glError " + error);
	/* 448:    */
		 }	/* 449:    */
	   }	/* 450:    */   
	/* 451:    */	   private static float clamp(float val, float min, float max)
	/* 452:    */
	   {	/* 453:515 */		 return Math.Max(min, Math.Min(max, val));
	/* 454:    */
	   }	/* 455:    */   
	/* 456:    */	   private class DistortionMesh
	/* 457:    */
	   {		   private readonly DistortionRenderer outerInstance;

	/* 458:    */		 internal const string TAG = "DistortionMesh";
	/* 459:    */		 public const int BYTES_PER_FLOAT = 4;
	/* 460:    */		 public const int BYTES_PER_INT = 4;
	/* 461:527 */		 public readonly int COMPONENTS_PER_VERT = 5;
	/* 462:528 */		 public readonly int DATA_STRIDE_BYTES = 20;
	/* 463:530 */		 public readonly int DATA_POS_OFFSET = 0;
	/* 464:531 */		 public readonly int DATA_VIGNETTE_OFFSET = 2;
	/* 465:532 */		 public readonly int DATA_UV_OFFSET = 3;
	/* 466:533 */		 public readonly int ROWS = 40;
	/* 467:534 */		 public readonly int COLS = 40;
	/* 468:537 */		 public readonly float VIGNETTE_SIZE_M_SCREEN = 0.002F;
	/* 469:    */		 public int nIndices;
	/* 470:542 */		 public int mArrayBufferId = -1;
	/* 471:543 */		 public int mElementBufferId = -1;
	/* 472:    */     
	/* 473:    */		 public DistortionMesh(DistortionRenderer outerInstance, EyeParams eye, Distortion distortion, float screenWidthM, float screenHeightM, float xEyeOffsetMScreen, float yEyeOffsetMScreen, float textureWidthM, float textureHeightM, float xEyeOffsetMTexture, float yEyeOffsetMTexture, float viewportXMTexture, float viewportYMTexture, float viewportWidthMTexture, float viewportHeightMTexture)
	/* 474:    */
		 {			 this.outerInstance = outerInstance;
	/* 475:553 */		   float mPerUScreen = screenWidthM;
	/* 476:554 */		   float mPerVScreen = screenHeightM;
	/* 477:555 */		   float mPerUTexture = textureWidthM;
	/* 478:556 */		   float mPerVTexture = textureHeightM;
	/* 479:    */       
	/* 480:558 */		   float[] vertexData = new float[8000];
	/* 481:559 */		   int vertexOffset = 0;
	/* 482:568 */		   for (int row = 0; row < 40; row++)
		   {
	/* 483:569 */			 for (int col = 0; col < 40; col++)
	/* 484:    */
			 {	/* 485:576 */			   float uTexture = col / 39.0F * (viewportWidthMTexture / textureWidthM) + viewportXMTexture / textureWidthM;
	/* 486:    */           
	/* 487:    */ 
	/* 488:579 */			   float vTexture = row / 39.0F * (viewportHeightMTexture / textureHeightM) + viewportYMTexture / textureHeightM;
	/* 489:    */           
	/* 490:    */ 
	/* 491:    */ 
	/* 492:583 */			   float xTexture = uTexture * mPerUTexture;
	/* 493:584 */			   float yTexture = vTexture * mPerVTexture;
	/* 494:585 */			   float xTextureEye = xTexture - xEyeOffsetMTexture;
	/* 495:586 */			   float yTextureEye = yTexture - yEyeOffsetMTexture;
	/* 496:587 */			   float rTexture = (float)Math.Sqrt(xTextureEye * xTextureEye + yTextureEye * yTextureEye);
	/* 497:    */           
	/* 498:589 */			   float textureToScreen = rTexture > 0.0F ? distortion.distortInverse(rTexture) / rTexture : 1.0F;
	/* 499:    */           
	/* 500:    */ 
	/* 501:592 */			   float xScreen = xTextureEye * textureToScreen + xEyeOffsetMScreen;
	/* 502:593 */			   float yScreen = yTextureEye * textureToScreen + yEyeOffsetMScreen;
	/* 503:594 */			   float uScreen = xScreen / mPerUScreen;
	/* 504:595 */			   float vScreen = yScreen / mPerVScreen;
	/* 505:596 */			   float vignetteSizeMTexture = 0.002F / textureToScreen;
	/* 506:    */           
	/* 507:    */ 
	/* 508:599 */			   float dxTexture = xTexture - DistortionRenderer.clamp(xTexture, viewportXMTexture + vignetteSizeMTexture, viewportXMTexture + viewportWidthMTexture - vignetteSizeMTexture);
	/* 509:    */           
	/* 510:    */ 
	/* 511:    */ 
	/* 512:603 */			   float dyTexture = yTexture - DistortionRenderer.clamp(yTexture, viewportYMTexture + vignetteSizeMTexture, viewportYMTexture + viewportHeightMTexture - vignetteSizeMTexture);
	/* 513:    */           
	/* 514:    */ 
	/* 515:    */ 
	/* 516:607 */			   float drTexture = (float)Math.Sqrt(dxTexture * dxTexture + dyTexture * dyTexture);
	/* 517:    */           
	/* 518:609 */			   float vignette = 1.0F - DistortionRenderer.clamp(drTexture / vignetteSizeMTexture, 0.0F, 1.0F);
	/* 519:    */           
	/* 520:611 */			   vertexData[(vertexOffset + 0)] = (2.0F * uScreen - 1.0F);
	/* 521:612 */			   vertexData[(vertexOffset + 1)] = (2.0F * vScreen - 1.0F);
	/* 522:613 */			   vertexData[(vertexOffset + 2)] = vignette;
	/* 523:614 */			   vertexData[(vertexOffset + 3)] = uTexture;
	/* 524:615 */			   vertexData[(vertexOffset + 4)] = vTexture;
	/* 525:    */           
	/* 526:617 */			   vertexOffset += 5;
	/* 527:    */
			 }	/* 528:    */
		   }	/* 529:645 */		   this.nIndices = 3158;
	/* 530:646 */		   int[] indexData = new int[this.nIndices];
	/* 531:647 */		   int indexOffset = 0;
	/* 532:648 */		   vertexOffset = 0;
	/* 533:649 */		   for (int row = 0; row < 39; row++)
	/* 534:    */
		   {	/* 535:650 */			 if (row > 0)
	/* 536:    */
			 {	/* 537:651 */			   indexData[indexOffset] = indexData[(indexOffset - 1)];
	/* 538:652 */			   indexOffset++;
	/* 539:    */
			 }	/* 540:654 */			 for (int col = 0; col < 40; col++)
	/* 541:    */
			 {	/* 542:655 */			   if (col > 0)
			   {
	/* 543:656 */				 if (row % 2 == 0)
				 {
	/* 544:658 */				   vertexOffset++;
	/* 545:    */
				 }				 else
				 {
	/* 546:661 */				   vertexOffset--;
	/* 547:    */
				 }	/* 548:    */
			   }	/* 549:664 */			   indexData[(indexOffset++)] = vertexOffset;
	/* 550:665 */			   indexData[(indexOffset++)] = (vertexOffset + 40);
	/* 551:    */
			 }	/* 552:667 */			 vertexOffset += 40;
	/* 553:    */
		   }	/* 554:670 */		   FloatBuffer vertexBuffer = ByteBuffer.allocateDirect(vertexData.Length * 4).order(ByteOrder.nativeOrder()).asFloatBuffer();
	/* 555:    */       
	/* 556:    */ 
	/* 557:673 */		   vertexBuffer.put(vertexData).position(0);
	/* 558:    */       
	/* 559:675 */		   IntBuffer indexBuffer = ByteBuffer.allocateDirect(indexData.Length * 4).order(ByteOrder.nativeOrder()).asIntBuffer();
	/* 560:    */       
	/* 561:    */ 
	/* 562:678 */		   indexBuffer.put(indexData).position(0);
	/* 563:    */       
	/* 564:680 */		   int[] bufferIds = new int[2];
	/* 565:681 */		   GLES20.glGenBuffers(2, bufferIds, 0);
	/* 566:682 */		   this.mArrayBufferId = bufferIds[0];
	/* 567:683 */		   this.mElementBufferId = bufferIds[1];
	/* 568:    */       
	/* 569:685 */		   GLES20.glBindBuffer(34962, this.mArrayBufferId);
	/* 570:686 */		   GLES20.glBufferData(34962, vertexData.Length * 4, vertexBuffer, 35044);
	/* 571:    */       
	/* 572:    */ 
	/* 573:689 */		   GLES20.glBindBuffer(34963, this.mElementBufferId);
	/* 574:690 */		   GLES20.glBufferData(34963, indexData.Length * 4, indexBuffer, 35044);
	/* 575:    */       
	/* 576:    */ 
	/* 577:693 */		   GLES20.glBindBuffer(34962, 0);
	/* 578:694 */		   GLES20.glBindBuffer(34963, 0);
	/* 579:    */
		 }	/* 580:    */
	   }	/* 581:    */
	 }

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     com.google.vrtoolkit.cardboard.DistortionRenderer
	 * JD-Core Version:    0.7.0.1
	 */
 }