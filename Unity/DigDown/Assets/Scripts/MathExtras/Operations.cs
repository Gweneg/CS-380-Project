using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MathExtras
{
	public static class Operations
	{
		public static float Modulo(float value, float divisor) 
		{
			return (value % divisor + divisor) % divisor;
		}
		public static uint Modulo(uint value, uint divisor) 
		{
			return (value % divisor + divisor) % divisor;
		}
		public static int Modulo(int value, int divisor) 
		{
			return (value % divisor + divisor) % divisor;
		}
		public static long Modulo(long value, long divisor) 
		{
			return (value % divisor + divisor) % divisor;
		}
	}

}