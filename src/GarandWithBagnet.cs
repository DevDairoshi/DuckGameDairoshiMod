namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Guns")]
    public class GarandWithBagnet : Musket
    {
        public GarandWithBagnet(float xval, float yval) : base(xval, yval)
        {
            this.ammo = 99;
            this._ammoType = (AmmoType)new ATGarand();
            this._type = "gun";
            this.graphic = new Sprite(GetPath("bagnet"));
            this.center = new Vec2(18f, 7f);
            this.collisionOffset = new Vec2(-18f, -7f);
            this.collisionSize = new Vec2(40f, 11f);
            this._barrelOffsetTL = new Vec2(50f, 1f);
            this._fireSound = "shotgun";
            this._kickForce = 2f;
            this._fireRumble = RumbleIntensity.Light;
            this._holdOffset = new Vec2(0f, 1f);
            this.editorTooltip = "Rifle older that Your father, it has attached bagnet, use it if You dont want to wait over 100 years to reload...";
        }

        public override void Update()
        {
            base.Update();

            if (this.duck != null)
            {
                foreach (PhysicsObject p in Level.CheckCircleAll<PhysicsObject>(Offset(new Vec2(barrelOffset.x + 10, barrelOffset.y)), 5f))
                {
                    if (p == this.duck)
                        continue;
                    if (p is Duck)
                    {
                        (p as Duck).Kill(new DTImpale(this));
                    }
                    else if (p is RagdollPart)
                    {
                        (p as RagdollPart)._doll._duck.Kill(new DTImpale(this));
                    }
                }
            }
        }
    }

    public class ATGarand : AmmoType
    {
        public ATGarand()
        {
            this.range = 470f;
            this.rangeVariation = 70f;
            this.accuracy = 0.8f;
            this.bulletThickness = 1.6f;
            this.bulletSpeed = 20f;
        }
    }
}
