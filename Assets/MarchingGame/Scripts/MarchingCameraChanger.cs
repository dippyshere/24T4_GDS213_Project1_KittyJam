using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

/// <summary>
/// Changes the camera follow offset to create a marching effect
/// </summary>
public class MarchingCameraChanger : MonoBehaviour
{
    [SerializeField, Tooltip("Reference to the cinemachine camera")] private CinemachineCamera _camera;
    [SerializeField, Tooltip("The cinemachine follow reference")] private CinemachineFollow _follow;
    [SerializeField, Tooltip("Follow offsets to change to")] private Vector3[] _positions;
    [SerializeField, Tooltip("Frequency of change")] private float _frequency = 5f;
    [SerializeField, Tooltip("Indexes where the input should be reversed")] private bool[] _reverseIndexes;

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => MarchingPlayer.Instance != null);
        _camera.Follow = MarchingPlayer.Instance.transform;
        _camera.LookAt = MarchingPlayer.Instance.transform;
        while (true)
        {
            for (int i = 0; i < _positions.Length; i++)
            {
                _follow.FollowOffset = _positions[i];
                MarchingPlayer.Instance.ReverseInput = _reverseIndexes[i];
                yield return new WaitForSeconds(_frequency);
            }
        }
    }
}
