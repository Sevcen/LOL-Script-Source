namespace Aimtec.SDK.Menu.Config
{
    using Aimtec.SDK.Menu.Components;
    using Aimtec.SDK.Util;

    using NLog.Fluent;

    public class GlobalKeys
    {
        #region Static Fields

        internal static Menu KeyConfig;

        #endregion

        #region Public Properties

        public static GlobalKey BurstKey { get; set; }

        //Main Keys - To be enabled by default
        public static GlobalKey ComboKey { get; set; }

        public static GlobalKey ComboNoOrbwalkKey { get; set; }

        public static GlobalKey FleeKey { get; set; }

        public static GlobalKey FreezeKey { get; set; }

        //Additional Keys that can be enabled by assemblies if they require it
        public static GlobalKey HarassKey { get; set; }

        public static GlobalKey LastHitKey { get; set; }

        public static GlobalKey MixedKey { get; set; }

        public static GlobalKey TowerDiveKey { get; set; }

        public static GlobalKey WaveClearKey { get; set; }

        #endregion

        #region Methods

        internal static void Load()
        {
            Log.Info().Message("Loading Global Keys").Write();

            KeyConfig = new Menu("Keys", "Keys");

            KeyConfig.Add(new MenuSeperator("seperator", "Main Keys"));

            ComboKey = new GlobalKey("Combo", "Combo", KeyCode.Space, KeybindType.Press, true);
            MixedKey = new GlobalKey("Mixed", "Mixed", KeyCode.C, KeybindType.Press, true);
            WaveClearKey = new GlobalKey("WaveClear", "Waveclear", KeyCode.V, KeybindType.Press, true);
            LastHitKey = new GlobalKey("LastHit", "LastHit", KeyCode.X, KeybindType.Press, true);

            KeyConfig.Add(new MenuSeperator("seperator2", "Additional Keys"));

            HarassKey = new GlobalKey("Harass", "Harass", KeyCode.H, KeybindType.Toggle, false);
            FreezeKey = new GlobalKey("Freeze", "Freeze", KeyCode.M, KeybindType.Toggle, false);
            BurstKey = new GlobalKey("Burst", "Burst", KeyCode.K, KeybindType.Press, false);
            FleeKey = new GlobalKey("Flee", "Flee", KeyCode.L, KeybindType.Press, false);
            TowerDiveKey = new GlobalKey("TowerDive", "Tower Dive", KeyCode.T, KeybindType.Press, false);

            ComboNoOrbwalkKey = new GlobalKey(
                "ComboNoOrbwalk",
                "Combo - No Orbwalk",
                KeyCode.J,
                KeybindType.Toggle,
                false);

            AimtecMenu.Instance.Add(KeyConfig);
        }

        #endregion
    }

    public class GlobalKey
    {
        #region Constructors and Destructors

        internal GlobalKey(string internalName, string displayName, KeyCode keyCode, KeybindType type, bool enabled)
        {
            this.KeyBindItem = new MenuKeyBind(internalName, displayName, keyCode, type);

            if (enabled)
            {
                this.Activate();
            }
        }

        #endregion

        #region Public Properties

        //Gets whether the keybind is active
        public bool Active
        {
            get
            {
                if (!this.AddedToMenu)
                {
                    this.Activate();
                }

                return this.KeyBindItem.Value;
            }
        }

        //The Menu item associated with this Key
        public MenuKeyBind KeyBindItem { get; }

        #endregion

        #region Properties

        private bool AddedToMenu { get; set; }

        #endregion

        #region Public Methods and Operators

        //Enables the key by adding it to the keylist
        public void Activate()
        {
            if (!this.AddedToMenu)
            {
                GlobalKeys.KeyConfig.Add(this.KeyBindItem);
                this.AddedToMenu = true;
            }
        }

        #endregion
    }
}