﻿////
//// System.Numerics.BigInteger
////
//// Authors:
////	Rodrigo Kumpera (rkumpera@novell.com)
////	Marek Safar  <marek.safar@gmail.com>
////
//// Copyright (C) 2010 Novell, Inc (http://www.novell.com)
//// Copyright (C) 2014 Xamarin Inc (http://www.xamarin.com)
////
//// Permission is hereby granted, free of charge, to any person obtaining
//// a copy of this software and associated documentation files (the
//// "Software"), to deal in the Software without restriction, including
//// without limitation the rights to use, copy, modify, merge, publish,
//// distribute, sublicense, and/or sell copies of the Software, and to
//// permit persons to whom the Software is furnished to do so, subject to
//// the following conditions:
//// 
//// The above copyright notice and this permission notice shall be
//// included in all copies or substantial portions of the Software.
//// 
//// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////
//// A big chuck of code comes the DLR (as hosted in http://ironpython.codeplex.com), 
//// which has the following License:
////

// *
// * Copyright (c) Microsoft Corporation. 
// *
// * This source code is subject to terms and conditions of the Microsoft Public License. A 
// * copy of the license can be found in the License.html file at the root of this distribution. If 
// * you cannot locate the  Microsoft Public License, please send an email to 
// * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
// * by the terms of the Microsoft Public License.
// *
// * You must not remove this notice, or any other, from this software.
// *
// *
// * ***************************************************************************/

using System;

//https://github.com/mono/mono/blob/mono-3.12.1/mcs/class/System.Numerics/System.Numerics/BigInteger.cs

namespace Bytewizer.TinyCLR.Numerics
{
    public struct BigInteger //: IComparable, IFormattable, IComparable<BigInteger>, IEquatable<BigInteger>
    {
        //LSB on [0]
        readonly uint[] data;
        readonly short sign;

        static readonly uint[] ONE = new uint[1] { 1 };

        BigInteger(short sign, uint[] data)
        {
            this.sign = sign;
            this.data = data;
        }

        public BigInteger(int value)
        {
            if (value == 0)
            {
                sign = 0;
                data = null;
            }
            else if (value > 0)
            {
                sign = 1;
                data = new uint[] { (uint)value };
            }
            else
            {
                sign = -1;
                data = new uint[1] { (uint)-value };
            }
        }

        //		[CLSCompliantAttribute(false)]
        //		public BigInteger(uint value)
        //		{
        //			if (value == 0)
        //			{
        //				sign = 0;
        //				data = null;
        //			}
        //			else
        //			{
        //				sign = 1;
        //				data = new uint[1] { value };
        //			}
        //		}

        //		public BigInteger(long value)
        //		{
        //			if (value == 0)
        //			{
        //				sign = 0;
        //				data = null;
        //			}
        //			else if (value > 0)
        //			{
        //				sign = 1;
        //				uint low = (uint)value;
        //				uint high = (uint)(value >> 32);

        //				data = new uint[high != 0 ? 2 : 1];
        //				data[0] = low;
        //				if (high != 0)
        //					data[1] = high;
        //			}
        //			else
        //			{
        //				sign = -1;
        //				value = -value;
        //				uint low = (uint)value;
        //				uint high = (uint)((ulong)value >> 32);

        //				data = new uint[high != 0 ? 2 : 1];
        //				data[0] = low;
        //				if (high != 0)
        //					data[1] = high;
        //			}
        //		}

        //		[CLSCompliantAttribute(false)]
        //		public BigInteger(ulong value)
        //		{
        //			if (value == 0)
        //			{
        //				sign = 0;
        //				data = null;
        //			}
        //			else
        //			{
        //				sign = 1;
        //				uint low = (uint)value;
        //				uint high = (uint)(value >> 32);

        //				data = new uint[high != 0 ? 2 : 1];
        //				data[0] = low;
        //				if (high != 0)
        //					data[1] = high;
        //			}
        //		}


        //		static bool Negative(byte[] v)
        //		{
        //			return ((v[7] & 0x80) != 0);
        //		}

        //		static ushort Exponent(byte[] v)
        //		{
        //			return (ushort)((((ushort)(v[7] & 0x7F)) << (ushort)4) | (((ushort)(v[6] & 0xF0)) >> 4));
        //		}

        //		static ulong Mantissa(byte[] v)
        //		{
        //			uint i1 = ((uint)v[0] | ((uint)v[1] << 8) | ((uint)v[2] << 16) | ((uint)v[3] << 24));
        //			uint i2 = ((uint)v[4] | ((uint)v[5] << 8) | ((uint)(v[6] & 0xF) << 16));

        //			return (ulong)((ulong)i1 | ((ulong)i2 << 32));
        //		}

        //		const int bias = 1075;
        //		public BigInteger(double value)
        //		{
        //			if (double.IsNaN(value) || Double.IsInfinity(value))
        //				throw new OverflowException();

        //			byte[] bytes = BitConverter.GetBytes(value);
        //			ulong mantissa = Mantissa(bytes);
        //			if (mantissa == 0)
        //			{
        //				// 1.0 * 2**exp, we have a power of 2
        //				int exponent = Exponent(bytes);
        //				if (exponent == 0)
        //				{
        //					sign = 0;
        //					data = null;
        //					return;
        //				}

        //				BigInteger res = Negative(bytes) ? MinusOne : One;
        //				res = res << (exponent - 0x3ff);
        //				this.sign = res.sign;
        //				this.data = res.data;
        //			}
        //			else
        //			{
        //				// 1.mantissa * 2**exp
        //				int exponent = Exponent(bytes);
        //				mantissa |= 0x10000000000000ul;
        //				BigInteger res = mantissa;
        //				res = exponent > bias ? res << (exponent - bias) : res >> (bias - exponent);

        //				this.sign = (short)(Negative(bytes) ? -1 : 1);
        //				this.data = res.data;
        //			}
        //		}

        //		public BigInteger(float value) : this((double)value)
        //		{
        //		}

        //		const Int32 DecimalScaleFactorMask = 0x00FF0000;
        //		const Int32 DecimalSignMask = unchecked((Int32)0x80000000);

        //		public BigInteger(decimal value)
        //		{
        //			// First truncate to get scale to 0 and extract bits
        //			int[] bits = Decimal.GetBits(Decimal.Truncate(value));

        //			int size = 3;
        //			while (size > 0 && bits[size - 1] == 0) size--;

        //			if (size == 0)
        //			{
        //				sign = 0;
        //				data = null;
        //				return;
        //			}

        //			sign = (short)((bits[3] & DecimalSignMask) != 0 ? -1 : 1);

        //			data = new uint[size];
        //			data[0] = (uint)bits[0];
        //			if (size > 1)
        //				data[1] = (uint)bits[1];
        //			if (size > 2)
        //				data[2] = (uint)bits[2];
        //		}

        //		[CLSCompliantAttribute(false)]
        public BigInteger(byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            int len = value.Length;

            if (len == 0 || (len == 1 && value[0] == 0))
            {
                sign = 0;
                data = null;
                return;
            }

            if ((value[len - 1] & 0x80) != 0)
                sign = -1;
            else
                sign = 1;

            if (sign == 1)
            {
                while (value[len - 1] == 0)
                {
                    if (--len == 0)
                    {
                        sign = 0;
                        data = null;
                        return;
                    }
                }

                int full_words, size;
                full_words = size = len / 4;
                if ((len & 0x3) != 0)
                    ++size;

                data = new uint[size];
                int j = 0;
                for (int i = 0; i < full_words; ++i)
                {
                    data[i] = (uint)value[j++] |
                                (uint)(value[j++] << 8) |
                                (uint)(value[j++] << 16) |
                                (uint)(value[j++] << 24);
                }
                size = len & 0x3;
                if (size > 0)
                {
                    int idx = data.Length - 1;
                    for (int i = 0; i < size; ++i)
                        data[idx] |= (uint)(value[j++] << (i * 8));
                }
            }
            else
            {
                int full_words, size;
                full_words = size = len / 4;
                if ((len & 0x3) != 0)
                    ++size;

                data = new uint[size];

                uint word, borrow = 1;
                ulong sub = 0;
                int j = 0;

                for (int i = 0; i < full_words; ++i)
                {
                    word = (uint)value[j++] |
                            (uint)(value[j++] << 8) |
                            (uint)(value[j++] << 16) |
                            (uint)(value[j++] << 24);

                    sub = (ulong)word - borrow;
                    word = (uint)sub;
                    borrow = (uint)(sub >> 32) & 0x1u;
                    data[i] = ~word;
                }
                size = len & 0x3;

                if (size > 0)
                {
                    word = 0;
                    uint store_mask = 0;
                    for (int i = 0; i < size; ++i)
                    {
                        word |= (uint)(value[j++] << (i * 8));
                        store_mask = (store_mask << 8) | 0xFF;
                    }

                    sub = word - borrow;
                    word = (uint)sub;
                    borrow = (uint)(sub >> 32) & 0x1u;

                    if ((~word & store_mask) == 0)
                        data = Resize(data, data.Length - 1);
                    else
                        data[data.Length - 1] = ~word & store_mask;
                }
                if (borrow != 0) //FIXME I believe this can't happen, can someone write a test for it?
                    throw new Exception("non zero final carry");
            }
        }

        public bool IsEven
        {
            get { return sign == 0 || (data[0] & 0x1) == 0; }
        }

        public bool IsOne
        {
            get { return sign == 1 && data.Length == 1 && data[0] == 1; }
        }


        //		//Gem from Hacker's Delight
        //		//Returns the number of bits set in @x
        //		static int PopulationCount(uint x)
        //		{
        //			x = x - ((x >> 1) & 0x55555555);
        //			x = (x & 0x33333333) + ((x >> 2) & 0x33333333);
        //			x = (x + (x >> 4)) & 0x0F0F0F0F;
        //			x = x + (x >> 8);
        //			x = x + (x >> 16);
        //			return (int)(x & 0x0000003F);
        //		}

        //		//Based on code by Zilong Tan on Ulib released under MIT license
        //		//Returns the number of bits set in @x
        //		static int PopulationCount(ulong x)
        //		{
        //			x -= (x >> 1) & 0x5555555555555555UL;
        //			x = (x & 0x3333333333333333UL) + ((x >> 2) & 0x3333333333333333UL);
        //			x = (x + (x >> 4)) & 0x0f0f0f0f0f0f0f0fUL;
        //			return (int)((x * 0x0101010101010101UL) >> 56);
        //		}

