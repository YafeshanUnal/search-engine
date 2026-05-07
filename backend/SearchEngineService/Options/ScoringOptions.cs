namespace SearchEngineService.Options;

public class ScoringOptions
{
    public double VideoTypeFactor { get; set; } = 1.5;
    public double TextTypeFactor { get; set; } = 1.0;

    public double VideoBaseViewsDivisor { get; set; } = 1000;
    public double VideoBaseLikesDivisor { get; set; } = 100;
    public double TextBaseReactionsDivisor { get; set; } = 50;

    public int RecencyWeekDays { get; set; } = 7;
    public int RecencyMonthDays { get; set; } = 30;
    public int RecencyQuarterDays { get; set; } = 90;
    public double RecencyWeekScore { get; set; } = 5;
    public double RecencyMonthScore { get; set; } = 3;
    public double RecencyQuarterScore { get; set; } = 1;
    public double RecencyOldScore { get; set; } = 0;

    public double VideoInteractionMultiplier { get; set; } = 10;
    public double TextInteractionMultiplier { get; set; } = 5;

    public double TextPopularityReadingTimeWeight { get; set; } = 2;
}
