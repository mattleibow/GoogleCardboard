using System.Threading;
using Android.Hardware;
using Android.Content;
using Android.OS;
using Google.Cardboard.Sensors.Internal;
using Java.Interop;
using Java.Lang;
using Android.Opengl;
using System;
using Environment = System.Environment;
using Object = Java.Lang.Object;
using Thread = Java.Lang.Thread;

namespace Google.Cardboard.Sensors
{
	public class HeadTracker
	{
		private const string TAG = "HeadTracker";
		// todo: private static readonly double NS2S = 1.E - 0x09D;
		private static readonly SensorType[] INPUT_SENSORS = new SensorType[]
		{
			SensorType.Accelerometer,
			SensorType.Gyroscope
		};

		private readonly Context mContext;
		private readonly float[] mEkfToHeadTracker = new float[16];
		private readonly float[] mTmpHeadView = new float[16];
		private readonly float[] mTmpRotatedEvent = new float[3];
		private Looper mSensorLooper;
		private ISensorEventListener mSensorEventListener;
		private volatile bool mTracking;
		private readonly OrientationEKF mTracker = new OrientationEKF();
		private long mLastGyroEventTimeTicks;

		public HeadTracker(Context context)
		{
			mContext = context;
			Matrix.SetRotateEulerM(mEkfToHeadTracker, 0, -90.0F, 0.0F, 0.0F);
		}

		public virtual void StartTracking()
		{
			if (mTracking)
			{
				return;
			}
			mTracker.reset();

			mSensorEventListener = new SensorEventListener(this);

			Thread sensorThread = new Thread(new Runnable(() =>
			{
				Looper.Prepare();
				mSensorLooper = Looper.MyLooper();
				Handler handler = new Handler();
				SensorManager sensorManager = mContext.GetSystemService(Context.SensorService).JavaCast<SensorManager>();
				foreach (SensorType sensorType in INPUT_SENSORS)
				{
					Sensor sensor = sensorManager.GetDefaultSensor(sensorType);
					sensorManager.RegisterListener(mSensorEventListener, sensor, 0, handler);
				}
				Looper.Loop();
			}));
			sensorThread.Start();
			mTracking = true;
		}

		private class SensorEventListener : Object, ISensorEventListener
		{
			private readonly HeadTracker parentHeadTracker;

			public SensorEventListener(HeadTracker headTracker)
			{
				parentHeadTracker = headTracker;
			}

			public void OnSensorChanged(SensorEvent e)
			{
				parentHeadTracker.ProcessSensorEvent(e);
			}

			public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
			{
			}
		}

		public virtual void StopTracking()
		{
			if (!mTracking)
			{
				return;
			}
			SensorManager sensorManager = mContext.GetSystemService(Context.SensorService).JavaCast<SensorManager>();

			sensorManager.UnregisterListener(mSensorEventListener);
			mSensorEventListener = null;

			mSensorLooper.Quit();
			mSensorLooper = null;
			mTracking = false;
		}

		public virtual void GetLastHeadView(float[] headView, int offset)
		{
			if (offset + 16 > headView.Length)
			{
				throw new ArgumentException("Not enough space to write the result");
			}
			lock (mTracker)
			{
				double secondsSinceLastGyroEvent = (Environment.TickCount - mLastGyroEventTimeTicks)*
				                                   TimeSpan.TicksPerSecond;


				double secondsToPredictForward = secondsSinceLastGyroEvent + 0.03333333333333333D;
				double[] mat = mTracker.getPredictedGLMatrix(secondsToPredictForward);
				for (int i = 0; i < headView.Length; i++)
				{
					mTmpHeadView[i] = ((float) mat[i]);
				}
			}
			Matrix.MultiplyMM(headView, offset, mTmpHeadView, 0, mEkfToHeadTracker, 0);
		}

		private void ProcessSensorEvent(SensorEvent @event)
		{
			long timeNanos = Environment.TickCount;

			mTmpRotatedEvent[0] = (-@event.Values[1]);
			mTmpRotatedEvent[1] = @event.Values[0];
			mTmpRotatedEvent[2] = @event.Values[2];
			lock (mTracker)
			{
				if (@event.Sensor.Type == SensorType.Accelerometer)
				{
					mTracker.processAcc(mTmpRotatedEvent, @event.Timestamp);
				}
				else if (@event.Sensor.Type == SensorType.Gyroscope)
				{
					mLastGyroEventTimeTicks = timeNanos;
					mTracker.processGyro(mTmpRotatedEvent, @event.Timestamp);
				}
			}
		}
	}
}