        //		static int LeadingZeroCount(uint value)
        //		{
        //			value |= value >> 1;
        //			value |= value >> 2;
        //			value |= value >> 4;
        //			value |= value >> 8;
        //			value |= value >> 16;
        //			return 32 - PopulationCount(value); // 32 = bits in uint
        //		}

        //		static int LeadingZeroCount(ulong value)
        //		{
        //			value |= value >> 1;
        //			value |= value >> 2;
        //			value |= value >> 4;
        //			value |= value >> 8;
        //			value |= value >> 16;
        //			value |= value >> 32;
        //			return 64 - PopulationCount(value); // 64 = bits in ulong
        //		}

        //		static double BuildDouble(int sign, ulong mantissa, int exponent)
        //		{
        //			const int exponentBias = 1023;
        //			const int mantissaLength = 52;
        //			const int exponentLength = 11;
        //			const int maxExponent = 2046;
        //			const long mantissaMask = 0xfffffffffffffL;
        //			const long exponentMask = 0x7ffL;
        //			const ulong negativeMark = 0x8000000000000000uL;

        //			if (sign == 0 || mantissa == 0)
        //			{
        //				return 0.0;
        //			}
        //			else
        //			{
        //				exponent += exponentBias + mantissaLength;
        //				int offset = LeadingZeroCount(mantissa) - exponentLength;
        //				if (exponent - offset > maxExponent)
        //				{
        //					return sign > 0 ? double.PositiveInfinity : double.NegativeInfinity;
        //				}
        //				else
        //				{
        //					if (offset < 0)
        //					{
        //						mantissa >>= -offset;
        //						exponent += -offset;
        //					}
        //					else if (offset >= exponent)
        //					{
        //						mantissa <<= exponent - 1;
        //						exponent = 0;
        //					}
        //					else
        //					{
        //						mantissa <<= offset;
        //						exponent -= offset;
        //					}
        //					mantissa = mantissa & mantissaMask;
        //					if ((exponent & exponentMask) == exponent)
        //					{
        //						unchecked
        //						{
        //							ulong bits = mantissa | ((ulong)exponent << mantissaLength);
        //							if (sign < 0)
        //							{
        //								bits |= negativeMark;
        //							}
        //							return BitConverter.Int64BitsToDouble((long)bits);
        //						}
        //					}
        //					else
        //					{
        //						return sign > 0 ? double.PositiveInfinity : double.NegativeInfinity;
        //					}
        //				}
        //			}
        //		}

        //		public bool IsPowerOfTwo
        //		{
        //			get
        //			{
        //				bool foundBit = false;
        //				if (sign != 1)
        //					return false;
        //				//This function is pop count == 1 for positive numbers
        //				for (int i = 0; i < data.Length; ++i)
        //				{
        //					int p = PopulationCount(data[i]);
        //					if (p > 0)
        //					{
        //						if (p > 1 || foundBit)
        //							return false;
        //						foundBit = true;
        //					}
        //				}
        //				return foundBit;
        //			}
        //		}

        public bool IsZero
        {
            get { return sign == 0; }
        }

        //public int Sign
        //{
        //    get { return sign; }
        //}

        public static BigInteger MinusOne
        {
            get { return new BigInteger(-1, ONE); }
        }

        public static BigInteger One
        {
            get { return new BigInteger(1, ONE); }
        }

        public static BigInteger Zero
        {
            get { return new BigInteger(0); }
        }

        //		public static explicit operator int(BigInteger value)
        //		{
        //			if (value.data == null)
        //				return 0;
        //			if (value.data.Length > 1)
        //				throw new OverflowException();
        //			uint data = value.data[0];

        //			if (value.sign == 1)
        //			{
        //				if (data > (uint)int.MaxValue)
        //					throw new OverflowException();
        //				return (int)data;
        //			}
        //			else if (value.sign == -1)
        //			{
        //				if (data > 0x80000000u)
        //					throw new OverflowException();
        //				return -(int)data;
        //			}

        //			return 0;
        //		}

        //		[CLSCompliantAttribute(false)]
        //		public static explicit operator uint(BigInteger value)
        //		{
        //			if (value.data == null)
        //				return 0;
        //			if (value.data.Length > 1 || value.sign == -1)
        //				throw new OverflowException();
        //			return value.data[0];
        //		}

        //		public static explicit operator short(BigInteger value)
        //		{
        //			int val = (int)value;
        //			if (val < short.MinValue || val > short.MaxValue)
        //				throw new OverflowException();
        //			return (short)val;
        //		}

        //		[CLSCompliantAttribute(false)]
        //		public static explicit operator ushort(BigInteger value)
        //		{
        //			uint val = (uint)value;
        //			if (val > ushort.MaxValue)
        //				throw new OverflowException();
        //			return (ushort)val;
        //		}

        //		public static explicit operator byte(BigInteger value)
        //		{
        //			uint val = (uint)value;
        //			if (val > byte.MaxValue)
        //				throw new OverflowException();
        //			return (byte)val;
        //		}

        //		[CLSCompliantAttribute(false)]
        //		public static explicit operator sbyte(BigInteger value)
        //		{
        //			int val = (int)value;
        //			if (val < sbyte.MinValue || val > sbyte.MaxValue)
        //				throw new OverflowException();
        //			return (sbyte)val;
        //		}


        //		public static explicit operator long(BigInteger value)
        //		{
        //			if (value.data == null)
        //				return 0;

        //			if (value.data.Length > 2)
        //				throw new OverflowException();

        //			uint low = value.data[0];

        //			if (value.data.Length == 1)
        //			{
        //				if (value.sign == 1)
        //					return (long)low;
        //				long res = (long)low;
        //				return -res;
        //			}

        //			uint high = value.data[1];

        //			if (value.sign == 1)
        //			{
        //				if (high >= 0x80000000u)
        //					throw new OverflowException();
        //				return (((long)high) << 32) | low;
        //			}

        //			/*
        //			We cannot represent negative numbers smaller than long.MinValue.
        //			Those values are encoded into what look negative numbers, so negating
        //			them produces a positive value, that's why it's safe to check for that
        //			condition.

        //			long.MinValue works fine since it's bigint encoding looks like a negative
        //			number, but since long.MinValue == -long.MinValue, we're good.
        //			*/

        //			long result = -((((long)high) << 32) | (long)low);
        //			if (result > 0)
        //				throw new OverflowException();
        //			return result;
        //		}

        //		[CLSCompliantAttribute(false)]
        //		public static explicit operator ulong(BigInteger value)
        //		{
        //			if (value.data == null)
        //				return 0;
        //			if (value.data.Length > 2 || value.sign == -1)
        //				throw new OverflowException();

        //			uint low = value.data[0];
        //			if (value.data.Length == 1)
        //				return low;

        //			uint high = value.data[1];
        //			return (((ulong)high) << 32) | low;
        //		}

        //		public static explicit operator double(BigInteger value)
        //		{
        //			if (value.data == null)
        //				return 0.0;

        //			switch (value.data.Length)
        //			{
        //				case 1:
        //					return BuildDouble(value.sign, value.data[0], 0);
        //				case 2:
        //					return BuildDouble(value.sign, (ulong)value.data[1] << 32 | (ulong)value.data[0], 0);
        //				default:
        //					var index = value.data.Length - 1;
        //					var word = value.data[index];
        //					var mantissa = ((ulong)word << 32) | value.data[index - 1];
        //					int missing = LeadingZeroCount(word) - 11; // 11 = bits in exponent
        //					if (missing > 0)
        //					{
        //						// add the missing bits from the next word
        //						mantissa = (mantissa << missing) | (value.data[index - 2] >> (32 - missing));
        //					}
        //					else
        //					{
        //						mantissa >>= -missing;
        //					}
        //					return BuildDouble(value.sign, mantissa, ((value.data.Length - 2) * 32) - missing);
        //			}
        //		}

        //		public static explicit operator float(BigInteger value)
        //		{
        //			return (float)(double)value;
        //		}

        //		public static explicit operator decimal(BigInteger value)
        //		{
        //			if (value.data == null)
        //				return Decimal.Zero;

        //			uint[] data = value.data;
        //			if (data.Length > 3)
        //				throw new OverflowException();

        //			int lo = 0, mi = 0, hi = 0;
        //			if (data.Length > 2)
        //				hi = (Int32)data[2];
        //			if (data.Length > 1)
        //				mi = (Int32)data[1];
        //			if (data.Length > 0)
        //				lo = (Int32)data[0];

        //			return new Decimal(lo, mi, hi, value.sign < 0, 0);
        //		}

        //		public static implicit operator BigInteger(int value)
        //		{
        //			return new BigInteger(value);
        //		}

        //		[CLSCompliantAttribute(false)]
        //		public static implicit operator BigInteger(uint value)
        //		{
        //			return new BigInteger(value);
        //		}

        //		public static implicit operator BigInteger(short value)
        //		{
        //			return new BigInteger(value);
        //		}

        //		[CLSCompliantAttribute(false)]
        //		public static implicit operator BigInteger(ushort value)
        //		{
        //			return new BigInteger(value);
        //		}

        //		public static implicit operator BigInteger(byte value)
        //		{
        //			return new BigInteger(value);
        //		}

        //		[CLSCompliantAttribute(false)]
        //		public static implicit operator BigInteger(sbyte value)
        //		{
        //			return new BigInteger(value);
        //		}

        //		public static implicit operator BigInteger(long value)
        //		{
        //			return new BigInteger(value);
        //		}

        //		[CLSCompliantAttribute(false)]
        //		public static implicit operator BigInteger(ulong value)
        //		{
        //			return new BigInteger(value);
        //		}

        //		public static explicit operator BigInteger(double value)
        //		{
        //			return new BigInteger(value);
        //		}

        //		public static explicit operator BigInteger(float value)
        //		{
        //			return new BigInteger(value);
        //		}

        //		public static explicit operator BigInteger(decimal value)
        //		{
        //			return new BigInteger(value);
        //		}

        //		public static BigInteger operator +(BigInteger left, BigInteger right)
        //		{
        //			if (left.sign == 0)
        //				return right;
        //			if (right.sign == 0)
        //				return left;

        //			if (left.sign == right.sign)
        //				return new BigInteger(left.sign, CoreAdd(left.data, right.data));

        //			int r = CoreCompare(left.data, right.data);

        //			if (r == 0)
        //				return Zero;

        //			if (r > 0) //left > right
        //				return new BigInteger(left.sign, CoreSub(left.data, right.data));

        //			return new BigInteger(right.sign, CoreSub(right.data, left.data));
        //		}

