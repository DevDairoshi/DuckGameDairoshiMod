namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Misc")]
    public class Trapper : Gun
    {
        private SpriteMap _barrelSteam;
        public Trapper(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("trapper"));

            this.center = new Vec2(15f, 5f);
            this.collisionOffset = new Vec2(-15f, -4f);
            this.collisionSize = new Vec2(30f, 10f);
            this._barrelOffsetTL = new Vec2(29f, 4f);
            this._holdOffset = new Vec2(-2f, -2f);

            this._fireRumble = RumbleIntensity.Light;
            wideBarrel = true;
            _kickForce = 2f;
            _fireWait = 5f;
            _weight = 6f;
            ammo = 5;
            _type = "gun";
            _ammoType = (AmmoType)new AT9mm();
            _fireSound = "campingThwoom";
            editorTooltip = "Spreads some nasty traps.";

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
                SFX.Play("campingThwoom", pitch: (Rando.Float(0.2f) - 0.1f + this._fireSoundPitch));
                this._barrelSteam.speed = 1f;
                this._barrelSteam.frame = 0;
                this.ApplyKick();
                if (this.receivingPress)
                    return;

                SpikeBall ball = new SpikeBall();
                ball.position = Offset(this.barrelOffset);
                
                Level.Add((Thing)ball);
                this.Fondle((Thing)ball);
                ball.clip.Add(this.owner as MaterialThing);

                ball.hSpeed = this.barrelVector.x * Rando.Float(2f, 5f);
                ball.vSpeed = this.barrelVector.y * Rando.Float(2f, 5f) - 0.5f;
            }
            else if (this.ammo == 0)
                this.DoAmmoClick();
        }

        public override void Fire()
        {
            // nothing here
        }
    }

    [EditorGroup("DairoshiMod|Environment")]
    public class SpikeBall : PhysicsObject, IPlatform
    {
        public SpikeBall() : base()
        {
            graphic = new Sprite(GetPath("spikeball"));

            this.center = new Vec2(7.5f, 7.5f);
            this.collisionOffset = new Vec2(-7.5f, -7.5f);
            this.collisionSize = new Vec2(15f, 15f);
            this.collideSounds.Add("rockHitGround2");
            physicsMaterial = PhysicsMaterial.Metal;
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom @from)
        {
            if (with is Duck)
            {
                (with as Duck).Kill(new DTImpale(this));
            }

            if (with is RagdollPart)
            {
                (with as RagdollPart)._doll._duck.Kill(new DTImpale(this));
            }


            base.OnImpact(with, @from);
        }
    }
}
