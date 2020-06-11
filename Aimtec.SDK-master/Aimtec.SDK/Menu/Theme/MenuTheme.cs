namespace Aimtec.SDK.Menu.Theme
{
    using System.Drawing;

    using Aimtec.SDK.Menu.Components;

    /// <summary>
    ///     Class MenuTheme.
    /// </summary>
    public abstract class MenuTheme
    {
        #region Constructors and Destructors

        public MenuTheme()
        {
            this.BaseMenuWidth = this.IndicatorWidth + this.LineWidth + this.TextSpacing;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the width of the menu.
        /// </summary>
        /// <value>The base width of a Menu</value>
        public virtual int BaseMenuWidth { get; set; }

        public virtual int IndicatorWidth { get; set; } = 35;

        public virtual Color LineColor { get; set; } = Color.FromArgb(82, 83, 57);

        public virtual int LineWidth { get; set; } = 1;

        public virtual Color MenuBoxBackgroundColor { get; set; } = Color.FromArgb(206, 16, 26, 29);

        /// <summary>
        ///     Gets the height of the menu.
        /// </summary>
        /// <value>The height of the menu.</value>
        public virtual int MenuHeight { get; } = 32;

        public virtual Color TextColor { get; set; } = Color.FromArgb(207, 195, 149);

        public virtual int TextSpacing { get; set; } = 15;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Builds the menu bool renderer.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <returns>IRenderManager&lt;MenuBool&gt;.</returns>
        public abstract IRenderManager<MenuBool> BuildMenuBoolRenderer(MenuBool component);

        /// <summary>
        ///     Builds the menu keybind renderer.
        /// </summary>
        /// <param name="keybind">The keybind.</param>
        /// <returns>IRenderManager&lt;MenuKeybind&gt;.</returns>
        public abstract IRenderManager<MenuKeyBind> BuildMenuKeyBindRenderer(MenuKeyBind keybind);

        /// <summary>
        ///     Builds the menu list.
        /// </summary>
        /// <param name="menuList">The menu list.</param>
        /// <returns>IRenderManager&lt;MenuList&gt;.</returns>
        public abstract IRenderManager<MenuList> BuildMenuList(MenuList menuList);

        /// <summary>
        ///     Builds the menu renderer.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <returns>IRenderManager&lt;IMenu&gt;.</returns>
        public abstract IRenderManager<Menu> BuildMenuRenderer(Menu menu);

        /// <summary>
        ///     Builds the menu seperator renderer.
        /// </summary>
        /// <param name="menuSeperator">The menu seperator.</param>
        /// <returns>IRenderManager&lt;MenuSeperator&gt;.</returns>
        public abstract IRenderManager<MenuSeperator> BuildMenuSeperatorRenderer(MenuSeperator menuSeperator);

        /// <summary>
        ///     Builds the menu slider bool renderer.
        /// </summary>
        /// <param name="menuList">The menu list.</param>
        /// <returns>IRenderManager&lt;MenuList&gt;.</returns>
        public abstract IRenderManager<MenuSliderBool> BuildMenuSliderBoolRenderer(MenuSliderBool menuSliderBool);

        /// <summary>
        ///     Builds the menu slider renderer.
        /// </summary>
        /// <param name="slider">The slider.</param>
        /// <returns>IRenderManager&lt;MenuSlider&gt;.</returns>
        public abstract IRenderManager<MenuSlider> BuildMenuSliderRenderer(MenuSlider slider);

        public abstract Rectangle GetMenuBoolControlBounds(Vector2 pos, int width);

        public abstract Rectangle[] GetMenuListControlBounds(Vector2 pos, int width);

        public abstract Rectangle[] GetMenuSliderBoolControlBounds(Vector2 pos, int width);

        public abstract Rectangle GetMenuSliderControlBounds(Vector2 pos, int width);

        #endregion
    }
}