using DuckGame;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Guns")]
    public class MicroGun : Gun
    {
        public MicroGun(float xval, float yval) : base(xval, yval)
        {
            this.ammo = 6;
            this._ammoType = (AmmoType)new ATMicro();
            this._type = "gun";
            this.graphic = new Sprite(GetPath("micro"));

            this.center = new Vec2(5f, 3f);
            this.collisionOffset = new Vec2(-5f, -3f);
            this.collisionSize = new Vec2(10f, 6f);
            this._barrelOffsetTL = new Vec2(10f, 1f);
            this._holdOffset = new Vec2(0f, 0f);
            
            _fireWait = 1f;
            this._fireSound = "littleGun";
            this._kickForce = 0f;
            this.editorTooltip = "Finally, a gun bigger than my dick XD";
        }
    }

    public class ATMicro : AmmoType
    {
        public ATMicro()
        {
            this.accuracy = 0.85f;
            this.range = 15f;
            this.rangeVariation = 0f;
            this.penetration = 0.1f;
            this.combustable = true;
        }

        public override void PopShell(float x, float y, int dir)
        {
            PistolShell pistolShell = new PistolShell(x, y);
            pistolShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)pistolShell);
        }
    }
}