        //		public static BigInteger operator -(BigInteger left, BigInteger right)
        //		{
        //			if (right.sign == 0)
        //				return left;
        //			if (left.sign == 0)
        //				return new BigInteger((short)-right.sign, right.data);

        //			if (left.sign == right.sign)
        //			{
        //				int r = CoreCompare(left.data, right.data);

        //				if (r == 0)
        //					return Zero;

        //				if (r > 0) //left > right
        //					return new BigInteger(left.sign, CoreSub(left.data, right.data));

        //				return new BigInteger((short)-right.sign, CoreSub(right.data, left.data));
        //			}

        //			return new BigInteger(left.sign, CoreAdd(left.data, right.data));
        //		}

        public static BigInteger operator *(BigInteger left, BigInteger right)
        {
            if (left.sign == 0 || right.sign == 0)
                return Zero;

            if (left.data[0] == 1 && left.data.Length == 1)
            {
                if (left.sign == 1)
                    return right;
                return new BigInteger((short)-right.sign, right.data);
            }

            if (right.data[0] == 1 && right.data.Length == 1)
            {
                if (right.sign == 1)
                    return left;
                return new BigInteger((short)-left.sign, left.data);
            }

            uint[] a = left.data;
            uint[] b = right.data;

            uint[] res = new uint[a.Length + b.Length];

            for (int i = 0; i < a.Length; ++i)
            {
                uint ai = a[i];
                int k = i;

                ulong carry = 0;
                for (int j = 0; j < b.Length; ++j)
                {
                    carry = carry + ((ulong)ai) * b[j] + res[k];
                    res[k++] = (uint)carry;
                    carry >>= 32;
                }

                while (carry != 0)
                {
                    carry += res[k];
                    res[k++] = (uint)carry;
                    carry >>= 32;
                }
            }

            int m;
            for (m = res.Length - 1; m >= 0 && res[m] == 0; --m) ;
            if (m < res.Length - 1)
                res = Resize(res, m + 1);

            return new BigInteger((short)(left.sign * right.sign), res);
        }

        //		public static BigInteger operator /(BigInteger dividend, BigInteger divisor)
        //		{
        //			if (divisor.sign == 0)
        //				throw new DivideByZeroException();

        //			if (dividend.sign == 0)
        //				return dividend;

        //			uint[] quotient;
        //			uint[] remainder_value;

        //			DivModUnsigned(dividend.data, divisor.data, out quotient, out remainder_value);

        //			int i;
        //			for (i = quotient.Length - 1; i >= 0 && quotient[i] == 0; --i) ;
        //			if (i == -1)
        //				return Zero;
        //			if (i < quotient.Length - 1)
        //				quotient = Resize(quotient, i + 1);

        //			return new BigInteger((short)(dividend.sign * divisor.sign), quotient);
        //		}

        public static BigInteger operator %(BigInteger dividend, BigInteger divisor)
        {
            if (divisor.sign == 0)
                throw new Exception("Divide by zero");

            if (dividend.sign == 0)
                return dividend;

            uint[] quotient;
            uint[] remainder_value;

            DivModUnsigned(dividend.data, divisor.data, out quotient, out remainder_value);

            int i;
            for (i = remainder_value.Length - 1; i >= 0 && remainder_value[i] == 0; --i) ;
            if (i == -1)
                return Zero;

            if (i < remainder_value.Length - 1)
                remainder_value = Resize(remainder_value, i + 1);
            return new BigInteger(dividend.sign, remainder_value);
        }

        //		public static BigInteger operator -(BigInteger value)
        //		{
        //			if (value.data == null)
        //				return value;
        //			return new BigInteger((short)-value.sign, value.data);
        //		}

        //		public static BigInteger operator +(BigInteger value)
        //		{
        //			return value;
        //		}

        //		public static BigInteger operator ++(BigInteger value)
        //		{
        //			if (value.data == null)
        //				return One;

        //			short sign = value.sign;
        //			uint[] data = value.data;
        //			if (data.Length == 1)
        //			{
        //				if (sign == -1 && data[0] == 1)
        //					return Zero;
        //				if (sign == 0)
        //					return new BigInteger(1, ONE);
        //			}

        //			if (sign == -1)
        //				data = CoreSub(data, 1);
        //			else
        //				data = CoreAdd(data, 1);

        //			return new BigInteger(sign, data);
        //		}

        public static BigInteger operator --(BigInteger value)
        {
            if (value.data == null)
                return MinusOne;

            short sign = value.sign;
            uint[] data = value.data;
            if (data.Length == 1)
            {
                if (sign == 1 && data[0] == 1)
                    return Zero;
                if (sign == 0)
                    return new BigInteger(-1, ONE);
            }

            if (sign == -1)
                data = CoreAdd(data, 1);
            else
                data = CoreSub(data, 1);

            return new BigInteger(sign, data);
        }

        //		public static BigInteger operator &(BigInteger left, BigInteger right)
        //		{
        //			if (left.sign == 0)
        //				return left;

        //			if (right.sign == 0)
        //				return right;

        //			uint[] a = left.data;
        //			uint[] b = right.data;
        //			int ls = left.sign;
        //			int rs = right.sign;

        //			bool neg_res = (ls == rs) && (ls == -1);

        //			uint[] result = new uint[Math.Max(a.Length, b.Length)];

        //			ulong ac = 1, bc = 1, borrow = 1;

        //			int i;
        //			for (i = 0; i < result.Length; ++i)
        //			{
        //				uint va = 0;
        //				if (i < a.Length)
        //					va = a[i];
        //				if (ls == -1)
        //				{
        //					ac = ~va + ac;
        //					va = (uint)ac;
        //					ac = (uint)(ac >> 32);
        //				}

        //				uint vb = 0;
        //				if (i < b.Length)
        //					vb = b[i];
        //				if (rs == -1)
        //				{
        //					bc = ~vb + bc;
        //					vb = (uint)bc;
        //					bc = (uint)(bc >> 32);
        //				}

        //				uint word = va & vb;

        //				if (neg_res)
        //				{
        //					borrow = word - borrow;
        //					word = ~(uint)borrow;
        //					borrow = (uint)(borrow >> 32) & 0x1u;
        //				}

        //				result[i] = word;
        //			}

        //			for (i = result.Length - 1; i >= 0 && result[i] == 0; --i) ;
        //			if (i == -1)
        //				return Zero;

        //			if (i < result.Length - 1)
        //				result = Resize(result, i + 1);

        //			return new BigInteger(neg_res ? (short)-1 : (short)1, result);
        //		}

        //		public static BigInteger operator |(BigInteger left, BigInteger right)
        //		{
        //			if (left.sign == 0)
        //				return right;

        //			if (right.sign == 0)
        //				return left;

        //			uint[] a = left.data;
        //			uint[] b = right.data;
        //			int ls = left.sign;
        //			int rs = right.sign;

        //			bool neg_res = (ls == -1) || (rs == -1);

        //			uint[] result = new uint[Math.Max(a.Length, b.Length)];

        //			ulong ac = 1, bc = 1, borrow = 1;

        //			int i;
        //			for (i = 0; i < result.Length; ++i)
        //			{
        //				uint va = 0;
        //				if (i < a.Length)
        //					va = a[i];
        //				if (ls == -1)
        //				{
        //					ac = ~va + ac;
        //					va = (uint)ac;
        //					ac = (uint)(ac >> 32);
        //				}

        //				uint vb = 0;
        //				if (i < b.Length)
        //					vb = b[i];
        //				if (rs == -1)
        //				{
        //					bc = ~vb + bc;
        //					vb = (uint)bc;
        //					bc = (uint)(bc >> 32);
        //				}

        //				uint word = va | vb;

        //				if (neg_res)
        //				{
        //					borrow = word - borrow;
        //					word = ~(uint)borrow;
        //					borrow = (uint)(borrow >> 32) & 0x1u;
        //				}

        //				result[i] = word;
        //			}

        //			for (i = result.Length - 1; i >= 0 && result[i] == 0; --i) ;
        //			if (i == -1)
        //				return Zero;

        //			if (i < result.Length - 1)
        //				result = Resize(result, i + 1);

        //			return new BigInteger(neg_res ? (short)-1 : (short)1, result);
        //		}

        //		public static BigInteger operator ^(BigInteger left, BigInteger right)
        //		{
        //			if (left.sign == 0)
        //				return right;

        //			if (right.sign == 0)
        //				return left;

        //			uint[] a = left.data;
        //			uint[] b = right.data;
        //			int ls = left.sign;
        //			int rs = right.sign;

        //			bool neg_res = (ls == -1) ^ (rs == -1);

        //			uint[] result = new uint[Math.Max(a.Length, b.Length)];

        //			ulong ac = 1, bc = 1, borrow = 1;

        //			int i;
        //			for (i = 0; i < result.Length; ++i)
        //			{
        //				uint va = 0;
        //				if (i < a.Length)
        //					va = a[i];
        //				if (ls == -1)
        //				{
        //					ac = ~va + ac;
        //					va = (uint)ac;
        //					ac = (uint)(ac >> 32);
        //				}

        //				uint vb = 0;
        //				if (i < b.Length)
        //					vb = b[i];
        //				if (rs == -1)
        //				{
        //					bc = ~vb + bc;
        //					vb = (uint)bc;
        //					bc = (uint)(bc >> 32);
        //				}

        //				uint word = va ^ vb;

        //				if (neg_res)
        //				{
        //					borrow = word - borrow;
        //					word = ~(uint)borrow;
        //					borrow = (uint)(borrow >> 32) & 0x1u;
        //				}

        //				result[i] = word;
        //			}

        //			for (i = result.Length - 1; i >= 0 && result[i] == 0; --i) ;
        //			if (i == -1)
        //				return Zero;

        //			if (i < result.Length - 1)
        //				result = Resize(result, i + 1);

        //			return new BigInteger(neg_res ? (short)-1 : (short)1, result);
        //		}

        //		public static BigInteger operator ~(BigInteger value)
        //		{
        //			if (value.data == null)
        //				return new BigInteger(-1, ONE);

        //			uint[] data = value.data;
        //			int sign = value.sign;

        //			bool neg_res = sign == 1;

        //			uint[] result = new uint[data.Length];

        //			ulong carry = 1, borrow = 1;

        //			int i;
        //			for (i = 0; i < result.Length; ++i)
        //			{
        //				uint word = data[i];
        //				if (sign == -1)
        //				{
        //					carry = ~word + carry;
        //					word = (uint)carry;
        //					carry = (uint)(carry >> 32);
        //				}

        //				word = ~word;

