using DuckGame;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Lasers")]
    public class TinyLaser : Gun
    {
        public TinyLaser(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("tiny"));

            center = new Vec2(7.5f, 4.5f);
            collisionSize = new Vec2(15f, 9f);
            collisionOffset = new Vec2(-7.5f, -4.5f);
            _barrelOffsetTL = new Vec2(15f, 3f);
            _holdOffset = new Vec2(0f, 0f);
            
            wideBarrel = true;
            _fullAuto = true;
            this._fireWait = 3f;
            ammo = 100;
            _ammoType = (AmmoType)new ATTinyLaser();
            _type = "gun";
            _kickForce = 0f;
            _fireSound = "phaserSmall";
            editorTooltip = "This puny laser won\'t hurt any duck... or will it?";
        }

        public override void Update()
        {
            base.Update();
            if (this.firing)
            {
                if (_kickForce < 5f)  _kickForce += 0.01f;
                if (_fireWait > 0.6f) _fireWait -= 0.01f;
            }
        }

        public override void OnReleaseAction()
        {
            ResetGun();
            base.OnReleaseAction();
        }

        public override void Fire()
        {
            base.Fire();
        }

        public void ResetGun()
        {
            _fireWait = 3f;
            _kickForce = 0f;
        }

    }

    public class ATTinyLaser : AmmoType
    {
        public ATTinyLaser()
        {
            this.accuracy = 0.2f;
            this.range = 100f;
            this.penetration = 0.01f;
            this.bulletSpeed = 10f;
            this.bulletThickness = 0.2f;
            this.bulletType = typeof(LaserBulletPurple);
            this.flawlessPipeTravel = true;
        }
    }
}
