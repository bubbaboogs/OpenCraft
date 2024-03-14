using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropHandler : MonoBehaviour
{
    [SerializeField] private UIItemSlot cursorSlot = null;
    private ItemSlot cursorItemSlot;

    [SerializeField] private GraphicRaycaster m_Raycaster = null;
    private PointerEventData m_PointerEventData;
    [SerializeField] private EventSystem m_EventSystem = null;

    World world;

    private void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();

        cursorItemSlot = new ItemSlot(cursorSlot);

    }

    private void Update()
    {
        if (!world.inUI)
            return;

        cursorSlot.transform.position = Input.mousePosition;
        if(Input.GetMouseButtonDown(0))
        {
            HandleSlotClick(CheckForSlot());
        }

        if (Input.GetMouseButtonDown(1))
        {
            HandleSlotRightClick(CheckForSlot());
        }

        if (Input.GetMouseButton(2))
        {
            HandleSlotMiddleClick(CheckForSlot());
        }


    }

    private void HandleSlotRightClick(UIItemSlot rightClickedSlot)
    {
        if (rightClickedSlot == null)
            return;


        if (rightClickedSlot.itemSlot.stack.amount > 1 && rightClickedSlot.itemSlot.isCreative)
        {
            cursorItemSlot.EmptySlot();
            cursorItemSlot.InsertStack(rightClickedSlot.itemSlot.TakeHalf());
        }
        else if (rightClickedSlot.itemSlot.stack.amount > 1)
        {
            cursorItemSlot.InsertStack(rightClickedSlot.itemSlot.TakeHalf());
        }
        else
        {
            return;
        }
    }

    private void HandleSlotMiddleClick(UIItemSlot middleClickedSlot)
    {
        if (middleClickedSlot == null)
            return;
        if (cursorItemSlot.stack.amount > 0 && cursorItemSlot.stack.id == middleClickedSlot.itemSlot.stack.id)
        {
            cursorSlot.itemSlot.stack.amount -= 1;
            middleClickedSlot.itemSlot.stack.amount += 1;
        }
        else
        {
            return;
        }
    }

    private void HandleSlotClick(UIItemSlot clickedSlot)
    {
        if (clickedSlot == null)
            return;

        if (!cursorSlot.HasItem && !clickedSlot.HasItem)
            return;

        if (clickedSlot.itemSlot.isCreative)
        {
            cursorItemSlot.EmptySlot();
            cursorItemSlot.InsertStack(clickedSlot.itemSlot.stack);
        }

        if(!cursorSlot.HasItem && clickedSlot.HasItem)
        {
            cursorItemSlot.InsertStack(clickedSlot.itemSlot.TakeAll());
            return;
        }
        if (cursorSlot.HasItem && !clickedSlot.HasItem)
        {

            clickedSlot.itemSlot.InsertStack(cursorItemSlot.TakeAll());
            return;
        }
        if (cursorSlot.HasItem && clickedSlot.HasItem)
        {
            if (cursorSlot.itemSlot.stack.id != clickedSlot.itemSlot.stack.id)
            {  // items carried and in slot are differents
                //print("Items are different");
                ItemStack previousCursorSlotStack = cursorSlot.itemSlot.TakeAll();
                ItemStack previousClickedSlotStack = clickedSlot.itemSlot.TakeAll();

                clickedSlot.itemSlot.InsertStack(previousCursorSlotStack);
                cursorSlot.itemSlot.InsertStack(previousClickedSlotStack);
            }
            else
            {
                //print("Items are the same");
                int clickedSlotItemID = (int)clickedSlot.itemSlot.stack.id;  // gets the id of item
                int maxStackSize = world.blocktypes[clickedSlotItemID].maxStackSize;  // gets the maxStackSize for this blocktype/item ID

                ItemStack previousCursorSlotStack = cursorSlot.itemSlot.TakeAll();
                ItemStack previousClickedSlotStack = clickedSlot.itemSlot.TakeAll();
                int combinedClickedSlotStackAmount = previousClickedSlotStack.amount + previousCursorSlotStack.amount;

                if (combinedClickedSlotStackAmount > maxStackSize)
                {
                    //print("Greater than max stack size");
                    //print(combinedClickedSlotStackAmount + "Combined");
                    ItemStack combinedClickedSlotStackFull = new ItemStack((blockType)clickedSlotItemID, maxStackSize);
                    ItemStack combinedClickedSlotStackLeftover = new ItemStack((blockType)clickedSlotItemID, combinedClickedSlotStackAmount - maxStackSize);
                    clickedSlot.itemSlot.InsertStack(combinedClickedSlotStackFull);
                    // cursorSlot.itemSlot.itemStack.amount = combinedClickedSlotStackLeftover.amount;
                    cursorSlot.itemSlot.InsertStack(combinedClickedSlotStackLeftover);
                    //print(combinedClickedSlotStackLeftover.amount + "Leftover");
                    return;
                }
                else
                {
                    //print("Smaller than max stack size");
                    ItemStack combinedClickedSlotStack = new ItemStack((blockType)clickedSlotItemID, combinedClickedSlotStackAmount);
                    clickedSlot.itemSlot.InsertStack(combinedClickedSlotStack);
                    return;
                }
            }
            return;
        }
    }


    private UIItemSlot CheckForSlot()
    {
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);

        foreach(RaycastResult result in results)
        {
            if(result.gameObject.tag == "UIItemSlot")
                return result.gameObject.GetComponent<UIItemSlot>();

        }

        return null;
    }
}
