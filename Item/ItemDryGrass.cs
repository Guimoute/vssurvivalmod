﻿using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

namespace Vintagestory.GameContent
{
    public class ItemDryGrass : Item
    {
        public override void OnHeldInteractStart(ItemSlot itemslot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handHandling)
        {
            if (blockSel == null || byEntity?.World == null || !byEntity.Controls.Sneak) return;

            IWorldAccessor world = byEntity.World;
            Block firepitBlock = world.GetBlock(new AssetLocation("firepit-construct1"));
            if (firepitBlock == null) return;


            BlockPos onPos = blockSel.DidOffset ? blockSel.Position : blockSel.Position.AddCopy(blockSel.Face);

            IPlayer byPlayer = byEntity.World.PlayerByUid((byEntity as EntityPlayer)?.PlayerUID);
            if (!byEntity.World.Claims.TryAccess(byPlayer, onPos, EnumBlockAccessFlags.BuildOrBreak))
            {
                return;
            }

            

            Block block = world.BlockAccessor.GetBlock(onPos.DownCopy());
            Block aimedBlock = world.BlockAccessor.GetBlock(blockSel.Position);
            if (aimedBlock is BlockGroundStorage)
            {
                if (!(aimedBlock is BlockPitkiln))
                {
                    BlockPitkiln blockpk = world.GetBlock(new AssetLocation("pitkiln")) as BlockPitkiln;
                    if (blockpk.TryCreateKiln(world, byPlayer, blockSel.Position))
                    {
                        handHandling = EnumHandHandling.PreventDefault;
                    }
                }
            }
            else
            {

                string useless = "";

                if (!block.CanAttachBlockAt(byEntity.World.BlockAccessor, firepitBlock, onPos.DownCopy(), BlockFacing.UP)) return;
                if (!firepitBlock.CanPlaceBlock(world, byPlayer, new BlockSelection() { Position = onPos, Face = BlockFacing.UP }, ref useless)) return;

                world.BlockAccessor.SetBlock(firepitBlock.BlockId, onPos);

                if (firepitBlock.Sounds != null) world.PlaySoundAt(firepitBlock.Sounds.Place, blockSel.Position.X, blockSel.Position.Y, blockSel.Position.Z, byPlayer);

                itemslot.Itemstack.StackSize--;
                handHandling = EnumHandHandling.PreventDefaultAction;
            }
        }


        public override WorldInteraction[] GetHeldInteractionHelp(ItemSlot inSlot)
        {
            return new WorldInteraction[] {
                new WorldInteraction()
                {
                    HotKeyCode = "sneak",
                    ActionLangCode = "heldhelp-createfirepit",
                    MouseButton = EnumMouseButton.Right,
                }
            }.Append(base.GetHeldInteractionHelp(inSlot));
        }

    }
}
