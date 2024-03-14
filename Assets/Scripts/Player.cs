using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public bool isGrounded;
    public bool isSprinting;
    public Text gameModeText;


    public GameMode selectedMode;
    public enum GameMode
    {
        Survival,
        Spectator,
        Adventure,
        Creative
    }

    public Transform cam;
    public World world;
    public Canvas IngameDebug;
    public bool inLog;

    public float flySpeed = 6f;
    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;
    public float jumpForce = 5f;
    public float gravity = -9.8f;
    public float threshold = 0.1f; // You can adjust this value as needed


    public float playerWidth = 0.6f;
    public float boundsTolerance = 0.1f;

    private float rotationY;
    private float horizontal;
    private float vertical;
    private float mouseHorizontal = 2.5f;
    private float mouseVertical = 2.5f;
    private Vector3 velocity;
    private float verticalMomentum = 0;
    private bool jumpRequest;

    public Transform highlightBlock;
    public Transform placeBlock;
    public float checkIncrement = 0.1f;
    public float reach = 8f;

    public Toolbar toolbar;


    private void Start()
    {
        selectedMode = GameMode.Survival;
        //world.inUI = false;
        cam = GameObject.Find("Main Camera").transform;
        world = GameObject.Find("World").GetComponent<World>();
    }

    private void FixedUpdate()
    {
        if (!world.inUI)
        {
            CalculateVelocity();
            if (selectedMode != GameMode.Spectator)
            {
                if (jumpRequest)
                    Jump();
            }
            transform.Translate(velocity, Space.World);

        }

    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.G))
        {
            if ((int)selectedMode < (System.Enum.GetValues(typeof(GameMode)).Length - 1))
                selectedMode++;
            else
                selectedMode = GameMode.Survival;
            StartCoroutine(GameModeText());
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            world.inUI = !world.inUI;
        }
        if (!world.inUI)
        {
            GetPlayerInputs();
            placeCursorBlocks();
            transform.Rotate(Vector3.up * mouseHorizontal * world.settings.mouseSensitivity);
            cam.Rotate(Vector3.right * -mouseVertical * world.settings.mouseSensitivity);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (world.settings.devOptions)
            {
                if (inLog == false)
                {
                    inLog = true;
                }
                else
                {
                    inLog = false;
                }
            }

        }

        if (inLog)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            if (!world.inUI)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

        }

    }

    public IEnumerator FadeTextToFullAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    public IEnumerator FadeTextToZeroAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }

    private IEnumerator GameModeText()
    {
        gameModeText.gameObject.SetActive(true);
        gameModeText.text = "Gamemode: " + selectedMode;
        StartCoroutine(FadeTextToFullAlpha(1f, gameModeText));

        yield return new WaitForSeconds(1f);

        StartCoroutine(FadeTextToZeroAlpha(1f, gameModeText));
        yield return new WaitForSeconds(1f);
        gameModeText.gameObject.SetActive(false);

    }

    void Jump()
    {
        verticalMomentum = jumpForce;
        isGrounded = false;
        jumpRequest = false;
    }

    private void CalculateVelocity()
    {
        if(selectedMode != GameMode.Spectator)
        {
            // Affect vertical momentum with gravity
            if (verticalMomentum > gravity)
                verticalMomentum += Time.fixedDeltaTime * gravity;

            // if we're sprinting, use the sprint multiplier
            if (isSprinting)
                velocity = ((transform.forward * vertical) + (transform.right * horizontal)).normalized * Time.fixedDeltaTime * sprintSpeed;
            else
                velocity = ((transform.forward * vertical) + (transform.right * horizontal)).normalized * Time.fixedDeltaTime * walkSpeed;

            // Apply vertical momentum (falling/jumping)
            velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

            if ((velocity.z > 0 && front) || (velocity.z < 0 && back))
                velocity.z = 0;
            if ((velocity.x > 0 && right) || (velocity.x < 0 && left))
                velocity.x = 0;

            if (velocity.y < 0)
                velocity.y = checkDownSpeed(velocity.y);
            else if (velocity.y > 0)
                velocity.y = checkUpSpeed(velocity.y);
        }
        else
        {
            velocity.y = 0;
            velocity.x = 0;
            velocity.z = 0;
            transform.position = (transform.position + Camera.main.transform.forward * vertical * Time.fixedDeltaTime * flySpeed) + (Camera.main.transform.right * horizontal * Time.fixedDeltaTime * flySpeed);
            //velocity = ((transform.forward * vertical) + (transform.right * horizontal)).normalized * Time.fixedDeltaTime * walkSpeed;
            //velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;
        }

    }

    private void GetPlayerInputs()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            print("Quit");
            Application.Quit();
        }

        if(selectedMode != GameMode.Spectator)
        {
            if (Input.GetButtonDown("Sprint") && isGrounded)
                isSprinting = true;
            if (Input.GetButtonUp("Sprint"))
                isSprinting = false;

            if (isGrounded && Input.GetButton("Jump"))
                jumpRequest = true;

            if(selectedMode != GameMode.Adventure)
            {
                if (highlightBlock.gameObject.activeSelf)
                {
                    // Destroy Block
                    if (Input.GetMouseButtonDown(0))
                    {
                        world.GetChunkFromVector3(highlightBlock.position).EditVoxel(highlightBlock.position, 0);
                    }

                    if (Input.GetMouseButtonDown(1))
                    {
                        if (toolbar.slots[toolbar.slotIndex].HasItem)
                        {
                            Vector3Int playerPosition = new Vector3Int(Mathf.FloorToInt(world.player.position.x), Mathf.FloorToInt(world.player.position.y), Mathf.FloorToInt(world.player.position.z));
                            Vector3Int placeBlockPosition = new Vector3Int(Mathf.RoundToInt(placeBlock.position.x), Mathf.RoundToInt(placeBlock.position.y), Mathf.RoundToInt(placeBlock.position.z));
                            if (placeBlockPosition != playerPosition && placeBlockPosition != playerPosition + new Vector3Int(0, 1, 0))
                            {
                                world.GetChunkFromVector3(placeBlock.position).EditVoxel(placeBlock.position, toolbar.slots[toolbar.slotIndex].itemSlot.stack.id);
                                if(selectedMode != GameMode.Creative)
                                    toolbar.slots[toolbar.slotIndex].itemSlot.Take(1);
                            }

                        }

                    }
                }
            }
        }
        /*else
        {
            if (Input.GetButtonDown("Sprint"))
            {
                velocity.y = checkDownSpeed(velocity.y);
            }
            if (Input.GetButtonUp("Sprint"))
            {
                velocity.y = 0;
            }

            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = checkUpSpeed(velocity.y);
            }
            if (Input.GetButtonUp("Jump"))
            {
                velocity.y = 0;
            }

        }*/
    }

    private void placeCursorBlocks()
    {
        if(selectedMode != GameMode.Spectator && selectedMode != GameMode.Adventure)
        {
            float step = checkIncrement;
            Vector3 lastPos = new Vector3();

            while (step < reach)
            {
                Vector3 pos = cam.position + (cam.forward * step);

                if (world.CheckForVoxel(pos))
                {
                    highlightBlock.position = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
                    placeBlock.position = lastPos;

                    highlightBlock.gameObject.SetActive(true);
                    placeBlock.gameObject.SetActive(true);

                    return;
                }

                lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));

                step += checkIncrement;
            }
        }
        highlightBlock.gameObject.SetActive(false);
        placeBlock.gameObject.SetActive(false);

    }

    private float checkDownSpeed(float downSpeed)
    {
        if (
            (world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) && (!left && !back)) ||
            (world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) && (!right && !back)) ||
            (world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth)) && (!right && !front)) ||
            (world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth)) && (!left && !front))
           )
        {
            isGrounded = true;
            return 0;
        }
        else
        {
            isGrounded = false;
            return downSpeed;
        }
    }


