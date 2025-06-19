using Newtonsoft.Json;

public class Program
{
    public static async Task Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = await getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = await getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static async Task<int> getTotalScoredGoals(string team, int year)
    {
        int totalGoals = 0;
        int page = 1;
        int totalPages = 1;

        using (HttpClient client = new HttpClient())
        {
            while (page <= totalPages)
            {
                string url1 = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&team1={team}&page={page}";
                string url2 = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&team2={team}&page={page}";

                // Get matches where the team is team1
                HttpResponseMessage response1 = await client.GetAsync(url1);
                response1.EnsureSuccessStatusCode();
                string json1 = await response1.Content.ReadAsStringAsync();
                dynamic data1 = JsonConvert.DeserializeObject(json1);

                if (data1.total_pages != null)
                {
                    totalPages = data1.total_pages;
                }

                foreach (var match in data1.data)
                {
                    totalGoals += int.Parse(match.team1goals.ToString());
                }

                // Get matches where the team is team2
                HttpResponseMessage response2 = await client.GetAsync(url2);
                response2.EnsureSuccessStatusCode();
                string json2 = await response2.Content.ReadAsStringAsync();
                dynamic data2 = JsonConvert.DeserializeObject(json2);

                if (data2.total_pages != null)
                {
                    totalPages = data2.total_pages;
                }

                foreach (var match in data2.data)
                {
                    totalGoals += int.Parse(match.team2goals.ToString());
                }

                page++;
            }
        }
        return totalGoals;
    }
}