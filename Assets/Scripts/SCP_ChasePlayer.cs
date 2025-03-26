using UnityEngine;

public class SCP_ChasePlayerSimple : MonoBehaviour
{
    public float detectionRadius = 30f;           // How far SCP-096 can detect the player
    public float moveSpeed = 10f;                 // Sprinting speed of SCP-096
    public float sprintDistance = 1.5f;           // How close SCP-096 should get before attacking
    public Animator animator;                     // Reference to Animator component
    public AudioSource scpScreamAudio;            // Audio source to play SCP scream sound
    public GameObject gameOverScreen;             // Reference to Game Over UI Screen

    private bool isProvoked = false;
    private Transform player;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (scpScreamAudio == null && GetComponent<AudioSource>() != null)
            scpScreamAudio = GetComponent<AudioSource>();

        // Find the player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);  // Hide the Game Over screen initially
    }

    void Update()
    {
        if (isProvoked && player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > sprintDistance)
            {
                // Face the player while chasing
                Vector3 direction = (player.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

                // Move towards the player
                transform.position += direction * moveSpeed * Time.deltaTime;

                // Trigger Sprint Animation
                animator.SetFloat("Speed", 1.0f);  // Full Sprint Animation
            }
            else
            {
                // When SCP touches the player
                GameOver();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isProvoked) return;

        if (collision.gameObject.CompareTag("Throwable") || 
            collision.gameObject.layer == LayerMask.NameToLayer("Throwable"))
        {
            Debug.Log("SCP-096 has been provoked by a Throwable Object!");

            isProvoked = true;

            if (scpScreamAudio != null)
                scpScreamAudio.Play();

            animator.SetFloat("Speed", 1.0f);  // Start Sprinting
        }
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER: SCP-096 has caught you!");
        animator.SetFloat("Speed", 0.0f);

        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        // Optionally, stop the game or restart
        Time.timeScale = 0f; // Freeze the game
    }
}
