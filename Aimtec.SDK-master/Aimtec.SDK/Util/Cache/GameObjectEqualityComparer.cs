namespace Aimtec.SDK.Util.Cache
{
    using System.Collections.Generic;

    /// <summary>
    ///     Class GameObjectEqualityComparer.
    /// </summary>
    /// <seealso cref="GameObject" />
    public class GameObjectEqualityComparer : IEqualityComparer<GameObject>
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <typeparamref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <typeparamref name="T" />paramref name="T" /> to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(GameObject x, GameObject y)
        {
            return x.NetworkId == y.NetworkId;
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public int GetHashCode(GameObject obj)
        {
            return obj.NetworkId;
        }

        #endregion
    }
}