namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System.Text.RegularExpressions;

public static class StringExtensions
{
    private static readonly Regex LikeClauseSpecialCharacters = new Regex($@"[_%\[\]]");

    public static string AsSqlBracketedValue(this string value)
    {
        if (value.StartsWith('[') && value.EndsWith(']'))
        {
            return value;
        }

        return $"[{value.EscapeForBracketedValue()}]";
    }

    public static string EscapeForBracketedValue(this string value) => value.Replace(@"]", @"]]");

    public static string EscapeForSingleQuotedValue(this string value) => value.Replace(@"'", @"\'");

    public static string EscapeForLikeClause(this string value) => LikeClauseSpecialCharacters.Replace(value, match => $"[{match.Value}]");
}
