using System.Collections;
using UnityEngine;

public interface IHitable
{
    void OnHit();
}

[RequireComponent(typeof(InputManager))]
public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    [Tooltip("Reference to the gun's transform for recoil")]
    [SerializeField] private Transform gunTransform;
    [Tooltip("The point where the laser begins")]
    [SerializeField] private Transform laserOrigin;
    [SerializeField] private LineRenderer lineRenderer; // The laser line renderer
    [Tooltip("Range of the laser shot")]
    [SerializeField] private float shootRange = 100f;
    [Tooltip("Max rounds per magazine")]
    [SerializeField] private int maxMagazine = 10;
    [Tooltip("Time between shots in seconds")]
    [SerializeField] private float fireRate = 0.2f;
    [Tooltip("How long to display the laser")]
    [SerializeField] private float laserDisplayTime = 0.05f;

    [Header("Recoil Settings")]
    [Tooltip("How far the gun moves back during recoil")]
    [SerializeField] private float recoilDistance = 0.1f;
    [Tooltip("How long the recoil lasts in seconds.")]
    [SerializeField] private float recoilSpeed = 0.1f;
    [Tooltip("In degrees how much to vary the recoil.")]
    [SerializeField] private Vector2 randomRecoilRotation = new Vector2(-4f, 3f);


    [Header("Reload Settings")]
    [Tooltip("Time to reload in seconds")]
    [SerializeField] private float reloadTime = 2f; // Time to reload

    [Header("Gun Sounds")]
    [SerializeField] private AudioClip[] shootingSounds;
    [SerializeField] private AudioClip[] reloadingSounds;
    [SerializeField] private AudioClip[] equipSounds;
    [SerializeField] private float shootingVolume = 1f;
    [SerializeField] private float reloadingVolume = 1f;
    [SerializeField] private float equipVolume = 1f;

    private int currentMagazine; // Tracks the current number of bullets in the magazine
    private bool isReloading = false;
    private bool canShoot = true; // Can we currently fire?
    private bool isGunEnabled = false;
    private InputManager inputManager;
    private Vector3 originalPosition = Vector3.zero;
    private Quaternion originalRotation = Quaternion.identity;

    private void Start()
    {
        // Initialise the magazine with full capacity
        currentMagazine = maxMagazine;
        lineRenderer.enabled = false;
        inputManager = GetComponent<InputManager>();

        originalPosition = gunTransform.localPosition;
        originalRotation = gunTransform.localRotation;

        // Disable the gun initially
        gunTransform.gameObject.SetActive(false);
        canShoot = false;
        isReloading = false;
        PlayerManager.OnPlayerGunEquip += EnableGun;
    }

    private void Update()
    {
        Shoot();
        Reload();
    }

    public void EnableGun()
    {
        if (isGunEnabled) return;
        if (AudioManager.Instance) AudioManager.Instance.PlayOneShot(equipSounds, equipVolume);
        gunTransform.gameObject.SetActive(true);
        StartCoroutine(BringGunIntoView());
    }

    private void Shoot()
    {
        if (isReloading || !canShoot) return;

        if (inputManager.IsAttackingOne() && currentMagazine > 0)
        {
            StartCoroutine(ShootLaser());
            if (HUDManager.Instance) HUDManager.Instance.UpdateCrosshair();
            if (AudioManager.Instance) AudioManager.Instance.PlayOneShot(shootingSounds, shootingVolume);

            currentMagazine--;
            if (HUDManager.Instance) HUDManager.Instance.Ammo = currentMagazine;
            StartCoroutine(RecoilEffect());

            // Enforce fire rate limit
            canShoot = false;
            Invoke(nameof(ResetShot), fireRate);
        }
    }

    private IEnumerator ShootLaser()
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        int layerMask = ~(1 << playerLayer);

        // Cast a ray from the center of the screen forward
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        // If the ray hits something
        if (Physics.Raycast(ray, out hit, shootRange, layerMask))
        {
            IHitable hitObject = hit.collider.GetComponent<IHitable>();

            // Checks if the Collider is NOT an SphereCollider, since the SphereCollider is being use for the sensing range
            if (hitObject != null && hit.collider.GetType() != typeof(SphereCollider))
            {
                hitObject.OnHit();
            }

            if (hit.collider.GetType() == typeof(SphereCollider))
            {
                // If hit the huge SphereCollider, draw the laser forward to the max range
                lineRenderer.SetPosition(0, laserOrigin.position);
                lineRenderer.SetPosition(1, laserOrigin.position + (Camera.main.transform.forward * shootRange));
            }
            else
            {
                // Draw the laser from the origin to the hit point
                lineRenderer.SetPosition(0, laserOrigin.position);
                lineRenderer.SetPosition(1, hit.point);
            }

        }
        else
        {
            // If we don't hit anything, draw the laser forward to the max range
            lineRenderer.SetPosition(0, laserOrigin.position);
            lineRenderer.SetPosition(1, laserOrigin.position + (Camera.main.transform.forward * shootRange));
        }

        lineRenderer.enabled = true;
        yield return new WaitForSeconds(laserDisplayTime);
        lineRenderer.enabled = false;
    }

    private IEnumerator RecoilEffect()
    {
        Vector3 originalPosition = gunTransform.localPosition;
        Quaternion originalRotation = gunTransform.localRotation;

        Vector3 recoilPosition = originalPosition - Vector3.forward * Random.Range(recoilDistance * 0.8f, recoilDistance * 1.2f);
        Quaternion recoilRotation = originalRotation * Quaternion.Euler(Random.Range(randomRecoilRotation.x, randomRecoilRotation.y), Random.Range(randomRecoilRotation.x, randomRecoilRotation.y), 0f);

        float recoilTime = 0;
        while (recoilTime < recoilSpeed)
        {
            recoilTime += Time.deltaTime;

            gunTransform.localPosition = Vector3.Lerp(originalPosition, recoilPosition, recoilTime / recoilSpeed);
            gunTransform.localRotation = Quaternion.Slerp(originalRotation, recoilRotation, recoilTime / recoilSpeed);

            yield return null;
        }

        recoilTime = 0;
        while (recoilTime < recoilSpeed)
        {
            recoilTime += Time.deltaTime;

            gunTransform.localPosition = Vector3.Lerp(recoilPosition, originalPosition, recoilTime / recoilSpeed);
            gunTransform.localRotation = Quaternion.Slerp(recoilRotation, originalRotation, recoilTime / recoilSpeed);

            yield return null;
        }
    }

    private void ResetShot()
    {
        canShoot = true;
    }

    private void Reload()
    {
        if (isReloading || currentMagazine == maxMagazine) return;

        if (inputManager.IsAttackingTwo() || currentMagazine == 0)
        {
            if (AudioManager.Instance) AudioManager.Instance.PlayOneShot(reloadingSounds, reloadingVolume);
            StartCoroutine(ReloadCoroutine());
        }
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        canShoot = false;

        Quaternion downRotation = Quaternion.Euler(gunTransform.localEulerAngles.x + 70f, gunTransform.localEulerAngles.y, gunTransform.localEulerAngles.z);
        Vector3 downPosition = originalPosition + gunTransform.localRotation * new Vector3(0f, -0.2f, -0.5f);

        float reloadProgress = 0;
        while (reloadProgress < reloadTime / 2f)
        {
            reloadProgress += Time.deltaTime;
            gunTransform.localPosition = Vector3.Lerp(originalPosition, downPosition, reloadProgress / (reloadTime / 2f));
            gunTransform.localRotation = Quaternion.Slerp(originalRotation, downRotation, reloadProgress / (reloadTime / 2f));

            yield return null;
        }

        reloadProgress = 0;
        while (reloadProgress < reloadTime / 2f)
        {
            reloadProgress += Time.deltaTime;
            gunTransform.localPosition = Vector3.Lerp(downPosition, originalPosition, reloadProgress / (reloadTime / 2f));
            gunTransform.localRotation = Quaternion.Slerp(downRotation, originalRotation, reloadProgress / (reloadTime / 2f));

            yield return null;
        }

        currentMagazine = maxMagazine;
        if (HUDManager.Instance) HUDManager.Instance.Ammo = currentMagazine;
        isReloading = false;
        canShoot = true;
    }

    private IEnumerator BringGunIntoView()
    {
        Quaternion downRotation = Quaternion.Euler(gunTransform.localEulerAngles.x + 70f, gunTransform.localEulerAngles.y, gunTransform.localEulerAngles.z);
        Vector3 downPosition = originalPosition + gunTransform.localRotation * new Vector3(0f, -0.2f, -0.5f);

        float reloadProgress = 0;
        while (reloadProgress < reloadTime / 2f)
        {
            reloadProgress += Time.deltaTime;
            gunTransform.localPosition = Vector3.Lerp(downPosition, originalPosition, reloadProgress / (reloadTime / 2f));
            gunTransform.localRotation = Quaternion.Slerp(downRotation, originalRotation, reloadProgress / (reloadTime / 2f));

            yield return null;
        }

        canShoot = true;
        isGunEnabled = true;
    }

    void OnDestroy()
    {
        PlayerManager.OnPlayerGunEquip -= EnableGun;
    }
}

