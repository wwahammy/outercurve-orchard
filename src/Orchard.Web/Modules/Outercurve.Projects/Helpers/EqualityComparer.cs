using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Outercurve.Projects.Helpers
{
    /// <summary>
    /// A generic implementation of a EqualityComparer that takes a delegate to compare with
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    /// <remarks>
    /// </remarks>
    /// <from>https://github.com/perpetual-motion/clrplus/blob/1a71d26845c3c2a03af89ffa1d90b3aa7dfb5667/Core/Extensions/Comparer.cs</from>
    public class EqualityComparer<T> : IEqualityComparer<T>
    {
        /// <summary>
        /// </summary>
        private readonly Func<T, T, bool> _equalityCompareFn;

        /// <summary>
        /// </summary>
        private readonly Func<T, int> _getHashCodeFn;

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualityComparer&lt;T&gt;" /> class.
        /// </summary>
        /// <param name="equalityCompareFn"> The equality compare fn. </param>
        /// <param name="getHashCodeFn"> The get hash code fn. </param>
        /// <remarks>
        /// </remarks>
        public EqualityComparer(Func<T, T, bool> equalityCompareFn, Func<T, int> getHashCodeFn)
        {
            _equalityCompareFn = equalityCompareFn;
            _getHashCodeFn = getHashCodeFn;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">
        /// The first object of type <paramref name="x" /> to compare.
        /// </param>
        /// <param name="y">
        /// The second object of type <paramref name="y" /> to compare.
        /// </param>
        /// <returns> true if the specified objects are equal; otherwise, false. </returns>
        /// <remarks>
        /// </remarks>
        public bool Equals(T x, T y)
        {
            return _equalityCompareFn(x, y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="T:System.Object" /> for which a hash code is to be returned.
        /// </param>
        /// <returns> A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The type of
        /// <paramref name="obj" />
        /// is a reference type and
        /// <paramref name="obj" />
        /// is null.
        /// </exception>
        /// <remarks>
        /// </remarks>
        public int GetHashCode(T obj)
        {
            return _getHashCodeFn(obj);
        }
    }
}