using System.Collections.Generic;

 namespace com.google.vrtoolkit.cardboard
 {
	 
		 using Uri = android.net.Uri;
		 using NdefMessage = android.nfc.NdefMessage;
		 using NdefRecord = android.nfc.NdefRecord;
		 using Log = android.util.Log;
		 
		 public class CardboardDeviceParams
	
	 {		   private const string TAG = "CardboardDeviceParams";
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
	
	   {			 this.mVendor = "com.google";
			 this.mModel = "cardboard";
			 this.mVersion = "1.0";
	     
			 this.mInterpupillaryDistance = 0.06F;
			 this.mVerticalDistanceToLensCenter = 0.035F;
			 this.mLensDiameter = 0.025F;
			 this.mScreenToLensDistance = 0.037F;
			 this.mEyeToLensDistance = 0.011F;
	     
			 this.mVisibleViewportSize = 0.06F;
			 this.mFovY = 65.0F;
	     
			 this.mDistortion = new Distortion();
	
	   }	   
		   public CardboardDeviceParams(CardboardDeviceParams @params)
	
	   {			 this.mNfcTagContents = @params.mNfcTagContents;
	     
			 this.mVendor = @params.mVendor;
			 this.mModel = @params.mModel;
			 this.mVersion = @params.mVersion;
	     
			 this.mInterpupillaryDistance = @params.mInterpupillaryDistance;
			 this.mVerticalDistanceToLensCenter = @params.mVerticalDistanceToLensCenter;
			 this.mLensDiameter = @params.mLensDiameter;
			 this.mScreenToLensDistance = @params.mScreenToLensDistance;
			 this.mEyeToLensDistance = @params.mEyeToLensDistance;
	     
			 this.mVisibleViewportSize = @params.mVisibleViewportSize;
			 this.mFovY = @params.mFovY;
	     
			 this.mDistortion = new Distortion(@params.mDistortion);
	
	   }	   
		   public static CardboardDeviceParams createFromNfcContents(NdefMessage tagContents)
	
	   {			 if (tagContents == null)
	
		 {			   Log.w("CardboardDeviceParams", "Could not get contents from NFC tag.");
			   return null;
	
		 }			 CardboardDeviceParams deviceParams = new CardboardDeviceParams();
			 foreach (NdefRecord record in tagContents.Records)
		 {
			   if (deviceParams.parseNfcUri(record))
		   {
				 break;
	
		   }	
		 }			 return deviceParams;
	
	   }	   
	
	   public virtual NdefMessage NfcTagContents
	   {
		   get
		
		   {					 return this.mNfcTagContents;
		
		   }
	   }	   
	
	   public virtual string Vendor
	   {
		   set
		
		   {					 this.mVendor = value;
		
		   }		   get
		
		   {					 return this.mVendor;
		
		   }
	   }	   
	
	   public virtual string Model
	   {
		   set
		
		   {					 this.mModel = value;
		
		   }		   get
		
		   {					 return this.mModel;
		
		   }
	   }	   
	
	   public virtual string Version
	   {
		   set
		
		   {					 this.mVersion = value;
		
		   }		   get
		
		   {					 return this.mVersion;
		
		   }
	   }	   
	
	   public virtual float InterpupillaryDistance
	   {
		   set
		
		   {					 this.mInterpupillaryDistance = value;
		
		   }		   get
		
		   {					 return this.mInterpupillaryDistance;
		
		   }
	   }	   
	
	   public virtual float VerticalDistanceToLensCenter
	   {
		   set
		
		   {					 this.mVerticalDistanceToLensCenter = value;
		
		   }		   get
		
		   {					 return this.mVerticalDistanceToLensCenter;
		
		   }
	   }	   
	
	   public virtual float VisibleViewportSize
	   {
		   set
		
		   {					 this.mVisibleViewportSize = value;
		
		   }		   get
		
		   {					 return this.mVisibleViewportSize;
		
		   }
	   }	   
	
	   public virtual float FovY
	   {
		   set
		
		   {					 this.mFovY = value;
		
		   }		   get
		
		   {					 return this.mFovY;
		
		   }
	   }	   
	
	   public virtual float LensDiameter
	   {
		   set
		
		   {					 this.mLensDiameter = value;
		
		   }		   get
		
		   {					 return this.mLensDiameter;
		
		   }
	   }	   
	
	   public virtual float ScreenToLensDistance
	   {
		   set
		
		   {					 this.mScreenToLensDistance = value;
		
		   }		   get
		
		   {					 return this.mScreenToLensDistance;
		
		   }
	   }	   
	
	   public virtual float EyeToLensDistance
	   {
		   set
		
		   {					 this.mEyeToLensDistance = value;
		
		   }		   get
		
		   {					 return this.mEyeToLensDistance;
		
		   }
	   }	   
	
	   public virtual Distortion Distortion
	   {
		   get
		
		   {					 return this.mDistortion;
		
		   }
	   }	   
		   public override bool Equals(object other)
	
	   {			 if (other == null)
		 {
			   return false;
	
		 }			 if (other == this)
		 {
			   return true;
	
		 }			 if (!(other is CardboardDeviceParams))
		 {
			   return false;
	
		 }			 CardboardDeviceParams o = (CardboardDeviceParams)other;
	     
			 return (this.mVendor == o.mVendor) && (this.mModel == o.mModel) && (this.mVersion == o.mVersion) && (this.mInterpupillaryDistance == o.mInterpupillaryDistance) && (this.mVerticalDistanceToLensCenter == o.mVerticalDistanceToLensCenter) && (this.mLensDiameter == o.mLensDiameter) && (this.mScreenToLensDistance == o.mScreenToLensDistance) && (this.mEyeToLensDistance == o.mEyeToLensDistance) && (this.mVisibleViewportSize == o.mVisibleViewportSize) && (this.mFovY == o.mFovY) && (this.mDistortion.Equals(o.mDistortion));
	
	   }	   
		   private bool parseNfcUri(NdefRecord record)
	
	   {			 Uri uri = record.toUri();
			 if (uri == null)
		 {
			   return false;
	
		 }			 if (uri.Host.Equals("v1.0.0"))
	
		 {			   this.mVendor = "com.google";
			   this.mModel = "cardboard";
			   this.mVersion = "1.0";
			   return true;
	
		 }			 IList<string> segments = uri.PathSegments;
			 if (segments.Count != 2)
		 {
			   return false;
	
		 }			 this.mVendor = uri.Host;
			 this.mModel = ((string)segments[0]);
			 this.mVersion = ((string)segments[1]);
	     
			 return true;
	
	   }	
	 }

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     com.google.vrtoolkit.cardboard.CardboardDeviceParams
	 * JD-Core Version:    0.7.0.1
	 */
 }