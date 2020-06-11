namespace Aimtec.SDK.Orbwalking
{
    using System;

    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Config;
    using Aimtec.SDK.Util;

    /// <summary>
    /// </summary>
    public interface IOrbwalker : IDisposable
    {
        #region Public Events

        /// <summary>
        ///     Occurs when there is a non killable minion
        /// </summary>
        event EventHandler<NonKillableMinionEventArgs> OnNonKillableMinion;

        /// <summary>
        ///     Occurs when after an attack has been launched and acknowledged by the server.
        /// </summary>
        event EventHandler<PostAttackEventArgs> PostAttack;

        /// <summary>
        ///     Occurs when the orbwalking is about to launch an attack.
        /// </summary>
        event EventHandler<PreAttackEventArgs> PreAttack;

        /// <summary>
        ///     Occurs before a movement order is issued.
        /// </summary>
        event EventHandler<PreMoveEventArgs> PreMove;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether the orbwalker should disable attacking.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the orbwalker should disable attacking; otherwise, <c>false</c>.
        /// </value>
        bool AttackingEnabled { get; set; }

        /// <summary>
        ///     Gets the target for this orbwalker mode
        /// </summary>
        string[] AttackResets { get; set; }

        /// <summary>
        ///     The Combo Orbwalker Mode
        /// </summary>
        OrbwalkerMode Combo { get; set; }

        /// <summary>
        ///     Gets if the orbwalker is in the middle of casting an auto attack
        ///     If true, then it is not safe to cast any commands. If false, then commands can be issued without interrupting the
        ///     attack.
        /// </summary>
        /// ///
        /// <value>
        ///     <c>true</c> if the orbwalker is in the process of auto attacking, otherwise <c>false</c>.
        /// </value>
        bool IsWindingUp { get; }

        /// <summary>
        ///     The Laneclear Orbwalker Mode
        /// </summary>
        OrbwalkerMode LaneClear { get; set; }

        /// <summary>
        ///     The Laneclear Orbwalker Mode
        /// </summary>
        OrbwalkerMode LastHit { get; set; }

        /// <summary>
        ///     The Mixed Orbwalker Mode
        /// </summary>
        OrbwalkerMode Mixed { get; set; }

        /// <summary>
        ///     Gets or sets the mode.
        /// </summary>
        /// <value>
        ///     The mode.
        /// </value>
        OrbwalkingMode Mode { get; }

        /// <summary>
        ///     Returns the current Orbwalker Mode Name as a string
        /// </summary>
        string ModeName { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the orbwalker should disable moving.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the orbwalker should disable moving; otherwise, <c>false</c>.
        /// </value>
        bool MovingEnabled { get; set; }

        /// <summary>
        ///     Gets the wind up time.
        /// </summary>
        /// <value>
        ///     The wind up time.
        /// </value>
        float WindUpTime { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds a new mode to this Orbwalker Instance
        /// </summary>
        void AddMode(OrbwalkerMode mode);

        /// <summary>
        ///     Adds to menu.
        /// </summary>
        /// <param name="menu">The menu.</param>
        void Attach(IMenu menu);

        /// <summary>
        ///     Processes a movement command through the Orbwalker, firing events, etc.
        /// </summary>
        bool Attack(AttackableUnit target);

        /// <summary>
        ///     Gets the if the player can attack.
        /// </summary>
        /// <value>
        ///     If the player can attack.
        /// </value>
        bool CanAttack();

        /// <summary>
        ///     Gets the if the player can move.
        /// </summary>
        /// <value>
        ///     If the player can move.
        /// </value>
        bool CanMove();

        /// <summary>
        ///     Duplicates an existing Orbwalker mode to a new name and key, useful if your assembly needs additional modes
        /// </summary>
        OrbwalkerMode DuplicateMode(OrbwalkerMode mode, string newName, KeyCode key);

        /// <summary>
        ///     Duplicates an existing Orbwalker mode to a new name and key, useful if your assembly needs additional modes
        /// </summary>
        OrbwalkerMode DuplicateMode(OrbwalkerMode mode, string newName, GlobalKey key);

        /// <summary>
        ///     Forces the target.
        /// </summary>
        /// <param name="unit">The unit.</param>
        void ForceTarget(AttackableUnit unit);

        /// <summary>
        ///     Gets the current active OrbwalkerMode
        /// </summary>
        OrbwalkerMode GetActiveMode();

        /// <summary>
        ///     Gets the target for this orbwalker mode
        /// </summary>
        AttackableUnit FindTarget(OrbwalkerMode mode);

        /// <summary>
        ///     Gets the target.
        /// </summary>
        /// <returns></returns>
        AttackableUnit FindTarget();

        /// <summary>
        ///     Gets the current Orbwalker target
        /// </summary>
        AttackableUnit GetOrbwalkingTarget();

        /// <summary>
        ///     Returns whether this the missile name is an auto attack reset
        /// </summary>
        bool IsReset(string missileName);

        /// <summary>
        ///     Processes a movement command through the Orbwalker, firing events, etc.
        /// </summary>
        bool Move(Vector3 movePosition);

        /// <summary>
        ///     The Orbwalking logic
        /// </summary>
        void Orbwalk();

        /// <summary>
        ///     Resets the automatic attack timer.
        /// </summary>
        void ResetAutoAttackTimer();

        #endregion
    }

    /// <summary>
    ///     A base class used by the Orbwalker for events.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class OrbwalkingEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        /// <value>
        ///     The target.
        /// </value>
        public AttackableUnit Target { get; set; }

        #endregion
    }

    /// <summary>
    ///     The event arguements for the <see cref="IOrbwalker.PreAttack" /> event.
    /// </summary>
    /// <seealso cref="Aimtec.SDK.Orbwalking.OrbwalkingEventArgs" />
    public class PreAttackEventArgs : OrbwalkingEventArgs
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="PreAttackEventArgs" /> is cancel.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel { get; set; } = false;

        #endregion
    }

    /// <summary>
    ///     The event arguements for the <see cref="IOrbwalker.PostAttack" /> event.
    /// </summary>
    /// <seealso cref="Aimtec.SDK.Orbwalking.OrbwalkingEventArgs" />
    public class PostAttackEventArgs : OrbwalkingEventArgs
    {
    }

    /// <summary>
    ///     The event arguements for the <see cref="AOrbwalker.OnNonKillableMinion" /> event.
    /// </summary>
    /// <seealso cref="Aimtec.SDK.Orbwalking.OrbwalkingEventArgs" />
    public class NonKillableMinionEventArgs : OrbwalkingEventArgs
    {
    }

    /// <summary>
    ///     The event arguements for the <see cref="IOrbwalker.PreMove" /> event.
    /// </summary>
    /// <seealso cref="Aimtec.SDK.Orbwalking.OrbwalkingEventArgs" />
    public class PreMoveEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="PreMoveEventArgs" /> is cancel.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel { get; set; }

        /// <summary>
        ///     Gets or sets the move position.
        /// </summary>
        /// <value>The move position.</value>
        public Vector3 MovePosition { get; set; }

        #endregion
    }
}