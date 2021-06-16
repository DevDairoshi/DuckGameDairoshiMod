using System.Collections.Generic;

namespace DuckGame.DairoshiMod
{
    [EditorGroup("DairoshiMod|Melee")]
    public class JackHammer : Gun
    {
        public SpriteMap map;
        public JackHammer(float xval, float yval) : base(xval, yval)
        {
            map = new SpriteMap(GetPath("jack"), 11, 18);
            map.AddAnimation("*drill*", 0.5f, true, 0, 1);
            map.AddAnimation("*calm*", 0.1f, false, 0,0);
            graphic = map;

            center = new Vec2(5.5f, 9f);
            collisionSize = new Vec2(11f, 14f);
            collisionOffset = new Vec2(-5.5f, -7f);
            _barrelOffsetTL = new Vec2(6f, 18f);
            _holdOffset = new Vec2(-1f, 9f);
            handOffset = new Vec2(3f, -1f);
            
            ammo = 9999;
            _ammoType = new ATNone();
            _fullAuto = true;
            _fireWait = 0.5f;
            _type = "gun";
            _kickForce = 0.3f;
            weight = 5.2f;
            _fireSound = GetPath("drill");
            _fireSoundPitch = -0.5f;
            editorTooltip = "For hardworking ducks.";
            _flare = new SpriteMap("rock", 0, 0);
        }

        public override void Fire()
        {
            map.SetAnimation("*drill*");
            ammo = 9999;

            foreach (BlockGroup blockGroup in Level.CheckLineAll<BlockGroup>(Offset(barrelOffset), Offset(barrelOffset) + new Vec2(0f, 5f)))
                blockGroup.Wreck();
            foreach (MaterialThing materialThing in Level.CheckLineAll<MaterialThing>(Offset(barrelOffset), Offset(barrelOffset) + new Vec2(0f, 5f)))
                if (materialThing.solid && materialThing.thickness > 0f && !(materialThing is Gun) &&
                    !(materialThing is Equipment) && !(materialThing is RagdollPart))
                {
                    if (materialThing == this.duck)
                        continue;
                    Dig(materialThing);
                }
            base.Fire();
        }

        public override void OnReleaseAction()
        {
            map.SetAnimation("*calm*");
        }

        public void Dig(MaterialThing materialThing)
        {
            if (materialThing is BlockGroup)
            {
                BlockGroup bg = materialThing as BlockGroup;
                foreach (Block bl in bg.blocks)
                    if (Collision.Circle(Offset(barrelOffset), 2f, bl.rectangle))
                        Dig(bl);
                bg.Wreck();
            }
            else if (materialThing is AutoBlock)
            {
                foreach (MaterialThing mT in Level.CheckCircleAll<MaterialThing>(materialThing.position, 24f))
                {
                    if (mT is BlockGroup)
                    {
                        BlockGroup bg = mT as BlockGroup;
                        bg.Wreck();
                    }
                    else if (mT is PhysicsObject)
                    {
                        if (mT.owner == null)
                            Thing.Fondle(mT, DuckNetwork.localConnection);
                        ((PhysicsObject)mT).sleeping = false;
                        mT.vSpeed = -2f;
                    }
                }

                HashSet<ushort> blocksToDestroy = new HashSet<ushort>();
                blocksToDestroy.Add((materialThing as AutoBlock).blockIndex);
                ((Block)materialThing).skipWreck = true;
                ((Block)materialThing).shouldWreck = true;
                if (Network.isActive && this.isLocal)
                    Send.Message(new NMDestroyBlocks(blocksToDestroy));
                SFX.Play(GetPath("hammer"), 1f, Rando.Float(-0.3f, 0.3f) - 0.5f);
            }
            else
            {
                if (materialThing.owner == null)
                    Thing.Fondle(materialThing, DuckNetwork.localConnection);
                if (materialThing.Destroy(new DTImpact(this)))
                    Level.Remove(materialThing);
            }

            Level.Add(SmallSmoke.New(Offset(barrelOffset).x, Offset(barrelOffset).y, 0.5f, 1.5f));
        }
    }

    public class ATNone : AmmoType
    {
        public ATNone()
        {
            range = 0f;
            deadly = false;
            immediatelyDeadly = false;
            impactPower = 0f;
            weight = 0f;
        }
    }
}