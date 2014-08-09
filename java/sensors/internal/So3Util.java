/*   1:    */ package com.google.vrtoolkit.cardboard.sensors.internal;
/*   2:    */ 
/*   3:    */ public class So3Util
/*   4:    */ {
/*   5:    */   private static final double M_SQRT1_2 = 0.7071067811865476D;
/*   6:    */   private static final double ONE_6TH = 0.16666667163372D;
/*   7:    */   private static final double ONE_20TH = 0.16666667163372D;
/*   8: 17 */   private static Vector3d temp31 = new Vector3d();
/*   9: 18 */   private static Vector3d sO3FromTwoVecN = new Vector3d();
/*  10: 19 */   private static Vector3d sO3FromTwoVecA = new Vector3d();
/*  11: 20 */   private static Vector3d sO3FromTwoVecB = new Vector3d();
/*  12: 21 */   private static Vector3d sO3FromTwoVecRotationAxis = new Vector3d();
/*  13: 22 */   private static Matrix3x3d sO3FromTwoVec33R1 = new Matrix3x3d();
/*  14: 23 */   private static Matrix3x3d sO3FromTwoVec33R2 = new Matrix3x3d();
/*  15: 24 */   private static Vector3d muFromSO3R2 = new Vector3d();
/*  16: 25 */   private static Vector3d rotationPiAboutAxisTemp = new Vector3d();
/*  17:    */   
/*  18:    */   public static void sO3FromTwoVec(Vector3d a, Vector3d b, Matrix3x3d result)
/*  19:    */   {
/*  20: 38 */     Vector3d.cross(a, b, sO3FromTwoVecN);
/*  21: 39 */     if (sO3FromTwoVecN.length() == 0.0D)
/*  22:    */     {
/*  23: 41 */       double dot = Vector3d.dot(a, b);
/*  24: 42 */       if (dot >= 0.0D)
/*  25:    */       {
/*  26: 43 */         result.setIdentity();
/*  27:    */       }
/*  28:    */       else
/*  29:    */       {
/*  30: 45 */         Vector3d.ortho(a, sO3FromTwoVecRotationAxis);
/*  31: 46 */         rotationPiAboutAxis(sO3FromTwoVecRotationAxis, result);
/*  32:    */       }
/*  33: 48 */       return;
/*  34:    */     }
/*  35: 52 */     sO3FromTwoVecA.set(a);
/*  36: 53 */     sO3FromTwoVecB.set(b);
/*  37:    */     
/*  38:    */ 
/*  39: 56 */     sO3FromTwoVecN.normalize();
/*  40: 57 */     sO3FromTwoVecA.normalize();
/*  41: 58 */     sO3FromTwoVecB.normalize();
/*  42:    */     
/*  43: 60 */     Matrix3x3d r1 = sO3FromTwoVec33R1;
/*  44: 61 */     r1.setColumn(0, sO3FromTwoVecA);
/*  45: 62 */     r1.setColumn(1, sO3FromTwoVecN);
/*  46: 63 */     Vector3d.cross(sO3FromTwoVecN, sO3FromTwoVecA, temp31);
/*  47: 64 */     r1.setColumn(2, temp31);
/*  48:    */     
/*  49: 66 */     Matrix3x3d r2 = sO3FromTwoVec33R2;
/*  50: 67 */     r2.setColumn(0, sO3FromTwoVecB);
/*  51: 68 */     r2.setColumn(1, sO3FromTwoVecN);
/*  52: 69 */     Vector3d.cross(sO3FromTwoVecN, sO3FromTwoVecB, temp31);
/*  53: 70 */     r2.setColumn(2, temp31);
/*  54:    */     
/*  55: 72 */     r1.transpose();
/*  56: 73 */     Matrix3x3d.mult(r2, r1, result);
/*  57:    */   }
/*  58:    */   
/*  59:    */   private static void rotationPiAboutAxis(Vector3d v, Matrix3x3d result)
/*  60:    */   {
/*  61: 78 */     rotationPiAboutAxisTemp.set(v);
/*  62: 79 */     rotationPiAboutAxisTemp.scale(3.141592653589793D / rotationPiAboutAxisTemp.length());
/*  63: 80 */     double invTheta = 0.3183098861837907D;
/*  64:    */     
/*  65: 82 */     double kA = 0.0D;
/*  66:    */     
/*  67: 84 */     double kB = 0.2026423672846756D;
/*  68: 85 */     rodriguesSo3Exp(rotationPiAboutAxisTemp, kA, kB, result);
/*  69:    */   }
/*  70:    */   
/*  71:    */   public static void sO3FromMu(Vector3d w, Matrix3x3d result)
/*  72:    */   {
/*  73: 95 */     double thetaSq = Vector3d.dot(w, w);
/*  74: 96 */     double theta = Math.sqrt(thetaSq);
/*  75:    */     double kB;
/*  76:    */     double kA;
/*  77:    */     double kB;
/*  78: 99 */     if (thetaSq < 1.0E-008D)
/*  79:    */     {
/*  80:100 */       double kA = 1.0D - 0.16666667163372D * thetaSq;
/*  81:101 */       kB = 0.5D;
/*  82:    */     }
/*  83:    */     else
/*  84:    */     {
/*  85:    */       double kA;
/*  86:103 */       if (thetaSq < 1.0E-006D)
/*  87:    */       {
/*  88:104 */         double kB = 0.5D - 0.0416666679084301D * thetaSq;
/*  89:105 */         kA = 1.0D - thetaSq * 0.16666667163372D * (1.0D - 0.16666667163372D * thetaSq);
/*  90:    */       }
/*  91:    */       else
/*  92:    */       {
/*  93:107 */         double invTheta = 1.0D / theta;
/*  94:108 */         kA = Math.sin(theta) * invTheta;
/*  95:109 */         kB = (1.0D - Math.cos(theta)) * (invTheta * invTheta);
/*  96:    */       }
/*  97:    */     }
/*  98:112 */     rodriguesSo3Exp(w, kA, kB, result);
/*  99:    */   }
/* 100:    */   
/* 101:    */   public static void muFromSO3(Matrix3x3d so3, Vector3d result)
/* 102:    */   {
/* 103:122 */     double cosAngle = (so3.get(0, 0) + so3.get(1, 1) + so3.get(2, 2) - 1.0D) * 0.5D;
/* 104:    */     
/* 105:124 */     result.set((so3.get(2, 1) - so3.get(1, 2)) / 2.0D, (so3.get(0, 2) - so3.get(2, 0)) / 2.0D, (so3.get(1, 0) - so3.get(0, 1)) / 2.0D);
/* 106:    */     
/* 107:    */ 
/* 108:    */ 
/* 109:128 */     double sinAngleAbs = result.length();
/* 110:129 */     if (cosAngle > 0.7071067811865476D)
/* 111:    */     {
/* 112:131 */       if (sinAngleAbs > 0.0D) {
/* 113:132 */         result.scale(Math.asin(sinAngleAbs) / sinAngleAbs);
/* 114:    */       }
/* 115:    */     }
/* 116:134 */     else if (cosAngle > -0.7071067811865476D)
/* 117:    */     {
/* 118:136 */       double angle = Math.acos(cosAngle);
/* 119:137 */       result.scale(angle / sinAngleAbs);
/* 120:    */     }
/* 121:    */     else
/* 122:    */     {
/* 123:142 */       double angle = 3.141592653589793D - Math.asin(sinAngleAbs);
/* 124:143 */       double d0 = so3.get(0, 0) - cosAngle;
/* 125:144 */       double d1 = so3.get(1, 1) - cosAngle;
/* 126:145 */       double d2 = so3.get(2, 2) - cosAngle;
/* 127:    */       
/* 128:147 */       Vector3d r2 = muFromSO3R2;
/* 129:148 */       if ((d0 * d0 > d1 * d1) && (d0 * d0 > d2 * d2)) {
/* 130:150 */         r2.set(d0, (so3.get(1, 0) + so3.get(0, 1)) / 2.0D, (so3.get(0, 2) + so3.get(2, 0)) / 2.0D);
/* 131:152 */       } else if (d1 * d1 > d2 * d2) {
/* 132:154 */         r2.set((so3.get(1, 0) + so3.get(0, 1)) / 2.0D, d1, (so3.get(2, 1) + so3.get(1, 2)) / 2.0D);
/* 133:    */       } else {
/* 134:158 */         r2.set((so3.get(0, 2) + so3.get(2, 0)) / 2.0D, (so3.get(2, 1) + so3.get(1, 2)) / 2.0D, d2);
/* 135:    */       }
/* 136:162 */       if (Vector3d.dot(r2, result) < 0.0D) {
/* 137:163 */         r2.scale(-1.0D);
/* 138:    */       }
/* 139:165 */       r2.normalize();
/* 140:166 */       r2.scale(angle);
/* 141:167 */       result.set(r2);
/* 142:    */     }
/* 143:    */   }
/* 144:    */   
/* 145:    */   private static void rodriguesSo3Exp(Vector3d w, double kA, double kB, Matrix3x3d result)
/* 146:    */   {
/* 147:185 */     double wx2 = w.x * w.x;
/* 148:186 */     double wy2 = w.y * w.y;
/* 149:187 */     double wz2 = w.z * w.z;
/* 150:    */     
/* 151:189 */     result.set(0, 0, 1.0D - kB * (wy2 + wz2));
/* 152:190 */     result.set(1, 1, 1.0D - kB * (wx2 + wz2));
/* 153:191 */     result.set(2, 2, 1.0D - kB * (wx2 + wy2));
/* 154:    */     
/* 155:    */ 
/* 156:194 */     double a = kA * w.z;
/* 157:195 */     double b = kB * (w.x * w.y);
/* 158:196 */     result.set(0, 1, b - a);
/* 159:197 */     result.set(1, 0, b + a);
/* 160:    */     
/* 161:    */ 
/* 162:200 */     double a = kA * w.y;
/* 163:201 */     double b = kB * (w.x * w.z);
/* 164:202 */     result.set(0, 2, b + a);
/* 165:203 */     result.set(2, 0, b - a);
/* 166:    */     
/* 167:    */ 
/* 168:206 */     double a = kA * w.x;
/* 169:207 */     double b = kB * (w.y * w.z);
/* 170:208 */     result.set(1, 2, b - a);
/* 171:209 */     result.set(2, 1, b + a);
/* 172:    */   }
/* 173:    */   
/* 174:    */   public static void generatorField(int i, Matrix3x3d pos, Matrix3x3d result)
/* 175:    */   {
/* 176:221 */     result.set(i, 0, 0.0D);
/* 177:222 */     result.set((i + 1) % 3, 0, -pos.get((i + 2) % 3, 0));
/* 178:223 */     result.set((i + 2) % 3, 0, pos.get((i + 1) % 3, 0));
/* 179:    */   }
/* 180:    */ }


/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
 * Qualified Name:     com.google.vrtoolkit.cardboard.sensors.internal.So3Util
 * JD-Core Version:    0.7.0.1
 */