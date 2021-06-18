using System.Collections.Generic;
using DuckGame;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Misc")]
    public class Bouncer : Gun
    {
        public Bouncer(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("bouncer"));
            
            center = new Vec2(10.5f, 7f);
            collisionSize = new Vec2(21f, 12f);
            collisionOffset = new Vec2(-10.5f, -5f);
            _barrelOffsetTL = new Vec2(21f, 5f);
            _holdOffset = new Vec2(1f, 2f);


            wideBarrel = true;
            ammo = 6;
            _type = "gun";
            _ammoType = (AmmoType)new ATBouncingGrenade();
            _ammoType.sprite = new Sprite(GetPath("bbullet"));
            _kickForce = 1f;
            _fireSound = "deepMachineGun";
            editorTooltip = "The bullet gonna bounce for quite a long time...";
        }

        public override void Fire()
        {
            _ammoType.barrelAngleDegrees = Rando.Float(-15f, 15f);
            base.Fire();
        }
    }
    
    public class ATBouncingGrenade : AmmoType
    {
        public ATBouncingGrenade()
        {
            this.accuracy = 1f;
            this.penetration = 0.35f;
            this.bulletSpeed = 5f;
            this.rangeVariation = 0.0f;
            this.speedVariation = 1.0f;
            this.range = 2000f;
            this.rebound = true;
            this.affectedByGravity = true;
            this.deadly = false;
            this.weight = 4f;
            this.ownerSafety = 4;
            this.bulletThickness = 5f;
            this.bulletColor = Color.Red;
            this.bulletType = typeof(BouncingBullet);
            this.immediatelyDeadly = true;
            this.flawlessPipeTravel = true;
        }

        public override void PopShell(float x, float y, int dir)
        {
            PistolShell pistolShell = new PistolShell(x, y);
            pistolShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)pistolShell);
        }
    }

    public class BouncingBullet : Bullet
    {
        private float _isVolatile = 10f;
        public BouncingBullet(float xval, float yval, AmmoType type, float ang = -1, Thing owner = null, bool rbound = false, float distance = -1, bool tracer = false, bool network = true) : base(xval, yval, type, ang, owner, rbound, distance, tracer, network)
        {
            graphic = new Sprite(GetPath("bbullet"));
            ammo.sprite = new Sprite(GetPath("bbullet"));
            ammo.sprite.CenterOrigin();
        }

        protected override void OnHit(bool destroyed)
        {
            if (!destroyed || !this.isLocal)
                return;
            for (int index = 0; index < 1; ++index)
            {
                ExplosionPart explosionPart = new ExplosionPart(this.x - 8f + Rando.Float(16f), this.y - 8f + Rando.Float(16f));
                explosionPart.xscale *= 0.7f;
                explosionPart.yscale *= 0.7f;
                Level.Add((Thing)explosionPart);
            }
            SFX.Play("explode");
            RumbleManager.AddRumbleEvent(this.position, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium));
            foreach (MaterialThing materialThing in Level.CheckCircleAll<TV>(this.position, 20f))
                materialThing.Destroy((DestroyType)new DTImpact((Thing)this));
            List<Bullet> varBullets = new List<Bullet>();
            Vec2 vec2 = this.position - this.travelDirNormalized;
            for (int index = 0; index < 12; ++index)
            {
                float ang = (float)((double)index * 30.0 - 10.0) + Rando.Float(20f);
                ATGrenadeLauncherShrapnel launcherShrapnel = new ATGrenadeLauncherShrapnel();
                launcherShrapnel.range = 25f + Rando.Float(10f);
                Bullet bullet = new Bullet(vec2.x, vec2.y, (AmmoType)launcherShrapnel, ang);
                bullet.firedFrom = (Thing)this;
                varBullets.Add(bullet);
                Level.Add((Thing)bullet);
            }
            if (Network.isActive && this.isLocal)
            {
                Send.Message((NetMessage)new NMFireGun((Gun)null, varBullets, (byte)0, false), NetMessagePriority.ReliableOrdered);
                varBullets.Clear();
            }
            foreach (Window window in Level.CheckCircleAll<Window>(this.position, 20f))
            {
                if (Level.CheckLine<Block>(this.position, window.position, (Thing)window) == null)
                    window.Destroy((DestroyType)new DTImpact((Thing)this));
            }
        }

        protected override void Rebound(Vec2 pos, float dir, float rng)
        {
            BouncingBullet bullet = this.ammo.GetBullet(pos.x, pos.y, angle: (-dir), firedFrom: this.firedFrom, distance: rng, tracer: this._tracer) as BouncingBullet;
            bullet._teleporter = this._teleporter;
            bullet._isVolatile = this._isVolatile;
            bullet.isLocal = this.isLocal;
            bullet.lastReboundSource = this.lastReboundSource;
            bullet.connection = this.connection;
            this.reboundCalled = true;
            Level.Add((Thing)bullet);
            SFX.Play("grenadeBounce", 0.8f, Rando.Float(-0.1f, 0.1f));
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
