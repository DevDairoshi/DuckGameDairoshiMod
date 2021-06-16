using System;
using System.Collections.Generic;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Explosives")]
    public class RocketLauncher : Pistol
    {
        private SpriteMap _barrelSteam;
        public RocketLauncher(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("homing"));

            this.center = new Vec2(15f, 5f);
            this.collisionOffset = new Vec2(-15f, -4f);
            this.collisionSize = new Vec2(30f, 10f);
            this._barrelOffsetTL = new Vec2(29f, 4f);
            this._holdOffset = new Vec2(-2f, -2f);

            wideBarrel = true;
            _kickForce = 2f;
            _weight = 4f;
            ammo = 1;
            _type = "gun";
            _ammoType = (AmmoType)new ATDefault();
            _fireSound = "Missile";
            editorTooltip = "Shoots a homing missile.";
            _editorName = "Rocket Launcher";

            this._barrelSteam = new SpriteMap("steamPuff", 16, 16);
            this._barrelSteam.center = new Vec2(0.0f, 14f);
            this._barrelSteam.AddAnimation("puff", 0.4f, false, 0, 1, 2, 3, 4, 5, 6, 7);
            this._barrelSteam.SetAnimation("puff");
            this._barrelSteam.speed = 0.0f;
        }

        public override void Update()
        {
            if ((double)this._barrelSteam.speed > 0.0 && this._barrelSteam.finished)
                this._barrelSteam.speed = 0.0f;
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
            if ((double)this._barrelSteam.speed > 0.0)
            {
                this._barrelSteam.alpha = 0.6f;
                this.Draw((Sprite)this._barrelSteam, barrelOffset);
            }
        }

        public override void OnPressAction()
        {
            if (this.ammo > 0)
            {
                --this.ammo;
                if (this.duck != null)
                    RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(this._fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                SFX.Play("missile", pitch: (Rando.Float(0.2f) - 0.1f + this._fireSoundPitch));
                this._barrelSteam.speed = 1f;
                this._barrelSteam.frame = 0;
                this.ApplyKick();
                if (this.receivingPress)
                    return;

                HomingMissile rocket = new HomingMissile(Offset(this.barrelOffset).x, Offset(this.barrelOffset).y, this.duck);

                Level.Add((Thing)rocket);
                this.Fondle((Thing)rocket);
                rocket.clip.Add(this.owner as MaterialThing);

                rocket.hSpeed = this.barrelVector.x * 5f;
                rocket.vSpeed = this.barrelVector.y * 5f;
            }
            else if (this.ammo == 0)
                this.DoAmmoClick();
        }

        public override void Fire()
        {
            // nothing here
        }
    }

    public class HomingMissile : PhysicsObject
    {
        public StateBinding _SpeedStateBinding = new StateBinding("_speed");
        public StateBinding _TimerStateBinding = new StateBinding("_timer");
        public StateBinding _PositStateBinding = new StateBinding("position");
        public StateBinding _AngleStateBinding = new StateBinding("_angle");
        public StateBinding _OwnerStateBinding = new StateBinding("_gunOwner");

        public SpriteMap _sprite;
        public Sprite _target;
        public float _speed;
        public float _timer;
        public Thing _gunOwner;
        public HomingMissile(float xpos, float ypos, Thing ow) : base()
        {
            _gunOwner = ow;
            _target = new Sprite(GetPath("target"));
            _target.CenterOrigin();
            _target.position = new Vec2(float.MaxValue, float.MaxValue);
            _sprite = new SpriteMap(GetPath("rocket"), 21, 10);
            _sprite.AddAnimation("*fly*", 0.5f, true, 0, 1, 2, 3);
            _sprite.SetAnimation("*fly*");
            graphic = _sprite;
            position = new Vec2(xpos, ypos);

            this.center = new Vec2(15f, 5f);
            this.collisionOffset = new Vec2(-15f, -5f);
            this.collisionSize = new Vec2(21f, 10f);

            gravMultiplier = 0f;
            _skipPlatforms = true;
            _speed = 1f;
            _timer = 0;
            friction = 0.1f;

            physicsMaterial = PhysicsMaterial.Metal;
        }

        public override void Update()
        {
            this.angleDegrees = -Maths.PointDirection(Vec2.Zero, new Vec2(this.hSpeed, this.vSpeed));
            Duck duck = Level.Nearest<Duck>(x, y, _gunOwner);

            if (_timer < 0.15f) _timer += 0.01f;
            else if ((duck.position - this.position).length < 2000f)
            {
                if (_speed < 1.3f) _speed += 0.03f;

                var vel = new Vec2(duck.collisionCenter.x - x, duck.collisionCenter.y - y);
                vel.Normalize();

                this._hSpeed += vel.x * _speed * 0.5f;
                this._vSpeed += vel.y * _speed * 0.5f;

                _hSpeed = Maths.Clamp(_hSpeed, -20f, 20f);
                _vSpeed = Maths.Clamp(_vSpeed, -20f, 20f);

                float pitch = -(duck.position - this.position).length/2000f;

                SFX.Play(GetPath("beep"), 0.2f, pitch);
                Level.Add(SmallSmoke.New(x-_hSpeed,y-_vSpeed));

                if (duck != null)
                {
                    _target.position = duck.collisionCenter;
                    _target._angle += 0.05f;
                }
            }
            else
            {
                _hSpeed *= 1.05f;
                Level.Add(SmallSmoke.New(x - _hSpeed, y - _vSpeed));
            }
            base.Update();
        }

        public void ExplodeMissile(MaterialThing with, ImpactedFrom @from)
        {
            ATMissile.DestroyRadius(this.position, 50f, this);
            Level.Add((Thing)new ExplosionPart(x, y));

            List<Bullet> varBullets = new List<Bullet>();
            for (int index = 0; index < 30; ++index)
            {
                float num = (float)((double)index * 30.0 - 10.0) + Rando.Float(20f);
                ATShrapnel atMissileShrapnel = new ATShrapnel();
                atMissileShrapnel.range = 40f + Rando.Float(20f);
                Vec2 vec2 = new Vec2((float)Math.Cos((double)Maths.DegToRad(num)), (float)Math.Sin((double)Maths.DegToRad(num)));
                Bullet bullet = new Bullet(x + vec2.x * 8f, y - vec2.y * 8f, (AmmoType)atMissileShrapnel, num);
                bullet.firedFrom = (Thing)this;
                varBullets.Add(bullet);
                Level.Add((Thing)bullet);
            }
            if (Network.isActive && this.isLocal)
            {
                Send.Message((NetMessage)new NMFireGun((Gun)null, varBullets, (byte)0, false), NetMessagePriority.ReliableOrdered);
                varBullets.Clear();
            }

            SFX.Play("explode");
            Level.Remove(this);
            _destroyed = true;
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom @from)
        {
            if ((with is Duck && (with as Duck) != _gunOwner) || with is Block || with is RagdollPart)
            {
                ExplodeMissile(with, from);
            }

            base.OnImpact(with, @from);
        }

        public override void Draw()
        {
            base.Draw();
            _target.Draw();
        }
    }
}
