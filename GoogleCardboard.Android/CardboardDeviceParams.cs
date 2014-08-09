using System.Collections.Generic;
using Android.Net;
using Android.Nfc;
using Android.Util;

namespace Google.Cardboard
{
	public class CardboardDeviceParams
	{
		private const string TAG = "CardboardDeviceParams";
		private const string DEFAULT_VENDOR = "com.google";
		private const string DEFAULT_MODEL = "cardboard";
		private const string DEFAULT_VERSION = "1.0";
		private const float DEFAULT_INTERPUPILLARY_DISTANCE = 0.06F;
		private const float DEFAULT_VERTICAL_DISTANCE_TO_LENS_CENTER = 0.035F;
		private const float DEFAULT_LENS_DIAMETER = 0.025F;
		private const float DEFAULT_SCREEN_TO_LENS_DISTANCE = 0.037F;
		private const float DEFAULT_EYE_TO_LENS_DISTANCE = 0.011F;
		private const float DEFAULT_VISIBLE_VIEWPORT_MAX_SIZE = 0.06F;
		private const float DEFAULT_FOV_Y = 65.0F;
		private NdefMessage mNfcTagContents;
		private string mVendor;
		private string mModel;
		private string mVersion;
		private float mInterpupillaryDistance;
		private float mVerticalDistanceToLensCenter;
		private float mLensDiameter;
		private float mScreenToLensDistance;
		private float mEyeToLensDistance;
		private float mVisibleViewportSize;
		private float mFovY;
		private Distortion mDistortion;

		public CardboardDeviceParams()
		{
			mVendor = "com.google";
			mModel = "cardboard";
			mVersion = "1.0";

			mInterpupillaryDistance = 0.06F;
			mVerticalDistanceToLensCenter = 0.035F;
			mLensDiameter = 0.025F;
			mScreenToLensDistance = 0.037F;
			mEyeToLensDistance = 0.011F;

			mVisibleViewportSize = 0.06F;
			mFovY = 65.0F;

			mDistortion = new Distortion();
		}

		public CardboardDeviceParams(CardboardDeviceParams @params)
		{
			mNfcTagContents = @params.mNfcTagContents;

			mVendor = @params.mVendor;
			mModel = @params.mModel;
			mVersion = @params.mVersion;

			mInterpupillaryDistance = @params.mInterpupillaryDistance;
			mVerticalDistanceToLensCenter = @params.mVerticalDistanceToLensCenter;
			mLensDiameter = @params.mLensDiameter;
			mScreenToLensDistance = @params.mScreenToLensDistance;
			mEyeToLensDistance = @params.mEyeToLensDistance;

			mVisibleViewportSize = @params.mVisibleViewportSize;
			mFovY = @params.mFovY;

			mDistortion = new Distortion(@params.mDistortion);
		}

		public static CardboardDeviceParams createFromNfcContents(NdefMessage tagContents)
		{
			if (tagContents == null)
			{
				Log.Warn("CardboardDeviceParams", "Could not get contents from NFC tag.");
				return null;
			}
			CardboardDeviceParams deviceParams = new CardboardDeviceParams();
			foreach (NdefRecord record in tagContents.GetRecords())
			{
				if (deviceParams.parseNfcUri(record))
				{
					break;
				}
			}
			return deviceParams;
		}

		public virtual NdefMessage NfcTagContents
		{
			get { return mNfcTagContents; }
		}

		public virtual string Vendor
		{
			set { mVendor = value; }
			get { return mVendor; }
		}

		public virtual string Model
		{
			set { mModel = value; }
			get { return mModel; }
		}

		public virtual string Version
		{
			set { mVersion = value; }
			get { return mVersion; }
		}

		public virtual float InterpupillaryDistance
		{
			set { mInterpupillaryDistance = value; }
			get { return mInterpupillaryDistance; }
		}

		public virtual float VerticalDistanceToLensCenter
		{
			set { mVerticalDistanceToLensCenter = value; }
			get { return mVerticalDistanceToLensCenter; }
		}

		public virtual float VisibleViewportSize
		{
			set { mVisibleViewportSize = value; }
			get { return mVisibleViewportSize; }
		}

		public virtual float FovY
		{
			set { mFovY = value; }
			get { return mFovY; }
		}

		public virtual float LensDiameter
		{
			set { mLensDiameter = value; }
			get { return mLensDiameter; }
		}

		public virtual float ScreenToLensDistance
		{
			set { mScreenToLensDistance = value; }
			get { return mScreenToLensDistance; }
		}

		public virtual float EyeToLensDistance
		{
			set { mEyeToLensDistance = value; }
			get { return mEyeToLensDistance; }
		}

		public virtual Distortion Distortion
		{
			get { return mDistortion; }
		}

		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (!(other is CardboardDeviceParams))
			{
				return false;
			}
			CardboardDeviceParams o = (CardboardDeviceParams) other;

			return (mVendor == o.mVendor) && (mModel == o.mModel) && (mVersion == o.mVersion) &&
			       (mInterpupillaryDistance == o.mInterpupillaryDistance) &&
			       (mVerticalDistanceToLensCenter == o.mVerticalDistanceToLensCenter) &&
			       (mLensDiameter == o.mLensDiameter) && (mScreenToLensDistance == o.mScreenToLensDistance) &&
			       (mEyeToLensDistance == o.mEyeToLensDistance) && (mVisibleViewportSize == o.mVisibleViewportSize) &&
			       (mFovY == o.mFovY) && (mDistortion.Equals(o.mDistortion));
		}

		private bool parseNfcUri(NdefRecord record)
		{
			Uri uri = record.ToUri();
			if (uri == null)
			{
				return false;
			}
			if (uri.Host.Equals("v1.0.0"))
			{
				mVendor = "com.google";
				mModel = "cardboard";
				mVersion = "1.0";
				return true;
			}
			IList<string> segments = uri.PathSegments;
			if (segments.Count != 2)
			{
				return false;
			}
			mVendor = uri.Host;
			mModel = ((string) segments[0]);
			mVersion = ((string) segments[1]);

			return true;
		}
	}
}