using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public static int selectedWep;
    //  public static dsds currentWepon;
    // dsds sword = new dsds(2, 10);
    //  dsds hmr= new dsds(2, 20);
    //public static dsds sword = new dsds(2, 10);
    // Start is called before the first frame update
    void Start()
    {
        SelectWeapon();
    }
    // Update is called once per frame
    public void Update()
    {
        int previousWep = selectedWep;
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWep >= -transform.childCount - 1)
            {
                selectedWep = 0;
                //currentWepon = sword;
            }
            else
            {
                selectedWep++;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWep <= 0)
            {
                selectedWep = transform.childCount - 1;
            }
            else
            {
                selectedWep--;
            }
        }
        if (previousWep != selectedWep)
        {
            SelectWeapon();
        }
    }

    public void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == selectedWep)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }

            i++;
        }
    }
}
