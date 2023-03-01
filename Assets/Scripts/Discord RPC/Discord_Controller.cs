using Discord;
using UnityEngine;

public class Discord_Controller : MonoBehaviour
{
    public long applicationID;
    [Space]
    public string details = "Running around at the speed of sound";
    public string state = "";
    [Space]
    public string largeImage = "icon";
    public string largeText = "Sonic Utopia Decomp";

    private Rigidbody rb;
    private long time;

    private static bool instanceExists;
    public Discord.Discord discord;

    void Awake() 
    {
        // Transition the GameObject between scenes, destroy any duplicates
        if (!instanceExists)
        {
            instanceExists = true;
            DontDestroyOnLoad(gameObject);
        }
        else if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Log in with the Application ID
        discord = new Discord.Discord(applicationID, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);

        rb = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
        time = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();

        UpdateStatus();
    }

    void Update()
    {
        // Destroy the GameObject if Discord isn't running
        try
        {
            discord.RunCallbacks();
        }
        catch
        {
            Destroy(gameObject);
        }
    }

    void LateUpdate() 
    {
        UpdateStatus();
    }

    void UpdateStatus()
    {
        // Update Status every frame
        try
        {
            var activityManager = discord.GetActivityManager();
            var activity = new Discord.Activity
            {
                Details = details,
                State = state,
                Assets = 
                {
                    LargeImage = largeImage,
                    LargeText = largeText
                },
                Timestamps =
                {
                    Start = time
                }
            };

            activityManager.UpdateActivity(activity, (res) =>
            {
                if (res != Discord.Result.Ok) Debug.LogWarning("Failed connecting to Discord!");
            });
        }
        catch
        {
            // If updating the status fails, Destroy the GameObject
            Destroy(gameObject);
        }
    }
}