        //				if (neg_res)
        //				{
        //					borrow = word - borrow;
        //					word = ~(uint)borrow;
        //					borrow = (uint)(borrow >> 32) & 0x1u;
        //				}

        //				result[i] = word;
        //			}

        //			for (i = result.Length - 1; i >= 0 && result[i] == 0; --i) ;
        //			if (i == -1)
        //				return Zero;

        //			if (i < result.Length - 1)
        //				result = Resize(result, i + 1);

        //			return new BigInteger(neg_res ? (short)-1 : (short)1, result);
        //		}

        //returns the 0-based index of the most significant set bit
        //returns 0 if no bit is set, so extra care when using it
        static int BitScanBackward(uint word)
        {
            for (int i = 31; i >= 0; --i)
            {
                uint mask = 1u << i;
                if ((word & mask) == mask)
                    return i;
            }
            return 0;
        }

        public static BigInteger operator <<(BigInteger value, int shift)
        {
            if (shift == 0 || value.data == null)
                return value;
            if (shift < 0)
                return value >> -shift;

            uint[] data = value.data;
            int sign = value.sign;

            int topMostIdx = BitScanBackward(data[data.Length - 1]);
            int bits = shift - (31 - topMostIdx);
            int extra_words = (bits >> 5) + ((bits & 0x1F) != 0 ? 1 : 0);

            uint[] res = new uint[data.Length + extra_words];

            int idx_shift = shift >> 5;
            int bit_shift = shift & 0x1F;
            int carry_shift = 32 - bit_shift;

            if (carry_shift == 32)
            {
                for (int i = 0; i < data.Length; ++i)
                {
                    uint word = data[i];
                    res[i + idx_shift] |= word << bit_shift;
                }
            }
            else
            {
                for (int i = 0; i < data.Length; ++i)
                {
                    uint word = data[i];
                    res[i + idx_shift] |= word << bit_shift;
                    if (i + idx_shift + 1 < res.Length)
                        res[i + idx_shift + 1] = word >> carry_shift;
                }
            }

            return new BigInteger((short)sign, res);
        }

        public static BigInteger operator >>(BigInteger value, int shift)
        {
            if (shift == 0 || value.sign == 0)
                return value;
            if (shift < 0)
                return value << -shift;

            uint[] data = value.data;
            int sign = value.sign;

            int topMostIdx = BitScanBackward(data[data.Length - 1]);
            int idx_shift = shift >> 5;
            int bit_shift = shift & 0x1F;

            int extra_words = idx_shift;
            if (bit_shift > topMostIdx)
                ++extra_words;
            int size = data.Length - extra_words;

            if (size <= 0)
            {
                if (sign == 1)
                    return Zero;
                return new BigInteger(-1, ONE);
            }

            uint[] res = new uint[size];
            int carry_shift = 32 - bit_shift;

            if (carry_shift == 32)
            {
                for (int i = data.Length - 1; i >= idx_shift; --i)
                {
                    uint word = data[i];

                    if (i - idx_shift < res.Length)
                        res[i - idx_shift] |= word >> bit_shift;
                }
            }
            else
            {
                for (int i = data.Length - 1; i >= idx_shift; --i)
                {
                    uint word = data[i];

                    if (i - idx_shift < res.Length)
                        res[i - idx_shift] |= word >> bit_shift;
                    if (i - idx_shift - 1 >= 0)
                        res[i - idx_shift - 1] = word << carry_shift;
                }

            }

            //Round down instead of toward zero
            if (sign == -1)
            {
                for (int i = 0; i < idx_shift; i++)
                {
                    if (data[i] != 0u)
                    {
                        var tmp = new BigInteger((short)sign, res);
                        --tmp;
                        return tmp;
                    }
                }
                if (bit_shift > 0 && (data[idx_shift] << carry_shift) != 0u)
                {
                    var tmp = new BigInteger((short)sign, res);
                    --tmp;
                    return tmp;
                }
            }
            return new BigInteger((short)sign, res);
        }

        //		public static bool operator <(BigInteger left, BigInteger right)
        //		{
        //			return Compare(left, right) < 0;
        //		}

        //		public static bool operator <(BigInteger left, long right)
        //		{
        //			return left.CompareTo(right) < 0;
        //		}


        //		public static bool operator <(long left, BigInteger right)
        //		{
        //			return right.CompareTo(left) > 0;
        //		}


        //		[CLSCompliantAttribute(false)]
        //		public static bool operator <(BigInteger left, ulong right)
        //		{
        //			return left.CompareTo(right) < 0;
        //		}

        //		[CLSCompliantAttribute(false)]
        //		public static bool operator <(ulong left, BigInteger right)
        //		{
        //			return right.CompareTo(left) > 0;
        //		}

        //		public static bool operator <=(BigInteger left, BigInteger right)
        //		{
        //			return Compare(left, right) <= 0;
        //		}

        //		public static bool operator <=(BigInteger left, long right)
        //		{
        //			return left.CompareTo(right) <= 0;
        //		}

        //		public static bool operator <=(long left, BigInteger right)
        //		{
        //			return right.CompareTo(left) >= 0;
        //		}

        //		[CLSCompliantAttribute(false)]
        //		public static bool operator <=(BigInteger left, ulong right)
        //		{
        //			return left.CompareTo(right) <= 0;
        //		}

        //		[CLSCompliantAttribute(false)]
        //		public static bool operator <=(ulong left, BigInteger right)
        //		{
        //			return right.CompareTo(left) >= 0;
        //		}

        //		public static bool operator >(BigInteger left, BigInteger right)
        //		{
        //			return Compare(left, right) > 0;
        //		}

        //		public static bool operator >(BigInteger left, long right)
        //		{
        //			return left.CompareTo(right) > 0;
        //		}

        //		public static bool operator >(long left, BigInteger right)
        //		{
        //			return right.CompareTo(left) < 0;
        //		}

        //		[CLSCompliantAttribute(false)]
        //		public static bool operator >(BigInteger left, ulong right)
        //		{
        //			return left.CompareTo(right) > 0;
        //		}

        //		[CLSCompliantAttribute(false)]
        //		public static bool operator >(ulong left, BigInteger right)
        //		{
        //			return right.CompareTo(left) < 0;
        //		}

        //		public static bool operator >=(BigInteger left, BigInteger right)
        //		{
        //			return Compare(left, right) >= 0;
        //		}

        //		public static bool operator >=(BigInteger left, long right)
        //		{
        //			return left.CompareTo(right) >= 0;
        //		}

        //		public static bool operator >=(long left, BigInteger right)
        //		{
        //			return right.CompareTo(left) <= 0;
        //		}

        //		[CLSCompliantAttribute(false)]
        //		public static bool operator >=(BigInteger left, ulong right)
        //		{
        //			return left.CompareTo(right) >= 0;
        //		}

        //		[CLSCompliantAttribute(false)]
        //		public static bool operator >=(ulong left, BigInteger right)
        //		{
        //			return right.CompareTo(left) <= 0;
        //		}

        //		public static bool operator ==(BigInteger left, BigInteger right)
        //		{
        //			return Compare(left, right) == 0;
        //		}

        //		public static bool operator ==(BigInteger left, long right)
        //		{
        //			return left.CompareTo(right) == 0;
        //		}

        //		public static bool operator ==(long left, BigInteger right)
        //		{
        //			return right.CompareTo(left) == 0;
        //		}

        //		[CLSCompliantAttribute(false)]
        //		public static bool operator ==(BigInteger left, ulong right)
        //		{
        //			return left.CompareTo(right) == 0;
        //		}

        //		[CLSCompliantAttribute(false)]
        //		public static bool operator ==(ulong left, BigInteger right)
        //		{
        //			return right.CompareTo(left) == 0;
        //		}

        //		public static bool operator !=(BigInteger left, BigInteger right)
        //		{
        //			return Compare(left, right) != 0;
        //		}

        //		public static bool operator !=(BigInteger left, long right)
        //		{
        //			return left.CompareTo(right) != 0;
        //		}

        //		public static bool operator !=(long left, BigInteger right)
        //		{
        //			return right.CompareTo(left) != 0;
        //		}

        //		[CLSCompliantAttribute(false)]
        //		public static bool operator !=(BigInteger left, ulong right)
        //		{
        //			return left.CompareTo(right) != 0;
        //		}

        //		[CLSCompliantAttribute(false)]
        //		public static bool operator !=(ulong left, BigInteger right)
        //		{
        //			return right.CompareTo(left) != 0;
        //		}

        //		public override bool Equals(object obj)
        //		{
        //			if (!(obj is BigInteger))
        //				return false;
        //			return Equals((BigInteger)obj);
        //		}

        //		public bool Equals(BigInteger other)
        //		{
        //			if (sign != other.sign)
        //				return false;

        //			int alen = data != null ? data.Length : 0;
        //			int blen = other.data != null ? other.data.Length : 0;

        //			if (alen != blen)
        //				return false;
        //			for (int i = 0; i < alen; ++i)
        //			{
        //				if (data[i] != other.data[i])
        //					return false;
        //			}
        //			return true;
        //		}

        //		public bool Equals(long other)
        //		{
        //			return CompareTo(other) == 0;
        //		}

        //		public override string ToString()
        //		{
        //			return ToString(10, null);
        //		}

        //		string ToStringWithPadding(string format, uint radix, IFormatProvider provider)
        //		{
        //			if (format.Length > 1)
        //			{
        //				int precision = Convert.ToInt32(format.Substring(1), CultureInfo.InvariantCulture.NumberFormat);
        //				string baseStr = ToString(radix, provider);
        //				if (baseStr.Length < precision)
        //				{
        //					string additional = new String('0', precision - baseStr.Length);
        //					if (baseStr[0] != '-')
        //					{
        //						return additional + baseStr;
        //					}
        //					else
        //					{
        //						return "-" + additional + baseStr.Substring(1);
        //					}
        //				}
        //				return baseStr;
        //			}
        //			return ToString(radix, provider);
        //		}

        //		public string ToString(string format)
        //		{
        //			return ToString(format, null);
        //		}

        //		public string ToString(IFormatProvider provider)
        //		{
        //			return ToString(null, provider);
        //		}


        //		public string ToString(string format, IFormatProvider provider)
        //		{
        //			if (format == null || format == "")
        //				return ToString(10, provider);

        //			switch (format[0])
        //			{
        //				case 'd':
        //				case 'D':
        //				case 'g':
        //				case 'G':
        //				case 'r':
        //				case 'R':
        //					return ToStringWithPadding(format, 10, provider);
        //				case 'x':
        //				case 'X':
        //					return ToStringWithPadding(format, 16, null);
        //				default:
        //					throw new FormatException(string.Format("format '{0}' not implemented", format));
        //			}
        //		}

