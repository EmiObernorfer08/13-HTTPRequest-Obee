using UnityEngine;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
 
public class LehrerScraper : MonoBehaviour
{
    private string LIST_URL = "https://www.htl-salzburg.ac.at/lehrerinnen.html";
    private string BASE_URL = "https://www.htl-salzburg.ac.at";
 
    private void Start()
    {
        LadeLehrer();
    }
 
    async void LadeLehrer()
    {
        string listHtml = await LadeSeite(LIST_URL);
        if (listHtml == "Seite nicht gefunden.")
        {
            Debug.LogError("Lehrerübersicht konnte nicht geladen werden.");
            return;
        }
 
        // Alle Links zu Lehrerprofilen finden
        MatchCollection links = Regex.Matches(
            listHtml,
            "href=\"(\\/lehrerinnen-details\\/[^\\\"]+\\.html)\""
        );
 
        int menge = Mathf.Min(5, links.Count);
 
        for (int i = 0; i < menge; i++)
        {
            string url = BASE_URL + links[i].Groups[1].Value;
            string html = await LadeSeite(url);
 
            string name = ExtrahiereName(html);
            string raum = ExtrahiereRaum(html);
            string stunde = ExtrahiereSprechstunde(html);
 
            AusgabeLehrer(i + 1, name, raum, stunde);
        }
    }
 
    // -------------------------------------------------------------------
    // ------------------------- EINZELFUNKTIONEN ------------------------
    // -------------------------------------------------------------------
 
    string ExtrahiereName(string html)
    {
        string regex = @"<h1 class=""value"">\s*<span class=""text"">(.*?)<\/span>";
        return Regex.Match(html, regex, RegexOptions.Singleline).Groups[1].Value.Trim();
    }
 
    string ExtrahiereRaum(string html)
    {
        string regex = @"<div class=""field Raum"">.*?<span class=""text"">(.*?)<\/span>";
        return Regex.Match(html, regex, RegexOptions.Singleline).Groups[1].Value.Trim();
    }
 
    string ExtrahiereSprechstunde(string html)
    {
        string regex = @"<div class=""field SprStunde"">.*?<span class=""text"">(.*?)<\/span>";
        return Regex.Match(html, regex, RegexOptions.Singleline).Groups[1].Value.Trim();
    }
 
    void AusgabeLehrer(int index, string name, string raum, string stunde)
    {
        Debug.Log(
            $"Lehrer {index}:\n" +
            $"Name: {name}\n" +
            $"Raum: {raum}\n" +
            $"Sprechstunde: {stunde}\n" +
            $"-----------------------------"
        );
    }
 
    async Task<string> LadeSeite(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch
            {
                Debug.LogError("Fehler beim Laden von: " + url);
                return "Seite nicht gefunden.";
            }
        }
    }
}