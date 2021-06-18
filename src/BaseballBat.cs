using System;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Melee")]
    public class BaseballBat : Gun
    {
        public StateBinding _BatStateBinding = new StateBinding("state");
        public StateBinding _HandAngleStateBinding = new StateBinding("handAngle");
        public StateBinding _SavedDuckStateBinding = new StateBinding("_savedDuck");
        public StateBinding _StartedAngStateBinding = new StateBinding("startingAng");
        public StateBinding _AngStateBinding = new StateBinding("ang");
        public StateBinding _BarrelStateBinding = new StateBinding("barrelOffset");
        public StateBinding _PositionStateBinding = new StateBinding("position");
        public StateBinding _ColldownStateBinding = new StateBinding("cooldown");

        public NetSoundBinding _BonkNetSoundBinding = new NetSoundBinding("_bonk");
        public NetSoundBinding _SwipeNetSoundBinding = new NetSoundBinding("_swipe");

        public NetSoundEffect _bonk = new NetSoundEffect(new string[1] { Mod.GetPath<DairoshiMod>("bonk") });
        public NetSoundEffect _swipe = new NetSoundEffect(new string[1] { "swipe" });

        public Duck _savedDuck;
        public int state;
        public Sprite sprite;
        public float startingAng;
        public float ang;
        public float cooldown;

        public BaseballBat(float xval, float yval) : base(xval, yval)
        {
            sprite = new Sprite(GetPath("bat"));
            graphic = sprite;

            center = new Vec2(19f, 2.5f);
            collisionSize = new Vec2(38f, 5f);
            collisionOffset = new Vec2(-19f, -2.5f);
            _barrelOffsetTL = new Vec2(28f, 2.5f);

            _holdOffset = new Vec2(9f, 3f);
            handOffset = new Vec2(-1f, -2f);

            _type = "gun";
            _hasTrigger = false;
            ammo = 1;
            weight = 2f;
            thickness = 5f;
            depth = -0.5f;
            
            state = 0;
            physicsMaterial = PhysicsMaterial.Wood;
            cooldown = 0f;
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (!(type is DTIncinerate))
                base.OnDestroy(type);
            else
            {
                Level.Remove(this);
                for (int index = 0; index < 8; ++index)
                {
                    Thing t = WoodDebris.New(this.x - 8f + Rando.Float(16f), this.y - 8f + Rando.Float(16f));
                    t.hSpeed = ((Rando.Float(1f) > 0.5f ? 1f : -1f) * Rando.Float(3f));
                    t.vSpeed = -Rando.Float(1f);
                    Level.Add(t);
                }
                return true;
            }
            return false;
        }

        public override void Update()
        {
            base.Update();

            if (this.duck != null)
            {
                _savedDuck = duck;
                handAngle = -3 * offDir * (float) Math.PI / 4;

                if (state == 1 || state == 2)
                {
                    this.duck.immobilized = true;
                }
                else
                {
                    this.duck.immobilized = false;
                }
            }
            else
            {
                _savedDuck = null;
            }

            if (cooldown > 0f)
            {
                cooldown -= 0.01f;
            }
            else
            {
                cooldown = 0f;
            }


            if (state == -1 && !receivingPress)
            {
                state = 0;
            }

            if (state == 1)
            {
                ang -= offDir * 0.6f;
                handAngle = ang;

                if (handAngle < startingAng - 2 * Math.PI || handAngle > startingAng + 2 * Math.PI)
                {
                    handAngle = -3 * offDir * (float)Math.PI / 4;
                    state = -1;
                    return;
                }

                foreach (Thing t in Level.CheckCircleAll<Thing>(Offset(barrelOffset), 10f))
                {
                    if (t == _savedDuck)
                        continue;

                    if (t is RagdollPart)
                    {
                        _bonk.Play(1, Rando.Float(-0.2f, 0.2f));

                        this.Fondle((t as RagdollPart));
                        t.ApplyForce(RandomForce(offDir));
                        state = 2;
                    }
                    else if (t is Duck)
                    {
                        _bonk.Play(1, Rando.Float(-0.2f, 0.2f));

                        this.Fondle(t);

                        //(t as Duck).Disarm(this);
                        t.ApplyForce(RandomForce(offDir));
                        (t as Duck).GoRagdoll();
                        state = 2;
                    }
                    else if (t is Baseball)
                    {
                        _bonk.Play(1, Rando.Float(-0.2f, 0.2f));
                        
                        this.Fondle(t);
                        t.ApplyForce(RandomForce(offDir));
                        state = 2;
                    }
                }
            }
            else if (state == 2)
            {
                ang += offDir * 0.3f;
                handAngle = ang;
                cooldown = 0.1f;

                if (Math.Abs(handAngle) - Math.Abs(startingAng) < 0.4f)
                {
                    handAngle = -3 * offDir * (float)Math.PI / 4;
                    state = -1;
                    return;
                }
            }
        }

        public override void OnPressAction()
        {
            if (duck != null && state == 0 && cooldown == 0.0f)
            {
                state = 1;
                cooldown = 0.2f;
                startingAng = handAngle;
                ang = handAngle;
                _swipe.Play(1f, Rando.Float(-0.2f, 0.2f));
                BananaSlip slip = new BananaSlip(this.duck.collisionCenter.x + offDir * 20, this.duck.collisionCenter.y + 9f, offDir > 0);
                Level.Add(slip);
                this.Fondle(slip);
            }
        }

        public override void OnReleaseAction()
        {
            if (state == -1)
            {
                cooldown = 0f;
            }
        }

        public override void CheckIfHoldObstructed()
        {
            if (owner == null)
                return;
            duck.holdObstructed = false;
        }

        public override void Thrown()
        {
            state = 0;
            handAngle = 0f;
            this.duck.immobilized = false;
            base.Thrown();
        }

        public Vec2 RandomForce(sbyte dir)
        {
            return new Vec2(dir * Rando.Float(2f,8f), Rando.Float(-16f, -6f));
        }
    }
}
