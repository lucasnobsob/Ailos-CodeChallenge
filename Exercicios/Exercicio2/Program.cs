using Newtonsoft.Json.Linq;
using System.Net.Http;

public class Program
{
    public static async Task Main()
    {
        await ShowTeamGoalsAsync("Paris Saint-Germain", 2013);
        await ShowTeamGoalsAsync("Chelsea", 2014);
    }

    private static async Task ShowTeamGoalsAsync(string teamName, int year)
    {
        int totalGoals = await GetTotalScoredGoalsAsync(teamName, year);
        Console.WriteLine($"Team {teamName} scored {totalGoals} goals in {year}");
    }

    private static async Task<int> GetTotalScoredGoalsAsync(string team, int year)
    {
        int totalGoals = 0;
        int currentPage = 1;
        int totalPages;

        using var client = new HttpClient();

        do
        {
            var team1Result = await GetGoalsForPageAsync(client, team, year, currentPage, isTeam1: true);
            var team2Result = await GetGoalsForPageAsync(client, team, year, currentPage, isTeam1: false);

            totalGoals += team1Result.Goals + team2Result.Goals;
            totalPages = Math.Max(team1Result.TotalPages, team2Result.TotalPages);

            currentPage++;
        } while (currentPage <= totalPages);

        return totalGoals;
    }

    private static async Task<MatchResult> GetGoalsForPageAsync(HttpClient client, string team, int year, int page, bool isTeam1)
    {
        string teamParam = isTeam1 ? "team1" : "team2";
        string goalsKey = isTeam1 ? "team1goals" : "team2goals";

        string url = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&{teamParam}={team}&page={page}";
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();
        JObject json = JObject.Parse(content);

        int totalPages = (int)json["total_pages"];
        int goals = json["data"].Sum(match => (int)match[goalsKey]);

        return new MatchResult(goals, totalPages);
    }

    private record MatchResult(int Goals, int TotalPages);
}
