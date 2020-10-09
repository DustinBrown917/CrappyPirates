
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityReceiver : MonoBehaviour
{

    private Rigidbody body_ = null;
    public Rigidbody Body { get => body_; }

    [SerializeField] private float gravityInfluenceFactor_ = 1.0f;
    public float GravityInfluenceFactor { get => gravityInfluenceFactor_; }

    private void Awake()
    {
        body_ = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        RegisterWithGravityManager();
    }

    private void OnDestroy()
    {
        DeregisterWithGravityManager();
    }

    public void RegisterWithGravityManager()
    {
        GravityManager.AddGravityReceiver(this);
    }

    public void DeregisterWithGravityManager()
    {
        GravityManager.RemoveGravityReceiver(this);
    }
}
