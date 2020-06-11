namespace Aimtec.SDK.Menu
{
    /// <summary>
    ///     Exposes a property of type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReturns<T>
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        T Value { get; set; }

        #endregion
    }
}