using System.Collections.Generic;

/*   1:    */ namespace com.google.vrtoolkit.cardboard
 {
	/*   2:    */ 
	/*   3:    */	 using Uri = android.net.Uri;
	/*   4:    */	 using NdefMessage = android.nfc.NdefMessage;
	/*   5:    */	 using NdefRecord = android.nfc.NdefRecord;
	/*   6:    */	 using Log = android.util.Log;
	/*   7:    */	/*   8:    */ 
	/*   9:    */	 public class CardboardDeviceParams
	/*  10:    */
	 {	/*  11:    */	   private const string TAG = "CardboardDeviceParams";
	/*  12:    */	   private const string DEFAULT_VENDOR = "com.google";
	/*  13:    */	   private const string DEFAULT_MODEL = "cardboard";
	/*  14:    */	   private const string DEFAULT_VERSION = "1.0";
	/*  15:    */	   private const float DEFAULT_INTERPUPILLARY_DISTANCE = 0.06F;
	/*  16:    */	   private const float DEFAULT_VERTICAL_DISTANCE_TO_LENS_CENTER = 0.035F;
	/*  17:    */	   private const float DEFAULT_LENS_DIAMETER = 0.025F;
	/*  18:    */	   private const float DEFAULT_SCREEN_TO_LENS_DISTANCE = 0.037F;
	/*  19:    */	   private const float DEFAULT_EYE_TO_LENS_DISTANCE = 0.011F;
	/*  20:    */	   private const float DEFAULT_VISIBLE_VIEWPORT_MAX_SIZE = 0.06F;
	/*  21:    */	   private const float DEFAULT_FOV_Y = 65.0F;
	/*  22:    */	   private NdefMessage mNfcTagContents;
	/*  23:    */	   private string mVendor;
	/*  24:    */	   private string mModel;
	/*  25:    */	   private string mVersion;
	/*  26:    */	   private float mInterpupillaryDistance;
	/*  27:    */	   private float mVerticalDistanceToLensCenter;
	/*  28:    */	   private float mLensDiameter;
	/*  29:    */	   private float mScreenToLensDistance;
	/*  30:    */	   private float mEyeToLensDistance;
	/*  31:    */	   private float mVisibleViewportSize;
	/*  32:    */	   private float mFovY;
	/*  33:    */	   private Distortion mDistortion;
	/*  34:    */   
	/*  35:    */	   public CardboardDeviceParams()
	/*  36:    */
	   {	/*  37: 94 */		 this.mVendor = "com.google";
	/*  38: 95 */		 this.mModel = "cardboard";
	/*  39: 96 */		 this.mVersion = "1.0";
	/*  40:    */     
	/*  41: 98 */		 this.mInterpupillaryDistance = 0.06F;
	/*  42: 99 */		 this.mVerticalDistanceToLensCenter = 0.035F;
	/*  43:100 */		 this.mLensDiameter = 0.025F;
	/*  44:101 */		 this.mScreenToLensDistance = 0.037F;
	/*  45:102 */		 this.mEyeToLensDistance = 0.011F;
	/*  46:    */     
	/*  47:104 */		 this.mVisibleViewportSize = 0.06F;
	/*  48:105 */		 this.mFovY = 65.0F;
	/*  49:    */     
	/*  50:107 */		 this.mDistortion = new Distortion();
	/*  51:    */
	   }	/*  52:    */   
	/*  53:    */	   public CardboardDeviceParams(CardboardDeviceParams @params)
	/*  54:    */
	   {	/*  55:116 */		 this.mNfcTagContents = @params.mNfcTagContents;
	/*  56:    */     
	/*  57:118 */		 this.mVendor = @params.mVendor;
	/*  58:119 */		 this.mModel = @params.mModel;
	/*  59:120 */		 this.mVersion = @params.mVersion;
	/*  60:    */     
	/*  61:122 */		 this.mInterpupillaryDistance = @params.mInterpupillaryDistance;
	/*  62:123 */		 this.mVerticalDistanceToLensCenter = @params.mVerticalDistanceToLensCenter;
	/*  63:124 */		 this.mLensDiameter = @params.mLensDiameter;
	/*  64:125 */		 this.mScreenToLensDistance = @params.mScreenToLensDistance;
	/*  65:126 */		 this.mEyeToLensDistance = @params.mEyeToLensDistance;
	/*  66:    */     
	/*  67:128 */		 this.mVisibleViewportSize = @params.mVisibleViewportSize;
	/*  68:129 */		 this.mFovY = @params.mFovY;
	/*  69:    */     
	/*  70:131 */		 this.mDistortion = new Distortion(@params.mDistortion);
	/*  71:    */
	   }	/*  72:    */   
	/*  73:    */	   public static CardboardDeviceParams createFromNfcContents(NdefMessage tagContents)
	/*  74:    */
	   {	/*  75:140 */		 if (tagContents == null)
	/*  76:    */
		 {	/*  77:141 */		   Log.w("CardboardDeviceParams", "Could not get contents from NFC tag.");
	/*  78:142 */		   return null;
	/*  79:    */
		 }	/*  80:145 */		 CardboardDeviceParams deviceParams = new CardboardDeviceParams();
	/*  81:148 */		 foreach (NdefRecord record in tagContents.Records)
		 {
	/*  82:149 */		   if (deviceParams.parseNfcUri(record))
		   {
	/*  83:    */			 break;
	/*  84:    */
		   }	/*  85:    */
		 }	/*  86:156 */		 return deviceParams;
	/*  87:    */
	   }	/*  88:    */   
	/*  89:    */
	   public virtual NdefMessage NfcTagContents
	   {
		   get
		/*  90:    */
		   {		/*  91:167 */			 return this.mNfcTagContents;
		/*  92:    */
		   }
	   }	/*  93:    */   
	/*  94:    */
	   public virtual string Vendor
	   {
		   set
		/*  95:    */
		   {		/*  96:178 */			 this.mVendor = value;
		/*  97:    */
		   }		   get
		/* 100:    */
		   {		/* 101:187 */			 return this.mVendor;
		/* 102:    */
		   }
	   }	/* 103:    */   
	/* 104:    */
	   public virtual string Model
	   {
		   set
		/* 105:    */
		   {		/* 106:196 */			 this.mModel = value;
		/* 107:    */
		   }		   get
		/* 110:    */
		   {		/* 111:205 */			 return this.mModel;
		/* 112:    */
		   }
	   }	/* 113:    */   
	/* 114:    */
	   public virtual string Version
	   {
		   set
		/* 115:    */
		   {		/* 116:214 */			 this.mVersion = value;
		/* 117:    */
		   }		   get
		/* 120:    */
		   {		/* 121:223 */			 return this.mVersion;
		/* 122:    */
		   }
	   }	/* 123:    */   
	/* 124:    */
	   public virtual float InterpupillaryDistance
	   {
		   set
		/* 125:    */
		   {		/* 126:232 */			 this.mInterpupillaryDistance = value;
		/* 127:    */
		   }		   get
		/* 130:    */
		   {		/* 131:241 */			 return this.mInterpupillaryDistance;
		/* 132:    */
		   }
	   }	/* 133:    */   
	/* 134:    */
	   public virtual float VerticalDistanceToLensCenter
	   {
		   set
		/* 135:    */
		   {		/* 136:251 */			 this.mVerticalDistanceToLensCenter = value;
		/* 137:    */
		   }		   get
		/* 140:    */
		   {		/* 141:261 */			 return this.mVerticalDistanceToLensCenter;
		/* 142:    */
		   }
	   }	/* 143:    */   
	/* 144:    */
	   public virtual float VisibleViewportSize
	   {
		   set
		/* 145:    */
		   {		/* 146:274 */			 this.mVisibleViewportSize = value;
		/* 147:    */
		   }		   get
		/* 150:    */
		   {		/* 151:287 */			 return this.mVisibleViewportSize;
		/* 152:    */
		   }
	   }	/* 153:    */   
	/* 154:    */
	   public virtual float FovY
	   {
		   set
		/* 155:    */
		   {		/* 156:300 */			 this.mFovY = value;
		/* 157:    */
		   }		   get
		/* 160:    */
		   {		/* 161:312 */			 return this.mFovY;
		/* 162:    */
		   }
	   }	/* 163:    */   
	/* 164:    */
	   public virtual float LensDiameter
	   {
		   set
		/* 165:    */
		   {		/* 166:321 */			 this.mLensDiameter = value;
		/* 167:    */
		   }		   get
		/* 170:    */
		   {		/* 171:330 */			 return this.mLensDiameter;
		/* 172:    */
		   }
	   }	/* 173:    */   
	/* 174:    */
	   public virtual float ScreenToLensDistance
	   {
		   set
		/* 175:    */
		   {		/* 176:339 */			 this.mScreenToLensDistance = value;
		/* 177:    */
		   }		   get
		/* 180:    */
		   {		/* 181:348 */			 return this.mScreenToLensDistance;
		/* 182:    */
		   }
	   }	/* 183:    */   
	/* 184:    */
	   public virtual float EyeToLensDistance
	   {
		   set
		/* 185:    */
		   {		/* 186:365 */			 this.mEyeToLensDistance = value;
		/* 187:    */
		   }		   get
		/* 190:    */
		   {		/* 191:376 */			 return this.mEyeToLensDistance;
		/* 192:    */
		   }
	   }	/* 193:    */   
	/* 194:    */
	   public virtual Distortion Distortion
	   {
		   get
		/* 195:    */
		   {		/* 196:385 */			 return this.mDistortion;
		/* 197:    */
		   }
	   }	/* 198:    */   
	/* 199:    */	   public override bool Equals(object other)
	/* 200:    */
	   {	/* 201:396 */		 if (other == null)
		 {
	/* 202:397 */		   return false;
	/* 203:    */
		 }	/* 204:400 */		 if (other == this)
		 {
	/* 205:401 */		   return true;
	/* 206:    */
		 }	/* 207:404 */		 if (!(other is CardboardDeviceParams))
		 {
	/* 208:405 */		   return false;
	/* 209:    */
		 }	/* 210:408 */		 CardboardDeviceParams o = (CardboardDeviceParams)other;
	/* 211:    */     
	/* 212:410 */		 return (this.mVendor == o.mVendor) && (this.mModel == o.mModel) && (this.mVersion == o.mVersion) && (this.mInterpupillaryDistance == o.mInterpupillaryDistance) && (this.mVerticalDistanceToLensCenter == o.mVerticalDistanceToLensCenter) && (this.mLensDiameter == o.mLensDiameter) && (this.mScreenToLensDistance == o.mScreenToLensDistance) && (this.mEyeToLensDistance == o.mEyeToLensDistance) && (this.mVisibleViewportSize == o.mVisibleViewportSize) && (this.mFovY == o.mFovY) && (this.mDistortion.Equals(o.mDistortion));
	/* 213:    */
	   }	/* 214:    */   
	/* 215:    */	   private bool parseNfcUri(NdefRecord record)
	/* 216:    */
	   {	/* 217:424 */		 Uri uri = record.toUri();
	/* 218:425 */		 if (uri == null)
		 {
	/* 219:426 */		   return false;
	/* 220:    */
		 }	/* 221:430 */		 if (uri.Host.Equals("v1.0.0"))
	/* 222:    */
		 {	/* 223:431 */		   this.mVendor = "com.google";
	/* 224:432 */		   this.mModel = "cardboard";
	/* 225:433 */		   this.mVersion = "1.0";
	/* 226:434 */		   return true;
	/* 227:    */
		 }	/* 228:437 */		 IList<string> segments = uri.PathSegments;
	/* 229:438 */		 if (segments.Count != 2)
		 {
	/* 230:439 */		   return false;
	/* 231:    */
		 }	/* 232:442 */		 this.mVendor = uri.Host;
	/* 233:443 */		 this.mModel = ((string)segments[0]);
	/* 234:444 */		 this.mVersion = ((string)segments[1]);
	/* 235:    */     
	/* 236:446 */		 return true;
	/* 237:    */
	   }	/* 238:    */
	 }

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     com.google.vrtoolkit.cardboard.CardboardDeviceParams
	 * JD-Core Version:    0.7.0.1
	 */
 }