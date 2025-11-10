using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour 
{
    public Image Image;
    public Sprite noPortal;
    public Sprite bluePortal;
    public Sprite orangePortal;
    public Sprite bothPortal;

    public void SetNoPortal()
    {
        Image.sprite = noPortal;
    }
    
    public void SetBluePortal()
    {
        Image.sprite = bluePortal;
    }

    public void SetOrangePortal()
    {
        Image.sprite = orangePortal;
    }

    public void SetBothPortal()
    {
        Image.sprite = bothPortal;
    }


}
