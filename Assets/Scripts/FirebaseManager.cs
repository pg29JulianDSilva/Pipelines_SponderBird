using UnityEngine;
using System.Runtime.InteropServices; //This one will allow us to comunicate with different systems outside the runtime

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }

    public bool IsAuthenticated { get; private set; } = false;
    public string UserId { get; private set; } = "";
    public string DisplayName { get; private set; } = "Player";
    public string IdToken { get; private set; } = "";
    public string ProjectId { get; private set; } = "";

    //This one will check that the game it is running in the WebGL and not in the editor (and the else is for the other way arround)
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] private static extern void InitFirebaseBridge();
    [DllImport("__Internal")] private static extern void SubmitScoreToFirestore(string jsonBody);

#else

    private static void InitFirebaseBridge()
        => Debug.Log("InitFirebaseBridge Stub");

    private static void SubmitScoreToFirestore(string jsonBody)
        => Debug.Log("SubmitScoreToFirestore Stub");
#endif

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitFirebaseBridge();
    }

    public void OnAuthReceived(string json)
    {
        Debug.Log($"Auth Received: {json}");

        var data = JsonUtility.FromJson<AuthPayLoad>(json);
        UserId = data.idToken;
        DisplayName = data.displayName;
        ProjectId = data.projectId;
        IsAuthenticated = !string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(data.idToken);

        Debug.Log($"User Authenticated as {DisplayName}, UID: {UserId}");
    }

    public void SubmitScore(int score, int pipes, int duration)
    {
        if (!IsAuthenticated)
        {
            Debug.Log("Not authenticated. Score not submitted");
            return;
        }

        var payload = new ScorePyaload
        {
            socre = score,
            pipes = pipes,
            duration = duration
        };

        string json = JsonUtility.ToJson(payload);
        SubmitScoreToFirestore(json);
    }

    [System.Serializable]
    private class AuthPayLoad
    {
        public string uid;
        public string idToken;
        public string displayName;
        public string projectId;
    }

    [System.Serializable]
    private class ScorePyaload
    {
        public int socre;
        public int pipes;
        public int duration;
    }
}
