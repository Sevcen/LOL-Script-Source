namespace Aimtec.SDK.Menu.Theme
{
    /// <summary>
    ///     Interface IRenderManager
    /// </summary>
    public interface IRenderManager
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Renders at the specified position.
        /// </summary>
        /// <param name="pos">The position.</param>
        void Render(Vector2 pos);

        #endregion
    }

    /// <summary>
    ///     Interface IRenderManager
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Aimtec.SDK.Menu.Theme.IRenderManager" />
    public interface IRenderManager<out T> : IRenderManager
        where T : IMenuComponent
    {
        #region Public Properties

        /// <summary>
        ///     Gets the component.
        /// </summary>
        /// <value>The component.</value>
        T Component { get; }

        #endregion
    }

    /// <summary>
    ///     Interface IRenderManager
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2">The type of the t2.</typeparam>
    /// <seealso cref="Aimtec.SDK.Menu.Theme.IRenderManager{T}" />
    public interface IRenderManager<out T, out T2> : IRenderManager<T>
        where T : IMenuComponent where T2 : MenuTheme
    {
        #region Public Properties

        /// <summary>
        ///     Gets the theme.
        /// </summary>
        /// <value>The theme.</value>
        T2 Theme { get; }

        #endregion
    }
}