private float checkUpSpeed(float upSpeed)
    {
        if (
            (world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 1.875f + upSpeed, transform.position.z - playerWidth)) && (!left && !back)) ||
            (world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 1.875f + upSpeed, transform.position.z - playerWidth)) && (!right && !back)) ||
            (world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 1.875f + upSpeed, transform.position.z + playerWidth)) && (!right && !front)) ||
            (world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 1.875f + upSpeed, transform.position.z + playerWidth)) && (!left && !front))
           )
        {
                verticalMomentum = 0;
 // set to 0 so the player falls when their head hits a block while jumping
            return 0;
        }
        else
        {
            return upSpeed;
        }
    }

    public bool front
    {
        get {
            if (
               world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z + playerWidth)) ||
               world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + playerWidth))
               )
                return true;
            else
                return false;
        }
    }
    public bool back
    {
        get
        {
            if (
               world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z - playerWidth)) ||
               world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - playerWidth))
               )
                return true;
            else
                return false;
        }
    }
    public bool left
    {
        get
        {
            if (
               world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y, transform.position.z)) ||
               world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z))
               )
                return true;
            else
                return false;
        }
    }
    public bool right
    {
        get
        {
            if (
               world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y, transform.position.z)) ||
               world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z))
               )
                return true;
            else
                return false;
        }
    }


}
