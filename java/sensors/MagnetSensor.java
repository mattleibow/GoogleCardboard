/*   1:    */ package com.google.vrtoolkit.cardboard.sensors;
/*   2:    */ 
/*   3:    */ import android.content.Context;
/*   4:    */ import android.hardware.Sensor;
/*   5:    */ import android.hardware.SensorEvent;
/*   6:    */ import android.hardware.SensorEventListener;
/*   7:    */ import android.hardware.SensorManager;
/*   8:    */ import android.os.Handler;
/*   9:    */ import android.os.Looper;
/*  10:    */ import android.os.Process;
/*  11:    */ import java.util.ArrayList;
/*  12:    */ 
/*  13:    */ public class MagnetSensor
/*  14:    */ {
/*  15:    */   private TriggerDetector mDetector;
/*  16:    */   private Thread mDetectorThread;
/*  17:    */   
/*  18:    */   public MagnetSensor(Context context)
/*  19:    */   {
/*  20: 54 */     this.mDetector = new TriggerDetector(context);
/*  21:    */   }
/*  22:    */   
/*  23:    */   public void start()
/*  24:    */   {
/*  25: 61 */     this.mDetectorThread = new Thread(this.mDetector);
/*  26: 62 */     this.mDetectorThread.start();
/*  27:    */   }
/*  28:    */   
/*  29:    */   public void stop()
/*  30:    */   {
/*  31: 69 */     if (this.mDetectorThread != null)
/*  32:    */     {
/*  33: 70 */       this.mDetectorThread.interrupt();
/*  34: 71 */       this.mDetector.stop();
/*  35:    */     }
/*  36:    */   }
/*  37:    */   
/*  38:    */   public void setOnCardboardTriggerListener(OnCardboardTriggerListener listener)
/*  39:    */   {
/*  40: 83 */     this.mDetector.setOnCardboardTriggerListener(listener, new Handler());
/*  41:    */   }
/*  42:    */   
/*  43:    */   private static class TriggerDetector
/*  44:    */     implements Runnable, SensorEventListener
/*  45:    */   {
/*  46:    */     private static final String TAG = "TriggerDetector";
/*  47:    */     private static final int SEGMENT_SIZE = 20;
/*  48:    */     private static final int NUM_SEGMENTS = 2;
/*  49:    */     private static final int WINDOW_SIZE = 40;
/*  50:    */     private static final int T1 = 30;
/*  51:    */     private static final int T2 = 130;
/*  52:    */     private SensorManager mSensorManager;
/*  53:    */     private Sensor mMagnetometer;
/*  54:    */     private ArrayList<float[]> mSensorData;
/*  55:119 */     private float[] mOffsets = new float[20];
/*  56:    */     private MagnetSensor.OnCardboardTriggerListener mListener;
/*  57:    */     private Handler mHandler;
/*  58:    */     
/*  59:    */     public TriggerDetector(Context context)
/*  60:    */     {
/*  61:125 */       this.mSensorData = new ArrayList();
/*  62:126 */       this.mSensorManager = ((SensorManager)context.getSystemService("sensor"));
/*  63:127 */       this.mMagnetometer = this.mSensorManager.getDefaultSensor(2);
/*  64:    */     }
/*  65:    */     
/*  66:    */     public synchronized void setOnCardboardTriggerListener(MagnetSensor.OnCardboardTriggerListener listener, Handler handler)
/*  67:    */     {
/*  68:132 */       this.mListener = listener;
/*  69:133 */       this.mHandler = handler;
/*  70:    */     }
/*  71:    */     
/*  72:    */     private void addData(float[] values, long time)
/*  73:    */     {
/*  74:137 */       if (this.mSensorData.size() > 40) {
/*  75:138 */         this.mSensorData.remove(0);
/*  76:    */       }
/*  77:140 */       this.mSensorData.add(values);
/*  78:    */       
/*  79:    */ 
/*  80:143 */       evaluateModel();
/*  81:    */     }
/*  82:    */     
/*  83:    */     private void evaluateModel()
/*  84:    */     {
/*  85:148 */       if (this.mSensorData.size() < 40) {
/*  86:149 */         return;
/*  87:    */       }
/*  88:152 */       float[] means = new float[2];
/*  89:153 */       float[] maximums = new float[2];
/*  90:154 */       float[] minimums = new float[2];
/*  91:    */       
/*  92:    */ 
/*  93:157 */       float[] baseline = (float[])this.mSensorData.get(this.mSensorData.size() - 1);
/*  94:160 */       for (int i = 0; i < 2; i++)
/*  95:    */       {
/*  96:161 */         int segmentStart = 20 * i;
/*  97:    */         
/*  98:    */ 
/*  99:164 */         float[] mOffsets = computeOffsets(segmentStart, baseline);
/* 100:    */         
/* 101:166 */         means[i] = computeMean(mOffsets);
/* 102:167 */         maximums[i] = computeMaximum(mOffsets);
/* 103:168 */         minimums[i] = computeMinimum(mOffsets);
/* 104:    */       }
/* 105:172 */       float min1 = minimums[0];
/* 106:173 */       float max2 = maximums[1];
/* 107:175 */       if ((min1 < 30.0F) && (max2 > 130.0F)) {
/* 108:176 */         handleButtonPressed();
/* 109:    */       }
/* 110:    */     }
/* 111:    */     
/* 112:    */     private void handleButtonPressed()
/* 113:    */     {
/* 114:182 */       this.mSensorData.clear();
/* 115:185 */       synchronized (this)
/* 116:    */       {
/* 117:186 */         if (this.mListener != null) {
/* 118:187 */           this.mHandler.post(new Runnable()
/* 119:    */           {
/* 120:    */             public void run()
/* 121:    */             {
/* 122:190 */               MagnetSensor.TriggerDetector.this.mListener.onCardboardTrigger();
/* 123:    */             }
/* 124:    */           });
/* 125:    */         }
/* 126:    */       }
/* 127:    */     }
/* 128:    */     
/* 129:    */     private float[] computeOffsets(int start, float[] baseline)
/* 130:    */     {
/* 131:198 */       for (int i = 0; i < 20; i++)
/* 132:    */       {
/* 133:199 */         float[] point = (float[])this.mSensorData.get(start + i);
/* 134:200 */         float[] o = { point[0] - baseline[0], point[1] - baseline[1], point[2] - baseline[2] };
/* 135:201 */         float magnitude = (float)Math.sqrt(o[0] * o[0] + o[1] * o[1] + o[2] * o[2]);
/* 136:202 */         this.mOffsets[i] = magnitude;
/* 137:    */       }
/* 138:204 */       return this.mOffsets;
/* 139:    */     }
/* 140:    */     
/* 141:    */     private float computeMean(float[] offsets)
/* 142:    */     {
/* 143:208 */       float sum = 0.0F;
/* 144:209 */       for (float o : offsets) {
/* 145:210 */         sum += o;
/* 146:    */       }
/* 147:212 */       return sum / offsets.length;
/* 148:    */     }
/* 149:    */     
/* 150:    */     private float computeMaximum(float[] offsets)
/* 151:    */     {
/* 152:216 */       float max = (1.0F / -1.0F);
/* 153:217 */       for (float o : offsets) {
/* 154:218 */         max = Math.max(o, max);
/* 155:    */       }
/* 156:220 */       return max;
/* 157:    */     }
/* 158:    */     
/* 159:    */     private float computeMinimum(float[] offsets)
/* 160:    */     {
/* 161:224 */       float min = (1.0F / 1.0F);
/* 162:225 */       for (float o : offsets) {
/* 163:226 */         min = Math.min(o, min);
/* 164:    */       }
/* 165:228 */       return min;
/* 166:    */     }
/* 167:    */     
/* 168:    */     public void run()
/* 169:    */     {
/* 170:235 */       Process.setThreadPriority(-19);
/* 171:236 */       Looper.prepare();
/* 172:237 */       this.mSensorManager.registerListener(this, this.mMagnetometer, 0);
/* 173:238 */       Looper.loop();
/* 174:    */     }
/* 175:    */     
/* 176:    */     public void stop()
/* 177:    */     {
/* 178:242 */       this.mSensorManager.unregisterListener(this);
/* 179:    */     }
/* 180:    */     
/* 181:    */     public void onSensorChanged(SensorEvent event)
/* 182:    */     {
/* 183:247 */       if (event.sensor.equals(this.mMagnetometer))
/* 184:    */       {
/* 185:248 */         float[] values = event.values;
/* 186:250 */         if ((values[0] == 0.0F) && (values[1] == 0.0F) && (values[2] == 0.0F)) {
/* 187:251 */           return;
/* 188:    */         }
/* 189:253 */         addData((float[])event.values.clone(), event.timestamp);
/* 190:    */       }
/* 191:    */     }
/* 192:    */     
/* 193:    */     public void onAccuracyChanged(Sensor sensor, int accuracy) {}
/* 194:    */   }
/* 195:    */   
/* 196:    */   public static abstract interface OnCardboardTriggerListener
/* 197:    */   {
/* 198:    */     public abstract void onCardboardTrigger();
/* 199:    */   }
/* 200:    */ }


/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
 * Qualified Name:     com.google.vrtoolkit.cardboard.sensors.MagnetSensor
 * JD-Core Version:    0.7.0.1
 */