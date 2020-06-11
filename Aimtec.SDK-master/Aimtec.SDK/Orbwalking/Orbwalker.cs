namespace Aimtec.SDK.Orbwalking
{
    using System;

    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Config;
    using Aimtec.SDK.Util;
    using System.Collections.Generic;

    /// <summary>
    ///     Orbwalker class
    /// </summary>
    public class Orbwalker : IOrbwalker
    {
        #region Static Fields

        private static IOrbwalker impl;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Orbwalker" /> class.
        /// </summary>
        /// <param name="impl">The implementation.</param>
        public Orbwalker(IOrbwalker impl)
        {
            Implementation = impl;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Orbwalker" /> class.
        /// </summary>
        public Orbwalker()
            : this(new OrbwalkingImpl())
        {
            OrbwalkerInstances.Add(this);
        }

        #endregion

        #region Public Events

        /// <inheritdoc cref="IOrbwalker" />
        public event EventHandler<NonKillableMinionEventArgs> OnNonKillableMinion
        {
            add => Implementation.OnNonKillableMinion += value;
            remove => Implementation.OnNonKillableMinion -= value;
        }

        /// <inheritdoc cref="IOrbwalker" />
        public event EventHandler<PostAttackEventArgs> PostAttack
        {
            add => Implementation.PostAttack += value;
            remove => Implementation.PostAttack -= value;
        }

        /// <inheritdoc cref="IOrbwalker" />
        public event EventHandler<PreAttackEventArgs> PreAttack
        {
            add => Implementation.PreAttack += value;
            remove => Implementation.PreAttack -= value;
        }

        /// <inheritdoc cref="IOrbwalker" />
        public event EventHandler<PreMoveEventArgs> PreMove
        {
            add => Implementation.PreMove += value;
            remove => Implementation.PreMove -= value;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the instances of the Orbwalker
        /// </summary>
        public static List<IOrbwalker> OrbwalkerInstances { get; set; } = new List<IOrbwalker>();

        /// <summary>
        ///     Gets or sets the implementation of the orbwalker.
        /// </summary>
        /// <value>
        ///     The implementation of the orbwalker.
        /// </value>
        public static IOrbwalker Implementation
        {
            get
            {
                if (impl == null)
                {
                    impl = new OrbwalkingImpl();
                }

                return impl;
            }

            set
            {
                impl?.Dispose();
                impl = value;
            }
        }

        /// <inheritdoc cref="IOrbwalker" />
        public bool AttackingEnabled
        {
            get => Implementation.AttackingEnabled;
            set => Implementation.AttackingEnabled = value;
        }

        /// <inheritdoc cref="IOrbwalker" />
        public string[] AttackResets
        {
            get => Implementation.AttackResets;
            set => Implementation.AttackResets = value;
        }

        /// <inheritdoc cref="IOrbwalker" />
        public OrbwalkerMode Combo
        {
            get => Implementation.Combo;
            set => Implementation.Combo = value;
        }

        /// <inheritdoc cref="IOrbwalker" />
        public bool IsWindingUp => Implementation.IsWindingUp;

        /// <inheritdoc cref="IOrbwalker" />
        public OrbwalkerMode LaneClear
        {
            get => Implementation.LaneClear;
            set => Implementation.LaneClear = value;
        }

        /// <inheritdoc cref="IOrbwalker" />
        public OrbwalkerMode LastHit
        {
            get => Implementation.LastHit;
            set => Implementation.LastHit = value;
        }

        /// <inheritdoc cref="IOrbwalker" />
        public OrbwalkerMode Mixed
        {
            get => Implementation.Mixed;
            set => Implementation.Mixed = value;
        }

        /// <inheritdoc cref="IOrbwalker" />
        public OrbwalkingMode Mode
        {
            get => Implementation.Mode;
        }

        /// <inheritdoc cref="IOrbwalker" />
        public string ModeName => Implementation.ModeName;

        /// <inheritdoc cref="IOrbwalker" />
        public bool MovingEnabled
        {
            get => Implementation.MovingEnabled;
            set => Implementation.MovingEnabled = value;
        }

        /// <inheritdoc cref="IOrbwalker" />
        public float WindUpTime => Implementation.WindUpTime;

        #endregion

        #region Public Methods and Operators

        /// <inheritdoc cref="IOrbwalker" />
        public void AddMode(OrbwalkerMode mode)
        {
            Implementation.AddMode(mode);
        }

        /// <inheritdoc cref="IOrbwalker" />
        public void Attach(IMenu menu)
        {
            Implementation.Attach(menu);
        }

        /// <inheritdoc cref="IOrbwalker" />
        public bool Attack(AttackableUnit target)
        {
            return Implementation.Attack(target);
        }

        /// <inheritdoc cref="IOrbwalker" />
        public bool CanAttack()
        {
            return Implementation.CanAttack();
        }

        /// <inheritdoc cref="IOrbwalker" />
        public bool CanMove()
        {
            return Implementation.CanMove();
        }

        /// <inheritdoc cref="IOrbwalker" />
        public void Dispose()
        {
            OrbwalkerInstances.Remove(Implementation);
            Implementation.Dispose();
        }

        /// <inheritdoc cref="IOrbwalker" />
        public OrbwalkerMode DuplicateMode(OrbwalkerMode mode, string newName, KeyCode key)
        {
            return Implementation.DuplicateMode(mode, newName, key);
        }

        /// <inheritdoc cref="IOrbwalker" />
        public OrbwalkerMode DuplicateMode(OrbwalkerMode mode, string newName, GlobalKey key)
        {
            return Implementation.DuplicateMode(mode, newName, key);
        }

        /// <inheritdoc cref="IOrbwalker" />
        public void ForceTarget(AttackableUnit unit)
        {
            Implementation.ForceTarget(unit);
        }

        /// <inheritdoc cref="IOrbwalker" />
        public OrbwalkerMode GetActiveMode()
        {
            return Implementation.GetActiveMode();
        }

        /// <inheritdoc cref="IOrbwalker" />
        public AttackableUnit FindTarget(OrbwalkerMode mode)
        {
            return Implementation.FindTarget(mode);
        }

        /// <inheritdoc cref="IOrbwalker" />
        public AttackableUnit FindTarget()
        {
            return Implementation.FindTarget();
        }

        /// <inheritdoc cref="IOrbwalker" />
        public AttackableUnit GetOrbwalkingTarget()
        {
            return Implementation.GetOrbwalkingTarget();
        }

        /// <inheritdoc cref="IOrbwalker" />
        public bool IsReset(string missileName)
        {
            return Implementation.IsReset(missileName);
        }

        /// <inheritdoc cref="IOrbwalker" />
        public bool Move(Vector3 movePosition)
        {
            return Implementation.Move(movePosition);
        }

        /// <inheritdoc cref="IOrbwalker" />
        public void Orbwalk()
        {
            Implementation.Orbwalk();
        }

        /// <inheritdoc cref="IOrbwalker" />
        public void ResetAutoAttackTimer()
        {
            Implementation.ResetAutoAttackTimer();
        }

        #endregion
    }
}