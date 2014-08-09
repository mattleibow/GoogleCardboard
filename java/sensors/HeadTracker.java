/*   1:    */ package com.google.vrtoolkit.cardboard.sensors;
/*   2:    */ 
/*   3:    */ import android.content.Context;
/*   4:    */ import android.hardware.Sensor;
/*   5:    */ import android.hardware.SensorEvent;
/*   6:    */ import android.hardware.SensorEventListener;
/*   7:    */ import android.hardware.SensorManager;
/*   8:    */ import android.opengl.Matrix;
/*   9:    */ import android.os.Handler;
/*  10:    */ import android.os.Looper;
/*  11:    */ import com.google.vrtoolkit.cardboard.sensors.internal.OrientationEKF;
/*  12:    */ 
/*  13:    */ public class HeadTracker
/*  14:    */ {
/*  15:    */   private static final String TAG = "HeadTracker";
/*  16:    */   private static final double NS2S = 1.E-009D;
/*  17: 40 */   private static final int[] INPUT_SENSORS = { 1, 4 };
/*  18:    */   private final Context mContext;
/*  19: 49 */   private final float[] mEkfToHeadTracker = new float[16];
/*  20: 51 */   private final float[] mTmpHeadView = new float[16];
/*  21: 53 */   private final float[] mTmpRotatedEvent = new float[3];
/*  22:    */   private Looper mSensorLooper;
/*  23:    */   private SensorEventListener mSensorEventListener;
/*  24:    */   private volatile boolean mTracking;
/*  25: 60 */   private final OrientationEKF mTracker = new OrientationEKF();
/*  26:    */   private long mLastGyroEventTimeNanos;
/*  27:    */   
/*  28:    */   public HeadTracker(Context context)
/*  29:    */   {
/*  30: 65 */     this.mContext = context;
/*  31: 66 */     Matrix.setRotateEulerM(this.mEkfToHeadTracker, 0, -90.0F, 0.0F, 0.0F);
/*  32:    */   }
/*  33:    */   
/*  34:    */   public void startTracking()
/*  35:    */   {
/*  36: 73 */     if (this.mTracking) {
/*  37: 74 */       return;
/*  38:    */     }
/*  39: 76 */     this.mTracker.reset();
/*  40:    */     
/*  41: 78 */     this.mSensorEventListener = new SensorEventListener()
/*  42:    */     {
/*  43:    */       public void onSensorChanged(SensorEvent event)
/*  44:    */       {
/*  45: 81 */         HeadTracker.this.processSensorEvent(event);
/*  46:    */       }
/*  47:    */       
/*  48:    */       public void onAccuracyChanged(Sensor sensor, int accuracy) {}
/*  49: 89 */     };
/*  50: 90 */     Thread sensorThread = new Thread(new Runnable()
/*  51:    */     {
/*  52:    */       public void run()
/*  53:    */       {
/*  54: 93 */         Looper.prepare();
/*  55:    */         
/*  56: 95 */         HeadTracker.this.mSensorLooper = Looper.myLooper();
/*  57: 96 */         Handler handler = new Handler();
/*  58:    */         
/*  59: 98 */         SensorManager sensorManager = (SensorManager)HeadTracker.this.mContext.getSystemService("sensor");
/*  60:101 */         for (int sensorType : HeadTracker.INPUT_SENSORS)
/*  61:    */         {
/*  62:102 */           Sensor sensor = sensorManager.getDefaultSensor(sensorType);
/*  63:103 */           sensorManager.registerListener(HeadTracker.this.mSensorEventListener, sensor, 0, handler);
/*  64:    */         }
/*  65:107 */         Looper.loop();
/*  66:    */       }
/*  67:110 */     });
/*  68:111 */     sensorThread.start();
/*  69:112 */     this.mTracking = true;
/*  70:    */   }
/*  71:    */   
/*  72:    */   public void stopTracking()
/*  73:    */   {
/*  74:119 */     if (!this.mTracking) {
/*  75:120 */       return;
/*  76:    */     }
/*  77:123 */     SensorManager sensorManager = (SensorManager)this.mContext.getSystemService("sensor");
/*  78:    */     
/*  79:125 */     sensorManager.unregisterListener(this.mSensorEventListener);
/*  80:126 */     this.mSensorEventListener = null;
/*  81:    */     
/*  82:128 */     this.mSensorLooper.quit();
/*  83:129 */     this.mSensorLooper = null;
/*  84:130 */     this.mTracking = false;
/*  85:    */   }
/*  86:    */   
/*  87:    */   public void getLastHeadView(float[] headView, int offset)
/*  88:    */   {
/*  89:142 */     if (offset + 16 > headView.length) {
/*  90:143 */       throw new IllegalArgumentException("Not enough space to write the result");
/*  91:    */     }
/*  92:147 */     synchronized (this.mTracker)
/*  93:    */     {
/*  94:148 */       double secondsSinceLastGyroEvent = (System.nanoTime() - this.mLastGyroEventTimeNanos) * 1.E-009D;
/*  95:    */       
/*  96:    */ 
/*  97:151 */       double secondsToPredictForward = secondsSinceLastGyroEvent + 0.03333333333333333D;
/*  98:152 */       double[] mat = this.mTracker.getPredictedGLMatrix(secondsToPredictForward);
/*  99:153 */       for (int i = 0; i < headView.length; i++) {
/* 100:154 */         this.mTmpHeadView[i] = ((float)mat[i]);
/* 101:    */       }
/* 102:    */     }
/* 103:158 */     Matrix.multiplyMM(headView, offset, this.mTmpHeadView, 0, this.mEkfToHeadTracker, 0);
/* 104:    */   }
/* 105:    */   
/* 106:    */   private void processSensorEvent(SensorEvent event)
/* 107:    */   {
/* 108:171 */     long timeNanos = System.nanoTime();
/* 109:    */     
/* 110:    */ 
/* 111:    */ 
/* 112:175 */     this.mTmpRotatedEvent[0] = (-event.values[1]);
/* 113:176 */     this.mTmpRotatedEvent[1] = event.values[0];
/* 114:177 */     this.mTmpRotatedEvent[2] = event.values[2];
/* 115:178 */     synchronized (this.mTracker)
/* 116:    */     {
/* 117:179 */       if (event.sensor.getType() == 1)
/* 118:    */       {
/* 119:180 */         this.mTracker.processAcc(this.mTmpRotatedEvent, event.timestamp);
/* 120:    */       }
/* 121:181 */       else if (event.sensor.getType() == 4)
/* 122:    */       {
/* 123:182 */         this.mLastGyroEventTimeNanos = timeNanos;
/* 124:183 */         this.mTracker.processGyro(this.mTmpRotatedEvent, event.timestamp);
/* 125:    */       }
/* 126:    */     }
/* 127:    */   }
/* 128:    */ }


/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
 * Qualified Name:     com.google.vrtoolkit.cardboard.sensors.HeadTracker
 * JD-Core Version:    0.7.0.1
 */