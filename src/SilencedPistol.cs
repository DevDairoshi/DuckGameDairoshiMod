namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Guns")]
    public class SilencedPistol : Gun
    {
        public SpriteMap _sprite;
        public SilencedPistol(float xval, float yval) : base(xval, yval)
        {
            this.ammo = 12;
            this._ammoType = (AmmoType)new AT9mmBoosted();
            this.wideBarrel = true;
            //this.barrelInsertOffset = new Vec2(0.0f, -1f);
            this._type = "gun";
            this._sprite = new SpriteMap(GetPath("silencedPistol"), 28, 10);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(14f, 5f);
            this.collisionOffset = new Vec2(-14f, -5f);
            this.collisionSize = new Vec2(28f, 10f);
            this._barrelOffsetTL = new Vec2(28f, 2f);
            this._holdOffset = new Vec2(6f, 2f);

            this._kickForce = 1f;
            this._fireRumble = RumbleIntensity.Kick;
            this.loseAccuracy = 0.1f;
            this.maxAccuracyLost = 0.6f;
            this._fireSound = GetPath("silenced");
            this._bio = "For some kind of assassin ducks...";
            this.physicsMaterial = PhysicsMaterial.Metal;
        }

        public override void Update()
        {
            if (this._sprite.currentAnimation == "fire" && this._sprite.finished)
                this._sprite.SetAnimation("idle");
            base.Update();
        }

        public override void OnPressAction()
        {
            if (this.ammo > 0)
            {
                for (int index = 0; index < 3; ++index)
                {
                    Vec2 vec2 = this.Offset(new Vec2(-9f, 0.0f));
                    Vec2 hitAngle = this.barrelVector.Rotate(Rando.Float(1f), Vec2.Zero);
                    Level.Add((Thing)Spark.New(vec2.x, vec2.y, hitAngle, 0.1f));
                }
            }
            else
                this._sprite.frame = 1;
            this.Fire();
        }
    }

    public class AT9mmBoosted : AmmoType
    {
        public AT9mmBoosted()
        {
            this.accuracy = 0.8f;
            this.range = 300f;
            this.penetration = 1f;
            this.combustable = true;
            this.bulletSpeed = 30f;
        }

        public override void PopShell(float x, float y, int dir)
        {
            PistolShell pistolShell = new PistolShell(x, y);
            pistolShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)pistolShell);
        }
    }
}
