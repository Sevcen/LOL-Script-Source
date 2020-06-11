namespace Aimtec.SDK.Menu.Theme.Default
{
    using System.Drawing;

    internal class DefaultMenu : IRenderManager<Menu, DefaultMenuTheme>
    {
        #region Constructors and Destructors

        public DefaultMenu(Menu component, DefaultMenuTheme theme)
        {
            this.Component = component;
            this.Theme = theme;
        }

        #endregion

        #region Public Properties

        public Menu Component { get; }

        public DefaultMenuTheme Theme { get; }

        #endregion

        #region Public Methods and Operators

        public void Render(Vector2 pos)
        {
            var width = this.Component.Parent.Width;

            this.Theme.DrawMenuItemBorder(pos, width);

            var position = pos + this.Theme.LineWidth;

            this.Theme.DrawMenuItemBox(position, width);

            var displayNamePosition = position + new Vector2(this.Theme.TextSpacing, this.Theme.MenuHeight / 2);

            Aimtec.Render.Text(
                displayNamePosition,
                this.Theme.TextColor,
                this.Component.DisplayName,
                RenderTextFlags.VerticalCenter | RenderTextFlags.HorizontalLeft);

            // Render arrow outline
            Aimtec.Render.Line(
                pos.X + width - this.Theme.IndicatorWidth - this.Theme.LineWidth,
                pos.Y,
                pos.X + width - this.Theme.IndicatorWidth - this.Theme.LineWidth,
                pos.Y + this.Theme.MenuHeight,
                this.Theme.LineColor);

            // Draw arrow box
            position += new Vector2(width - this.Theme.IndicatorWidth - this.Theme.LineWidth, 0);

            var arrowBoxColor = Color.FromArgb(14, 59, 73);

            if (!this.Component.Toggled)
            {
                arrowBoxColor = Color.FromArgb(11, 18, 20);
            }

            Aimtec.Render.Rectangle(
                position,
                this.Theme.IndicatorWidth,
                this.Theme.MenuHeight - this.Theme.LineWidth,
                arrowBoxColor);

            Aimtec.Render.Text(
                position + new Vector2(
                    this.Theme.IndicatorWidth / 2,
                    (this.Theme.MenuHeight - this.Theme.LineWidth) / 2),
                Color.FromArgb(207, 195, 149),
                ">",
                RenderTextFlags.VerticalCenter | RenderTextFlags.HorizontalCenter);
        }

        #endregion
    }
}