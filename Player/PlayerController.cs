using System;
using System.Collections;
using System.Collections.Generic;
using Network.Crud;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private void FixedUpdate() {
        SendInputToServer();
    }

    private void SendInputToServer() {
        bool[] inputs = new bool[4];
        inputs[0] = Input.GetKey(KeyCode.W);
        inputs[1] = Input.GetKey(KeyCode.S);
        inputs[2] = Input.GetKey(KeyCode.A);
        inputs[3] = Input.GetKey(KeyCode.D);
        bool sendPackage = false;
        for (int i = 0; i < inputs.Length; i++) {
            if (inputs[i])
                sendPackage = true;
        }
        if(sendPackage)
            ClientSend.PlayerMovement(inputs);
    }
}
