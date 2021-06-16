using DuckGame;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Lasers")]
    public class Destroyer : Gun
    {
        public Destroyer(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("destroyer"));

            center = new Vec2(14f, 6.5f);
            collisionSize = new Vec2(28f, 10f);
            collisionOffset = new Vec2(-14f, -4.5f);
            _barrelOffsetTL = new Vec2(28f, 6f);
            _holdOffset = new Vec2(0f, -1f);

            _kickForce = 10f;
            ammo = 5;
            _fireWait = 7f;
            _type = "gun";
            _ammoType = (AmmoType)new ATDestroyer();
            _fireSound = GetPath("destroyerShot");
            editorTooltip = "Powerful laser, perfect!";

            this.laserSight = true;
            this._laserOffsetTL = new Vec2(21f, 1f);
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
    }

    public class ATDestroyer : AmmoType
    {
        public ATDestroyer()
        {
            this.accuracy = 1f;
            this.range = 500f;
            this.penetration = 9f;
            this.bulletSpeed = 35f;
            this.bulletThickness = 1f;
            this.bulletType = typeof(LaserBulletOrange);
            this.flawlessPipeTravel = true;
        }
    }
}
