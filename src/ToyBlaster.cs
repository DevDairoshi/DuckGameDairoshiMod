using System;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Misc")]
    public class ToyBlaster : Gun
    {
        public sbyte _loadProgress = 100;
        public float _loadAnimation = 1f;
        protected SpriteMap _loaderSprite;

        public ToyBlaster(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("toyBlaster"));

            center = new Vec2(16f, 8.5f);
            collisionSize = new Vec2(32f, 15f);
            collisionOffset = new Vec2(-16f, -6.5f);
            _barrelOffsetTL = new Vec2(32f, 7f);
            _holdOffset = new Vec2(1f, 0f);

            wideBarrel = true;
            _manualLoad = true;
            ammo = 10;
            _type = "gun";
            _ammoType = (AmmoType)new ATToyBall();
            _ammoType.sprite = new Sprite(GetPath("toyBall"));
            _kickForce = 0f;
            _fireSound = GetPath("toy");
            editorTooltip = "Deez balls...";

            _loaderSprite = new SpriteMap(GetPath("toyLoader"), 10, 5);
            _loaderSprite.center = new Vec2(30f, 12.5f);
        }

        public override void Update()
        {
            base.Update();
            if ((double)this._loadAnimation == -1.0)
            {
                SFX.Play("shotgunLoad");
                this._loadAnimation = 0.0f;
            }
            if ((double)this._loadAnimation >= 0.0)
            {
                if ((double)this._loadAnimation == 0.5 && this.ammo != 0)
                    this.PopShell();
                if ((double)this._loadAnimation < 1.0)
                    this._loadAnimation += 0.1f;
                else
                    this._loadAnimation = 1f;
            }
            if (this._loadProgress < (sbyte)0)
                return;
            if (this._loadProgress == (sbyte)50)
                this.Reload(false);
            if (this._loadProgress < (sbyte)100)
                this._loadProgress += (sbyte)10;
            else
                this._loadProgress = (sbyte)100;
        }

        public override void OnPressAction()
        {
            if (this.loaded)
            {
                _ammoType.barrelAngleDegrees = Rando.Float(-15f, 15f);
                base.OnPressAction();
                this._loadProgress = (sbyte)-1;
                this._loadAnimation = -0.01f;
            }
            else
            {
                if (this._loadProgress != (sbyte)-1)
                    return;
                this._loadProgress = (sbyte)0;
                this._loadAnimation = -1f;
            }
        }

        public override void Draw()
        {
            base.Draw();
            Vec2 vec2 = new Vec2(13f, -2f);
            float num = (float)Math.Sin((double)this._loadAnimation * 3.14000010490417) * 3f;
            this.Draw((Sprite)this._loaderSprite, new Vec2(vec2.x + 8f - num, vec2.y + 3f));
        }
    }

    public class ATToyBall : AmmoType
    {
        public ATToyBall()
        {
            this.accuracy = 1f;
            this.penetration = 0.35f;
            this.bulletSpeed = 5f;
            this.rangeVariation = 1.5f;
            this.speedVariation = 3.0f;
            this.range = 1000f;
            this.rebound = true;
            this.affectedByGravity = true;
            this.deadly = false;
            this.weight = 10f;
            this.bulletThickness = 0f;
            this.bulletType = typeof(ToyBall);
        }
    }

    public class ToyBall : Bullet
    {
        public StateBinding _VolatileStateBinding = new StateBinding("_isVolatile");
        public StateBinding _PositionStateBinding = new CompressedVec2Binding("position");

        private float _isVolatile = 15f;
        public ToyBall(float xval, float yval, AmmoType type, float ang = -1, Thing owner = null, bool rbound = false, float distance = -1, bool tracer = false, bool network = true) : base(xval, yval, type, ang, owner, rbound, distance, tracer, network)
        {
            graphic = new Sprite(GetPath("toyBall"));
            ammo.sprite = new Sprite(GetPath("toyBall"));
            ammo.sprite.CenterOrigin();
        }

        protected override void OnHit(bool destroyed)
        {
            foreach (Duck duck in Level.CheckCircleAll<Duck>(this.position, 30f))
            {
                if (this.owner != null)
                    this.responsibleProfile = this.owner.responsibleProfile;

                SFX.Play(GetPath("bounce"), 1f, Rando.Float(-0.1f, 0.1f));
                this.Fondle(duck);

                //duck.Disarm(this);
                duck.ApplyForce(new Vec2(this.hSpeed * 0.8f, -4f));
                duck.GoRagdoll();
            }
        }

        protected override void Rebound(Vec2 pos, float dir, float rng)
        {
            Level.Add(SmallSmoke.New(pos.x, pos.y));
            ToyBall bullet = this.ammo.GetBullet(pos.x, pos.y, angle: (-dir), firedFrom: this.firedFrom, distance: rng, tracer: this._tracer) as ToyBall;
            bullet._teleporter = this._teleporter;
            bullet._isVolatile = this._isVolatile;
            bullet.isLocal = this.isLocal;
            bullet.lastReboundSource = this.lastReboundSource;
            bullet.connection = this.connection;
            this.reboundCalled = true;
            Level.Add((Thing)bullet);
            SFX.Play(GetPath("bounce"), 1f, Rando.Float(-0.1f, 0.1f));
        }

        public override void Update()
        {
            this._isVolatile -= 0.01f;
            if ((double)this._isVolatile <= 0.0)
                this.rebound = false;
            base.Update();
        }
    }
}
