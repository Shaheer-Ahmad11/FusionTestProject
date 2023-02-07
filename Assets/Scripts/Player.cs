using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private NetworkCharacterControllerPrototype _cc;
    [SerializeField] private Ball _prefabBall;
    [SerializeField] private GameObject _myCamera;
    private Vector3 _forward;
    [Networked] private TickTimer delay { get; set; }
    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterControllerPrototype>();
        _forward = transform.forward;
    }
    private void Start()
    {
        if (Object.HasInputAuthority)
        {
            _myCamera.SetActive(true);
        }
    }
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(5 * data.direction * Runner.DeltaTime);
        }
        if (data.direction.sqrMagnitude > 0)
        {
            _forward = data.direction;
        }
        if (delay.ExpiredOrNotRunning(Runner))
        {
            if ((data.buttons & NetworkInputData.MOUSEBUTTON1) != 0)
            {
                delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                Runner.Spawn(_prefabBall,
                transform.position + _forward, Quaternion.LookRotation(_forward),
                Object.InputAuthority, (runner, o) =>
                {
                    // Initialize the Ball before synchronizing it
                    o.GetComponent<Ball>().Init();
                });
            }
        }
    }
}