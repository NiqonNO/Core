using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace NiqonNO.Core
{
	[Serializable]
	public struct Vector4Int : IEquatable<Vector4Int>, IEquatable<int>, IFormattable, IComparable,
		IComparable<Vector4Int>, IComparable<int>, ICloneable, IEnumerable, IEnumerable<int>,
		IEnumerable<(int, IEquatable<int>)>, IEnumerable<string>, IEnumerable<(string, IEquatable<int>)>, ISerializable
	{
		// Private properties
		[SerializeField] private int m_X;

		[SerializeField] private int m_Y;

		[SerializeField] private int m_Z;

		[SerializeField] private int m_W;

		private (int x, int y, int z, int w) m_Origin;

		// Private static properties
		private static readonly Vector4Int s_Zero = new Vector4Int(0, 0, 0, 0);

		private static readonly Vector4Int s_One = new Vector4Int(1, 1, 1, 1);

		private static readonly Vector4Int s_Up = new Vector4Int(0, 1, 0, 0);

		private static readonly Vector4Int s_Down = new Vector4Int(0, -1, 0, 0);

		private static readonly Vector4Int s_Left = new Vector4Int(-1, 0, 0, 0);

		private static readonly Vector4Int s_Right = new Vector4Int(1, 0, 0, 0);

		private static readonly Vector4Int s_Forward = new Vector4Int(0, 0, 1, 0);

		private static readonly Vector4Int s_Back = new Vector4Int(0, 0, -1, 0);

		private static readonly Vector4Int s_Outward = new Vector4Int(0, 0, 0, -1);

		private static readonly Vector4Int s_Inward = new Vector4Int(0, 0, 0, 1);

		// Properties
		/// <summary>
		///    X component of the vector.
		/// </summary>
		public int x
		{
			get { return m_X; }
			set { m_X = value; }
		}

		/// <summary>
		///    Y component of the vector.
		/// </summary>
		public int y
		{
			get { return m_Y; }
			set { m_Y = value; }
		}

		/// <summary>
		///    Z component of the vector.
		/// </summary>
		public int z
		{
			get { return m_Z; }
			set { m_Z = value; }
		}

		/// <summary>
		///    W component of the vector.
		/// </summary>
		public int w
		{
			get { return m_W; }
			set { m_W = value; }
		}

		/// <summary>
		///    Access the x, y, z or w components using [0], [1], [2] or [3] respectively.
		/// </summary>
		public int this[int index]
		{
			get
			{
				return index switch
				{
					0 => x,
					1 => y,
					2 => z,
					3 => w,
					_ => throw new IndexOutOfRangeException($"Invalid Vector4Int index addressed: {(object)index}!")
				};
			}
			set
			{
				switch (index)
				{
					case 0:
						x = value;
						break;
					case 1:
						y = value;
						break;
					case 2:
						z = value;
						break;
					case 3:
						w = value;
						break;
					default:
						throw new IndexOutOfRangeException($"Invalid Vector4Int index addressed: {(object)index}!");
				}
			}
		}

		/// <summary>
		///     Returns a copy of this vector's origin point (Read Only).
		/// </summary>
		public Vector4Int origin => new Vector4Int(m_Origin.x, m_Origin.y, m_Origin.z, m_Origin.w);

		/// <summary>
		///     Returns a copy of this vector with a magnitude of 1 (Read Only).
		/// </summary>
		public Vector4Int normalized => Normalize(this);

		/// <summary>
		///     Returns the distance of this vector from Vector4Int(0, 0, 0, 0) (Read Only).
		/// </summary>
		public double distFromZero => DistanceFromZero(this);

		/// <summary>
		///     Returns the magnitude of this vector (Read Only).
		/// </summary>
		public double magnitude => Magnitude(this);

		/// <summary>
		///     Returns the square magnitude of this vector (Read Only).
		/// </summary>
		public double sqrMagnitude => SqrMagnitude(this);

		/// <summary>
		///     Returns the cubic magnitude of this vector (Read Only).
		/// </summary>
		public double cbcMagnitude => CbcMagnitude(this);

		/// <summary>
		///     Returns the natural magnitude of this vector (Read Only).
		/// </summary>
		public double natMagnitude => NatMagnitude(this);

		/// <summary>
		///     Returns the exponentiated magnitude of this vector (Read Only).
		/// </summary>
		public double expMagnitude => ExpMagnitude(this);

		/// <summary>
		///     Returns the exponentiated sqaure magnitude of this vector (Read Only).
		/// </summary>
		public double expSqrMagnitude => ExpSqrMagnitude(this);

		/// <summary>
		///     Returns the exponentiated cubic magnitude of this vector (Read Only).
		/// </summary>
		public double expCbcMagnitude => ExpCbcMagnitude(this);

		/// <summary>
		///     Returns the exponentiated natural magnitude of this vector (Read Only).
		/// </summary>
		public double expNatMagnitude => ExpNatMagnitude(this);

		/// <summary>
		///     Returns a copy of this vector with every component clamped to [0,1] (Read Only).
		/// </summary>
		public Vector4Int zeroOneClamped => Clamp01(this);

		/// <summary>
		///     Returns a copy of this vector with every component clamped to [-1,1] (Read Only).
		/// </summary>
		public Vector4Int unitClamped => ClampUnit(this);

		/// <summary>
		///     Returns a copy of this vector with the sign values of every component (Read Only).
		/// </summary>
		public Vector4Int sign => Sign(this);

		/// <summary>
		///     Returns a copy of this vector with the sign values of every component and any zeros returning 1 (Read Only).
		/// </summary>
		public Vector4Int signPosZero => SignPositiveZero(this);

		/// <summary>
		///     Returns a copy of this vector with the sign values of every component and any zeros returning -1 (Read Only).
		/// </summary>
		public Vector4Int signNegZero => SignNegativeZero(this);

		// Static properties
		/// <summary>
		///     Shorthand for writing Vector4Int(0, 0, 0, 0).
		/// </summary>
		public static Vector4Int zero => s_Zero;

		/// <summary>
		///     Shorthand for writing Vector4Int(1, 1, 1, 1).
		/// </summary>
		public static Vector4Int one => s_One;

		/// <summary>
		///     Shorthand for writing Vector4Int(0, 1, 0, 0).
		/// </summary>
		public static Vector4Int up => s_Up;

		/// <summary>
		///     Shorthand for writing Vector4Int(0, -1, 0, 0).
		/// </summary>
		public static Vector4Int down => s_Down;

		/// <summary>
		///     Shorthand for writing Vector4Int(-1, 0, 0, 0).
		/// </summary>
		public static Vector4Int left => s_Left;

		/// <summary>
		///     Shorthand for writing Vector4Int(1, 0, 0, 0).
		/// </summary>
		public static Vector4Int right => s_Right;

		/// <summary>
		///     Shorthand for writing Vector4Int(0, 0, 1, 0).
		/// </summary>
		public static Vector4Int forward => s_Forward;

		/// <summary>
		///     Shorthand for writing Vector4Int(0, 0, -1, 0).
		/// </summary>
		public static Vector4Int back => s_Back;

		/// <summary>
		///     Shorthand for writing Vector4Int(0, 0, 0, -1).
		/// </summary>
		public static Vector4Int outward => s_Outward;

		/// <summary>
		///     Shorthand for writing Vector4Int(0, 0, 0, 1).
		/// </summary>
		public static Vector4Int inward => s_Inward;

		// Constructors
		/// <summary>
		///     Constructs a new Vector4Int.
		/// </summary>
		public Vector4Int(int x, int y, int z, int w)
		{
			m_X = x;
			m_Y = y;
			m_Z = z;
			m_W = w;
			m_Origin = (m_X, m_Y, m_Z, m_W);
		}

		/// <summary>
		///     Constructs a new Vector4Int and sets w to zero.
		/// </summary>
		public Vector4Int(int x, int y, int z)
		{
			m_X = x;
			m_Y = y;
			m_Z = z;
			m_W = 0;
			m_Origin = (m_X, m_Y, m_Z, m_W);
		}

		/// <summary>
		///     Constructs a new Vector4Int and sets z, w to zero.
		/// </summary>
		public Vector4Int(int x, int y)
		{
			m_X = x;
			m_Y = y;
			m_Z = 0;
			m_W = 0;
			m_Origin = (m_X, m_Y, m_Z, m_W);
		}

		/// <summary>
		///     Constructs a new Vector4Int and sets y, z and w to zero.
		/// </summary>
		public Vector4Int(int x)
		{
			m_X = x;
			m_Y = 0;
			m_Z = 0;
			m_W = 0;
			m_Origin = (m_X, m_Y, m_Z, m_W);
		}

		/// <summary>
		///     Constructs a new Vector4Int from a Vector3Int with the given w component.
		/// </summary>
		public Vector4Int(Vector3Int vec, int w)
		{
			m_X = vec.x;
			m_Y = vec.y;
			m_Z = vec.z;
			m_W = w;
			m_Origin = (m_X, m_Y, m_Z, m_W);
		}

		/// <summary>
		///     Constructs a new Vector4Int from a Vector3Int and sets w to zero.
		/// </summary>
		public Vector4Int(Vector3Int vec)
		{
			m_X = vec.x;
			m_Y = vec.y;
			m_Z = vec.z;
			m_W = 0;
			m_Origin = (m_X, m_Y, m_Z, m_W);
		}

		/// <summary>
		///     Constructs a new Vector4Int from two Vector2Ints.
		/// </summary>
		public Vector4Int(Vector2Int a, Vector2Int b)
		{
			m_X = a.x;
			m_Y = a.y;
			m_Z = b.x;
			m_W = b.y;
			m_Origin = (m_X, m_Y, m_Z, m_W);
		}

		/// <summary>
		///     Constructs a new Vector4Int from a Vector2Int with the given z, w components.
		/// </summary>
		public Vector4Int(Vector2Int vec, int z, int w)
		{
			m_X = vec.x;
			m_Y = vec.y;
			m_Z = z;
			m_W = w;
			m_Origin = (m_X, m_Y, m_Z, m_W);
		}

		/// <summary>
		///     Constructs a new Vector4Int from a Vector2Int with the given z component and sets w to zero.
		/// </summary>
		public Vector4Int(Vector2Int vec, int z)
		{
			m_X = vec.x;
			m_Y = vec.y;
			m_Z = z;
			m_W = 0;
			m_Origin = (m_X, m_Y, m_Z, m_W);
		}

		/// <summary>
		///     Constructs a new Vector4Int from a Vector2Int and sets z, w to zero.
		/// </summary>
		public Vector4Int(Vector2Int vec)
		{
			m_X = vec.x;
			m_Y = vec.y;
			m_Z = 0;
			m_W = 0;
			m_Origin = (m_X, m_Y, m_Z, m_W);
		}

		// Conversion operators
		public static explicit operator Vector4Int(Vector4 vec)
		{
			return new Vector4Int((int)vec.x, (int)vec.y, (int)vec.z, (int)vec.w);
		}

		public static implicit operator Vector4(Vector4Int vec)
		{
			return new Vector4(vec.x, vec.y, vec.z, vec.w);
		}

		public static explicit operator Vector4Int(Vector3Int vec)
		{
			return new Vector4Int(vec.x, vec.y, vec.z, 0);
		}

		public static explicit operator Vector3Int(Vector4Int vec)
		{
			return new Vector3Int(vec.x, vec.y, vec.z);
		}

		public static explicit operator Vector4Int(Vector2Int vec)
		{
			return new Vector4Int(vec.x, vec.y, 0, 0);
		}

		public static explicit operator Vector2Int(Vector4Int vec)
		{
			return new Vector2Int(vec.x, vec.y);
		}

		public static explicit operator Vector4Int(Color32 c)
		{
			return new Vector4Int(c.r, c.g, c.b, c.a);
		}

		public static explicit operator Color32(Vector4Int vec)
		{
			byte bx = (byte)Mathf.Clamp(vec.x, 0, 255);
			byte by = (byte)Mathf.Clamp(vec.y, 0, 255);
			byte bz = (byte)Mathf.Clamp(vec.z, 0, 255);
			byte bw = (byte)Mathf.Clamp(vec.w, 0, 255);
			return new Color32(bx, by, bz, bw);
		}

		public static explicit operator (int x, int y, int z, int w)(Vector4Int vec)
		{
			return (vec.x, vec.y, vec.z, vec.w);
		}

		public static explicit operator Vector4Int((int x, int y, int z, int w) t)
		{
			return new Vector4Int(t.x, t.y, t.z, t.w);
		}

		public static explicit operator int[](Vector4Int vec)
		{
			return new int[4] { vec.x, vec.y, vec.z, vec.w };
		}

		/// <summary>
		///     For the conversion to work, the length of the given integer array must be equal to 4.
		/// </summary>
		public static explicit operator Vector4Int(int[] arr)
		{
			if (arr.Length == 4)
				return new Vector4Int(arr[0], arr[1], arr[2], arr[3]);
			else
				throw new ArgumentException(
					"The length of the given integer array must be equal to 4 when converting to Vector4Int.",
					nameof(arr));
		}

		// Math operators
		public static Vector4Int operator +(Vector4Int a, Vector4Int b)
		{
			return new Vector4Int(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		}

		public static Vector4Int operator +(Vector4Int a, int b)
		{
			return new Vector4Int(a.x + b, a.y + b, a.z + b, a.w + b);
		}

		public static Vector4Int operator +(int a, Vector4Int b)
		{
			return new Vector4Int(a + b.x, a + b.y, a + b.z, a + b.w);
		}

		public static Vector4Int operator +(Vector4Int vec)
		{
			return new Vector4Int(+vec.x, +vec.y, +vec.z, +vec.w);
		}

		public static Vector4Int operator ++(Vector4Int vec)
		{
			vec.x++;
			vec.y++;
			vec.z++;
			vec.w++;
			return vec;
		}

		public static Vector4Int operator -(Vector4Int a, Vector4Int b)
		{
			return new Vector4Int(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		}

		public static Vector4Int operator -(Vector4Int a, int b)
		{
			return new Vector4Int(a.x - b, a.y - b, a.z - b, a.w - b);
		}

		public static Vector4Int operator -(int a, Vector4Int b)
		{
			return new Vector4Int(a - b.x, a - b.y, a - b.z, a - b.w);
		}

		public static Vector4Int operator -(Vector4Int vec)
		{
			return new Vector4Int(-vec.x, -vec.y, -vec.z, -vec.w);
		}

		public static Vector4Int operator --(Vector4Int vec)
		{
			vec.x--;
			vec.y--;
			vec.z--;
			vec.w--;
			return vec;
		}

		public static Vector4Int operator *(Vector4Int a, Vector4Int b)
		{
			return new Vector4Int(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
		}

		public static Vector4Int operator *(Vector4Int a, int b)
		{
			return new Vector4Int(a.x * b, a.y * b, a.z * b, a.w * b);
		}

		public static Vector4Int operator *(int a, Vector4Int b)
		{
			return new Vector4Int(a * b.x, a * b.y, a * b.z, a * b.w);
		}

		public static Vector4Int operator *(Vector4Int a, float b)
		{
			return new Vector4Int((int)(a.x * b), (int)(a.y * b), (int)(a.z * b), (int)(a.w * b));
		}

		public static Vector4Int operator *(float a, Vector4Int b)
		{
			return new Vector4Int((int)(a * b.x), (int)(a * b.y), (int)(a * b.z), (int)(a * b.w));
		}

		public static Vector4Int operator *(Vector4Int a, double b)
		{
			return new Vector4Int((int)(a.x * b), (int)(a.y * b), (int)(a.z * b), (int)(a.w * b));
		}

		public static Vector4Int operator *(double a, Vector4Int b)
		{
			return new Vector4Int((int)(a * b.x), (int)(a * b.y), (int)(a * b.z), (int)(a * b.w));
		}

		public static Vector4Int operator /(Vector4Int a, Vector4Int b)
		{
			return new Vector4Int(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);
		}

		public static Vector4Int operator /(Vector4Int a, int b)
		{
			return new Vector4Int(a.x / b, a.y / b, a.z / b, a.w / b);
		}

		public static Vector4Int operator /(Vector4Int a, float b)
		{
			return new Vector4Int((int)(a.x / b), (int)(a.y / b), (int)(a.z / b), (int)(a.w / b));
		}

		public static Vector4Int operator /(Vector4Int a, double b)
		{
			return new Vector4Int((int)(a.x / b), (int)(a.y / b), (int)(a.z / b), (int)(a.w / b));
		}

		public static Vector4Int operator %(Vector4Int a, Vector4Int b)
		{
			return new Vector4Int(a.x % b.x, a.y % b.y, a.z % b.z, a.w % b.w);
		}

		public static Vector4Int operator %(Vector4Int a, int b)
		{
			return new Vector4Int(a.x % b, a.y % b, a.z % b, a.w % b);
		}

		public static Vector4Int operator %(int a, Vector4Int b)
		{
			return new Vector4Int(a % b.x, a % b.y, a % b.z, a % b.w);
		}

		public static Vector4Int operator %(Vector4Int a, float b)
		{
			return new Vector4Int((int)(a.x % b), (int)(a.y % b), (int)(a.z % b), (int)(a.w % b));
		}

		public static Vector4Int operator %(float a, Vector4Int b)
		{
			return new Vector4Int((int)(a % b.x), (int)(a % b.y), (int)(a % b.z), (int)(a % b.w));
		}

		public static Vector4Int operator %(Vector4Int a, double b)
		{
			return new Vector4Int((int)(a.x % b), (int)(a.y % b), (int)(a.z % b), (int)(a.w % b));
		}

		public static Vector4Int operator %(double a, Vector4Int b)
		{
			return new Vector4Int((int)(a % b.x), (int)(a % b.y), (int)(a % b.z), (int)(a % b.w));
		}

		// Conditional operators
		public static bool operator ==(Vector4Int lhs, Vector4Int rhs)
		{
			return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z && lhs.w == rhs.w;
		}

		public static bool operator ==(Vector4Int lhs, Vector4 rhs)
		{
			return Mathf.Approximately(lhs.x, rhs.x) && Mathf.Approximately(lhs.y, rhs.y)
			                                         && Mathf.Approximately(lhs.z, rhs.z) &&
			                                         Mathf.Approximately(lhs.w, rhs.w);
		}

		public static bool operator ==(Vector4 lhs, Vector4Int rhs)
		{
			return Mathf.Approximately(lhs.x, rhs.x) && Mathf.Approximately(lhs.y, rhs.y)
			                                         && Mathf.Approximately(lhs.z, rhs.z) &&
			                                         Mathf.Approximately(lhs.w, rhs.w);
		}

		public static bool operator ==(Vector4Int lhs, int rhs)
		{
			return lhs.x == rhs && lhs.y == rhs && lhs.z == rhs && lhs.w == rhs;
		}

		/// <summary>
		///     Extended form of the !!Vector4Int operation which allows checking for any number instead of just 0.
		///     <br>However, for readability, if checking for a value of 0, using the !!Vector4Int operation is recommended instead.</br>
		///     <para>The parameter 'rhs' being nullable is to differentiate between this operator and the one with its non-nullable counterpart.
		///     <br>Despite this, the value of 'rhs' may not be null, as it will throw an exception if it is.</br></para>
		///     <para>Returns a vector containing 1s and 0s based on if a component of 'lhs' had the value of 'rhs' or not.</para>
		/// </summary>
		public static Vector4Int operator ==(Vector4Int lhs, [DisallowNull] int? rhs)
		{
			if (!rhs.HasValue)
				throw new ArgumentNullException(nameof(rhs), "The given value of 'rhs' may not be null.");

			return new Vector4Int(lhs.x == rhs.Value ? 1 : 0, lhs.y == rhs.Value ? 1 : 0, lhs.z == rhs.Value ? 1 : 0,
				lhs.w == rhs.Value ? 1 : 0);
		}

		public static bool operator ==(Vector4Int lhs, (int x, int y, int z, int w) rhs)
		{
			return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z && lhs.w == rhs.w;
		}

		/// <summary>
		///     Extended form of the !!Vector4Int operation which allows checking for any number instead of just 0 for each individual component.
		///     <br>However, for readability, if checking all for a value of 0, using the !!Vector4Int operation is recommended instead.</br>
		///     <para>The parameter 'rhs' being nullable is to differentiate between this operator and the one with its non-nullable counterpart.
		///     <br>Despite this, the value tuple of 'rhs' may not be null, as it will throw an exception if it is.</br></para>
		///     <para>Returns a vector containing 1s and 0s based on if a component of 'lhs' had the value of 'rhs' for said component or not.</para>
		/// </summary>
		public static Vector4Int operator ==(Vector4Int lhs, [DisallowNull] (int x, int y, int z, int w)? rhs)
		{
			if (!rhs.HasValue)
				throw new ArgumentNullException(nameof(rhs), "The given value tuple of 'rhs' may not be null.");

			return new Vector4Int(lhs.x == rhs.Value.x ? 1 : 0, lhs.y == rhs.Value.y ? 1 : 0,
				lhs.z == rhs.Value.z ? 1 : 0, lhs.w == rhs.Value.w ? 1 : 0);
		}

		public static bool operator !=(Vector4Int lhs, Vector4Int rhs)
		{
			return lhs.x != rhs.x && lhs.y != rhs.y && lhs.z != rhs.z && lhs.w != rhs.w;
		}

		public static bool operator !=(Vector4Int lhs, Vector4 rhs)
		{
			return !Mathf.Approximately(lhs.x, rhs.x) && !Mathf.Approximately(lhs.y, rhs.y)
			                                          && !Mathf.Approximately(lhs.z, rhs.z) &&
			                                          !Mathf.Approximately(lhs.w, rhs.w);
		}

		public static bool operator !=(Vector4 lhs, Vector4Int rhs)
		{
			return !Mathf.Approximately(lhs.x, rhs.x) && !Mathf.Approximately(lhs.y, rhs.y)
			                                          && !Mathf.Approximately(lhs.z, rhs.z) &&
			                                          !Mathf.Approximately(lhs.w, rhs.w);
		}

		public static bool operator !=(Vector4Int lhs, int rhs)
		{
			return lhs.x != rhs && lhs.y != rhs && lhs.z != rhs && lhs.w != rhs;
		}

		/// <summary>
		///     Extended form of the !Vector4Int operation which allows checking for any number instead of just 0.
		///     <br>However, for readability, if checking for a value of 0, using the !Vector4Int operation is recommended instead.</br>
		///     <para>The parameter 'rhs' being nullable is to differentiate between this operator and the one with its non-nullable counterpart.
		///     <br>Despite this, the value of 'rhs' may not be null, as it will throw an exception if it is.</br></para>
		///     <para>Returns a vector containing 0s and 1s based on if a component of 'lhs' had the value of 'rhs' or not.</para>
		/// </summary>
		public static Vector4Int operator !=(Vector4Int lhs, [DisallowNull] int? rhs)
		{
			if (!rhs.HasValue)
				throw new ArgumentNullException(nameof(rhs), "The given value of 'rhs' may not be null.");

			return new Vector4Int(lhs.x != rhs.Value ? 1 : 0, lhs.y != rhs.Value ? 1 : 0, lhs.z != rhs.Value ? 1 : 0,
				lhs.w != rhs.Value ? 1 : 0);
		}

		public static bool operator !=(Vector4Int lhs, (int x, int y, int z, int w) rhs)
		{
			return lhs.x != rhs.x && lhs.y != rhs.y && lhs.z != rhs.z && lhs.w != rhs.w;
		}

		/// <summary>
		///     Extended form of the !Vector4Int operation which allows checking for any number instead of just 0 for each individual component.
		///     <br>However, for readability, if checking all for a value of 0, using the !Vector4Int operation is recommended instead.</br>
		///     <para>The parameter 'rhs' being nullable is to differentiate between this operator and the one with its non-nullable counterpart.
		///     <br>Despite this, the value tuple of 'rhs' may not be null, as it will throw an exception if it is.</br></para>
		///     <para>Returns a vector containing 0s and 1s based on if a component of 'lhs' had the value of 'rhs' for said component or not.</para>
		/// </summary>
		public static Vector4Int operator !=(Vector4Int lhs, [DisallowNull] (int x, int y, int z, int w)? rhs)
		{
			if (!rhs.HasValue)
				throw new ArgumentNullException(nameof(rhs), "The given value tuple of 'rhs' may not be null.");

			return new Vector4Int(lhs.x != rhs.Value.x ? 1 : 0, lhs.y != rhs.Value.y ? 1 : 0,
				lhs.z != rhs.Value.z ? 1 : 0, lhs.w != rhs.Value.w ? 1 : 0);
		}

		public static bool operator >(Vector4Int lhs, Vector4Int rhs)
		{
			return lhs.x > rhs.x && lhs.y > rhs.y && lhs.z > rhs.z && lhs.w > rhs.w;
		}

		public static bool operator >(Vector4Int lhs, int rhs)
		{
			return lhs.x > rhs && lhs.y > rhs && lhs.z > rhs && lhs.w > rhs;
		}

		public static bool operator <(Vector4Int lhs, Vector4Int rhs)
		{
			return lhs.x < rhs.x && lhs.y < rhs.y && lhs.z < rhs.z && lhs.w < rhs.w;
		}

		public static bool operator <(Vector4Int lhs, int rhs)
		{
			return lhs.x < rhs && lhs.y < rhs && lhs.z < rhs && lhs.w < rhs;
		}

		public static bool operator >=(Vector4Int lhs, Vector4Int rhs)
		{
			return lhs.x >= rhs.x && lhs.y >= rhs.y && lhs.z >= rhs.z && lhs.w >= rhs.w;
		}

		public static bool operator >=(Vector4Int lhs, int rhs)
		{
			return lhs.x >= rhs && lhs.y >= rhs && lhs.z >= rhs && lhs.w >= rhs;
		}

		public static bool operator <=(Vector4Int lhs, Vector4Int rhs)
		{
			return lhs.x <= rhs.x && lhs.y <= rhs.y && lhs.z <= rhs.z && lhs.w <= rhs.w;
		}

		public static bool operator <=(Vector4Int lhs, int rhs)
		{
			return lhs.x <= rhs && lhs.y <= rhs && lhs.z <= rhs && lhs.w <= rhs;
		}

		// Logical operators
		public static Vector4Int operator &(Vector4Int lhs, Vector4Int rhs)
		{
			return new Vector4Int(lhs.x & rhs.x, lhs.y & rhs.y, lhs.z & rhs.z, lhs.w & rhs.w);
		}

		public static Vector4Int operator &(Vector4Int lhs, int rhs)
		{
			return new Vector4Int(lhs.x & rhs, lhs.y & rhs, lhs.z & rhs, lhs.w & rhs);
		}

		public static Vector4Int operator &(int lhs, Vector4Int rhs)
		{
			return new Vector4Int(lhs & rhs.x, lhs & rhs.y, lhs & rhs.z, lhs & rhs.w);
		}

		public static Vector4Int operator |(Vector4Int lhs, Vector4Int rhs)
		{
			return new Vector4Int(lhs.x | rhs.x, lhs.y | rhs.y, lhs.z | rhs.z, lhs.w | rhs.w);
		}

		public static Vector4Int operator |(Vector4Int lhs, int rhs)
		{
			return new Vector4Int(lhs.x | rhs, lhs.y | rhs, lhs.z | rhs, lhs.w | rhs);
		}

		public static Vector4Int operator |(int lhs, Vector4Int rhs)
		{
			return new Vector4Int(lhs | rhs.x, lhs | rhs.y, lhs | rhs.z, lhs | rhs.w);
		}

		public static Vector4Int operator ^(Vector4Int lhs, Vector4Int rhs)
		{
			return new Vector4Int(lhs.x ^ rhs.x, lhs.y ^ rhs.y, lhs.z ^ rhs.z, lhs.w ^ rhs.w);
		}

		public static Vector4Int operator ^(Vector4Int lhs, int rhs)
		{
			return new Vector4Int(lhs.x ^ rhs, lhs.y ^ rhs, lhs.z ^ rhs, lhs.w ^ rhs);
		}

		public static Vector4Int operator ^(int lhs, Vector4Int rhs)
		{
			return new Vector4Int(lhs ^ rhs.x, lhs ^ rhs.y, lhs ^ rhs.z, lhs ^ rhs.w);
		}

		public static Vector4Int operator ~(Vector4Int vec)
		{
			return new Vector4Int(~vec.x, ~vec.y, ~vec.z, ~vec.w);
		}

		public static Vector4Int operator <<(Vector4Int vec, int shiftl)
		{
			return new Vector4Int(vec.x << shiftl, vec.y << shiftl, vec.z << shiftl, vec.w << shiftl);
		}

		public static Vector4Int operator >> (Vector4Int vec, int shiftr)
		{
			return new Vector4Int(vec.x >> shiftr, vec.y >> shiftr, vec.z >> shiftr, vec.w >> shiftr);
		}

		/// <summary>
		///     Returns a vector containing 1s and 0s depending on if a component of 'vec' was a zero or non-zero value.
		///     <br>Can be doubled up to make !!vec, which returns a vector containing 0s and 1s depending on if a component of 'vec' was a zero or non-zero value.</br>
		/// </summary>
		public static Vector4Int operator !(Vector4Int vec)
		{
			return new Vector4Int(vec.x == 0 ? 1 : 0, vec.y == 0 ? 1 : 0, vec.z == 0 ? 1 : 0, vec.w == 0 ? 1 : 0);
		}

		// Additional methods
		/// <summary>
		///     Returns a new vector with all its x, y, z and w components set to the integer value of 'val'.
		///     <para>It is not recommended to use this method if making a new vector of all 0s or 1s.
		///     <br>In those cases, using the static properties, Vector4Int.zero and Vector4Int.one respectively, is recommended instead.</br></para>
		/// </summary>
		public static Vector4Int NewAllValue(int val)
		{
			return new Vector4Int(val, val, val, val);
		}

		/// <summary>
		///     Sets all the x, y, z and w components of this vector to the integer value of 'val'.
		/// </summary>
		public void Set(int val)
		{
			m_X = val;
			m_Y = val;
			m_Z = val;
			m_W = val;
		}

		/// <summary>
		///     Sets the x, y, z and w components of this vector.
		/// </summary>
		public void Set(int x, int y, int z, int w)
		{
			m_X = x;
			m_Y = y;
			m_Z = z;
			m_W = w;
		}

		/// <summary>
		///     Copies the x, y, z and w components from an existing vector over to this vector.
		/// </summary>
		public Vector4Int Copy(Vector4Int vec)
		{
			m_X = vec.x;
			m_Y = vec.y;
			m_Z = vec.z;
			m_W = vec.w;
			return new Vector4Int(vec.x, vec.y, vec.z, vec.w);
		}

		/// <summary>
		///     Resets the x, y, z and w components of this vector back to their origin point.
		/// </summary>
		public Vector4Int Reset()
		{
			m_X = m_Origin.x;
			m_Y = m_Origin.y;
			m_Z = m_Origin.z;
			m_W = m_Origin.w;
			return new Vector4Int(m_X, m_Y, m_Z, m_W);
		}

		/// <summary>
		///     Sets the origin point of this vector.
		/// </summary>
		public Vector4Int SetOriginHere()
		{
			m_Origin = (m_X, m_Y, m_Z, m_W);
			return new Vector4Int(m_Origin.x, m_Origin.y, m_Origin.z, m_Origin.w);
		}

		/// <summary>
		///     Sets the origin point of this vector to the specified position.
		/// </summary>
		public Vector4Int SetOriginTo(int x, int y, int z, int w)
		{
			m_Origin = (x, y, z, w);
			return new Vector4Int(m_Origin.x, m_Origin.y, m_Origin.z, m_Origin.w);
		}

		/// <summary>
		///     Sets the origin point of this vector to the specified position using an existing vector.
		/// </summary>
		public Vector4Int SetOriginTo(Vector4Int vec)
		{
			m_Origin = (vec.x, vec.y, vec.z, vec.w);
			return new Vector4Int(m_Origin.x, m_Origin.y, m_Origin.z, m_Origin.w);
		}

		/// <summary>
		///     Replaces any components in this vector or its origin point that match the value of 'oldNum' with the value of 'newNum'.
		/// </summary>
		public Vector4Int Replace(int oldNum, int newNum)
		{
			if (oldNum == newNum)
				return this;

			int prepx = x == oldNum ? newNum : x;
			int prepy = y == oldNum ? newNum : y;
			int prepz = z == oldNum ? newNum : z;
			int prepw = w == oldNum ? newNum : w;

			int orepx = m_Origin.x == oldNum ? newNum : m_Origin.x;
			int orepy = m_Origin.y == oldNum ? newNum : m_Origin.y;
			int orepz = m_Origin.z == oldNum ? newNum : m_Origin.z;
			int orepw = m_Origin.w == oldNum ? newNum : m_Origin.w;

			x = prepx;
			y = prepy;
			z = prepz;
			w = prepw;
			m_Origin = (orepx, orepy, orepz, orepw);
			return this;
		}

		/// <summary>
		///     Replaces any components in this vector that match the value of 'oldNum' with the value of 'newNum'.
		/// </summary>
		public Vector4Int ReplacePosition(int oldNum, int newNum)
		{
			if (oldNum == newNum)
				return this;

			int prepx = x == oldNum ? newNum : x;
			int prepy = y == oldNum ? newNum : y;
			int prepz = z == oldNum ? newNum : z;
			int prepw = w == oldNum ? newNum : w;
			x = prepx;
			y = prepy;
			z = prepz;
			w = prepw;
			return this;
		}

		/// <summary>
		///     Replaces any components in this vector's origin point that match the value of 'oldNum' with the value of 'newNum'.
		/// </summary>
		public Vector4Int ReplaceOrigin(int oldNum, int newNum)
		{
			if (oldNum == newNum)
				return this;

			int orepx = m_Origin.x == oldNum ? newNum : m_Origin.x;
			int orepy = m_Origin.y == oldNum ? newNum : m_Origin.y;
			int orepz = m_Origin.z == oldNum ? newNum : m_Origin.z;
			int orepw = m_Origin.w == oldNum ? newNum : m_Origin.w;
			m_Origin.x = orepx;
			m_Origin.y = orepy;
			m_Origin.z = orepz;
			m_Origin.w = orepw;
			return this;
		}

		/// <summary>
		///     Returns the absolute vector of the given vector.
		/// </summary>
		public static Vector4Int Abs(Vector4Int vec)
		{
			int ax = vec.x < 0 ? -vec.x : vec.x;
			int ay = vec.y < 0 ? -vec.y : vec.y;
			int az = vec.z < 0 ? -vec.z : vec.z;
			int aw = vec.w < 0 ? -vec.w : vec.w;
			return new Vector4Int(ax, ay, az, aw);
		}

		/// <summary>
		///     Converts this vector into its absolute vector.
		/// </summary>
		public Vector4Int Abs()
		{
			int ax = x < 0 ? -x : x;
			int ay = y < 0 ? -y : y;
			int az = z < 0 ? -z : z;
			int aw = w < 0 ? -w : w;
			x = ax;
			y = ay;
			z = az;
			w = aw;
			return new Vector4Int(ax, ay, az, aw);
		}

		/// <summary>
		///     Converts the given Vector4 to a Vector4Int by doing a Floor to each value.
		/// </summary>
		public static Vector4Int FloorToInt(Vector4 vec)
		{
			return new Vector4Int(Mathf.FloorToInt(vec.x), Mathf.FloorToInt(vec.y), Mathf.FloorToInt(vec.z),
				Mathf.FloorToInt(vec.w));
		}

		/// <summary>
		///     Converts the given Vector4 to a Vector4Int by doing a Ceil to each value.
		/// </summary>
		public static Vector4Int CeilToInt(Vector4 vec)
		{
			return new Vector4Int(Mathf.CeilToInt(vec.x), Mathf.CeilToInt(vec.y), Mathf.CeilToInt(vec.z),
				Mathf.CeilToInt(vec.w));
		}

		/// <summary>
		///     Converts the given Vector4 to a Vector4Int by doing a Round to each value.
		/// </summary>
		public static Vector4Int RoundToInt(Vector4 vec)
		{
			return new Vector4Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z),
				Mathf.RoundToInt(vec.w));
		}

		/// <summary>
		///     Multiplies two vectors component-wise and returns the result.
		/// </summary>
		public static Vector4Int Upscale(Vector4Int a, Vector4Int b)
		{
			return new Vector4Int(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
		}

		/// <summary>
		///     Multiplies every component of this vector by the same component of scale.
		/// </summary>
		public void Upscale(Vector4Int scale)
		{
			x *= scale.x;
			y *= scale.y;
			z *= scale.z;
			w *= scale.w;
		}

		/// <summary>
		///     Divides two vectors component-wise and returns the result.
		/// </summary>
		public static Vector4Int Downscale(Vector4Int a, Vector4Int b)
		{
			return new Vector4Int(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);
		}

		/// <summary>
		///     Divides every component of this vector by the same component of scale.
		/// </summary>
		public void Downscale(Vector4Int scale)
		{
			x /= scale.x;
			y /= scale.y;
			z /= scale.z;
			w /= scale.w;
		}

		/// <summary>
		///     Exponentiates two vectors component-wise and returns the result.
		/// </summary>
		public static Vector4Int Pow(Vector4Int a, Vector4Int b)
		{
			return new Vector4Int((int)Math.Pow(a.x, b.x), (int)Math.Pow(a.y, b.y), (int)Math.Pow(a.z, b.z),
				(int)Math.Pow(a.w, b.w));
		}

		/// <summary>
		///     Exponentiates every component of this vector by the same component of 'power'.
		/// </summary>
		public Vector4Int Pow(Vector4Int power)
		{
			int ex = (int)Math.Pow(x, power.x);
			int ey = (int)Math.Pow(y, power.y);
			int ez = (int)Math.Pow(z, power.z);
			int ew = (int)Math.Pow(w, power.w);
			x = ex;
			y = ey;
			z = ez;
			w = ew;
			return new Vector4Int(ex, ey, ez, ew);
		}

		/// <summary>
		///     Returns a vector with every component of the given vector exponentiated by the given integer 'b'.
		/// </summary>
		public static Vector4Int Pow(Vector4Int a, int b)
		{
			return new Vector4Int((int)Math.Pow(a.x, b), (int)Math.Pow(a.y, b), (int)Math.Pow(a.z, b),
				(int)Math.Pow(a.w, b));
		}

		/// <summary>
		///     Exponentiates every component of this vector by the given integer of 'power'.
		/// </summary>
		public Vector4Int Pow(int power)
		{
			int ex = (int)Math.Pow(x, power);
			int ey = (int)Math.Pow(y, power);
			int ez = (int)Math.Pow(z, power);
			int ew = (int)Math.Pow(w, power);
			x = ex;
			y = ey;
			z = ez;
			w = ew;
			return new Vector4Int(ex, ey, ez, ew);
		}

		/// <summary>
		///     Returns a vector with every component of the given vector exponentiated by the given floating-point 'b'.
		/// </summary>
		public static Vector4Int Pow(Vector4Int a, float b)
		{
			return new Vector4Int((int)Math.Pow(a.x, b), (int)Math.Pow(a.y, b), (int)Math.Pow(a.z, b),
				(int)Math.Pow(a.w, b));
		}

		/// <summary>
		///     Exponentiates every component of this vector by the given floating-point of 'power'.
		/// </summary>
		public Vector4Int Pow(float power)
		{
			int ex = (int)Math.Pow(x, power);
			int ey = (int)Math.Pow(y, power);
			int ez = (int)Math.Pow(z, power);
			int ew = (int)Math.Pow(w, power);
			x = ex;
			y = ey;
			z = ez;
			w = ew;
			return new Vector4Int(ex, ey, ez, ew);
		}

		/// <summary>
		///     Returns a vector with every component of the given vector exponentiated by the given double 'b'.
		/// </summary>
		public static Vector4Int Pow(Vector4Int a, double b)
		{
			return new Vector4Int((int)Math.Pow(a.x, b), (int)Math.Pow(a.y, b), (int)Math.Pow(a.z, b),
				(int)Math.Pow(a.w, b));
		}

		/// <summary>
		///     Exponentiates every component of this vector by the given double of 'power'.
		/// </summary>
		public Vector4Int Pow(double power)
		{
			int ex = (int)Math.Pow(x, power);
			int ey = (int)Math.Pow(y, power);
			int ez = (int)Math.Pow(z, power);
			int ew = (int)Math.Pow(w, power);
			x = ex;
			y = ey;
			z = ez;
			w = ew;
			return new Vector4Int(ex, ey, ez, ew);
		}

		/// <summary>
		///     Returns a vector with the square root of every component of the given vector.
		/// </summary>
		public static Vector4Int Sqrt(Vector4Int a)
		{
			return new Vector4Int((int)Math.Sqrt(a.x),
				(int)Math.Sqrt(a.y),
				(int)Math.Sqrt(a.z),
				(int)Math.Sqrt(a.w)
			);
		}

		/// <summary>
		///     Square roots every component of this vector.
		/// </summary>
		public Vector4Int Sqrt()
		{
			int rx = (int)Math.Sqrt(x);
			int ry = (int)Math.Sqrt(y);
			int rz = (int)Math.Sqrt(z);
			int rw = (int)Math.Sqrt(w);
			x = rx;
			y = ry;
			z = rz;
			w = rw;
			return new Vector4Int(rx, ry, rz, rw);
		}

		/// <summary>
		///     Returns a vector with the cube root of every component of the given vector.
		/// </summary>
		public static Vector4Int Cbrt(Vector4Int vec)
		{
			int rx = Math.Sign(vec.x) * (int)Math.Pow(Math.Abs(vec.x), 1.0 / 3.0);
			int ry = Math.Sign(vec.y) * (int)Math.Pow(Math.Abs(vec.y), 1.0 / 3.0);
			int rz = Math.Sign(vec.z) * (int)Math.Pow(Math.Abs(vec.z), 1.0 / 3.0);
			int rw = Math.Sign(vec.w) * (int)Math.Pow(Math.Abs(vec.w), 1.0 / 3.0);
			return new Vector4Int(rx, ry, rz, rw);
		}

		/// <summary>
		///     Cube roots every component of this vector.
		/// </summary>
		public Vector4Int Cbrt()
		{
			int rx = Math.Sign(x) * (int)Math.Pow(Math.Abs(x), 1.0 / 3.0);
			int ry = Math.Sign(y) * (int)Math.Pow(Math.Abs(y), 1.0 / 3.0);
			int rz = Math.Sign(z) * (int)Math.Pow(Math.Abs(z), 1.0 / 3.0);
			int rw = Math.Sign(w) * (int)Math.Pow(Math.Abs(w), 1.0 / 3.0);
			x = rx;
			y = ry;
			z = rz;
			w = rw;
			return new Vector4Int(rx, ry, rz, rw);
		}

		/// <summary>
		///     Returns a vector with the given integer root index 'b' of every component of the given vector.
		///     <br>If using a root index of 2, it is recommended to use Sqrt(Vector4Int) instead.</br>
		/// </summary>
		public static Vector4Int Root(Vector4Int a, int b)
		{
			return new Vector4Int((int)Math.Pow(a.x, 1.0 / b),
				(int)Math.Pow(a.y, 1.0 / b),
				(int)Math.Pow(a.z, 1.0 / b),
				(int)Math.Pow(a.w, 1.0 / b)
			);
		}

		/// <summary>
		///     Roots every component of this vector by the given integer root index 'n'.
		///     <br>If using a root index of 2, it is recommended to use Vector4Int.Sqrt instead.</br>
		/// </summary>
		public Vector4Int Root(int n)
		{
			int rx = (int)Math.Pow(x, 1.0 / n);
			int ry = (int)Math.Pow(y, 1.0 / n);
			int rz = (int)Math.Pow(z, 1.0 / n);
			int rw = (int)Math.Pow(w, 1.0 / n);
			x = rx;
			y = ry;
			z = rz;
			w = rw;
			return new Vector4Int(rx, ry, rz, rw);
		}

		/// <summary>
		///     Roots every component of this vector by the given floating-point root index 'b'.
		///     <br>If using a root index of 2f, it is recommended to use Sqrt(Vector4Int) instead.</br>
		/// </summary>
		public static Vector4Int Root(Vector4Int a, float b)
		{
			return new Vector4Int((int)Math.Pow(a.x, 1.0 / b),
				(int)Math.Pow(a.y, 1.0 / b),
				(int)Math.Pow(a.z, 1.0 / b),
				(int)Math.Pow(a.w, 1.0 / b)
			);
		}

		/// <summary>
		///     Roots every component of this vector by the given floating-point root index 'n'.
		///     <br>If using a root index of 2f, it is recommended to use Vector4Int.Sqrt instead.</br>
		/// </summary>
		public Vector4Int Root(float n)
		{
			int rx = (int)Math.Pow(x, 1.0 / n);
			int ry = (int)Math.Pow(y, 1.0 / n);
			int rz = (int)Math.Pow(z, 1.0 / n);
			int rw = (int)Math.Pow(w, 1.0 / n);
			x = rx;
			y = ry;
			z = rz;
			w = rw;
			return new Vector4Int(rx, ry, rz, rw);
		}

		/// <summary>
		///     Returns a vector with the given double root index 'b' of every component of the given vector.
		///     <br>If using a root index of 2.0, it is recommended to use Sqrt(Vector4Int) instead.</br>
		/// </summary>
		public static Vector4Int Root(Vector4Int a, double b)
		{
			return new Vector4Int((int)Math.Pow(a.x, 1.0 / b),
				(int)Math.Pow(a.y, 1.0 / b),
				(int)Math.Pow(a.z, 1.0 / b),
				(int)Math.Pow(a.w, 1.0 / b)
			);
		}

		/// <summary>
		///     Roots every component of this vector by the given double root index 'n'.
		///     <br>If using a root index of 2.0, it is recommended to use Vector4Int.Sqrt instead.</br>
		/// </summary>
		public Vector4Int Root(double n)
		{
			int rx = (int)Math.Pow(x, 1.0 / n);
			int ry = (int)Math.Pow(y, 1.0 / n);
			int rz = (int)Math.Pow(z, 1.0 / n);
			int rw = (int)Math.Pow(w, 1.0 / n);
			x = rx;
			y = ry;
			z = rz;
			w = rw;
			return new Vector4Int(rx, ry, rz, rw);
		}

		/// <summary>
		///     Returns the logarithm of the given vector in the specified base.
		///     <br>If using the bases of e, pi, phi, 10, or 2, it is recommended to use their specialized log methods instead.</br>
		/// </summary>
		public static Vector4Int Log(Vector4Int vec, int baseValue)
		{
			return new Vector4Int(
				(int)Math.Log(vec.x, baseValue),
				(int)Math.Log(vec.y, baseValue),
				(int)Math.Log(vec.z, baseValue),
				(int)Math.Log(vec.w, baseValue)
			);
		}

		/// <summary>
		///     Sets every component in this vector to its logarithm in the specified base.
		///     <br>If using the bases of e, pi, phi, 10, or 2, it is recommended to use their specialized log methods instead.</br>
		/// </summary>
		public Vector4Int Log(int baseValue)
		{
			int lx = (int)Math.Log(x, baseValue);
			int ly = (int)Math.Log(x, baseValue);
			int lz = (int)Math.Log(x, baseValue);
			int lw = (int)Math.Log(x, baseValue);
			x = lx;
			y = ly;
			z = lz;
			w = lw;
			return new Vector4Int(lx, ly, lz, lw);
		}

		/// <summary>
		///     Returns the natural logarithm of the given vector.
		/// </summary>
		public static Vector4Int LogN(Vector4Int vec)
		{
			return new Vector4Int(
				(int)Math.Log(vec.x),
				(int)Math.Log(vec.y),
				(int)Math.Log(vec.z),
				(int)Math.Log(vec.w)
			);
		}

		/// <summary>
		///     Sets every component in this vector to its natural logarithm.
		/// </summary>
		public Vector4Int LogN()
		{
			int lnx = (int)Math.Log(x);
			int lny = (int)Math.Log(y);
			int lnz = (int)Math.Log(z);
			int lnw = (int)Math.Log(w);
			x = lnx;
			y = lny;
			z = lnz;
			w = lnw;
			return new Vector4Int(lnx, lny, lnz, lnw);
		}

		/// <summary>
		///     Returns the pi-base logarithm of the given vector.
		/// </summary>
		public static Vector4Int LogPi(Vector4Int vec)
		{
			return new Vector4Int(
				(int)Math.Log(vec.x, Math.PI),
				(int)Math.Log(vec.y, Math.PI),
				(int)Math.Log(vec.z, Math.PI),
				(int)Math.Log(vec.w, Math.PI)
			);
		}

		/// <summary>
		///     Sets every component in this vector to its pi-base logarithm.
		/// </summary>
		public Vector4Int LogPi()
		{
			int lpx = (int)Math.Log(x, Math.PI);
			int lpy = (int)Math.Log(y, Math.PI);
			int lpz = (int)Math.Log(z, Math.PI);
			int lpw = (int)Math.Log(w, Math.PI);
			x = lpx;
			y = lpy;
			z = lpz;
			w = lpw;
			return new Vector4Int(lpx, lpy, lpz, lpw);
		}

		/// <summary>
		///     Returns the phi-base logarithm of the given vector.
		/// </summary>
		public static Vector4Int LogPhi(Vector4Int vec)
		{
			return new Vector4Int(
				(int)Math.Log(vec.x, (1 + Math.Sqrt(5)) / 2),
				(int)Math.Log(vec.y, (1 + Math.Sqrt(5)) / 2),
				(int)Math.Log(vec.z, (1 + Math.Sqrt(5)) / 2),
				(int)Math.Log(vec.w, (1 + Math.Sqrt(5)) / 2)
			);
		}

		/// <summary>
		///     Sets every component in this vector to its phi-base logarithm.
		/// </summary>
		public Vector4Int LogPhi()
		{
			int lphx = (int)Math.Log(x, (1 + Math.Sqrt(5)) / 2);
			int lphy = (int)Math.Log(y, (1 + Math.Sqrt(5)) / 2);
			int lphz = (int)Math.Log(z, (1 + Math.Sqrt(5)) / 2);
			int lphw = (int)Math.Log(w, (1 + Math.Sqrt(5)) / 2);
			x = lphx;
			y = lphy;
			z = lphz;
			w = lphw;
			return new Vector4Int(lphx, lphy, lphz, lphw);
		}

		/// <summary>
		///     Returns the 10-base logarithm of the given vector.
		/// </summary>
		public static Vector4Int Log10(Vector4Int vec)
		{
			return new Vector4Int(
				(int)Math.Log10(vec.x),
				(int)Math.Log10(vec.y),
				(int)Math.Log10(vec.z),
				(int)Math.Log10(vec.w)
			);
		}

		/// <summary>
		///     Sets every component in this vector to its 10-base logarithm.
		/// </summary>
		public Vector4Int Log10()
		{
			int ltnx = (int)Math.Log10(x);
			int ltny = (int)Math.Log10(y);
			int ltnz = (int)Math.Log10(z);
			int ltnw = (int)Math.Log10(w);
			x = ltnx;
			y = ltny;
			z = ltnz;
			w = ltnw;
			return new Vector4Int(ltnx, ltny, ltnz, ltnw);
		}

		/// <summary>
		///     Returns the 2-base logarithm of the given vector.
		/// </summary>
		public static Vector4Int Log2(Vector4Int vec)
		{
			return new Vector4Int(
				(int)Math.Log(vec.x, 2),
				(int)Math.Log(vec.y, 2),
				(int)Math.Log(vec.z, 2),
				(int)Math.Log(vec.w, 2)
			);
		}

		/// <summary>
		///     Sets every component in this vector to its 10-base logarithm.
		/// </summary>
		public Vector4Int Log2()
		{
			int ltx = (int)Math.Log(x, 2);
			int lty = (int)Math.Log(y, 2);
			int ltz = (int)Math.Log(z, 2);
			int ltw = (int)Math.Log(w, 2);
			x = ltx;
			y = lty;
			z = ltz;
			w = ltw;
			return new Vector4Int(ltx, lty, ltz, ltw);
		}

		/// <summary>
		///     Returns the factorial of the given vector.
		/// </summary>
		public static Vector4Int Fact(Vector4Int vec)
		{
			if (vec.ContainsLessThanEqual(-1))
				throw new InvalidOperationException("The given vector may not contain negative numbers.");
			Vector4Int copyVec = vec;
			for (int comp = 0; comp < 4; comp++)
			{
				if (copyVec[comp] <= 1)
					copyVec[comp] = 1;
				else
				{
					int result = 1;
					for (int i = 2; i <= copyVec[comp]; i++)
					{
						result *= i;
					}

					copyVec[comp] = result;
				}
			}

			return copyVec;
		}

		/// <summary>
		///     Sets every component in this vector to its factorial.
		/// </summary>
		public Vector4Int Fact()
		{
			if (ContainsLessThanEqual(-1))
				throw new InvalidOperationException("This vector may not contain negative numbers.");
			for (int comp = 0; comp < 4; comp++)
			{
				if (this[comp] <= 1)
					this[comp] = 1;
				else
				{
					int result = 1;
					for (int i = 2; i <= this[comp]; i++)
					{
						result *= i;
					}

					this[comp] = result;
				}
			}

			return this;
		}

		/// <summary>
		///     Returns the inverse factorial of the given vector.
		/// </summary>
		public static Vector4Int InverseFact(Vector4Int vec)
		{
			if (vec.ContainsLessThanEqual(-1))
				throw new InvalidOperationException("The given vector may not contain negative numbers.");
			Vector4Int copyVec = vec;
			for (int comp = 0; comp < 4; comp++)
			{
				int result = 1;
				int i = 2;
				while (result <= copyVec[comp])
				{
					result *= i;
					i++;
				}

				copyVec[comp] = i - 2; // Subtract 2 to get the correct inverse factorial
			}

			return copyVec;
		}

		/// <summary>
		///     Sets every component in this vector to its inverse factorial.
		/// </summary>
		public Vector4Int InverseFact()
		{
			if (ContainsLessThanEqual(-1))
				throw new InvalidOperationException("This vector may not contain negative numbers.");
			for (int comp = 0; comp < 4; comp++)
			{
				int result = 1;
				int i = 2;
				while (result <= this[comp])
				{
					result *= i;
					i++;
				}

				this[comp] = i - 2; // Subtract 2 to get the correct inverse factorial
			}

			return this;
		}

		/// <summary>
		///     Returns a vector that is made from the smallest components of the two given vectors.
		/// </summary>
		public static Vector4Int Min(Vector4Int lhs, Vector4Int rhs)
		{
			return new Vector4Int(
				Mathf.Min(lhs.x, rhs.x),
				Mathf.Min(lhs.y, rhs.y),
				Mathf.Min(lhs.z, rhs.z),
				Mathf.Min(lhs.w, rhs.w)
			);
		}

		/// <summary>
		///     Returns a vector that is made from the averages between the components of the two given vectors.
		/// </summary>
		public static Vector4Int Mid(Vector4Int lhs, Vector4Int rhs)
		{
			return new Vector4Int(
				(lhs.x + rhs.x) / 2,
				(lhs.y + rhs.y) / 2,
				(lhs.z + rhs.z) / 2,
				(lhs.w + rhs.w) / 2
			);
		}

		/// <summary>
		///     Returns a vector that is made from the largest components of the two given vectors.
		/// </summary>
		public static Vector4Int Max(Vector4Int lhs, Vector4Int rhs)
		{
			return new Vector4Int(
				Mathf.Max(lhs.x, rhs.x),
				Mathf.Max(lhs.y, rhs.y),
				Mathf.Max(lhs.z, rhs.z),
				Mathf.Max(lhs.w, rhs.w)
			);
		}

		/// <summary>
		///     Returns a vector that is in the center of each component of the given vector, rounded to the nearest integer.
		/// </summary>
		public static Vector4Int Center(Vector4Int vec)
		{
			return new Vector4Int(
				(int)Math.Round((double)(vec.x / 4)),
				(int)Math.Round((double)(vec.y / 4)),
				(int)Math.Round((double)(vec.z / 4)),
				(int)Math.Round((double)(vec.w / 4))
			);
		}

		/// <summary>
		///     Centers this vector to the point that is in the center of each component, rounded to the nearest integer.
		/// </summary>
		public Vector4Int Center()
		{
			int mcx = (int)Math.Round((double)(x / 4));
			int mcy = (int)Math.Round((double)(y / 4));
			int mcz = (int)Math.Round((double)(z / 4));
			int mcw = (int)Math.Round((double)(w / 4));
			x = mcx;
			y = mcy;
			z = mcz;
			w = mcw;
			return new Vector4Int(mcx, mcy, mcz, mcw);
		}

		/// <summary>
		///     Returns a vector that is in the center of each component of the given vector, mirrored along the specified axes, and rounded to the nearest integer.
		///     <br>If you are not mirroring the center point along any of the axes, it is recommended to use Center(Vector4Int) instead.</br>
		/// </summary>
		public static Vector4Int MirrCenter(Vector4Int vec, (bool x, bool y, bool z, bool w) mirroredAxes)
		{
			int mx = mirroredAxes.x ? -1 : 1;
			int my = mirroredAxes.y ? -1 : 1;
			int mz = mirroredAxes.z ? -1 : 1;
			int mw = mirroredAxes.w ? -1 : 1;
			return new Vector4Int(
				mx * (int)Math.Round((double)(vec.x / 4)),
				my * (int)Math.Round((double)(vec.y / 4)),
				mz * (int)Math.Round((double)(vec.z / 4)),
				mw * (int)Math.Round((double)(vec.w / 4))
			);
		}

		/// <summary>
		///     Centers this vector to the point that is in the center of each component, mirrored along the specified axes, and rounded to the nearest integer.
		///     <br>If you are not mirroring the center point along any of the axes, it is recommended to use Vector4Int.Center instead.</br>
		/// </summary>
		public Vector4Int MirrCenter((bool x, bool y, bool z, bool w) mirroredAxes)
		{
			int mx = mirroredAxes.x ? -1 : 1;
			int my = mirroredAxes.y ? -1 : 1;
			int mz = mirroredAxes.z ? -1 : 1;
			int mw = mirroredAxes.w ? -1 : 1;
			int mcx = mx * (int)Math.Round((double)(x / 4));
			int mcy = my * (int)Math.Round((double)(y / 4));
			int mcz = mz * (int)Math.Round((double)(z / 4));
			int mcw = mw * (int)Math.Round((double)(w / 4));
			x = mcx;
			y = mcy;
			z = mcz;
			w = mcw;
			return new Vector4Int(mcx, mcy, mcz, mcw);
		}

		/// <summary>
		///     Returns the Dot Product of vectors a and b.
		/// </summary>
		public static int Dot(Vector4Int a, Vector4Int b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}

		/// <summary>
		///     Returns the Antidot Product of vectors a and b.
		/// </summary>
		public static int Antidot(Vector4Int a, Vector4Int b)
		{
			return a.x * b.x - a.y * b.y - a.z * b.z - a.w * b.w;
		}

		/// <summary>
		///    Returns the Cross Product of vectors a and b.
		/// </summary>
		public static Vector4Int Cross(Vector4Int a, Vector4Int b)
		{
			return new Vector4Int(
				a.y * b.z - a.z * b.y,
				a.z * b.w - a.w * b.z,
				a.w * b.x - a.x * b.w,
				a.x * b.y - a.y * b.x
			);
		}

		/// <summary>
		///    Returns the Anticross Product of vectors a and b.
		/// </summary>
		public static Vector4Int Anticross(Vector4Int a, Vector4Int b)
		{
			return new Vector4Int(
				a.y * b.z + a.z * b.y,
				a.z * b.w + a.w * b.z,
				a.w * b.x + a.x * b.w,
				a.x * b.y + a.y * b.x
			);
		}


		/// <summary>
		///    Returns the Sum Product of vectors a and b.
		/// </summary>
		public static Vector4Int Sum(Vector4Int a, Vector4Int b)
		{
			return new Vector4Int(
				a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w,
				a.x * b.y + a.y * b.x + a.z * b.w + a.w * b.z,
				a.x * b.z + a.y * b.w + a.z * b.x + a.w * b.y,
				a.x * b.w + a.y * b.z + a.z * b.y + a.w * b.x
			);
		}

		/// <summary>
		///    Returns the Difference Product of vectors a and b.
		/// </summary>
		public static Vector4Int Difference(Vector4Int a, Vector4Int b)
		{
			return new Vector4Int(
				a.x * b.x - a.y * b.y - a.z * b.z - a.w * b.w,
				a.x * b.y - a.y * b.x - a.z * b.w - a.w * b.z,
				a.x * b.z - a.y * b.w - a.z * b.x - a.w * b.y,
				a.x * b.w - a.y * b.z - a.z * b.y - a.w * b.x
			);
		}

		/// <summary>
		///    Returns the Product Product of vectors a and b.
		/// </summary>
		public static Vector4Int Product(Vector4Int a, Vector4Int b)
		{
			return new Vector4Int(
				(a.x * b.x) * (a.y * b.y) * (a.z * b.z) * (a.w * b.w),
				(a.x * b.y) * (a.y * b.x) * (a.z * b.w) * (a.w * b.z),
				(a.x * b.z) * (a.y * b.w) * (a.z * b.x) * (a.w * b.y),
				(a.x * b.w) * (a.y * b.z) * (a.z * b.y) * (a.w * b.x)
			);
		}

		/// <summary>
		///    Returns the Quotient Product of vectors a and b.
		/// </summary>
		public static Vector4Int Quotient(Vector4Int a, Vector4Int b)
		{
			return new Vector4Int(
				(a.x * b.x) / (a.y * b.y) / (a.z * b.z) / (a.w * b.w),
				(a.x * b.y) / (a.y * b.x) / (a.z * b.w) / (a.w * b.z),
				(a.x * b.z) / (a.y * b.w) / (a.z * b.x) / (a.w * b.y),
				(a.x * b.w) / (a.y * b.z) / (a.z * b.y) / (a.w * b.x)
			);
		}

		/// <summary>
		///    Computes the intersection point of three planes defined by three non-collinear points in 4D space.
		///    <para>Returns a vector of zero if the given vectors are collinear or if they all coincide.</para>
		/// </summary>
		public static Vector4Int Triangulate(Vector4Int a, Vector4Int b, Vector4Int c)
		{
			// Check if all points are the same
			if (a == b && b == c)
			{
				// All points are the same, return a default value
				return zero;
			}

			// Check for collinear points
			Vector4Int ab = b - a;
			Vector4Int ac = c - a;
			if (Cross(ab, ac) == zero)
			{
				// The points are collinear, return a default value
				return zero;
			}

			// Compute normal vectors of the three planes defined by the vectors
			Vector4Int n1 = Cross(ab, ac);
			Vector4Int n2 = Cross(c - b, a - b);
			Vector4Int n3 = Cross(a - c, b - c);

			// Compute the intersection point of the three planes
			int denom = Dot(n1, Cross(n2, n3));
			Vector4Int p = (-n1.w * Cross(n2, n3) - n2.w * Cross(n3, n1) - n3.w * Cross(n1, n2)) / denom;

			return p;
		}

		/// <summary>
		///     Returns the slope between vectors a and b.
		/// </summary>
		public static double Slope(Vector4Int a, Vector4Int b)
		{
			double dx = b.x - a.x;
			double dy = b.y - a.y;
			double dz = b.z - a.z;
			double dw = b.w - a.w;

			if (dx == 0 && dy == 0 && dz == 0 && dw == 0)
			{
				// The two points are the same, so the slope is undefined
				return double.NaN;
			}
			else
			{
				// Calculate the 4D slope using the formula: slope = deltaY / deltaX
				return Math.Sqrt(dx * dx + dy * dy + dz * dz + dw * dw) /
				       Math.Sqrt(4 * (dx * dx + dy * dy + dz * dz + dw * dw));
			}
		}

		/// <summary>
		///     Returns the slope between this vector and another vector.
		/// </summary>
		public double Slope(Vector4Int vec)
		{
			double dx = vec.x - x;
			double dy = vec.y - y;
			double dz = vec.z - z;
			double dw = vec.w - w;

			if (dx == 0 && dy == 0 && dz == 0 && dw == 0)
			{
				// The two points are the same, so the slope is undefined
				return double.NaN;
			}
			else
			{
				// Calculate the 4D slope using the formula: slope = deltaY / deltaX
				return Math.Sqrt(dx * dx + dy * dy + dz * dz + dw * dw) /
				       Math.Sqrt(4 * (dx * dx + dy * dy + dz * dz + dw * dw));
			}
		}

		/// <summary>
		///     Reflects a vector off the plane defined by a normal.
		/// </summary>
		public static Vector4Int Reflect(Vector4Int inDirection, Vector4Int inNormal)
		{
			int num = -2 * Dot(inNormal, inDirection);
			return new Vector4Int(
				num * inNormal.x + inDirection.x,
				num * inNormal.y + inDirection.y,
				num * inNormal.z + inDirection.z,
				num * inNormal.w + inDirection.w
			);
		}

		/// <summary>
		///     Reflects this vector off the plane defined by a normal.
		/// </summary>
		public Vector4Int Reflect(Vector4Int inNormal)
		{
			int num = -2 * Dot(inNormal, this);
			int rfx = num * inNormal.x + x;
			int rfy = num * inNormal.y + y;
			int rfz = num * inNormal.z + z;
			int rfw = num * inNormal.w + w;
			x = rfx;
			y = rfy;
			z = rfz;
			w = rfw;
			return new Vector4Int(rfx, rfy, rfz, rfw);
		}

		/// <summary>
		///     Projects a vector onto another vector.
		/// </summary>
		public static Vector4Int Project(Vector4Int vec, Vector4Int onNormal)
		{
			int num = Dot(onNormal, onNormal);
			if (num < 1)
			{
				return zero;
			}

			int num2 = Dot(vec, onNormal);
			return new Vector4Int(
				onNormal.x * num2 / num,
				onNormal.y * num2 / num,
				onNormal.z * num2 / num,
				onNormal.w * num2 / num
			);
		}

		/// <summary>
		///     Projects this vector onto another vector.
		/// </summary>
		public Vector4Int Project(Vector4Int onNormal)
		{
			int num = Dot(onNormal, onNormal);
			if (num < 1)
			{
				this = zero;
				return zero;
			}

			int num2 = Dot(this, onNormal);
			int px = onNormal.x * num2 / num;
			int py = onNormal.y * num2 / num;
			int pz = onNormal.z * num2 / num;
			int pw = onNormal.w * num2 / num;
			x = px;
			y = py;
			z = pz;
			w = pw;
			return new Vector4Int(px, py, pz, pw);
		}

		/// <summary>
		///     Projects a vector onto a plane defined by a normal orthogonal to the plane.
		/// </summary>
		public static Vector4Int ProjectOnPlane(Vector4Int vec, Vector4Int planeNormal)
		{
			int num = Dot(planeNormal, planeNormal);
			if (num < 1)
			{
				return zero;
			}

			int num2 = Dot(vec, planeNormal);
			return new Vector4Int(
				vec.x - planeNormal.x * num2 / num,
				vec.y - planeNormal.y * num2 / num,
				vec.z - planeNormal.z * num2 / num,
				vec.w - planeNormal.w * num2 / num
			);
		}

		/// <summary>
		///     Projects a vector onto a plane defined by a normal orthogonal to the plane.
		/// </summary>
		public Vector4Int ProjectOnPlane(Vector4Int planeNormal)
		{
			int num = Dot(planeNormal, planeNormal);
			if (num < 1)
			{
				this = zero;
				return zero;
			}

			int num2 = Dot(this, planeNormal);
			int popx = x - planeNormal.x * num2 / num;
			int popy = y - planeNormal.y * num2 / num;
			int popz = z - planeNormal.z * num2 / num;
			int popw = w - planeNormal.w * num2 / num;
			x = popx;
			y = popy;
			z = popz;
			w = popw;
			return new Vector4Int(popx, popy, popz, popw);
		}

		/// <summary>
		///     Calculates the angle between the vectors of 'from' and 'to'.
		/// </summary>
		public static float Angle(Vector4Int from, Vector4Int to)
		{
			float num = (float)Math.Sqrt(Dot(from, from) * Dot(to, to));
			if (num < 1E-15f)
			{
				return 0f;
			}

			float num2 = Mathf.Clamp(Dot(from, to) / num, -1f, 1f);
			return (float)(Math.Acos(num2) * 57.29578f);
		}

		/// <summary>
		///     Calculates the signed angle between the vectors of 'from' and 'to' in relation to 'axis'.
		/// </summary>
		public static float SignedAngle(Vector4Int from, Vector4Int to, Vector4Int axis)
		{
			float num = Angle(from, to);
			float num2 = from.y * to.z - from.z * to.y;
			float num3 = from.z * to.w - from.w * to.z;
			float num4 = from.w * to.x - from.x * to.w;
			float num5 = from.x * to.y - from.y * to.x;
			float num6 = Mathf.Sign(axis.x * num2 + axis.y * num3 + axis.z * num4 + axis.w * num5);
			return num * num6;
		}

		/// <summary>
		///     Calculates the angle between the vectors of 'from' and 'to' in radians.
		/// </summary>
		public static float RadAngle(Vector4Int from, Vector4Int to)
		{
			float dotProduct = Dot(Normalize(from), Normalize(to));
			return Mathf.Acos(Mathf.Clamp(dotProduct, -1f, 1f));
		}

		/// <summary>
		///     Calculates the signed angle between the vectors of 'from' and 'to' in relation to 'axis' in radians.
		/// </summary>
		public static float SignedRadAngle(Vector4Int from, Vector4Int to, Vector4Int axis)
		{
			float num = RadAngle(from, to);
			float num2 = from.y * to.z - from.z * to.y;
			float num3 = from.z * to.w - from.w * to.z;
			float num4 = from.w * to.x - from.x * to.w;
			float num5 = from.x * to.y - from.y * to.x;
			float num6 = Mathf.Sign(axis.x * num2 + axis.y * num3 + axis.z * num4 + axis.w * num5);
			return num * num6;
		}

		/// <summary>
		///     Returns the distance between vectors a and b.
		/// </summary>
		public static double Distance(Vector4Int a, Vector4Int b)
		{
			int dx = b.x - a.x;
			int dy = b.y - a.y;
			int dz = b.z - a.z;
			int dw = b.w - a.w;
			return (double)Math.Sqrt(dx * dx + dy * dy + dz * dz + dw * dw);
		}

		/// <summary>
		///     Returns the distance between this vector and another vector.
		/// </summary>
		public double Distance(Vector4Int vec)
		{
			int dx = vec.x - x;
			int dy = vec.y - y;
			int dz = vec.z - z;
			int dw = vec.w - w;
			return (double)Math.Sqrt(dx * dx + dy * dy + dz * dz + dw * dw);
		}

		/// <summary>
		///     Returns the distance between the given vector and Vector4Int(0, 0, 0, 0).
		/// </summary>
		public static double DistanceFromZero(Vector4Int vec)
		{
			int dx = -vec.x;
			int dy = -vec.y;
			int dz = -vec.z;
			int dw = -vec.w;
			return (double)Math.Sqrt(dx * dx + dy * dy + dz * dz + dw * dw);
		}

		/// <summary>
		///     Returns the distance between this vector and Vector4Int(0, 0, 0, 0).
		/// </summary>
		public double DistanceFromZero()
		{
			int dx = -x;
			int dy = -y;
			int dz = -z;
			int dw = -w;
			return (double)Math.Sqrt(dx * dx + dy * dy + dz * dz + dw * dw);
		}

		/// <summary>
		///     Returns the magnitude of the given vector.
		/// </summary>
		public static double Magnitude(Vector4Int vec)
		{
			return (double)Math.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z + vec.w * vec.w);
		}

		/// <summary>
		///     Returns the magnitude of this vector.
		/// </summary>
		public double Magnitude()
		{
			return (double)Math.Sqrt(x * x + y * y + z * z + w * w);
		}

		/// <summary>
		///     Returns the square magnitude of the given vector.
		/// </summary>
		public static double SqrMagnitude(Vector4Int vec)
		{
			return vec.x * vec.x + vec.y * vec.y + vec.z * vec.z + vec.w * vec.w;
		}

		/// <summary>
		///     Returns the square magnitude of this vector.
		/// </summary>
		public double SqrMagnitude()
		{
			return x * x + y * y + z * z + w * w;
		}

		/// <summary>
		///     Returns the cubic magnitude of the given vector.
		/// </summary>
		public static double CbcMagnitude(Vector4Int vec)
		{
			return Math.Pow(Math.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z + vec.w * vec.w), 3);
		}

		/// <summary>
		///     Returns the cubic magnitude of this vector.
		/// </summary>
		public double CbcMagnitude()
		{
			return Math.Pow(Math.Sqrt(x * x + y * y + z * z + w * w), 3);
		}

		/// <summary>
		///     Returns the natural magnitude of the given vector.
		/// </summary>
		public static double NatMagnitude(Vector4Int vec)
		{
			return Math.Pow(Math.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z + vec.w * vec.w), Math.E);
		}

		/// <summary>
		///     Returns the natural magnitude of this vector.
		/// </summary>
		public double NatMagnitude()
		{
			return Math.Pow(Math.Sqrt(x * x + y * y + z * z + w * w), Math.E);
		}

		/// <summary>
		///     Returns the magnitude raised to the power of pi of the given vector.
		/// </summary>
		public static double PiMagnitude(Vector4Int vec)
		{
			return Math.Pow(Math.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z + vec.w * vec.w), Math.PI);
		}

		/// <summary>
		///     Returns the magnitude raised to the power of pi of this vector.
		/// </summary>
		public double PiMagnitude()
		{
			return Math.Pow(Math.Sqrt(x * x + y * y + z * z + w * w), Math.PI);
		}

		/// <summary>
		///     Returns the magnitude raised to the power of phi of the given vector.
		/// </summary>
		public static double PhiMagnitude(Vector4Int vec)
		{
			return Math.Pow(Math.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z + vec.w * vec.w),
				(1 + Math.Sqrt(5)) / 2);
		}

		/// <summary>
		///     Returns the magnitude raised to the power of phi of this vector.
		/// </summary>
		public double PhiMagnitude()
		{
			return Math.Pow(Math.Sqrt(x * x + y * y + z * z + w * w), (1 + Math.Sqrt(5)) / 2);
		}

		/// <summary>
		///     Returns the magnitude raised to the given integer of 'power' of the given vector.
		/// </summary>
		public static double PowMagnitude(Vector4Int vec, int power)
		{
			return Math.Pow(Math.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z + vec.w * vec.w), power);
		}

		/// <summary>
		///     Returns the magnitude raised to the given double of 'power' of this vector.
		/// </summary>
		public double PowMagnitude(int power)
		{
			return Math.Pow(Math.Sqrt(x * x + y * y + z * z + w * w), power);
		}

		/// <summary>
		///     Returns the magnitude raised to the given floating-point of 'power' of the given vector.
		/// </summary>
		public static double PowMagnitude(Vector4Int vec, float power)
		{
			return Math.Pow(Math.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z + vec.w * vec.w), power);
		}

		/// <summary>
		///     Returns the magnitude raised to the given double of 'power' of this vector.
		/// </summary>
		public double PowMagnitude(float power)
		{
			return Math.Pow(Math.Sqrt(x * x + y * y + z * z + w * w), power);
		}

		/// <summary>
		///     Returns the magnitude raised to the given double of 'power' of the given vector.
		/// </summary>
		public static double PowMagnitude(Vector4Int vec, double power)
		{
			return Math.Pow(Math.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z + vec.w * vec.w), power);
		}

		/// <summary>
		///     Returns the magnitude raised to the given double of 'power' of this vector.
		/// </summary>
		public double PowMagnitude(double power)
		{
			return Math.Pow(Math.Sqrt(x * x + y * y + z * z + w * w), power);
		}

		/// <summary>
		///     Returns the exponentiated magnitude of the given vector.
		/// </summary>
		public static double ExpMagnitude(Vector4Int vec)
		{
			return (double)Math.Pow(Magnitude(vec), Magnitude(vec));
		}

		/// <summary>
		///     Returns the exponentiated magnitude of this vector.
		/// </summary>
		public double ExpMagnitude()
		{
			return (double)Math.Pow(Magnitude(this), Magnitude(this));
		}

		/// <summary>
		///     Returns the exponentiated square magnitude of the given vector.
		/// </summary>
		public static double ExpSqrMagnitude(Vector4Int vec)
		{
			return (double)Math.Pow(SqrMagnitude(vec), SqrMagnitude(vec));
		}

		/// <summary>
		///     Returns the exponentiated square magnitude of this vector.
		/// </summary>
		public double ExpSqrMagnitude()
		{
			return (double)Math.Pow(SqrMagnitude(this), SqrMagnitude(this));
		}

		/// <summary>
		///     Returns the exponentiated cubic magnitude of the given vector.
		/// </summary>
		public static double ExpCbcMagnitude(Vector4Int vec)
		{
			return (double)Math.Pow(CbcMagnitude(vec), CbcMagnitude(vec));
		}

		/// <summary>
		///     Returns the exponentiated cubic magnitude of this vector.
		/// </summary>
		public double ExpCbcMagnitude()
		{
			return (double)Math.Pow(CbcMagnitude(this), CbcMagnitude(this));
		}

		/// <summary>
		///     Returns the exponentiated natural magnitude of the given vector.
		/// </summary>
		public static double ExpNatMagnitude(Vector4Int vec)
		{
			return (double)Math.Pow(NatMagnitude(vec), NatMagnitude(vec));
		}

		/// <summary>
		///     Returns the exponentiated natural magnitude of this vector.
		/// </summary>
		public double ExpNatMagnitude()
		{
			return (double)Math.Pow(NatMagnitude(this), NatMagnitude(this));
		}

		/// <summary>
		///     Returns the exponentiated magnitude raised to the power of pi of the given vector.
		/// </summary>
		public static double ExpPiMagnitude(Vector4Int vec)
		{
			return (double)Math.Pow(PiMagnitude(vec), PiMagnitude(vec));
		}

		/// <summary>
		///     Returns the exponentiated magnitude raised to the power of pi of this vector.
		/// </summary>
		public double ExpPiMagnitude()
		{
			return (double)Math.Pow(PiMagnitude(this), PiMagnitude(this));
		}

		/// <summary>
		///     Returns the exponentiated magnitude raised to the power of phi of the given vector.
		/// </summary>
		public static double ExpPhiMagnitude(Vector4Int vec)
		{
			return (double)Math.Pow(PhiMagnitude(vec), PhiMagnitude(vec));
		}

		/// <summary>
		///     Returns the exponentiated magnitude raised to the power of phi of this vector.
		/// </summary>
		public double ExpPhiMagnitude()
		{
			return (double)Math.Pow(PhiMagnitude(this), PhiMagnitude(this));
		}

		/// <summary>
		///     Returns the exponentiated magnitude raised to the given integer of 'power' of the given vector.
		/// </summary>
		public static double ExpPowMagnitude(Vector4Int vec, int power)
		{
			return (double)Math.Pow(PowMagnitude(vec, power), PowMagnitude(vec, power));
		}

		/// <summary>
		///     Returns the exponentiated magnitude raised to the given integer of 'power' of this vector.
		/// </summary>
		public double ExpPowMagnitude(int power)
		{
			return (double)Math.Pow(PowMagnitude(this, power), PowMagnitude(this, power));
		}

		/// <summary>
		///     Returns the exponentiated magnitude raised to the given floating-point of 'power' of the given vector.
		/// </summary>
		public static double ExpPowMagnitude(Vector4Int vec, float power)
		{
			return (double)Math.Pow(PowMagnitude(vec, power), PowMagnitude(vec, power));
		}

		/// <summary>
		///     Returns the exponentiated magnitude raised to the given floating-point of 'power' of this vector.
		/// </summary>
		public double ExpPowMagnitude(float power)
		{
			return (double)Math.Pow(PowMagnitude(this, power), PowMagnitude(this, power));
		}

		/// <summary>
		///     Returns the exponentiated magnitude raised to the given double of 'power' of the given vector.
		/// </summary>
		public static double ExpPowMagnitude(Vector4Int vec, double power)
		{
			return (double)Math.Pow(PowMagnitude(vec, power), PowMagnitude(vec, power));
		}

		/// <summary>
		///     Returns the exponentiated magnitude raised to the given double of 'power' of this vector.
		/// </summary>
		public double ExpPowMagnitude(double power)
		{
			return (double)Math.Pow(PowMagnitude(this, power), PowMagnitude(this, power));
		}

		/// <summary>
		///     Returns a copy of this vector which has a magnitude of 1.
		/// </summary>
		public static Vector4Int Normalize(Vector4Int vec)
		{
			int nx = vec.x;
			int ny = vec.y;
			int nz = vec.z;
			int nw = vec.w;

			double mag = Magnitude(vec);
			if (mag > 0f)
			{
				double invMag = 1.0 / mag;
				nx = Mathf.RoundToInt((float)(vec.x * invMag));
				ny = Mathf.RoundToInt((float)(vec.y * invMag));
				nz = Mathf.RoundToInt((float)(vec.z * invMag));
				nw = Mathf.RoundToInt((float)(vec.w * invMag));
			}

			return new Vector4Int(nx, ny, nz, nw);
		}

		/// <summary>
		///     Makes the given vector have a magnitude of 1.
		/// </summary>
		public void Normalize()
		{
			double mag = Magnitude(this);
			if (mag > 0f)
			{
				double invMag = 1.0 / mag;
				x = Mathf.RoundToInt((float)(x * invMag));
				y = Mathf.RoundToInt((float)(y * invMag));
				z = Mathf.RoundToInt((float)(z * invMag));
				w = Mathf.RoundToInt((float)(w * invMag));
			}
		}

		/// <summary>
		///     Orthogonalizes and normalizes a copy of each of the given normal and tangent vectors.
		///     <br>Returns the respective ortho-normalized vector based on the value of 'returnSwitch'.</br>
		///     <para>Return paths of 'returnSwitch':
		///     <br>- Returns the ortho-normalized vector of 'normal' if false</br>
		///     <br>- Returns the ortho-normalized vector of 'tangent' if true</br></para>
		/// </summary>
		public static Vector4Int OrthoNormalize(Vector4Int normal, Vector4Int tangent, bool returnSwitch)
		{
			Vector4Int normalizedNormal = Normalize(normal);
			Vector4Int projectedTangent = tangent - Dot(normalizedNormal, tangent) * normalizedNormal;
			Vector4Int normalizedTangent = Normalize(projectedTangent);

			return returnSwitch ? normalizedTangent : normalizedNormal;
		}

		/// <summary>
		///     Orthogonalizes and normalizes a copy of each of the given normal, tangent and binormal vectors.
		///     <br>Returns both ortho-normalized vectors in one tuple.</br>
		/// </summary>
		public static (Vector4Int orthonormalizedNormal, Vector4Int orthonormalizedTangent) OrthoNormalize(
			Vector4Int normal, Vector4Int tangent)
		{
			Vector4Int normalizedNormal = Normalize(normal);
			Vector4Int projectedTangent = tangent - Dot(normalizedNormal, tangent) * normalizedNormal;
			Vector4Int normalizedTangent = Normalize(projectedTangent);

			return (normalizedNormal, normalizedTangent);
		}

		/// <summary>
		///     Orthogonalizes and normalizes a copy of each of the given normal, tangent and binormal vectors.
		///     <br>Returns the respective ortho-normalized vector based on the value of 'returnSwitch'.</br>
		///     <para>Return paths of 'returnSwitch':
		///     <br>- Returns the ortho-normalized vector of 'normal' if false</br>
		///     <br>- Returns the ortho-normalized vector of 'tangent' if true</br>
		///     <br>- Returns the ortho-normalized vector of 'binormal' if null</br></para>
		/// </summary>
		public static Vector4Int OrthoNormalize(Vector4Int normal, Vector4Int tangent, Vector4Int binormal,
			bool? returnSwitch)
		{
			Vector4Int normalizedNormal = Normalize(normal);
			Vector4Int projectedTangent = tangent - Dot(normalizedNormal, tangent) * normalizedNormal;
			Vector4Int normalizedTangent = Normalize(projectedTangent);

			Vector4Int projectedBinormal = binormal - Dot(normalizedNormal, binormal) * normalizedNormal
			                                        - Dot(normalizedTangent, binormal) * normalizedTangent;
			Vector4Int normalizedBinormal = Normalize(projectedBinormal);

			if (returnSwitch == true)
				return normalizedTangent;
			else if (returnSwitch == false)
				return normalizedNormal;
			else
				return normalizedBinormal;
		}

		/// <summary>
		///     Orthogonalizes and normalizes a copy of each of the given normal, tangent and binormal vectors.
		///     <br>Returns all the ortho-normalized vectors in one tuple.</br>
		/// </summary>
		public static (Vector4Int orthonormalizedNormal, Vector4Int orthonormalizedTangent, Vector4Int
			orthonormalizedBinormal) OrthoNormalize(Vector4Int normal, Vector4Int tangent, Vector4Int binormal)
		{
			Vector4Int normalizedNormal = Normalize(normal);
			Vector4Int projectedTangent = tangent - Dot(normalizedNormal, tangent) * normalizedNormal;
			Vector4Int normalizedTangent = Normalize(projectedTangent);

			Vector4Int projectedBinormal = binormal - Dot(normalizedNormal, binormal) * normalizedNormal
			                                        - Dot(normalizedTangent, binormal) * normalizedTangent;
			Vector4Int normalizedBinormal = Normalize(projectedBinormal);

			return (normalizedNormal, normalizedTangent, normalizedBinormal);
		}

		/// <summary>
		///     Returns a copy of the given vector with every component clamped to the components of min and max.
		/// </summary>
		public static Vector4Int Clamp(Vector4Int vec, Vector4Int min, Vector4Int max)
		{
			int cx = Mathf.Clamp(vec.x, min.x, max.x);
			int cy = Mathf.Clamp(vec.y, min.y, max.y);
			int cz = Mathf.Clamp(vec.z, min.z, max.z);
			int cw = Mathf.Clamp(vec.w, min.w, max.w);
			return new Vector4Int(cx, cy, cz, cw);
		}

		/// <summary>
		///     Clamps every component of this vector to the components of min and max.
		/// </summary>
		public void Clamp(Vector4Int min, Vector4Int max)
		{
			x = Mathf.Clamp(x, min.x, max.x);
			y = Mathf.Clamp(y, min.y, max.y);
			z = Mathf.Clamp(z, min.z, max.z);
			w = Mathf.Clamp(w, min.w, max.w);
		}

		/// <summary>
		///     Returns a copy of the given vector with every component clamped to the integers of min and max.
		/// </summary>
		public static Vector4Int Clamp(Vector4Int vec, int min, int max)
		{
			int cx = Mathf.Clamp(vec.x, min, max);
			int cy = Mathf.Clamp(vec.y, min, max);
			int cz = Mathf.Clamp(vec.z, min, max);
			int cw = Mathf.Clamp(vec.w, min, max);
			return new Vector4Int(cx, cy, cz, cw);
		}

		/// <summary>
		///     Clamps every component of this vector to the integers of min and max.
		/// </summary>
		public void Clamp(int min, int max)
		{
			x = Mathf.Clamp(x, min, max);
			y = Mathf.Clamp(y, min, max);
			z = Mathf.Clamp(z, min, max);
			w = Mathf.Clamp(w, min, max);
		}

		/// <summary>
		///     Returns a copy of the given vector with every component clamped to [0,1].
		/// </summary>
		public static Vector4Int Clamp01(Vector4Int vec)
		{
			int cx = (int)Mathf.Clamp01(vec.x);
			int cy = (int)Mathf.Clamp01(vec.y);
			int cz = (int)Mathf.Clamp01(vec.z);
			int cw = (int)Mathf.Clamp01(vec.w);
			return new Vector4Int(cx, cy, cz, cw);
		}

		/// <summary>
		///     Clamps every component of this vector to [0,1].
		/// </summary>
		public void Clamp01()
		{
			x = (int)Mathf.Clamp01(x);
			y = (int)Mathf.Clamp01(y);
			z = (int)Mathf.Clamp01(z);
			w = (int)Mathf.Clamp01(w);
		}

		/// <summary>
		///     Returns a copy of the given vector with every component clamped to [-1,1].
		/// </summary>
		public static Vector4Int ClampUnit(Vector4Int vec)
		{
			int cx = Mathf.Clamp(vec.x, -1, 1);
			int cy = Mathf.Clamp(vec.y, -1, 1);
			int cz = Mathf.Clamp(vec.z, -1, 1);
			int cw = Mathf.Clamp(vec.w, -1, 1);
			return new Vector4Int(cx, cy, cz, cw);
		}

		/// <summary>
		///     Clamps every component of this vector to [-1,1].
		/// </summary>
		public void ClampUnit()
		{
			x = Mathf.Clamp(x, -1, 1);
			y = Mathf.Clamp(y, -1, 1);
			z = Mathf.Clamp(z, -1, 1);
			w = Mathf.Clamp(w, -1, 1);
		}

		/// <summary>
		///     Returns a copy of the given vector with its magnitude clamped to 'maxLength'.
		/// </summary>
		public static Vector4Int ClampMagnitude(Vector4Int vec, int maxLength)
		{
			int sqrMagnitude = vec.x * vec.x + vec.y * vec.y + vec.z * vec.z + vec.w * vec.w;
			if (sqrMagnitude > maxLength * maxLength)
			{
				double magnitude = Mathf.Sqrt(sqrMagnitude);
				double normalizedX = vec.x / magnitude;
				double normalizedY = vec.y / magnitude;
				double normalizedZ = vec.z / magnitude;
				double normalizedW = vec.w / magnitude;
				return new Vector4Int(
					Mathf.RoundToInt((float)(normalizedX * maxLength)),
					Mathf.RoundToInt((float)(normalizedY * maxLength)),
					Mathf.RoundToInt((float)(normalizedZ * maxLength)),
					Mathf.RoundToInt((float)(normalizedW * maxLength))
				);
			}

			return vec;
		}

		/// <summary>
		///     Clamps the magnitude of this vector to 'maxLength'.
		/// </summary>
		public void ClampMagnitude(int maxLength)
		{
			int sqrMagnitude = x * x + y * y + z * z + w * w;
			if (sqrMagnitude > maxLength * maxLength)
			{
				double magnitude = Mathf.Sqrt(sqrMagnitude);
				double normalizedX = x / magnitude;
				double normalizedY = y / magnitude;
				double normalizedZ = z / magnitude;
				double normalizedW = w / magnitude;
				x = Mathf.RoundToInt((float)(normalizedX * maxLength));
				y = Mathf.RoundToInt((float)(normalizedY * maxLength));
				z = Mathf.RoundToInt((float)(normalizedZ * maxLength));
				w = Mathf.RoundToInt((float)(normalizedW * maxLength));
			}
		}

		/// <summary>
		///     Returns a copy of the given vector with the sign values of every component.
		/// </summary>
		public static Vector4Int Sign(Vector4Int vec)
		{
			int cx = Math.Sign(vec.x);
			int cy = Math.Sign(vec.y);
			int cz = Math.Sign(vec.z);
			int cw = Math.Sign(vec.w);
			return new Vector4Int(cx, cy, cz, cw);
		}

		/// <summary>
		///     Turns every component of this vector into its sign value.
		/// </summary>
		public void Sign()
		{
			x = Math.Sign(x);
			y = Math.Sign(y);
			z = Math.Sign(z);
			w = Math.Sign(w);
		}

		/// <summary>
		///     Returns a copy of the given vector with the sign values of every component and any zeros returning 1.
		/// </summary>
		public static Vector4Int SignPositiveZero(Vector4Int vec)
		{
			int cx = Math.Sign(vec.x) == 0 ? 1 : Math.Sign(vec.x);
			int cy = Math.Sign(vec.y) == 0 ? 1 : Math.Sign(vec.y);
			int cz = Math.Sign(vec.z) == 0 ? 1 : Math.Sign(vec.z);
			int cw = Math.Sign(vec.w) == 0 ? 1 : Math.Sign(vec.w);
			return new Vector4Int(cx, cy, cz, cw);
		}

		/// <summary>
		///     Turns every component of this vector into its sign value with any zeros returning 1.
		/// </summary>
		public void SignPositiveZero()
		{
			x = Math.Sign(x) == 0 ? 1 : Math.Sign(x);
			y = Math.Sign(y) == 0 ? 1 : Math.Sign(y);
			z = Math.Sign(z) == 0 ? 1 : Math.Sign(z);
			w = Math.Sign(w) == 0 ? 1 : Math.Sign(w);
		}

		/// <summary>
		///     Returns a copy of the given vector with the sign values of every component and any zeros returning -1.
		/// </summary>
		public static Vector4Int SignNegativeZero(Vector4Int vec)
		{
			int cx = Math.Sign(vec.x) == 0 ? -1 : Math.Sign(vec.x);
			int cy = Math.Sign(vec.y) == 0 ? -1 : Math.Sign(vec.y);
			int cz = Math.Sign(vec.z) == 0 ? -1 : Math.Sign(vec.z);
			int cw = Math.Sign(vec.w) == 0 ? -1 : Math.Sign(vec.w);
			return new Vector4Int(cx, cy, cz, cw);
		}

		/// <summary>
		///     Turns every component of this vector into its sign value with any zeros returning -1.
		/// </summary>
		public void SignNegativeZero()
		{
			x = Math.Sign(x) == 0 ? -1 : Math.Sign(x);
			y = Math.Sign(y) == 0 ? -1 : Math.Sign(y);
			z = Math.Sign(z) == 0 ? -1 : Math.Sign(z);
			w = Math.Sign(w) == 0 ? -1 : Math.Sign(w);
		}

		/// <summary>
		///     Calculates the linear parameter t that produces the given interpolant vector of 'vec' within the range [a,b].
		/// </summary>
		public static float InverseLerp(Vector4Int a, Vector4Int b, Vector4Int vec)
		{
			if (a != b)
			{
				Vector4Int delta = b - a;
				float t = Mathf.Clamp01(Dot(vec - a, delta) / Dot(delta, delta));

				// Return t if the given interpolant vector of 'vec' actually falls on the line produced by the vectors 'a' and 'b', otherwise throw an ArgumentException.
				if (LerpUnclamped(a, b, t) == vec)
					return t;
				else
					throw new ArgumentException(
						"The given interpolant vector of 'vec' does not fall on the line produced by the vectors 'a' and 'b'.",
						nameof(vec));
			}
			else return 0f;
		}

		/// <summary>
		///     Calculates the linear parameter t that produces the given interpolant vector of 'vec' within the range [a,b].
		///     <para>Returns the calculated value for t, even if the vector of 'vec' does not fall on the line produced by the vectors 'a' and 'b'.</para>
		/// </summary>
		public static float InverseLerpUnbounded(Vector4Int a, Vector4Int b, Vector4Int vec)
		{
			if (a != b)
			{
				Vector4Int delta = b - a;
				float t = Mathf.Clamp01(Dot(vec - a, delta) / Dot(delta, delta));

				return t;
			}
			else return 0f;
		}

		/// <summary>
		///     Linearly interpolates between two vectors where t is clamped to [0,1].
		/// </summary>
		public static Vector4Int Lerp(Vector4Int a, Vector4Int b, float t)
		{
			t = Mathf.Clamp01(t);
			int x = Mathf.RoundToInt(Mathf.Lerp(a.x, b.x, t));
			int y = Mathf.RoundToInt(Mathf.Lerp(a.y, b.y, t));
			int z = Mathf.RoundToInt(Mathf.Lerp(a.z, b.z, t));
			int w = Mathf.RoundToInt(Mathf.Lerp(a.w, b.w, t));
			return new Vector4Int(x, y, z, w);
		}

		/// <summary>
		///     Linearly interpolates along two vectors.
		/// </summary>
		public static Vector4Int LerpUnclamped(Vector4Int a, Vector4Int b, float t)
		{
			int x = Mathf.RoundToInt(Mathf.Lerp(a.x, b.x, t));
			int y = Mathf.RoundToInt(Mathf.Lerp(a.y, b.y, t));
			int z = Mathf.RoundToInt(Mathf.Lerp(a.z, b.z, t));
			int w = Mathf.RoundToInt(Mathf.Lerp(a.w, b.w, t));
			return new Vector4Int(x, y, z, w);
		}

		/// <summary>
		///     Hyperspherically interpolates between two vectors where t is clamped to [0,1].
		/// </summary>
		public static Vector4Int Hyperslerp(Vector4Int a, Vector4Int b, float t)
		{
			t = Mathf.Clamp01(t);

			double num = Math.Acos(0);
			double num2 = 1.0 / Math.Sin(num);
			double num3;
			double num4;
			Vector4Int result = default;

			num3 = Math.Sin((1.0 - t) * num) * num2;
			num4 = Math.Sin(t * num) * num2;

			result.x = (int)Math.Round(num3 * a.x + num4 * b.x, 0);
			result.y = (int)Math.Round(num3 * a.y + num4 * b.y, 0);
			result.z = (int)Math.Round(num3 * a.z + num4 * b.z, 0);
			result.w = (int)Math.Round(num3 * a.w + num4 * b.w, 0);

			return result;
		}

		/// <summary>
		///     Hyperspherically interpolates between two vectors where t is clamped to [0,1].
		///     <br>If 'reverseDir' is set to true, the function will perform hyperspherical interpolation in the opposite direction.</br>
		/// </summary>
		public static Vector4Int Hyperslerp(Vector4Int a, Vector4Int b, float t, bool reverseDir)
		{
			t = Mathf.Clamp01(t);

			double num = Math.Acos(0);
			double num2 = 1.0 / Math.Sin(num);
			double num3;
			double num4;
			Vector4Int result = default;

			if (reverseDir)
			{
				num3 = Math.Sin((1.0 - 3.0 * t) * num) * num2;
				num4 = Math.Sin(3.0 * t * num) * num2;

				result.x = (int)Math.Round(num3 * a.x - num4 * b.x, 0);
				result.y = (int)Math.Round(num3 * a.y - num4 * b.y, 0);
				result.z = (int)Math.Round(num3 * a.z - num4 * b.z, 0);
				result.w = (int)Math.Round(num3 * a.w - num4 * b.w, 0);
			}
			else
			{
				num3 = Math.Sin((1.0 - t) * num) * num2;
				num4 = Math.Sin(t * num) * num2;

				result.x = (int)Math.Round(num3 * a.x + num4 * b.x, 0);
				result.y = (int)Math.Round(num3 * a.y + num4 * b.y, 0);
				result.z = (int)Math.Round(num3 * a.z + num4 * b.z, 0);
				result.w = (int)Math.Round(num3 * a.w + num4 * b.w, 0);
			}

			return result;
		}

		/// <summary>
		///     Hyperspherically interpolates along two vectors.
		/// </summary>
		public static Vector4Int HyperslerpUnclamped(Vector4Int a, Vector4Int b, float t)
		{
			double num = Math.Acos(0);
			double num2 = 1.0 / Math.Sin(num);
			double num3;
			double num4;
			Vector4Int result = default;

			num3 = Math.Sin((1.0 - t) * num) * num2;
			num4 = Math.Sin(t * num) * num2;

			result.x = (int)Math.Round(num3 * a.x + num4 * b.x, 0);
			result.y = (int)Math.Round(num3 * a.y + num4 * b.y, 0);
			result.z = (int)Math.Round(num3 * a.z + num4 * b.z, 0);
			result.w = (int)Math.Round(num3 * a.w + num4 * b.w, 0);

			return result;
		}

		/// <summary>
		///     Hyperspherically interpolates along two vectors.
		///     <br>If 'reverseDir' is set to true, the function will perform hyperspherical interpolation in the opposite direction.</br>
		/// </summary>
		public static Vector4Int HyperslerpUnclamped(Vector4Int a, Vector4Int b, float t, bool reverseDir)
		{
			double num = Math.Acos(0);
			double num2 = 1.0 / Math.Sin(num);
			double num3;
			double num4;

			Vector4Int result = default;

			if (reverseDir)
			{
				num3 = Math.Sin((1.0 - 3.0 * t) * num) * num2;
				num4 = Math.Sin(3.0 * t * num) * num2;

				result.x = (int)Math.Round(num3 * a.x - num4 * b.x, 0);
				result.y = (int)Math.Round(num3 * a.y - num4 * b.y, 0);
				result.z = (int)Math.Round(num3 * a.z - num4 * b.z, 0);
				result.w = (int)Math.Round(num3 * a.w - num4 * b.w, 0);
			}
			else
			{
				num3 = Math.Sin((1.0 - t) * num) * num2;
				num4 = Math.Sin(t * num) * num2;

				result.x = (int)Math.Round(num3 * a.x + num4 * b.x, 0);
				result.y = (int)Math.Round(num3 * a.y + num4 * b.y, 0);
				result.z = (int)Math.Round(num3 * a.z + num4 * b.z, 0);
				result.w = (int)Math.Round(num3 * a.w + num4 * b.w, 0);
			}

			return result;
		}

		/// <summary>
		///     Hyperspherically interpolates around the vector 'c' between two vectors where t is clamped to [0,1].
		/// </summary>
		public static Vector4Int HyperslerpCenter(Vector4Int a, Vector4Int b, Vector4Int c, float t)
		{
			t = Mathf.Clamp01(t);

			double num = Math.Acos(0);
			double num2 = 1.0 / Math.Sin(num);
			double num3;
			double num4;

			Vector4Int result = default;

			num3 = Math.Sin((1.0 - t) * num) * num2;
			num4 = Math.Sin(t * num) * num2;

			result.x = (int)Math.Round(num3 * a.x + num4 * b.x, 0);
			result.y = (int)Math.Round(num3 * a.y + num4 * b.y, 0);
			result.z = (int)Math.Round(num3 * a.z + num4 * b.z, 0);
			result.w = (int)Math.Round(num3 * a.w + num4 * b.w, 0);

			return result + c;
		}

		/// <summary>
		///     Hyperspherically interpolates around the vector 'c' between two vectors where t is clamped to [0,1].
		///     <br>If 'reverseDir' is set to true, the function will perform hyperspherical interpolation in the opposite direction.</br>
		/// </summary>
		public static Vector4Int HyperslerpCenter(Vector4Int a, Vector4Int b, Vector4Int c, float t, bool reverseDir)
		{
			t = Mathf.Clamp01(t);

			double num = Math.Acos(0);
			double num2 = 1.0 / Math.Sin(num);
			double num3;
			double num4;

			Vector4Int result = default;

			if (reverseDir)
			{
				num3 = Math.Sin((1.0 - 3.0 * t) * num) * num2;
				num4 = Math.Sin(3.0 * t * num) * num2;

				result.x = (int)Math.Round(num3 * a.x - num4 * b.x, 0);
				result.y = (int)Math.Round(num3 * a.y - num4 * b.y, 0);
				result.z = (int)Math.Round(num3 * a.z - num4 * b.z, 0);
				result.w = (int)Math.Round(num3 * a.w - num4 * b.w, 0);
			}
			else
			{
				num3 = Math.Sin((1.0 - t) * num) * num2;
				num4 = Math.Sin(t * num) * num2;

				result.x = (int)Math.Round(num3 * a.x + num4 * b.x, 0);
				result.y = (int)Math.Round(num3 * a.y + num4 * b.y, 0);
				result.z = (int)Math.Round(num3 * a.z + num4 * b.z, 0);
				result.w = (int)Math.Round(num3 * a.w + num4 * b.w, 0);
			}

			return result + c;
		}

		/// <summary>
		///     Hyperspherically interpolates around the vector 'c' along two vectors.
		/// </summary>
		public static Vector4Int HyperslerpCenterUnclamped(Vector4Int a, Vector4Int b, Vector4Int c, float t)
		{
			double num = Math.Acos(0);
			double num2 = 1.0 / Math.Sin(num);
			double num3;
			double num4;

			Vector4Int result = default;

			num3 = Math.Sin((1.0 - t) * num) * num2;
			num4 = Math.Sin(t * num) * num2;

			result.x = (int)Math.Round(num3 * a.x + num4 * b.x, 0);
			result.y = (int)Math.Round(num3 * a.y + num4 * b.y, 0);
			result.z = (int)Math.Round(num3 * a.z + num4 * b.z, 0);
			result.w = (int)Math.Round(num3 * a.w + num4 * b.w, 0);

			return result + c;
		}

		/// <summary>
		///     Hyperspherically interpolates around the vector 'c' along two vectors.
		///     <br>If 'reverseDir' is set to true, the function will perform hyperspherical interpolation in the opposite direction.</br>
		/// </summary>
		public static Vector4Int HyperslerpCenterUnclamped(Vector4Int a, Vector4Int b, Vector4Int c, float t,
			bool reverseDir)
		{
			double num = Math.Acos(0);
			double num2 = 1.0 / Math.Sin(num);
			double num3;
			double num4;

			Vector4Int result = default;

			if (reverseDir)
			{
				num3 = Math.Sin((1.0 - 3.0 * t) * num) * num2;
				num4 = Math.Sin(3.0 * t * num) * num2;

				result.x = (int)Math.Round(num3 * a.x - num4 * b.x, 0);
				result.y = (int)Math.Round(num3 * a.y - num4 * b.y, 0);
				result.z = (int)Math.Round(num3 * a.z - num4 * b.z, 0);
				result.w = (int)Math.Round(num3 * a.w - num4 * b.w, 0);
			}
			else
			{
				num3 = Math.Sin((1.0 - t) * num) * num2;
				num4 = Math.Sin(t * num) * num2;

				result.x = (int)Math.Round(num3 * a.x + num4 * b.x, 0);
				result.y = (int)Math.Round(num3 * a.y + num4 * b.y, 0);
				result.z = (int)Math.Round(num3 * a.z + num4 * b.z, 0);
				result.w = (int)Math.Round(num3 * a.w + num4 * b.w, 0);
			}

			return result + c;
		}

		/// <summary>
		///     Returns a vector with the components of the given input vector around the integer values of min and max.
		/// </summary>
		public static Vector4Int Wrap(Vector4Int input, int min, int max)
		{
			Vector4Int minVec = new Vector4Int(min, min, min, min);
			Vector4Int maxVec = new Vector4Int(max, max, max, max);

			if (input < minVec)
				return maxVec - (minVec - input) % (maxVec - minVec);
			else return minVec + (input - minVec) % (maxVec - minVec);

		}

		/// <summary>
		///     Wraps this vector's components around the integer values of min and max.
		/// </summary>
		public Vector4Int Wrap(int min, int max)
		{
			Vector4Int minVec = new Vector4Int(min, min, min, min);
			Vector4Int maxVec = new Vector4Int(max, max, max, max);

			if (this < minVec)
				this = maxVec - (minVec - this) % (maxVec - minVec);
			else this = minVec + (this - minVec) % (maxVec - minVec);

			return this;
		}

		/// <summary>
		///     Returns a vector with the components of the given input vector around the components of the min and max vectors.
		/// </summary>
		public static Vector4Int Wrap(Vector4Int input, Vector4Int min, Vector4Int max)
		{
			if (input < min)
				return max - (min - input) % (max - min);
			else return min + (input - min) % (max - min);
		}

		/// <summary>
		///     Wraps this vector's components around the components of the min and max vectors.
		/// </summary>
		public Vector4Int Wrap(Vector4Int min, Vector4Int max)
		{
			if (this < min)
				this = max - (min - this) % (max - min);
			else this = min + (this - min) % (max - min);

			return this;
		}

		/// <summary>
		///     Returns true if this vector has the given integer value in any component, otherwise returns false.
		/// </summary>
		public bool Contains(int val)
		{
			return x == val || y == val || z == val || w == val;
		}

		/// <summary>
		///     Returns true if this vector has any of the given integer values in any component, otherwise returns false.
		/// </summary>
		public bool ContainsOr(params int[] vals)
		{
			if (vals.Length < 2)
			{
				throw new ArgumentException("Vector4Int.ContainsOr requires at least two given values.", nameof(vals));
			}

			foreach (int val in vals)
			{
				if (x == val || y == val || z == val || w == val)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		///     Returns true if this vector has exactly one of the given integer values in any component, otherwise returns false.
		/// </summary>
		public bool ContainsXor(params int[] vals)
		{
			if (vals.Length < 2)
			{
				throw new ArgumentException("Vector4Int.ContainsXor requires at least two given values.", nameof(vals));
			}

			int trueCount = 0;
			foreach (int val in vals)
			{
				if (x == val || y == val || z == val || w == val)
				{
					trueCount++;
				}
			}

			return trueCount == 1;
		}

		/// <summary>
		///     Returns true if this vector has all of the given integer values in any component, otherwise returns false.
		/// </summary>
		public bool ContainsAnd(params int[] vals)
		{
			if (vals.Length < 2)
			{
				throw new ArgumentException("Vector4Int.ContainsAnd requires at least two given values.", nameof(vals));
			}
			else if (vals.Length > 4)
			{
				throw new ArgumentException("Vector4Int.ContainsAnd cannot have more than four given values.",
					nameof(vals));
			}

			foreach (int val in vals)
			{
				if (!(x == val || y == val || z == val || w == val))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		///     Returns true if this vector has a value less than the given integer value in any component, otherwise returns false.
		/// </summary>
		public bool ContainsLessThan(int val)
		{
			return x < val || y < val || z < val || w < val;
		}

		/// <summary>
		///     Returns true if this vector has a value greater than the given integer value in any component, otherwise returns false.
		/// </summary>
		public bool ContainsGreaterThan(int val)
		{
			return x > val || y > val || z > val || w > val;
		}

		/// <summary>
		///     Returns true if this vector has a value less than or equal to the given integer value in any component, otherwise returns false.
		/// </summary>
		public bool ContainsLessThanEqual(int val)
		{
			return x <= val || y <= val || z <= val || w <= val;
		}

		/// <summary>
		///     Returns true if this vector has a value greater than or equal to the given integer value in any component, otherwise returns false.
		/// </summary>
		public bool ContainsGreaterThanEqual(int val)
		{
			return x >= val || y >= val || z >= val || w >= val;
		}

		/// <summary>
		///     Returns true if this vector does not have the given integer value in any component, otherwise returns false.
		/// </summary>
		public bool DoesNotContain(int val)
		{
			return !(x == val || y == val || z == val || w == val);
		}

		/// <summary>
		///     Returns true if this vector does not have any of the given integer values in any component, otherwise returns false.
		/// </summary>
		public bool DoesNotContainOr(params int[] vals)
		{
			if (vals.Length < 2)
			{
				throw new ArgumentException("Vector4Int.DoesNotContainOr requires at least two given values.",
					nameof(vals));
			}

			foreach (int val in vals)
			{
				if (!(x == val || y == val || z == val || w == val))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		///     Returns true if this vector does not have exactly one of the given integer values in any component, otherwise returns false.
		/// </summary>
		public bool DoesNotContainXor(params int[] vals)
		{
			if (vals.Length < 2)
			{
				throw new ArgumentException("Vector4Int.DoesNotContainXor requires at least two given values.",
					nameof(vals));
			}

			int trueCount = 0;
			foreach (int val in vals)
			{
				if (x == val || y == val || z == val || w == val)
				{
					trueCount++;
				}
			}

			return trueCount != 1;
		}

		/// <summary>
		///     Returns true if this vector does not have all of the given integer values in any component, otherwise returns false.
		/// </summary>
		public bool DoesNotContainAnd(params int[] vals)
		{
			if (vals.Length < 2)
			{
				throw new ArgumentException("Vector4Int.DoesNotContainAnd requires at least two given values.",
					nameof(vals));
			}
			else if (vals.Length > 4)
			{
				throw new ArgumentException("Vector4Int.DoesNotContainAnd cannot have more than four given values.",
					nameof(vals));
			}

			foreach (int val in vals)
			{
				if (x == val || y == val || z == val || w == val)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		///     Copies this vector's components and returns them in the form of an integer array of length 4.
		/// </summary>
		public int[] ToIntArray()
		{
			return new int[4] { x, y, z, w };
		}

		/// <summary>
		///     Copies this vector's components and returns them in the form of an integer list.
		/// </summary>
		public List<int> ToIntList()
		{
			return new List<int>() { x, y, z, w };
		}

		/// <summary>
		///     Copies this vector's components and returns them in the form of an integer list.
		///     <br>If 'setCapacity' is set to true, the resulting integer list will have an automatically determined capacity.</br>
		/// </summary>
		public List<int> ToIntList(bool setCapacity)
		{
			if (setCapacity)
				return new List<int>(4) { x, y, z, w };
			else
				return new List<int>() { x, y, z, w };
		}

		/// <summary>
		///     Copies this vector's components and returns them in the form of an integer list with the given capacity.
		/// </summary>
		public List<int> ToIntList(int capacity)
		{
			return new List<int>(capacity) { x, y, z, w };
		}

		// Interface constructors
		/// <summary>
		///     Constructs a Vector4Int using serialization info.
		/// </summary>
		public Vector4Int(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
				throw new ArgumentNullException(nameof(info), "The given serialization info may not be null.");

			m_X = (int)info.GetValue("x", typeof(int));
			m_Y = (int)info.GetValue("y", typeof(int));
			m_Z = (int)info.GetValue("z", typeof(int));
			m_W = (int)info.GetValue("w", typeof(int));
			m_Origin = (m_X, m_Y, m_Z, m_W);
		}

		// Interface methods
		/// <summary>
		///     Returns a formatted string for this vector.
		/// </summary>
		public override string ToString()
		{
			return ToString(null, null);
		}

		/// <summary>
		///     Returns a formatted string for this vector with the given numeric format string.
		/// </summary>
		public string ToString(string format)
		{
			return ToString(format, null);
		}

		/// <summary>
		///     Returns a formatted string for this vector with the given numeric format string and the given culture-specific formatting object.
		/// </summary>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (string.IsNullOrEmpty(format))
			{
				format = "F0";
			}

			// Assign 'formatProvider' if it is null.
			formatProvider ??= CultureInfo.InvariantCulture.NumberFormat;

			return
				$"({(object)this.m_X.ToString(format, formatProvider)}, {(object)this.m_Y.ToString(format, formatProvider)}, {(object)this.m_Z.ToString(format, formatProvider)})";
		}

		/// <summary>
		///     Returns true if this vector and the given object are equal.
		/// </summary>
		public override bool Equals(object other)
		{
			if (!(other is Vector4Int) && !(other is int))
			{
				return false;
			}

			if (other is Vector4Int vector)
				return Equals(vector);
			else
				return Equals((int)other);
		}

		/// <summary>
		///     Returns true if this vector and the given vector are equal.
		/// </summary>
		public bool Equals(Vector4Int other)
		{
			return this == other;
		}

		/// <summary>
		///     Returns true if every component of this vector and the given integer are equal.
		/// </summary>
		public bool Equals(int other)
		{
			return x == other && y == other && z == other && w == other;
		}

		/// <summary>
		///     Gets the hash code for this vector.
		/// </summary>
		public override int GetHashCode()
		{
			int hashCode = y.GetHashCode();
			int hashCode2 = z.GetHashCode();
			int hashCode3 = w.GetHashCode();
			return x.GetHashCode() ^ (hashCode << 2) ^ (hashCode >> 30) ^ (hashCode2 << 4) ^ (hashCode2 >> 28) ^
			       (hashCode3 << 6) ^ (hashCode3 >> 26);
		}

		/// <summary>
		///     Compares the components of this vector to the given object and returns an indicator on the relative order of their values.
		/// </summary>
		public int CompareTo(object other)
		{
			// Check if the value of other is null and, if it is, return 1.
			if (other == null)
				return 1;

			// Check if the value of other is of an accepted IComparable interface type.
			if (other is Vector4Int vector)
				return CompareTo(vector);
			else if (other is int integer)
				return CompareTo(integer);

			// Throw an exception if it isn't of an accepted IComparable interface type.
			throw new ArgumentException("The given object must be of type 'Vector4Int' or 'int'.", nameof(other));
		}

		/// <summary>
		///     Compares the components of this vector to those of the given vector and returns an indicator on the relative order of their values.
		/// </summary>
		public int CompareTo(Vector4Int other)
		{
			// Compare the value of each component of this vector to the same vector component of 'other'.
			int xComparison = x.CompareTo(other.x);
			int yComparison = y.CompareTo(other.y);
			int zComparison = z.CompareTo(other.z);
			int wComparison = w.CompareTo(other.w);

			// Return the combined result of the 4 comparisons.
			return xComparison + yComparison + zComparison + wComparison;
		}

		/// <summary>
		///     Compares the components of this vector to the given integer and returns an indicator on the relative order of their values.
		/// </summary>
		public int CompareTo(int other)
		{
			// Compare the value of each component of this vector to the integer value of 'other'.
			int xComparison = x.CompareTo(other);
			int yComparison = y.CompareTo(other);
			int zComparison = z.CompareTo(other);
			int wComparison = w.CompareTo(other);

			// Return the combined result of the 4 comparisons.
			return xComparison + yComparison + zComparison + wComparison;
		}

		/// <summary>
		///     Creates a shallow copy of this vector.
		/// </summary>
		public object Clone()
		{
			return MemberwiseClone();
		}

		/// <summary>
		///     Gets the integer value enumerator for this vector.
		/// </summary>
		public IEnumerator GetEnumerator()
		{
			yield return x;
			yield return y;
			yield return z;
			yield return w;
		}

		/// <summary>
		///     Gets the integer value enumerator for this vector.
		/// </summary>
		IEnumerator<int> IEnumerable<int>.GetEnumerator()
		{
			yield return x;
			yield return y;
			yield return z;
			yield return w;
		}

		/// <summary>
		///     Gets the integer value-index enumerator for this vector.
		/// </summary>
		IEnumerator<(int, IEquatable<int>)> IEnumerable<(int, IEquatable<int>)>.GetEnumerator()
		{
			yield return (x, 0);
			yield return (y, 1);
			yield return (z, 2);
			yield return (w, 3);
		}

		/// <summary>
		///     Gets the integer to string value enumerator for this vector.
		/// </summary>
		IEnumerator<string> IEnumerable<string>.GetEnumerator()
		{
			yield return x.ToString();
			yield return y.ToString();
			yield return z.ToString();
			yield return w.ToString();
		}

		/// <summary>
		///     Gets the integer to string value-index enumerator for this vector.
		/// </summary>
		IEnumerator<(string, IEquatable<int>)> IEnumerable<(string, IEquatable<int>)>.GetEnumerator()
		{
			yield return (x.ToString(), 0);
			yield return (y.ToString(), 1);
			yield return (z.ToString(), 2);
			yield return (w.ToString(), 3);
		}

		/// <summary>
		///     Gets the object data for serialization.
		/// </summary>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
				throw new ArgumentNullException(nameof(info), "The given serialization info may not be null.");

			info.AddValue("x", x);
			info.AddValue("y", y);
			info.AddValue("z", z);
			info.AddValue("w", w);
		}

		/// <summary>
		///     Serializes this vector to a byte array.
		/// </summary>
		public byte[] Serialize()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(memoryStream, this);
				return memoryStream.ToArray();
			}
		}

		/// <summary>
		///     Sets this vector by deserializing the given byte array.
		/// </summary>
		public Vector4Int Deserialize(byte[] data)
		{
			using (MemoryStream memoryStream = new MemoryStream(data))
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				var deserializedObj = binaryFormatter.Deserialize(memoryStream);

				if (deserializedObj is Vector4Int vector)
					return vector;
				else
					throw new SerializationException("Cannot deserialize data as it is not of type 'Vector4Int'.");
			}
		}
	}
}