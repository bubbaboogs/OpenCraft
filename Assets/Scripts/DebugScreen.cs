using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugScreen : MonoBehaviour
{
    World world;
    Text text;
    Player player;

    float frameRate;
    float timer;

    int halfWorldSizeInVoxels;
    int halfWorldSizeInChunks;
    // Start is called before the first frame update
    void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();
        player = GameObject.Find("Player").GetComponent<Player>();
        text = GetComponent<Text>();

        halfWorldSizeInChunks = VoxelData.WorldSizeInChunks / 2;
        halfWorldSizeInVoxels = VoxelData.WorldSizeInVoxels / 2;
    }

    // Update is called once per frame
    void Update()
    {
        

        Direction playerFacing = Direction.North; // Assuming North as default

        float playerRotation = world.player.transform.rotation.eulerAngles.y;

        if (playerRotation <= 29.99f || playerRotation >= 337f)
            playerFacing = Direction.North;
        else if (playerRotation <= 59.99f && playerRotation >= 30f)
            playerFacing = Direction.NorthEast;
        else if (playerRotation <= 119.99f && playerRotation >= 60f)
            playerFacing = Direction.East;
        else if (playerRotation <= 149.99f && playerRotation >= 120f)
            playerFacing = Direction.SouthEast;
        else if (playerRotation <= 209.99f && playerRotation >= 150f)
            playerFacing = Direction.South;
        else if (playerRotation <= 239.99f && playerRotation >= 210f)
            playerFacing = Direction.SouthWest;
        else if (playerRotation <= 299.99f && playerRotation >= 240f)
            playerFacing = Direction.West;
        else if (playerRotation <= 329.99f && playerRotation >= 300f)
            playerFacing = Direction.NorthWest;


        string debugText = "OpenCraft Version " + world.settings.version;
        debugText += "\n";
        debugText += frameRate + " fps";
        debugText += "\n\n";
        debugText += "XYZ: " + (Mathf.FloorToInt(world.player.transform.position.x) - halfWorldSizeInVoxels) + " / " + Mathf.FloorToInt(world.player.transform.position.y) + " / " + (Mathf.FloorToInt(world.player.transform.position.z) - halfWorldSizeInVoxels);
        debugText += "\n";
        debugText += "Chunk: " + (world.playerChunkCoord.x - halfWorldSizeInChunks) + " / " + (world.playerChunkCoord.z - halfWorldSizeInChunks);
        debugText += "\n";
        debugText += "Facing: " + playerFacing;
        debugText += "\n";
        debugText += "Biome: " + world.GetBiome(player.transform.position).biomeName;
        debugText += "\n";
        debugText += "Yaw: " + world.player.transform.rotation.eulerAngles.y;
        debugText += "\n";
        debugText += "Pitch: " + player.cam.transform.rotation.eulerAngles.x;


        text.text = debugText;

        if (timer > 1f)
        {
            frameRate = (int)(1f / Time.unscaledDeltaTime);
            timer = 0;
        }
        else
            timer += Time.deltaTime;
    }

    public enum Direction
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }

}
