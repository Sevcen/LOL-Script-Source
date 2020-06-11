namespace Aimtec.SDK.Menu.Theme.Default
{
    using System.Drawing;

    using Aimtec.SDK.Menu.Components;

    internal class DefaultMenuBool : IRenderManager<MenuBool, DefaultMenuTheme>
    {
        #region Constructors and Destructors

        public DefaultMenuBool(MenuBool component, DefaultMenuTheme theme)
        {
            this.Component = component;
            this.Theme = theme;
        }

        #endregion

        #region Public Properties

        public MenuBool Component { get; }

        public DefaultMenuTheme Theme { get; }

        #endregion

        #region Public Methods and Operators

        public void Render(Vector2 pos)
        {
            var width = this.Component.Parent.Width;

            this.Theme.DrawMenuItemBorder(pos, width);

            var position = pos + this.Theme.LineWidth;

            this.Theme.DrawMenuItemBox(position, width);

            var centerPoint = pos + new Vector2(
                width - this.Theme.LineWidth * 2 / 2,
                this.Theme.MenuHeight - this.Theme.LineWidth * 2 / 2);

            var displayNamePosition = position + new Vector2(this.Theme.TextSpacing, this.Theme.MenuHeight / 2);

            Aimtec.Render.Text(
                displayNamePosition,
                Color.FromArgb(207, 195, 149),
                this.Component.DisplayName,
                RenderTextFlags.VerticalCenter);

            // Render indicator box outline
            Aimtec.Render.Line(
                pos.X + width - this.Theme.IndicatorWidth - this.Theme.LineWidth,
                pos.Y,
                pos.X + width - this.Theme.IndicatorWidth - this.Theme.LineWidth,
                pos.Y + this.Theme.MenuHeight,
                Color.FromArgb(82, 83, 57));

            // Draw indicator box
            var indBoxPosition = position + new Vector2(width - this.Theme.IndicatorWidth - this.Theme.LineWidth, 0);

            var boolColor = this.Component.Value ? Color.FromArgb(39, 96, 17) : Color.FromArgb(85, 25, 15);

            Aimtec.Render.Rectangle(
                indBoxPosition,
                this.Theme.IndicatorWidth,
                this.Theme.MenuHeight - this.Theme.LineWidth,
                boolColor);

            var centerArrowBox = indBoxPosition + new Vector2(this.Theme.IndicatorWidth / 2, this.Theme.MenuHeight / 2);

            Aimtec.Render.Text(
                centerArrowBox,
                Color.AliceBlue,
                this.Component.Value ? "ON" : "OFF",
                RenderTextFlags.HorizontalCenter | RenderTextFlags.VerticalCenter);
        }

        #endregion
    }
}