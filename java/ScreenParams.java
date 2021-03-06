/*   1:    */ package com.google.vrtoolkit.cardboard;
/*   2:    */ 
/*   3:    */ import android.util.DisplayMetrics;
/*   4:    */ import android.view.Display;
/*   5:    */ 
/*   6:    */ public class ScreenParams
/*   7:    */ {
/*   8:    */   public static final float METERS_PER_INCH = 0.0254F;
/*   9:    */   private static final float DEFAULT_BORDER_SIZE_METERS = 0.003F;
/*  10:    */   private int mWidth;
/*  11:    */   private int mHeight;
/*  12:    */   private float mXMetersPerPixel;
/*  13:    */   private float mYMetersPerPixel;
/*  14:    */   private float mBorderSizeMeters;
/*  15:    */   
/*  16:    */   public ScreenParams(Display display)
/*  17:    */   {
/*  18: 43 */     DisplayMetrics metrics = new DisplayMetrics();
/*  19:    */     try
/*  20:    */     {
/*  21: 47 */       display.getRealMetrics(metrics);
/*  22:    */     }
/*  23:    */     catch (NoSuchMethodError e)
/*  24:    */     {
/*  25: 49 */       display.getMetrics(metrics);
/*  26:    */     }
/*  27: 52 */     this.mXMetersPerPixel = (0.0254F / metrics.xdpi);
/*  28: 53 */     this.mYMetersPerPixel = (0.0254F / metrics.ydpi);
/*  29: 54 */     this.mWidth = metrics.widthPixels;
/*  30: 55 */     this.mHeight = metrics.heightPixels;
/*  31: 56 */     this.mBorderSizeMeters = 0.003F;
/*  32: 61 */     if (this.mHeight > this.mWidth)
/*  33:    */     {
/*  34: 62 */       int tempPx = this.mWidth;
/*  35: 63 */       this.mWidth = this.mHeight;
/*  36: 64 */       this.mHeight = tempPx;
/*  37:    */       
/*  38: 66 */       float tempMetersPerPixel = this.mXMetersPerPixel;
/*  39: 67 */       this.mXMetersPerPixel = this.mYMetersPerPixel;
/*  40: 68 */       this.mYMetersPerPixel = tempMetersPerPixel;
/*  41:    */     }
/*  42:    */   }
/*  43:    */   
/*  44:    */   public ScreenParams(ScreenParams params)
/*  45:    */   {
/*  46: 78 */     this.mWidth = params.mWidth;
/*  47: 79 */     this.mHeight = params.mHeight;
/*  48: 80 */     this.mXMetersPerPixel = params.mXMetersPerPixel;
/*  49: 81 */     this.mYMetersPerPixel = params.mYMetersPerPixel;
/*  50: 82 */     this.mBorderSizeMeters = params.mBorderSizeMeters;
/*  51:    */   }
/*  52:    */   
/*  53:    */   public void setWidth(int width)
/*  54:    */   {
/*  55: 91 */     this.mWidth = width;
/*  56:    */   }
/*  57:    */   
/*  58:    */   public int getWidth()
/*  59:    */   {
/*  60:100 */     return this.mWidth;
/*  61:    */   }
/*  62:    */   
/*  63:    */   public void setHeight(int height)
/*  64:    */   {
/*  65:109 */     this.mHeight = height;
/*  66:    */   }
/*  67:    */   
/*  68:    */   public int getHeight()
/*  69:    */   {
/*  70:118 */     return this.mHeight;
/*  71:    */   }
/*  72:    */   
/*  73:    */   public float getWidthMeters()
/*  74:    */   {
/*  75:127 */     return this.mWidth * this.mXMetersPerPixel;
/*  76:    */   }
/*  77:    */   
/*  78:    */   public float getHeightMeters()
/*  79:    */   {
/*  80:136 */     return this.mHeight * this.mYMetersPerPixel;
/*  81:    */   }
/*  82:    */   
/*  83:    */   public void setBorderSizeMeters(float screenBorderSize)
/*  84:    */   {
/*  85:148 */     this.mBorderSizeMeters = screenBorderSize;
/*  86:    */   }
/*  87:    */   
/*  88:    */   public float getBorderSizeMeters()
/*  89:    */   {
/*  90:157 */     return this.mBorderSizeMeters;
/*  91:    */   }
/*  92:    */   
/*  93:    */   public boolean equals(Object other)
/*  94:    */   {
/*  95:168 */     if (other == null) {
/*  96:169 */       return false;
/*  97:    */     }
/*  98:172 */     if (other == this) {
/*  99:173 */       return true;
/* 100:    */     }
/* 101:176 */     if (!(other instanceof ScreenParams)) {
/* 102:177 */       return false;
/* 103:    */     }
/* 104:180 */     ScreenParams o = (ScreenParams)other;
/* 105:    */     
/* 106:182 */     return (this.mWidth == o.mWidth) && (this.mHeight == o.mHeight) && (this.mXMetersPerPixel == o.mXMetersPerPixel) && (this.mYMetersPerPixel == o.mYMetersPerPixel) && (this.mBorderSizeMeters == o.mBorderSizeMeters);
/* 107:    */   }
/* 108:    */ }


/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
 * Qualified Name:     com.google.vrtoolkit.cardboard.ScreenParams
 * JD-Core Version:    0.7.0.1
 */