using UnityEngine;
using TMPro; 
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System;

public class httpreq : MonoBehaviour
{
    
    public string targetURL = "https://www.htl-salzburg.ac.at/lehrerinnen-details/meerwald-stadler-susanne-prof-dipl-ing-g-009.html";

    
    public TMP_Text outputText; 

    
    const string REGEX_PATTERN = 
        
        @"<div\s+class=""field\s+Lehrername"">.*?<span\s+class=""text"">(.*?)<\/span>" + 
        
        @".*?<div\s+class=""field\s+Raum"">.*?<span\s+class=""text"">(.*?)<\/span>" + 
        
        @".*?<div\s+class=""field\s+SprStunde"">.*?<span\s+class=""text"">(.*?)<\/span>";

    
    static readonly HttpClient client = new HttpClient();

    void Start()
    {
        
        if (outputText == null)
        {
            Debug.LogError("outputText (TextMeshPro) is not assigned in the Inspector! Cannot display output.");
            return;
        }
        
        
        FetchAndParseDetails();
    }

    async void FetchAndParseDetails()
    {
        try
        {
            
            outputText.text = "Loading data from URL...";

            
            string htmlContent = await client.GetStringAsync(targetURL);
            
            
            Match match = Regex.Match(htmlContent, REGEX_PATTERN, RegexOptions.Singleline);

            if (match.Success)
            {
                
                string name = match.Groups[1].Value.Trim();
                string room = match.Groups[2].Value.Trim();
                string officeHour = match.Groups[3].Value.Trim();

                
                string resultText = $"Name: {name}\n" +
                                    $"Room: {room}\n" +
                                    $"Time: {officeHour}";
                
                
                outputText.text = resultText;
                Debug.Log("Extraction successful.");
            }
            else
            {
                
                outputText.text = "Error: Could not parse details using Regex.";
                Debug.LogError("Regex failed to find all three fields (Name, Room, Time).");
            }
        }
        catch (Exception e)
        {
            
            outputText.text = $"HTTP Error: Check URL and Network.\n{e.Message}";
            Debug.LogError("An error occurred during network fetch or parsing: " + e.Message);
        }
    }
}