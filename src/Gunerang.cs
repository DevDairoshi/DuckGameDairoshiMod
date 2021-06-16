using DuckGame;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Misc")]
    public class Gunerang : Pistol
    {
        private SpriteMap _barrelSteam;
        public Gunerang(float xval, float yval)
            : base(xval, yval)
        {
            this.ammo = 6;
            this._ammoType = (AmmoType)new AT9mm();
            this.wideBarrel = true;
            this.barrelInsertOffset = new Vec2(0.0f, -1f);
            this._type = "gun";
            this.graphic = new Sprite(GetPath("gunerang"));
            
            this.center = new Vec2(11.5f, 5f);
            this.collisionOffset = new Vec2(-11.5f, -5f);
            this.collisionSize = new Vec2(23f, 10f);
            this._barrelOffsetTL = new Vec2(23f, 3f);
            this._holdOffset = new Vec2(4f, 1f);
            
            this._editorName = "Gunerang";
            this._fireWait = 3f;
            this._fireSound = "magnum";
            this._kickForce = 2f;
            this._fireRumble = RumbleIntensity.Kick;
            this.editorTooltip = "You get a bullet, you get a bullet... and I get a bullet?";
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
                SFX.Play(GetPath("gunerangShot"), pitch: (Rando.Float(0.2f) - 0.1f + this._fireSoundPitch));
                this._barrelSteam.speed = 1f;
                this._barrelSteam.frame = 0;
                this.ApplyKick();
                if (this.receivingPress)
                    return;

                Bullerang bullet = new Bullerang();
                bullet.position = Offset(this.barrelOffset);
                Level.Add((Thing)bullet);
                this.Fondle((Thing)bullet);

                bullet.clip.Add(this.owner as MaterialThing);
                bullet.hSpeed = this.barrelVector.x * 15f;
                bullet.vSpeed = this.barrelVector.y * 15f;
                bullet.SetStartingSpeed(bullet.hSpeed, bullet.vSpeed);
            }
            else if (this.ammo == 0)
                this.DoAmmoClick();
        }

        public override void Fire()
        {
            // nothing here

        }
    }

    public class Bullerang : PhysicsObject
    {
        public StateBinding _xSpeedStateBinding = new StateBinding("_hSpeed");
        public StateBinding _ySpeedStateBinding = new StateBinding("_vSpeed");

        public Bullerang() : base()
        {
            graphic = new Sprite(GetPath("bullerang"));

            this.center = new Vec2(4f, 4f);
            this.collisionOffset = new Vec2(-4f, -4f);
            this.collisionSize = new Vec2(8f, 8f);
            gravMultiplier = 0f;
            this._skipPlatforms = true;
            this._skipAutoPlatforms = true;

            physicsMaterial = PhysicsMaterial.Metal;
        }

        public override void Update()
        {
            _hSpeed -= _xSpeedStart * 0.03f;
            _vSpeed -= _ySpeedStart * 0.03f;
            base.Update();
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom @from)
        {
            if (with.GetType() == typeof(Duck))
            {
                (with as Duck).Kill(new DTImpale(this));
                Level.Remove(this);
                _destroyed = true;
            }
            if (with is RagdollPart)
            {
                (with as RagdollPart)._doll._duck.Kill(new DTImpale(this));
                Level.Remove(this);
                _destroyed = true;
            }

            if (with is Block)
            {
                Level.Add(SmallSmoke.New(x, y));
                Level.Remove(this);
                _destroyed = true;
            }

            base.OnImpact(with, @from);
        }

        public void SetStartingSpeed(float sx, float sy)
        {
            _xSpeedStart = sx;
            _ySpeedStart = sy;
        }

        public override void Draw()
        {
            base.Draw();
        }

        private float _xSpeedStart;
        private float _ySpeedStart;
    }
}
