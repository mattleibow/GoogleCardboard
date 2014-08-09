using System;

/*   1:    */ namespace com.google.vrtoolkit.cardboard.sensors.@internal
 {
	/*   2:    */ 
	/*   3:    */	 public class OrientationEKF
	/*   4:    */
	 {	/*   5:    */	   private static readonly float NS2S = 1.0E-0x09F;
	/*   6: 19 */	   private double[] rotationMatrix = new double[16];
	/*   7: 21 */	   private Matrix3x3d so3SensorFromWorld = new Matrix3x3d();
	/*   8: 22 */	   private Matrix3x3d so3LastMotion = new Matrix3x3d();
	/*   9: 23 */	   private Matrix3x3d mP = new Matrix3x3d();
	/*  10: 24 */	   private Matrix3x3d mQ = new Matrix3x3d();
	/*  11: 25 */	   private Matrix3x3d mR = new Matrix3x3d();
	/*  12: 26 */	   private Matrix3x3d mRaccel = new Matrix3x3d();
	/*  13: 27 */	   private Matrix3x3d mS = new Matrix3x3d();
	/*  14: 28 */	   private Matrix3x3d mH = new Matrix3x3d();
	/*  15: 29 */	   private Matrix3x3d mK = new Matrix3x3d();
	/*  16: 30 */	   private Vector3d mNu = new Vector3d();
	/*  17: 31 */	   private Vector3d mz = new Vector3d();
	/*  18: 32 */	   private Vector3d mh = new Vector3d();
	/*  19: 33 */	   private Vector3d mu = new Vector3d();
	/*  20: 34 */	   private Vector3d mx = new Vector3d();
	/*  21: 35 */	   private Vector3d down = new Vector3d();
	/*  22: 36 */	   private Vector3d north = new Vector3d();
	/*  23:    */	   private long sensorTimeStampGyro;
	/*  24:    */	   private long sensorTimeStampAcc;
	/*  25:    */	   private long sensorTimeStampMag;
	/*  26: 43 */	   private float[] lastGyro = new float[3];
	/*  27:    */	   private float filteredGyroTimestep;
	/*  28: 47 */	   private bool timestepFilterInit = false;
	/*  29:    */	   private int numGyroTimestepSamples;
	/*  30: 49 */	   private bool gyroFilterValid = true;
	/*  31: 54 */	   private Matrix3x3d getPredictedGLMatrixTempM1 = new Matrix3x3d();
	/*  32: 55 */	   private Matrix3x3d getPredictedGLMatrixTempM2 = new Matrix3x3d();
	/*  33: 56 */	   private Vector3d getPredictedGLMatrixTempV1 = new Vector3d();
	/*  34: 59 */	   private Matrix3x3d setHeadingDegreesTempM1 = new Matrix3x3d();
	/*  35: 62 */	   private Matrix3x3d processGyroTempM1 = new Matrix3x3d();
	/*  36: 63 */	   private Matrix3x3d processGyroTempM2 = new Matrix3x3d();
	/*  37: 66 */	   private Matrix3x3d processAccTempM1 = new Matrix3x3d();
	/*  38: 67 */	   private Matrix3x3d processAccTempM2 = new Matrix3x3d();
	/*  39: 68 */	   private Matrix3x3d processAccTempM3 = new Matrix3x3d();
	/*  40: 69 */	   private Matrix3x3d processAccTempM4 = new Matrix3x3d();
	/*  41: 70 */	   private Matrix3x3d processAccTempM5 = new Matrix3x3d();
	/*  42: 71 */	   private Vector3d processAccTempV1 = new Vector3d();
	/*  43: 72 */	   private Vector3d processAccTempV2 = new Vector3d();
	/*  44: 73 */	   private Vector3d processAccVDelta = new Vector3d();
	/*  45: 76 */	   private Vector3d processMagTempV1 = new Vector3d();
	/*  46: 77 */	   private Vector3d processMagTempV2 = new Vector3d();
	/*  47: 78 */	   private Vector3d processMagTempV3 = new Vector3d();
	/*  48: 79 */	   private Vector3d processMagTempV4 = new Vector3d();
	/*  49: 80 */	   private Vector3d processMagTempV5 = new Vector3d();
	/*  50: 81 */	   private Matrix3x3d processMagTempM1 = new Matrix3x3d();
	/*  51: 82 */	   private Matrix3x3d processMagTempM2 = new Matrix3x3d();
	/*  52: 83 */	   private Matrix3x3d processMagTempM4 = new Matrix3x3d();
	/*  53: 84 */	   private Matrix3x3d processMagTempM5 = new Matrix3x3d();
	/*  54: 85 */	   private Matrix3x3d processMagTempM6 = new Matrix3x3d();
	/*  55: 88 */	   private Matrix3x3d updateCovariancesAfterMotionTempM1 = new Matrix3x3d();
	/*  56: 89 */	   private Matrix3x3d updateCovariancesAfterMotionTempM2 = new Matrix3x3d();
	/*  57: 92 */	   private Matrix3x3d accObservationFunctionForNumericalJacobianTempM = new Matrix3x3d();
	/*  58: 96 */	   private Matrix3x3d magObservationFunctionForNumericalJacobianTempM = new Matrix3x3d();
	/*  59:    */   
	/*  60:    */	   public OrientationEKF()
	/*  61:    */
	   {	/*  62:101 */		 reset();
	/*  63:    */
	   }	/*  64:    */   
	/*  65:    */	   public virtual void reset()
	/*  66:    */
	   {	/*  67:108 */		 this.sensorTimeStampGyro = 0L;
	/*  68:109 */		 this.sensorTimeStampAcc = 0L;
	/*  69:110 */		 this.sensorTimeStampMag = 0L;
	/*  70:    */     
	/*  71:112 */		 this.so3SensorFromWorld.setIdentity();
	/*  72:113 */		 this.so3LastMotion.setIdentity();
	/*  73:    */     
	/*  74:    */ 
	/*  75:116 */		 double initialSigmaP = 5.0D;
	/*  76:    */     
	/*  77:118 */		 this.mP.setZero();
	/*  78:119 */		 this.mP.SameDiagonal = 25.0D;
	/*  79:    */     
	/*  80:    */ 
	/*  81:122 */		 double initialSigmaQ = 1.0D;
	/*  82:123 */		 this.mQ.setZero();
	/*  83:124 */		 this.mQ.SameDiagonal = 1.0D;
	/*  84:    */     
	/*  85:    */ 
	/*  86:127 */		 double initialSigmaR = 0.25D;
	/*  87:128 */		 this.mR.setZero();
	/*  88:129 */		 this.mR.SameDiagonal = 0.0625D;
	/*  89:    */     
	/*  90:    */ 
	/*  91:132 */		 double initialSigmaRaccel = 0.75D;
	/*  92:133 */		 this.mRaccel.setZero();
	/*  93:134 */		 this.mRaccel.SameDiagonal = 0.5625D;
	/*  94:    */     
	/*  95:136 */		 this.mS.setZero();
	/*  96:137 */		 this.mH.setZero();
	/*  97:138 */		 this.mK.setZero();
	/*  98:139 */		 this.mNu.setZero();
	/*  99:140 */		 this.mz.setZero();
	/* 100:141 */		 this.mh.setZero();
	/* 101:142 */		 this.mu.setZero();
	/* 102:143 */		 this.mx.setZero();
	/* 103:    */     
	/* 104:145 */		 this.down.set(0.0D, 0.0D, 9.810000000000001D);
	/* 105:146 */		 this.north.set(0.0D, 1.0D, 0.0D);
	/* 106:    */
	   }	/* 107:    */   
	/* 108:    */
	   public virtual bool Ready
	   {
		   get
		/* 109:    */
		   {		/* 110:154 */			 return this.sensorTimeStampAcc != 0L;
		/* 111:    */
		   }
	   }	/* 112:    */   
	/* 113:    */
	   public virtual double HeadingDegrees
	   {
		   get
		/* 114:    */
		   {		/* 115:163 */			 double x = this.so3SensorFromWorld.get(2, 0);
		/* 116:164 */			 double y = this.so3SensorFromWorld.get(2, 1);
		/* 117:165 */			 double mag = Math.Sqrt(x * x + y * y);
		/* 118:167 */			 if (mag < 0.1D)
			 {
		/* 119:168 */			   return 0.0D;
		/* 120:    */
			 }		/* 121:171 */			 double heading = -90.0D - Math.Atan2(y, x) / 3.141592653589793D * 180.0D;
		/* 122:172 */			 if (heading < 0.0D)
			 {
		/* 123:173 */			   heading += 360.0D;
		/* 124:    */
			 }		/* 125:175 */			 if (heading >= 360.0D)
			 {
		/* 126:176 */			   heading -= 360.0D;
		/* 127:    */
			 }		/* 128:178 */			 return heading;
		/* 129:    */
		   }		   set
		/* 132:    */
		   {			   lock (this)
			   {
			/* 133:187 */				 double currentHeading = HeadingDegrees;
			/* 134:188 */				 double deltaHeading = value - currentHeading;
			/* 135:189 */				 double s = Math.Sin(deltaHeading / 180.0D * 3.141592653589793D);
			/* 136:190 */				 double c = Math.Cos(deltaHeading / 180.0D * 3.141592653589793D);
			/* 137:    */     
			/* 138:192 */
				 double[][] deltaHeadingRotationVals = new double[][]
				 {
					 new double[] {c, -s, 0.0D},
					 new double[] {s, c, 0.0D},
					 new double[] {0.0D, 0.0D, 1.0D}
				 };
			/* 139:    */     
			/* 140:194 */				 arrayAssign(deltaHeadingRotationVals, this.setHeadingDegreesTempM1);
			/* 141:195 */				 Matrix3x3d.mult(this.so3SensorFromWorld, this.setHeadingDegreesTempM1, this.so3SensorFromWorld);
			/* 142:    */
			   }
		   }
	   }	/* 143:    */   
	/* 144:    */
	   public virtual double[] GLMatrix
	   {
		   get
		/* 145:    */
		   {		/* 146:206 */			 return glMatrixFromSo3(this.so3SensorFromWorld);
		/* 147:    */
		   }
	   }	/* 148:    */   
	/* 149:    */	   public virtual double[] getPredictedGLMatrix(double secondsAfterLastGyroEvent)
	/* 150:    */
	   {	/* 151:220 */		 double dT = secondsAfterLastGyroEvent;
	/* 152:221 */		 Vector3d pmu = this.getPredictedGLMatrixTempV1;
	/* 153:222 */		 pmu.set(this.lastGyro[0] * -dT, this.lastGyro[1] * -dT, this.lastGyro[2] * -dT);
	/* 154:223 */		 Matrix3x3d so3PredictedMotion = this.getPredictedGLMatrixTempM1;
	/* 155:224 */		 So3Util.sO3FromMu(pmu, so3PredictedMotion);
	/* 156:    */     
	/* 157:226 */		 Matrix3x3d so3PredictedState = this.getPredictedGLMatrixTempM2;
	/* 158:227 */		 Matrix3x3d.mult(so3PredictedMotion, this.so3SensorFromWorld, so3PredictedState);
	/* 159:    */     
	/* 160:229 */		 return glMatrixFromSo3(so3PredictedState);
	/* 161:    */
	   }	/* 162:    */   
	/* 163:    */	   private double[] glMatrixFromSo3(Matrix3x3d so3)
	/* 164:    */
	   {	/* 165:234 */		 for (int r = 0; r < 3; r++)
		 {
	/* 166:235 */		   for (int c = 0; c < 3; c++)
		   {
	/* 167:237 */			 this.rotationMatrix[(4 * c + r)] = so3.get(r, c);
	/* 168:    */
		   }	/* 169:    */
		 }	/* 170:242 */		 double tmp62_61 = (this.rotationMatrix[11] = 0.0D);
		 this.rotationMatrix[7] = tmp62_61;
		 this.rotationMatrix[3] = tmp62_61;
		 double tmp86_85 = (this.rotationMatrix[14] = 0.0D);
	/* 171:243 */
		 this.rotationMatrix[13] = tmp86_85;
		 this.rotationMatrix[12] = tmp86_85;
	/* 172:    */     
	/* 173:    */ 
	/* 174:246 */		 this.rotationMatrix[15] = 1.0D;
	/* 175:    */     
	/* 176:248 */		 return this.rotationMatrix;
	/* 177:    */
	   }	/* 178:    */   
	/* 179:    */	   public virtual void processGyro(float[] gyro, long sensorTimeStamp)
	/* 180:    */
	   {		   lock (this)
		   {
		/* 181:258 */			 float kTimeThreshold = 0.04F;
		/* 182:259 */			 float kdTdefault = 0.01F;
		/* 183:260 */			 if (this.sensorTimeStampGyro != 0L)
		/* 184:    */
			 {		/* 185:261 */			   float dT = (float)(sensorTimeStamp - this.sensorTimeStampGyro) * 1.0E-0x09F;
		/* 186:262 */			   if (dT > 0.04F)
			   {
		/* 187:263 */				 dT = this.gyroFilterValid ? this.filteredGyroTimestep : 0.01F;
		/* 188:    */
			   }			   else
			   {
		/* 189:265 */				 filterGyroTimestep(dT);
		/* 190:    */
			   }		/* 191:268 */			   this.mu.set(gyro[0] * -dT, gyro[1] * -dT, gyro[2] * -dT);
		/* 192:269 */			   So3Util.sO3FromMu(this.mu, this.so3LastMotion);
		/* 193:    */       
		/* 194:271 */			   this.processGyroTempM1.set(this.so3SensorFromWorld);
		/* 195:272 */			   Matrix3x3d.mult(this.so3LastMotion, this.so3SensorFromWorld, this.processGyroTempM1);
		/* 196:273 */			   this.so3SensorFromWorld.set(this.processGyroTempM1);
		/* 197:    */       
		/* 198:275 */			   updateCovariancesAfterMotion();
		/* 199:    */       
		/* 200:277 */			   this.processGyroTempM2.set(this.mQ);
		/* 201:278 */			   this.processGyroTempM2.scale(dT * dT);
		/* 202:279 */			   this.mP.plusEquals(this.processGyroTempM2);
		/* 203:    */
			 }		/* 204:281 */			 this.sensorTimeStampGyro = sensorTimeStamp;
		/* 205:282 */			 this.lastGyro[0] = gyro[0];
		/* 206:283 */			 this.lastGyro[1] = gyro[1];
		/* 207:284 */			 this.lastGyro[2] = gyro[2];
		/* 208:    */
		   }
	   }	/* 209:    */   
	/* 210:    */	   public virtual void processAcc(float[] acc, long sensorTimeStamp)
	/* 211:    */
	   {		   lock (this)
		   {
		/* 212:295 */			 this.mz.set(acc[0], acc[1], acc[2]);
		/* 213:297 */			 if (this.sensorTimeStampAcc != 0L)
		/* 214:    */
			 {		/* 215:298 */			   accObservationFunctionForNumericalJacobian(this.so3SensorFromWorld, this.mNu);
		/* 216:    */       
		/* 217:    */ 
		/* 218:301 */			   double eps = 1.0E-0x7D;
		/* 219:302 */			   for (int dof = 0; dof < 3; dof++)
		/* 220:    */
			   {		/* 221:303 */				 Vector3d delta = this.processAccVDelta;
		/* 222:304 */				 delta.setZero();
		/* 223:305 */				 delta.setComponent(dof, eps);
		/* 224:    */         
		/* 225:307 */				 So3Util.sO3FromMu(delta, this.processAccTempM1);
		/* 226:308 */				 Matrix3x3d.mult(this.processAccTempM1, this.so3SensorFromWorld, this.processAccTempM2);
		/* 227:    */         
		/* 228:310 */				 accObservationFunctionForNumericalJacobian(this.processAccTempM2, this.processAccTempV1);
		/* 229:    */         
		/* 230:312 */				 Vector3d withDelta = this.processAccTempV1;
		/* 231:    */         
		/* 232:314 */				 Vector3d.sub(this.mNu, withDelta, this.processAccTempV2);
		/* 233:315 */				 this.processAccTempV2.scale(1.0D / eps);
		/* 234:316 */				 this.mH.setColumn(dof, this.processAccTempV2);
		/* 235:    */
			   }		/* 236:320 */			   this.mH.transpose(this.processAccTempM3);
		/* 237:321 */			   Matrix3x3d.mult(this.mP, this.processAccTempM3, this.processAccTempM4);
		/* 238:322 */			   Matrix3x3d.mult(this.mH, this.processAccTempM4, this.processAccTempM5);
		/* 239:323 */			   Matrix3x3d.add(this.processAccTempM5, this.mRaccel, this.mS);
		/* 240:    */       
		/* 241:    */ 
		/* 242:326 */			   this.mS.invert(this.processAccTempM3);
		/* 243:327 */			   this.mH.transpose(this.processAccTempM4);
		/* 244:328 */			   Matrix3x3d.mult(this.processAccTempM4, this.processAccTempM3, this.processAccTempM5);
		/* 245:329 */			   Matrix3x3d.mult(this.mP, this.processAccTempM5, this.mK);
		/* 246:    */       
		/* 247:    */ 
		/* 248:332 */			   Matrix3x3d.mult(this.mK, this.mNu, this.mx);
		/* 249:    */       
		/* 250:    */ 
		/* 251:335 */			   Matrix3x3d.mult(this.mK, this.mH, this.processAccTempM3);
		/* 252:336 */			   this.processAccTempM4.setIdentity();
		/* 253:337 */			   this.processAccTempM4.minusEquals(this.processAccTempM3);
		/* 254:338 */			   Matrix3x3d.mult(this.processAccTempM4, this.mP, this.processAccTempM3);
		/* 255:339 */			   this.mP.set(this.processAccTempM3);
		/* 256:    */       
		/* 257:341 */			   So3Util.sO3FromMu(this.mx, this.so3LastMotion);
		/* 258:    */       
		/* 259:343 */			   Matrix3x3d.mult(this.so3LastMotion, this.so3SensorFromWorld, this.so3SensorFromWorld);
		/* 260:    */       
		/* 261:345 */			   updateCovariancesAfterMotion();
		/* 262:    */
			 }		/* 263:    */			 else
		/* 264:    */
			 {		/* 265:350 */			   So3Util.sO3FromTwoVec(this.down, this.mz, this.so3SensorFromWorld);
		/* 266:    */
			 }		/* 267:352 */			 this.sensorTimeStampAcc = sensorTimeStamp;
		/* 268:    */
		   }
	   }	/* 269:    */   
	/* 270:    */	   public virtual void processMag(float[] mag, long sensorTimeStamp)
	/* 271:    */
	   {	/* 272:362 */		 this.mz.set(mag[0], mag[1], mag[2]);
	/* 273:363 */		 this.mz.normalize();
	/* 274:    */     
	/* 275:365 */		 Vector3d downInSensorFrame = new Vector3d();
	/* 276:366 */		 this.so3SensorFromWorld.getColumn(2, downInSensorFrame);
	/* 277:    */     
	/* 278:368 */		 Vector3d.cross(this.mz, downInSensorFrame, this.processMagTempV1);
	/* 279:369 */		 Vector3d perpToDownAndMag = this.processMagTempV1;
	/* 280:370 */		 perpToDownAndMag.normalize();
	/* 281:    */     
	/* 282:372 */		 Vector3d.cross(downInSensorFrame, perpToDownAndMag, this.processMagTempV2);
	/* 283:373 */		 Vector3d magHorizontal = this.processMagTempV2;
	/* 284:    */     
	/* 285:    */ 
	/* 286:376 */		 magHorizontal.normalize();
	/* 287:377 */		 this.mz.set(magHorizontal);
	/* 288:379 */		 if (this.sensorTimeStampMag != 0L)
	/* 289:    */
		 {	/* 290:380 */		   magObservationFunctionForNumericalJacobian(this.so3SensorFromWorld, this.mNu);
	/* 291:    */       
	/* 292:    */ 
	/* 293:383 */		   double eps = 1.0E-0x7D;
	/* 294:384 */		   for (int dof = 0; dof < 3; dof++)
	/* 295:    */
		   {	/* 296:385 */			 Vector3d delta = this.processMagTempV3;
	/* 297:386 */			 delta.setZero();
	/* 298:387 */			 delta.setComponent(dof, eps);
	/* 299:    */         
	/* 300:389 */			 So3Util.sO3FromMu(delta, this.processMagTempM1);
	/* 301:390 */			 Matrix3x3d.mult(this.processMagTempM1, this.so3SensorFromWorld, this.processMagTempM2);
	/* 302:    */         
	/* 303:392 */			 magObservationFunctionForNumericalJacobian(this.processMagTempM2, this.processMagTempV4);
	/* 304:    */         
	/* 305:394 */			 Vector3d withDelta = this.processMagTempV4;
	/* 306:    */         
	/* 307:396 */			 Vector3d.sub(this.mNu, withDelta, this.processMagTempV5);
	/* 308:397 */			 this.processMagTempV5.scale(1.0D / eps);
	/* 309:    */         
	/* 310:399 */			 this.mH.setColumn(dof, this.processMagTempV5);
	/* 311:    */
		   }	/* 312:403 */		   this.mH.transpose(this.processMagTempM4);
	/* 313:404 */		   Matrix3x3d.mult(this.mP, this.processMagTempM4, this.processMagTempM5);
	/* 314:405 */		   Matrix3x3d.mult(this.mH, this.processMagTempM5, this.processMagTempM6);
	/* 315:406 */		   Matrix3x3d.add(this.processMagTempM6, this.mR, this.mS);
	/* 316:    */       
	/* 317:    */ 
	/* 318:409 */		   this.mS.invert(this.processMagTempM4);
	/* 319:410 */		   this.mH.transpose(this.processMagTempM5);
	/* 320:411 */		   Matrix3x3d.mult(this.processMagTempM5, this.processMagTempM4, this.processMagTempM6);
	/* 321:412 */		   Matrix3x3d.mult(this.mP, this.processMagTempM6, this.mK);
	/* 322:    */       
	/* 323:    */ 
	/* 324:415 */		   Matrix3x3d.mult(this.mK, this.mNu, this.mx);
	/* 325:    */       
	/* 326:    */ 
	/* 327:418 */		   Matrix3x3d.mult(this.mK, this.mH, this.processMagTempM4);
	/* 328:419 */		   this.processMagTempM5.setIdentity();
	/* 329:420 */		   this.processMagTempM5.minusEquals(this.processMagTempM4);
	/* 330:421 */		   Matrix3x3d.mult(this.processMagTempM5, this.mP, this.processMagTempM4);
	/* 331:422 */		   this.mP.set(this.processMagTempM4);
	/* 332:    */       
	/* 333:424 */		   So3Util.sO3FromMu(this.mx, this.so3LastMotion);
	/* 334:    */       
	/* 335:426 */		   Matrix3x3d.mult(this.so3LastMotion, this.so3SensorFromWorld, this.processMagTempM4);
	/* 336:427 */		   this.so3SensorFromWorld.set(this.processMagTempM4);
	/* 337:    */       
	/* 338:429 */		   updateCovariancesAfterMotion();
	/* 339:    */
		 }	/* 340:    */		 else
	/* 341:    */
		 {	/* 342:433 */		   magObservationFunctionForNumericalJacobian(this.so3SensorFromWorld, this.mNu);
	/* 343:434 */		   So3Util.sO3FromMu(this.mx, this.so3LastMotion);
	/* 344:    */       
	/* 345:436 */		   Matrix3x3d.mult(this.so3LastMotion, this.so3SensorFromWorld, this.processMagTempM4);
	/* 346:437 */		   this.so3SensorFromWorld.set(this.processMagTempM4);
	/* 347:    */       
	/* 348:439 */		   updateCovariancesAfterMotion();
	/* 349:    */
		 }	/* 350:441 */		 this.sensorTimeStampMag = sensorTimeStamp;
	/* 351:    */
	   }	/* 352:    */   
	/* 353:    */	   private void filterGyroTimestep(float timeStep)
	/* 354:    */
	   {	/* 355:451 */		 float kFilterCoeff = 0.95F;
	/* 356:452 */		 float kMinSamples = 10.0F;
	/* 357:453 */		 if (!this.timestepFilterInit)
	/* 358:    */
		 {	/* 359:454 */		   this.filteredGyroTimestep = timeStep;
	/* 360:455 */		   this.numGyroTimestepSamples = 1;
	/* 361:456 */		   this.timestepFilterInit = true;
	/* 362:    */
		 }	/* 363:    */		 else
	/* 364:    */
		 {	/* 365:459 */		   this.filteredGyroTimestep = (0.95F * this.filteredGyroTimestep + 0.05000001F * timeStep);
	/* 366:461 */		   if (++this.numGyroTimestepSamples > 10.0F)
		   {
	/* 367:462 */			 this.gyroFilterValid = true;
	/* 368:    */
		   }	/* 369:    */
		 }	/* 370:    */
	   }	/* 371:    */   
	/* 372:    */	   private void updateCovariancesAfterMotion()
	/* 373:    */
	   {	/* 374:468 */		 this.so3LastMotion.transpose(this.updateCovariancesAfterMotionTempM1);
	/* 375:469 */		 Matrix3x3d.mult(this.mP, this.updateCovariancesAfterMotionTempM1, this.updateCovariancesAfterMotionTempM2);
	/* 376:    */     
	/* 377:471 */		 Matrix3x3d.mult(this.so3LastMotion, this.updateCovariancesAfterMotionTempM2, this.mP);
	/* 378:472 */		 this.so3LastMotion.setIdentity();
	/* 379:    */
	   }	/* 380:    */   
	/* 381:    */	   private void accObservationFunctionForNumericalJacobian(Matrix3x3d so3SensorFromWorldPred, Vector3d result)
	/* 382:    */
	   {	/* 383:481 */		 Matrix3x3d.mult(so3SensorFromWorldPred, this.down, this.mh);
	/* 384:482 */		 So3Util.sO3FromTwoVec(this.mh, this.mz, this.accObservationFunctionForNumericalJacobianTempM);
	/* 385:    */     
	/* 386:    */ 
	/* 387:485 */		 So3Util.muFromSO3(this.accObservationFunctionForNumericalJacobianTempM, result);
	/* 388:    */
	   }	/* 389:    */   
	/* 390:    */	   private void magObservationFunctionForNumericalJacobian(Matrix3x3d so3SensorFromWorldPred, Vector3d result)
	/* 391:    */
	   {	/* 392:497 */		 Matrix3x3d.mult(so3SensorFromWorldPred, this.north, this.mh);
	/* 393:498 */		 So3Util.sO3FromTwoVec(this.mh, this.mz, this.magObservationFunctionForNumericalJacobianTempM);
	/* 394:    */     
	/* 395:500 */		 So3Util.muFromSO3(this.magObservationFunctionForNumericalJacobianTempM, result);
	/* 396:    */
	   }	/* 397:    */   
	/* 398:    */	   public static void arrayAssign(double[][] data, Matrix3x3d m)
	/* 399:    */
	   {	/* 400:510 */		 assert(3 == data.Length);
	/* 401:511 */		 assert(3 == data[0].Length);
	/* 402:512 */		 assert(3 == data[1].Length);
	/* 403:513 */		 assert(3 == data[2].Length);
	/* 404:514 */		 m.set(data[0][0], data[0][1], data[0][2], data[1][0], data[1][1], data[1][2], data[2][0], data[2][1], data[2][2]);
	/* 405:    */
	   }	/* 406:    */
	 }

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     com.google.vrtoolkit.cardboard.sensors.internal.OrientationEKF
	 * JD-Core Version:    0.7.0.1
	 */
 }