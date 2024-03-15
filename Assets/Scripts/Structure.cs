using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Structure
{

    public static blockType block;

    public static Queue<VoxelMod> GenerateMajorFlora(majorFlora index, Vector3 position, int minTrunkHeight, int maxTrunkHeight)
    {
        switch (index)
        {
            case majorFlora.Oak_Tree:
                return MakeTree(position, minTrunkHeight, maxTrunkHeight);
            case majorFlora.Cactus:
                return MakeCacti(position, minTrunkHeight, maxTrunkHeight);
        }

        return new Queue<VoxelMod>();

    }

    public static Queue<VoxelMod> MakeTree(Vector3 position, int minTrunkHeight, int maxTrunkHeight)
    {
        Queue<VoxelMod> queue = new Queue<VoxelMod>();
        int height = (int)(maxTrunkHeight * Noise.Get2DPerlin(new Vector2(position.x, position.z), 250f, 3f));

        if (height < minTrunkHeight)
            height = minTrunkHeight;

        for (int x = -2; x < 3; x++)
        {
            for (int z = -2; z < 3; z++)
            {
                queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height - 2, position.z + z), blockType.Leaves));
                queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height - 3, position.z + z), blockType.Leaves));
            }
        }

        for (int x = -1; x < 2; x++)
        {
            for (int z = -1; z < 2; z++)
            {
                queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height - 1, position.z + z), blockType.Leaves));
            }
        }
        for (int x = -1; x < 2; x++)
        {
            if (x == 0)
                for (int z = -1; z < 2; z++)
                {
                    queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height, position.z + z), blockType.Leaves));
                }
            else
                queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height, position.z), blockType.Leaves));
        }

        for (int i = 1; i < height; i++)
            queue.Enqueue(new VoxelMod(new Vector3(position.x, position.y + i, position.z), blockType.Oak_Log));

        return queue;
    }

    public static Queue<VoxelMod> MakeCacti(Vector3 position, int minTrunkHeight, int maxTrunkHeight)
    {

        Queue<VoxelMod> queue = new Queue<VoxelMod>();

        int height = (int)(maxTrunkHeight * Noise.Get2DPerlin(new Vector2(position.x, position.z), 23456f, 2f));

        if (height < minTrunkHeight)
            height = minTrunkHeight;

        for (int i = 1; i <= height; i++)
            queue.Enqueue(new VoxelMod(new Vector3(position.x, position.y + i, position.z), blockType.Cactus));

        return queue;

    }
}
