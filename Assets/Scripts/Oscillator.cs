using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour 
{
    [SerializeField] Vector3 movementVector = new Vector3(10f, 0f, 0f);
    [SerializeField] float period = 2f; // it's time to complete one full cycle, we default it to 2 seconds 
    // todo remove from inspector later
    [Range(0, 1)][SerializeField] float movementFactor; // 0 for not moved, 1 for fully moved 

    Vector3 startingPos;
     
	// Use this for initialization
	void Start () 
	{
        // we're getting obstacle's transform 
        startingPos = transform.position;
	} 
	
	// Update is called once per frame
	void Update () 
	{
        if(period == 0)
        {
            period = 2f;
        }

        float cycles = Time.time / period; // grows continually from 0, if the game time is 10 we devide it by 2 and it's 5 cycles, because it's based on time we receive an ongoing loop  

        const float tau = Mathf.PI * 2f; // about 6.28
        float rawSinWave = Mathf.Sin(cycles * tau);
        print("cycles: " + cycles);

        print("rawSinWave " + rawSinWave);

        movementFactor = rawSinWave / 2f + 0.5f;

        print("movementFactor: " + movementFactor);

        Vector3 offset = movementVector * movementFactor;
        print("offset " + offset);
        transform.position = startingPos + offset;
    }
}
 