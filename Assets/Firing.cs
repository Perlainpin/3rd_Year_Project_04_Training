using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class Firing : MonoBehaviour
{
    public GameObject fire;

    public GameObject bullet;
    public float shootForce, upwardForce;
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;

    public int magazineSize, bulletPerTap;

    public bool allowButtonHold;

    private int bulletsLeft, bulletsShot;

    public Rigidbody playerRb;
    public float recoilForce;

    private bool shooting, readyToShoot, reloading;

    public Camera fpsCam;
    public Transform attackPoint;

    //public GameObject muzzleFlash;

    public bool allowInvoke = true;

    public GameObject Casing;
    public float Casingspeed = 100f;

    public GameObject spawnBullet;
    public GameObject spawnCasing;
    public GameObject spawnFlash;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        fire.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        MyInput();
    }

    private void MyInput()
    {
        //to rework with new input manager
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKey(KeyCode.Mouse0);

        if(Input.GetKey(KeyCode.R) && bulletsLeft < magazineSize && !reloading) 
            Reload();

        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0)
            Reload();

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;
            
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        GameObject newCasing = Instantiate(Casing, spawnCasing.transform.position, Casing.transform.rotation);
        newCasing.GetComponent<Renderer>().enabled = true;
        Destroy(newCasing, 5f);

        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75);

        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        currentBullet.GetComponent<Renderer>().enabled = true;

        float maxLifeTime = currentBullet.GetComponent<CustomBullet>().maxLifeTime;
        
        currentBullet.transform.forward = directionWithSpread.normalized;

        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        Destroy(currentBullet, maxLifeTime);

        playerRb.AddForce(-directionWithSpread.normalized * recoilForce, ForceMode.Impulse);

        if(fire != null)
        {
            GameObject muzzleFlash =  Instantiate(fire, spawnFlash.transform.position, fire.transform.rotation);
            muzzleFlash.GetComponent<Renderer>().enabled = true;
            muzzleFlash.SetActive(true);
            Destroy(muzzleFlash, 0.1f);
        }
            
        bulletsLeft--;
        bulletsShot++;

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }

        if (bulletsShot < bulletPerTap && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }

}
