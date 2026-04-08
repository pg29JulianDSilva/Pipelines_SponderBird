using UnityEngine;
using NUnit.Framework;

[TestFixture]
public class FirebaseManagerTests 
{
    private GameObject firebaseObj;
    private FirebaseManager firebaseManager;

    //It is the start elements for the condition in stuff
    [SetUp]
    public void SetUp()
    {
        firebaseObj = new GameObject("TestFirebaseManager");
        firebaseManager = firebaseObj.AddComponent<FirebaseManager>();
    }

    //This is code for the clean up system
    [TearDown]
    public void TearDown()
    {
        //This is more for the editor that for the scene itself
        Object.DestroyImmediate(firebaseObj);

        ResetSingleton<FirebaseManager>("Instance");
    }

    //Reflection it is a "const" function that basically reflects the data inside the editor to do changes in iself, useful to manage elements like singletons

    private void ResetSingleton<T>(string propertyName) where T : class
    {
        //Here are we using bitwise contrast elements to accept both Public and Static flags
        var prop = typeof(T).GetProperty(propertyName,
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

        //Here we remove the flags to avoid problems with this one
        prop?.SetValue(null, null);
    }

    [Test]
    public void InitialState_IsNotAuthenticated()
    {
        Assert.IsFalse(firebaseManager.IsAuthenticated);
    }

    [Test]
    public void InitialDisplayName_IsPlayer()
    {
        Assert.AreEqual("Player", firebaseManager.DisplayName);
    }

    [Test]
    public void InitialUserId_IsEmpty() 
    {
        Assert.AreEqual("", firebaseManager.UserId);
    }

    [Test]
    public void InitialdToken_IsEmpty()
    {
        Assert.AreEqual("", firebaseManager.IdToken);
    }

    [Test]
    public void InitialProjectId_IsEmpty()
    {
        Assert.AreEqual("", firebaseManager.ProjectId);
    }

    [Test]
    public void OnAuthReceived_ValidPayload_SetsAuthenticated()
    {
        string json = BuildAuthJson("1234","token_1234","Sponder","beetle-ball");
        firebaseManager.OnAuthReceived(json);
        Assert.AreEqual("1234", firebaseManager.UserId);
    }

    [Test]
    public void OnAuthReceived_SetsDisplayName()
    {
        string json = BuildAuthJson("1234","token_1234","Sponder","beetle-ball");
        firebaseManager.OnAuthReceived(json);
        Assert.AreEqual("Sponder", firebaseManager.DisplayName);
    }

    [Test]
    public void OnAuthReceived_SetsIdToken()
    {
        string json = BuildAuthJson("1234","token_1234","Sponder","beetle-ball");
        firebaseManager.OnAuthReceived(json);
        Assert.AreEqual("token_1234", firebaseManager.IdToken);
    }

    [Test]
    public void OnAuthReceived_SetsProjectId()
    {
        string json = BuildAuthJson("1234","token_1234","Sponder","beetle-ball");
        firebaseManager.OnAuthReceived(json);
        Assert.AreEqual("beetle-ball", firebaseManager.ProjectId);
    }

    private string BuildAuthJson(string uid, string idToken, string displayName, string projectId)
    {
        return $"{{\"uid\":\"{uid}\",\"idToken\":\"{idToken}\",\"displayName\":\"{displayName}\",\"projectId\":\"{projectId}\"}}";
    }

}
