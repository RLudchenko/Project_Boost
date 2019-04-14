using UnityEngine;
using UnityEngine.SceneManagement;

public class SpaceShip : MonoBehaviour 
{
    // rcs reaction control system
    // using SerializedField so that we can change it from the inspector but not from other scritps 
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip deathSound;

    [SerializeField] ParticleSystem mainEngineParicles; 
    [SerializeField] ParticleSystem winParticles;
    [SerializeField] ParticleSystem deathParicles;

    AudioSource audioSource;
    bool isPlaying = false;

    enum State { Alive, Dying, Transcending };
    State state = State.Alive;

    public bool collisionsDisabled = false;

    Rigidbody rigidBody;

	// Use this for initialization
	void Start () 
	{
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update ()
    {
        if(state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
            Back();
        }

        if(Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }

    private void RespondToDebugKeys()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if(Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled; // if it's true it goes to false and vise-versa 
        }
    }

    private void RespondToThrustInput()
    {
        // TODO apply it in the next session of the lesson 
        //float movingUp = mainThrust * Time.deltaTime;

        // we use relative force because it applies force to the local object, if the ship turned, the force will be applied correctly not to the world itself 
        if (Input.GetKey(KeyCode.Space)) // can thrust while rotating
        {
            ApplyThrust();
        }

        else
        { 
            audioSource.Stop();
            mainEngineParicles.Stop();
            //FadeOut();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive || collisionsDisabled)
        {
            return; // it means that it leaves the method aka stop execution at this point  
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly": 
                // do nothing 
                break;

            case "Finish":
                Success();
                break;

            default:
                Death();
                break;
        }
    }

    private void Success()
    {
        audioSource.PlayOneShot(winSound);
        winParticles.Play();
        state = State.Transcending;
        Invoke("LoadNextLevel", levelLoadDelay); // parameterise time 
    }

    private void Death()
    {
        // kills player 
        state = State.Dying;
        audioSource.Stop();
        deathParicles.Play();
        audioSource.PlayOneShot(deathSound);
        mainThrust = 0;
        Invoke("RestartLevel", levelLoadDelay); // parameterise time 
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    } 

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            // this how we deal with multiple audio clips 
            audioSource.PlayOneShot(mainEngine);
            //isPlaying = true;
        }
        mainEngineParicles.Play();
    }

    // allows player to move backwards 
    private void Back()
    {
        if(Input.GetKey(KeyCode.S))
        {
            rigidBody.AddRelativeForce(Vector3.down * mainThrust * Time.deltaTime);
        }
    }

    private void RespondToRotateInput()
    {

        rigidBody.freezeRotation = true; // take manual control of rotation 
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }

        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; // resume physics control of rotation 
    }
}
 