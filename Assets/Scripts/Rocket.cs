
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {


    [SerializeField] float mainThrust = 100f;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] List<float> probabilities = new List<float>();
    [SerializeField] float crazyThrustMultiplayer = 50f;
    [SerializeField] float lerpTime = 50f;
    [SerializeField] Vector3 scaleInWormHole = new Vector3(0.05f, 0.05f, 0.05f);
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] Vector3 rotateInWormHole = Vector3.zero;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip rotateEngine;
    [SerializeField] AudioClip DieExplosion;
    [SerializeField] AudioClip LevelWin;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem leftThrustParticles;
    [SerializeField] ParticleSystem rightThrustParticles;
    [SerializeField] ParticleSystem crazyModeParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem successParticles;
    //TODO audio clip fot craze thrust

 
    Rigidbody rigidBody;
    AudioSource audioSource;

    float currentLerpTime = 0f;
    Vector3 colliderPosition ;
    Vector3 collisionPoint ;
    string colliderName ;


    bool isColliderEnabled = true;
    bool isOnBase = true;

    enum State {Alive, Dying, Transcending };
    State state = State.Alive;

    private void Awake()
    {
        
    }

    // Use this for initialization
    void Start ()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
      
       
    }
	
	// Update is called once per frame
	void Update ()
    {       
        ProcessInput();
    }

    private void ProcessInput()
    {
        if (state == State.Alive)
        {
            RespondOnThrustInput();
            RespondOnRotateInput();
            if (Debug.isDebugBuild)
            {
                RespondOnDebugInput();
            }
        }
        if (state == State.Transcending)
        {
            GoingToWormHole();
        }
    }

    private void RespondOnThrustInput()
    {
        // count random multiplayer frpm destribution
        float ThrustMultiplayer = CountThrustMultiplayer();
        if (ThrustMultiplayer == 3) { ThrustMultiplayer = crazyThrustMultiplayer; }
      
        if (Input.GetKey(KeyCode.Space))
        {
           
            ApplyThrust(ThrustMultiplayer);
        }
        else
        {
            StopThrust();
        }

    }

    private void StopThrust()
    {
        if (audioSource.clip == mainEngine)
        {
            audioSource.Stop();
        }
        mainEngineParticles.Stop();
    }

    private float CountThrustMultiplayer()
    {
        float ThrustMultiplayer = RandomFromDistribution.RandomChoiceFollowingDistribution(probabilities);
        ThrustMultiplayer = ThrustMultiplayer + 1;
        return ThrustMultiplayer;
    }

    private void ApplyThrust( float ThrustMultiplayer)
    {
        float thrustThisFrame = mainThrust * Time.deltaTime * ThrustMultiplayer;
        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
        ApplyThrustParticlesAndAudio(ThrustMultiplayer);
    }

    private void ApplyThrustParticlesAndAudio(float ThrustMultiplayer) //TODO Add sound for carzy mod, maybe some pilot voice?
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = mainEngine;
            audioSource.PlayOneShot(mainEngine);
        }
        if (ThrustMultiplayer == crazyThrustMultiplayer)
        {
            GameManager.instance.crazyThrust++;
            mainEngineParticles.Stop();
            crazyModeParticles.Play();
        }
        else
        {
            crazyModeParticles.Stop();
            mainEngineParticles.Play();
        }
    }

    private void RespondOnRotateInput()
    {

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        rigidBody.angularVelocity = Vector3.zero;//remove rotation by physics

        if (Input.GetKey(KeyCode.A) && !isOnBase)
        {
            ApplyRightThrust(rotationThisFrame);
            
        }
        else if (Input.GetKey(KeyCode.D) && !isOnBase)
        {
            ApplyLeftThrust(rotationThisFrame);
           
        }
        else
        {
            StopSideThrusts();

        }

    }

    private void StopSideThrusts()
    {
        rightThrustParticles.Stop();
        leftThrustParticles.Stop();
        if (audioSource.clip == rotateEngine)
        {
            audioSource.Stop();
        }
    }

    private void ApplyRightThrust(float rotationThisFrame)
    {
        transform.Rotate(Vector3.forward * rotationThisFrame);
        if (!audioSource.isPlaying)
        {
            audioSource.clip = rotateEngine;
            audioSource.PlayOneShot(rotateEngine);
        }
        rightThrustParticles.Play();
    }

    private void ApplyLeftThrust(float rotationThisFrame)
    {
        transform.Rotate(-Vector3.forward * rotationThisFrame);
        if (!audioSource.isPlaying)
        {
            audioSource.clip = rotateEngine;
            audioSource.PlayOneShot(rotateEngine);
        }
        leftThrustParticles.Play();
    }

    private void RespondOnDebugInput()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            isColliderEnabled = !isColliderEnabled;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || !isColliderEnabled) { return; } // ignore collision if dead
             
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                isOnBase = true;
             
                break;
            case "Finish":

                SartWinSequence();
                break;
            default:
                StartDeathSequence();
                
                break;
        }
        
        
    }

       private void OnCollisionExit(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                isOnBase = false;
               
                break;
            default:
                break;
        }
    }

    private void SartWinSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        mainEngineParticles.Stop();
        leftThrustParticles.Stop();
        rightThrustParticles.Stop();
        crazyModeParticles.Stop();
        GetComponentInChildren<Light>().intensity = 0;

        //need to find success particles
        successParticles.Play();

        audioSource.PlayOneShot(LevelWin);
        GameObject exitCollider = GameObject.FindGameObjectWithTag("Friendly");
        Destroy(exitCollider.GetComponent<SphereCollider>());
        Destroy(GetComponent<Rigidbody>());
            
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void GoingToWormHole()
    {
        collisionPoint = transform.position;
        GameObject exitObj = GameObject.FindGameObjectWithTag("Finish");
        Vector3 colliderPosition = exitObj.transform.localPosition;

       
        //ship is going to the wormhole center
        currentLerpTime += Time.deltaTime;
        if (currentLerpTime >= lerpTime)
        {
            currentLerpTime = lerpTime;
        }
        float perc = currentLerpTime / lerpTime;
        transform.position = Vector3.Lerp(collisionPoint, colliderPosition, perc);
        transform.Rotate(rotateInWormHole * Time.deltaTime);
        //ship is flying in wormhole so we need to make it smaller
        Vector3 currentScale = transform.localScale;
       
        if (currentScale.x >= Mathf.Epsilon || currentScale.y >= Mathf.Epsilon || currentScale.z >= Mathf.Epsilon)
        {
            transform.localScale -= scaleInWormHole;
        }
    }

    private void StartDeathSequence()
    {
        GameManager.instance.score++;
        state = State.Dying;
        audioSource.Stop();
        mainEngineParticles.Stop();
        leftThrustParticles.Stop();
        rightThrustParticles.Stop();
        crazyModeParticles.Stop();
        deathParticles.Play();
        audioSource.PlayOneShot(DieExplosion);
        Invoke("LevelOnDeath", levelLoadDelay);
    }
    
    private void LevelOnDeath()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        
    }

    private void LoadNextLevel()
    {
        int countScenes = SceneManager.sceneCountInBuildSettings;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentSceneIndex + 1;

        if ( nextIndex == countScenes )
        {
            nextIndex = 0;
        }
        SceneManager.LoadScene(nextIndex);


         
    }
}
