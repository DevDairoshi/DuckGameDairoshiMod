using DuckGame;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Guns")]
    public class Magnum69420 : Magnum
    {
        public Magnum69420(float xval, float yval) : base(xval, yval)
        {
            this.ammo = 6;
            this._ammoType = (AmmoType)new ATMagnum69420();
            this._type = "gun";
            this.graphic = new Sprite(GetPath("magnum69420"));
            
            this.center = new Vec2(17f, 7.5f);
            this.collisionOffset = new Vec2(-17f, -6.5f);
            this.collisionSize = new Vec2(34f, 12f);
            this._barrelOffsetTL = new Vec2(34f, 2f);
            this._holdOffset = new Vec2(9f, 1f);

            _fireWait = 1f;
            this._fireSound = GetPath("magnum");
            this._kickForce = 7f;
            this._fireRumble = RumbleIntensity.Light;
            this.handOffset = new Vec2(0.0f, 1f);
            this.editorTooltip = "New model of well known Magnum, 69x420, deadly w chuj";
        }
    }

    public class ATMagnum69420 : AmmoType
    {
        public float angle;

        public ATMagnum69420()
        {
            this.accuracy = 1f;
            this.range = 400f;
            this.penetration = 4f;
            this.bulletSpeed = 48f;
            this.combustable = true;
            this.bulletThickness = 4f;
        }

        public override void PopShell(float x, float y, int dir)
        {
            MagnumShell magnumShell = new MagnumShell(x, y);
            magnumShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)magnumShell);
        }
    }
}
