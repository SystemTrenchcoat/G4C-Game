using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceDetect : MonoBehaviour
{
    Vector2 lastMousePos;
    public LayerMask fruitLayer; //LayerMask to only detect "Fruit" objects
                                 //LayerMask is used to make sure only thing that being destroyed is
                                 //object that has the 'Fruit' mask.
                                 //(i am lacking imagination here so i just picked fruit as the name since we are referencing fruit ninja anyway)
    void Update()
    {
        //make sure mouse is held and last mouse pos is avaliable
        if (Input.GetMouseButton(0))
        {
            Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (lastMousePos != Vector2.zero)
            {
                //'Raycast': it shoots an invisible line from point A to point B, drag out by mouse, act as the 'slicing'
                //this line detect everything that touches it, combine with layermask to single out gameobject
                RaycastHit2D[] hits = Physics2D.RaycastAll(lastMousePos, (currentMousePos - lastMousePos),
                                                            Vector2.Distance(lastMousePos, currentMousePos), fruitLayer);

                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider != null)
                    {
                        Debug.Log("intercept/ HIT!/ SLICED!");
                        Destroy(hit.collider.gameObject); //"Slice" the fruit -> destroying it
                    }
                }

                //debug, check to see if the positions are working
                Debug.DrawLine(lastMousePos, currentMousePos, Color.red, 0.1f);
            }

            lastMousePos = currentMousePos;
        }
        else
        {
            lastMousePos = Vector2.zero;
        }
    }
}
