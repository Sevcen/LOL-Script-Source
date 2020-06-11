namespace Aimtec.SDK.Menu.Theme.Default
{
    using System.Drawing;

    using Aimtec.SDK.Menu.Components;

    internal class DefaultMenuSeperator : IRenderManager<MenuSeperator, DefaultMenuTheme>
    {
        #region Constructors and Destructors

        public DefaultMenuSeperator(MenuSeperator component, DefaultMenuTheme theme)
        {
            this.Component = component;
            this.Theme = theme;
        }

        #endregion

        #region Public Properties

        public MenuSeperator Component { get; }

        public DefaultMenuTheme Theme { get; }

        #endregion

        #region Public Methods and Operators

        public void Render(Vector2 pos)
        {
            var width = this.Component.Parent.Width;

            this.Theme.DrawMenuItemBorder(pos, width);

            var position = pos + this.Theme.LineWidth;

            this.Theme.DrawMenuItemBoxFull(position, width);

            Aimtec.Render.Text(
                position + new Vector2(width / 2f, this.Theme.MenuHeight / 2f),
                Color.FromArgb(207, 195, 149),
                this.Component.Value,
                RenderTextFlags.HorizontalCenter | RenderTextFlags.VerticalCenter);
        }

        #endregion
    }
}