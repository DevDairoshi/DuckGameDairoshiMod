namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Misc")]
    public class Blowgun : Gun
    {
        public Blowgun(float xval, float yval) : base(xval, yval)
        {
            this.ammo = 10;
            this._ammoType = (AmmoType)new ATPoisonDart();
            this.wideBarrel = true;
            this._type = "gun";
            this.graphic = new Sprite(GetPath("pipe"));

            this.center = new Vec2(20.5f, 3f);
            this.collisionOffset = new Vec2(-20.5f, -3f);
            this.collisionSize = new Vec2(41f, 6f);
            this._barrelOffsetTL = new Vec2(41f, 2f);
            this._holdOffset = new Vec2(15f, -2f); 
            this.handOffset = new Vec2(2f, -4f);

            this._fireWait = 3f;
            this._kickForce = 0f;
            this._fireSound = GetPath("pipeShot");
            this._clickSound = "";
            this.editorTooltip = "Ugabuga, paralyze the pray! Too much of poison and death can occur just like that.";
        }

        public new void DoAmmoClick()
        {

        }
    }

    public class ATPoisonDart : AmmoType
    {
        public ATPoisonDart()
        {
            affectedByGravity = true;
            deadly = false;
            range = 1000f;
            accuracy = 0.9f;
            bulletSpeed = 12f;
            gravityMultiplier = 0.3f;
            bulletColor = Color.GreenYellow;
            weight = 0.1f;
            bulletThickness = 0f;
            penetration = 0.35f;
            bulletType = typeof(PoisonDart);
        }
    }

    public class PoisonDart : Bullet
    {
        public PoisonDart(float xval, float yval, AmmoType type, float ang = -1, Thing owner = null, bool rbound = false, float distance = -1, bool tracer = false, bool network = true) : base(xval, yval, type, ang, owner, rbound, distance, tracer, network)
        {
            graphic = new Sprite(GetPath("dart"));
            ammo.sprite = new Sprite(GetPath("dart"));
            ammo.sprite.CenterOrigin();
        }

        protected override void OnHit(bool destroyed)
        {
            foreach (Duck duck in Level.CheckCircleAll<Duck>(this.position, 20f))
            {
                if (this.owner != null)
                    this.responsibleProfile = this.owner.responsibleProfile;

                Level.Add(SmallSmoke.New(x,y));

                this.Fondle(duck);
                duck.Swear();
                duck.runMax -= 0.9f;
                if (duck.runMax < 0f)
                {
                    duck.Kill(new DTImpale(this));
                }
                duck.GoRagdoll();
            }

            foreach (RagdollPart part in Level.CheckCircleAll<RagdollPart>(this.position, 20f))
            {
                if (part._doll._duck.dead) continue;

                if (this.owner != null)
                    this.responsibleProfile = this.owner.responsibleProfile;

                Level.Add(SmallSmoke.New(x, y));

                this.Fondle(part);
                part._doll._duck.Swear();
                part._doll._duck.runMax -= 0.9f;
                if (part._doll._duck.runMax < 0f)
                {
                    part._doll._duck.Kill(new DTImpale(this));
                }
            }
        }
    }
}
