using System;
using DuckGame;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Explosives")]
    public class Molotov : Grenade
    {
        public bool _pin = true;
        public float _timer = 999f;
        private SpriteMap _sprite;
        private bool _exploded;
        private bool _didBonus;
        private Duck _cookThrower;
        private float _cookTimeOnThrow;

        public Molotov(float xval, float yval) : base(xval, yval)
        {
            _sprite = new SpriteMap(GetPath("molotov"), 10, 20, false);
            _sprite.AddAnimation("*burn*", 0.5f, true, 1,2,3,4);
            graphic = _sprite;

            center = new Vec2(5f, 13f);
            collisionOffset = new Vec2(-4f, -10f);
            collisionSize = new Vec2(7f, 17f);
            _barrelOffsetTL = new Vec2(5f, 13f);

            base.bouncy = 0.3f;
            friction = 0.1f;
            
            editorTooltip = "Oi blyat";
            _editorName = "Molotov";
        }

        public override void OnPressAction()
        {
            if (_pin)
            {
                _pin = false;

                if (this.duck != null)
                    RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(this._fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                SFX.Play(GetPath("lighter"));
                _sprite.SetAnimation("*burn*");
            }
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom @from)
        {
            if (!_pin) _timer = 0f;
            base.OnSolidImpact(with, @from);
        }

        public override void Update()
        {
            base.Update();
            if (!_pin)
            {
                _timer -= 0.01f;
            }
            if (this._timer < 0.5f && this.owner == null && !this._didBonus)
            {
                this._didBonus = true;
                if (Recorder.currentRecording != null)
                {
                    Recorder.currentRecording.LogBonus();
                }
            }

            if (_timer < 0f && !_exploded)
            {
                if (this._explodeFrames < 0)
                {
                    this.CreateExplosion(this.position);
                    this._explodeFrames = 4;
                }
                else
                {
                    --this._explodeFrames;
                    if (this._explodeFrames == 0)
                    {
                        float x = this.x;
                        float num1 = this.y - 2f;
                        FluidData fluid = Fluid.Gas;
                        fluid.amount = 0.03f;
                        if (this.isServerForObject)
                        {
                            for (int index = 0; index < 10; ++index)
                            {
                                Level.Add((Thing)new Fluid(this.barrelPosition.x, this.barrelPosition.y, new Vec2(0.0f, -1f), fluid));
                            }
                            for (int i = 0; i < 50; i++)
                            {
                                float speedx = Rando.Float(4f) - 2f;
                                float speedy = Rando.Float(4f) - 2f;
                                Level.Add((Thing)SmallFire.New(this.barrelPosition.x, this.barrelPosition.y, speedx, speedy, firedFrom: ((Thing)this)));
                            }
                            foreach (Window window in Level.CheckCircleAll<Window>(this.position, 40f))
                            {
                                if (Level.CheckLine<Block>(this.position, window.position, (Thing)window) == null)
                                    window.Destroy((DestroyType)new DTImpact((Thing)this));
                            }
                            this.bulletFireIndex += (byte)20;
                            if (Network.isActive)
                            {
                                Send.Message((NetMessage)new NMFireGun((Gun)this, this.firedBullets, this.bulletFireIndex, false), NetMessagePriority.ReliableOrdered);

                                this.firedBullets.Clear();
                            }
                        }
                        Level.Remove((Thing)this);
                        SFX.Play(GetPath("glass"));
                        this._destroyed = true;
                        this._explodeFrames = -1;
                    }
                }
            }

            if (base.prevOwner != null && this._cookThrower == null)
            {
                this._cookThrower = base.prevOwner as Duck;
                this._cookTimeOnThrow = this._timer;
            }
        }
    }
}
