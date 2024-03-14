using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MarchingCameraChanger : MonoBehaviour
{
    [SerializeField, Tooltip("Reference to the cinemachine camera")] private CinemachineVirtualCamera _camera;
    [SerializeField, Tooltip("Follow offsets to change to")] private Vector3[] _positions;
    [SerializeField, Tooltip("Frequency of change")] private float _frequency = 5f;
    [SerializeField, Tooltip("Reference to the player controller")] private MarchingPlayer _playerController;
    [SerializeField, Tooltip("Indexes where the input should be reversed")] private bool[] _reverseIndexes;
    [Tooltip("The cinemachine transposer reference")] private CinemachineTransposer _transposer;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        _transposer = _camera.GetCinemachineComponent<CinemachineTransposer>();
        while (true)
        {
            for (int i = 0; i < _positions.Length; i++)
            {
                _transposer.m_FollowOffset = _positions[i];
                _playerController.ReverseInput = _reverseIndexes[i];
                yield return new WaitForSeconds(_frequency);
            }
        }
    }
}