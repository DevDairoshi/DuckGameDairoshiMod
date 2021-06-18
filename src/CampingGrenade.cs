using System;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Explosives")]
    public class CampingGrenade : Grenade
    {
        public bool _pin = true;
        public float _timer = 3f;
        private SpriteMap _sprite;
        private bool _exploded;
        private bool _didBonus;
        private Duck _cookThrower;
        private float _cookTimeOnThrow;

        public CampingGrenade(float xval, float yval)
          : base(xval, yval)
        {
            _sprite = new SpriteMap(GetPath("camping"), 18, 18, false);
            graphic = _sprite;

            center = new Vec2(9f, 9f);
            collisionOffset = new Vec2(-9f, -9f);
            collisionSize = new Vec2(18f, 18f);
            _barrelOffsetTL = new Vec2(0f, 0f);

            base.bouncy = 0.9f;
            friction = 0.1f;
            airFrictionMult = 0.1f;
            weight = 0.3f;
            _impactThreshold = 0.05f;

            editorTooltip = "It will capture every duck, why are You running? WHY ARE YOU RUNNING!?";
            _editorName = "Camping Grenade";
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
                                CampingBall ball = new CampingBall(this.x, this.y, this.duck);
                                float speedx = Rando.Float(30f) - 15f;
                                float speedy = Rando.Float(30f) - 15f;
                                ball.ApplyForce(new Vec2(speedx, speedy));
                                Level.Add(ball);
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
