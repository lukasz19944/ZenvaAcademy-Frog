using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public delegate void PlayerHandler();
    public event PlayerHandler OnPlayerMoved;
    public event PlayerHandler OnPlayerEscaped;

    public float jumpDistance = 0.32f;

    private bool jumped = false;
    private Vector3 startingPosition;

	// Use this for initialization
	void Start () {
        startingPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        // movement
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        if (!jumped) {
            Vector2 targetPosition = Vector2.zero;
            bool tryingToMove = false;

            if (horizontalMovement != 0) {
                tryingToMove = true;
                targetPosition = new Vector2(
                    transform.position.x + (horizontalMovement > 0 ? jumpDistance : -jumpDistance), 
                    transform.position.y
                );
            }

            if (verticalMovement != 0) {
                tryingToMove = true;
                targetPosition = new Vector2(
                   transform.position.x,
                   transform.position.y + (verticalMovement > 0 ? jumpDistance : -jumpDistance)
               );
            }

            // check if player can move
            Collider2D hitCollider = Physics2D.OverlapCircle(targetPosition, 0.1f);

            if (tryingToMove == true && (hitCollider == null || hitCollider.GetComponent<EnemyController>() != null)) {
                transform.position = targetPosition;

                jumped = true;

                GetComponent<AudioSource>().Play();

                if (OnPlayerMoved != null) {        // check if anybody wants to know if the player has moved
                    OnPlayerMoved();
                }
            }
        } else {
            if (horizontalMovement == 0 && verticalMovement == 0) {
                jumped = false;
            }
        }

        // keep the frog inside bounds
        if (transform.position.y < -(Screen.height / 100f) / 2f) {
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y + jumpDistance
            );
        }

        if (transform.position.y > (Screen.height / 100f) / 2f) {
            transform.position = startingPosition;

            if (OnPlayerEscaped != null) {
                OnPlayerEscaped();
            }
        }

        if (transform.position.x < -(Screen.width / 100f) / 2f) {
            transform.position = new Vector3(
                transform.position.x + jumpDistance,
                transform.position.y
            );
        }

        if (transform.position.x > (Screen.width / 100f) / 2f) {
            transform.position = new Vector3(
                transform.position.x - jumpDistance,
                transform.position.y
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<EnemyController>() != null) {
            Destroy(gameObject);
        }
    }
}
