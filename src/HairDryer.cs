using System;
using DuckGame;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Misc")]
    public class HairDryer : Gun
    {
        public StateBinding _onStateBinding = new StateBinding("_on");
        public StateBinding _cooldownStateBinding = new StateBinding("_cooldown");
        public StateBinding _PowerStateBinding = new StateBinding("_power");

        public HotAir _air;

        public HairDryer(float xval, float yval) : base(xval, yval)
        {
            graphic = new Sprite(GetPath("dryer"));

            center = new Vec2(10.5f, 8.5f);
            collisionSize = new Vec2(21f, 17f);
            collisionOffset = new Vec2(-10.5f, -8.5f);
            _barrelOffsetTL = new Vec2(30f, 3.5f);
            _holdOffset = new Vec2(0f, 0f);

            wideBarrel = true;
            ammo = 100;
            _type = "gun";
            _kickForce = 0f;
            _ammoType = (AmmoType)new ATDefault();
            _fireSound = null;
            editorTooltip = "Old hair dryer, legends say that it blows to hard.";

            physicsMaterial = PhysicsMaterial.Plastic;
            _on = false;
            _sound = new Sound(GetPath("blow"), 1f, 1f, 0f, true);
            _power = -1f;
        }

        public override void Terminate()
        {
            if (_air != null)
                Level.Remove(_air);
            base.Terminate();
        }

        public override void CheckIfHoldObstructed()
        {
            if (owner != null)
                duck.holdObstructed = false;
        }

        public override void OnPressAction()
        {
            if (ammo > 0)
            {
                if (_air == null && isServerForObject)
                {
                    _air = new HotAir(x + (float)offDir * 32f, y, this);
                    Level.Add(_air);
                }
                _on = true;
                _sound.Play();
            }
            else
                SFX.Play(_clickSound, 1f, 0.0f, 0.0f, false);
        }

        public override void OnReleaseAction()
        {
            _on = false;
            _power += 0.2f;
            base.OnReleaseAction();
        }

        public override void Update()
        {
            if (_cooldown > 0)
                _cooldown -= 0.01f;

            if (_on)
            {
                if (_cooldown < 0f)
                {
                    if (ammo == 0)
                    {
                        _on = false;
                        SFX.Play(_clickSound, 1f, 0.0f, 0.0f, false);
                    }
                    else
                    {
                        ammo--;
                    }
                    _cooldown = 1f;
                }
                else
                    _cooldown -= 0.01f;

                _power += 0.03f;
                if (_power > 0.5f) _power = 0.5f;
                _sound.Pitch = _power;
            }
            else
            {
                _power -= 0.05f;
                if (_power < -0.5f)
                {
                    _power = -0.5f;
                    _sound.Stop();
                }
                _sound.Pitch = _power;
            }

            base.Update();
        }

        public override void Fire()
        {
            // do nothing
        }

        public bool _on;
        private float _cooldown;
        private Sound _sound;
        public float _power;
    }

    public class HotAir : MaterialThing
    {
        public StateBinding _dryerStateBinding = new StateBinding("_hd");
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");

        public SpriteMap sprite;
        private HairDryer _hd;

        public HotAir(float xpos, float ypos, HairDryer hd)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(GetPath("hotair"), 40, 20);
            sprite.AddAnimation("*schwoo*", 0.5f, true, 0, 1, 2, 3);
            sprite.SetAnimation("*schwoo*");
            graphic = sprite;
            center = new Vec2(20f, 10f);
            depth = 0.3f;
            _hd = hd;
            visible = false;
        }

        public override void Update()
        {
            if (_hd == null || !_hd._on)
            {
                visible = false;
                return;
            }

            visible = _hd._on;

            offDir = _hd.offDir;
            if (offDir < 0)
                sprite.flipH = true;
            else
                sprite.flipH = false;

            x = _hd.x + _hd.offDir * 28f;
            y = _hd.y - 4f;

            foreach (PhysicsObject physicsObject in Level.CheckRectAll<PhysicsObject>(position,
                new Vec2(position.x + sprite.width, y + sprite.height)))
            {
                if ((Level.CheckLine<Block>(_hd.position, physicsObject.position, physicsObject) == null) &&
                    physicsObject != _hd && physicsObject != _hd.owner)
                {
                    physicsObject.hSpeed += offDir * 2.5f * (float)Math.Pow((physicsObject.position - position).length, 1 / 3);
                    if (physicsObject.weight <= 10f)
                        physicsObject.vSpeed -= 0.3f / (float)Math.Sqrt(physicsObject.weight);
                }
            }
        }
    }
}
