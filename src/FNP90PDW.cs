using DuckGame;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Guns")]
    public class FNP90PDW : Gun
    {
        public FNP90PDW(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("fnp90pdw"));

            center = new Vec2(15f, 6.5f);
            collisionSize = new Vec2(30f, 11f);
            collisionOffset = new Vec2(-15f, -4.5f);
            _barrelOffsetTL = new Vec2(30f, 5f);
            _holdOffset = new Vec2(-7f, 0f);

            wideBarrel = true;
            _fullAuto = true;
            this._fireWait = 0.8f;
            ammo = 30;
            _ammoType = (AmmoType)new ATpdw();
            _type = "gun";
            _kickForce = 1f;
            _fireSound = "smg";
            editorTooltip = "Powerful modern machine gun.";

            this.laserSight = true;
            this._laserOffsetTL = new Vec2(24f, 2f);
        }

        public override void Update()
        {
            base.Update();
            if (this.owner != null && ammo > 0)
            {
                this.laserSight = true;
            }
            else
            {
                this.laserSight = false;
            }

        }

        public override void OnPressAction()
        {
            Fire();
            base.OnPressAction();
        }

        protected override void PlayFireSound()
        {
            SFX.Play(GetPath("pdwShot"), 1, Rando.Float(0.2f) - 0.1f);
        }
    }

    public class ATpdw : AmmoType
    {
        public ATpdw()
        {
            this.accuracy = 0.95f;
            this.range = 400f;
            this.penetration = 1.5f;
            this.combustable = true;
            this.bulletThickness = 2f;
            this.bulletSpeed = 20f;
        }

        public override void PopShell(float x, float y, int dir)
        {
            SniperShell sniperShell = new SniperShell(x, y);
            sniperShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)sniperShell);
        }
    }
}
