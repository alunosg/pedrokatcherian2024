using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 8f;
    public float gravity = -9.81f;
    public bool chaveUm = false;


    Vector3 velocity;

    public Transform GroundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    bool isGrounded;

    public int hp = 3;
    public float stunDuration = 0.2f;
    public float deathDuration = 5f;
    public GameObject hitParticle;

    private bool locked = false;
    private bool dead = false;
        void Update()
        {
           if(!locked)
           {
            isGrounded = Physics.CheckSphere(GroundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }


        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical"); 


        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);



        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
           }
    }

    public void GetHit(int damage,Vector3 particlePos)
    {
        if (!dead)
        {
            locked = true;
            hp -= damage;
            Instantiate(hitParticle, particlePos, Quaternion.identity);
            CancelInvoke("DealDamage");
            CancelInvoke("Unlock");

            if (hp <= 0)
            {
                //rig.constraints = RigidBodyConstraints.None;
                //rig.AddTorque(new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random,Range(-10, 10)));
                dead = true;
                Invoke("Reload", deathDuration);
            }
            else
            {
                Invoke("Unlock", stunDuration);
            }
        }
    }

    void Unlock()
    {
        locked = false;
    }

    void Reload()
    {
        SceneManager.LoadScene(0);
    }
}