using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleGenerateManager : MonoBehaviour
{
    GameObject singleGameObject;

    public void SetSingleGameObject(GameObject setObject)
    {
        //ˆø”‚ªnull‚È‚çsingleGameObject‚ğnull‚É‚µ‚Äˆ—‚ğI—¹
        if(setObject == null)
        {
            singleGameObject = null;
            return;
        }

        //singleGameObject‚ª‚à‚Æ‚à‚Ænull‚È‚ç•’Ê‚É‘ã“ü
        if(singleGameObject == null)
        {
            singleGameObject = setObject;
        }
        //singleGameObject‚É‰½‚©‚ª“ü‚Á‚Ä‚¢‚éó‘Ô‚ÅŒÄ‚Î‚ê‚½ê‡‚É‚ÍŒ³‚ğíœ‚µ‚Äˆø”‚Ì‚à‚Ì‚ÉXV
        else
        {
            Destroy(singleGameObject);
            singleGameObject = setObject;
        }
    }

    public GameObject GetSingleGameObject()
    {
        return singleGameObject;
    }
}