        //		static uint[] MakeTwoComplement(uint[] v)
        //		{
        //			uint[] res = new uint[v.Length];

        //			ulong carry = 1;
        //			for (int i = 0; i < v.Length; ++i)
        //			{
        //				uint word = v[i];
        //				carry = (ulong)~word + carry;
        //				word = (uint)carry;
        //				carry = (uint)(carry >> 32);
        //				res[i] = word;
        //			}

        //			uint last = res[res.Length - 1];
        //			int idx = FirstNonFFByte(last);
        //			uint mask = 0xFF;
        //			for (int i = 1; i < idx; ++i)
        //				mask = (mask << 8) | 0xFF;

        //			res[res.Length - 1] = last & mask;
        //			return res;
        //		}

        //		string ToString(uint radix, IFormatProvider provider)
        //		{
        //			const string characterSet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        //			if (characterSet.Length < radix)
        //				throw new ArgumentException("charSet length less than radix", "characterSet");
        //			if (radix == 1)
        //				throw new ArgumentException("There is no such thing as radix one notation", "radix");

        //			if (sign == 0)
        //				return "0";
        //			if (data.Length == 1 && data[0] == 1)
        //				return sign == 1 ? "1" : "-1";

        //			List<char> digits = new List<char>(1 + data.Length * 3 / 10);

        //			BigInteger a;
        //			if (sign == 1)
        //				a = this;
        //			else
        //			{
        //				uint[] dt = data;
        //				if (radix > 10)
        //					dt = MakeTwoComplement(dt);
        //				a = new BigInteger(1, dt);
        //			}

        //			while (a != 0)
        //			{
        //				BigInteger rem;
        //				a = DivRem(a, radix, out rem);
        //				digits.Add(characterSet[(int)rem]);
        //			}

        //			if (sign == -1 && radix == 10)
        //			{
        //				NumberFormatInfo info = null;
        //				if (provider != null)
        //					info = provider.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo;
        //				if (info != null)
        //				{
        //					string str = info.NegativeSign;
        //					for (int i = str.Length - 1; i >= 0; --i)
        //						digits.Add(str[i]);
        //				}
        //				else
        //				{
        //					digits.Add('-');
        //				}
        //			}

        //			char last = digits[digits.Count - 1];
        //			if (sign == 1 && radix > 10 && (last < '0' || last > '9'))
        //				digits.Add('0');

        //			digits.Reverse();

        //			return new String(digits.ToArray());
        //		}

        //		public static BigInteger Parse(string value)
        //		{
        //			Exception ex;
        //			BigInteger result;

        //			if (!Parse(value, false, out result, out ex))
        //				throw ex;
        //			return result;
        //		}

        //		public static bool TryParse(string value, out BigInteger result)
        //		{
        //			Exception ex;
        //			return Parse(value, true, out result, out ex);
        //		}

        //		public static BigInteger Parse(string value, NumberStyles style)
        //		{
        //			return Parse(value, style, null);
        //		}

        //		public static BigInteger Parse(string value, IFormatProvider provider)
        //		{
        //			return Parse(value, NumberStyles.Integer, provider);
        //		}

        //		public static BigInteger Parse(
        //			string value, NumberStyles style, IFormatProvider provider)
        //		{
        //			Exception exc;
        //			BigInteger res;

        //			if (!Parse(value, style, provider, false, out res, out exc))
        //				throw exc;

        //			return res;
        //		}

        //		public static bool TryParse(
        //			string value, NumberStyles style, IFormatProvider provider,
        //			out BigInteger result)
        //		{
        //			Exception exc;
        //			if (!Parse(value, style, provider, true, out result, out exc))
        //			{
        //				result = Zero;
        //				return false;
        //			}

        //			return true;
        //		}

        //		internal static bool Parse(string s, NumberStyles style, IFormatProvider fp, bool tryParse, out BigInteger result, out Exception exc)
        //		{
        //			result = Zero;
        //			exc = null;

        //			if (s == null)
        //			{
        //				if (!tryParse)
        //					exc = new ArgumentNullException("s");
        //				return false;
        //			}

        //			if (s.Length == 0)
        //			{
        //				if (!tryParse)
        //					exc = GetFormatException();
        //				return false;
        //			}

        //			NumberFormatInfo nfi = null;
        //			if (fp != null)
        //			{
        //				Type typeNFI = typeof(System.Globalization.NumberFormatInfo);
        //				nfi = (NumberFormatInfo)fp.GetFormat(typeNFI);
        //			}
        //			if (nfi == null)
        //				nfi = Thread.CurrentThread.CurrentCulture.NumberFormat;

        //			if (!CheckStyle(style, tryParse, ref exc))
        //				return false;

        //			bool AllowCurrencySymbol = (style & NumberStyles.AllowCurrencySymbol) != 0;
        //			bool AllowHexSpecifier = (style & NumberStyles.AllowHexSpecifier) != 0;
        //			bool AllowThousands = (style & NumberStyles.AllowThousands) != 0;
        //			bool AllowDecimalPoint = (style & NumberStyles.AllowDecimalPoint) != 0;
        //			bool AllowParentheses = (style & NumberStyles.AllowParentheses) != 0;
        //			bool AllowTrailingSign = (style & NumberStyles.AllowTrailingSign) != 0;
        //			bool AllowLeadingSign = (style & NumberStyles.AllowLeadingSign) != 0;
        //			bool AllowTrailingWhite = (style & NumberStyles.AllowTrailingWhite) != 0;
        //			bool AllowLeadingWhite = (style & NumberStyles.AllowLeadingWhite) != 0;
        //			bool AllowExponent = (style & NumberStyles.AllowExponent) != 0;

        //			int pos = 0;

        //			if (AllowLeadingWhite && !JumpOverWhite(ref pos, s, true, tryParse, ref exc))
        //				return false;

        //			bool foundOpenParentheses = false;
        //			bool negative = false;
        //			bool foundSign = false;
        //			bool foundCurrency = false;

        //			// Pre-number stuff
        //			if (AllowParentheses && s[pos] == '(')
        //			{
        //				foundOpenParentheses = true;
        //				foundSign = true;
        //				negative = true; // MS always make the number negative when there parentheses
        //								 // even when NumberFormatInfo.NumberNegativePattern != 0!!!
        //				pos++;
        //				if (AllowLeadingWhite && !JumpOverWhite(ref pos, s, true, tryParse, ref exc))
        //					return false;

        //				if (s.Substring(pos, nfi.NegativeSign.Length) == nfi.NegativeSign)
        //				{
        //					if (!tryParse)
        //						exc = GetFormatException();
        //					return false;
        //				}

        //				if (s.Substring(pos, nfi.PositiveSign.Length) == nfi.PositiveSign)
        //				{
        //					if (!tryParse)
        //						exc = GetFormatException();
        //					return false;
        //				}
        //			}

        //			if (AllowLeadingSign && !foundSign)
        //			{
        //				// Sign + Currency
        //				FindSign(ref pos, s, nfi, ref foundSign, ref negative);
        //				if (foundSign)
        //				{
        //					if (AllowLeadingWhite && !JumpOverWhite(ref pos, s, true, tryParse, ref exc))
        //						return false;
        //					if (AllowCurrencySymbol)
        //					{
        //						FindCurrency(ref pos, s, nfi,
        //								  ref foundCurrency);
        //						if (foundCurrency && AllowLeadingWhite &&
        //							!JumpOverWhite(ref pos, s, true, tryParse, ref exc))
        //							return false;
        //					}
        //				}
        //			}

        //			if (AllowCurrencySymbol && !foundCurrency)
        //			{
        //				// Currency + sign
        //				FindCurrency(ref pos, s, nfi, ref foundCurrency);
        //				if (foundCurrency)
        //				{
        //					if (AllowLeadingWhite && !JumpOverWhite(ref pos, s, true, tryParse, ref exc))
        //						return false;
        //					if (foundCurrency)
        //					{
        //						if (!foundSign && AllowLeadingSign)
        //						{
        //							FindSign(ref pos, s, nfi, ref foundSign,
        //								  ref negative);
        //							if (foundSign && AllowLeadingWhite &&
        //								!JumpOverWhite(ref pos, s, true, tryParse, ref exc))
        //								return false;
        //						}
        //					}
        //				}
        //			}

        //			BigInteger number = Zero;
        //			int nDigits = 0;
        //			int decimalPointPos = -1;
        //			byte digitValue;
        //			char hexDigit;
        //			bool firstHexDigit = true;

        //			// Number stuff
        //			while (pos < s.Length)
        //			{

        //				if (!ValidDigit(s[pos], AllowHexSpecifier))
        //				{
        //					if (AllowThousands &&
        //						(FindOther(ref pos, s, nfi.NumberGroupSeparator)
        //						|| FindOther(ref pos, s, nfi.CurrencyGroupSeparator)))
        //						continue;

        //					if (AllowDecimalPoint && decimalPointPos < 0 &&
        //						(FindOther(ref pos, s, nfi.NumberDecimalSeparator)
        //						|| FindOther(ref pos, s, nfi.CurrencyDecimalSeparator)))
        //					{
        //						decimalPointPos = nDigits;
        //						continue;
        //					}

        //					break;
        //				}

        //				nDigits++;

        //				if (AllowHexSpecifier)
        //				{
        //					hexDigit = s[pos++];
        //					if (Char.IsDigit(hexDigit))
        //						digitValue = (byte)(hexDigit - '0');
        //					else if (Char.IsLower(hexDigit))
        //						digitValue = (byte)(hexDigit - 'a' + 10);
        //					else
        //						digitValue = (byte)(hexDigit - 'A' + 10);

        //					if (firstHexDigit && (byte)digitValue >= 8)
        //						negative = true;

        //					number = number * 16 + digitValue;
        //					firstHexDigit = false;
        //					continue;
        //				}

        //				number = number * 10 + (byte)(s[pos++] - '0');
        //			}

        //			// Post number stuff
        //			if (nDigits == 0)
        //			{
        //				if (!tryParse)
        //					exc = GetFormatException();
        //				return false;
        //			}

        //			//Signed hex value (Two's Complement)
        //			if (AllowHexSpecifier && negative)
        //			{
        //				BigInteger mask = BigInteger.Pow(16, nDigits) - 1;
        //				number = (number ^ mask) + 1;
        //			}

        //			int exponent = 0;
        //			if (AllowExponent)
        //				if (FindExponent(ref pos, s, ref exponent, tryParse, ref exc) && exc != null)
        //					return false;

