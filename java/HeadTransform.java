/*   1:    */ package com.google.vrtoolkit.cardboard;
/*   2:    */ 
/*   3:    */ import android.opengl.Matrix;
/*   4:    */ import android.util.FloatMath;
/*   5:    */ 
/*   6:    */ public class HeadTransform
/*   7:    */ {
/*   8:    */   private static final float GIMBAL_LOCK_EPSILON = 0.01F;
/*   9:    */   private static final float PI = 3.141593F;
/*  10:    */   private final float[] mHeadView;
/*  11:    */   
/*  12:    */   public HeadTransform()
/*  13:    */   {
/*  14: 35 */     this.mHeadView = new float[16];
/*  15: 36 */     Matrix.setIdentityM(this.mHeadView, 0);
/*  16:    */   }
/*  17:    */   
/*  18:    */   float[] getHeadView()
/*  19:    */   {
/*  20: 48 */     return this.mHeadView;
/*  21:    */   }
/*  22:    */   
/*  23:    */   public void getHeadView(float[] headView, int offset)
/*  24:    */   {
/*  25: 62 */     if (offset + 16 > headView.length) {
/*  26: 63 */       throw new IllegalArgumentException("Not enough space to write the result");
/*  27:    */     }
/*  28: 66 */     System.arraycopy(this.mHeadView, 0, headView, offset, 16);
/*  29:    */   }
/*  30:    */   
/*  31:    */   public void getTranslation(float[] translation, int offset)
/*  32:    */   {
/*  33: 86 */     if (offset + 3 > translation.length) {
/*  34: 87 */       throw new IllegalArgumentException("Not enough space to write the result");
/*  35:    */     }
/*  36: 91 */     for (int i = 0; i < 3; i++) {
/*  37: 92 */       translation[(i + offset)] = this.mHeadView[(12 + i)];
/*  38:    */     }
/*  39:    */   }
/*  40:    */   
/*  41:    */   public void getForwardVector(float[] forward, int offset)
/*  42:    */   {
/*  43:108 */     if (offset + 3 > forward.length) {
/*  44:109 */       throw new IllegalArgumentException("Not enough space to write the result");
/*  45:    */     }
/*  46:113 */     for (int i = 0; i < 3; i++) {
/*  47:114 */       forward[(i + offset)] = (-this.mHeadView[(8 + i)]);
/*  48:    */     }
/*  49:    */   }
/*  50:    */   
/*  51:    */   public void getUpVector(float[] up, int offset)
/*  52:    */   {
/*  53:127 */     if (offset + 3 > up.length) {
/*  54:128 */       throw new IllegalArgumentException("Not enough space to write the result");
/*  55:    */     }
/*  56:132 */     for (int i = 0; i < 3; i++) {
/*  57:133 */       up[(i + offset)] = this.mHeadView[(4 + i)];
/*  58:    */     }
/*  59:    */   }
/*  60:    */   
/*  61:    */   public void getRightVector(float[] right, int offset)
/*  62:    */   {
/*  63:146 */     if (offset + 3 > right.length) {
/*  64:147 */       throw new IllegalArgumentException("Not enough space to write the result");
/*  65:    */     }
/*  66:151 */     for (int i = 0; i < 3; i++) {
/*  67:152 */       right[(i + offset)] = this.mHeadView[i];
/*  68:    */     }
/*  69:    */   }
/*  70:    */   
/*  71:    */   public void getQuaternion(float[] quaternion, int offset)
/*  72:    */   {
/*  73:165 */     if (offset + 4 > quaternion.length) {
/*  74:166 */       throw new IllegalArgumentException("Not enough space to write the result");
/*  75:    */     }
/*  76:170 */     float[] m = this.mHeadView;
/*  77:171 */     float t = m[0] + m[5] + m[10];
/*  78:    */     float z;
/*  79:    */     float z;
/*  80:    */     float x;
/*  81:    */     float y;
/*  82:    */     float w;
/*  83:174 */     if (t >= 0.0F)
/*  84:    */     {
/*  85:175 */       float s = FloatMath.sqrt(t + 1.0F);
/*  86:176 */       float w = 0.5F * s;
/*  87:177 */       s = 0.5F / s;
/*  88:178 */       float x = (m[9] - m[6]) * s;
/*  89:179 */       float y = (m[2] - m[8]) * s;
/*  90:180 */       z = (m[4] - m[1]) * s;
/*  91:    */     }
/*  92:    */     else
/*  93:    */     {
/*  94:    */       float w;
/*  95:182 */       if ((m[0] > m[5]) && (m[0] > m[10]))
/*  96:    */       {
/*  97:183 */         float s = FloatMath.sqrt(1.0F + m[0] - m[5] - m[10]);
/*  98:184 */         float x = s * 0.5F;
/*  99:185 */         s = 0.5F / s;
/* 100:186 */         float y = (m[4] + m[1]) * s;
/* 101:187 */         float z = (m[2] + m[8]) * s;
/* 102:188 */         w = (m[9] - m[6]) * s;
/* 103:    */       }
/* 104:    */       else
/* 105:    */       {
/* 106:    */         float w;
/* 107:190 */         if (m[5] > m[10])
/* 108:    */         {
/* 109:191 */           float s = FloatMath.sqrt(1.0F + m[5] - m[0] - m[10]);
/* 110:192 */           float y = s * 0.5F;
/* 111:193 */           s = 0.5F / s;
/* 112:194 */           float x = (m[4] + m[1]) * s;
/* 113:195 */           float z = (m[9] + m[6]) * s;
/* 114:196 */           w = (m[2] - m[8]) * s;
/* 115:    */         }
/* 116:    */         else
/* 117:    */         {
/* 118:199 */           float s = FloatMath.sqrt(1.0F + m[10] - m[0] - m[5]);
/* 119:200 */           z = s * 0.5F;
/* 120:201 */           s = 0.5F / s;
/* 121:202 */           x = (m[2] + m[8]) * s;
/* 122:203 */           y = (m[9] + m[6]) * s;
/* 123:204 */           w = (m[4] - m[1]) * s;
/* 124:    */         }
/* 125:    */       }
/* 126:    */     }
/* 127:207 */     quaternion[(offset + 0)] = x;
/* 128:208 */     quaternion[(offset + 1)] = y;
/* 129:209 */     quaternion[(offset + 2)] = z;
/* 130:210 */     quaternion[(offset + 3)] = w;
/* 131:    */   }
/* 132:    */   
/* 133:    */   public void getEulerAngles(float[] eulerAngles, int offset)
/* 134:    */   {
/* 135:238 */     if (offset + 3 > eulerAngles.length) {
/* 136:239 */       throw new IllegalArgumentException("Not enough space to write the result");
/* 137:    */     }
/* 138:243 */     float pitch = (float)Math.asin(this.mHeadView[6]);
/* 139:    */     float roll;
/* 140:    */     float yaw;
/* 141:    */     float roll;
/* 142:247 */     if (FloatMath.sqrt(1.0F - this.mHeadView[6] * this.mHeadView[6]) >= 0.01F)
/* 143:    */     {
/* 144:250 */       float yaw = (float)Math.atan2(-this.mHeadView[2], this.mHeadView[10]);
/* 145:251 */       roll = (float)Math.atan2(-this.mHeadView[4], this.mHeadView[5]);
/* 146:    */     }
/* 147:    */     else
/* 148:    */     {
/* 149:255 */       yaw = 0.0F;
/* 150:256 */       roll = (float)Math.atan2(this.mHeadView[1], this.mHeadView[0]);
/* 151:    */     }
/* 152:262 */     eulerAngles[(offset + 0)] = (-pitch);
/* 153:263 */     eulerAngles[(offset + 1)] = (-yaw);
/* 154:264 */     eulerAngles[(offset + 2)] = (-roll);
/* 155:    */   }
/* 156:    */ }


/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
 * Qualified Name:     com.google.vrtoolkit.cardboard.HeadTransform
 * JD-Core Version:    0.7.0.1
 */