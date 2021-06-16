using System;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Explosives")]
    public class AtomicBomb : Grenade, IPlatform
    {
        public StateBinding _CyclesStateBinding = new StateBinding("_cycles");
        public StateBinding _PitchStateBinding = new StateBinding("_pitch");
        public StateBinding _TimeStateBinding = new StateBinding("_time");

        private SpriteMap _sprite;
        private bool _exploded;
        private bool _didBonus;
        private Duck _cookThrower;
        private float _cookTimeOnThrow;

        public AtomicBomb(float xval, float yval) : base(xval, yval)
        {
            _sprite = new SpriteMap(GetPath("atomic"), 17, 27, false);
            graphic = _sprite;

            center = new Vec2(8f, 13f);
            collisionSize = new Vec2(17f, 27f);
            collisionOffset = new Vec2(-8f, -13f);
            _barrelOffsetTL = new Vec2(0, 0);
            
            this.friction = 0.2f;
            this._fireRumble = RumbleIntensity.Kick;
            this._dontCrush = false;

            weight = 10;
            editorTooltip = "\"Pls not again\" - people of Hiroshima before second nuke";
            _editorName = "Atomic Bomb";
            _timer = 8f;
            _pin = true;
            _time = 0.8f;
            _cycles = 0;
            ammo = 99;
        }

        public void CreateExplosion(Vec2 pos)
        {
            float radius = 200f;
            ATMissile.DestroyRadius(pos, radius, this);

            for (int i = 0; i < 100; i++)
            {
                float speedx = Rando.Float(20f) - 10f;
                float speedy = Rando.Float(10f) - 5f;
                Level.Add((Thing)SmallFire.New(this.barrelPosition.x, this.barrelPosition.y, speedx, speedy, firedFrom: ((Thing)this)));
            }

            foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(pos, radius))
            {
                if (physicsObject.GetType() == typeof(Duck))
                {
                    (physicsObject as Duck).Kill(new DTIncinerate(physicsObject));
                }
                else if (physicsObject.GetType() == typeof(RagdollPart))
                {
                    (physicsObject as RagdollPart)._doll._duck.Kill(new DTIncinerate(physicsObject));
                }
            }

            RumbleManager.AddRumbleEvent(pos, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Long, RumbleFalloff.Long));
            Graphics.FlashScreen();
            SFX.Play("explode");
            explode = true;
        }

        public override void Update()
        {
            base.Update();

            if (!this._pin)
            {
                Grenade grenade = this;

                if (_cycles >= 15)
                {
                    _time -= 0.03f;
                }

                _time -= 0.01f;

                if (_time < 0f)
                {
                    _cycles++;
                    _time = 0.4f;
                    SFX.Play(GetPath("timer"), 1f, _pitch);
                    _pitch += 0.02f;
                }
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
                --this._explodeFrames;
                if (this._explodeFrames == 0)
                {
                    float x = this.x;
                    float num1 = this.y - 2f;
                    Graphics.FlashScreen();
                    if (this.isServerForObject)
                    {

                        CreateExplosion(position);

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
            if (base.prevOwner != null && this._cookThrower == null)
            {
                this._cookThrower = base.prevOwner as Duck;
                this._cookTimeOnThrow = this._timer;
            }
            this._sprite.frame = (this._pin ? 0 : 1);
        }

        public override void OnPressAction()
        {
            if (this._pin)
            {
                this._pin = false;
                _time = 0.2f;
                SFX.Play(GetPath("press"));
            }
        }

        public override void Fire()
        {
            // nothing here
        }

        private float _time;
        private int _cycles;
        private float _pitch;
    }
}