        //			if (AllowTrailingSign && !foundSign)
        //			{
        //				// Sign + Currency
        //				FindSign(ref pos, s, nfi, ref foundSign, ref negative);
        //				if (foundSign && pos < s.Length)
        //				{
        //					if (AllowTrailingWhite && !JumpOverWhite(ref pos, s, true, tryParse, ref exc))
        //						return false;
        //				}
        //			}

        //			if (AllowCurrencySymbol && !foundCurrency)
        //			{
        //				if (AllowTrailingWhite && pos < s.Length && !JumpOverWhite(ref pos, s, false, tryParse, ref exc))
        //					return false;

        //				// Currency + sign
        //				FindCurrency(ref pos, s, nfi, ref foundCurrency);
        //				if (foundCurrency && pos < s.Length)
        //				{
        //					if (AllowTrailingWhite && !JumpOverWhite(ref pos, s, true, tryParse, ref exc))
        //						return false;
        //					if (!foundSign && AllowTrailingSign)
        //						FindSign(ref pos, s, nfi, ref foundSign,
        //							  ref negative);
        //				}
        //			}

        //			if (AllowTrailingWhite && pos < s.Length && !JumpOverWhite(ref pos, s, false, tryParse, ref exc))
        //				return false;

        //			if (foundOpenParentheses)
        //			{
        //				if (pos >= s.Length || s[pos++] != ')')
        //				{
        //					if (!tryParse)
        //						exc = GetFormatException();
        //					return false;
        //				}
        //				if (AllowTrailingWhite && pos < s.Length && !JumpOverWhite(ref pos, s, false, tryParse, ref exc))
        //					return false;
        //			}

        //			if (pos < s.Length && s[pos] != '\u0000')
        //			{
        //				if (!tryParse)
        //					exc = GetFormatException();
        //				return false;
        //			}

        //			if (decimalPointPos >= 0)
        //				exponent = exponent - nDigits + decimalPointPos;

        //			if (exponent < 0)
        //			{
        //				//
        //				// Any non-zero values after decimal point are not allowed
        //				//
        //				BigInteger remainder;
        //				number = BigInteger.DivRem(number, BigInteger.Pow(10, -exponent), out remainder);

        //				if (!remainder.IsZero)
        //				{
        //					if (!tryParse)
        //						exc = new OverflowException("Value too large or too small. exp=" + exponent + " rem = " + remainder + " pow = " + BigInteger.Pow(10, -exponent));
        //					return false;
        //				}
        //			}
        //			else if (exponent > 0)
        //			{
        //				number = BigInteger.Pow(10, exponent) * number;
        //			}

        //			if (number.sign == 0)
        //				result = number;
        //			else if (negative)
        //				result = new BigInteger(-1, number.data);
        //			else
        //				result = new BigInteger(1, number.data);

        //			return true;
        //		}

        //		internal static bool CheckStyle(NumberStyles style, bool tryParse, ref Exception exc)
        //		{
        //			if ((style & NumberStyles.AllowHexSpecifier) != 0)
        //			{
        //				NumberStyles ne = style ^ NumberStyles.AllowHexSpecifier;
        //				if ((ne & NumberStyles.AllowLeadingWhite) != 0)
        //					ne ^= NumberStyles.AllowLeadingWhite;
        //				if ((ne & NumberStyles.AllowTrailingWhite) != 0)
        //					ne ^= NumberStyles.AllowTrailingWhite;
        //				if (ne != 0)
        //				{
        //					if (!tryParse)
        //						exc = new ArgumentException(
        //							"With AllowHexSpecifier only " +
        //							"AllowLeadingWhite and AllowTrailingWhite " +
        //							"are permitted.");
        //					return false;
        //				}
        //			}
        //			else if ((uint)style > (uint)NumberStyles.Any)
        //			{
        //				if (!tryParse)
        //					exc = new ArgumentException("Not a valid number style");
        //				return false;
        //			}

        //			return true;
        //		}

        //		internal static bool JumpOverWhite(ref int pos, string s, bool reportError, bool tryParse, ref Exception exc)
        //		{
        //			while (pos < s.Length && Char.IsWhiteSpace(s[pos]))
        //				pos++;

        //			if (reportError && pos >= s.Length)
        //			{
        //				if (!tryParse)
        //					exc = GetFormatException();
        //				return false;
        //			}

        //			return true;
        //		}

        //		internal static void FindSign(ref int pos, string s, NumberFormatInfo nfi,
        //					  ref bool foundSign, ref bool negative)
        //		{
        //			if ((pos + nfi.NegativeSign.Length) <= s.Length &&
        //				string.CompareOrdinal(s, pos, nfi.NegativeSign, 0, nfi.NegativeSign.Length) == 0)
        //			{
        //				negative = true;
        //				foundSign = true;
        //				pos += nfi.NegativeSign.Length;
        //			}
        //			else if ((pos + nfi.PositiveSign.Length) <= s.Length &&
        //			  string.CompareOrdinal(s, pos, nfi.PositiveSign, 0, nfi.PositiveSign.Length) == 0)
        //			{
        //				negative = false;
        //				pos += nfi.PositiveSign.Length;
        //				foundSign = true;
        //			}
        //		}

        //		internal static void FindCurrency(ref int pos,
        //						 string s,
        //						 NumberFormatInfo nfi,
        //						 ref bool foundCurrency)
        //		{
        //			if ((pos + nfi.CurrencySymbol.Length) <= s.Length &&
        //				 s.Substring(pos, nfi.CurrencySymbol.Length) == nfi.CurrencySymbol)
        //			{
        //				foundCurrency = true;
        //				pos += nfi.CurrencySymbol.Length;
        //			}
        //		}

        //		internal static bool FindExponent(ref int pos, string s, ref int exponent, bool tryParse, ref Exception exc)
        //		{
        //			exponent = 0;

        //			if (pos >= s.Length || (s[pos] != 'e' && s[pos] != 'E'))
        //			{
        //				exc = null;
        //				return false;
        //			}

        //			var i = pos + 1;
        //			if (i == s.Length)
        //			{
        //				exc = tryParse ? null : GetFormatException();
        //				return true;
        //			}

        //			bool negative = false;
        //			if (s[i] == '-')
        //			{
        //				negative = true;
        //				if (++i == s.Length)
        //				{
        //					exc = tryParse ? null : GetFormatException();
        //					return true;
        //				}
        //			}

        //			if (s[i] == '+' && ++i == s.Length)
        //			{
        //				exc = tryParse ? null : GetFormatException();
        //				return true;
        //			}

        //			long exp = 0; // temp long value
        //			for (; i < s.Length; i++)
        //			{
        //				if (!Char.IsDigit(s[i]))
        //				{
        //					exc = tryParse ? null : GetFormatException();
        //					return true;
        //				}

        //				// Reduce the risk of throwing an overflow exc
        //				exp = checked(exp * 10 - (int)(s[i] - '0'));
        //				if (exp < Int32.MinValue || exp > Int32.MaxValue)
        //				{
        //					exc = tryParse ? null : new OverflowException("Value too large or too small.");
        //					return true;
        //				}
        //			}

        //			// exp value saved as negative
        //			if (!negative)
        //				exp = -exp;

        //			exc = null;
        //			exponent = (int)exp;
        //			pos = i;
        //			return true;
        //		}

        //		internal static bool FindOther(ref int pos, string s, string other)
        //		{
        //			if ((pos + other.Length) <= s.Length &&
        //				 s.Substring(pos, other.Length) == other)
        //			{
        //				pos += other.Length;
        //				return true;
        //			}

        //			return false;
        //		}

        //		internal static bool ValidDigit(char e, bool allowHex)
        //		{
        //			if (allowHex)
        //				return Char.IsDigit(e) || (e >= 'A' && e <= 'F') || (e >= 'a' && e <= 'f');

        //			return Char.IsDigit(e);
        //		}

        //		static Exception GetFormatException()
        //		{
        //			return new FormatException("Input string was not in the correct format");
        //		}

        //		static bool ProcessTrailingWhitespace(bool tryParse, string s, int position, ref Exception exc)
        //		{
        //			int len = s.Length;

        //			for (int i = position; i < len; i++)
        //			{
        //				char c = s[i];

        //				if (c != 0 && !Char.IsWhiteSpace(c))
        //				{
        //					if (!tryParse)
        //						exc = GetFormatException();
        //					return false;
        //				}
        //			}
        //			return true;
        //		}

        //		static bool Parse(string s, bool tryParse, out BigInteger result, out Exception exc)
        //		{
        //			int len;
        //			int i, sign = 1;
        //			bool digits_seen = false;

        //			result = Zero;
        //			exc = null;

        //			if (s == null)
        //			{
        //				if (!tryParse)
        //					exc = new ArgumentNullException("value");
        //				return false;
        //			}

        //			len = s.Length;

        //			char c;
        //			for (i = 0; i < len; i++)
        //			{
        //				c = s[i];
        //				if (!Char.IsWhiteSpace(c))
        //					break;
        //			}

        //			if (i == len)
        //			{
        //				if (!tryParse)
        //					exc = GetFormatException();
        //				return false;
        //			}

        //			var info = Thread.CurrentThread.CurrentCulture.NumberFormat;

        //			string negative = info.NegativeSign;
        //			string positive = info.PositiveSign;

        //			if (string.CompareOrdinal(s, i, positive, 0, positive.Length) == 0)
        //				i += positive.Length;
        //			else if (string.CompareOrdinal(s, i, negative, 0, negative.Length) == 0)
        //			{
        //				sign = -1;
        //				i += negative.Length;
        //			}

        //			BigInteger val = Zero;
        //			for (; i < len; i++)
        //			{
        //				c = s[i];

        //				if (c == '\0')
        //				{
        //					i = len;
        //					continue;
        //				}

        //				if (c >= '0' && c <= '9')
        //				{
        //					byte d = (byte)(c - '0');

        //					val = val * 10 + d;

        //					digits_seen = true;
        //				}
        //				else if (!ProcessTrailingWhitespace(tryParse, s, i, ref exc))
        //					return false;
        //			}

        //			if (!digits_seen)
        //			{
        //				if (!tryParse)
        //					exc = GetFormatException();
        //				return false;
        //			}

        //			if (val.sign == 0)
        //				result = val;
        //			else if (sign == -1)
        //				result = new BigInteger(-1, val.data);
        //			else
        //				result = new BigInteger(1, val.data);

        //			return true;
        //		}

        //		public static BigInteger Min(BigInteger left, BigInteger right)
        //		{
        //			int ls = left.sign;
        //			int rs = right.sign;

