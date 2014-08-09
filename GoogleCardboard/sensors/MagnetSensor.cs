using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

 namespace com.google.vrtoolkit.cardboard.sensors
 {
	 
		 using Context = android.content.Context;
		 using Sensor = android.hardware.Sensor;
		 using SensorEvent = android.hardware.SensorEvent;
		 using SensorEventListener = android.hardware.SensorEventListener;
		 using SensorManager = android.hardware.SensorManager;
		 using Handler = android.os.Handler;
		 using Looper = android.os.Looper;
		 using Process = android.os.Process;
		 
		 public class MagnetSensor
	
	 {		   private TriggerDetector mDetector;
		   private Thread mDetectorThread;
	   
		   public MagnetSensor(Context context)
	
	   {			 this.mDetector = new TriggerDetector(context);
	
	   }	   
		   public virtual void start()
	
	   {			 this.mDetectorThread = new Thread(this.mDetector);
			 this.mDetectorThread.Start();
	
	   }	   
		   public virtual void stop()
	
	   {			 if (this.mDetectorThread != null)
	
		 {			   this.mDetectorThread.Interrupt();
			   this.mDetector.stop();
	
		 }	
	   }	   
	
	   public virtual OnCardboardTriggerListener OnCardboardTriggerListener
	   {
		   set
		
		   {					 this.mDetector.setOnCardboardTriggerListener(value, new Handler());
		
		   }
	   }	   
		   private class TriggerDetector : Runnable, SensorEventListener
	
	
	   {			 internal const string TAG = "TriggerDetector";
			 internal const int SEGMENT_SIZE = 20;
			 internal const int NUM_SEGMENTS = 2;
			 internal const int WINDOW_SIZE = 40;
			 internal const int T1 = 30;
			 internal const int T2 = 130;
			 internal SensorManager mSensorManager;
			 internal Sensor mMagnetometer;
			 internal List<float[]> mSensorData;
			 internal float[] mOffsets = new float[20];
			 internal MagnetSensor.OnCardboardTriggerListener mListener;
			 internal Handler mHandler;
	     
			 public TriggerDetector(Context context)
	
		 {			   this.mSensorData = new ArrayList();
			   this.mSensorManager = ((SensorManager)context.getSystemService("sensor"));
			   this.mMagnetometer = this.mSensorManager.getDefaultSensor(2);
	
		 }	     
			 public virtual void setOnCardboardTriggerListener(MagnetSensor.OnCardboardTriggerListener listener, Handler handler)
	
		 {			 lock (this)
			 {
					   this.mListener = listener;
					   this.mHandler = handler;
		
			 }
		 }	     
			 internal virtual void addData(float[] values, long time)
	
		 {			   if (this.mSensorData.Count > 40)
		   {
				 this.mSensorData.RemoveAt(0);
	
		   }			   this.mSensorData.Add(values);
	       
	 
			   evaluateModel();
	
		 }	     
			 internal virtual void evaluateModel()
	
		 {			   if (this.mSensorData.Count < 40)
		   {
				 return;
	
		   }			   float[] means = new float[2];
			   float[] maximums = new float[2];
			   float[] minimums = new float[2];
	       
	 
			   float[] baseline = (float[])this.mSensorData[this.mSensorData.Count - 1];
			   for (int i = 0; i < 2; i++)
	
		   {				 int segmentStart = 20 * i;
	         
	 
				 float[] mOffsets = computeOffsets(segmentStart, baseline);
	         
				 means[i] = computeMean(mOffsets);
				 maximums[i] = computeMaximum(mOffsets);
				 minimums[i] = computeMinimum(mOffsets);
	
		   }			   float min1 = minimums[0];
			   float max2 = maximums[1];
			   if ((min1 < 30.0F) && (max2 > 130.0F))
		   {
				 handleButtonPressed();
	
		   }	
		 }	     
			 internal virtual void handleButtonPressed()
	
		 {			   this.mSensorData.Clear();
			   lock (this)
	
		   {				 if (this.mListener != null)
			 {
				   this.mHandler.post(new RunnableAnonymousInnerClassHelper(this));
	
			 }	
		   }	
		 }
		 private class RunnableAnonymousInnerClassHelper : Runnable
		 {
			 private readonly TriggerDetector outerInstance;

			 public RunnableAnonymousInnerClassHelper(TriggerDetector outerInstance)
			 {
				 this.outerInstance = outerInstance;
			 }

		 		 			 public virtual void run()
		 
			 {		 			   outerInstance.outerInstance.mListener.onCardboardTrigger();
		 
			 }		 
		 }	     
			 internal virtual float[] computeOffsets(int start, float[] baseline)
	
		 {			   for (int i = 0; i < 20; i++)
	
		   {				 float[] point = (float[])this.mSensorData[start + i];
				 float[] o = new float[] {point[0] - baseline[0], point[1] - baseline[1], point[2] - baseline[2]};
				 float magnitude = (float)Math.Sqrt(o[0] * o[0] + o[1] * o[1] + o[2] * o[2]);
				 this.mOffsets[i] = magnitude;
	
		   }			   return this.mOffsets;
	
		 }	     
			 internal virtual float computeMean(float[] offsets)
	
		 {			   float sum = 0.0F;
			   foreach (float o in offsets)
		   {
				 sum += o;
	
		   }			   return sum / offsets.Length;
	
		 }	     
			 internal virtual float computeMaximum(float[] offsets)
	
		 {			   float max = (1.0F / -1.0F);
			   foreach (float o in offsets)
		   {
				 max = Math.Max(o, max);
	
		   }			   return max;
	
		 }	     
			 internal virtual float computeMinimum(float[] offsets)
	
		 {			   float min = (1.0F / 1.0F);
			   foreach (float o in offsets)
		   {
				 min = Math.Min(o, min);
	
		   }			   return min;
	
		 }	     
			 public virtual void run()
	
		 {			   Process.ThreadPriority = -19;
			   Looper.prepare();
			   this.mSensorManager.registerListener(this, this.mMagnetometer, 0);
			   Looper.loop();
	
		 }	     
			 public virtual void stop()
	
		 {			   this.mSensorManager.unregisterListener(this);
	
		 }	     
			 public virtual void onSensorChanged(SensorEvent @event)
	
		 {			   if (@event.sensor.Equals(this.mMagnetometer))
	
		   {				 float[] values = @event.values;
				 if ((values[0] == 0.0F) && (values[1] == 0.0F) && (values[2] == 0.0F))
			 {
				   return;
	
			 }				 addData((float[])@event.values.clone(), @event.timestamp);
	
		   }	
		 }	     
			 public virtual void onAccuracyChanged(Sensor sensor, int accuracy)
		 {
		 }
	
	   }	   
		   public abstract interface OnCardboardTriggerListener
	
	   {			 void onCardboardTrigger();
	
	   }	
	 }

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     com.google.vrtoolkit.cardboard.sensors.MagnetSensor
	 * JD-Core Version:    0.7.0.1
	 */
 }