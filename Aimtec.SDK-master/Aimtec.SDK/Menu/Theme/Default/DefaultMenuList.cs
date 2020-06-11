namespace Aimtec.SDK.Menu.Theme.Default
{
    using System.Drawing;

    using Aimtec.SDK.Menu.Components;

    /// <summary>
    ///     Class DefaultMenuList.
    /// </summary>
    /// <seealso
    ///     cref="Aimtec.SDK.Menu.Theme.IRenderManager{MenuList, DefaultMenuTheme}" />
    internal class DefaultMenuList : IRenderManager<MenuList, DefaultMenuTheme>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultMenuList" /> class.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="theme">The theme.</param>
        public DefaultMenuList(MenuList component, DefaultMenuTheme theme)
        {
            this.Component = component;
            this.Theme = theme;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the component.
        /// </summary>
        /// <value>The component.</value>
        public MenuList Component { get; }

        /// <summary>
        ///     Gets the theme.
        /// </summary>
        /// <value>The theme.</value>
        public DefaultMenuTheme Theme { get; }

        #endregion

        #region Public Methods and Operators

        #region Public Methods and Operators

        /// <summary>
        ///     Renders at the specified position.
        /// </summary>
        /// <param name="pos">The position.</param>
        public void Render(Vector2 pos)
        {
            var width = this.Component.Parent.Width;

            this.Theme.DrawMenuItemBorder(pos, width);

            var position = pos + this.Theme.LineWidth;

            this.Theme.DrawMenuItemBox(position, width);

            var displayNamePosition = position + new Vector2(this.Theme.TextSpacing, this.Theme.MenuHeight / 2);

            Aimtec.Render.Text(
                displayNamePosition,
                Color.FromArgb(207, 195, 149),
                this.Component.DisplayName,
                RenderTextFlags.VerticalCenter);

            // Render arrow outline 1 - left arrow 
            Aimtec.Render.Line(
                pos.X + width - this.Theme.IndicatorWidth * 2.1F - this.Theme.LineWidth,
                pos.Y,
                pos.X + width - this.Theme.IndicatorWidth * 2.1F - this.Theme.LineWidth,
                pos.Y + this.Theme.MenuHeight,
                Color.FromArgb(82, 83, 57));

            // Render arrow outline 2 - right arrow
            Aimtec.Render.Line(
                pos.X + width - this.Theme.IndicatorWidth - this.Theme.LineWidth,
                pos.Y,
                pos.X + width - this.Theme.IndicatorWidth - this.Theme.LineWidth,
                pos.Y + this.Theme.MenuHeight,
                Color.FromArgb(82, 83, 57));

            var leftBoxPosition = position + new Vector2(
                width - this.Theme.IndicatorWidth * 2.1f - this.Theme.LineWidth,
                0);
            var rightBoxPosition = position + new Vector2(width - this.Theme.IndicatorWidth - this.Theme.LineWidth, 0);

            Aimtec.Render.Text(
                leftBoxPosition + new Vector2(-this.Theme.TextSpacing, this.Theme.MenuHeight / 2),
                Color.FromArgb(207, 195, 149),
                this.Component.Items[this.Component.Value],
                RenderTextFlags.VerticalCenter | RenderTextFlags.HorizontalRight);

            // Draw arrow boxes
            Aimtec.Render.Rectangle(
                leftBoxPosition,
                this.Theme.IndicatorWidth,
                this.Theme.MenuHeight - this.Theme.LineWidth,
                Color.FromArgb(16, 26, 29));

            Aimtec.Render.Text(
                leftBoxPosition + new Vector2(this.Theme.IndicatorWidth / 2, this.Theme.MenuHeight / 2),
                Color.FromArgb(207, 195, 149),
                "<",
                RenderTextFlags.HorizontalCenter | RenderTextFlags.VerticalCenter);

            Aimtec.Render.Rectangle(
                rightBoxPosition,
                this.Theme.IndicatorWidth,
                this.Theme.MenuHeight - this.Theme.LineWidth,
                Color.FromArgb(16, 26, 29));

            Aimtec.Render.Text(
                rightBoxPosition + new Vector2(this.Theme.IndicatorWidth / 2, this.Theme.MenuHeight / 2),
                Color.FromArgb(207, 195, 149),
                ">",
                RenderTextFlags.HorizontalCenter | RenderTextFlags.VerticalCenter);

            #endregion
        }

        #endregion
    }
}