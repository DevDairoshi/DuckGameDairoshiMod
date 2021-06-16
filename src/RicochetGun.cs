
namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Funny")]
    public class RicochetGun : Gun
    {
        public RicochetGun(float xval, float yval) : base(xval, yval)
        {
            this.ammo = 15;
            this._ammoType = (AmmoType)new ATRicochet();
            this._type = "gun";
            this.graphic = new Sprite(GetPath("ricochet"));

            this.center = new Vec2(10.5f, 6.5f);
            this.collisionOffset = new Vec2(-10.5f, -6.5f);
            this.collisionSize = new Vec2(21f, 13f);
            this._barrelOffsetTL = new Vec2(21f, 2f);
            this._holdOffset = new Vec2(0f, 0f); 

            _fireWait = 2f;
            this._fireSound = "smg";
            this._kickForce = 1f;
            this._fireRumble = RumbleIntensity.Light;
            this._holdOffset = new Vec2(1f, 2f);
            this.handOffset = new Vec2(0.0f, 1f);
            this.editorTooltip = "Use with caution.";
        }
    }

    public class ATRicochet : AmmoType
    {
        public ATRicochet()
        {
            this.accuracy = 0.1f;
            this.range = 5000f;
            this.rebound = true;
            this.penetration = 1f;
            this.combustable = true;
            this.bulletSpeed = 20f;
            this.bulletColor = Color.Red;
            this.bulletThickness = 2f;
        }

        public override void PopShell(float x, float y, int dir)
        {
            PistolShell pistolShell = new PistolShell(x, y);
            pistolShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)pistolShell);
        }
    }
}
