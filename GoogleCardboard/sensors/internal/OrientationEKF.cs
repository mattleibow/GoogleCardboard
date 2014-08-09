using System;

 namespace com.google.vrtoolkit.cardboard.sensors.@internal
 {
	 
		 public class OrientationEKF
	
	 {		   private static readonly float NS2S = 1.0E-0x09F;
		   private double[] rotationMatrix = new double[16];
		   private Matrix3x3d so3SensorFromWorld = new Matrix3x3d();
		   private Matrix3x3d so3LastMotion = new Matrix3x3d();
		   private Matrix3x3d mP = new Matrix3x3d();
		   private Matrix3x3d mQ = new Matrix3x3d();
		   private Matrix3x3d mR = new Matrix3x3d();
		   private Matrix3x3d mRaccel = new Matrix3x3d();
		   private Matrix3x3d mS = new Matrix3x3d();
		   private Matrix3x3d mH = new Matrix3x3d();
		   private Matrix3x3d mK = new Matrix3x3d();
		   private Vector3d mNu = new Vector3d();
		   private Vector3d mz = new Vector3d();
		   private Vector3d mh = new Vector3d();
		   private Vector3d mu = new Vector3d();
		   private Vector3d mx = new Vector3d();
		   private Vector3d down = new Vector3d();
		   private Vector3d north = new Vector3d();
		   private long sensorTimeStampGyro;
		   private long sensorTimeStampAcc;
		   private long sensorTimeStampMag;
		   private float[] lastGyro = new float[3];
		   private float filteredGyroTimestep;
		   private bool timestepFilterInit = false;
		   private int numGyroTimestepSamples;
		   private bool gyroFilterValid = true;
		   private Matrix3x3d getPredictedGLMatrixTempM1 = new Matrix3x3d();
		   private Matrix3x3d getPredictedGLMatrixTempM2 = new Matrix3x3d();
		   private Vector3d getPredictedGLMatrixTempV1 = new Vector3d();
		   private Matrix3x3d setHeadingDegreesTempM1 = new Matrix3x3d();
		   private Matrix3x3d processGyroTempM1 = new Matrix3x3d();
		   private Matrix3x3d processGyroTempM2 = new Matrix3x3d();
		   private Matrix3x3d processAccTempM1 = new Matrix3x3d();
		   private Matrix3x3d processAccTempM2 = new Matrix3x3d();
		   private Matrix3x3d processAccTempM3 = new Matrix3x3d();
		   private Matrix3x3d processAccTempM4 = new Matrix3x3d();
		   private Matrix3x3d processAccTempM5 = new Matrix3x3d();
		   private Vector3d processAccTempV1 = new Vector3d();
		   private Vector3d processAccTempV2 = new Vector3d();
		   private Vector3d processAccVDelta = new Vector3d();
		   private Vector3d processMagTempV1 = new Vector3d();
		   private Vector3d processMagTempV2 = new Vector3d();
		   private Vector3d processMagTempV3 = new Vector3d();
		   private Vector3d processMagTempV4 = new Vector3d();
		   private Vector3d processMagTempV5 = new Vector3d();
		   private Matrix3x3d processMagTempM1 = new Matrix3x3d();
		   private Matrix3x3d processMagTempM2 = new Matrix3x3d();
		   private Matrix3x3d processMagTempM4 = new Matrix3x3d();
		   private Matrix3x3d processMagTempM5 = new Matrix3x3d();
		   private Matrix3x3d processMagTempM6 = new Matrix3x3d();
		   private Matrix3x3d updateCovariancesAfterMotionTempM1 = new Matrix3x3d();
		   private Matrix3x3d updateCovariancesAfterMotionTempM2 = new Matrix3x3d();
		   private Matrix3x3d accObservationFunctionForNumericalJacobianTempM = new Matrix3x3d();
		   private Matrix3x3d magObservationFunctionForNumericalJacobianTempM = new Matrix3x3d();
	   
		   public OrientationEKF()
	
	   {			 reset();
	
	   }	   
		   public virtual void reset()
	
	   {			 this.sensorTimeStampGyro = 0L;
			 this.sensorTimeStampAcc = 0L;
			 this.sensorTimeStampMag = 0L;
	     
			 this.so3SensorFromWorld.setIdentity();
			 this.so3LastMotion.setIdentity();
	     
	 
			 double initialSigmaP = 5.0D;
	     
			 this.mP.setZero();
			 this.mP.SameDiagonal = 25.0D;
	     
	 
			 double initialSigmaQ = 1.0D;
			 this.mQ.setZero();
			 this.mQ.SameDiagonal = 1.0D;
	     
	 
			 double initialSigmaR = 0.25D;
			 this.mR.setZero();
			 this.mR.SameDiagonal = 0.0625D;
	     
	 
			 double initialSigmaRaccel = 0.75D;
			 this.mRaccel.setZero();
			 this.mRaccel.SameDiagonal = 0.5625D;
	     
			 this.mS.setZero();
			 this.mH.setZero();
			 this.mK.setZero();
			 this.mNu.setZero();
			 this.mz.setZero();
			 this.mh.setZero();
			 this.mu.setZero();
			 this.mx.setZero();
	     
			 this.down.set(0.0D, 0.0D, 9.810000000000001D);
			 this.north.set(0.0D, 1.0D, 0.0D);
	
	   }	   
	
	   public virtual bool Ready
	   {
		   get
		
		   {					 return this.sensorTimeStampAcc != 0L;
		
		   }
	   }	   
	
	   public virtual double HeadingDegrees
	   {
		   get
		
		   {					 double x = this.so3SensorFromWorld.get(2, 0);
					 double y = this.so3SensorFromWorld.get(2, 1);
					 double mag = Math.Sqrt(x * x + y * y);
					 if (mag < 0.1D)
			 {
					   return 0.0D;
		
			 }					 double heading = -90.0D - Math.Atan2(y, x) / 3.141592653589793D * 180.0D;
					 if (heading < 0.0D)
			 {
					   heading += 360.0D;
		
			 }					 if (heading >= 360.0D)
			 {
					   heading -= 360.0D;
		
			 }					 return heading;
		
		   }		   set
		
		   {			   lock (this)
			   {
							 double currentHeading = HeadingDegrees;
							 double deltaHeading = value - currentHeading;
							 double s = Math.Sin(deltaHeading / 180.0D * 3.141592653589793D);
							 double c = Math.Cos(deltaHeading / 180.0D * 3.141592653589793D);
			     
			
				 double[][] deltaHeadingRotationVals = new double[][]
				 {
					 new double[] {c, -s, 0.0D},
					 new double[] {s, c, 0.0D},
					 new double[] {0.0D, 0.0D, 1.0D}
				 };
			     
							 arrayAssign(deltaHeadingRotationVals, this.setHeadingDegreesTempM1);
							 Matrix3x3d.mult(this.so3SensorFromWorld, this.setHeadingDegreesTempM1, this.so3SensorFromWorld);
			
			   }
		   }
	   }	   
	
	   public virtual double[] GLMatrix
	   {
		   get
		
		   {					 return glMatrixFromSo3(this.so3SensorFromWorld);
		
		   }
	   }	   
		   public virtual double[] getPredictedGLMatrix(double secondsAfterLastGyroEvent)
	
	   {			 double dT = secondsAfterLastGyroEvent;
			 Vector3d pmu = this.getPredictedGLMatrixTempV1;
			 pmu.set(this.lastGyro[0] * -dT, this.lastGyro[1] * -dT, this.lastGyro[2] * -dT);
			 Matrix3x3d so3PredictedMotion = this.getPredictedGLMatrixTempM1;
			 So3Util.sO3FromMu(pmu, so3PredictedMotion);
	     
			 Matrix3x3d so3PredictedState = this.getPredictedGLMatrixTempM2;
			 Matrix3x3d.mult(so3PredictedMotion, this.so3SensorFromWorld, so3PredictedState);
	     
			 return glMatrixFromSo3(so3PredictedState);
	
	   }	   
		   private double[] glMatrixFromSo3(Matrix3x3d so3)
	
	   {			 for (int r = 0; r < 3; r++)
		 {
			   for (int c = 0; c < 3; c++)
		   {
				 this.rotationMatrix[(4 * c + r)] = so3.get(r, c);
	
		   }	
		 }			 double tmp62_61 = (this.rotationMatrix[11] = 0.0D);
		 this.rotationMatrix[7] = tmp62_61;
		 this.rotationMatrix[3] = tmp62_61;
		 double tmp86_85 = (this.rotationMatrix[14] = 0.0D);
	
		 this.rotationMatrix[13] = tmp86_85;
		 this.rotationMatrix[12] = tmp86_85;
	     
	 
			 this.rotationMatrix[15] = 1.0D;
	     
			 return this.rotationMatrix;
	
	   }	   
		   public virtual void processGyro(float[] gyro, long sensorTimeStamp)
	
	   {		   lock (this)
		   {
					 float kTimeThreshold = 0.04F;
					 float kdTdefault = 0.01F;
					 if (this.sensorTimeStampGyro != 0L)
		
			 {					   float dT = (float)(sensorTimeStamp - this.sensorTimeStampGyro) * 1.0E-0x09F;
					   if (dT > 0.04F)
			   {
						 dT = this.gyroFilterValid ? this.filteredGyroTimestep : 0.01F;
		
			   }			   else
			   {
						 filterGyroTimestep(dT);
		
			   }					   this.mu.set(gyro[0] * -dT, gyro[1] * -dT, gyro[2] * -dT);
					   So3Util.sO3FromMu(this.mu, this.so3LastMotion);
		       
					   this.processGyroTempM1.set(this.so3SensorFromWorld);
					   Matrix3x3d.mult(this.so3LastMotion, this.so3SensorFromWorld, this.processGyroTempM1);
					   this.so3SensorFromWorld.set(this.processGyroTempM1);
		       
					   updateCovariancesAfterMotion();
		       
					   this.processGyroTempM2.set(this.mQ);
					   this.processGyroTempM2.scale(dT * dT);
					   this.mP.plusEquals(this.processGyroTempM2);
		
			 }					 this.sensorTimeStampGyro = sensorTimeStamp;
					 this.lastGyro[0] = gyro[0];
					 this.lastGyro[1] = gyro[1];
					 this.lastGyro[2] = gyro[2];
		
		   }
	   }	   
		   public virtual void processAcc(float[] acc, long sensorTimeStamp)
	
	   {		   lock (this)
		   {
					 this.mz.set(acc[0], acc[1], acc[2]);
					 if (this.sensorTimeStampAcc != 0L)
		
			 {					   accObservationFunctionForNumericalJacobian(this.so3SensorFromWorld, this.mNu);
		       
		 
					   double eps = 1.0E-0x7D;
					   for (int dof = 0; dof < 3; dof++)
		
			   {						 Vector3d delta = this.processAccVDelta;
						 delta.setZero();
						 delta.setComponent(dof, eps);
		         
						 So3Util.sO3FromMu(delta, this.processAccTempM1);
						 Matrix3x3d.mult(this.processAccTempM1, this.so3SensorFromWorld, this.processAccTempM2);
		         
						 accObservationFunctionForNumericalJacobian(this.processAccTempM2, this.processAccTempV1);
		         
						 Vector3d withDelta = this.processAccTempV1;
		         
						 Vector3d.sub(this.mNu, withDelta, this.processAccTempV2);
						 this.processAccTempV2.scale(1.0D / eps);
						 this.mH.setColumn(dof, this.processAccTempV2);
		
			   }					   this.mH.transpose(this.processAccTempM3);
					   Matrix3x3d.mult(this.mP, this.processAccTempM3, this.processAccTempM4);
					   Matrix3x3d.mult(this.mH, this.processAccTempM4, this.processAccTempM5);
					   Matrix3x3d.add(this.processAccTempM5, this.mRaccel, this.mS);
		       
		 
					   this.mS.invert(this.processAccTempM3);
					   this.mH.transpose(this.processAccTempM4);
					   Matrix3x3d.mult(this.processAccTempM4, this.processAccTempM3, this.processAccTempM5);
					   Matrix3x3d.mult(this.mP, this.processAccTempM5, this.mK);
		       
		 
					   Matrix3x3d.mult(this.mK, this.mNu, this.mx);
		       
		 
					   Matrix3x3d.mult(this.mK, this.mH, this.processAccTempM3);
					   this.processAccTempM4.setIdentity();
					   this.processAccTempM4.minusEquals(this.processAccTempM3);
					   Matrix3x3d.mult(this.processAccTempM4, this.mP, this.processAccTempM3);
					   this.mP.set(this.processAccTempM3);
		       
					   So3Util.sO3FromMu(this.mx, this.so3LastMotion);
		       
					   Matrix3x3d.mult(this.so3LastMotion, this.so3SensorFromWorld, this.so3SensorFromWorld);
		       
					   updateCovariancesAfterMotion();
		
			 }					 else
		
			 {					   So3Util.sO3FromTwoVec(this.down, this.mz, this.so3SensorFromWorld);
		
			 }					 this.sensorTimeStampAcc = sensorTimeStamp;
		
		   }
	   }	   
		   public virtual void processMag(float[] mag, long sensorTimeStamp)
	
	   {			 this.mz.set(mag[0], mag[1], mag[2]);
			 this.mz.normalize();
	     
			 Vector3d downInSensorFrame = new Vector3d();
			 this.so3SensorFromWorld.getColumn(2, downInSensorFrame);
	     
			 Vector3d.cross(this.mz, downInSensorFrame, this.processMagTempV1);
			 Vector3d perpToDownAndMag = this.processMagTempV1;
			 perpToDownAndMag.normalize();
	     
			 Vector3d.cross(downInSensorFrame, perpToDownAndMag, this.processMagTempV2);
			 Vector3d magHorizontal = this.processMagTempV2;
	     
	 
			 magHorizontal.normalize();
			 this.mz.set(magHorizontal);
			 if (this.sensorTimeStampMag != 0L)
	
		 {			   magObservationFunctionForNumericalJacobian(this.so3SensorFromWorld, this.mNu);
	       
	 
			   double eps = 1.0E-0x7D;
			   for (int dof = 0; dof < 3; dof++)
	
		   {				 Vector3d delta = this.processMagTempV3;
				 delta.setZero();
				 delta.setComponent(dof, eps);
	         
				 So3Util.sO3FromMu(delta, this.processMagTempM1);
				 Matrix3x3d.mult(this.processMagTempM1, this.so3SensorFromWorld, this.processMagTempM2);
	         
				 magObservationFunctionForNumericalJacobian(this.processMagTempM2, this.processMagTempV4);
	         
				 Vector3d withDelta = this.processMagTempV4;
	         
				 Vector3d.sub(this.mNu, withDelta, this.processMagTempV5);
				 this.processMagTempV5.scale(1.0D / eps);
	         
				 this.mH.setColumn(dof, this.processMagTempV5);
	
		   }			   this.mH.transpose(this.processMagTempM4);
			   Matrix3x3d.mult(this.mP, this.processMagTempM4, this.processMagTempM5);
			   Matrix3x3d.mult(this.mH, this.processMagTempM5, this.processMagTempM6);
			   Matrix3x3d.add(this.processMagTempM6, this.mR, this.mS);
	       
	 
			   this.mS.invert(this.processMagTempM4);
			   this.mH.transpose(this.processMagTempM5);
			   Matrix3x3d.mult(this.processMagTempM5, this.processMagTempM4, this.processMagTempM6);
			   Matrix3x3d.mult(this.mP, this.processMagTempM6, this.mK);
	       
	 
			   Matrix3x3d.mult(this.mK, this.mNu, this.mx);
	       
	 
			   Matrix3x3d.mult(this.mK, this.mH, this.processMagTempM4);
			   this.processMagTempM5.setIdentity();
			   this.processMagTempM5.minusEquals(this.processMagTempM4);
			   Matrix3x3d.mult(this.processMagTempM5, this.mP, this.processMagTempM4);
			   this.mP.set(this.processMagTempM4);
	       
			   So3Util.sO3FromMu(this.mx, this.so3LastMotion);
	       
			   Matrix3x3d.mult(this.so3LastMotion, this.so3SensorFromWorld, this.processMagTempM4);
			   this.so3SensorFromWorld.set(this.processMagTempM4);
	       
			   updateCovariancesAfterMotion();
	
		 }			 else
	
		 {			   magObservationFunctionForNumericalJacobian(this.so3SensorFromWorld, this.mNu);
			   So3Util.sO3FromMu(this.mx, this.so3LastMotion);
	       
			   Matrix3x3d.mult(this.so3LastMotion, this.so3SensorFromWorld, this.processMagTempM4);
			   this.so3SensorFromWorld.set(this.processMagTempM4);
	       
			   updateCovariancesAfterMotion();
	
		 }			 this.sensorTimeStampMag = sensorTimeStamp;
	
	   }	   
		   private void filterGyroTimestep(float timeStep)
	
	   {			 float kFilterCoeff = 0.95F;
			 float kMinSamples = 10.0F;
			 if (!this.timestepFilterInit)
	
		 {			   this.filteredGyroTimestep = timeStep;
			   this.numGyroTimestepSamples = 1;
			   this.timestepFilterInit = true;
	
		 }			 else
	
		 {			   this.filteredGyroTimestep = (0.95F * this.filteredGyroTimestep + 0.05000001F * timeStep);
			   if (++this.numGyroTimestepSamples > 10.0F)
		   {
				 this.gyroFilterValid = true;
	
		   }	
		 }	
	   }	   
		   private void updateCovariancesAfterMotion()
	
	   {			 this.so3LastMotion.transpose(this.updateCovariancesAfterMotionTempM1);
			 Matrix3x3d.mult(this.mP, this.updateCovariancesAfterMotionTempM1, this.updateCovariancesAfterMotionTempM2);
	     
			 Matrix3x3d.mult(this.so3LastMotion, this.updateCovariancesAfterMotionTempM2, this.mP);
			 this.so3LastMotion.setIdentity();
	
	   }	   
		   private void accObservationFunctionForNumericalJacobian(Matrix3x3d so3SensorFromWorldPred, Vector3d result)
	
	   {			 Matrix3x3d.mult(so3SensorFromWorldPred, this.down, this.mh);
			 So3Util.sO3FromTwoVec(this.mh, this.mz, this.accObservationFunctionForNumericalJacobianTempM);
	     
	 
			 So3Util.muFromSO3(this.accObservationFunctionForNumericalJacobianTempM, result);
	
	   }	   
		   private void magObservationFunctionForNumericalJacobian(Matrix3x3d so3SensorFromWorldPred, Vector3d result)
	
	   {			 Matrix3x3d.mult(so3SensorFromWorldPred, this.north, this.mh);
			 So3Util.sO3FromTwoVec(this.mh, this.mz, this.magObservationFunctionForNumericalJacobianTempM);
	     
			 So3Util.muFromSO3(this.magObservationFunctionForNumericalJacobianTempM, result);
	
	   }	   
		   public static void arrayAssign(double[][] data, Matrix3x3d m)
	
	   {			 assert(3 == data.Length);
			 assert(3 == data[0].Length);
			 assert(3 == data[1].Length);
			 assert(3 == data[2].Length);
			 m.set(data[0][0], data[0][1], data[0][2], data[1][0], data[1][1], data[1][2], data[2][0], data[2][1], data[2][2]);
	
	   }	
	 }

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     com.google.vrtoolkit.cardboard.sensors.internal.OrientationEKF
	 * JD-Core Version:    0.7.0.1
	 */
 }