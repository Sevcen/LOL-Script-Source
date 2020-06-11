namespace Aimtec.SDK.Menu.Components
{
    using Aimtec.SDK.Menu.Theme;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public sealed class MenuSeperator : MenuComponent, IReturns<string>
    {
        #region Constructors and Destructors

        public MenuSeperator(string internalName, string text = "")
        {
            this.InternalName = internalName;
            this.DisplayName = text;
            this.Value = text;
        }

        #endregion

        #region Public Properties

        public new string Value { get; set; }

        #endregion

        #region Properties

        internal override bool SavableMenuItem { get; set; } = false;

        internal override string Serialized => JsonConvert.SerializeObject(this, Formatting.Indented);

        #endregion

        #region Methods

        internal override IRenderManager GetRenderManager()
        {
            return MenuManager.Instance.Theme.BuildMenuSeperatorRenderer(this);
        }

        internal override void LoadValue()
        {
        }

        #endregion
    }
}