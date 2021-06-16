using System;
using System.Linq;
using DuckGame;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Funny")]
    public class XDDDDDDDDDDDD : Gun
    {
        public XDDDDDDDDDDDD(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("piupistol"));

            center = new Vec2(6f, 4.5f);
            collisionSize = new Vec2(12f, 9f);
            collisionOffset = new Vec2(-6f, -4.5f);
            _barrelOffsetTL = new Vec2(12f, 3f);
            _holdOffset = new Vec2(-1f, 1f);
            
            wideBarrel = true;
            _fullAuto = true;
            this._fireWait = 2f;
            ammo = 15;
            _ammoType = (AmmoType)new ATXD();
            _ammoType.sprite = new Sprite(GetPath("XD"));
            _type = "gun";
            _kickForce = 3f;
            loseAccuracy = 0.5f;
            _fireSound = "deepMachineGun";
            editorTooltip = "XDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD";
        }

        public override void Update()
        {
            base.Update();
        }

        public override void OnPressAction()
        {
            Fire();
            base.OnPressAction();
        }

        protected override void PlayFireSound()
        {
            SFX.Play(GetPath("piu"));
        }

        public override void Fire()
        {
            base.Fire();
        }

    }

    public class ATXD : AmmoType
    {
        public ATXD()
        {
            this.accuracy = 0.1f;
            this.penetration = 0.35f;
            this.bulletSpeed = 4f;
            this.rangeVariation = 0.0f;
            this.speedVariation = 0.0f;
            this.range = 300f;
            this.weight = 4f;
            this.bulletThickness = 2f;
            this.immediatelyDeadly = true;
            this.flawlessPipeTravel = true;
            this.bulletType = typeof(XD);
        }
    }

    public class XD : Bullet
    {
        public XD(float xval, float yval, AmmoType type, float ang = -1, Thing owner = null, bool rbound = false, float distance = -1, bool tracer = false, bool network = true) : base(xval, yval, type, ang, owner, rbound, distance, tracer, network)
        {
            graphic = new Sprite(GetPath("xd"));
            ammo.sprite = new Sprite(GetPath("xd"));
            ammo.sprite.CenterOrigin();
        }

        public override void Draw()
        {
            if (this._tracer || (double)this._bulletDistance <= 0.100000001490116)
                return;
            if (this.gravityAffected)
            {
                if (this.prev.Count < 1)
                    return;
                int num = (int)Math.Ceiling(((double)this.drawdist - (double)this.startpoint) / 8.0);
                Vec2 p2 = this.prev.Last<Vec2>();
                for (int index = 0; index < num; ++index)
                {
                    Vec2 pointOnArc = this.GetPointOnArc((float)(index * 8));
                    Graphics.DrawLine(pointOnArc, p2, this.color * (float)(1.0 - (double)index / (double)num) * this.alpha, this.ammo.bulletThickness, (Depth)0.9f);
                    if (pointOnArc == this.prev.First<Vec2>())
                        break;
                    p2 = pointOnArc;
                    if (index == 0 && this.ammo.sprite != null && !this.doneTravelling)
                    {
                        this.ammo.sprite.depth = (Depth)1f;
                        this.ammo.sprite.angleDegrees = -Maths.PointDirection(Vec2.Zero, this.travelDirNormalized);
                        Graphics.Draw(this.ammo.sprite, p2.x, p2.y);
                    }
                }
            }
            else
            {
                if (this.ammo.sprite != null && !this.doneTravelling)
                {
                    this.ammo.sprite.depth = this.depth + 10;
                    this.ammo.sprite.angleDegrees = -Maths.PointDirection(Vec2.Zero, this.travelDirNormalized);
                    Graphics.Draw(this.ammo.sprite, this.drawEnd.x, this.drawEnd.y);
                }
                float length = (this.drawStart - this.drawEnd).length;
                float val = 0.0f;
                float num1 = (float)(1.0 / ((double)length / 8.0));
                float num2 = 1f;
                float num3 = 8f;
                while (true)
                {
                    bool flag = false;
                    if ((double)val + (double)num3 > (double)length)
                    {
                        num3 = length - Maths.Clamp(val, 0.0f, 99f);
                        flag = true;
                    }
                    num2 -= num1;
                    --Graphics.currentDrawIndex;
                    //Graphics.DrawLine(this.drawStart + this.travelDirNormalized * length - this.travelDirNormalized * val, this.drawStart + this.travelDirNormalized * length - this.travelDirNormalized * (val + num3), this.color * num2, this.ammo.bulletThickness, this.depth);
                    if (!flag)
                        val += 8f;
                    else
                        break;
                }
            }
        }
    }
}
