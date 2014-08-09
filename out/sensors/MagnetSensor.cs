using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

/*   1:    */ namespace com.google.vrtoolkit.cardboard.sensors
 {
	/*   2:    */ 
	/*   3:    */	 using Context = android.content.Context;
	/*   4:    */	 using Sensor = android.hardware.Sensor;
	/*   5:    */	 using SensorEvent = android.hardware.SensorEvent;
	/*   6:    */	 using SensorEventListener = android.hardware.SensorEventListener;
	/*   7:    */	 using SensorManager = android.hardware.SensorManager;
	/*   8:    */	 using Handler = android.os.Handler;
	/*   9:    */	 using Looper = android.os.Looper;
	/*  10:    */	 using Process = android.os.Process;
	/*  11:    */	/*  12:    */ 
	/*  13:    */	 public class MagnetSensor
	/*  14:    */
	 {	/*  15:    */	   private TriggerDetector mDetector;
	/*  16:    */	   private Thread mDetectorThread;
	/*  17:    */   
	/*  18:    */	   public MagnetSensor(Context context)
	/*  19:    */
	   {	/*  20: 54 */		 this.mDetector = new TriggerDetector(context);
	/*  21:    */
	   }	/*  22:    */   
	/*  23:    */	   public virtual void start()
	/*  24:    */
	   {	/*  25: 61 */		 this.mDetectorThread = new Thread(this.mDetector);
	/*  26: 62 */		 this.mDetectorThread.Start();
	/*  27:    */
	   }	/*  28:    */   
	/*  29:    */	   public virtual void stop()
	/*  30:    */
	   {	/*  31: 69 */		 if (this.mDetectorThread != null)
	/*  32:    */
		 {	/*  33: 70 */		   this.mDetectorThread.Interrupt();
	/*  34: 71 */		   this.mDetector.stop();
	/*  35:    */
		 }	/*  36:    */
	   }	/*  37:    */   
	/*  38:    */
	   public virtual OnCardboardTriggerListener OnCardboardTriggerListener
	   {
		   set
		/*  39:    */
		   {		/*  40: 83 */			 this.mDetector.setOnCardboardTriggerListener(value, new Handler());
		/*  41:    */
		   }
	   }	/*  42:    */   
	/*  43:    */	   private class TriggerDetector : Runnable, SensorEventListener
	/*  44:    */
	/*  45:    */
	   {	/*  46:    */		 internal const string TAG = "TriggerDetector";
	/*  47:    */		 internal const int SEGMENT_SIZE = 20;
	/*  48:    */		 internal const int NUM_SEGMENTS = 2;
	/*  49:    */		 internal const int WINDOW_SIZE = 40;
	/*  50:    */		 internal const int T1 = 30;
	/*  51:    */		 internal const int T2 = 130;
	/*  52:    */		 internal SensorManager mSensorManager;
	/*  53:    */		 internal Sensor mMagnetometer;
	/*  54:    */		 internal List<float[]> mSensorData;
	/*  55:119 */		 internal float[] mOffsets = new float[20];
	/*  56:    */		 internal MagnetSensor.OnCardboardTriggerListener mListener;
	/*  57:    */		 internal Handler mHandler;
	/*  58:    */     
	/*  59:    */		 public TriggerDetector(Context context)
	/*  60:    */
		 {	/*  61:125 */		   this.mSensorData = new ArrayList();
	/*  62:126 */		   this.mSensorManager = ((SensorManager)context.getSystemService("sensor"));
	/*  63:127 */		   this.mMagnetometer = this.mSensorManager.getDefaultSensor(2);
	/*  64:    */
		 }	/*  65:    */     
	/*  66:    */		 public virtual void setOnCardboardTriggerListener(MagnetSensor.OnCardboardTriggerListener listener, Handler handler)
	/*  67:    */
		 {			 lock (this)
			 {
		/*  68:132 */			   this.mListener = listener;
		/*  69:133 */			   this.mHandler = handler;
		/*  70:    */
			 }
		 }	/*  71:    */     
	/*  72:    */		 internal virtual void addData(float[] values, long time)
	/*  73:    */
		 {	/*  74:137 */		   if (this.mSensorData.Count > 40)
		   {
	/*  75:138 */			 this.mSensorData.RemoveAt(0);
	/*  76:    */
		   }	/*  77:140 */		   this.mSensorData.Add(values);
	/*  78:    */       
	/*  79:    */ 
	/*  80:143 */		   evaluateModel();
	/*  81:    */
		 }	/*  82:    */     
	/*  83:    */		 internal virtual void evaluateModel()
	/*  84:    */
		 {	/*  85:148 */		   if (this.mSensorData.Count < 40)
		   {
	/*  86:149 */			 return;
	/*  87:    */
		   }	/*  88:152 */		   float[] means = new float[2];
	/*  89:153 */		   float[] maximums = new float[2];
	/*  90:154 */		   float[] minimums = new float[2];
	/*  91:    */       
	/*  92:    */ 
	/*  93:157 */		   float[] baseline = (float[])this.mSensorData[this.mSensorData.Count - 1];
	/*  94:160 */		   for (int i = 0; i < 2; i++)
	/*  95:    */
		   {	/*  96:161 */			 int segmentStart = 20 * i;
	/*  97:    */         
	/*  98:    */ 
	/*  99:164 */			 float[] mOffsets = computeOffsets(segmentStart, baseline);
	/* 100:    */         
	/* 101:166 */			 means[i] = computeMean(mOffsets);
	/* 102:167 */			 maximums[i] = computeMaximum(mOffsets);
	/* 103:168 */			 minimums[i] = computeMinimum(mOffsets);
	/* 104:    */
		   }	/* 105:172 */		   float min1 = minimums[0];
	/* 106:173 */		   float max2 = maximums[1];
	/* 107:175 */		   if ((min1 < 30.0F) && (max2 > 130.0F))
		   {
	/* 108:176 */			 handleButtonPressed();
	/* 109:    */
		   }	/* 110:    */
		 }	/* 111:    */     
	/* 112:    */		 internal virtual void handleButtonPressed()
	/* 113:    */
		 {	/* 114:182 */		   this.mSensorData.Clear();
	/* 115:185 */		   lock (this)
	/* 116:    */
		   {	/* 117:186 */			 if (this.mListener != null)
			 {
	/* 118:187 */			   this.mHandler.post(new RunnableAnonymousInnerClassHelper(this));
	/* 125:    */
			 }	/* 126:    */
		   }	/* 127:    */
		 }
		 private class RunnableAnonymousInnerClassHelper : Runnable
		 {
			 private readonly TriggerDetector outerInstance;

			 public RunnableAnonymousInnerClassHelper(TriggerDetector outerInstance)
			 {
				 this.outerInstance = outerInstance;
			 }

		 /* 119:    */		 /* 120:    */			 public virtual void run()
		 /* 121:    */
			 {		 /* 122:190 */			   outerInstance.outerInstance.mListener.onCardboardTrigger();
		 /* 123:    */
			 }		 /* 124:    */
		 }	/* 128:    */     
	/* 129:    */		 internal virtual float[] computeOffsets(int start, float[] baseline)
	/* 130:    */
		 {	/* 131:198 */		   for (int i = 0; i < 20; i++)
	/* 132:    */
		   {	/* 133:199 */			 float[] point = (float[])this.mSensorData[start + i];
	/* 134:200 */			 float[] o = new float[] {point[0] - baseline[0], point[1] - baseline[1], point[2] - baseline[2]};
	/* 135:201 */			 float magnitude = (float)Math.Sqrt(o[0] * o[0] + o[1] * o[1] + o[2] * o[2]);
	/* 136:202 */			 this.mOffsets[i] = magnitude;
	/* 137:    */
		   }	/* 138:204 */		   return this.mOffsets;
	/* 139:    */
		 }	/* 140:    */     
	/* 141:    */		 internal virtual float computeMean(float[] offsets)
	/* 142:    */
		 {	/* 143:208 */		   float sum = 0.0F;
	/* 144:209 */		   foreach (float o in offsets)
		   {
	/* 145:210 */			 sum += o;
	/* 146:    */
		   }	/* 147:212 */		   return sum / offsets.Length;
	/* 148:    */
		 }	/* 149:    */     
	/* 150:    */		 internal virtual float computeMaximum(float[] offsets)
	/* 151:    */
		 {	/* 152:216 */		   float max = (1.0F / -1.0F);
	/* 153:217 */		   foreach (float o in offsets)
		   {
	/* 154:218 */			 max = Math.Max(o, max);
	/* 155:    */
		   }	/* 156:220 */		   return max;
	/* 157:    */
		 }	/* 158:    */     
	/* 159:    */		 internal virtual float computeMinimum(float[] offsets)
	/* 160:    */
		 {	/* 161:224 */		   float min = (1.0F / 1.0F);
	/* 162:225 */		   foreach (float o in offsets)
		   {
	/* 163:226 */			 min = Math.Min(o, min);
	/* 164:    */
		   }	/* 165:228 */		   return min;
	/* 166:    */
		 }	/* 167:    */     
	/* 168:    */		 public virtual void run()
	/* 169:    */
		 {	/* 170:235 */		   Process.ThreadPriority = -19;
	/* 171:236 */		   Looper.prepare();
	/* 172:237 */		   this.mSensorManager.registerListener(this, this.mMagnetometer, 0);
	/* 173:238 */		   Looper.loop();
	/* 174:    */
		 }	/* 175:    */     
	/* 176:    */		 public virtual void stop()
	/* 177:    */
		 {	/* 178:242 */		   this.mSensorManager.unregisterListener(this);
	/* 179:    */
		 }	/* 180:    */     
	/* 181:    */		 public virtual void onSensorChanged(SensorEvent @event)
	/* 182:    */
		 {	/* 183:247 */		   if (@event.sensor.Equals(this.mMagnetometer))
	/* 184:    */
		   {	/* 185:248 */			 float[] values = @event.values;
	/* 186:250 */			 if ((values[0] == 0.0F) && (values[1] == 0.0F) && (values[2] == 0.0F))
			 {
	/* 187:251 */			   return;
	/* 188:    */
			 }	/* 189:253 */			 addData((float[])@event.values.clone(), @event.timestamp);
	/* 190:    */
		   }	/* 191:    */
		 }	/* 192:    */     
	/* 193:    */		 public virtual void onAccuracyChanged(Sensor sensor, int accuracy)
		 {
		 }
	/* 194:    */
	   }	/* 195:    */   
	/* 196:    */	   public abstract interface OnCardboardTriggerListener
	/* 197:    */
	   {	/* 198:    */		 void onCardboardTrigger();
	/* 199:    */
	   }	/* 200:    */
	 }

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     com.google.vrtoolkit.cardboard.sensors.MagnetSensor
	 * JD-Core Version:    0.7.0.1
	 */
 }