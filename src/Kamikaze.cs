using System;
using DuckGame;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Explosives")]
    public class Kamikaze : Grenade
    {
        public bool _pin = true;
        public float _timer = 0f;
        private Sprite _sprite;
        private bool _exploded;
        private bool _didBonus;
        private Duck _cookThrower;
        private float _cookTimeOnThrow;

        public Kamikaze(float xval, float yval) : base(xval, yval)
        {
            _sprite = new Sprite(GetPath("kamikaze"));
            graphic = _sprite;

            center = new Vec2(7.5f, 5.5f);
            collisionOffset = new Vec2(-7.5f, -3.5f);
            collisionSize = new Vec2(15f, 9f);
            _barrelOffsetTL = new Vec2(0f, 0f);
            _holdOffset = new Vec2(-4f, 4f);

            base.bouncy = 0.1f;
            friction = 0.2f;

            editorTooltip = "\"Are You sure about this?\" - John Cena";
            _editorName = "Kamikaze";
        }

        public override void OnPressAction()
        {
            if (_pin)
            {
                _pin = false;

                if (this.duck != null)
                {
                    RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(this._fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                    this.duck.Kill(new DTIncinerate(this));
                }
            }
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
                        Graphics.FlashScreen();
                        ATMissile.DestroyRadius(this.position, 50f, this);

                        if (this.isServerForObject)
                        {
                            for (int index = 0; index < 20; ++index)
                            {
                                float num2 = (float)((double)index * 18.0 - 5.0) + Rando.Float(10f);
                                ATShrapnel atShrapnel = new ATShrapnel();
                                atShrapnel.range = 60f + Rando.Float(18f);
                                Bullet bullet = new Bullet(x + (float)(Math.Cos((double)Maths.DegToRad(num2)) * 6.0), num1 - (float)(Math.Sin((double)Maths.DegToRad(num2)) * 6.0), (AmmoType)atShrapnel, num2);
                                bullet.firedFrom = (Thing)this;
                                this.firedBullets.Add(bullet);
                                Level.Add((Thing)bullet);
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
