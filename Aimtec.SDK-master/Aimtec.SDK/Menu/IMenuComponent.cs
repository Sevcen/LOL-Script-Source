namespace Aimtec.SDK.Menu
{
    using System.Drawing;

    using Aimtec.SDK.Menu.Theme;
    using Aimtec.SDK.Menu.Components;
    using static Aimtec.SDK.Menu.MenuComponent;
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Interface IMenuComponent
    /// </summary>
    public interface IMenuComponent : IDisposable
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        string DisplayName { get; set; }

        /// <summary>
        ///     Gets or sets the internal name
        /// </summary>
        /// <value>The internal name.</value>
        string InternalName { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="IMenuComponent" /> is enabled.
        /// </summary>
        /// <remarks>
        ///     This property will only succeed if the MenuComponent implements <see cref="IReturns{bool}" />.
        ///     This property will only succeed for <see cref="MenuBool"/>, <see cref="MenuKeyBind"/>, <see cref="MenuSliderBool"/>.
        /// </remarks>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        bool Enabled { get; }

        /// <summary>
        ///     Gets a value associated with this <see cref="IMenuComponent" />
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        int Value { get; }

        /// <summary>
        ///     Gets the parent of this <see cref="IMenuComponent" /> if it has one.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        Menu Parent { get; set; }

        /// <summary>
        ///     Gets the children.
        /// </summary>
        /// <value>The children.</value>
        Dictionary<string, MenuComponent> Children { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Converts the <see cref="IMenuComponent" /> to the specified <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        T As<T>()
            where T : IMenuComponent;

        #endregion

        #region Public Indexers

        /// <summary>
        ///      The indexer to get items contained in this menu
        /// </summary>
        /// <remarks>
        ///     This property will only succeed if this class is type <see cref="Menu"/>".
        /// </remarks>
        IMenuComponent this[string name] { get; }

        #endregion

        #region Public Events

        /// <summary>
        ///    The event fired when the a change occurs in this see cref="IMenuComponent" />
        /// </summary>
        event ValueChangedHandler OnValueChanged;

        #endregion

    }
}