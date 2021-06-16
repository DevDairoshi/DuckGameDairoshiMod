using System;
using DuckGame;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Explosives")]
    public class PackedGrenade : Grenade
    {
        public bool _pin = true;
        public float _timer = 2f;
        private SpriteMap _sprite;
        private bool _exploded;
        private bool _didBonus;
        private Duck _cookThrower;
        private float _cookTimeOnThrow;

        public PackedGrenade(float xval, float yval)
          : base(xval, yval)
        {
            _sprite = new SpriteMap(GetPath("packed"), 16, 16, false);
            graphic = _sprite;

            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-6f, -8f);
            collisionSize = new Vec2(12f, 16f);
            _barrelOffsetTL = new Vec2(0f, 0f);

            base.bouncy = 0.1f;
            friction = 0.2f;

            editorTooltip = "#1 Pull pin. #2 Throw grenade. #3 More grenades.";
            _editorName = "Packed Grenade";
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

                                if (index % 2 == 0)
                                {
                                    Grenade grenade = new Grenade(this.x, this.y);
                                    grenade._pin = false;
                                    float speedx = Rando.Float(30f) - 15f;
                                    float speedy = Rando.Float(30f) - 15f;
                                    grenade.ApplyForce(new Vec2(speedx, speedy));
                                    Level.Add(grenade);
                                }
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
            this._sprite.frame = (this._pin ? 0 : 1);
        }

        public override void OnPressAction()
        {
            if (_pin)
            {
                _pin = false;

                GrenadePin grenadePin = new GrenadePin(this.x, this.y);
                grenadePin.hSpeed = (float)-this.offDir * (1.5f + Rando.Float(0.5f));
                grenadePin.vSpeed = -2f;
                Level.Add((Thing)grenadePin);
                if (this.duck != null)
                    RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(this._fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                SFX.Play("pullPin");
            }
        }


    }
}
