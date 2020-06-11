namespace Aimtec.SDK.Menu.Theme.Default
{
    using System.Drawing;

    using Aimtec.SDK.Menu.Components;

    public class DefaultMenuTheme : MenuTheme
    {
        #region Public Methods and Operators

        public static void DrawRectangleOutline(
            float x,
            float y,
            float width,
            float height,
            float lineWidth,
            Color color)
        {
            // Top left to top right
            Render.Line(x, y, x + width, y, lineWidth, true, color);

            // Top right to bottom right
            Render.Line(x + width, y, x + width, y + height, lineWidth, true, color);

            // Bottom right to bottom left
            Render.Line(x + width, y + height, x, y + height, lineWidth, true, color);

            // Bottom left to top left
            Render.Line(x, y + height, x, y, lineWidth, true, color);
        }

        public override IRenderManager<MenuBool> BuildMenuBoolRenderer(MenuBool component)
        {
            return new DefaultMenuBool(component, this);
        }

        public override IRenderManager<MenuKeyBind> BuildMenuKeyBindRenderer(MenuKeyBind keybind)
        {
            return new DefaultMenuKeyBind(keybind, this);
        }

        public override IRenderManager<MenuList> BuildMenuList(MenuList menuList)
        {
            return new DefaultMenuList(menuList, this);
        }

        public override IRenderManager<Menu> BuildMenuRenderer(Menu menu)
        {
            return new DefaultMenu(menu, this);
        }

        public override IRenderManager<MenuSeperator> BuildMenuSeperatorRenderer(MenuSeperator menuSeperator)
        {
            return new DefaultMenuSeperator(menuSeperator, this);
        }

        public override IRenderManager<MenuSliderBool> BuildMenuSliderBoolRenderer(MenuSliderBool menuSliderBool)
        {
            return new DefaultMenuSliderBool(menuSliderBool, this);
        }

        public override IRenderManager<MenuSlider> BuildMenuSliderRenderer(MenuSlider slider)
        {
            return new DefaultMenuSlider(slider, this);
        }

        public override Rectangle GetMenuBoolControlBounds(Vector2 pos, int width)
        {
            var newPos = pos + new Vector2(width - this.IndicatorWidth - this.LineWidth, 0);
            return new Rectangle((int) newPos.X, (int) newPos.Y, this.IndicatorWidth, this.MenuHeight);
        }

        public override Rectangle[] GetMenuListControlBounds(Vector2 pos, int width)
        {
            var leftBox = pos + new Vector2(width - this.IndicatorWidth * 2.1f - this.LineWidth, 0);
            var rightBox = pos + new Vector2(width - this.IndicatorWidth - this.LineWidth, 0);
            var rect1 = new Rectangle((int) leftBox.X, (int) leftBox.Y, this.IndicatorWidth, this.MenuHeight);
            var rect2 = new Rectangle((int) rightBox.X, (int) rightBox.Y, this.IndicatorWidth, this.MenuHeight);
            return new Rectangle[] { rect1, rect2 };
        }

        public override Rectangle[] GetMenuSliderBoolControlBounds(Vector2 pos, int width)
        {
            var boolPosition = pos + new Vector2(width - this.IndicatorWidth - this.LineWidth, 0);

            var sliderPosition = pos;

            var boolBounds = new Rectangle(
                (int) boolPosition.X,
                (int) boolPosition.Y,
                this.IndicatorWidth,
                this.MenuHeight);
            ;

            var sliderBounds = new Rectangle(
                (int) sliderPosition.X,
                (int) sliderPosition.Y,
                width - this.IndicatorWidth,
                this.MenuHeight);

            return new Rectangle[] { sliderBounds, boolBounds };
        }

        public override Rectangle GetMenuSliderControlBounds(Vector2 pos, int width)
        {
            var sliderPosition = pos;
            var sliderBounds = new Rectangle((int) sliderPosition.X, (int) sliderPosition.Y, width, this.MenuHeight);
            return sliderBounds;
        }

        #endregion

        #region Methods

        internal void DrawMenuItemBorder(Vector2 pos, int width)
        {
            DrawRectangleOutline(pos.X, pos.Y, width, this.MenuHeight, this.LineWidth, this.LineColor);
        }

        internal void DrawMenuItemBox(Vector2 position, int boxWidth)
        {
            Render.Rectangle(
                position,
                boxWidth - this.IndicatorWidth - this.LineWidth,
                this.MenuHeight - this.LineWidth,
                this.MenuBoxBackgroundColor);
        }

        internal void DrawMenuItemBoxFull(Vector2 position, int boxWidth)
        {
            Render.Rectangle(
                position,
                boxWidth - this.LineWidth,
                this.MenuHeight - this.LineWidth,
                this.MenuBoxBackgroundColor);
        }

        #endregion
    }
}