        //			if (ls < rs)
        //				return left;
        //			if (rs < ls)
        //				return right;

        //			int r = CoreCompare(left.data, right.data);
        //			if (ls == -1)
        //				r = -r;

        //			if (r <= 0)
        //				return left;
        //			return right;
        //		}


        //		public static BigInteger Max(BigInteger left, BigInteger right)
        //		{
        //			int ls = left.sign;
        //			int rs = right.sign;

        //			if (ls > rs)
        //				return left;
        //			if (rs > ls)
        //				return right;

        //			int r = CoreCompare(left.data, right.data);
        //			if (ls == -1)
        //				r = -r;

        //			if (r >= 0)
        //				return left;
        //			return right;
        //		}

        public static BigInteger Abs(BigInteger value)
        {
            return new BigInteger((short)Math.Abs(value.sign), value.data);
        }


        //		public static BigInteger DivRem(BigInteger dividend, BigInteger divisor, out BigInteger remainder)
        //		{
        //			if (divisor.sign == 0)
        //				throw new DivideByZeroException();

        //			if (dividend.sign == 0)
        //			{
        //				remainder = dividend;
        //				return dividend;
        //			}

        //			uint[] quotient;
        //			uint[] remainder_value;

        //			DivModUnsigned(dividend.data, divisor.data, out quotient, out remainder_value);

        //			int i;
        //			for (i = remainder_value.Length - 1; i >= 0 && remainder_value[i] == 0; --i) ;
        //			if (i == -1)
        //			{
        //				remainder = Zero;
        //			}
        //			else
        //			{
        //				if (i < remainder_value.Length - 1)
        //					remainder_value = Resize(remainder_value, i + 1);
        //				remainder = new BigInteger(dividend.sign, remainder_value);
        //			}

        //			for (i = quotient.Length - 1; i >= 0 && quotient[i] == 0; --i) ;
        //			if (i == -1)
        //				return Zero;
        //			if (i < quotient.Length - 1)
        //				quotient = Resize(quotient, i + 1);

        //			return new BigInteger((short)(dividend.sign * divisor.sign), quotient);
        //		}

        //		public static BigInteger Pow(BigInteger value, int exponent)
        //		{
        //			if (exponent < 0)
        //				throw new ArgumentOutOfRangeException("exponent", "exp must be >= 0");
        //			if (exponent == 0)
        //				return One;
        //			if (exponent == 1)
        //				return value;

        //			BigInteger result = One;
        //			while (exponent != 0)
        //			{
        //				if ((exponent & 1) != 0)
        //					result = result * value;
        //				if (exponent == 1)
        //					break;

        //				value = value * value;
        //				exponent >>= 1;
        //			}
        //			return result;
        //		}

        public static BigInteger ModPow(BigInteger value, BigInteger exponent, BigInteger modulus)
        {
            if (exponent.sign == -1)
                throw new ArgumentOutOfRangeException("exponent", "power must be >= 0");
            if (modulus.sign == 0)
                throw new Exception("Divide by zero");

            BigInteger result = One % modulus;
            while (exponent.sign != 0)
            {
                if (!exponent.IsEven)
                {
                    result = result * value;
                    result = result % modulus;
                }
                if (exponent.IsOne)
                    break;
                value = value * value;
                value = value % modulus;
                exponent >>= 1;
            }
            return result;
        }

        //		public static BigInteger GreatestCommonDivisor(BigInteger left, BigInteger right)
        //		{
        //			if (left.sign != 0 && left.data.Length == 1 && left.data[0] == 1)
        //				return new BigInteger(1, ONE);
        //			if (right.sign != 0 && right.data.Length == 1 && right.data[0] == 1)
        //				return new BigInteger(1, ONE);
        //			if (left.IsZero)
        //				return Abs(right);
        //			if (right.IsZero)
        //				return Abs(left);

        //			BigInteger x = new BigInteger(1, left.data);
        //			BigInteger y = new BigInteger(1, right.data);

        //			BigInteger g = y;

        //			while (x.data.Length > 1)
        //			{
        //				g = x;
        //				x = y % x;
        //				y = g;

        //			}
        //			if (x.IsZero) return g;

        //			// TODO: should we have something here if we can convert to long?

        //			//
        //			// Now we can just do it with single precision. I am using the binary gcd method,
        //			// as it should be faster.
        //			//

        //			uint yy = x.data[0];
        //			uint xx = (uint)(y % yy);

        //			int t = 0;

        //			while (((xx | yy) & 1) == 0)
        //			{
        //				xx >>= 1; yy >>= 1; t++;
        //			}
        //			while (xx != 0)
        //			{
        //				while ((xx & 1) == 0) xx >>= 1;
        //				while ((yy & 1) == 0) yy >>= 1;
        //				if (xx >= yy)
        //					xx = (xx - yy) >> 1;
        //				else
        //					yy = (yy - xx) >> 1;
        //			}

        //			return yy << t;
        //		}

        //		/*LAMESPEC Log doesn't specify to how many ulp is has to be precise
        //		We are equilavent to MS with about 2 ULP
        //		*/
        //		public static double Log(BigInteger value, Double baseValue)
        //		{
        //			if (value.sign == -1 || baseValue == 1.0d || baseValue == -1.0d ||
        //					baseValue == Double.NegativeInfinity || double.IsNaN(baseValue))
        //				return double.NaN;

        //			if (baseValue == 0.0d || baseValue == Double.PositiveInfinity)
        //				return value.IsOne ? 0 : double.NaN;

        //			if (value.data == null)
        //				return double.NegativeInfinity;

        //			int length = value.data.Length - 1;
        //			int bitCount = -1;
        //			for (int curBit = 31; curBit >= 0; curBit--)
        //			{
        //				if ((value.data[length] & (1 << curBit)) != 0)
        //				{
        //					bitCount = curBit + length * 32;
        //					break;
        //				}
        //			}

        //			long bitlen = bitCount;
        //			Double c = 0, d = 1;

        //			BigInteger testBit = One;
        //			long tempBitlen = bitlen;
        //			while (tempBitlen > Int32.MaxValue)
        //			{
        //				testBit = testBit << Int32.MaxValue;
        //				tempBitlen -= Int32.MaxValue;
        //			}
        //			testBit = testBit << (int)tempBitlen;

        //			for (long curbit = bitlen; curbit >= 0; --curbit)
        //			{
        //				if ((value & testBit).sign != 0)
        //					c += d;
        //				d *= 0.5;
        //				testBit = testBit >> 1;
        //			}
        //			return (System.Math.Log(c) + System.Math.Log(2) * bitlen) / System.Math.Log(baseValue);
        //		}


        //		public static double Log(BigInteger value)
        //		{
        //			return Log(value, Math.E);
        //		}


        //		public static double Log10(BigInteger value)
        //		{
        //			return Log(value, 10);
        //		}

        //		[CLSCompliantAttribute(false)]
        //		public bool Equals(ulong other)
        //		{
        //			return CompareTo(other) == 0;
        //		}

        //		public override int GetHashCode()
        //		{
        //			uint hash = (uint)(sign * 0x01010101u);
        //			int len = data != null ? data.Length : 0;

        //			for (int i = 0; i < len; ++i)
        //				hash ^= data[i];
        //			return (int)hash;
        //		}

        //		public static BigInteger Add(BigInteger left, BigInteger right)
        //		{
        //			return left + right;
        //		}

        //		public static BigInteger Subtract(BigInteger left, BigInteger right)
        //		{
        //			return left - right;
        //		}

        //		public static BigInteger Multiply(BigInteger left, BigInteger right)
        //		{
        //			return left * right;
        //		}

        //		public static BigInteger Divide(BigInteger dividend, BigInteger divisor)
        //		{
        //			return dividend / divisor;
        //		}

        //		public static BigInteger Remainder(BigInteger dividend, BigInteger divisor)
        //		{
        //			return dividend % divisor;
        //		}

        //		public static BigInteger Negate(BigInteger value)
        //		{
        //			return -value;
        //		}

        //		public int CompareTo(object obj)
        //		{
        //			if (obj == null)
        //				return 1;

        //			if (!(obj is BigInteger))
        //				return -1;

        //			return Compare(this, (BigInteger)obj);
        //		}

        //		public int CompareTo(BigInteger other)
        //		{
        //			return Compare(this, other);
        //		}

        //		[CLSCompliantAttribute(false)]
        //		public int CompareTo(ulong other)
        //		{
        //			if (sign < 0)
        //				return -1;
        //			if (sign == 0)
        //				return other == 0 ? 0 : -1;

        //			if (data.Length > 2)
        //				return 1;

        //			uint high = (uint)(other >> 32);
        //			uint low = (uint)other;

        //			return LongCompare(low, high);
        //		}

        //		int LongCompare(uint low, uint high)
        //		{
        //			uint h = 0;
        //			if (data.Length > 1)
        //				h = data[1];

        //			if (h > high)
        //				return 1;
        //			if (h < high)
        //				return -1;

        //			uint l = data[0];

        //			if (l > low)
        //				return 1;
        //			if (l < low)
        //				return -1;

        //			return 0;
        //		}

        //		public int CompareTo(long other)
        //		{
        //			int ls = sign;
        //			int rs = Math.Sign(other);

        //			if (ls != rs)
        //				return ls > rs ? 1 : -1;

        //			if (ls == 0)
        //				return 0;

        //			if (data.Length > 2)
        //				return sign;

        //			if (other < 0)
        //				other = -other;
        //			uint low = (uint)other;
        //			uint high = (uint)((ulong)other >> 32);

        //			int r = LongCompare(low, high);
        //			if (ls == -1)
        //				r = -r;

        //			return r;
        //		}

        //		public static int Compare(BigInteger left, BigInteger right)
        //		{
        //			int ls = left.sign;
        //			int rs = right.sign;

        //			if (ls != rs)
        //				return ls > rs ? 1 : -1;

        //			int r = CoreCompare(left.data, right.data);
        //			if (ls < 0)
        //				r = -r;
        //			return r;
        //		}


        static int TopByte(uint x)
        {
            if ((x & 0xFFFF0000u) != 0)
            {
                if ((x & 0xFF000000u) != 0)
                    return 4;
                return 3;
            }
            if ((x & 0xFF00u) != 0)
                return 2;
            return 1;
        }

        static int FirstNonFFByte(uint word)
        {
            if ((word & 0xFF000000u) != 0xFF000000u)
                return 4;
            else if ((word & 0xFF0000u) != 0xFF0000u)
                return 3;
            else if ((word & 0xFF00u) != 0xFF00u)
                return 2;
            return 1;
        }

