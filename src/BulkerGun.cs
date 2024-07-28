using System.Collections.Generic;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Lasers")]
    public class BulkerGun : Gun
    {
        private SpriteMap _barrelSteam;

        public BulkerGun(float xval, float yval) : base(xval, yval)
        {
            this.ammo = 6;
            this._ammoType = (AmmoType)new ATDefault();
            this.wideBarrel = true;
            this._type = "gun";
            this.graphic = new Sprite(GetPath("bulker"));

            this.center = new Vec2(10.5f, 6.5f);
            this.collisionOffset = new Vec2(-10.5f, -6.5f);
            this.collisionSize = new Vec2(21f, 13f);
            this._barrelOffsetTL = new Vec2(21f, 3f);
            this._holdOffset = new Vec2(-0.5f, -0.5f);

            this._editorName = "Bulker Gun";
            this._fireWait = 4f;
            this._fireSound = GetPath("bulk");
            this._kickForce = 3f;
            this._fireRumble = RumbleIntensity.Kick;
            this.editorTooltip = "You cannot run and You cannot hide";
            this.physicsMaterial = PhysicsMaterial.Metal;

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
            if (this.ammo > 0 && _wait == 0f)
            {
                --this.ammo;
                _wait = _fireWait;
                if (this.duck != null)
                    RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(this._fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                SFX.Play(GetPath("bulk"), pitch: (Rando.Float(0.2f) - 0.1f + this._fireSoundPitch));
                this._barrelSteam.speed = 1f;
                this._barrelSteam.frame = 0;
                this.ApplyKick();
                if (this.receivingPress)
                    return;

                BulkyBullet bullet = new BulkyBullet();
                bullet.position = Offset(this.barrelOffset);
                Level.Add((Thing)bullet);
                this.Fondle((Thing)bullet);

                bullet.clip.Add(this.owner as MaterialThing);
                bullet.hSpeed = this.barrelVector.x * 10f;
                bullet.vSpeed = this.barrelVector.y * 10f;
            }
            else if (this.ammo == 0)
                this.DoAmmoClick();
        }
    }

    public class BulkyBullet : PhysicsObject
    {
        private float _timer;
        public BulkyBullet() : base()
        {
            graphic = new Sprite(GetPath("bulkBall"));
            this.center = new Vec2(4f, 4f);
            this.collisionOffset = new Vec2(-4f, -4f);
            this.collisionSize = new Vec2(8f, 8f);
            this.gravMultiplier = 0f;
            this.airFrictionMult = 0;
            this._skipPlatforms = true;
            this._skipAutoPlatforms = true;
            physicsMaterial = PhysicsMaterial.Metal;

            _timer = 0.2f;
        }

        public override void Update()
        {
            _timer -= 0.01f;
            if (_timer < 0)
            {
                Explode();
            }
            base.Update();
        }

        public void Explode()
        {
            for (int i = 0; i < 36; i++)
            {
                ATRedLaser at = new ATRedLaser();
                at.range = 50f;
                RedLaser laser = new RedLaser(x, y, at, 10f * i);
                laser.firedFrom = this;
                Level.Add(laser);
            }
            foreach (Window window in Level.CheckCircleAll<Window>(this.position, 40f))
            {
                if (Level.CheckLine<Block>(this.position, window.position, (Thing)window) == null)
                    window.Destroy((DestroyType)new DTImpact((Thing)this));
            }
           
            for (int i = 0; i < 5; i++)
            {
                SFX.Play(GetPath("redShot"), 1, Rando.Float(-0.4f, 0.4f));
            }

            SFX.Play("explode", 0.3f);
            Level.Add(new ExplosionPart(x,y));
            this._destroyed = true;
            Level.Remove((Thing)this);
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom @from)
        {
            if (with.GetType() == typeof(Duck))
            {
                (with as Duck).Kill(new DTImpale(this));
                Explode();
                Level.Remove(this);
                _destroyed = true;
            }
            if (with is RagdollPart)
            {
                (with as RagdollPart)._doll._duck.Kill(new DTImpale(this));
                Explode();
                Level.Remove(this);
                _destroyed = true;
            }

            if (with is Block || with is Spikes)
            {
                Level.Add(SmallSmoke.New(x, y));
                Explode();
                Level.Remove(this);
                _destroyed = true;
            }

            base.OnImpact(with, @from);
        }
    }
}
