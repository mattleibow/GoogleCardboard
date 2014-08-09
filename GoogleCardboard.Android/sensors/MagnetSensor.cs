using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Java.Interop;
using Java.Lang;
using System.Linq;
using Math = Java.Lang.Math;
using Object = Java.Lang.Object;
using Process = Android.OS.Process;
using Thread = Java.Lang.Thread;
using ThreadPriority = Android.OS.ThreadPriority;

namespace Google.Cardboard.Sensors
{
	public class MagnetSensor
	{
		private TriggerDetector mDetector;
		private Thread mDetectorThread;

		public MagnetSensor(Context context)
		{
			mDetector = new TriggerDetector(context);
		}

		public virtual void Start()
		{
			mDetectorThread = new Thread(mDetector);
			mDetectorThread.Start();
		}

		public virtual void Stop()
		{
			if (mDetectorThread != null)
			{
				mDetectorThread.Interrupt();
				mDetector.Stop();
			}
		}

		public virtual IOnCardboardTriggerListener OnCardboardTriggerListener
		{
			set { mDetector.SetOnCardboardTriggerListener(value, new Handler()); }
		}

		private class TriggerDetector : Object, IRunnable, ISensorEventListener
		{
			internal const string TAG = "TriggerDetector";
			internal const int SEGMENT_SIZE = 20;
			internal const int NUM_SEGMENTS = 2;
			internal const int WINDOW_SIZE = 40;
			internal const int T1 = 30;
			internal const int T2 = 130;
			internal SensorManager mSensorManager;
			internal Sensor mMagnetometer;
			internal List<float[]> mSensorData;
			internal float[] mOffsets = new float[20];
			internal IOnCardboardTriggerListener mListener;
			internal Handler mHandler;

			public TriggerDetector(Context context)
			{
				mSensorData = new List<float[]>();
				mSensorManager = context.GetSystemService(Context.SensorService).JavaCast<SensorManager>();
				mMagnetometer = mSensorManager.GetDefaultSensor(SensorType.MagneticField);
			}

			public virtual void SetOnCardboardTriggerListener(IOnCardboardTriggerListener listener, Handler handler)
			{
				lock (this)
				{
					mListener = listener;
					mHandler = handler;
				}
			}

			internal virtual void AddData(float[] values, long time)
			{
				if (mSensorData.Count > 40)
				{
					mSensorData.RemoveAt(0);
				}
				mSensorData.Add(values);


				EvaluateModel();
			}

			internal virtual void EvaluateModel()
			{
				if (mSensorData.Count < 40)
				{
					return;
				}
				float[] means = new float[2];
				float[] maximums = new float[2];
				float[] minimums = new float[2];


				float[] baseline = (float[]) mSensorData[mSensorData.Count - 1];
				for (int i = 0; i < 2; i++)
				{
					int segmentStart = 20*i;


					float[] mOffsets = ComputeOffsets(segmentStart, baseline);

					means[i] = ComputeMean(mOffsets);
					maximums[i] = ComputeMaximum(mOffsets);
					minimums[i] = ComputeMinimum(mOffsets);
				}
				float min1 = minimums[0];
				float max2 = maximums[1];
				if ((min1 < 30.0F) && (max2 > 130.0F))
				{
					HandleButtonPressed();
				}
			}

			internal virtual void HandleButtonPressed()
			{
				mSensorData.Clear();
				lock (this)
				{
					if (mListener != null)
					{
						mHandler.Post(() => { mListener.OnCardboardTrigger(); });
					}
				}
			}

			internal virtual float[] ComputeOffsets(int start, float[] baseline)
			{
				for (int i = 0; i < 20; i++)
				{
					float[] point = (float[]) mSensorData[start + i];
					float[] o = new float[]
					{
						point[0] - baseline[0],
						point[1] - baseline[1],
						point[2] - baseline[2]
					};
					float magnitude = (float) Math.Sqrt(o[0]*o[0] + o[1]*o[1] + o[2]*o[2]);
					mOffsets[i] = magnitude;
				}
				return mOffsets;
			}

			internal virtual float ComputeMean(float[] offsets)
			{
				float sum = 0.0F;
				foreach (float o in offsets)
				{
					sum += o;
				}
				return sum/offsets.Length;
			}

			internal virtual float ComputeMaximum(float[] offsets)
			{
				float max = (1.0F/-1.0F);
				foreach (float o in offsets)
				{
					max = Math.Max(o, max);
				}
				return max;
			}

			internal virtual float ComputeMinimum(float[] offsets)
			{
				float min = (1.0F/1.0F);
				foreach (float o in offsets)
				{
					min = Math.Min(o, min);
				}
				return min;
			}

			public void Run()
			{
				Process.SetThreadPriority(ThreadPriority.UrgentAudio);
				Looper.Prepare();
				mSensorManager.RegisterListener(this, mMagnetometer, 0);
				Looper.Loop();
			}

			public virtual void Stop()
			{
				mSensorManager.UnregisterListener(this);
			}

			public virtual void OnSensorChanged(SensorEvent @event)
			{
				if (@event.Sensor.Equals(mMagnetometer))
				{
					IList<float> values = @event.Values;
					if ((values[0] == 0.0F) && (values[1] == 0.0F) && (values[2] == 0.0F))
					{
						return;
					}
					AddData(@event.Values.ToArray(), @event.Timestamp);
				}
			}

			public virtual void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
			{
			}
		}

		public interface IOnCardboardTriggerListener
		{
			void OnCardboardTrigger();
		}
	}
}