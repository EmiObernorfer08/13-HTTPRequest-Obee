using UnityEngine;
using TMPro; // 1. IMPORTANT: Include the TextMeshPro namespace
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System;

public class httpreq : MonoBehaviour
{
    // 2. Public variable for the URL (editable in the Inspector)
    public string targetURL = "https://www.htl-salzburg.ac.at/lehrerinnen-details/meerwald-stadler-susanne-prof-dipl-ing-g-009.html";

    // 3. Public variable to link the TextMeshPro object (editable in the Inspector)
    public TMP_Text outputText; 

    // THE ROBUST REGEX PATTERN: Captures Name, Room, and Office Hour (Time)
    const string REGEX_PATTERN = 
        // Group 1: Name
        @"<div\s+class=""field\s+Lehrername"">.*?<span\s+class=""text"">(.*?)<\/span>" + 
        // Group 2: Room
        @".*?<div\s+class=""field\s+Raum"">.*?<span\s+class=""text"">(.*?)<\/span>" + 
        // Group 3: Office Hour/Time
        @".*?<div\s+class=""field\s+SprStunde"">.*?<span\s+class=""text"">(.*?)<\/span>";

    // Used to send web requests (HttpClient is best reused)
    static readonly HttpClient client = new HttpClient();

    void Start()
    {
        // Safety check to ensure the outputText object is assigned
        if (outputText == null)
        {
            Debug.LogError("outputText (TextMeshPro) is not assigned in the Inspector! Cannot display output.");
            return;
        }
        
        // Start the fetching process
        FetchAndParseDetails();
    }

    async void FetchAndParseDetails()
    {
        try
        {
            // Set temporary loading text
            outputText.text = "Loading data from URL...";

            // 1. Fetch the page content using the public variable
            string htmlContent = await client.GetStringAsync(targetURL);
            
            // 2. Run the pattern against the content
            Match match = Regex.Match(htmlContent, REGEX_PATTERN, RegexOptions.Singleline);

            if (match.Success)
            {
                // Capture the groups
                string name = match.Groups[1].Value.Trim();
                string room = match.Groups[2].Value.Trim();
                string officeHour = match.Groups[3].Value.Trim();

                // 3. Format the results
                string resultText = $"Name: {name}\n" +
                                    $"Room: {room}\n" +
                                    $"Time: {officeHour}";
                
                // 4. Update the TextMeshPro object
                outputText.text = resultText;
                Debug.Log("Extraction successful.");
            }
            else
            {
                // Update the TextMeshPro object with the error
                outputText.text = "Error: Could not parse details using Regex.";
                Debug.LogError("Regex failed to find all three fields (Name, Room, Time).");
            }
        }
        catch (Exception e)
        {
            // Update the TextMeshPro object with the exception error
            outputText.text = $"❌ HTTP Error: Check URL and Network.\n{e.Message}";
            Debug.LogError("An error occurred during network fetch or parsing: " + e.Message);
        }
    }
}