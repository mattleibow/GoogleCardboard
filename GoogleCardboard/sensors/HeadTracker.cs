using System.Threading;

 namespace com.google.vrtoolkit.cardboard.sensors
 {
	 
		 using Context = android.content.Context;
		 using Sensor = android.hardware.Sensor;
		 using SensorEvent = android.hardware.SensorEvent;
		 using SensorEventListener = android.hardware.SensorEventListener;
		 using SensorManager = android.hardware.SensorManager;
		 using Matrix = android.opengl.Matrix;
		 using Handler = android.os.Handler;
		 using Looper = android.os.Looper;
		 using OrientationEKF = com.google.vrtoolkit.cardboard.sensors.@internal.OrientationEKF;
	 
		 public class HeadTracker
	
	 {		   private const string TAG = "HeadTracker";
		   private static readonly double NS2S = 1.E-0x09D;
		   private static readonly int[] INPUT_SENSORS = new int[] {1, 4};
		   private readonly Context mContext;
		   private readonly float[] mEkfToHeadTracker = new float[16];
		   private readonly float[] mTmpHeadView = new float[16];
		   private readonly float[] mTmpRotatedEvent = new float[3];
		   private Looper mSensorLooper;
		   private SensorEventListener mSensorEventListener;
		   private volatile bool mTracking;
		   private readonly OrientationEKF mTracker = new OrientationEKF();
		   private long mLastGyroEventTimeNanos;
	   
		   public HeadTracker(Context context)
	
	   {			 this.mContext = context;
			 Matrix.setRotateEulerM(this.mEkfToHeadTracker, 0, -90.0F, 0.0F, 0.0F);
	
	   }	   
		   public virtual void startTracking()
	
	   {			 if (this.mTracking)
		 {
			   return;
	
		 }			 this.mTracker.reset();
	     
			 this.mSensorEventListener = new SensorEventListenerAnonymousInnerClassHelper(this);
			 Thread sensorThread = new Thread(new RunnableAnonymousInnerClassHelper(this));
			 sensorThread.Start();
			 this.mTracking = true;
	
	   }
	   private class SensorEventListenerAnonymousInnerClassHelper : SensorEventListener
	   {
		   private readonly HeadTracker outerInstance;

		   public SensorEventListenerAnonymousInnerClassHelper(HeadTracker outerInstance)
		   {
			   this.outerInstance = outerInstance;
		   }

	   	   		   public virtual void onSensorChanged(SensorEvent @event)
	   
		   {	   			 outerInstance.processSensorEvent(@event);
	   
		   }	          
	   		   public virtual void onAccuracyChanged(Sensor sensor, int accuracy)
		   {
		   }
	   
	   }
	   private class RunnableAnonymousInnerClassHelper : Runnable
	   {
		   private readonly HeadTracker outerInstance;

		   public RunnableAnonymousInnerClassHelper(HeadTracker outerInstance)
		   {
			   this.outerInstance = outerInstance;
		   }

	   	   		   public virtual void run()
	   
		   {	   			 Looper.prepare();
	            
	   			 outerInstance.mSensorLooper = Looper.myLooper();
	   			 Handler handler = new Handler();
	            
	   			 SensorManager sensorManager = (SensorManager)HeadTracker.this.mContext.getSystemService("sensor");
	   			 foreach (int sensorType in HeadTracker.INPUT_SENSORS)
	   
			 {	   			   Sensor sensor = sensorManager.getDefaultSensor(sensorType);
	   			   sensorManager.registerListener(outerInstance.mSensorEventListener, sensor, 0, handler);
	   
			 }	   			 Looper.loop();
	   
		   }	   
	   }	   
		   public virtual void stopTracking()
	
	   {			 if (!this.mTracking)
		 {
			   return;
	
		 }			 SensorManager sensorManager = (SensorManager)this.mContext.getSystemService("sensor");
	     
			 sensorManager.unregisterListener(this.mSensorEventListener);
			 this.mSensorEventListener = null;
	     
			 this.mSensorLooper.quit();
			 this.mSensorLooper = null;
			 this.mTracking = false;
	
	   }	   
		   public virtual void getLastHeadView(float[] headView, int offset)
	
	   {			 if (offset + 16 > headView.Length)
		 {
			   throw new System.ArgumentException("Not enough space to write the result");
	
		 }			 lock (this.mTracker)
	
		 {			   double secondsSinceLastGyroEvent = (System.nanoTime() - this.mLastGyroEventTimeNanos) * 1.E-0x09D;
	       
	 
			   double secondsToPredictForward = secondsSinceLastGyroEvent + 0.03333333333333333D;
			   double[] mat = this.mTracker.getPredictedGLMatrix(secondsToPredictForward);
			   for (int i = 0; i < headView.Length; i++)
		   {
				 this.mTmpHeadView[i] = ((float)mat[i]);
	
		   }	
		 }			 Matrix.multiplyMM(headView, offset, this.mTmpHeadView, 0, this.mEkfToHeadTracker, 0);
	
	   }	   
		   private void processSensorEvent(SensorEvent @event)
	
	   {			 long timeNanos = System.nanoTime();
	     
	 
	 
			 this.mTmpRotatedEvent[0] = (-@event.values[1]);
			 this.mTmpRotatedEvent[1] = @event.values[0];
			 this.mTmpRotatedEvent[2] = @event.values[2];
			 lock (this.mTracker)
	
		 {			   if (@event.sensor.Type == 1)
	
		   {				 this.mTracker.processAcc(this.mTmpRotatedEvent, @event.timestamp);
	
		   }			   else if (@event.sensor.Type == 4)
	
		   {				 this.mLastGyroEventTimeNanos = timeNanos;
				 this.mTracker.processGyro(this.mTmpRotatedEvent, @event.timestamp);
	
		   }	
		 }	
	   }	
	 }

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     com.google.vrtoolkit.cardboard.sensors.HeadTracker
	 * JD-Core Version:    0.7.0.1
	 */
 }