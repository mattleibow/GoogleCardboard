/*   1:    */ package com.google.vrtoolkit.cardboard;
/*   2:    */ 
/*   3:    */ import android.view.Display;
/*   4:    */ 
/*   5:    */ public class HeadMountedDisplay
/*   6:    */ {
/*   7:    */   private ScreenParams mScreen;
/*   8:    */   private CardboardDeviceParams mCardboard;
/*   9:    */   
/*  10:    */   public HeadMountedDisplay(Display display)
/*  11:    */   {
/*  12: 38 */     this.mScreen = new ScreenParams(display);
/*  13: 39 */     this.mCardboard = new CardboardDeviceParams();
/*  14:    */   }
/*  15:    */   
/*  16:    */   public HeadMountedDisplay(HeadMountedDisplay hmd)
/*  17:    */   {
/*  18: 48 */     this.mScreen = new ScreenParams(hmd.mScreen);
/*  19: 49 */     this.mCardboard = new CardboardDeviceParams(hmd.mCardboard);
/*  20:    */   }
/*  21:    */   
/*  22:    */   public void setScreen(ScreenParams screen)
/*  23:    */   {
/*  24: 58 */     this.mScreen = new ScreenParams(screen);
/*  25:    */   }
/*  26:    */   
/*  27:    */   public ScreenParams getScreen()
/*  28:    */   {
/*  29: 67 */     return this.mScreen;
/*  30:    */   }
/*  31:    */   
/*  32:    */   public void setCardboard(CardboardDeviceParams cardboard)
/*  33:    */   {
/*  34: 76 */     this.mCardboard = new CardboardDeviceParams(cardboard);
/*  35:    */   }
/*  36:    */   
/*  37:    */   public CardboardDeviceParams getCardboard()
/*  38:    */   {
/*  39: 85 */     return this.mCardboard;
/*  40:    */   }
/*  41:    */   
/*  42:    */   public boolean equals(Object other)
/*  43:    */   {
/*  44: 96 */     if (other == null) {
/*  45: 96 */       return false;
/*  46:    */     }
/*  47: 97 */     if (other == this) {
/*  48: 97 */       return true;
/*  49:    */     }
/*  50: 98 */     if (!(other instanceof HeadMountedDisplay)) {
/*  51: 98 */       return false;
/*  52:    */     }
/*  53: 99 */     HeadMountedDisplay o = (HeadMountedDisplay)other;
/*  54:    */     
/*  55:101 */     return (this.mScreen.equals(o.mScreen)) && (this.mCardboard.equals(o.mCardboard));
/*  56:    */   }
/*  57:    */ }


/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
 * Qualified Name:     com.google.vrtoolkit.cardboard.HeadMountedDisplay
 * JD-Core Version:    0.7.0.1
 */