        public byte[] ToByteArray()
        {
            if (sign == 0)
                return new byte[1];

            //number of bytes not counting upper word
            int bytes = (data.Length - 1) * 4;
            bool needExtraZero = false;

            uint topWord = data[data.Length - 1];
            int extra;

            //if the topmost bit is set we need an extra 
            if (sign == 1)
            {
                extra = TopByte(topWord);
                uint mask = 0x80u << ((extra - 1) * 8);
                if ((topWord & mask) != 0)
                {
                    needExtraZero = true;
                }
            }
            else
            {
                extra = TopByte(topWord);
            }

            byte[] res = new byte[bytes + extra + (needExtraZero ? 1 : 0)];
            if (sign == 1)
            {
                int j = 0;
                int end = data.Length - 1;
                for (int i = 0; i < end; ++i)
                {
                    uint word = data[i];

                    res[j++] = (byte)word;
                    res[j++] = (byte)(word >> 8);
                    res[j++] = (byte)(word >> 16);
                    res[j++] = (byte)(word >> 24);
                }
                while (extra-- > 0)
                {
                    res[j++] = (byte)topWord;
                    topWord >>= 8;
                }
            }
            else
            {
                int j = 0;
                int end = data.Length - 1;

                uint carry = 1, word;
                ulong add;
                for (int i = 0; i < end; ++i)
                {
                    word = data[i];
                    add = (ulong)~word + carry;
                    word = (uint)add;
                    carry = (uint)(add >> 32);

                    res[j++] = (byte)word;
                    res[j++] = (byte)(word >> 8);
                    res[j++] = (byte)(word >> 16);
                    res[j++] = (byte)(word >> 24);
                }

                add = (ulong)~topWord + (carry);
                word = (uint)add;
                carry = (uint)(add >> 32);
                if (carry == 0)
                {
                    int ex = FirstNonFFByte(word);
                    bool needExtra = (word & (1 << (ex * 8 - 1))) == 0;
                    int to = ex + (needExtra ? 1 : 0);

                    if (to != extra)
                        res = Resize(res, bytes + to);

                    while (ex-- > 0)
                    {
                        res[j++] = (byte)word;
                        word >>= 8;
                    }
                    if (needExtra)
                        res[j++] = 0xFF;
                }
                else
                {
                    res = Resize(res, bytes + 5);
                    res[j++] = (byte)word;
                    res[j++] = (byte)(word >> 8);
                    res[j++] = (byte)(word >> 16);
                    res[j++] = (byte)(word >> 24);
                    res[j++] = 0xFF;
                }
            }

            return res;
        }

        static byte[] Resize(byte[] v, int len)
        {
            byte[] res = new byte[len];
            Array.Copy(v, res, Math.Min(v.Length, len));
            return res;
        }

        static uint[] Resize(uint[] v, int len)
        {
            uint[] res = new uint[len];
            Array.Copy(v, res, Math.Min(v.Length, len));
            return res;
        }

        //static uint[] CoreAdd(uint[] a, uint[] b)
        //{
        //    if (a.Length < b.Length)
        //    {
        //        uint[] tmp = a;
        //        a = b;
        //        b = tmp;
        //    }

        //    int bl = a.Length;
        //    int sl = b.Length;

        //    uint[] res = new uint[bl];

        //    ulong sum = 0;

        //    int i = 0;
        //    for (; i < sl; i++)
        //    {
        //        sum = sum + a[i] + b[i];
        //        res[i] = (uint)sum;
        //        sum >>= 32;
        //    }

        //    for (; i < bl; i++)
        //    {
        //        sum = sum + a[i];
        //        res[i] = (uint)sum;
        //        sum >>= 32;
        //    }

        //    if (sum != 0)
        //    {
        //        res = Resize(res, bl + 1);
        //        res[i] = (uint)sum;
        //    }

        //    return res;
        //}

        ///*invariant a > b*/
        //static uint[] CoreSub(uint[] a, uint[] b)
        //{
        //    int bl = a.Length;
        //    int sl = b.Length;

        //    uint[] res = new uint[bl];

        //    ulong borrow = 0;
        //    int i;
        //    for (i = 0; i < sl; ++i)
        //    {
        //        borrow = (ulong)a[i] - b[i] - borrow;

        //        res[i] = (uint)borrow;
        //        borrow = (borrow >> 32) & 0x1;
        //    }

        //    for (; i < bl; i++)
        //    {
        //        borrow = (ulong)a[i] - borrow;
        //        res[i] = (uint)borrow;
        //        borrow = (borrow >> 32) & 0x1;
        //    }

        //    //remove extra zeroes
        //    for (i = bl - 1; i >= 0 && res[i] == 0; --i) ;
        //    if (i < bl - 1)
        //        res = Resize(res, i + 1);

        //    return res;
        //}


        static uint[] CoreAdd(uint[] a, uint b)
        {
            int len = a.Length;
            uint[] res = new uint[len];

            ulong sum = b;
            int i;
            for (i = 0; i < len; i++)
            {
                sum = sum + a[i];
                res[i] = (uint)sum;
                sum >>= 32;
            }

            if (sum != 0)
            {
                res = Resize(res, len + 1);
                res[i] = (uint)sum;
            }

            return res;
        }

        static uint[] CoreSub(uint[] a, uint b)
        {
            int len = a.Length;
            uint[] res = new uint[len];

            ulong borrow = b;
            int i;
            for (i = 0; i < len; i++)
            {
                borrow = (ulong)a[i] - borrow;
                res[i] = (uint)borrow;
                borrow = (borrow >> 32) & 0x1;
            }

            //remove extra zeroes
            for (i = len - 1; i >= 0 && res[i] == 0; --i) ;
            if (i < len - 1)
                res = Resize(res, i + 1);

            return res;
        }

        //		static int CoreCompare(uint[] a, uint[] b)
        //		{
        //			int al = a != null ? a.Length : 0;
        //			int bl = b != null ? b.Length : 0;

        //			if (al > bl)
        //				return 1;
        //			if (bl > al)
        //				return -1;

        //			for (int i = al - 1; i >= 0; --i)
        //			{
        //				uint ai = a[i];
        //				uint bi = b[i];
        //				if (ai > bi)
        //					return 1;
        //				if (ai < bi)
        //					return -1;
        //			}
        //			return 0;
        //		}

        static int GetNormalizeShift(uint value)
        {
            int shift = 0;

            if ((value & 0xFFFF0000) == 0) { value <<= 16; shift += 16; }
            if ((value & 0xFF000000) == 0) { value <<= 8; shift += 8; }
            if ((value & 0xF0000000) == 0) { value <<= 4; shift += 4; }
            if ((value & 0xC0000000) == 0) { value <<= 2; shift += 2; }
            if ((value & 0x80000000) == 0) { value <<= 1; shift += 1; }

            return shift;
        }

        static void Normalize(uint[] u, int l, uint[] un, int shift)
        {
            uint carry = 0;
            int i;
            if (shift > 0)
            {
                int rshift = 32 - shift;
                for (i = 0; i < l; i++)
                {
                    uint ui = u[i];
                    un[i] = (ui << shift) | carry;
                    carry = ui >> rshift;
                }
            }
            else
            {
                for (i = 0; i < l; i++)
                {
                    un[i] = u[i];
                }
            }

            while (i < un.Length)
            {
                un[i++] = 0;
            }

            if (carry != 0)
            {
                un[l] = carry;
            }
        }

        static void Unnormalize(uint[] un, out uint[] r, int shift)
        {
            int length = un.Length;
            r = new uint[length];

            if (shift > 0)
            {
                int lshift = 32 - shift;
                uint carry = 0;
                for (int i = length - 1; i >= 0; i--)
                {
                    uint uni = un[i];
                    r[i] = (uni >> shift) | carry;
                    carry = (uni << lshift);
                }
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    r[i] = un[i];
                }
            }
        }

        const ulong Base = 0x100000000;
        static void DivModUnsigned(uint[] u, uint[] v, out uint[] q, out uint[] r)
        {
            int m = u.Length;
            int n = v.Length;

            if (n <= 1)
            {
                //  Divide by single digit
                //
                ulong rem = 0;
                uint v0 = v[0];
                q = new uint[m];
                r = new uint[1];

                for (int j = m - 1; j >= 0; j--)
                {
                    rem *= Base;
                    rem += u[j];

                    ulong div = rem / v0;
                    rem -= div * v0;
                    q[j] = (uint)div;
                }
                r[0] = (uint)rem;
            }
            else if (m >= n)
            {
                int shift = GetNormalizeShift(v[n - 1]);

                uint[] un = new uint[m + 1];
                uint[] vn = new uint[n];

                Normalize(u, m, un, shift);
                Normalize(v, n, vn, shift);

                q = new uint[m - n + 1];
                r = null;

                //  Main division loop
                //
                for (int j = m - n; j >= 0; j--)
                {
                    ulong rr, qq;
                    int i;

                    rr = Base * un[j + n] + un[j + n - 1];
                    qq = rr / vn[n - 1];
                    rr -= qq * vn[n - 1];

                    for (; ; )
                    {
                        // Estimate too big ?
                        //
                        if ((qq >= Base) || (qq * vn[n - 2] > (rr * Base + un[j + n - 2])))
                        {
                            qq--;
                            rr += (ulong)vn[n - 1];
                            if (rr < Base)
                                continue;
                        }
                        break;
                    }


                    //  Multiply and subtract
                    //
                    long b = 0;
                    long t = 0;
                    for (i = 0; i < n; i++)
                    {
                        ulong p = vn[i] * qq;
                        t = (long)un[i + j] - (long)(uint)p - b;
                        un[i + j] = (uint)t;
                        p >>= 32;
                        t >>= 32;
                        b = (long)p - t;
                    }
                    t = (long)un[j + n] - b;
                    un[j + n] = (uint)t;

                    //  Store the calculated value
                    //
                    q[j] = (uint)qq;

                    //  Add back vn[0..n] to un[j..j+n]
                    //
                    if (t < 0)
                    {
                        q[j]--;
                        ulong c = 0;
                        for (i = 0; i < n; i++)
                        {
                            c = (ulong)vn[i] + un[j + i] + c;
                            un[j + i] = (uint)c;
                            c >>= 32;
                        }
                        c += (ulong)un[j + n];
                        un[j + n] = (uint)c;
                    }
                }

                Unnormalize(un, out r, shift);
            }
            else
            {
                q = new uint[] { 0 };
                r = u;
            }
        }
    }
}