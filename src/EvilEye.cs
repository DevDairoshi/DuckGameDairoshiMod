using System.Collections.Generic;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Environment")]
    [BaggedProperty("canSpawn", false)]
    public class EvilEye : PhysicsObject
    {
        public StateBinding _AngleStateBinding = new StateBinding("_angle");
        public StateBinding _SpeeedStateBinding = new StateBinding("_speed");

        public SpriteMap _sprite;
        public float _speed;
        protected bool followCamAdded;
        public EvilEye(float xpos, float ypos) : base(xpos, ypos)
        {
            _sprite = new SpriteMap(GetPath("eye"), 31, 21);
            _sprite.AddAnimation("*move*", 0.5f, true, 0, 1, 2, 3);
            _sprite.SetAnimation("*move*");
            graphic = _sprite;

            center = new Vec2(20.5f, 10.5f);
            collisionSize = new Vec2(21f, 21f);
            collisionOffset = new Vec2(-10.5f, -10.5f);

            _skipPlatforms = true;
            _skipAutoPlatforms = true;
            gravMultiplier = 0f;
            _speed = 0.5f;
        }

        public override void Terminate()
        {
            FollowCam followCam = Level.current.camera as FollowCam;
            if (followCam != null)
                followCam.Remove(this);

            base.Terminate();
        }

        public override void Update()
        {
            base.Update();

            if (!followCamAdded)
            {
                FollowCam followCam = Level.current.camera as FollowCam;
                if (followCam != null)
                {
                    followCam.Add(this);
                    followCamAdded = true;
                }
            }

            List<Thing> things = new List<Thing>();
            foreach (Thing t in Level.CheckCircleAll<Thing>(collisionCenter, 2000f))
            {
                if (t is Duck) things.Add(t);
                if (t is Ragdoll && !(t as Ragdoll)._duck.dead) things.Add(t);
            }

            if (things.Count == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    MusketSmoke musketSmoke = new MusketSmoke(this.collisionCenter.x + Rando.Float(-30f, 30f), this.collisionCenter.y + Rando.Float(-30f, 30f));
                    musketSmoke.depth = (Depth)(float)(0.9 + (double)i * (1.0 / 1000.0));
                    Level.Add(musketSmoke);
                }
                _destroyed = true;
                this.Destroy(new DTFade());
                Level.Remove(this);
                return;
            }

            List<float> dist = new List<float>();
            foreach (Thing t in things)
            {
                dist.Add((position - t.position).length);
            }

            float min = float.MaxValue;
            int index = -1;
            for (int i = 0; i < dist.Count; i++)
            {
                if (dist[i] < min)
                {
                    min = dist[i];
                    index = i;
                }
            }

            Vec2 target = things[index].position;

            if (_speed < 3f) _speed += 0.0005f;
            this.angleDegrees = -Maths.PointDirection(Vec2.Zero, new Vec2(this.hSpeed, this.vSpeed));

            var vel = new Vec2(target.x - collisionCenter.x, target.y - collisionCenter.y);

            vel.Normalize();

            _hSpeed += vel.x * 0.3f;
            _vSpeed += vel.y * 0.3f;

            _hSpeed = Maths.Clamp(_hSpeed, -_speed, _speed);
            _vSpeed = Maths.Clamp(_vSpeed, -_speed, _speed);
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom @from)
        {
            if (with is Duck)
            {
                (with as Duck).Kill(new DTImpale(this));
            }
            if (with is RagdollPart)
            {
                (with as RagdollPart)._doll._duck.Kill(new DTImpale(this));
            }
            if (with is Door)
            {
                (with as Door).Destroy(new DTCrush(this));
            }

            //if (with.GetType().IsSubclassOf(typeof(Block)) || with is Block)
            //{
            //    if (from == ImpactedFrom.Left || from == ImpactedFrom.Right)
            //    {
            //        this._hSpeed *= -1;
            //    }
            //    if (from == ImpactedFrom.Top || from == ImpactedFrom.Bottom)
            //    {
            //        this._vSpeed *= -1;
            //    }
            //}

            base.OnImpact(with, @from);
        }
    }
}
