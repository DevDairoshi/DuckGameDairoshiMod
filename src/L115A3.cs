namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Guns")]
    public class L115A3 : Sharpshot
    {
        public L115A3(float xval, float yval) : base(xval, yval)
        {
            this.ammo = 8;
            this._ammoType = (AmmoType)new ATSniperOP();
            this.wideBarrel = true;
            this._type = "gun";
            this.graphic = new Sprite(GetPath("l115a3"));

            this.center = new Vec2(22.5f, 9.5f);
            this.collisionOffset = new Vec2(-22.5f, -3.5f);
            this.collisionSize = new Vec2(55f, 12f);
            this._barrelOffsetTL = new Vec2(24f, 1f);
            this._holdOffset = new Vec2(6f, -1f);

            this._fireWait = 3f;
            this._kickForce = 12f;
            this._fireRumble = RumbleIntensity.Kick;
            this._fireSound = GetPath("sniper");
            this.editorTooltip = "Penetrates everything!";
            this.physicsMaterial = PhysicsMaterial.Metal;

            this._laserOffsetTL = new Vec2(53f, 7f);
        }
    }

    public class ATSniperOP : AmmoType
    {
        public ATSniperOP()
        {
            this.combustable = true;
            this.range = 2000f;
            this.accuracy = 1f;
            this.penetration = 999f;
            this.bulletSpeed = 96f;
            this.impactPower = 10f;
            this.bulletThickness = 3f;
            this.bulletColor = Color.Red;
        }

        public override void PopShell(float x, float y, int dir)
        {
            SniperShell sniperShell = new SniperShell(x, y);
            sniperShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)sniperShell);
        }
    }
}
