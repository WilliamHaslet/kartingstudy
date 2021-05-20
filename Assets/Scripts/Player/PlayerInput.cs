using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    private CartMovement cartMovement;
    private BoostManager boostManager;

    private void Start()
    {

        //Cursor.lockState = CursorLockMode.Locked;

        cartMovement = GetComponent<CartMovement>();

        boostManager = GetComponent<BoostManager>();

    }

    private void Update()
    {

        Vector2 moveInput = Vector2.zero;

        if (Input.GetKey(KeyCode.S))
        {

            moveInput.y++;

        }
        
        if (Input.GetKey(KeyCode.W))
        {

            moveInput.y--;

        }
        
        if (Input.GetKey(KeyCode.A))
        {

            moveInput.x--;

        }
        
        if (Input.GetKey(KeyCode.D))
        {

            moveInput.x++;

        }

        if (moveInput == Vector2.zero)
        {

            float y = 0;

            if (Input.GetKey(KeyCode.JoystickButton2))
            {

                y++;

            }
            
            if (Input.GetKey(KeyCode.JoystickButton0))
            {

                y--;

            }

            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), y);

        }

        cartMovement.Move(moveInput);

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton7))
        {

            cartMovement.Drift(true);

        }
        else
        {

            cartMovement.Drift(false);

        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton3))
        {

            boostManager.Boost();

        }

        if (Input.GetKeyDown(KeyCode.Q))
        {

            cartMovement.SetCanMove(false);

        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {

            cartMovement.SetCanMove(true);

        }

    }

}
